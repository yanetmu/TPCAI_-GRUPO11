using Microsoft.Data.Sqlite;
using Products.API.models;

namespace Products.API.Data;

public class ProductRepository
{
    private readonly string _connectionString;

    public ProductRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? "Data Source=products.db";
    }

    public List<Product> GetAll()
    {
        var list = new List<Product>();
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = "SELECT Id, Nombre, Descripcion, Precio, Stock, Categoria, FechaCreacion FROM Products";

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            list.Add(new Product
            {
                Id = Guid.Parse(reader.GetString(0)),
                Nombre = reader.GetString(1),
                Descripcion = reader.IsDBNull(2) ? null : reader.GetString(2),
                Precio = reader.GetDecimal(3),
                Stock = reader.GetInt32(4),
                Categoria = reader.GetString(5),
                FechaCreacion = DateTime.Parse(reader.GetString(6))
            });
        }

        return list;
    }

    public Product? GetById(Guid id)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = "SELECT Id, Nombre, Descripcion, Precio, Stock, Categoria, FechaCreacion FROM Products WHERE Id = $id";
        command.Parameters.AddWithValue("$id", id.ToString());

        using var reader = command.ExecuteReader();
        if (reader.Read())
        {
            return new Product
            {
                Id = Guid.Parse(reader.GetString(0)),
                Nombre = reader.GetString(1),
                Descripcion = reader.IsDBNull(2) ? null : reader.GetString(2),
                Precio = reader.GetDecimal(3),
                Stock = reader.GetInt32(4),
                Categoria = reader.GetString(5),
                FechaCreacion = DateTime.Parse(reader.GetString(6))
            };
        }

        return null;
    }

    public void Add(Product product)
    {
        if (product.Id == Guid.Empty)
        {
            product.Id = Guid.NewGuid();
        }

        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = @"
            INSERT INTO Products (Id, Nombre, Descripcion, Precio, Stock, Categoria, FechaCreacion)
            VALUES ($id, $nombre, $descripcion, $precio, $stock, $categoria, $fechaCreacion)";

        command.Parameters.AddWithValue("$id", product.Id.ToString());
        command.Parameters.AddWithValue("$nombre", product.Nombre);
        command.Parameters.AddWithValue("$descripcion", (object?)product.Descripcion ?? DBNull.Value);
        command.Parameters.AddWithValue("$precio", product.Precio);
        command.Parameters.AddWithValue("$stock", product.Stock);
        command.Parameters.AddWithValue("$categoria", product.Categoria);
        command.Parameters.AddWithValue("$fechaCreacion", product.FechaCreacion.ToString("o"));

        command.ExecuteNonQuery();
    }

    public void Update(Product product)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var command = connection.CreateCommand();

        command.CommandText = @"
        UPDATE Products
        SET Nombre = $nombre,
            Descripcion = $descripcion,
            Precio = $precio,
            Stock = $stock,
            Categoria = $categoria
        WHERE Id = $id";

        command.Parameters.AddWithValue("$id", product.Id.ToString());
        command.Parameters.AddWithValue("$nombre", product.Nombre);
        command.Parameters.AddWithValue("$descripcion",
            (object?)product.Descripcion ?? DBNull.Value);
        command.Parameters.AddWithValue("$precio", product.Precio);
        command.Parameters.AddWithValue("$stock", product.Stock);
        command.Parameters.AddWithValue("$categoria", product.Categoria);

        command.ExecuteNonQuery();
    }

    public void Delete(Guid id)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = "DELETE FROM Products WHERE Id = $id";
        command.Parameters.AddWithValue("$id", id.ToString());

        command.ExecuteNonQuery();
    }
}







