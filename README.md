# Products.API — Microservicio de Productos

## 1. Descripción

Products.API es el microservicio encargado de administrar el catálogo de productos del sistema de E-Commerce basado en microservicios.

Permite consultar, registrar, actualizar y eliminar productos mediante una API REST desarrollada con ASP.NET Core y .NET 10.

Cada producto contiene un identificador único, nombre, descripción, precio, stock, categoría y fecha de creación.

El microservicio utiliza SQLite para almacenar la información y se comunica con Orders.API para impedir la eliminación de productos asociados a órdenes activas.

## 2. Tecnologías utilizadas

- **ASP.NET Core Web API (.NET 10):** desarrollo de los endpoints REST.
- **SQLite:** almacenamiento persistente de productos.
- **Swagger:** documentación y pruebas de los endpoints.
- **Serilog:** registro de eventos y solicitudes.
- **Health Checks:** monitoreo del estado del microservicio.
- **HttpClient:** comunicación HTTP con Orders.API.
- **IExceptionHandler y ProblemDetails:** manejo centralizado de errores.

## 3. Estructura del microservicio

Products.API está organizado en las siguientes carpetas y componentes:

| Componente | Funcionalidad |
|---|---|
| Controllers | Define los endpoints HTTP |
| Models | Contiene el modelo Product |
| Service | Implementa la lógica de negocio y las validaciones |
| Data | Gestiona el acceso y la inicialización de la base de datos |
| Exceptions | Define las excepciones del microservicio |
| ExceptionHandlers | Centraliza el manejo de errores |

La clase `ProductService` contiene la lógica de negocio, mientras que `ProductRepository` realiza las operaciones sobre SQLite.

## 4. Modelo de datos y validaciones

El modelo `Product` contiene los siguientes atributos:

| Atributo | Tipo | Restricción |
|---|---|---|
| Id | Guid | Identificador único generado automáticamente |
| Nombre | string | Obligatorio, máximo 100 caracteres |
| Descripcion | string | Opcional, máximo 500 caracteres |
| Precio | decimal | Mayor o igual a 0,01 |
| Stock | int | No puede ser negativo |
| Categoria | string | Obligatoria |
| FechaCreacion | DateTime | Generada automáticamente al registrar el producto |

Las validaciones permiten evitar el registro de productos con datos inválidos. Además, durante la creación se verifica que no exista otro producto con el mismo nombre y categoría.

## 5. Endpoints implementados

La ruta base del microservicio es `/api/products`.

| Método | Endpoint | Funcionalidad |
|---|---|---|
| GET | `/api/products` | Obtiene todos los productos y permite filtrar por nombre o categoría |
| GET | `/api/products/{id}` | Obtiene un producto por su identificador |
| GET | `/api/products/category/{categoria}` | Obtiene los productos de una categoría |
| POST | `/api/products` | Registra un nuevo producto |
| PUT | `/api/products/{id}` | Actualiza los datos de un producto existente |
| DELETE | `/api/products/{id}` | Elimina un producto si no está asociado a órdenes activas |

Los endpoints están documentados mediante Swagger, que permite consultar parámetros, visualizar respuestas y ejecutar solicitudes de prueba.

## 6. Persistencia de datos

Products.API utiliza SQLite para almacenar la información de los productos.

La clase `ProductRepository` implementa las operaciones de consulta, inserción, actualización y eliminación mediante instrucciones SQL parametrizadas.

La clase `DatabaseInitializer` se encarga de inicializar la base de datos al iniciar la aplicación.

**Pruebas realizadas:** se comprobó que los productos pueden registrarse, recuperarse, actualizarse y eliminarse, y que la información permanece almacenada en SQLite.

## 7. Manejo de errores y reglas de negocio

Products.API utiliza `IExceptionHandler` y `ProblemDetails` para centralizar el tratamiento de excepciones y devolver respuestas HTTP estructuradas.

Los errores contemplados son:

| Código | HTTP | Descripción |
|---|---|---|
| PRD-001 | 404 Not Found | Producto no encontrado |
| PRD-002 | 400 Bad Request | Datos del producto inválidos |
| PRD-003 | 409 Conflict | Producto duplicado por nombre y categoría |
| PRD-004 | 409 Conflict | El producto tiene órdenes activas y no puede eliminarse |
| PRD-005 | 500 Internal Server Error | Error interno del servidor |

Las respuestas de error incluyen información como `errorCode`, `errorMessage` y `correlationId`, facilitando la identificación de los problemas.

### PRD-001 — Producto no encontrado

