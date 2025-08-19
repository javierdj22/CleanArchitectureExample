using MyApp.Domain.Entities;

namespace MyApp.Domain.Repositories
{
    public interface IProductoRepository
    {
        Task<IEnumerable<Producto>> GetProductosAsync();
        Task<Producto> GetProductoByIdAsync(int id);
        Task<Producto> AddProductoAsync(Producto producto);
        Task<Producto> UpdateProductoAsync(Producto producto);
        Task<Producto> DeleteProductoAsync(int id);
    }
}
