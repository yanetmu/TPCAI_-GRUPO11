
using Microsoft.AspNetCore.Mvc;
using Products.API.models;
using Products.API.service;

namespace Products.API.Controllers
{
    [ApiController]
    [Route("api/products")]
    public class ProductsController : ControllerBase
    {
        private readonly ProductService _productService;

        public ProductsController()
        {
            _productService = new ProductService();
        }

        // GET: api/products
        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_productService.ObtenerTodos());
        }

        // GET: api/products/{id}
        [HttpGet("{id:guid}")]
        public IActionResult GetById(Guid id)
        {
            Product? producto = _productService.ObtenerPorId(id);

            if (producto == null)
            {
                return NotFound();
            }

            return Ok(producto);
        }
    }
}

