using Cart.API.Models;

namespace Cart.API.Services
{
    public class ProductService
    {
        private IHttpClientFactory httpClientFactory;

        public ProductService(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        public async Task<ProductInfo?> ObtenerProducto(Guid productoId)
        {
            HttpClient cliente = httpClientFactory.CreateClient();

            HttpResponseMessage respuesta = await cliente.GetAsync(
                "http://localhost:5281/api/products/" + productoId);


            if (respuesta.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }

            respuesta.EnsureSuccessStatusCode();

            ProductInfo? producto =
                await respuesta.Content.ReadFromJsonAsync<ProductInfo>();

            return producto;
        }
    }
}
