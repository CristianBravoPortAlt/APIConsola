using System.Net.Http.Headers;
using System.Text.Json;
namespace APIConsola;

class Proveedor : IProveedor
{
    public static Authenticate autenticador;
    static JsonSerializerOptions GetOptions()
    {
        return new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
    }
    #region API
    public static async Task Authenticate()
    {
        var client = new HttpClient();
        var request = new HttpRequestMessage(HttpMethod.Post, "(api)");
        var content = new StringContent("{  \"username\": \"usuario\",\r\n  \"password\": \"contraseña\"\r\n}", null, "application/json");
        request.Content = content;
        var response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();
        autenticador = JsonSerializer.Deserialize<Authenticate>(await response.Content.ReadAsStringAsync(), GetOptions());
    }
    public static async Task Categories()
    {
        var client = new HttpClient();
        var request = new HttpRequestMessage(HttpMethod.Get, "(api)");
        request.Headers.Add("Authorization", $"Bearer {autenticador.Token}");
        var content = new StringContent(string.Empty);
        content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        request.Content = content;
        var response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();
        File.WriteAllText("Assets/categorias.json", await response.Content.ReadAsStringAsync());
    }

    public static async Task Products()
    {
        var client = new HttpClient();
        var request = new HttpRequestMessage(HttpMethod.Get, "(api)");
        request.Headers.Add("Authorization", $"Bearer {autenticador.Token}");
        var content = new StringContent(string.Empty);
        content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        request.Content = content;
        var response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();
        File.WriteAllText("Assets/productos.json", await response.Content.ReadAsStringAsync());
    }
    public static async Task Price()
    {
        var client = new HttpClient();
        var request = new HttpRequestMessage(HttpMethod.Post, "(api)");
        request.Headers.Add("Authorization", $"Bearer {autenticador.Token}");
        var content = new StringContent("{\"productsIds\": []}", null, "application/json");
        //var content = new StringContent($"{{\r\n    \"productsIds\": [\r\n     {productId}\r\n    ]\r\n}}", null, "application/json");
        request.Content = content;
        var response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();
        File.WriteAllText("Assets/precio.json", await response.Content.ReadAsStringAsync());
    }
    #endregion
    #region CrearCsv
    public static void CrearCsvCategorias()
    {
        var categorias = JsonSerializer.Deserialize<List<Categories>>(File.ReadAllText("Assets/categorias.json").Replace(';', ':'), GetOptions());
        if (categorias != null)
        {
            string csv = $"categoryId;categoryName;parentCategoryId\n";
            foreach (var item in categorias)
                csv += $"{item.CategoryId};{item.CategoryName};{item.ParentCategoryId}\n";

            File.WriteAllText("Resultados/categorias.csv", csv[..^1]);
        }

    }

    public static void CrearCsvProductos()
    {
        var productos = JsonSerializer.Deserialize<List<Product>>(File.ReadAllText("Assets/productos.json").Replace(';', ':'), GetOptions());
        var precios = JsonSerializer.Deserialize<List<Prices>>(File.ReadAllText("Assets/precio.json").Replace(';', ':'), GetOptions());
        if (productos != null && precios != null)
        {
            string csv = File.ReadAllLines("Assets/Proveedor.csv")[0] + "\n";
            //string csv = $"ARTICULO;IMAGEN;PESO;IVA;PRECIOCANO;TIPCANON;COMPRA;VENTA;FAMILIA;MARCA;DENOMINA;URL;STOCK;PARTNUMBER;CODIGOBAR;DESCEXTRTF;EAN;CATEGORIA_ID;SUBCATEGORIA_ID;SUBCATEGORIADEPRODUCTO;CODIGO_MAYORISTA;CODIGO_FABRICANTE;VOLUMEN;SHORTDESC;LONGDESC;HighPic;LowPic;ThumbPic\n";

            foreach (var p in productos)
            {
                const int IVA = 21;
                double precioCano = 0;
                string tipCanon = string.Empty, compra = string.Empty, venta = string.Empty;
                foreach (var precio in precios)
                {
                    if (p.ProductId == precio.ProductId)
                    {
                        precioCano = precio.Canon;
                        tipCanon = precio.CanonDescription;
                        compra = precio.Price.ToString();
                        venta = precio.Pvp.ToString();
                        break;
                    }
                }
                string Thumbnail = string.Empty, LargePhoto = string.Empty, SmallPhoto = string.Empty;
                if (p.MainImage != null && p.MainImage.Length > 0)
                {
                    Thumbnail = p.MainImage[0].Thumbnail!;
                    LargePhoto = p.MainImage[0].LargePhoto!;
                    SmallPhoto = p.MainImage[0].SmallPhoto!;
                }
                double? Weight = 0, volume = 0;
                if (p.Logistics != null && p.Logistics.Length > 0)
                {
                    Weight = p.Logistics[0].Weight!;
                    volume = p.Logistics[0].Volume!;
                }
                string ShortSummary = string.Empty, LongDescription = string.Empty, ShortDescription = string.Empty;
                if (p.MarketingText != null && p.MarketingText.Length > 0)
                {
                    ShortSummary = p.MarketingText[0].ShortSummary!;
                    LongDescription = p.MarketingText[0].LongDescription!;
                    ShortDescription = p.MarketingText[0].ShortDescription!;
                }
                //string productId = p.ProductId ?? string.Empty;
                csv += $"{p.ProductId};{Thumbnail};{Weight:0.00};{IVA};{precioCano:0.00};{tipCanon};{compra:0.00};{venta:0.00};{p.Category};{p.Manufacturer};{p.Name};{p.Url};{p.Stock};{p.ManufacturerCode};;{ShortSummary};{p.Ean};{p.CategoryId};{p.SubCategoryId};{p.SubCategory};{p.ProductId};{p.ManufacturerCode};{volume:0.00};{ShortDescription};{LongDescription};{LargePhoto};{SmallPhoto};{Thumbnail}".Replace("\n", "\\n") + "\n";
            }
            File.WriteAllText("Resultados/Productos.csv", csv[..^1]);
        }
        else
            Console.WriteLine("No se pudo deserializar el JSON de productos o precios");
    }
    #endregion
}