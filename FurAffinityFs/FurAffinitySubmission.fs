namespace FurAffinityFs

open System
open System.Text
open System.IO
open System.Net
open FSharp.Data

module FurAffinitySubmission =
    let private BaseUri =
        new Uri("https://www.furaffinity.net/")

    let private ToUri (path: string) =
        new Uri(BaseUri, path)

    let private ExtractAuthenticityToken (html: HtmlDocument) =
        let m =
            html.CssSelect("form[name=myform] input[name=key]")
            |> Seq.map (fun e -> e.AttributeValue("value"))
            |> Seq.tryHead
        match m with
            | Some token -> token
            | None -> failwith "Form \"myform\" with hidden input \"key\" not found in HTML from server"

    let private GetCookiesFor (credentials: IFurAffinityCredentials) =
        let c = new CookieContainer()
        c.Add(BaseUri, new Cookie("a", credentials.A))
        c.Add(BaseUri, new Cookie("b", credentials.B))
        c

    let UserAgent =
        "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:78.0) Gecko/20100101 Firefox/78.0"

    let private CreateRequest (credentials: IFurAffinityCredentials) (uri: Uri) =
        WebRequest.CreateHttp(uri, UserAgent = UserAgent, CookieContainer = GetCookiesFor credentials)

    type Artwork = {
        data: byte[]
        contentType: string
        title: string
        message: string
        keywords: string seq
        cat: FurAffinityCategory
        scrap: bool
        atype: FurAffinityType
        species: FurAffinitySpecies
        gender: FurAffinityGender
        rating: FurAffinityRating
        lock_comments: bool
    }

    let AsyncPostArtwork (credentials: IFurAffinityCredentials) (submission: Artwork) = async {
        // multipart separators
        let boundary = sprintf "-----------------------------%d" DateTime.UtcNow.Ticks
        let interior_boundary = sprintf "--%s" boundary
        let final_boundary = sprintf "--%s--" boundary

        let ext = Seq.last (submission.contentType.Split('/'))
        let filename = sprintf "file.%s" ext

        let initial_submission_page_req = "/submit/" |> ToUri |> CreateRequest credentials
        initial_submission_page_req.Method <- "POST"
        initial_submission_page_req.ContentType <- "application/x-www-form-urlencoded"

        do! async {
            use! reqStream = initial_submission_page_req.GetRequestStreamAsync() |> Async.AwaitTask
            use sw = new StreamWriter(reqStream)
            do! "part=2&submission_type=submission" |> sw.WriteLineAsync |> Async.AwaitTask
        }
        
        let! (artwork_submission_page_key, artwork_submission_page_url) = async {
            use! resp = initial_submission_page_req.AsyncGetResponse()
            use sr = new StreamReader(resp.GetResponseStream())
            let! html = sr.ReadToEndAsync() |> Async.AwaitTask
            let token = html |> HtmlDocument.Parse |> ExtractAuthenticityToken
            return (token, resp.ResponseUri)
        }

        let artwork_submission_page_req = CreateRequest credentials artwork_submission_page_url
        artwork_submission_page_req.Method <- "POST"
        artwork_submission_page_req.ContentType <- sprintf "multipart/form-data; boundary=%s" boundary

        do! async {
            use memory_buffer = new MemoryStream()
            let w (s: string) =
                let bytes = Encoding.UTF8.GetBytes(sprintf "%s\n" s)
                memory_buffer.Write(bytes, 0, bytes.Length)
            
            w interior_boundary
            w "Content-Disposition: form-data; name=\"part\""
            w ""
            w "3"
            w interior_boundary
            w "Content-Disposition: form-data; name=\"key\""
            w ""
            w artwork_submission_page_key
            w interior_boundary
            w "Content-Disposition: form-data; name=\"submission_type\""
            w ""
            w "submission"
            w interior_boundary
            w (sprintf "Content-Disposition: form-data; name=\"submission\"; filename=\"%s\"" filename)
            w (sprintf "Content-Type: %s" submission.contentType)
            w ""
            memory_buffer.Flush()
            memory_buffer.Write(submission.data, 0, submission.data.Length)
            memory_buffer.Flush()
            w ""
            w interior_boundary
            w "Content-Disposition: form-data; name=\"thumbnail\"; filename=\"\""
            w "Content-Type: application/octet-stream"
            w ""
            w ""
            w final_boundary

            use! reqStream = artwork_submission_page_req.GetRequestStreamAsync() |> Async.AwaitTask
            memory_buffer.Position <- 0L
            do! memory_buffer.CopyToAsync reqStream |> Async.AwaitTask
        }

        let! (finalize_submission_page_key, finalize_submission_page_url) = async {
            use! resp = artwork_submission_page_req.AsyncGetResponse()
            use sr = new StreamReader(resp.GetResponseStream())
            let! html = sr.ReadToEndAsync() |> Async.AwaitTask
            if html.Contains "Security code missing or invalid." then
                failwithf "Security code missing or invalid for page %s" resp.ResponseUri.AbsoluteUri
            let token = html |> HtmlDocument.Parse |> ExtractAuthenticityToken
            return (token, resp.ResponseUri)
        }

        let finalize_submission_page_req = CreateRequest credentials finalize_submission_page_url
        finalize_submission_page_req.Method <- "POST"
        finalize_submission_page_req.ContentType <- sprintf "multipart/form-data; boundary=%s" boundary

        do! async {
            use ms = new MemoryStream()
            let w (s: string) =
                let bytes = Encoding.UTF8.GetBytes(sprintf "%s\n" s)
                ms.Write(bytes, 0, bytes.Length)

            w interior_boundary
            w "Content-Disposition: form-data; name=\"part\""
            w ""
            w "5"
            w interior_boundary
            w "Content-Disposition: form-data; name=\"key\""
            w ""
            w finalize_submission_page_key
            w interior_boundary
            w "Content-Disposition: form-data; name=\"submission_type\""
            w ""
            w "submission"
            w interior_boundary
            w "Content-Disposition: form-data; name=\"cat_duplicate\""
            w ""
            w ""
            w interior_boundary
            w "Content-Disposition: form-data; name=\"title\""
            w ""
            w submission.title
            w interior_boundary
            w "Content-Disposition: form-data; name=\"message\""
            w ""
            w submission.message
            w interior_boundary
            w "Content-Disposition: form-data; name=\"keywords\""
            w ""
            w (submission.keywords |> Seq.map (fun s -> s.Replace(' ', '_')) |> String.concat " ")
            w interior_boundary
            w "Content-Disposition: form-data; name=\"cat\""
            w ""
            w (submission.cat.ToString("d"))
            w interior_boundary
            w "Content-Disposition: form-data; name=\"atype\""
            w ""
            w (submission.atype.ToString("d"))
            w interior_boundary
            w "Content-Disposition: form-data; name=\"species\""
            w ""
            w (submission.species.ToString("d"))
            w interior_boundary
            w "Content-Disposition: form-data; name=\"gender\""
            w ""
            w (submission.gender.ToString("d"))
            w interior_boundary
            w "Content-Disposition: form-data; name=\"rating\""
            w ""
            w (submission.rating.ToString("d"))
            w interior_boundary
            w "Content-Disposition: form-data; name=\"create_folder_name\""
            w ""
            w ""
            if submission.scrap then
                w interior_boundary
                w "Content-Disposition: form-data; name=\"scrap\""
                w ""
                w "1"
            if submission.lock_comments then
                w interior_boundary
                w "Content-Disposition: form-data; name=\"lock_comments\""
                w ""
                w "on"
            w final_boundary

            use! reqStream = finalize_submission_page_req.GetRequestStreamAsync() |> Async.AwaitTask
            ms.Position <- 0L
            do! ms.CopyToAsync(reqStream) |> Async.AwaitTask
        }

        return! async {
            use! resp = finalize_submission_page_req.AsyncGetResponse()
            use sr = new StreamReader(resp.GetResponseStream())
            let! html = sr.ReadToEndAsync() |> Async.AwaitTask
            if html.Contains "Security code missing or invalid." then
                failwithf "Security code missing or invalid for page %s" resp.ResponseUri.AbsoluteUri
            return resp.ResponseUri
        }
    }

    let PostArtworkAsync credentials submission =
        AsyncPostArtwork credentials submission
        |> Async.StartAsTask