using Microsoft.AspNetCore.Mvc;
using Orders.API.DTOs;
using Orders.API.Models;
using Orders.API.Services;
using Orders.API.Exceptions;

namespace Orders.API.Controllers
{
    [ApiController]
    [Route("api/orders")]
    [Tags("Orders")]
    public class OrdersController : ControllerBase
    {
        private readonly OrderService _orderService;

        public OrdersController(OrderService orderService)
        {
            _orderService = orderService;
        }

        /// <summary>
        /// Obtiene todas las órdenes, con filtro opcional por usuario.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<List<Order>> ObtenerTodos([FromQuery] Guid? usuarioId)
        {
            return Ok(_orderService.ObtenerTodos(usuarioId));
        }

        /// <summary>
        /// Obtiene una orden por su identificador.
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<Order> ObtenerPorId(Guid id)
        {
            Order? orden = _orderService.ObtenerPorId(id);

            if (orden == null)
            {
                throw new OrderException("ORD-001", "Orden no encontrada.", 404);
            }

            return Ok(orden);
        }

        /// <summary>
        /// Crea una orden y valida el usuario, los productos y el stock.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Order>> Crear(CreateOrderRequest request)
        {
            Order orden = await _orderService.Crear(request);

            return CreatedAtAction(
                nameof(ObtenerPorId),
                new { id = orden.Id },
                orden);
        }

        /// <summary>
        /// Actualiza el estado de una orden.
        /// </summary>
        [HttpPut("{id}/status")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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
