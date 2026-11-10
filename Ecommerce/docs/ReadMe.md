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
