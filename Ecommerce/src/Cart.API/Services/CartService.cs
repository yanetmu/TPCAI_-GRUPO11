namespace Cart.API.Services
{
    public class CartService
    {
        private List<global::Cart.API.Models.Cart> carritos =
            new List<global::Cart.API.Models.Cart>();

        public global::Cart.API.Models.Cart? ObtenerCarrito(Guid usuarioId)
        {
            foreach (global::Cart.API.Models.Cart carrito in carritos)
            {
                if (carrito.UsuarioId == usuarioId)
                {
                    return carrito;
                }
            }

            return null;
        }
    }
}


