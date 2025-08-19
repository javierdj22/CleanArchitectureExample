using MyApp.Domain.Entities;
using MyApp.Domain.Repositories;
using System;
using System.Threading.Tasks;

namespace MyApp.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly ITokenService _tokenService;

        public AuthService(IUsuarioRepository usuarioRepository, ITokenService tokenService)
        {
            _usuarioRepository = usuarioRepository ?? throw new ArgumentNullException(nameof(usuarioRepository));
            _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
        }

        // Login: Verifica las credenciales y genera un token
        public async Task<string> Login(string username, string passwordHash)
        {
            // Espera el resultado de la tarea asincrónica
            var user = await _usuarioRepository.GetByUsernameAndPassword(username, passwordHash);

            // Si el usuario no se encuentra o las credenciales son incorrectas
            if (user == null)
                return null;

            // Genera el token después de haber obtenido al usuario
            return _tokenService.GenerateToken(user);
        }

        // Register: Registra un nuevo usuario
        public async Task<Usuario> RegisterAsync(Usuario usuario)
        {
            if (await _usuarioRepository.ExistsAsync(usuario.Username))
                return null;

            return await _usuarioRepository.AddAsync(usuario);
        }

        // Obtener usuario por ID
        public async Task<Usuario> GetUserByIdAsync(int id)
        {
            return await _usuarioRepository.GetByIdAsync(id);
        }
    }
}
