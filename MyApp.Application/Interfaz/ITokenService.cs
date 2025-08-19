using MyApp.Domain.Entities;

namespace MyApp.Application.Services
{
    public interface ITokenService
    {
        string GenerateToken(Usuario usuario);
    }
}
