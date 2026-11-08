\# Ecommerce - Trabajo Práctico de Microservicios



\## Users.API



Microservicio encargado de la gestión de usuarios, que permite registrar usuarios y realizar el inicio de sesión.



\### Tecnologías utilizadas



\- C# y ASP.NET Core (.NET 10)

\- SQLite

\- Dapper

\- Serilog

\- Swagger



\### Endpoints



| Método | Endpoint | Descripción |

|---|---|---|

| POST | `/api/users/register` | Registra un usuario |

| POST | `/api/users/login` | Permite iniciar sesión |

| GET | `/health` | Verifica el estado del servicio |

| GET | `/health/ready` | Verifica el estado de preparación |

| GET | `/health/live` | Verifica el estado de funcionamiento |



\### Códigos de respuesta



| Código HTTP | Código de error | Descripción |

|---|---|---|

| 201 | — | Usuario registrado correctamente |

| 200 | — | Inicio de sesión correcto |

| 400 | USR-002 | Datos inválidos |

| 401 | USR-003 | Credenciales incorrectas |

| 403 | USR-004 | Usuario bloqueado por intentos fallidos |

| 403 | USR-005 | Usuario bloqueado |

| 409 | USR-001 | Usuario ya registrado |

| 500 | USR-006 | Error interno del servidor |



\### Seguridad y validaciones



Las contraseñas se almacenan mediante hashing y no se incluyen en las respuestas de la API.



El sistema controla los intentos fallidos de inicio de sesión y bloquea al usuario después de tres intentos incorrectos.



\### Manejo de errores



Se utiliza `IExceptionHandler` para centralizar el manejo de excepciones y `ProblemDetails` para devolver errores en formato JSON.



\### Logs y trazabilidad



Se utiliza Serilog para registrar eventos en consola y archivos JSON. Los registros incluyen información del servicio, endpoint, nivel del evento y Correlation ID.



El encabezado `X-Correlation-ID` permite identificar y relacionar las solicitudes con sus registros.



\### Documentación y pruebas



La API dispone de Swagger para consultar y probar los endpoints.



Swagger: `https://localhost:7191/swagger`



Los endpoints de registro, login, manejo de errores y Health Checks fueron probados mediante Swagger.

