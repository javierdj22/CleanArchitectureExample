using MyApp.Domain.Entities;
using System.Threading.Tasks;

namespace MyApp.Application.Services
{
    public interface IAuthService
    {
        Task<string> Login(string username, string passwordHash);
        Task<Usuario> RegisterAsync(Usuario usuario);
        Task<Usuario> GetUserByIdAsync(int id);
    }
}
