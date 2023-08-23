using System.Text;
using Newtonsoft.Json;

namespace GoogleParcer;

public class ImageSearcher
{
    private readonly HttpClient _client;
    private readonly ImageDownloader _imageDownloader;
    private const int DownloadNext = 1;

    public ImageSearcher(HttpClient client, ImageDownloader imageDownloader)
    {
        _client = client;
        _imageDownloader = imageDownloader;
    }

    public async Task SearchAndDownloadImagesAsync(string apiKey, string cx, string searchTerm, string customTerm)
    {
        var fullTerm = searchTerm + customTerm;
        
        for (var i = 0; i < DownloadNext; i++)
        {
            var searchQuery = CreateSearchQuery(apiKey, cx, fullTerm, i);
            var imageUrl = await SearchImagesAsync(searchQuery);
            
            var imageExtension = Path.GetExtension(imageUrl);
            var directoryPath = CreateImageDirectory(searchTerm);

            var fileName = $"{searchTerm}_{Guid.NewGuid():N}{imageExtension}";
            var filePath = Path.Combine(directoryPath, fileName);

            await _imageDownloader.DownloadImageAsync(imageUrl, filePath);

            Console.WriteLine($"Downloaded image for {searchTerm}");
        }
    }

    private static string CreateSearchQuery(string apiKey, string cx, string searchTerm, int i)
    {
        const string imageType = "jpg";
        const string imageSize = "HUGE";
        const int imageQuantity = 1;
        const string rightsParam = "cc_publicdomain|cc_attribute|cc_sharealike|cc_noncommercial|cc_nonderived";
        
        var escapedSearchTerm = Uri.EscapeDataString(searchTerm);

        var searchQuery = new StringBuilder();
        searchQuery.Append(
            $"https://customsearch.googleapis.com/customsearch/v1?" 
            + $"cx={cx}&fileType={imageType}" 
            + $"&imgSize={imageSize}" 
            + $"&imgType=photo&num={imageQuantity}" 
            + $"&q={escapedSearchTerm}"
            // $"&rights={rightsParam}" +
            + $"&searchType=image" 
            + $"&start={i}" 
            + $"&key={apiKey}"
        );

        return searchQuery.ToString();
    }

    private async Task<string> SearchImagesAsync(string searchQuery)
    {
        var response = await _client.GetAsync(searchQuery);

        if (response.IsSuccessStatusCode)
        {
            var responseBody = await response.Content.ReadAsStringAsync();
            dynamic json = JsonConvert.DeserializeObject(responseBody) ?? throw new InvalidOperationException();
            return json.items[0].link;
        }

        Console.WriteLine($"Error: {response.StatusCode}");

        return string.Empty;
    }

    private static string CreateImageDirectory(string searchTerm)
    {
        var directoryPath = Path.Combine("Images", searchTerm);
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }
        return directoryPath;
    }
}