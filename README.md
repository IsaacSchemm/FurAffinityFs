# FurAffinityFs

This is a .NET library that allows artwork uploads to [Fur Affinity](https://sfw.furaffinity.net/).

Usage

    public static async Task Main() {
        Console.Write("a cookie: ");
        string a = Console.ReadLine();
        Console.Write("b cookie: ");
        string b = Console.ReadLine();

        Uri uri = await FurAffinityFs.Api.PostArtwork.ExecuteAsync(
            new FurAffinityFs.FurAffinityCredentials(a, b),
            new FurAffinityFs.Models.Artwork(
                data: File.ReadAllBytes(@"C:\Windows\Web\Wallpaper\Windows\img0.jpg"),
                contentType: "image/jpeg",
                title: "Test 1",
                message: "This is a test",
                keywords: new[] { "test_1", "test_2" },
                cat: FurAffinityFs.Models.Category.Scraps,
                scrap: true,
                atype: FurAffinityFs.Models.Type.General_Furry_Art,
                species: FurAffinityFs.Models.Species.Exotic_Other,
                gender: FurAffinityFs.Models.Gender.Other_or_Not_Specified,
                rating: FurAffinityFs.Models.Rating.General,
                lock_comments: true)
        );

        Console.WriteLine(uri);
    }