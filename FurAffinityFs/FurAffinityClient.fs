namespace FurAffinityFs

open System
open System.Net
open System.IO
open System.Text.RegularExpressions
open System.Text

type FurAffinityClientException(message: string) =
    inherit ApplicationException(message)

type FurAffinityClient(a: string, b: string) =
    let get_authenticity_token html =
        let m = Regex.Match(html, """<input type="hidden" name="key" value="([^"]+)".""")

        if m.Success
            then m.Groups.[1].Value
            else raise (FurAffinityClientException "Input \"key\" not found in HTML")

    let cookies =
        let c = new CookieContainer()
        c.Add(new Uri("https://www.furaffinity.net"), new Cookie("a", a))
        c.Add(new Uri("https://www.furaffinity.net"), new Cookie("b", b))
        c

    let createRequest (url: Uri) =
        WebRequest.CreateHttp(url, UserAgent = "FurAffinityFs/0.1 (https://github.com/libertyernie)", CookieContainer = cookies)

    member __.AsyncSubmitPost (submission: FurAffinitySubmission) = async {
        let ext = Seq.last (submission.contentType.Split('/'))
        let filename = sprintf "file.%s" ext

        let req1 = createRequest <| new Uri("https://www.furaffinity.net/submit/")
        req1.Method <- "POST"
        req1.ContentType <- "application/x-www-form-urlencoded"

        do! async {
            use! reqStream = req1.GetRequestStreamAsync() |> Async.AwaitTask
            use sw = new StreamWriter(reqStream)
            do! "part=2&submission_type=submission" |> sw.WriteLineAsync |> Async.AwaitTask
        }
        
        let! (key1, url1) = async {
            use! resp = req1.AsyncGetResponse()
            use sr = new StreamReader(resp.GetResponseStream())
            let! html = sr.ReadToEndAsync() |> Async.AwaitTask
            return (get_authenticity_token html, resp.ResponseUri)
        }

        // multipart separators
        let h1 = sprintf "-----------------------------%d" DateTime.UtcNow.Ticks
        let h2 = sprintf "--%s" h1
        let h3 = sprintf "--%s--" h1

        let req2 = createRequest url1
        req2.Method <- "POST"
        req2.ContentType <- sprintf "multipart/form-data; boundary=%s" h1

        do! async {
            use ms = new MemoryStream()
            let w (s: string) =
                let bytes = Encoding.UTF8.GetBytes(sprintf "%s\n" s)
                ms.Write(bytes, 0, bytes.Length)
            
            w h2
            w "Content-Disposition: form-data; name=\"part\""
            w ""
            w "3"
            w h2
            w "Content-Disposition: form-data; name=\"key\""
            w ""
            w key1
            w h2
            w "Content-Disposition: form-data; name=\"submission_type\""
            w ""
            w "submission"
            w h2
            w (sprintf "Content-Disposition: form-data; name=\"submission\"; filename=\"%s\"" filename)
            w (sprintf "Content-Type: %s" submission.contentType)
            w ""
            ms.Flush()
            ms.Write(submission.data, 0, submission.data.Length)
            ms.Flush()
            w ""
            w h2
            w "Content-Disposition: form-data; name=\"thumbnail\"; filename=\"\""
            w "Content-Type: application/octet-stream"
            w ""
            w ""
            w h3

            use! reqStream = req2.GetRequestStreamAsync() |> Async.AwaitTask
            ms.Position <- 0L
            do! ms.CopyToAsync(reqStream) |> Async.AwaitTask
        }

        let! (key2, url2) = async {
            use! resp = req2.AsyncGetResponse()
            use sr = new StreamReader(resp.GetResponseStream())
            let! html = sr.ReadToEndAsync() |> Async.AwaitTask
            return (get_authenticity_token html, resp.ResponseUri)
        }

        let req3 = createRequest url2
        req3.Method <- "POST"
        req3.ContentType <- sprintf "multipart/form-data; boundary=%s" h1

        do! async {
            use ms = new MemoryStream()
            let w (s: string) =
                let bytes = Encoding.UTF8.GetBytes(sprintf "%s\n" s)
                ms.Write(bytes, 0, bytes.Length)
            
            w h2
            w "Content-Disposition: form-data; name=\"part\""
            w ""
            w "5"
            w h2
            w "Content-Disposition: form-data; name=\"key\""
            w ""
            w key2
            w h2
            w "Content-Disposition: form-data; name=\"submission_type\""
            w ""
            w "submission"
            w h2
            w "Content-Disposition: form-data; name=\"cat_duplicate\""
            w ""
            w ""
            w h2
            w "Content-Disposition: form-data; name=\"title\""
            w ""
            w submission.title
            w h2
            w "Content-Disposition: form-data; name=\"message\""
            w ""
            w submission.message
            w h2
            w "Content-Disposition: form-data; name=\"keywords\""
            w ""
            w (submission.keywords |> Seq.map (fun s -> s.Replace(' ', '_')) |> String.concat " ")
            w h2
            w "Content-Disposition: form-data; name=\"cat\""
            w ""
            w (sprintf "%d" submission.cat)
            w h2
            w "Content-Disposition: form-data; name=\"atype\""
            w ""
            w (sprintf "%d" submission.atype)
            w h2
            w "Content-Disposition: form-data; name=\"species\""
            w ""
            w (sprintf "%d" submission.species)
            w h2
            w "Content-Disposition: form-data; name=\"gender\""
            w ""
            w (sprintf "%d" submission.gender)
            w h2
            w "Content-Disposition: form-data; name=\"rating\""
            w ""
            w (sprintf "%d" submission.rating)
            w h2
            w "Content-Disposition: form-data; name=\"create_folder_name\""
            w ""
            w ""
            w h3

            use! reqStream = req3.GetRequestStreamAsync() |> Async.AwaitTask
            ms.Position <- 0L
            do! ms.CopyToAsync(reqStream) |> Async.AwaitTask
        }

        return! async {
            use! resp = req3.AsyncGetResponse()
            return resp.ResponseUri
        }
    }

    member this.SubmitPostAsync post = this.AsyncSubmitPost post |> Async.StartAsTask