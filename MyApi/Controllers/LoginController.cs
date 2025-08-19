using Microsoft.AspNetCore.Mvc;
using MyApp.Application.Services;
using MyApp.Domain.Entities;
using System;
using System.Threading.Tasks;

namespace MyApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] Usuario request)
        {
            if (request == null || string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.PasswordHash))
            {
                return BadRequest(new { message = "Username y password son requeridos." });
            }

            string token = await _authService.Login(request.Username, request.PasswordHash);

            if (string.IsNullOrEmpty(token)) {
                return Unauthorized(new { message = "Usuario o contraseña inválidos." });
            }

            return Ok(new { token });
        }

        // Registro de Usuario
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] Usuario request)
        {
            if (request == null || string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.PasswordHash))
            {
                return BadRequest(new { message = "Username y password son requeridos." });
            }
            var createdUser = await _authService.RegisterAsync(request);

            if (createdUser == null) {
                return BadRequest(new { message = "No se pudo registrar el usuario." });
            }
            var userResponse = new
            {
                createdUser.Id,
                createdUser.Username
            };
            return CreatedAtAction(nameof(GetById), new { id = createdUser.Id }, userResponse);
        }

        // Obtener usuario por ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var user = await _authService.GetUserByIdAsync(id);
            if (user == null) {
                return NotFound(new { message = "Usuario no encontrado." });
            }

            return Ok(user); 
        }
    }
}
