# Cart.API

Microservicio encargado de la gestión del carrito de compras de los usuarios. Permite consultar el carrito, agregar productos, actualizar sus cantidades y eliminar productos o el carrito completo.

## Tecnologías utilizadas

- C# y ASP.NET Core (.NET 10)
- Swagger
- Serilog
- Health Checks

## Endpoints

| Método | Endpoint | Descripción |
|---|---|---|
| GET | `/api/cart/{userId}` | Consulta el carrito |
| POST | `/api/cart/{userId}/items` | Agrega un producto |
| PUT | `/api/cart/{userId}/items/{productId}` | Actualiza la cantidad |
| DELETE | `/api/cart/{userId}/items/{productId}` | Elimina un producto |
| DELETE | `/api/cart/{userId}` | Elimina el carrito |

## Códigos de respuesta

| Código HTTP | Código de error | Descripción |
|---|---|---|
| 200 | — | Operación realizada correctamente |
| 204 | — | Eliminación realizada correctamente |
| 400 | CRT-004 | Cantidad inválida |
| 404 | CRT-001 | Carrito no encontrado |
| 404 | CRT-002 | Producto no encontrado |
| 422 | CRT-003 | Stock insuficiente |
| 500 | CRT-005 | Error interno del servidor |

## Validaciones e integración

Se verifica que la cantidad de productos sea mayor a cero y que exista stock suficiente para realizar la operación.

El microservicio se comunica con `Products.API` mediante HTTP para consultar la existencia y disponibilidad de los productos.

## Almacenamiento de datos

Los carritos se almacenan en memoria mediante `CartRepository`, utilizando una lista de carritos. Los datos se mantienen mientras la aplicación está en ejecución y se pierden al reiniciar el servicio.

## Manejo de errores

Se utiliza `IExceptionHandler` para centralizar el manejo de excepciones y `ProblemDetails` para devolver errores en formato JSON, incluyendo los códigos de error correspondientes.

## Logs y trazabilidad

Se utiliza Serilog para registrar eventos en consola y archivos JSON.

El encabezado `X-Correlation-Id` permite identificar cada solicitud y relacionarla con sus registros mediante el identificador `CorrelationId`.

## Documentación y pruebas

La API dispone de Swagger para consultar y probar los endpoints.

Swagger: `https://localhost:7167/swagger`

Se probaron los endpoints de consulta, agregado, actualización y eliminación del carrito, las principales validaciones, los códigos de error y los Health Checks.



# Notifications.API

Microservicio encargado de la gestión de notificaciones de los usuarios. Permite enviar notificaciones y consultar las notificaciones asociadas a un usuario.

## Tecnologías utilizadas

- C# y ASP.NET Core (.NET 10)
- Swagger
- Serilog
- Health Checks
- HttpClient

## Endpoints

| Método | Endpoint | Descripción |
|---|---|---|
| POST | `/api/notifications` | Envía una notificación a un usuario |
| GET | `/api/notifications/{usuarioId}` | Consulta las notificaciones de un usuario |

## Códigos de respuesta

| Código HTTP | Código de error | Descripción |
|---|---|---|
| 200 | — | Consulta realizada correctamente |
| 201 | — | Notificación creada correctamente |
| 400 | NTF-002 | Datos de la notificación inválidos |
| 404 | NTF-001 | Usuario no encontrado |
| 404 | NTF-003 | Notificación no encontrada |
| 500 | NTF-004 | Error interno del servidor |

## Almacenamiento de datos

Las notificaciones se almacenan en memoria mediante `NotificationRepository`, utilizando una lista de notificaciones. 
Los datos se mantienen mientras la aplicación está en ejecución y se pierden al reiniciar el servicio.

## Validaciones e integración

Se validan los datos necesarios para enviar una notificación y se verifica que el usuario destinatario exista.

El microservicio se comunica con `Users.API` mediante HTTP para comprobar la existencia del usuario antes de crear la notificación.

## Manejo de errores

Se utiliza `IExceptionHandler` para centralizar el manejo de excepciones y `ProblemDetails` para devolver errores en formato JSON, incluyendo `errorCode`, `errorMessage` y `correlationId`.

## Logs y trazabilidad

Se utiliza Serilog para registrar eventos de la aplicación.

El encabezado `X-Correlation-Id` permite identificar cada solicitud y relacionarla con sus registros mediante el identificador `CorrelationId`.

## Documentación y pruebas

La API dispone de Swagger para consultar y probar los endpoints.

