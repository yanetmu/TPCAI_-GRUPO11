using Cart.API.Data;
using Cart.API.Models;
using Carrito = global::Cart.API.Models.Cart;

namespace Cart.API.Services
{
    public class CartService
    {
        private CartRepository repositorio;
        private ProductService productService;

        public CartService(CartRepository repositorio, ProductService productService)
        {
            this.repositorio = repositorio;
            this.productService = productService;
        }


        public Carrito? ObtenerCarrito(Guid usuarioId)
        {
            return repositorio.ObtenerPorUsuario(usuarioId);
        }

        public async Task AgregarProducto(Guid usuarioId, Guid productoId, int cantidad)
        {

            if (cantidad <= 0)
            {
                throw new ArgumentException("CRT-004: La cantidad debe ser mayor a cero.");
            }
            ProductInfo? producto = await productService.ObtenerProducto(productoId);

            if (producto == null)
            {
                throw new KeyNotFoundException("CRT-002: Producto no encontrado.");
            }

            Carrito? carrito = repositorio.ObtenerPorUsuario(usuarioId);

            if (carrito == null)
            {
                carrito = new Carrito();
                carrito.UsuarioId = usuarioId;
            }

            bool encontrado = false;

            foreach (CartItem item in carrito.Items)
            {
                if (item.ProductoId == productoId)
                {
                    item.Cantidad += cantidad;
                    encontrado = true;
                }
            }

            if (encontrado == false)
            {
                CartItem nuevoItem = new CartItem();
                nuevoItem.ProductoId = productoId;
                nuevoItem.Cantidad = cantidad;

                carrito.Items.Add(nuevoItem);
            }

            carrito.FechaActualizacion = DateTime.UtcNow;

            repositorio.Guardar(carrito);
        }

    }
}
