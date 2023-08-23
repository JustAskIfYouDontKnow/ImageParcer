namespace GoogleParcer;

public class ImageDownloader
{
    private readonly HttpClient _client;

    public ImageDownloader(HttpClient client)
    {
        _client = client;
    }

    public async Task DownloadImageAsync(string imageUrl, string destinationPath)
    {
        using var imageResponse = await _client.GetAsync(imageUrl);
        if (imageResponse.IsSuccessStatusCode)
        {
            await using var imageStream = await imageResponse.Content.ReadAsStreamAsync();
            await using var fileStream = File.Create(destinationPath);
            await imageStream.CopyToAsync(fileStream);
        }
        else
        {
            Console.WriteLine($"Failed to download image from {imageUrl}. Status code: {imageResponse.StatusCode}");
        }
    }
}