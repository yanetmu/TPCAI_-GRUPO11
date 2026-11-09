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

            Carrito? carritoExistente = repositorio.ObtenerPorUsuario(usuarioId);

            int cantidadActual = 0;

            if (carritoExistente != null)
            {
                foreach (CartItem item in carritoExistente.Items)
                {
                    if (item.ProductoId == productoId)
                    {
                        cantidadActual = item.Cantidad;
                    }
                }
            }

            if (cantidadActual + cantidad > producto.Stock)
            {
                throw new InvalidOperationException("CRT-003: Stock insuficiente.");
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

        public async Task ActualizarProducto(Guid usuarioId, Guid productoId, int cantidad)
        {
            if (cantidad <= 0)
            {
                throw new ArgumentException("CRT-004: La cantidad debe ser mayor a cero.");
            }

            Carrito? carrito = repositorio.ObtenerPorUsuario(usuarioId);

            if (carrito == null)
            {
                throw new KeyNotFoundException("CRT-001: Carrito no encontrado.");
            }

            CartItem? itemEncontrado = null;

            foreach (CartItem item in carrito.Items)
            {
                if (item.ProductoId == productoId)
                {
                    itemEncontrado = item;
                }
            }

            if (itemEncontrado == null)
            {
                throw new KeyNotFoundException("CRT-001: Producto no encontrado en el carrito.");
            }

            ProductInfo? producto = await productService.ObtenerProducto(productoId);

            if (producto == null)
            {
                throw new KeyNotFoundException("CRT-002: Producto no encontrado.");
            }

            if (cantidad > producto.Stock)
            {
                throw new InvalidOperationException("CRT-003: Stock insuficiente.");
            }

            itemEncontrado.Cantidad = cantidad;
            carrito.FechaActualizacion = DateTime.UtcNow;

            repositorio.Guardar(carrito);
        }

        public void EliminarProducto(Guid usuarioId, Guid productoId)
        {
            Carrito? carrito = repositorio.ObtenerPorUsuario(usuarioId);

            if (carrito == null)
            {
                throw new KeyNotFoundException("CRT-001: Carrito no encontrado.");
            }

            CartItem? itemEncontrado = null;

            foreach (CartItem item in carrito.Items)
            {
                if (item.ProductoId == productoId)
                {
                    itemEncontrado = item;
                }
            }

            if (itemEncontrado == null)
            {
                throw new KeyNotFoundException("CRT-001: Producto no encontrado en el carrito.");
            }

            carrito.Items.Remove(itemEncontrado);
            carrito.FechaActualizacion = DateTime.UtcNow;

            repositorio.Guardar(carrito);
        }

        public void EliminarCarrito(Guid usuarioId)
        {
            Carrito? carrito = repositorio.ObtenerPorUsuario(usuarioId);

            if (carrito == null)
            {
                throw new KeyNotFoundException("CRT-001: Carrito no encontrado.");
            }

            repositorio.Eliminar(usuarioId);
        }



    }
}
