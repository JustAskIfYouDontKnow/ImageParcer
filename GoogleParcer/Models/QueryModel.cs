namespace GoogleParcer.models;

public class QueryModel
{
    public string imageType { get; set; }

    public string imageSize { get; set; }

    public int imageQuantity { get; set; }

    public string searchTerm { get; set; }

    public string customSearchTerm { get; set; }

    public string searchType { get; set; }

    public string apiKey { get; set; }

    public string cx { get; set; }
}