Se probaron el envío de notificaciones a usuarios existentes e inexistentes, la consulta de notificaciones y los códigos de respuesta correspondientes.

Swagger: `https://localhost:7104/swagger`


# Users.API

Microservicio de gestión de usuarios del proyecto Ecommerce. Permite registrar usuarios, iniciar sesión y consultar si un usuario existe por su identificador.

## Tecnologías y organización

Desarrollado con ASP.NET Core y C# (.NET 10). Organizado en controladores, servicios, DTOs, modelos, repositorio y manejo global de excepciones. Utiliza SQLite y Dapper para almacenar usuarios, `PasswordHasher<User>` para proteger contraseñas, Swagger/OpenAPI para documentar los endpoints, Serilog para registrar eventos y Health Checks para comprobar el estado del servicio.

## Endpoints

| Método | Ruta | Función | Respuesta correcta |
|---|---|---|---|
| POST | `/api/users/register` | Registrar un usuario | 201 |
| POST | `/api/users/login` | Iniciar sesión | 200 |
| GET | `/api/users/{id}` | Verificar si existe un usuario por ID | 200 si existe; 404 si no existe |

### Registro de usuario

`POST /api/users/register`

```json
{
  "nombre": "Maria",
  "apellido": "Gonzalez",
  "email": "maria@email.com",
  "password": "MiPassword123!"
}
```

El servicio comprueba que el correo electrónico no esté registrado, genera un identificador `Guid`, establece la fecha de registro y el estado activo, almacena el hash de la contraseña y devuelve los datos públicos del usuario sin exponer la contraseña.

### Inicio de sesión

`POST /api/users/login`

```json
{
  "email": "maria@email.com",
  "password": "MiPassword123!"
}
```

El servicio busca al usuario por correo y verifica la contraseña mediante `PasswordHasher<User>`. Cuenta los intentos fallidos y bloquea al usuario después de tres intentos incorrectos. Cuando el inicio de sesión es correcto, reinicia el contador y devuelve los datos del usuario.

### Verificación de usuario por ID

`GET /api/users/{id}`

Este endpoint permite que otros microservicios, como Orders.API y Notifications.API, comprueben la existencia de un usuario. Responde `200 OK` cuando existe y `404 Not Found` cuando no existe. La consulta se realiza mediante `UserService` y `UserRepository`.

## Manejo de errores

| Código | HTTP | Descripción |
|---|---|---|
| USR-001 | 409 | El correo ya está registrado |
| USR-002 | 400 | Datos inválidos |
| USR-003 | 401 | Credenciales incorrectas |
| USR-004 | 403 | Bloqueo por intentos fallidos |
| USR-005 | 403 | Usuario bloqueado |
| USR-006 | 500 | Error interno |

Las excepciones se gestionan de forma centralizada con `IExceptionHandler` y respuestas `ProblemDetails`. Las respuestas de validación incluyen `errorCode`, `errorMessage` y `correlationId`.

## Persistencia

Se utiliza SQLite con Dapper a través de `UserRepository`. `DatabaseInitializer` inicializa la base de datos al arrancar la API. El repositorio incluye consultas por correo y por identificador, así como creación y actualización de usuarios.

## Registro, trazabilidad y monitoreo

Serilog registra eventos en consola y en archivos JSON. La API acepta o genera el encabezado `X-Correlation-ID` y lo devuelve en la respuesta para facilitar el seguimiento de solicitudes.

Endpoints de salud: `/health`, `/health/ready` y `/health/live`.

## Swagger y pruebas

Swagger en el entorno de desarrollo: `https://localhost:7191/swagger` (según la configuración local utilizada en las pruebas).

Se comprobó el registro de un usuario con respuesta HTTP 201 y se utilizó la consulta por ID para validar usuarios desde Orders.API. Los demás comportamientos se describen de acuerdo con el código revisado; no se presentan como pruebas ejecutadas.


# Orders.API

Microservicio del proyecto Ecommerce que permite crear órdenes, consultarlas, filtrarlas por usuario y modificar su estado. Para crear una orden, se comunica con Users.API y Products.API.

## Tecnologías y organización

Desarrollado con ASP.NET Core y C# (.NET 10). Utiliza controladores, servicios, DTOs, modelos, manejo de excepciones, `IHttpClientFactory`, Swagger/OpenAPI, Serilog y Health Checks.

## Endpoints

