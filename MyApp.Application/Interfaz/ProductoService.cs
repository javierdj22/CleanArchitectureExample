using MyApp.Domain.Entities;
using MyApp.Domain.Repositories;

namespace MyApp.Application.Services
{
    public class ProductoService : IProductoService
    {
        private readonly IProductoRepository _productoRepository;

        public ProductoService(IProductoRepository productoRepository)
        {
            _productoRepository = productoRepository;
        }

        public async Task<IEnumerable<Producto>> ObtenerProductosAsync()
        {
            return await _productoRepository.GetProductosAsync();
        }

        public async Task<Producto> ObtenerPorIdAsync(int id)
        {
            return await _productoRepository.GetProductoByIdAsync(id);
        }

        public async Task<Producto> CrearProductoAsync(Producto producto)
        {
            return await _productoRepository.AddProductoAsync(producto);
        }

        public async Task<Producto> ActualizarProductoAsync(Producto producto)
        {
            return await _productoRepository.UpdateProductoAsync(producto);
        }

        public async Task<Producto> EliminarProductoAsync(int id)
        {
            return await _productoRepository.DeleteProductoAsync(id);
        }

    }
}
