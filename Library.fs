namespace FurAffinityFs

open System
open System.Text
open System.IO
open System.Net
open FSharp.Data

module FurAffinity =
    type ICredentials =
        abstract member A: string
        abstract member B: string

    type Credentials = {
        a: string
        b: string
    } with
        interface ICredentials with
            member this.A = this.a
            member this.B = this.b

    type IFile =
        abstract member Data: byte array
        abstract member ContentType: string

    type File = {
        data: byte array
        contentType: string
    } with
        interface IFile with
            member this.Data = this.data
            member this.ContentType = this.contentType

    type Category =
    | ``All`` = 1

    type Type =
    | ``All`` = 1

    type Species =
    | ``Unspecified_Any`` = 1

    type Gender =
    | ``Any`` = 0

    type PostOption<'T when 'T :> Enum> = {
        Group: string option
        Name: string
        Value: 'T
    } with
        override this.ToString() = $"""{this.Name} ({this.Value.ToString("d")})"""

    type PostOptions = {
        Categories: PostOption<Category> list
        Types: PostOption<Type> list
        Species: PostOption<Species> list
        Genders: PostOption<Gender> list
    }

    type Rating =
    | General = 0
    | Adult = 1
    | Mature = 2

    let private BaseAddress =
        new Uri("https://www.furaffinity.net/")

    let private ExtractAuthenticityToken (html: HtmlDocument) =
        let m =
            html.CssSelect("form[name=myform] input[name=key]")
            |> Seq.map (fun e -> e.AttributeValue("value"))
            |> Seq.tryHead
        match m with
            | Some token -> token
            | None -> failwith "Form \"myform\" with hidden input \"key\" not found in HTML from server"

    let private ToNewCookieContainer (credentials: ICredentials) =
        let c = new CookieContainer()
        c.Add(BaseAddress, new Cookie("a", credentials.A))
        c.Add(BaseAddress, new Cookie("b", credentials.B))
        c

    let UserAgent =
        "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:125.0) Gecko/20100101 Firefox/125.0"

    let AsyncWhoami credentials = async {
        let req = WebRequest.CreateHttp(new Uri(BaseAddress, "/help/"), UserAgent = UserAgent, CookieContainer = ToNewCookieContainer(credentials))
        use! resp = req.AsyncGetResponse()
        use sr = new StreamReader(resp.GetResponseStream())
        let! html = sr.ReadToEndAsync() |> Async.AwaitTask
        let document = HtmlDocument.Parse html
        return String.concat " / " [
            for item in document.CssSelect("#my-username") do
                item.InnerText().Trim().TrimStart('~')
        ]
    }

    let WhoamiAsync credentials =
        AsyncWhoami credentials
        |> Async.StartAsTask

    let AsyncListPostOptions() = async {
        let req = WebRequest.CreateHttp(new Uri(BaseAddress, "/browse/"), UserAgent = UserAgent)
        use! resp = req.AsyncGetResponse()
        use sr = new StreamReader(resp.GetResponseStream())
        let! html = sr.ReadToEndAsync() |> Async.AwaitTask
        let document = HtmlDocument.Parse(html)

        let getName node =
            node
            |> HtmlNode.innerText
        let getValue node =
            node
            |> HtmlNode.tryGetAttribute "value"
            |> Option.map (fun a -> HtmlAttribute.value a)
            |> Option.defaultValue (getName node)
        let getLabel node =
            node
            |> HtmlNode.tryGetAttribute "label"
            |> Option.map (fun a -> HtmlAttribute.value a)

        let getDescendants node = HtmlNode.descendants false (fun _ -> true) node

        let processSelects (selector: string) = [
            for select in document.CssSelect selector do
                for x in getDescendants select do
                    match (HtmlNode.name x).ToLowerInvariant() with
                    | "option" ->
                        { Group = None; Value = getValue x |> int |> enum; Name = getName x}
                    | "optgroup" ->
                        for y in getDescendants x do
                            { Group = getLabel x; Value = getValue y |> int |> enum; Name = getName y }
                    | _ -> ()
        ]

        return {
            Categories = processSelects "select[name=cat]"
            Types = processSelects "select[name=atype]"
            Species = processSelects "select[name=species]"
            Genders = processSelects "select[name=gender]"
        }
    }

    let ListPostOptionsAsync() =
        AsyncListPostOptions()
        |> Async.StartAsTask

    type ExistingGalleryFolderInformation = { FolderId: int64; Name: string }

    let AsyncListGalleryFolders (credentials: ICredentials) = async {
        let req = WebRequest.CreateHttp(new Uri(BaseAddress, $"/controls/folders/submissions/"), UserAgent = UserAgent, CookieContainer = ToNewCookieContainer(credentials))
        use! resp = req.AsyncGetResponse()
        use sr = new StreamReader(resp.GetResponseStream())
        let! html = sr.ReadToEndAsync() |> Async.AwaitTask
        let document = HtmlDocument.Parse html

        let regex = new System.Text.RegularExpressions.Regex("^/gallery/[^/]+/folder/([0-9]+)/")
        let extractId href =
            let m = regex.Match href
            if m.Success then Some (Int64.Parse m.Groups[1].Value) else None

        return [
            for link in document.CssSelect "a" do
                let id =
                    link.TryGetAttribute "href"
                    |> Option.map (fun a -> HtmlAttribute.value a)
                    |> Option.bind extractId
                match id with
                | Some s -> { FolderId = s; Name = HtmlNode.innerText link }
                | None -> ()
        ]
    }

    let ListGalleryFoldersAsync credentials =
        AsyncListGalleryFolders credentials
        |> Async.StartAsTask

    type ArtworkMetadata = {
        title: string
        message: string
        keywords: string list
        cat: Category
        scrap: bool
        atype: Type
        species: Species
        gender: Gender
        rating: Rating
        lock_comments: bool
        folder_ids: Set<int64>
        create_folder_name: string option
    }

    let Keywords ([<ParamArray>] arr: string[]) = List.ofArray arr
    let FolderIds ([<ParamArray>] arr: int64[]) = Set.ofArray arr
    let NewFolder (name: string) = Some name
    let NoNewFolder: string option = None

    let AsyncPostArtwork (credentials: ICredentials) (file: File) (metadata: ArtworkMetadata) = async {
        let toUri (path: string) =
            new Uri(BaseAddress, path)

        let createRequest (credentials: ICredentials) (uri: Uri) =
            WebRequest.CreateHttp(uri, UserAgent = UserAgent, CookieContainer = ToNewCookieContainer(credentials))

        // multipart separators
        let boundary = sprintf "-----------------------------%d" DateTime.UtcNow.Ticks
        let interior_boundary = sprintf "--%s" boundary
        let final_boundary = sprintf "--%s--" boundary

        let ext = Seq.last (file.contentType.Split('/'))
        let filename = sprintf "file.%s" ext

        let! artwork_submission_page_key = async {
            let req = "/submit/" |> toUri |> createRequest credentials
            use! resp = req.AsyncGetResponse()
            use sr = new StreamReader(resp.GetResponseStream())
            let! html = sr.ReadToEndAsync() |> Async.AwaitTask
            let token = html |> HtmlDocument.Parse |> ExtractAuthenticityToken
            return token
        }

        let artwork_submission_page_req = createRequest credentials (toUri "/submit/upload")
        artwork_submission_page_req.Method <- "POST"
        artwork_submission_page_req.ContentType <- sprintf "multipart/form-data; boundary=%s" boundary

        let body1 =
            use memory_buffer = new MemoryStream()
            let w (s: string) =
                let bytes = Encoding.UTF8.GetBytes(sprintf "%s\n" s)
                memory_buffer.Write(bytes, 0, bytes.Length)

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
            w (sprintf "Content-Type: %s" file.contentType)
            w ""
            memory_buffer.Flush()
            memory_buffer.Write(file.data, 0, file.data.Length)
            memory_buffer.Flush()
            w ""
            w interior_boundary
            w "Content-Disposition: form-data; name=\"thumbnail\"; filename=\"\""
            w "Content-Type: application/octet-stream"
            w ""
            w ""
            w final_boundary

            memory_buffer.ToArray()

        do! async {
            use! reqStream = artwork_submission_page_req.GetRequestStreamAsync() |> Async.AwaitTask
            do! reqStream.WriteAsync(body1, 0, body1.Length) |> Async.AwaitTask
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

        let finalize_submission_page_req = createRequest credentials finalize_submission_page_url
        finalize_submission_page_req.Method <- "POST"
        finalize_submission_page_req.ContentType <- sprintf "multipart/form-data; boundary=%s" boundary

        let body2 =
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
            w metadata.title
            w interior_boundary
            w "Content-Disposition: form-data; name=\"message\""
            w ""
            w metadata.message
            w interior_boundary
            w "Content-Disposition: form-data; name=\"keywords\""
            w ""
            w (metadata.keywords |> Seq.map (fun s -> s.Replace(' ', '_')) |> String.concat " ")
            w interior_boundary
            w "Content-Disposition: form-data; name=\"cat\""
            w ""
            w (metadata.cat.ToString("d"))
            w interior_boundary
            w "Content-Disposition: form-data; name=\"atype\""
            w ""
            w (metadata.atype.ToString("d"))
            w interior_boundary
            w "Content-Disposition: form-data; name=\"species\""
            w ""
            w (metadata.species.ToString("d"))
            w interior_boundary
            w "Content-Disposition: form-data; name=\"gender\""
            w ""
            w (metadata.gender.ToString("d"))
            w interior_boundary
            w "Content-Disposition: form-data; name=\"rating\""
            w ""
            w (metadata.rating.ToString("d"))
            w interior_boundary
            w "Content-Disposition: form-data; name=\"create_folder_name\""
            w ""
            w ""
            if metadata.scrap then
                w interior_boundary
                w "Content-Disposition: form-data; name=\"scrap\""
                w ""
                w "1"
            if metadata.lock_comments then
                w interior_boundary
                w "Content-Disposition: form-data; name=\"lock_comments\""
                w ""
                w "on"
            for id in metadata.folder_ids do
                w interior_boundary
                w "Content-Disposition: form-data; name=\"folder_ids[]\""
                w ""
                w (sprintf "%d" id)
            match metadata.create_folder_name with
            | Some name ->
                w interior_boundary
                w "Content-Disposition: form-data; name=\"create_folder_name\""
                w ""
                w name
            | None -> ()
            w final_boundary

            ms.ToArray()

        do! async {
            use! reqStream = finalize_submission_page_req.GetRequestStreamAsync() |> Async.AwaitTask
            do! reqStream.WriteAsync(body2, 0, body2.Length) |> Async.AwaitTask
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

    let PostArtworkAsync credentials file metadata =
        AsyncPostArtwork credentials file metadata
        |> Async.StartAsTask
