using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace FurAffinityFs.AzureFunctionsEndpoint {
    public static class PostArtwork {
        public class Parameters {
            [JsonRequired]
            public string A { get; set; }

            [JsonRequired]
            public string B { get; set; }

            [JsonRequired]
            public string Base64 { get; set; }

            [JsonRequired]
            public string ContentType { get; set; }

            [JsonRequired]
            public string Title { get; set; }

            [JsonRequired]
            public Models.Rating Rating { get; set; }

            public string Description { get; set; }
            public IEnumerable<string> Tags { get; set; }
            public Models.Category? Category { get; set; }
            public bool? Scraps { get; set; }
            public Models.Type? Type { get; set; }
            public Models.Species? Species { get; set; }
            public Models.Gender? Gender { get; set; }
            public bool? LockComments { get; set; }
        }

        [FunctionName("artwork")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req) {
            if (!req.ContentType.StartsWith("application/json") && !req.ContentType.StartsWith("text/json"))
                return new StatusCodeResult((int)HttpStatusCode.NotAcceptable);

            try {
                using var stream = req.Body;
                using var sr = new StreamReader(stream);
                string json = await sr.ReadToEndAsync();

                var parameters = JsonConvert.DeserializeObject<Parameters>(json);
                var uri = await Api.PostArtwork.ExecuteAsync(
                    new FurAffinityCredentials(
                        a: parameters.A,
                        b: parameters.B),
                    new Models.Artwork(
                        data: Convert.FromBase64String(parameters.Base64),
                        contentType: parameters.ContentType,
                        title: parameters.Title,
                        rating: parameters.Rating,
                        message: parameters.Description ?? "",
                        keywords: parameters.Tags ?? Enumerable.Empty<string>(),
                        cat: parameters.Category ?? Models.Category.All,
                        scrap: parameters.Scraps ?? false,
                        atype: parameters.Type ?? Models.Type.All,
                        species: parameters.Species ?? Models.Species.Unspecified,
                        gender: parameters.Gender ?? Models.Gender.Any,
                        lock_comments: parameters.LockComments ?? false));
                return new OkObjectResult(uri.AbsoluteUri);
            } catch (JsonException ex) {
                return new BadRequestObjectResult(new { error = ex.Message });
            } catch (WebException) {
                return new StatusCodeResult((int)HttpStatusCode.BadGateway);
            } catch (Exception) {
                return new StatusCodeResult((int)HttpStatusCode.InternalServerError);
            }
        }
    }
}
