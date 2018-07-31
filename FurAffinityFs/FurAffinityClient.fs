namespace FurAffinityFs

open System
open System.Net
open System.IO
open System.Text.RegularExpressions
open System.Text

type FurAffinityClientException(message: string) =
    inherit ApplicationException(message)

type FurAffinityClient(a: string, b: string) =
    let fafail str = raise (FurAffinityClientException str)

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
            use! reqStream = req2.GetRequestStreamAsync() |> Async.AwaitTask

            use fs = new FileStream("C:/Users/admin/Desktop/test.bin", FileMode.Create, FileAccess.Write)
            let w (s: string) =
                let bytes = Encoding.UTF8.GetBytes(sprintf "%s\n" s)
                fs.Write(bytes, 0, bytes.Length)
                reqStream.AsyncWrite(bytes, 0, bytes.Length)
            
            do! w h2
            do! w "Content-Disposition: form-data; name=\"part\""
            do! w ""
            do! w "3"
            do! w h2
            do! w "Content-Disposition: form-data; name=\"key\""
            do! w ""
            do! w key1
            do! w h2
            do! w "Content-Disposition: form-data; name=\"submission_type\""
            do! w ""
            do! w "submission"
            do! w h2
            do! w (sprintf "Content-Disposition: form-data; name=\"submission\"; filename=\"%s\"" filename)
            do! w (sprintf "Content-Type: %s" submission.contentType)
            do! w ""
            do! reqStream.FlushAsync() |> Async.AwaitTask
            do! reqStream.AsyncWrite (submission.data, 0, submission.data.Length)
            do! reqStream.FlushAsync() |> Async.AwaitTask
            do! fs.FlushAsync() |> Async.AwaitTask
            do! fs.AsyncWrite (submission.data, 0, submission.data.Length)
            do! fs.FlushAsync() |> Async.AwaitTask
            do! w ""
            do! w h2
            do! w "Content-Disposition: form-data; name=\"thumbnail\"; filename=\"\""
            do! w "Content-Type: application/octet-stream"
            do! w ""
            do! w ""
            do! w h3
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
            use! reqStream = req3.GetRequestStreamAsync() |> Async.AwaitTask
            use sw = new StreamWriter(reqStream)

            let w (s: string) = sw.WriteLineAsync s |> Async.AwaitTask
            
            do! w h2
            do! w "Content-Disposition: form-data; name=\"part\""
            do! w ""
            do! w "5"
            do! w h2
            do! w "Content-Disposition: form-data; name=\"key\""
            do! w ""
            do! w key2
            do! w h2
            do! w "Content-Disposition: form-data; name=\"submission_type\""
            do! w ""
            do! w "submission"
            do! w h2
            do! w "Content-Disposition: form-data; name=\"cat_duplicate\""
            do! w ""
            do! w ""
            do! w h2
            do! w "Content-Disposition: form-data; name=\"title\""
            do! w ""
            do! w submission.title
            do! w h2
            do! w "Content-Disposition: form-data; name=\"message\""
            do! w ""
            do! w submission.message
            do! w h2
            do! w "Content-Disposition: form-data; name=\"keywords\""
            do! w ""
            do! w (submission.keywords |> Seq.map (fun s -> s.Replace(' ', '_')) |> String.concat " ")
            do! w h2
            do! w "Content-Disposition: form-data; name=\"cat\""
            do! w ""
            do! w (sprintf "%d" submission.cat)
            do! w h2
            do! w "Content-Disposition: form-data; name=\"atype\""
            do! w ""
            do! w (sprintf "%d" submission.atype)
            do! w h2
            do! w "Content-Disposition: form-data; name=\"species\""
            do! w ""
            do! w (sprintf "%d" submission.species)
            do! w h2
            do! w "Content-Disposition: form-data; name=\"gender\""
            do! w ""
            do! w (sprintf "%d" submission.gender)
            do! w h2
            do! w "Content-Disposition: form-data; name=\"rating\""
            do! w ""
            do! w (sprintf "%d" submission.rating)
            do! w h2
            do! w "Content-Disposition: form-data; name=\"create_folder_name\""
            do! w ""
            do! w ""
            do! w h3
        }

        return! async {
            use! resp = req3.AsyncGetResponse()
            return resp.ResponseUri
        }
    }

    member this.SubmitPostAsync post = this.AsyncSubmitPost post |> Async.StartAsTask