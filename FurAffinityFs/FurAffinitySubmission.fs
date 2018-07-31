namespace FurAffinityFs

type FurAffinitySubmission = {
    data: byte[]
    contentType: string
    title: string
    message: string
    keywords: seq<string>
    cat: int
    atype: int
    species: int
    gender: int
    rating: int
}