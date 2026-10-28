
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





        // POST: api/products
        [HttpPost]
        public IActionResult Create(Product producto)
        {
            bool agregado = _productService.Agregar(producto);

            if (!agregado)
            {
                return Conflict(new
                {
                    errorCode = "PRD-003",
                    errorMessage = "Ya existe un producto con ese nombre en esa categoría."
                });
            }

            return CreatedAtAction(
                nameof(GetById),
                new { id = producto.Id },
                producto
            );
        }








    }
}

