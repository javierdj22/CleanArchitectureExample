using MyApp.Domain.Entities;
using Dapper;
using Microsoft.Extensions.Configuration;
using MyApp.Domain.Repositories;
using Npgsql;
using System.Data;
using Microsoft.Extensions.DependencyInjection;


public class ProductoRepository : IProductoRepository
{
    private readonly IServiceProvider _serviceProvider;

    public ProductoRepository(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }


    public async Task<IEnumerable<Producto>> GetProductosAsync()
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var connection = scope.ServiceProvider.GetRequiredService<IDbConnection>();

            var sql = "SELECT * FROM productos";
            return await connection.QueryAsync<Producto>(sql);
        }
        catch (Exception ex)
        {
            // Aquí puedes registrar el error en un log antes de relanzar
            throw new ApplicationException("Error al obtener los productos", ex);
        }
    }


    // Obtener un Producto por ID
    public async Task<Producto> GetProductoByIdAsync(int id)
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var _connection = scope.ServiceProvider.GetRequiredService<IDbConnection>();
            var sql = "SELECT * FROM productos WHERE Id = @Id";
            return await _connection.QueryFirstOrDefaultAsync<Producto>(sql, new { Id = id });
        }
    }

    // Crear un Producto
    public async Task<Producto> AddProductoAsync(Producto Producto)
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var _connection = scope.ServiceProvider.GetRequiredService<IDbConnection>();
            var sql = @"INSERT INTO productos (name, price, category) 
                VALUES (@name, @price, @category) RETURNING *";

            var productoCreado = await _connection.QueryFirstOrDefaultAsync<Producto>(sql, Producto);

            return productoCreado;
        }
    }


    // Actualizar un Producto
    public Task<Producto> UpdateProductoAsync(Producto Producto)
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var _connection = scope.ServiceProvider.GetRequiredService<IDbConnection>();
            var sql = @"UPDATE productos SET name = @name, price = @price, category = @category 
                WHERE id = @id RETURNING *";

            var productoActualizado = _connection.QueryFirstOrDefaultAsync<Producto>(sql,
            new { Id = Producto.Id, name = Producto.Name, price = Producto.Price, category = Producto.Category });

            return productoActualizado;
        }
    }

    // Eliminar un Producto
    public Task<Producto> DeleteProductoAsync(int id)
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var _connection = scope.ServiceProvider.GetRequiredService<IDbConnection>();
            var sql = "DELETE FROM productos WHERE id = @Id RETURNING *";  
            var productoEliminado = _connection.QueryFirstOrDefaultAsync<Producto>(sql, new { Id = id });

            return productoEliminado;
        }
    }
}
