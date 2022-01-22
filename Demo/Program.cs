using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Demo {
    class Program {
        static async Task Main() {
            var species = await FurAffinityFs.FurAffinitySubmission.ListSpeciesAsync();
            var lizard = species
                .Where(x => x.Name == "Lizard")
                .Select(x => x.Id)
                .Single();
            var result = await FurAffinityFs.FurAffinitySubmission.PostArtworkAsync(
                new FurAffinityFs.FurAffinityCredentials(
                    a: "afcfaa2e-06f8-4f83-90f4-4dfdf7df93f0",
                    b: "6afd3cf9-8de9-4938-b9eb-2a39771337cf"),
                new FurAffinityFs.FurAffinityFile(
                    File.ReadAllBytes(@"C:\Users\admin\Pictures\mascot.png"),
                    "image/png"),
                new FurAffinityFs.FurAffinitySubmission.ArtworkMetadata(
                    title: "test title",
                    message: "test description",
                    lock_comments: true,
                    keywords: new[] { "tag1", "tag2" },
                    cat: FurAffinityFs.FurAffinityCategory.Artwork_Digital,
                    scrap: true,
                    atype: FurAffinityFs.FurAffinityType.General_Furry_Art,
                    species: lizard,
                    gender: FurAffinityFs.FurAffinityGender.Male,
                    rating: FurAffinityFs.FurAffinityRating.General));
            Console.WriteLine(result);
        }
    }
}
