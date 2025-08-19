namespace MyApp.Domain.Entities
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; } // Puedes utilizar un hash de la contraseña
    }
}
