using Orders.API.Models;
using System.Net;
using System.Net.Http.Json;
using Orders.API.DTOs;
using Orders.API.Exceptions;

namespace Orders.API.Services
{
    public class OrderService
    {
        private readonly List<Order> _orders = new List<Order>();

        private readonly IHttpClientFactory _httpClientFactory;

        public OrderService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<Order> Crear(CreateOrderRequest request)
        {
            if (request.UsuarioId == Guid.Empty ||
    request.Items == null ||
    request.Items.Count == 0)
            {
                throw new OrderException("ORD-002", "Los datos de la orden son inválidos.", 400);
            }

            foreach (OrderItemRequest item in request.Items)
            {
                if (item.ProductoId == Guid.Empty || item.Cantidad <= 0)
                {
                    throw new OrderException("ORD-002", "Los datos de la orden son inválidos.", 400);
                }
            }

            HttpClient cliente = _httpClientFactory.CreateClient();

            HttpResponseMessage respuestaUsuario =
                await cliente.GetAsync($"https://localhost:7191/api/users/{request.UsuarioId}");

            if (respuestaUsuario.StatusCode == HttpStatusCode.NotFound)
            {
                throw new OrderException("ORD-003", "Usuario no encontrado al crear la orden.", 404);
            }

            respuestaUsuario.EnsureSuccessStatusCode();

            Order orden = new Order();
            orden.Id = Guid.NewGuid();
            orden.UsuarioId = request.UsuarioId;
            orden.Estado = "Pendiente";
            orden.FechaCreacion = DateTime.UtcNow;

            foreach (OrderItemRequest item in request.Items)
            {
                HttpResponseMessage respuestaProducto =
    await cliente.GetAsync(
        $"https://localhost:7223/api/products/{item.ProductoId}");

                if (respuestaProducto.StatusCode == HttpStatusCode.NotFound)
                {
                    throw new OrderException("ORD-004", "Producto no encontrado al crear la orden.", 404);
                }

                respuestaProducto.EnsureSuccessStatusCode();

                ProductResponse? producto =
                    await respuestaProducto.Content.ReadFromJsonAsync<ProductResponse>();

                if (producto == null)
                {
                    throw new Exception("ORD-007: Error al obtener el producto.");
                }

                if (item.Cantidad > producto.Stock)
                {
                    throw new OrderException("ORD-005", "Stock insuficiente para uno o más productos.", 422);
                }

                OrderItem nuevoItem = new OrderItem();
                nuevoItem.ProductoId = item.ProductoId;
                nuevoItem.Cantidad = item.Cantidad;
                nuevoItem.PrecioUnitario = producto.Precio;

                orden.Items.Add(nuevoItem);
                orden.Total += producto.Precio * item.Cantidad;
            }

            _orders.Add(orden);

            return orden;
        }

        public List<Order> ObtenerTodos(Guid? usuarioId)
        {
            List<Order> resultado = new List<Order>();

            foreach (Order orden in _orders)
            {
                if (usuarioId == null || orden.UsuarioId == usuarioId)
                {
                    resultado.Add(orden);
                }
            }

            return resultado;
        }

        public Order? ObtenerPorId(Guid id)
        {
            foreach (Order orden in _orders)
            {
                if (orden.Id == id)
                {
                    return orden;
                }
            }

            return null;
        }

        public Order? ActualizarEstado(Guid id, string nuevoEstado)
        {
            Order? orden = ObtenerPorId(id);

            if (orden == null)
            {
                return null;
            }

            if (nuevoEstado != "Pendiente" &&
                nuevoEstado != "Confirmada" &&
                nuevoEstado != "Enviada" &&
                nuevoEstado != "Entregada" &&
                nuevoEstado != "Cancelada")
            {
                throw new OrderException("ORD-002", "El estado ingresado es inválido.", 400);
            }

            bool transicionValida = false;

            if (orden.Estado == "Pendiente" &&
                (nuevoEstado == "Confirmada" || nuevoEstado == "Cancelada"))
            {
                transicionValida = true;
            }

            if (orden.Estado == "Confirmada" &&
                (nuevoEstado == "Enviada" || nuevoEstado == "Cancelada"))
            {
                transicionValida = true;
            }

            if (orden.Estado == "Enviada" && nuevoEstado == "Entregada")
            {
                transicionValida = true;
            }

            if (!transicionValida)
            {
                throw new OrderException("ORD-006", "El estado de la orden no puede ser modificado.", 409);
            }

            orden.Estado = nuevoEstado;

            return orden;
        }
    }
}
