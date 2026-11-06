
using Products.API.Exceptions;
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

        public ProductsController(ProductService productService)
        {
            _productService = productService;
        }

        // GET: api/products

        /// <summary>
        /// Obtiene todos los productos y permite filtrarlos por categoría o nombre.
        /// </summary>
        /// <param name="categoria">Categoría del producto.</param>
        /// <param name="nombre">Nombre del producto.</param>
        /// <returns>Lista de productos.</returns>
        /// <response code="200">Lista de productos obtenida correctamente.</response>
        /// <response code="500">PRD-005: Error interno al procesar el producto.</response>
        [HttpGet]
        [ProducesResponseType(typeof(List<Product>), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public IActionResult GetAll(
            [FromQuery] string? categoria,
            [FromQuery] string? nombre)
        {
            var productos = _productService.ObtenerTodos();

            if (!string.IsNullOrWhiteSpace(categoria))
            {
                productos = productos
                    .Where(p => p.Categoria.Equals(
                        categoria,
                        StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            if (!string.IsNullOrWhiteSpace(nombre))
            {
                productos = productos
                    .Where(p => p.Nombre.Contains(
                        nombre,
                        StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            return Ok(productos);
        }

        // GET: api/products/category/{categoria}

        /// <summary>
        /// Obtiene los productos de una categoría.
        /// </summary>
        /// <param name="categoria">Categoría que se desea consultar.</param>
        /// <returns>Lista de productos de esa categoría.</returns>
        /// <response code="200">Lista de productos obtenida correctamente.</response>
        /// <response code="500">PRD-005: Error interno al procesar el producto.</response>
        [HttpGet("category/{categoria}")]
        [ProducesResponseType(typeof(List<Product>), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public IActionResult GetByCategory(string categoria)
        {
            var productos = _productService.ObtenerTodos()
                .Where(p => p.Categoria.Equals(
                    categoria,
                    StringComparison.OrdinalIgnoreCase))
                .ToList();

            return Ok(productos);
        }

        // GET: api/products/{id}

        /// <summary>
        /// Obtiene un producto por su ID.
        /// </summary>
        /// <param name="id">Identificador del producto.</param>
        /// <returns>Producto encontrado.</returns>
        /// <response code="200">Producto encontrado correctamente.</response>
        /// <response code="404">PRD-001: Producto no encontrado.</response>
        /// <response code="500">PRD-005: Error interno al procesar el producto.</response>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(Product), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 404)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public IActionResult GetById(Guid id)
        {
            Product? producto = _productService.ObtenerPorId(id);

            if (producto == null)
            {
                throw new NotFoundException("Producto no encontrado.");
            }

            return Ok(producto);
        }

        // POST: api/products

        /// <summary>
        /// Crea un nuevo producto.
        /// </summary>
        /// <param name="producto">Datos del producto que se desea crear.</param>
        /// <returns>Producto creado.</returns>
        /// <response code="201">Producto creado correctamente.</response>
        /// <response code="400">PRD-002: Datos del producto inválidos.</response>
        /// <response code="409">PRD-003: Ya existe un producto con ese nombre en la categoría.</response>
        /// <response code="500">PRD-005: Error interno al procesar el producto.</response>
        [HttpPost]
        [ProducesResponseType(typeof(Product), 201)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        [ProducesResponseType(typeof(ErrorResponse), 409)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public IActionResult Create(Product producto)
        {
            bool agregado = _productService.Agregar(producto);

            if (!agregado)
            {
                throw new BusinessRuleException(
                    "PRD-003",
                    "Ya existe un producto con ese nombre en la categoría.");
            }

            return CreatedAtAction(
                nameof(GetById),
                new { id = producto.Id },
                producto
            );
        }

        // PUT: api/products/{id}

        /// <summary>
        /// Actualiza los datos de un producto.
        /// </summary>
        /// <param name="id">Identificador del producto que se desea actualizar.</param>
        /// <param name="producto">Nuevos datos del producto.</param>
        /// <returns>Producto actualizado.</returns>
        /// <response code="200">Producto actualizado correctamente.</response>
        /// <response code="400">PRD-002: Datos del producto inválidos.</response>
        /// <response code="404">PRD-001: Producto no encontrado.</response>
        /// <response code="500">PRD-005: Error interno al procesar el producto.</response>
        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(Product), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        [ProducesResponseType(typeof(ErrorResponse), 404)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public IActionResult Update(Guid id, Product producto)
        {
            bool actualizado = _productService.Actualizar(id, producto);

            if (!actualizado)
            {
                throw new NotFoundException("Producto no encontrado.");
            }

            return Ok(producto);
        }

        // DELETE: api/products/{id}

        /// <summary>
        /// Elimina un producto por su ID.
        /// </summary>
        /// <param name="id">Identificador del producto que se desea eliminar.</param>
        /// <response code="204">Producto eliminado correctamente.</response>
        /// <response code="404">PRD-001: Producto no encontrado.</response>
        /// <response code="409">PRD-004: El producto tiene órdenes activas y no puede eliminarse.</response>
        /// <response code="500">PRD-005: Error interno al procesar el producto.</response>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(ErrorResponse), 404)]
        [ProducesResponseType(typeof(ErrorResponse), 409)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public IActionResult Delete(Guid id)
        {
            bool eliminado = _productService.Eliminar(id);

            if (!eliminado)
            {
                throw new NotFoundException("Producto no encontrado.");
            }

            return NoContent();
        }
    }
}













