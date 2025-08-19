using MyApp.Domain.Entities;
using MyApp.Domain.Repositories;

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

        // Lister: lista y valida un usuario
        public async Task<string> Login(string username, string passwordHash)
        {
            passwordHash = EncodeBase64(passwordHash);
            var user = await _usuarioRepository.GetByUsernameAndPassword(username, passwordHash);

            if (user == null)
                return null;

            return _tokenService.GenerateToken(user);
        }

        // Register: Registra un nuevo usuario
        public async Task<Usuario> RegisterAsync(Usuario usuario)
        {
            if (await _usuarioRepository.ExistsAsync(usuario.Username))
                return null;

            usuario.PasswordHash = EncodeBase64(usuario.PasswordHash);
            return await _usuarioRepository.AddAsync(usuario);
        }

        // Obtener usuario por ID
        public async Task<Usuario> GetUserByIdAsync(int id)
        {
            return await _usuarioRepository.GetByIdAsync(id);
        }

        private static string EncodeBase64(string plainText)
        {
            byte[] plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }
    }
}
