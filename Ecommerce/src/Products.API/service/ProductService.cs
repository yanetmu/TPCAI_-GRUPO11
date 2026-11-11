using Products.API.Data;
using Products.API.models;
using System.Net.Http.Json;
using Products.API.Exceptions;

namespace Products.API.service
{
    public class ProductService
    {

        private ProductRepository _repository;
        private HttpClient _httpClient;
        private string _ordersApiUrl;

        public ProductService(
    ProductRepository repository,
    IHttpClientFactory httpClientFactory,
    IConfiguration configuration)
        {
            _repository = repository;
            _httpClient = httpClientFactory.CreateClient();
            _ordersApiUrl = configuration["Services:OrdersApiUrl"]
                ?? throw new Exception("Falta configurar OrdersApiUrl.");
        }

        public class OrdenConsulta
        {
            public string Estado { get; set; } = "";
            public List<ItemConsulta> Items { get; set; } = new List<ItemConsulta>();
        }

        public class ItemConsulta
        {
            public Guid ProductoId { get; set; }
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

        public async Task<bool> Eliminar(Guid id)
        {
            Product producto = ObtenerPorId(id);

            if (producto == null)
            {
                return false;
            }

            List<OrdenConsulta>? ordenes =
    await _httpClient.GetFromJsonAsync<List<OrdenConsulta>>(
        _ordersApiUrl + "/api/orders");

            if (ordenes == null)
            {
                throw new Exception("No se pudieron consultar las órdenes.");
            }

            foreach (OrdenConsulta orden in ordenes)
            {
                if (orden.Estado == "Pendiente" ||
                    orden.Estado == "Confirmada")
                {
                    foreach (ItemConsulta item in orden.Items)
                    {
                        if (item.ProductoId == id)
                        {
                            throw new BusinessRuleException(
                                "PRD-004",
                                "El producto tiene órdenes activas y no puede eliminarse.");
                        }
                    }
                }
            }

            _repository.Delete(id);

            return true;
        }
    }





















    

}  


