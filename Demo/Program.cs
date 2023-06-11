﻿using FurAffinityFs;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Demo {
    class Program {
        static async Task Main() {
            var species = await FurAffinity.ListSpeciesAsync();
            var lizard = species
                .Where(x => x.Name == "Lizard")
                .Select(x => x.Id)
                .Single();
            var result = await FurAffinity.PostArtworkAsync(
                new FurAffinity.Credentials(
                    a: "afcfaa2e-06f8-4f83-90f4-4dfdf7df93f0",
                    b: "6afd3cf9-8de9-4938-b9eb-2a39771337cf"),
                new FurAffinity.File(
                    File.ReadAllBytes(@"C:\Users\admin\Pictures\mascot.png"),
                    "image/png"),
                new FurAffinity.ArtworkMetadata(
                    title: "test title",
                    message: "test description",
                    lock_comments: true,
                    keywords: new[] { "tag1", "tag2" },
                    cat: FurAffinity.Category.Artwork_Digital,
                    scrap: true,
                    atype: FurAffinity.Type.General_Furry_Art,
                    species: lizard,
                    gender: FurAffinity.Gender.Male,
                    rating: FurAffinity.Rating.General));
            Console.WriteLine(result);
        }
    }
}
