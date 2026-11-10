
using Dapper;
using Microsoft.Data.Sqlite;
using Users.API.Models;

namespace Users.API.Data
{
    public class UserRepository
    {
        private readonly IConfiguration _config;

        public UserRepository(IConfiguration config)
        {
            _config = config;
        }

        private SqliteConnection CreateConnection()
        {
            string connectionString = _config.GetConnectionString("DefaultConnection")
                ?? "Data Source=users.db";

            return new SqliteConnection(connectionString);
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            using var connection = CreateConnection();

            return await connection.QuerySingleOrDefaultAsync<User>(
                "SELECT * FROM users WHERE Email = @Email COLLATE NOCASE",
                new { Email = email });
        }

        public async Task<User?> GetByIdAsync(Guid id)
        {
            using var connection = CreateConnection();

            return await connection.QuerySingleOrDefaultAsync<User>(
                "SELECT * FROM users WHERE Id = @Id",
                new { Id = id });
        }

        public async Task CreateAsync(User user)
        {
            using var connection = CreateConnection();

            await connection.ExecuteAsync("""
                INSERT INTO users
                (Id, Nombre, Apellido, Email, PasswordHash,
                 FechaRegistro, Activo, IntentosFallidos)
                VALUES
                (@Id, @Nombre, @Apellido, @Email, @PasswordHash,
                 @FechaRegistro, @Activo, @IntentosFallidos)
                """, user);
        }

        public async Task UpdateAsync(User user)
        {
            using var connection = CreateConnection();

            await connection.ExecuteAsync("""
                UPDATE users
                SET Activo = @Activo,
                    IntentosFallidos = @IntentosFallidos
                WHERE Id = @Id
                """, user);
        }
    }
}
