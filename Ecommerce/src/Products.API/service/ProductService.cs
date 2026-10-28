using Products.API.models;

namespace Products.API.service
{
    public class ProductService
    {
        private List<Product> productos = new List<Product>();

        public List<Product> ObtenerTodos()
        {
            return productos;
        }

        public Product? ObtenerPorId(Guid id)
        {
            foreach (Product producto in productos)
            {
                if (producto.Id == id)
                {
                    return producto;
                }
            }

            return null;
        }

        public bool Agregar(Product producto)
        {
            foreach (Product productoExistente in productos)
            {
                if (productoExistente.Nombre == producto.Nombre &&
                    productoExistente.Categoria == producto.Categoria)
                {
                    return false;
                }
            }

            producto.Id = Guid.NewGuid();
            producto.FechaCreacion = DateTime.UtcNow;

            productos.Add(producto);

            return true;
        }
    }
}
    

