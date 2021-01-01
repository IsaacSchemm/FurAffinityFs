# FurAffinityFs

This is a .NET library that allows artwork uploads to [Fur Affinity](https://sfw.furaffinity.net/).

Usage

    class Creds : IFurAffinityCredentials {
        public string A { get; set; }
        public string B { get; set; }
    }

    public static async Task Main() {
        Console.Write("a cookie: ");
        string a = Console.ReadLine();
        Console.Write("b cookie: ");
        string b = Console.ReadLine();

        Uri uri = await FurAffinityFs.Api.CreateSubmission.ExecuteAsync(
            new Creds { A = a, B = b },
            new Artwork(
                data: File.ReadAllBytes(@"C:\Windows\Web\Wallpaper\Windows\img0.jpg"),
                contentType: "image/jpeg",
                title: "Test 1",
                message: "This is a test",
                keywords: new[] { "test_1", "test_2" },
                cat: Category.Scraps,
                scrap: true,
                atype: FurAffinityFs.Models.Type.General_Furry_Art,
                species: Species.Exotic_Other,
                gender: Gender.Other_or_Not_Specified,
                rating: Rating.General,
                lock_comments: true)
        );

        Console.WriteLine(uri);
    }