using Cart.API.DTOs;
using Cart.API.Services;
using Microsoft.AspNetCore.Mvc;
using Carrito = global::Cart.API.Models.Cart;

namespace Cart.API.Controllers
{
    [ApiController]
    [Route("api/cart")]
    public class CartController : ControllerBase
    {
        private CartService cartService;

        public CartController(CartService cartService)
        {
            this.cartService = cartService;
        }

        [HttpGet("{userId:guid}")]
        public IActionResult ObtenerCarrito(Guid userId)
        {
            Carrito? carrito = cartService.ObtenerCarrito(userId);

            if (carrito == null)
            {
                return NotFound();
            }

            return Ok(carrito);
        }

        [HttpPost("{userId:guid}/items")]
        public async Task<IActionResult> AgregarProducto(
            Guid userId,
            [FromBody] AddCartItemRequest request)
        {
            await cartService.AgregarProducto(
                userId,
                request.ProductoId,
                request.Cantidad);

            Carrito? carrito = cartService.ObtenerCarrito(userId);

            return Ok(carrito);
        }


        [HttpPut("{userId:guid}/items/{productId:guid}")]
        public async Task<IActionResult> ActualizarProducto(
            Guid userId,
            Guid productId,
            [FromBody] UpdateCartItemRequest request)
        {
            await cartService.ActualizarProducto(
                userId,
                productId,
                request.Cantidad);

            Carrito? carrito = cartService.ObtenerCarrito(userId);

            return Ok(carrito);
        }

        [HttpDelete("{userId:guid}/items/{productId:guid}")]
        public IActionResult EliminarProducto(Guid userId, Guid productId)
        {
            cartService.EliminarProducto(userId, productId);

            return NoContent();
        }

        [HttpDelete("{userId:guid}")]
        public IActionResult EliminarCarrito(Guid userId)
        {
            cartService.EliminarCarrito(userId);

            return NoContent();
        }


    }
}
