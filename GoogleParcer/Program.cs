namespace GoogleParcer;

public static class Program
{
    private static async Task Main(string[] args)
    {
        const string apiKey = "AIzaSyAw4S9jnHPMli4bnrqmjFzDkNE3evYfVy4";
        const string cx = "b08f25dda3ed1404d";
        
        using var client = new HttpClient();
        client.DefaultRequestHeaders.UserAgent.ParseAdd("MyUserAgent/1.0");

        var imageDownloader = new ImageDownloader(client);
        var imageSearcher = new ImageSearcher(client, imageDownloader);

        
        //todo: get terms from excel
        string[] searchTerms =
        {
            // "BMW X2", 
            // "Geely EMGRAND GT",
            // "Lexus UX200",
            // "Lexus ES200",
            // "Lexus LX450",
            // "Lexus NX200", 
            "Lexus RX350",
        };
        
        const string customTerm = " full body car";
        
        var tasks = searchTerms
            .Select(term => imageSearcher.SearchAndDownloadImagesAsync(apiKey, cx, term, customTerm));
        
        await Task.WhenAll(tasks);

        Console.WriteLine("Work is done");
    }
}