Cuando se intenta consultar, actualizar o eliminar un producto inexistente, el microservicio devuelve `404 Not Found`.

### PRD-002 — Datos inválidos

Al registrar o actualizar un producto, se validan los datos recibidos. Si no cumplen las restricciones establecidas, el servicio devuelve `400 Bad Request`.

### PRD-003 — Producto duplicado

Antes de registrar un producto, se verifica que no exista otro con el mismo nombre y categoría.

Si se encuentra un producto duplicado, se devuelve `409 Conflict` con el código `PRD-003`.

### PRD-004 — Producto asociado a órdenes activas

Antes de eliminar un producto, Products.API consulta Orders.API para verificar si está asociado a una orden activa.

Se consideran órdenes activas aquellas cuyo estado sea `Pendiente` o `Confirmada`.

Si el producto pertenece a una orden activa, el microservicio impide su eliminación y devuelve:

- **HTTP:** 409 Conflict.
- **errorCode:** PRD-004.
- **errorMessage:** El producto tiene órdenes activas y no puede eliminarse.

Si el producto existe y no tiene órdenes activas, se permite su eliminación y se devuelve `204 No Content`.

### PRD-005 — Error interno

Los errores inesperados son procesados por el manejador global de excepciones, que devuelve `500 Internal Server Error`.

## 8. Integración con Orders.API

Products.API utiliza `HttpClient` para comunicarse con Orders.API mediante HTTP.

Esta integración permite implementar la regla de negocio PRD-004 sin duplicar la información de las órdenes dentro de Products.API.

**Proceso de eliminación:**

1. Products.API verifica que el producto exista.
2. Consulta `GET /api/orders`.
3. Recorre las órdenes y sus productos.
4. Comprueba si alguna orden contiene el producto y se encuentra en estado `Pendiente` o `Confirmada`.
5. Si encuentra una orden activa, impide la eliminación y devuelve `409 Conflict` con el código `PRD-004`.
6. Si no existen órdenes activas asociadas, elimina el producto y devuelve `204 No Content`.

La dirección de Orders.API se obtiene mediante la configuración `Services:OrdersApiUrl` de Products.API, evitando escribir el puerto directamente en la lógica del servicio.

## 9. Registro de eventos con Serilog

Se implementó Serilog para registrar información sobre el funcionamiento del microservicio.

Los registros se generan tanto en la consola como en archivos JSON dentro de la carpeta `logs`.

Entre los datos registrados se encuentran:

- Método HTTP y endpoint solicitado.
- Código de respuesta.
- Duración de la solicitud.
- Identificador del servicio.
- Correlation ID.

**Pruebas realizadas:** se ejecutó `GET /api/products` desde Swagger y se comprobó que la consola y el archivo JSON registraron correctamente la solicitud, el código HTTP `200` y su duración.

## 10. Correlation ID

Se implementó el uso de Correlation ID para identificar y relacionar los registros correspondientes a una misma solicitud.

El microservicio utiliza el encabezado `X-Correlation-Id`. Si la solicitud no contiene uno, genera automáticamente un identificador.

Este identificador se incorpora a la respuesta HTTP y a los registros generados por Serilog.

**Pruebas realizadas:** se verificó que el mismo Correlation ID aparece en los registros de inicio y finalización de una solicitud.

La propagación del Correlation ID entre microservicios queda pendiente de una verificación específica de integración.

## 11. Health Checks

Products.API cuenta con tres endpoints de monitoreo:

| Endpoint | Funcionalidad |
|---|---|
| `/health` | Informa el estado general del microservicio |
| `/health/ready` | Expone el estado de disponibilidad configurado |
| `/health/live` | Expone el estado de funcionamiento configurado |

Los tres endpoints están configurados mediante el sistema de Health Checks de ASP.NET Core y devuelven información del estado en formato JSON.

Actualmente utilizan comprobaciones básicas; no se configuró una verificación específica de conectividad con SQLite.

**Pruebas realizadas:** se ejecutaron los endpoints de Health Checks y se verificaron sus respuestas.

## 12. Documentación con Swagger

Se configuró Swagger para facilitar la documentación y prueba de los endpoints.

La interfaz permite visualizar las operaciones disponibles, los parámetros, los modelos de datos y los códigos de respuesta.

También se incorporaron comentarios XML y ejemplos de respuestas exitosas y de error.

**Swagger de Products.API:**

`https://localhost:7223/swagger`

**Pruebas realizadas:** se utilizaron los endpoints desde Swagger para comprobar las operaciones, las validaciones y los mensajes de error.

