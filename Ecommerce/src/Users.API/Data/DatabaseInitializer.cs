using Dapper;
using Microsoft.Data.Sqlite;

namespace Users.API.Data
{
    public class DatabaseInitializer
    {
        private readonly IConfiguration _config;

        public DatabaseInitializer(IConfiguration config)
        {
            _config = config;
        }

        public void Initialize()
        {
            string connectionString = _config.GetConnectionString("DefaultConnection")
                ?? "Data Source=users.db";

            using var connection = new SqliteConnection(connectionString);
            connection.Open();

            connection.Execute("""
                CREATE TABLE IF NOT EXISTS users (
                    Id TEXT PRIMARY KEY,
                    Nombre TEXT NOT NULL,
                    Apellido TEXT NOT NULL,
                    Email TEXT NOT NULL UNIQUE,
                    PasswordHash TEXT NOT NULL,
                    FechaRegistro TEXT NOT NULL,
                    Activo INTEGER NOT NULL,
                    IntentosFallidos INTEGER NOT NULL
                );
                """);
        }
    }
}
