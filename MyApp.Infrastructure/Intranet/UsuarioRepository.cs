using Dapper;
using MyApp.Domain.Entities;
using MyApp.Domain.Repositories;
using System.Data;
using System.Threading.Tasks;

namespace MyApp.Infrastructure.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly IDbConnection _connection;

        public UsuarioRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<Usuario> AddAsync(Usuario usuario)
        {
            var sql = "INSERT INTO usuarios (username, password_hash) VALUES (@Username, @PasswordHash) RETURNING *";
            return await _connection.QueryFirstOrDefaultAsync<Usuario>(sql, usuario);
        }

        public async Task<Usuario> GetByIdAsync(int id)
        {
            var sql = "SELECT * FROM usuarios WHERE id = @Id";
            return await _connection.QueryFirstOrDefaultAsync<Usuario>(sql, new { Id = id });
        }

        public async Task<Usuario> GetByUsernameAndPassword(string username, string passwordHash)
        {
            var sql = "SELECT * FROM usuarios WHERE username = @Username AND password_hash = @PasswordHash";
            return await _connection.QueryFirstOrDefaultAsync<Usuario>(sql, new { Username = username, PasswordHash = passwordHash });
        }

        public async Task<bool> ExistsAsync(string username)
        {
            var sql = "SELECT COUNT(1) FROM usuarios WHERE username = @Username";
            return await _connection.ExecuteScalarAsync<bool>(sql, new { Username = username });
        }
    }
}
