using MyApp.Domain.Entities;
using Dapper;
using Microsoft.Extensions.Configuration;
using MyApp.Domain.Repositories;
using Npgsql;
using System.Data;


public class ProductoRepository : IProductoRepository
{
    private readonly IDbConnection _connection;

    public ProductoRepository(IDbConnection connection)
    {
        _connection = connection;
    }

    public async Task<IEnumerable<Producto>> GetProductosAsync()
    {
        var sql = "SELECT * FROM productos"; 
        return await _connection.QueryAsync<Producto>(sql); 
    }


    // Obtener un Producto por ID
    public async Task<Producto> GetProductoByIdAsync(int id)
    {
        var sql = "SELECT * FROM productos WHERE Id = @Id";
        return await _connection.QueryFirstOrDefaultAsync<Producto>(sql, new { Id = id });
    }

    // Crear un Producto
    public async Task<Producto> AddProductoAsync(Producto Producto)
    {
        var sql = @"INSERT INTO productos (name, price, category) 
                VALUES (@name, @price, @category) RETURNING *";

        var productoCreado = await _connection.QueryFirstOrDefaultAsync<Producto>(sql, Producto);

        return productoCreado;
    }


    // Actualizar un Producto
    public Task<Producto> UpdateProductoAsync(Producto Producto)
    {
        var sql = @"UPDATE productos SET name = @name, price = @price, category = @category 
                WHERE id = @id RETURNING *";

        var productoActualizado = _connection.QueryFirstOrDefaultAsync<Producto>(sql,
            new { Id = Producto.Id, name = Producto.Name, price = Producto.Price, category = Producto.Category });

        return productoActualizado;
    }

    // Eliminar un Producto
    public Task<Producto> DeleteProductoAsync(int id)
    {
        var sql = "DELETE FROM productos WHERE id = @Id RETURNING *";  

        var productoEliminado = _connection.QueryFirstOrDefaultAsync<Producto>(sql, new { Id = id });

        return productoEliminado;
    }
}
