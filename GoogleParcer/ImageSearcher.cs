using System.Text;
using GoogleParcer.models;
using Newtonsoft.Json;

namespace GoogleParcer;

public class ImageSearcher
{
    private readonly HttpClient _client;
    private readonly ImageDownloader _imageDownloader;
    
    public ImageSearcher(HttpClient client, ImageDownloader imageDownloader)
    {
        _client = client;
        _imageDownloader = imageDownloader;
    }
    
    private async Task<string[]> SearchImagesAsync(string searchQuery)
    {
        var response = await _client.GetAsync(searchQuery);
        var imageLinks = new List<string>(); // Создаем список для хранения ссылок на изображения

        if (response.IsSuccessStatusCode)
        {
            var responseBody = await response.Content.ReadAsStringAsync();
            dynamic json = JsonConvert.DeserializeObject(responseBody) ?? throw new InvalidOperationException();

            foreach (var item in json.items)
            {
                string imageUrl = item.link;
                imageLinks.Add(imageUrl);
            }
        }
        else
        {
            Console.WriteLine($"Error: {response.StatusCode}");
            throw new Exception();
        }

        return imageLinks.ToArray();
    }
    
    public async Task SearchAndDownloadImagesAsync(QueryModel queryModel)
    {
        var searchQuery = CreateSearchQuery(queryModel);
        var imageUrls = await SearchImagesAsync(searchQuery);

        foreach (var imageUrl in imageUrls)
        {
            var filePath = CreateFilePath(queryModel);

            await _imageDownloader.DownloadImageAsync(imageUrl, filePath);

            Console.WriteLine($"Downloaded image for {queryModel.searchTerm}");
        }
    }
    
    private static string CreateSearchQuery(QueryModel queryModel)
    {
   
        const string rightsParam = "cc_publicdomain|cc_attribute|cc_sharealike|cc_noncommercial|cc_nonderived";
        
        var escapedSearchTerm = Uri.EscapeDataString(queryModel.searchTerm + queryModel.customSearchTerm);

        var searchQuery = new StringBuilder();
        searchQuery.Append(
            $"https://customsearch.googleapis.com/customsearch/v1?" 
            + $"cx={queryModel.cx}" 
            + $"&fileType={queryModel.imageType}" 
            + $"&imgSize={queryModel.imageSize}" 
            + $"&imgType=photo&num={queryModel.imageQuantity}" 
            + $"&q={escapedSearchTerm}"
            //+ $"&rights={rightsParam}"
            + $"&searchType={queryModel.searchType}" 
            + $"&start=1" 
            + $"&key={queryModel.apiKey}"
        );

        return searchQuery.ToString();
    }
    
    private static string CreateFilePath(QueryModel queryModel)
    {
        var directoryPath = CreateImageDirectory(queryModel.searchTerm);
        var invalidChars = Path.GetInvalidFileNameChars();
        var sanitizedSearchTerm = new string(queryModel.searchTerm.Select(c => invalidChars.Contains(c) ? '_' : c).ToArray());
        var fileName = $"{sanitizedSearchTerm}_{Guid.NewGuid():N}.{queryModel.imageType}";
        var filePath = Path.Combine(directoryPath, fileName);
        return filePath;
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