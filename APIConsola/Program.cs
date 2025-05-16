using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace APIConsola;

class Program
{
    public static readonly string[] Proveedores = ["Proveedor", "prueba"];
    static bool PreguntaDeSeguridad(string pregunta)
    {
        Console.Write(pregunta);
        var respuesta = Console.ReadKey()!;
        if (respuesta.KeyChar == 's')
            return true;
        else if (respuesta.KeyChar == 'n')
            return false;
        else
        {
            Console.WriteLine("Respuesta no válida");
            return PreguntaDeSeguridad(pregunta);
        }
    }
    static async Task ElegirOpcionProveedor(string op)
    {
        switch (op)
        {
            case "1":
                await Proveedor.Authenticate();
                await Proveedor.Categories();
                Proveedor.CrearCsvCategorias();
                Console.WriteLine("CSV de categorias creado");
                break;
            case "2":
                if (PreguntaDeSeguridad("¿Quieres generar el JSON de productos? (s/n): "))
                {
                    Console.WriteLine("Este proceso puede tardar un tiempo, por favor espera...");
                    await Proveedor.Authenticate();
                    await Proveedor.Products();
                    Console.WriteLine("JSON de productos creado");
                    if (PreguntaDeSeguridad("¿Quieres generar el CSV de productos? (s/n): "))
                    {
                        Proveedor.CrearCsvProductos();
                        Console.WriteLine("CSV de productos creado");
                    }
                }
                break;
            case "3":
                if (PreguntaDeSeguridad("¿Quieres generar el JSON de precios? (s/n): "))
                {
                    await Proveedor.Authenticate();
                    await Proveedor.Price();
                    Console.WriteLine("JSON de precios creado");
                }
                break;
            case "4":
                Console.WriteLine("Este proceso puede tardar un tiempo, por favor espera...");
                Proveedor.CrearCsvProductos();
                Console.WriteLine("CSV de productos creado");
                break;
            default:
                Console.WriteLine("Opción no válida");
                break;
        }
    }
    static async Task ElegirProveedor(string[] opciones)
    {
        switch (opciones[1].ToLower())
        {
            case "Proveedor":
                await ElegirOpcionProveedor(opciones[0]);
                break;
            default:
                Console.WriteLine("Proveedor no válido");
                break;
        }
    }

    static async Task Main(string[] args)
    {
        Console.Clear();
        if (args.Length == 2)
        {
            await ElegirProveedor(args);
        }
        else
        {
            Menu.MostrarMenu();
            int op = Menu.ControlMenu();
            switch (op - 1)
            {
                case 0:
                    string mensaje = "Argumentos no válidos debe haber dos argumentos (Opción y proovedor), opciones de Proovedor:";
                    Console.WriteLine(mensaje + "\n" +
                    new string('-', mensaje.Length) + "\n" +
                    "1: Crear CSV categorias.\n" +
                    "2: Generar JSON productos.\n" +
                    "3: Generar JSON precios.\n" +
                    "4: Crear CSV de productos.\n\n" +
                    $"Proveedores: {string.Join(", ", Proveedores)}\n" +
                    new string('-', 18) + "\n" +
                    "| Ejemplo: 1 Proveedor |\n" +
                    new string('-', 18));
                    break;
                default:
                    Console.Clear();
                    break;
            }

        }
    }
}