## 13. Ejecución del microservicio

Para ejecutar Products.API:

1. Abrir la solución `Ecommerce` en Visual Studio.
2. Seleccionar Products.API como proyecto de inicio.
3. Ejecutar el proyecto utilizando el perfil HTTPS.
4. Acceder a Swagger para consultar y probar los endpoints.

Para probar la integración PRD-004, también debe estar ejecutándose Orders.API. Si se necesita crear una orden nueva, debe estar disponible Users.API para validar el usuario.

**Direcciones utilizadas durante las pruebas locales:**

| Microservicio | Dirección HTTPS |
|---|---|
| Products.API | `https://localhost:7223` |
| Orders.API | `https://localhost:7176` |
| Users.API | `https://localhost:7191` |

Los puertos pueden variar según la configuración local de cada integrante.

## 14. Pruebas funcionales realizadas

Durante el desarrollo se realizaron pruebas de las principales funcionalidades de Products.API.

| Funcionalidad | Resultado |
|---|---|
| Consulta de productos | Correcto |
| Creación de productos | Correcto |
| Actualización de productos | Correcto |
| Eliminación de productos sin órdenes activas | Correcto |
| Validaciones y errores implementados | Correcto |
| Persistencia en SQLite | Correcto |
| Health Checks | Correcto |
| Documentación con Swagger | Correcto |
| Registros de Serilog en consola | Correcto |
| Registros de Serilog en archivos JSON | Correcto |
| Correlation ID en los registros | Correcto |
| PRD-004 con orden pendiente | Correcto |
|PRD-004 con orden Confirmada | Correcto |

### Prueba de integración PRD-004

Para verificar la regla PRD-004 se realizaron las siguientes operaciones:

| Operación | Resultado obtenido |
|---|---|
| Consultar órdenes en Orders.API | 200 OK |
| Consultar productos en Products.API | 200 OK |
| Eliminar un producto sin órdenes activas | 204 No Content |
| Registrar un usuario de prueba en Users.API | 201 Created |
| Crear una orden en estado Pendiente | 201 Created |
| Intentar eliminar el producto asociado a la orden | 409 Conflict — PRD-004 |
| Consultar nuevamente el producto protegido | 200 OK |
| Actualizar el estado de la orden a Confirmada | 200 OK |
| Intentar eliminar el producto con orden Confirmada | 409 Conflict — PRD-004 |

Durante la prueba se creó una orden `Pendiente` que contenía el producto Notebook Lenovo Prueba.

Posteriormente, se intentó eliminar el producto mediante `DELETE /api/products/{id}`.

El microservicio respondió con `409 Conflict`, el código `PRD-004` y el mensaje correspondiente.

Finalmente, se consultó nuevamente el producto mediante `GET /api/products/{id}` y se obtuvo `200 OK`, confirmando que no había sido eliminado.

**Resultado:** se verificó correctamente la integración entre Products.API y Orders.API para impedir la eliminación de productos asociados a órdenes pendientes.

Se verificó correctamente la regla PRD-004 para órdenes en estado `Pendiente` y `Confirmada`. En ambos casos, Products.API devolvió `409 Conflict`, impidiendo la eliminación del producto asociado a la orden activa.

## 15. Estado de implementación

Products.API cuenta con las operaciones de gestión de productos, persistencia SQLite, validaciones, manejo centralizado de errores, registros con Serilog, Correlation ID, Health Checks y documentación con Swagger.

También se implementó la integración con Orders.API correspondiente a PRD-004 y se comprobó su funcionamiento con una orden en estado `Pendiente`.

Las pruebas realizadas permitieron verificar tanto el funcionamiento independiente del microservicio como la regla de eliminación asociada a órdenes activas.


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

Microservicio encargado de la gestión de notificaciones de los usuarios. 
Permite enviar notificaciones y consultar las notificaciones asociadas a un usuario.

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

Se utiliza `IExceptionHandler` para centralizar el manejo de excepciones y `ProblemDetails` para devolver errores en formato JSON, 
incluyendo `errorCode`, `errorMessage` y `correlationId`.

## Logs y trazabilidad

Se utiliza Serilog para registrar eventos de la aplicación.

El encabezado `X-Correlation-Id` permite identificar cada solicitud y relacionarla con sus registros mediante el identificador `CorrelationId`.

## Documentación y pruebas

La API dispone de Swagger para consultar y probar los endpoints.

Se probaron el envío de notificaciones a usuarios existentes e inexistentes, 
la consulta de notificaciones y los códigos de respuesta correspondientes.

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
