using System.Net.Http.Json;

class Program
{
    private static async Task Main()
    {
        const string apiKey = "sh_Ga3yM4n68Pr72VOXu6aJx9pGwe9SybJ2GWWpIEJE";
        const string query = "Audi Q8 the object is fully visible";

        using var client = new HttpClient();
        client.DefaultRequestHeaders.Add("Authorization", $"Client-ID {apiKey}");

        var searchUrl = $"https://api.unsplash.com/search/photos?query={query}&per_page=1";
        var response = await client.GetAsync(searchUrl);

        if (response.IsSuccessStatusCode)
        {
            var searchResult = await response.Content.ReadFromJsonAsync<UnsplashSearchResult>();
            if (searchResult?.Results?.Count > 0)
            {
                var imageUrl = searchResult.Results[0].Urls.Regular;

                var imageResponse = await client.GetAsync(imageUrl);
                if (imageResponse.IsSuccessStatusCode)
                {
                    var imageBytes = await imageResponse.Content.ReadAsByteArrayAsync();
                    var imageName = $"{query.Replace(" ", "_")}.jpg";

                    await File.WriteAllBytesAsync(imageName, imageBytes);
                    Console.WriteLine($"Image saved as {imageName}");
                }
            }
            else
            {
                Console.WriteLine("No images found for the query.");
            }
        }
        else
        {
            Console.WriteLine("Failed to fetch data from Unsplash API.");
        }
    }
}

public class UnsplashSearchResult
{
    public List<UnsplashImage> Results { get; set; }
}

public class UnsplashImage
{
    public string Id { get; set; }
    public UnsplashUrls Urls { get; set; }
}

public class UnsplashUrls
{
    public string Regular { get; set; }
}