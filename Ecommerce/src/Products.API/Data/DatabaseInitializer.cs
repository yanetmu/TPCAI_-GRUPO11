using Microsoft.Data.Sqlite;

namespace Products.API.Data;

public class DatabaseInitializer
{
    private readonly string _connectionString;

    public DatabaseInitializer(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? "Data Source=products.db";
    }

    public void Initialize()
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = @"
            CREATE TABLE IF NOT EXISTS Products (
                Id TEXT PRIMARY KEY,
                Nombre TEXT NOT NULL,
                Descripcion TEXT,
                Precio REAL NOT NULL,
                Stock INTEGER NOT NULL,
                Categoria TEXT NOT NULL,
                FechaCreacion TEXT NOT NULL
            );";

        command.ExecuteNonQuery();
    }
}








