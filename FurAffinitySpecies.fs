namespace FurAffinityFs

type FurAffinitySpeciesId = FurAffinitySpeciesId of string
with
    static member Unspecified = FurAffinitySpeciesId "1"

type FurAffinitySpecies = {
    Group: string option
    Name: string
    Id: FurAffinitySpeciesId
}
