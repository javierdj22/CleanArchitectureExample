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
            var productos = await _productoService.ObtenerProductosAsync();
            return Ok(productos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var producto = await _productoService.ObtenerPorIdAsync(id);
            return Ok(producto);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Producto producto)
        {
            if (producto == null) {
                return BadRequest(new { message = "Los datos del producto son inválidos." });
            }

            if (string.IsNullOrEmpty(producto.Name) || producto.Price <= 0) {
                return BadRequest(new { message = "El nombre del producto y el precio son obligatorios y deben ser válidos." });
            }

            var createdProducto = await _productoService.CrearProductoAsync(producto);
            return CreatedAtAction(nameof(GetById), new { id = createdProducto.Id }, createdProducto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Producto producto)
        {
            if (id != producto.Id) {
                return BadRequest(new { message = "El ID proporcionado no coincide con el ID del producto." });
            }

            var updatedProducto = await _productoService.ActualizarProductoAsync(producto);
            if (updatedProducto == null) {
                return NotFound(new { message = "Producto no encontrado." });
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deletedProducto = await _productoService.EliminarProductoAsync(id);
            if (deletedProducto == null) {
                return NotFound(new { message = "Producto no encontrado." });
            }

            return NoContent();
        }
    }
}
