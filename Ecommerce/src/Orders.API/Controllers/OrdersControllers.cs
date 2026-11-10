using Microsoft.AspNetCore.Mvc;
using Orders.API.DTOs;
using Orders.API.Models;
using Orders.API.Services;
using Orders.API.Exceptions;

namespace Orders.API.Controllers
{
    [ApiController]
    [Route("api/orders")]
    public class OrdersController : ControllerBase
    {
        private readonly OrderService _orderService;

        public OrdersController(OrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet]
        public ActionResult<List<Order>> ObtenerTodos([FromQuery] Guid? usuarioId)
        {
            return Ok(_orderService.ObtenerTodos(usuarioId));
        }

        [HttpGet("{id}")]
        public ActionResult<Order> ObtenerPorId(Guid id)
        {
            Order? orden = _orderService.ObtenerPorId(id);

            if (orden == null)
            {
                throw new OrderException("ORD-001", "Orden no encontrada.", 404);
            }

            return Ok(orden);
        }

        [HttpPost]
        public async Task<ActionResult<Order>> Crear(CreateOrderRequest request)
        {
            Order orden = await _orderService.Crear(request);

            return CreatedAtAction(
                nameof(ObtenerPorId),
                new { id = orden.Id },
                orden);
        }

        [HttpPut("{id}/status")]
        public ActionResult<Order> ActualizarEstado(
    Guid id,
    UpdateOrderStatusRequest request)
        {
            Order? orden = _orderService.ActualizarEstado(id, request.Estado);

            if (orden == null)
            {
                throw new OrderException("ORD-001", "Orden no encontrada.", 404);
            }

            return Ok(orden);
        }
    }
}