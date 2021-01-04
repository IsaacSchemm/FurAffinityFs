# FurAffinityFs

This is a .NET library that lets you upload artwork to [Fur Affinity](https://sfw.furaffinity.net/).

Not supported:
* Uploading other types of submissions (e.g. poetry, journals)
* Fetching info on submissions already posted (use [FAExport](https://github.com/Deer-Spangle/faexport) instead)

Usage

    public static async Task Main() {
        Console.Write("a cookie: ");
        string a = Console.ReadLine();
        Console.Write("b cookie: ");
        string b = Console.ReadLine();

        Uri uri = await FurAffinitySubmission.PostArtworkAsync(
            new FurAffinityCredentials(a, b),
            new FurAffinityFile(
                File.ReadAllBytes(@"C:\Windows\Web\Wallpaper\Windows\img0.jpg"),
                "image/jpeg"),
            new FurAffinitySubmission.ArtworkMetadata(
                title: "Test 1",
                message: "This is a test",
                keywords: new[] { "test_1", "test_2" },
                cat: FurAffinityCategory.Scraps,
                scrap: true,
                atype: FurAffinityType.General_Furry_Art,
                species: FurAffinitySpecies.Exotic_Other,
                gender: FurAffinityGender.Other_or_Not_Specified,
                rating: FurAffinityRating.General,
                lock_comments: true)
        );

        Console.WriteLine(uri);
    }
