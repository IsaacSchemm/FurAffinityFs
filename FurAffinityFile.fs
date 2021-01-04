namespace FurAffinityFs

type IFurAffinityFile =
    abstract member Data: byte array
    abstract member ContentType: string

type FurAffinityFile = {
    data: byte array
    contentType: string
} with
    interface IFurAffinityFile with
        member this.Data = this.data
        member this.ContentType = this.contentType