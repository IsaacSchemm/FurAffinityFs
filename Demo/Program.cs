using FurAffinityFs;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Demo {
    class Program {
        static async Task Main() {
            var credentials = new FurAffinity.Credentials(
                a: "00000000-0000-0000-0000-000000000000",
                b: "00000000-0000-0000-0000-000000000000");
            string username = await FurAffinity.WhoamiAsync(credentials);
            Console.WriteLine(username);

            var postOptions = await FurAffinity.ListPostOptionsAsync(credentials);
            var slime = postOptions.Species
                .Where(x => x.Name.Contains("Slime"))
                .Select(x => x.Value)
                .Single();
            var male = postOptions.Genders
                .Where(x => x.Name == "Male")
                .Select(x => x.Value)
                .Single();
            var galleries = await FurAffinity.ListGalleryFoldersAsync(credentials);
            var result = await FurAffinity.PostArtworkAsync(
                credentials,
                new FurAffinity.File(
                    "test.png",
                    File.ReadAllBytes(@"C:\Users\isaac\Desktop\test.png")),
                new FurAffinity.ArtworkMetadata(
                    title: "test title",
                    message: "test description",
                    lock_comments: true,
                    keywords: FurAffinity.Keywords("tag1", "tag2"),
                    cat: FurAffinity.Category.All,
                    scrap: false,
                    atype: FurAffinity.Type.All,
                    species: slime,
                    gender: male,
                    rating: FurAffinity.Rating.General,
                    folder_ids: [
                        galleries[0].FolderId,
                        galleries[2].FolderId
                    ],
                    create_folder_name: FurAffinity.NewFolder("abc")));
            Console.WriteLine(result);
        }
    }
}
