using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyApp.Domain.Entities;

namespace MyApp.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ProductosController : ControllerBase
    {
        private readonly IProductoService _productoService;

        public ProductosController(IProductoService productoService)
        {
            _productoService = productoService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var productos = await _productoService.ObtenerProductosAsync();
                if (productos == null || !productos.Any())
                {
                    return NotFound(new { message = "No se encontraron productos." });
                }
                return Ok(productos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener los productos.", detail = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var producto = await _productoService.ObtenerPorIdAsync(id);
                if (producto == null)
                {
                    return NotFound(new { message = "Producto no encontrado." });
                }
                return Ok(producto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener el producto.", detail = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Producto producto)
        {
            try
            {
                if (producto == null)
                {
                    return BadRequest(new { message = "Los datos del producto son inválidos." });
                }

                if (string.IsNullOrEmpty(producto.Name) || producto.Price <= 0)
                {
                    return BadRequest(new { message = "El nombre del producto y el precio son obligatorios y deben ser válidos." });
                }

                var createdProducto = await _productoService.CrearProductoAsync(producto);
                return CreatedAtAction(nameof(GetById), new { id = createdProducto.Id }, createdProducto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al crear el producto.", detail = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Producto producto)
        {
            try
            {
                if (id != producto.Id)
                {
                    return BadRequest(new { message = "El ID proporcionado no coincide con el ID del producto." });
                }

                var updatedProducto = await _productoService.ActualizarProductoAsync(producto);
                if (updatedProducto == null)
                {
                    return NotFound(new { message = "Producto no encontrado." });
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al actualizar el producto.", detail = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var deletedProducto = await _productoService.EliminarProductoAsync(id);
                if (deletedProducto == null)
                {
                    return NotFound(new { message = "Producto no encontrado." });
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al eliminar el producto.", detail = ex.Message });
            }
        }
    }
}
