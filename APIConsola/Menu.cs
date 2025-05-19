using APIConsola;

class Menu
{
    public static void MostrarMenu()
    {
        int num = 1;
        string menu = "Seleccione un proveedor para ver sus opciones:\n\n";
        
        foreach (var proveedor in Program.Proveedores)
            menu += $"\t{num++}. {proveedor}\n";
        Console.WriteLine(menu + $"\t{Program.Proveedores.Length + 1}. Salir");
    }
    public static int ControlMenu()
    {
        int opcion = 1, anterior = 1;
        Console.SetCursorPosition(8, 2);
        ConsoleKeyInfo tecla = new();
        while (tecla.Key != ConsoleKey.Enter)
        {
            Console.BackgroundColor = ConsoleColor.DarkGray;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.Write(opcion + ".");
            Console.ResetColor();
            Console.SetCursorPosition(8, 1 + anterior);
            if (anterior != opcion)
                Console.WriteLine(anterior + ".");
            Console.SetCursorPosition(8, 1 + opcion);
            tecla = Console.ReadKey(true);
            anterior = opcion;
            if (tecla.Key == ConsoleKey.UpArrow)
            {
                if (opcion > 1)
                {
                    opcion--;
                    Console.SetCursorPosition(8, 1 + Program.Proveedores.Length - (Program.Proveedores.Length - opcion ));
                }
                else
                {
                    opcion = Program.Proveedores.Length + 1;
                    Console.SetCursorPosition(8, 2 + opcion -1);
                }
            }
            else if (tecla.Key == ConsoleKey.DownArrow)
            {
                if (opcion < Program.Proveedores.Length + 1)
                {
                    opcion++;
                    Console.SetCursorPosition(8, 2 + opcion -1);
                }
                else
                {
                    opcion = 1;
                    Console.SetCursorPosition(8, 2);
                }
            }
        }
        Console.SetCursorPosition(0,0);
        return opcion;
    }
}