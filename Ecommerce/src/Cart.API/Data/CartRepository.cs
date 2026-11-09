using Cart.API.Models;
using Carrito = global::Cart.API.Models.Cart;

namespace Cart.API.Data
{
    public class CartRepository
    {
        private List<Carrito> carritos = new List<Carrito>();

        public Carrito? ObtenerPorUsuario(Guid usuarioId)
        {
            foreach (Carrito carrito in carritos)
            {
                if (carrito.UsuarioId == usuarioId)
                {
                    return carrito;
                }
            }

            return null;
        }

        public void Guardar(Carrito carrito)
        {
            Carrito? carritoExistente = ObtenerPorUsuario(carrito.UsuarioId);

            if (carritoExistente == null)
            {
                carritos.Add(carrito);
            }
            else
            {
                int posicion = carritos.IndexOf(carritoExistente);
                carritos[posicion] = carrito;
            }
        }

        public void Eliminar(Guid usuarioId)
        {
            Carrito? carrito = ObtenerPorUsuario(usuarioId);

            if (carrito != null)
            {
                carritos.Remove(carrito);
            }
        }


    }
}
