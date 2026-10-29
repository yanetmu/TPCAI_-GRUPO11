
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
        [HttpGet("{id:int}")]
        public IActionResult GetById(int id)
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



        // PUT: api/products/{id}
        [HttpPut("{id:int}")]
        public IActionResult Update(int id, Product producto)
        {
            bool actualizado = _productService.Actualizar(id, producto);

            if (!actualizado)
            {
                return NotFound(new
                {
                    errorCode = "PRD-001",
                    errorMessage = "No se encontró el producto."
                });
            }

            return Ok(producto);
        }


        // DELETE: api/products/{id}
        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            bool eliminado = _productService.Eliminar(id);

            if (!eliminado)
            {
                return NotFound(new
                {
                    errorCode = "PRD-001",
                    errorMessage = "No se encontró el producto."
                });
            }

            return Ok("Producto eliminado correctamente.");
        }

           

    }
}

