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
    





        public bool Actualizar(Guid id, Product producto)
        {
            Product? productoExistente = ObtenerPorId(id);

            if (productoExistente == null)
            {
                return false;
            }

            productoExistente.Nombre = producto.Nombre;
            productoExistente.Descripcion = producto.Descripcion;
            productoExistente.Precio = producto.Precio;
            productoExistente.Stock = producto.Stock;
            productoExistente.Categoria = producto.Categoria;

            return true;
        }

        public bool Eliminar(Guid id)
        {
            Product? producto = ObtenerPorId(id);

            if (producto == null)
            {
                return false;
            }

            productos.Remove(producto);

            return true;
        }


    }  
}   

