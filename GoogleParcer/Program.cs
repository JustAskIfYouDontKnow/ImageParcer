using GoogleParcer.models;

namespace GoogleParcer;

public static class Program
{

    private const string ApiKey = "AIzaSyAw4S9jnHPMli4bnrqmjFzDkNE3evYfVy4";
    private const string Cx = "b08f25dda3ed1404d";
    
    // private const string CustomTerm = " full body newest model car";
    private const string CustomTerm = "";

    private static async Task Main(string[] args)
    {
        var client = InitClient();
        
        var searchTerms = GetSearchTerms();
        
        var queryList = GenerateQueryList(searchTerms);
        
        await DownloadImageFromQueryListAsync(client, queryList);
        
        Console.WriteLine("Work is done");
    }

    private static ImageSearcher InitClient()
    {
        var client = new HttpClient();
        client.DefaultRequestHeaders.UserAgent.ParseAdd("MyUserAgent/1.0");

        var imageDownloader = new ImageDownloader(client);
        var imageSearcher = new ImageSearcher(client, imageDownloader);
        return imageSearcher;
    }
    private static IEnumerable<string> GetSearchTerms()
    {
        return new[]
        {
            "BMW X2",
            "Geely EMGRAND GT",
            "Lexus UX200",
            "Lexus ES200",
            "Lexus LX450",
            "Lexus NX200", 
            "Lexus RX350",
        };
    }
    private static IEnumerable<QueryModel> GenerateQueryList(IEnumerable<string> searchTerms)
    {
        return searchTerms.Select(
                term => new QueryModel
                {
                    imageType = "jpg",
                    imageQuantity = 1,
                    imageSize = ImageSizes.Large.ToString(),
                    searchTerm = term,
                    customSearchTerm = CustomTerm,
                    searchType = "image",
                    apiKey = ApiKey,
                    cx = Cx,
                }).ToList();
    }
    private static async Task DownloadImageFromQueryListAsync(ImageSearcher client, IEnumerable<QueryModel> queryList)
    {
        var tasks = queryList.Select(client.SearchAndDownloadImagesAsync);
        await Task.WhenAll(tasks);
    }
}