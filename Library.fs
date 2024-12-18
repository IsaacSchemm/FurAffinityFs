﻿namespace FurAffinityFs

open System
open System.Net.Http
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
        abstract member FileName: string
        abstract member Data: byte array

    type File = {
        fileName: string
        data: byte array
    } with
        interface IFile with
            member this.FileName = this.fileName
            member this.Data = this.data

    type Category = All = 1

    type Type = All = 1

    type Species = Unspecified_Any = 1

    type Gender = Any = 0

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

    type ExistingGalleryFolderInformation = {
        FolderId: int64
        Name: string
    }

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

    type Journal = {
        subject: string
        message: string
        disable_comments: bool
        make_featured: bool
    }

    #if NETFRAMEWORK
    let private handler = lazy new HttpClientHandler(UseCookies = false)
    #else
    let private handler = lazy new SocketsHttpHandler(UseCookies = false, PooledConnectionLifetime = TimeSpan.FromMinutes(5))
    #endif

    let private getClient (credentials: ICredentials) =
        let client = new HttpClient(handler.Value, disposeHandler = false)
        client.BaseAddress <- new Uri("https://www.furaffinity.net/")
        client.DefaultRequestHeaders.Add("Cookie", $"a={credentials.A}; b={credentials.B}")
        client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:125.0) Gecko/20100101 Firefox/125.0")
        client

    let private ExtractAuthenticityToken (formName: string) (html: HtmlDocument) =
        let m =
            html.CssSelect($"form[name={formName}] input[name=key]")
            |> Seq.map (fun e -> e.AttributeValue("value"))
            |> Seq.tryHead
        match m with
            | Some token -> token
            | None -> failwith $"Form \"{formName}\" with hidden input \"key\" not found in HTML from server"

    let WhoamiAsync credentials = task {
        use client = getClient credentials
        use! resp = client.GetAsync "/help/"
        ignore (resp.EnsureSuccessStatusCode())

        let! html = resp.Content.ReadAsStringAsync()
        let document = HtmlDocument.Parse html
        return String.concat " / " [
            for item in document.CssSelect("#my-username") do
                item.InnerText().Trim().TrimStart('~')
        ]
    }

    module private Scraper =
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

        let getDescendants node =
            HtmlNode.descendants false (fun _ -> true) node

        let getPostOptions (selector: string) (document: HtmlDocument) = [
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

    let ListPostOptionsAsync credentials = task {
        use client = getClient credentials
        use! resp = client.GetAsync "/browse/"
        ignore (resp.EnsureSuccessStatusCode())

        let! html = resp.Content.ReadAsStringAsync()
        let document = HtmlDocument.Parse(html)

        return {
            Categories = document |> Scraper.getPostOptions "select[name=cat]"
            Types = document |> Scraper.getPostOptions "select[name=atype]"
            Species = document |> Scraper.getPostOptions "select[name=species]"
            Genders = document |> Scraper.getPostOptions "select[name=gender]"
        }
    }

    let ListGalleryFoldersAsync (credentials: ICredentials) = task {
        use client = getClient credentials
        use! resp = client.GetAsync "/controls/folders/submissions/"
        ignore (resp.EnsureSuccessStatusCode())

        let! html = resp.Content.ReadAsStringAsync()
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
                | Some s ->
                    { FolderId = s; Name = HtmlNode.innerText link }
                | None -> ()
        ]
    }

    let Keywords ([<ParamArray>] arr: string[]) = List.ofArray arr
    let FolderIds ([<ParamArray>] arr: int64[]) = Set.ofArray arr
    let NewFolder (name: string) = Some name
    let NoNewFolder: string option = None

    type MultipartSegmentValue =
    | FieldPart of string
    | FilePart of IFile

    let private multipart segments =
        let content = new MultipartFormDataContent()
        for segment in segments do
            match segment with
            | name, FieldPart value -> content.Add(new StringContent(value), name)
            | name, FilePart file -> content.Add(new ByteArrayContent(file.Data), name, file.FileName)
        content

    let PostArtworkAsync (credentials: ICredentials) (file: IFile) (metadata: ArtworkMetadata) = task {
        use client = getClient credentials

        let! artwork_submission_page_key = task {
            use! resp = client.GetAsync "/submit/"
            ignore (resp.EnsureSuccessStatusCode())
            let! html = resp.Content.ReadAsStringAsync()
            let token = html |> HtmlDocument.Parse |> ExtractAuthenticityToken "myform"
            return token
        }

        let! finalize_submission_page_key = task {
            use req = new HttpRequestMessage(System.Net.Http.HttpMethod.Post, "/submit/upload")
            req.Content <- multipart [
                "key", FieldPart artwork_submission_page_key
                "submission_type", FieldPart "submission"
                "submission", FilePart file
            ]

            use! resp = client.SendAsync req
            ignore (resp.EnsureSuccessStatusCode())
            let! html = resp.Content.ReadAsStringAsync()
            if html.Contains "Security code missing or invalid." then
                failwith "Security code missing or invalid for page"
            return html |> HtmlDocument.Parse |> ExtractAuthenticityToken "myform"
        }

        return! task {
            let req = new HttpRequestMessage(System.Net.Http.HttpMethod.Post, "/submit/finalize/")
            req.Content <- multipart [
                "part", FieldPart "5"
                "key", FieldPart finalize_submission_page_key
                "submission_type", FieldPart "submission"
                "cat_duplicate", FieldPart ""
                "title", FieldPart metadata.title
                "message", FieldPart metadata.message
                "keywords", FieldPart (metadata.keywords |> Seq.map (fun s -> s.Replace(' ', '_')) |> String.concat " ")
                "cat", FieldPart (metadata.cat.ToString("d"))
                "atype", FieldPart (metadata.atype.ToString("d"))
                "species", FieldPart (metadata.species.ToString("d"))
                "gender", FieldPart (metadata.gender.ToString("d"))
                "rating", FieldPart (metadata.rating.ToString("d"))
                if metadata.scrap then
                    "scrap", FieldPart "1"
                if metadata.lock_comments then
                    "lock_comments", FieldPart "on"
                for id in metadata.folder_ids do
                    "folder_ids[]", FieldPart $"{id}"
                match metadata.create_folder_name with
                | None -> ()
                | Some name ->
                    "create_folder_name", FieldPart name
            ]
            use! resp = client.SendAsync req
            let! html = resp.Content.ReadAsStringAsync()
            if html.Contains "Security code missing or invalid." then
                failwith "Security code missing or invalid for page"

            return resp.RequestMessage.RequestUri
        }
    }

    let private form segments =
        let content = new FormUrlEncodedContent(dict segments)
        content

    let PostJournalAsync (credentials: ICredentials) (journal: Journal) = task {
        use client = getClient credentials

        let! journal_submission_page_key = task {
            use! resp = client.GetAsync "/controls/journal/"
            ignore (resp.EnsureSuccessStatusCode())
            let! html = resp.Content.ReadAsStringAsync()
            let token = html |> HtmlDocument.Parse |> ExtractAuthenticityToken "MsgForm"
            return token
        }

        return! task {
            let req = new HttpRequestMessage(System.Net.Http.HttpMethod.Post, "/controls/journal/")
            req.Content <- form [
                "id", "0"
                "key", journal_submission_page_key
                "do", "update"
                "subject", journal.subject
                "message", journal.message
                if journal.disable_comments then
                    "disable_comments", "on"
                if journal.make_featured then
                    "make_featured", "on"
            ]
            use! resp = client.SendAsync req
            let! html = resp.Content.ReadAsStringAsync()
            if html.Contains "Security code missing or invalid." then
                failwith "Security code missing or invalid for page"
        }
    }
