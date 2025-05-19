class Product
{
    public string? ProductId { get; set; }
    public string? ManufacturerCode { get; set; }
    public string? Ean { get; set; }
    public string? Category { get; set; }
    public string? CategoryId { get; set; }
    public string? SubCategory { get; set; }
    public string? SubCategoryId { get; set; }
    public string? Manufacturer { get; set; }
    public string? Name { get; set; }
    public MarketingText[]? MarketingText { get; set; }
    public MainImage[]? MainImage { get; set; }
    public int Stock { get; set; }
    public Logistics[]? Logistics { get; set; }
    public string? Url { get; set; }
}
struct MarketingText
{
    public string? LongDescription { get; set; }
    public string? ShortDescription { get; set; }
    public string? ShortSummary { get; set; }
}

struct MainImage
{
    public string? LargePhoto { get; set; }
    public string? SmallPhoto { get; set; }
    public string? Thumbnail { get; set; }
}
struct Logistics
{
    public double? Volume { get; set; }
    public double? Weight { get; set; }
}

