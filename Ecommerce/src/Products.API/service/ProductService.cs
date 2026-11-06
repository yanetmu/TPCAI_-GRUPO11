using Products.API.Data;
using Products.API.models;

namespace Products.API.service
{
    public class ProductService
    {

        private ProductRepository _repository;

        public ProductService(ProductRepository repository)
        {
            _repository = repository;
        }

        public List<Product> ObtenerTodos()
        {
            return _repository.GetAll();
        }

        public Product ObtenerPorId(Guid id)
        {
            return _repository.GetById(id);
        }

        public bool Agregar(Product producto)
        {
            List<Product> productos = _repository.GetAll();

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

            _repository.Add(producto);

            return true;
        }


        public bool Actualizar(Guid id, Product producto)
        {
            Product productoExistente = ObtenerPorId(id);

            if (productoExistente == null)
            {
                return false;
            }

            productoExistente.Nombre = producto.Nombre;
            productoExistente.Descripcion = producto.Descripcion;
            productoExistente.Precio = producto.Precio;
            productoExistente.Stock = producto.Stock;
            productoExistente.Categoria = producto.Categoria;

            _repository.Update(productoExistente);

            return true;
        }



        /*
        public bool Actualizar(Guid id, Product producto)
        {
            Product productoExistente = ObtenerPorId(id);

            if (productoExistente == null)
            {
                return false;
            }

            productoExistente.Nombre = producto.Nombre;
            productoExistente.Descripcion = producto.Descripcion;
            productoExistente.Precio = producto.Precio;
            productoExistente.Stock = producto.Stock;
            productoExistente.Categoria = producto.Categoria;

            _repository.Add(productoExistente);

            return true;
        }
        */

        public bool Eliminar(Guid id)
        {
            Product producto = ObtenerPorId(id);

            if (producto == null)
            {
                return false;
            }

            _repository.Delete(id);

            return true;
        }
    }





















    

}  


