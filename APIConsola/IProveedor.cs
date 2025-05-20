namespace APIConsola;
public interface IProveedor
{
    static abstract Task Authenticate();
    static abstract Task Categories();
    static abstract void CrearCsvCategorias();
    static abstract Task CrearCsvProductos();
    static abstract Task Price();
    static abstract Task Products();
}