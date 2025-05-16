class Product
{
    public int DataSheetComplete { get; set; }
    public int Id { get; set; }
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
    public string? Classification { get; set; }
    public Logistics[]? Logistics { get; set; }
    public string? Url { get; set; }
    public string[]? Eans { get; set; }
    public string[]? PartNumbers { get; set; }
    public Bullets[]? Bullets { get; set;}
    public Medias[]? Medias {get; set;}
    public Options[]? Options {get; set;}
    public Specification[]? Specifications {get; set;}
}
struct MarketingText
{
    public string? LongDescription { get; set; }
    public string? ShortDescription { get; set; }
    public string? LongSummary { get; set; }
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
    public double? Height { get; set; }
    public double? Length { get; set; }
    public double? Width { get; set; }
    public double? Volume { get; set; }
    public double? UnitPerBox { get; set; }
    public string? Service { get; set; }
    public double? Weight { get; set; }
}
struct Bullets
{
    public string? Id { get; set; }
    public string? Value { get; set; }
}
struct Medias
{
    public string? Id { get; set; }
    public string? Type { get; set; }
    public string? Url { get; set; }
}
struct Options
{
    public string? PartNumber { get; set; }
}
struct Specification
{
    public string? GroupName { get; set; }
    public int? IdGroup { get; set; }
    public Especificaciones[] Specifications {get; set;}

}
struct Especificaciones
{
    public string? Name {get; set;}
    public string? Value {get; set;}
    public int? IdSpec {get; set;}
}
