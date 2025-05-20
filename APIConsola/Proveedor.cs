using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
namespace APIConsola;

class Proveedor : IProveedor
{
    public static Authenticate autenticador;
    static JsonSerializerOptions Opciones()
    {
        return new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
    }
    #region API
    public static async Task Authenticate()
    {
        Log.StartLogging();
        var client = new HttpClient();
        var request = new HttpRequestMessage(HttpMethod.Post, "(api)");
        var content = new StringContent("{  \"username\": \"usuario\",\r\n  \"password\": \"contraseña\"\r\n}", null, "application/json");
        request.Content = content;
        var response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();
        autenticador = JsonSerializer.Deserialize<Authenticate>(await response.Content.ReadAsStringAsync(), Opciones());
        Log.StopLogging("Autenticando");
    }
    public static async Task Categories()
    {
        Log.StartLogging();
        var client = new HttpClient();
        var request = new HttpRequestMessage(HttpMethod.Get, "(api)");
        request.Headers.Add("Authorization", $"Bearer {autenticador.Token}");
        var content = new StringContent(string.Empty);
        content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        request.Content = content;
        var response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();
        File.WriteAllText("Assets/categorias.json", await response.Content.ReadAsStringAsync());
        Log.StopLogging("Escribiendo JSON de categorias");
    }

    public static async Task Products()
    {
        Log.StartLogging();
        var client = new HttpClient();
        var request = new HttpRequestMessage(HttpMethod.Get, "(api)");
        request.Headers.Add("Authorization", $"Bearer {autenticador.Token}");
        var content = new StringContent(string.Empty);
        content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        request.Content = content;
        var response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();
        File.WriteAllText("Assets/productos.json", await response.Content.ReadAsStringAsync());
        Log.StopLogging("Escribiendo JSON de productos");
    }
    public static async Task Price()
    {
        Log.StartLogging();
        var client = new HttpClient();
        var request = new HttpRequestMessage(HttpMethod.Post, "(api)");
        request.Headers.Add("Authorization", $"Bearer {autenticador.Token}");
        var content = new StringContent("{\"productsIds\": []}", null, "application/json");
        //var content = new StringContent($"{{\r\n    \"productsIds\": [\r\n     {productId}\r\n    ]\r\n}}", null, "application/json");
        request.Content = content;
        var response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();
        File.WriteAllText("Assets/precio.json", await response.Content.ReadAsStringAsync());
        Log.StopLogging("Escribiendo JSON de precios");
    }
    #endregion
    #region CrearCsv
    public static void CrearCsvCategorias()
    {
        Log.StartLogging();
        var categorias = JsonSerializer.Deserialize<List<Categories>>(File.ReadAllText("Assets/categorias.json").Replace(';', ':'), Opciones());
        if (categorias != null)
        {
            var sb = new StringBuilder();
            sb.Append("categoryId;categoryName;parentCategoryId\n");
            foreach (var item in categorias)
                sb.Append($"{item.CategoryId};{item.CategoryName};{item.ParentCategoryId}\n");

            File.WriteAllText("Resultados/categorias.csv", sb.ToString()[..^1]);
            Log.StopLogging("Escribiendo CSV de categorias");
        }

    }

    public static async Task CrearCsvProductos()
    {
        const int IVA = 21;

        Log.StartLogging();
        using var productosStream = File.OpenRead("Assets/productos.json");
        using var preciosStream = File.OpenRead("Assets/precio.json");

        var productos = JsonSerializer.DeserializeAsyncEnumerable<Product>(productosStream, Opciones());
        var precios = JsonSerializer.DeserializeAsyncEnumerable<Prices>(preciosStream, Opciones());

        var preciosDict = new Dictionary<string, Prices>();
        await foreach (var precio in precios)
        {
            if (!string.IsNullOrEmpty(precio.ProductId))
            preciosDict[precio.ProductId] = precio;
        }
        Log.StopLogging("Deserializando JSON de productos y precios");

        Log.StartLogging();
        var sb = new StringBuilder();
        if (File.Exists("Assets/Proveedor.csv"))
            sb.Append(File.ReadAllLines("Assets/Proveedor.csv").First() + "\n");
        else
            sb.Append($"ARTICULO;IMAGEN;PESO;IVA;PRECIOCANO;TIPCANON;COMPRA;VENTA;FAMILIA;MARCA;DENOMINA;URL;STOCK;PARTNUMBER;CODIGOBAR;DESCEXTRTF;EAN;CATEGORIA_ID;SUBCATEGORIA_ID;SUBCATEGORIADEPRODUCTO;CODIGO_MAYORISTA;CODIGO_FABRICANTE;VOLUMEN;SHORTDESC;LONGDESC;HighPic;LowPic;ThumbPic\n");

        await foreach (var p in productos)
        {
            if (p == null)
                continue;
            
            preciosDict.TryGetValue(p.ProductId ?? "", out var precio);
            var mainImage = p.MainImage?.FirstOrDefault();
            var logistics = p.Logistics?.FirstOrDefault();
            var marketing = p.MarketingText?.FirstOrDefault();

            sb.AppendLine($@"{p.ProductId};{mainImage?.Thumbnail ?? string.Empty};{logistics?.Weight ?? 0:0.00};{IVA};{precio.Canon:0.00};{precio.CanonDescription};{precio.Price:0.00};{precio.Pvp:0.00};{p.Category};{p.Manufacturer};{p.Name};{p.Url};{p.Stock};{p.ManufacturerCode};;{marketing?.ShortSummary ?? string.Empty};{p.Ean};{p.CategoryId};{p.SubCategoryId};{p.SubCategory};{p.ProductId};{p.ManufacturerCode};{p.ManufacturerCode};{logistics?.Volume ?? 0:0.00};{marketing?.ShortDescription ?? string.Empty};{marketing?.LongDescription ?? string.Empty};{mainImage?.LargePhoto ?? string.Empty};{mainImage?.SmallPhoto ?? string.Empty};{mainImage?.Thumbnail ?? string.Empty}".Replace("\n", "\\n").Replace("\r", "\\r"));
        }
        Log.StopLogging("Generando datos");

        Log.StartLogging();
        File.WriteAllText("Resultados/Productos.csv", sb.ToString().TrimEnd('\n')[..^1]);
        Log.StopLogging("Escribiendo CSV de productos");
    }
    #endregion
}