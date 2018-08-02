namespace FurAffinityFs

type FurAffinitySubmission = {
    data: byte[]
    contentType: string
    title: string
    message: string
    keywords: seq<string>
    cat: FurAffinityCategory
    atype: FurAffinityType
    species: FurAffinitySpecies
    gender: FurAffinityGender
    rating: FurAffinityRating
}