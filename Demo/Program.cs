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
                a: "fc71372a-f89c-4dc6-9009-b7d462f88951",
                b: "cfd98cfb-113a-45bb-801c-0677a263ba59");
            string username = await FurAffinity.WhoamiAsync(credentials);
            Console.WriteLine(username);

            var slime = species
                .Where(x => x.Name.Contains("Slime"))
                .Select(x => x.Species)
                .Single();
            var galleries = await FurAffinity.ListGalleryFoldersAsync(credentials);
            var result = await FurAffinity.PostArtworkAsync(
                credentials,
                new FurAffinity.File(
                    File.ReadAllBytes(@"C:\Users\isaac\Desktop\test.png"),
                    "image/png"),
                new FurAffinity.ArtworkMetadata(
                    title: "test title",
                    message: "test description",
                    lock_comments: true,
                    keywords: FurAffinity.Keywords("tag1", "tag2"),
                    cat: FurAffinity.Category.Artwork_Digital,
                    scrap: true,
                    atype: FurAffinity.Type.General_Furry_Art,
                    species: slime,
                    gender: FurAffinity.Gender.Male,
                    rating: FurAffinity.Rating.General,
                    folder_ids: FurAffinity.FolderIds(
                        galleries[0].FolderId,
                        galleries[2].FolderId),
                    create_folder_name: FurAffinity.NoNewFolder));
            Console.WriteLine(result);
        }
    }
}
