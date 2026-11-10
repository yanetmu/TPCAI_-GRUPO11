using Users.API.Exceptions;
using Users.API.Data;
using Users.API.DTOs;
using Users.API.Models;
using Microsoft.AspNetCore.Identity;

namespace Users.API.Services
{
    public class UserService
    {
        private readonly UserRepository _repository;
        private readonly PasswordHasher<User> _passwordHasher = new();

        public UserService(UserRepository repository)
        {
            _repository = repository;
        }

        public async Task<RegisterResponse> RegisterAsync(RegisterRequest request)
        {
            User? existente = await _repository.GetByEmailAsync(request.Email);

            if (existente != null)
            {
                throw new UserException("USR-001", "El email ya está registrado", 409);
            }

            User user = new User
            {
                Id = Guid.NewGuid(),
                Nombre = request.Nombre,
                Apellido = request.Apellido,
                Email = request.Email,
                FechaRegistro = DateTime.UtcNow,
                Activo = true,
                IntentosFallidos = 0
            };

            user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);

            await _repository.CreateAsync(user);

            return new RegisterResponse
            {
                Id = user.Id,
                Nombre = user.Nombre,
                Apellido = user.Apellido,
                Email = user.Email,
                FechaRegistro = user.FechaRegistro,
                Activo = user.Activo
            };
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            User? user = await _repository.GetByEmailAsync(request.Email);

            if (user == null)
            {
                throw new UserException("USR-003", "Credenciales incorrectas", 401);
            }

            if (!user.Activo)
            {
                if (user.IntentosFallidos >= 3)
                {
                    throw new UserException("USR-004", "Usuario bloqueado por intentos fallidos", 403); 
                }

                throw new UserException("USR-005", "Usuario bloqueado", 403);
            }

            var resultado = _passwordHasher.VerifyHashedPassword(
                user, user.PasswordHash, request.Password);

            if (resultado == PasswordVerificationResult.Failed)
            {
                user.IntentosFallidos++;

                if (user.IntentosFallidos >= 3)
                {
                    user.Activo = false;
                }

                await _repository.UpdateAsync(user);

                if (!user.Activo)
                {
                    throw new UserException("USR-004", "Usuario bloqueado por intentos fallidos", 403);
                }

                throw new UserException("USR-003", "Credenciales incorrectas", 401);
            }

            user.IntentosFallidos = 0;
            await _repository.UpdateAsync(user);

            return new LoginResponse
            {
                Id = user.Id,
                Nombre = user.Nombre,
                Apellido = user.Apellido,
                Email = user.Email
            };

        }

        public async Task<bool> ExisteUsuarioAsync(Guid id)
        {
            User? usuario = await _repository.GetByIdAsync(id);

            if (usuario == null)
            {
                return false;
            }

            return true;
        }
    }
}

