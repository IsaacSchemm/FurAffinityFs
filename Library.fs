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
    | ``Artwork_Digital`` = 2
    | ``Artwork_Traditional`` = 3
    | ``Cellshading`` = 4
    | ``Crafting`` = 5
    | ``Designs`` = 6
    | ``Flash`` = 7
    | ``Fursuiting`` = 8
    | ``Icons`` = 9
    | ``Mosaics`` = 10
    | ``Photography`` = 11
    | ``Sculpting`` = 12
    | ``Story`` = 13
    | ``Poetry`` = 14
    | ``Prose`` = 15
    | ``Music`` = 16
    | ``Podcasts`` = 17
    | ``Skins`` = 18
    | ``Handhelds`` = 19
    | ``Resources`` = 20
    | ``Adoptables`` = 21
    | ``Auctions`` = 22
    | ``Contests`` = 23
    | ``Current_Events`` = 24
    | ``Desktops`` = 25
    | ``Stockart`` = 26
    | ``Screenshots`` = 27
    | ``Scraps`` = 28
    | ``Wallpaper`` = 29
    | ``YCH_or_Sale`` = 30
    | ``Other`` = 31

    type Type =
    | ``All`` = 1
    | ``Abstract`` = 2
    | ``Animal_related_non_anthro`` = 3
    | ``Anime`` = 4
    | ``Comics`` = 5
    | ``Doodle`` = 6
    | ``Fanart`` = 7
    | ``Fantasy`` = 8
    | ``Human`` = 9
    | ``Portraits`` = 10
    | ``Scenery`` = 11
    | ``Still_Life`` = 12
    | ``Tutorials`` = 13
    | ``Miscellaneous`` = 14
    | ``Baby_fur`` = 101
    | ``Bondage`` = 102
    | ``Digimon`` = 103
    | ``Fat_Furs`` = 104
    | ``Fetish_Other`` = 105
    | ``Fursuit`` = 106
    | ``Gore_or_Macabre_Art`` = 119
    | ``Hyper`` = 107
    | ``Inflation`` = 108
    | ``Macro_or_Micro`` = 109
    | ``Muscle`` = 110
    | ``My_Little_Pony_or_Brony`` = 111
    | ``Paw`` = 112
    | ``Pokemon`` = 113
    | ``Pregnancy`` = 114
    | ``Sonic`` = 115
    | ``Transformation`` = 116
    | ``TF_and_TG`` = 120
    | ``Vore`` = 117
    | ``Water_Sports`` = 118
    | ``General_Furry_Art`` = 100
    | ``Techno`` = 201
    | ``Trance`` = 202
    | ``House`` = 203
    | ``90s`` = 204
    | ``80s`` = 205
    | ``70s`` = 206
    | ``60s`` = 207
    | ``Pre_60s`` = 208
    | ``Classical`` = 209
    | ``Game_Music`` = 210
    | ``Rock`` = 211
    | ``Pop`` = 212
    | ``Rap`` = 213
    | ``Industrial`` = 214
    | ``Other_Music`` = 200

    type Species = Species of id: string
    with static member Unspecified = Species "1"

    type SpeciesInformation = {
        Group: string option
        Name: string
        Species: Species
    }

    type Gender =
    | ``Any`` = 0
    | ``Male`` = 2
    | ``Female`` = 3
    | ``Herm`` = 4
    | ``Intersex`` = 11
    | ``Trans_Male`` = 8
    | ``Trans_Female`` = 9
    | ``Non_Binary`` = 10
    | ``Multiple_characters`` = 6
    | ``Other_or_Not_Specified`` = 7

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
        "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:102.0) Gecko/20100101 Firefox/102.0"

    let AsyncListSpecies() = async {
        let req = WebRequest.CreateHttp(new Uri(BaseAddress, "/browse/"), UserAgent = UserAgent)
        use! resp = req.AsyncGetResponse()
        use sr = new StreamReader(resp.GetResponseStream())
        let! html = sr.ReadToEndAsync() |> Async.AwaitTask
        let document = HtmlDocument.Parse html

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

        return [
            for select in document.CssSelect "select[name=species]" do
                for x in getDescendants select do
                    match (HtmlNode.name x).ToLowerInvariant() with
                    | "option" ->
                        { Group = None; Species = Species (getValue x); Name = getName x}
                    | "optgroup" ->
                        for y in getDescendants x do
                            { Group = getLabel x; Species = Species (getValue y); Name = getName y }
                    | _ -> ()
        ]
    }

    let ListSpeciesAsync() =
        AsyncListSpecies()
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
            let (Species species_id) = metadata.species
            w species_id
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