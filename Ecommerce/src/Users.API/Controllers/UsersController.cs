using Microsoft.AspNetCore.Mvc;
using Users.API.DTOs;
using Users.API.Services;

namespace Users.API.Controllers
{
    /// Registro e inicio de sesión de usuarios.
 
    [ApiController]
    [Route("api/users")]
    [Tags("Users")]
    public class UsersController : ControllerBase
    {
        private readonly UserService _userService;

        public UsersController(UserService userService)
        {
            _userService = userService;
        }

        
        /// Registra un usuario nuevo.
       
        /// <remarks>
        /// Ejemplo de registro:
        ///
        ///     {
        ///       "nombre": "Maria",
        ///       "apellido": "Gonzalez",
        ///       "email": "maria@email.com",
        ///       "password": "MiPassword123!"
        ///     }
        /// </remarks>
        /// <response code="201">Usuario registrado correctamente.</response>
        /// <response code="400">USR-002: Datos invalidos.</response>
        /// <response code="409">USR-001: El email ya esta registrado.</response>
        /// <response code="500">USR-006: Error interno.</response>
        [HttpPost("register")]
        [ProducesResponseType(typeof(RegisterResponse), 201)]
        [ProducesResponseType(typeof(ProblemDetails), 400)]
        [ProducesResponseType(typeof(ProblemDetails), 409)]
        [ProducesResponseType(typeof(ProblemDetails), 500)]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            RegisterResponse response = await _userService.RegisterAsync(request);
            return StatusCode(201, response);
        }

        
        /// Inicia sesion con email y contraseña.
        
        /// <remarks>
        /// Ejemplo de inicio de sesion:
        ///
        ///     {
        ///       "email": "maria@email.com",
        ///       "password": "MiPassword123!"
        ///     }
        /// </remarks>
        /// <response code="200">Inicio de sesion correcto.</response>
        /// <response code="400">USR-002: Datos invalidos.</response>
        /// <response code="401">USR-003: Credenciales incorrectas.</response>
        /// <response code="403">USR-004 o USR-005: Usuario bloqueado.</response>
        /// <response code="500">USR-006: Error interno.</response>
        [HttpPost("login")]
        [ProducesResponseType(typeof(LoginResponse), 200)]
        [ProducesResponseType(typeof(ProblemDetails), 400)]
        [ProducesResponseType(typeof(ProblemDetails), 401)]
        [ProducesResponseType(typeof(ProblemDetails), 403)]
        [ProducesResponseType(typeof(ProblemDetails), 500)]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            LoginResponse response = await _userService.LoginAsync(request);
            return Ok(response);
        }

        /// <summary>
        /// Verifica si un usuario existe por su ID.
        /// </summary>
        /// <response code="200">El usuario existe.</response>
        /// <response code="404">El usuario no existe.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> ExisteUsuario(Guid id)
        {
            bool existe = await _userService.ExisteUsuarioAsync(id);

            if (!existe)
            {
                return NotFound();
            }

            return Ok();
        }
    }
}