| Método | Ruta | Función | Respuesta correcta |
|---|---|---|---|
| GET | `/api/orders` | Obtener todas las órdenes; admite el filtro `?usuarioId=` | 200 |
| GET | `/api/orders/{id}` | Obtener una orden por su identificador | 200 |
| POST | `/api/orders` | Crear una orden y validar usuario, productos y stock | 201 |
| PUT | `/api/orders/{id}/status` | Actualizar el estado de una orden | 200 |

## Creación de órdenes

`POST /api/orders`

Ejemplo de solicitud utilizando los identificadores empleados en las pruebas de integración:

```json
{
  "usuarioId": "35edb5ae-6472-466e-aa87-e0dfef69e4b5",
  "items": [
    {
      "productoId": "b0ae6be7-f383-4881-a7a4-6e598d99adb8",
      "cantidad": 2
    }
  ]
}
```

El servicio valida que el identificador del usuario sea válido, que la lista de productos no esté vacía y que cada producto tenga un identificador y una cantidad positiva. Consulta `GET /api/users/{id}` en Users.API y `GET /api/products/{id}` en Products.API. Comprueba que el producto exista y que su stock alcance para la cantidad solicitada. Obtiene el precio del producto, calcula el total y crea la orden con estado inicial `Pendiente`.

En las pruebas realizadas, una orden con dos unidades de un producto de precio 15000 devolvió `201 Created` y total 30000.

## Consulta de órdenes

`GET /api/orders` devuelve las órdenes disponibles y admite el filtro opcional `usuarioId`. `GET /api/orders/{id}` devuelve la orden correspondiente al identificador indicado. Ambas operaciones se probaron con resultado HTTP 200.

## Actualización de estado

`PUT /api/orders/{id}/status`

Transiciones admitidas por el servicio:

- `Pendiente` → `Confirmada` o `Cancelada`.
- `Confirmada` → `Enviada` o `Cancelada`.
- `Enviada` → `Entregada`.

Las transiciones no permitidas generan HTTP 409 y código `ORD-006`. Se comprobó una transición válida de `Pendiente` a `Confirmada` con respuesta 200 y una transición inválida con respuesta 409.

## Errores y validaciones

| Código | HTTP | Descripción |
|---|---|---|
| ORD-001 | 404 | Orden no encontrada |
| ORD-002 | 400 | Datos de la orden o estado inválidos |
| ORD-003 | 404 | Usuario inexistente |
| ORD-004 | 404 | Producto inexistente |
| ORD-005 | 422 | Stock insuficiente |
| ORD-006 | 409 | Transición de estado no permitida |
| ORD-007 | 500 | Error interno |

Las respuestas de error observadas en Swagger utilizan JSON `ProblemDetails`, con campos `errorCode` y `errorMessage`.

## Integración con otros microservicios

- **Users.API:** verifica la existencia del usuario antes de crear una orden. En las pruebas se utilizó `https://localhost:7191`.
- **Products.API:** consulta existencia, precio y stock de los productos. En las pruebas se utilizó `https://localhost:7223`.

Se realizaron pruebas de creación exitosa, usuario inexistente (`ORD-003`, 404), producto inexistente (`ORD-004`, 404) y stock insuficiente (`ORD-005`, 422).

## Persistencia actual

El código revisado guarda las órdenes en una lista en memoria (`List<Order>` en `OrderService`). **Todavía no utiliza una base de datos o repositorio persistente**, por lo que las órdenes no se conservan al reiniciar la aplicación. Este punto queda pendiente de implementación o verificación según la biblioteca de persistencia requerida por la consigna.

## Registro, trazabilidad y monitoreo

La API incorpora Serilog, manejo global de errores y Correlation ID. En las pruebas de Swagger se observó el encabezado `x-correlation-id` en las respuestas. Los servicios involucrados respondieron correctamente en sus comprobaciones de salud (`healthy`).

## Swagger y pruebas realizadas

Swagger: `https://localhost:7176/swagger` (según la configuración local utilizada).

| Prueba | Resultado observado |
|---|---|
| Crear orden válida | 201 |
| Consultar orden por ID | 200 |
| Consultar órdenes filtradas por usuario | 200 |
| Cambiar estado de Pendiente a Confirmada | 200 |
| Intentar transición inválida | 409 / ORD-006 |
| Crear orden con stock insuficiente | 422 / ORD-005 |
| Crear orden con producto inexistente | 404 / ORD-004 |
| Crear orden con usuario inexistente | 404 / ORD-003 |
 


