using MyApp.Domain.Entities;
using System.Threading.Tasks;

namespace MyApp.Domain.Repositories
{
    public interface IUsuarioRepository
    {
        Task<Usuario> AddAsync(Usuario usuario);
        Task<Usuario> GetByIdAsync(int id);
        Task<Usuario> GetByUsernameAndPassword(string username, string passwordHash);
        Task<bool> ExistsAsync(string username);
    }
}
