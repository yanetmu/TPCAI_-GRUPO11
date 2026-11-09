using Cart.API.DTOs;
using Cart.API.Services;
using Microsoft.AspNetCore.Mvc;
using Carrito = global::Cart.API.Models.Cart;
using Cart.API.Exceptions;


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

        /// <summary>
        /// Obtiene el carrito de un usuario.
        /// </summary>
        /// <param name="userId">Identificador del usuario.</param>
        /// <returns>El carrito del usuario.</returns>
        [HttpGet("{userId:guid}")]
        [ProducesResponseType(typeof(Carrito), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult ObtenerCarrito(Guid userId)
        {
            Carrito? carrito = cartService.ObtenerCarrito(userId);

            if (carrito == null)
            {
                throw new NotFoundException("Carrito no encontrado.");
            }

            return Ok(carrito);
        }

        /// <summary>
        /// Agrega un producto al carrito de un usuario.
        /// </summary>
        /// <param name="userId">Identificador del usuario.</param>
        /// <param name="request">Producto y cantidad que se desean agregar.</param>
        /// <returns>El carrito actualizado.</returns>
        [HttpPost("{userId:guid}/items")]
        [ProducesResponseType(typeof(Carrito), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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

        /// <summary>
        /// Actualiza la cantidad de un producto del carrito.
        /// </summary>
        /// <param name="userId">Identificador del usuario.</param>
        /// <param name="productId">Identificador del producto.</param>
        /// <param name="request">Nueva cantidad del producto.</param>
        /// <returns>El carrito actualizado.</returns>
        [HttpPut("{userId:guid}/items/{productId:guid}")]
        [ProducesResponseType(typeof(Carrito), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

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

        /// <summary>
        /// Elimina un producto del carrito de un usuario.
        /// </summary>
        /// <param name="userId">Identificador del usuario.</param>
        /// <param name="productId">Identificador del producto que se desea eliminar.</param>
        /// <returns>Una respuesta sin contenido si la eliminación es correcta.</returns>
        [HttpDelete("{userId:guid}/items/{productId:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult EliminarProducto(Guid userId, Guid productId)
        {
            cartService.EliminarProducto(userId, productId);

            return NoContent();
        }

        /// <summary>
        /// Elimina el carrito completo de un usuario.
        /// </summary>
        /// <param name="userId">Identificador del usuario.</param>
        /// <returns>Una respuesta sin contenido si la eliminación es correcta.</returns>
        [HttpDelete("{userId:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult EliminarCarrito(Guid userId)
        {
            cartService.EliminarCarrito(userId);

            return NoContent();
        }


    }
}
