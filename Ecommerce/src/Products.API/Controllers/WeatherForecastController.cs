using Microsoft.AspNetCore.Mvc;
using Products.API.models;

namespace Products.API.Controllers
{
    [ApiController]
    [Route("api/products")]
    public class ProductsController : ControllerBase
    {
        // GET: api/products
        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok("Endpoint para obtener todos los productos");
        }

        // GET: api/products/{id}
        [HttpGet("{id:guid}")]
        public IActionResult GetById(Guid id)
        {
            return Ok("Endpoint para obtener un producto por ID");
        }
    }
}
