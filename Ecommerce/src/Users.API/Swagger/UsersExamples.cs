
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Text.Json.Nodes;

namespace Users.API.Swagger
{
    public class UsersExamples : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            string? endpoint = context.ApiDescription.RelativePath;

            if (endpoint == "api/users/register")
            {
                AgregarEjemplo(operation, "201", """
                {
                    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
                    "nombre": "Maria",
                    "apellido": "Gonzalez",
                    "email": "maria@email.com",
                    "fechaRegistro": "2026-11-08T15:20:22Z",
                    "activo": true
                }
                """);

                AgregarError(operation, "400", "USR-002", "Los datos ingresados no son válidos");
                AgregarError(operation, "409", "USR-001", "El email ya está registrado");
                AgregarError(operation, "500", "USR-006", "Error interno");
            }

            if (endpoint == "api/users/login")
            {
                AgregarEjemplo(operation, "200", """
                {
                    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
                    "nombre": "Maria",
                    "apellido": "Gonzalez",
                    "email": "maria@email.com"
                }
                """);

                AgregarError(operation, "400", "USR-002", "Los datos ingresados no son válidos");
                AgregarError(operation, "401", "USR-003", "Credenciales incorrectas");
                AgregarError(operation, "403", "USR-004", "Usuario bloqueado");
                AgregarError(operation, "500", "USR-006", "Error interno");
            }
        }

        private void AgregarError(OpenApiOperation operation, string codigoHttp,
            string codigoError, string mensaje)
        {
            string json = $$"""
            {
                "type": "about:blank",
                "title": "Error",
                "status": {{codigoHttp}},
                "detail": "{{mensaje}}",
                "instance": "/api/users",
                "errorCode": "{{codigoError}}",
                "errorMessage": "{{mensaje}}"
            }
            """;

            AgregarEjemplo(operation, codigoHttp, json);
        }

        private void AgregarEjemplo(OpenApiOperation operation, string codigo, string json)
        {
            if (operation.Responses.TryGetValue(codigo, out var respuesta))
            {
                if (respuesta.Content != null &&
                    respuesta.Content.TryGetValue("application/json", out var contenido))
                {
                    contenido.Example = JsonNode.Parse(json);
                }
            }
        }
    }
}
