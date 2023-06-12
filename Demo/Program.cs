using FurAffinityFs;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Demo {
    class Program {
        static async Task Main() {
            var species = await FurAffinity.ListSpeciesAsync();
            var credentials = new FurAffinity.Credentials(
                a: "afcfaa2e-06f8-4f83-90f4-4dfdf7df93f0",
                b: "6afd3cf9-8de9-4938-b9eb-2a39771337cf");
            var lizard = species
                .Where(x => x.Name == "Lizard")
                .Select(x => x.Species)
                .Single();
            var galleries = await FurAffinity.ListGalleryFoldersAsync(credentials);
            var result = await FurAffinity.PostArtworkAsync(
                credentials,
                new FurAffinity.File(
                    File.ReadAllBytes(@"C:\Users\admin\Pictures\mascot.png"),
                    "image/png"),
                new FurAffinity.ArtworkMetadata(
                    title: "test title",
                    message: "test description",
                    lock_comments: true,
                    keywords: FurAffinity.Keywords("tag1", "tag2"),
                    cat: FurAffinity.Category.Artwork_Digital,
                    scrap: true,
                    atype: FurAffinity.Type.General_Furry_Art,
                    species: lizard,
                    gender: FurAffinity.Gender.Male,
                    rating: FurAffinity.Rating.General,
                    folder_ids: FurAffinity.FolderIds(
                        galleries[0].Folder.id,
                        galleries[2].Folder.id),
                    create_folder_name: FurAffinity.NoNewFolder));
            Console.WriteLine(result);
        }
    }
}
