
using MyApp.Domain.Entities;

public interface IProductoService
{
    Task<IEnumerable<Producto>> ObtenerProductosAsync();
    Task<Producto> ObtenerPorIdAsync(int id);
    Task<Producto> CrearProductoAsync(Producto producto);
    Task<Producto> ActualizarProductoAsync(Producto producto);
    Task<Producto> EliminarProductoAsync(int id);
}
