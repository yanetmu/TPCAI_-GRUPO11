using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Products.API.Data;
using Products.API.ExceptionHandlers;
using Products.API.service;
using Serilog;

var builder = WebApplication.CreateBuilder(args);




Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Service", "Products.API")
    .WriteTo.Console()
    .WriteTo.File(
        new Serilog.Formatting.Json.JsonFormatter(),
        "logs/products-.json",
        rollingInterval: RollingInterval.Day)
    .CreateLogger();


builder.Host.UseSerilog();

builder.Services.AddControllers();
//asp.net valida los datos automaticamente, para que salga el codigo de error necesitamos hacer esto

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var problem = new ProblemDetails
        {
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            Title = "Bad Request",
            Status = 400,
            Detail = "Los datos del producto son inválidos.",
            Instance = context.HttpContext.Request.Path
        };

        problem.Extensions["errorCode"] = "PRD-002";
        problem.Extensions["errorMessage"] = "Los datos del producto son inválidos.";
        problem.Extensions["correlationId"] = context.HttpContext.TraceIdentifier;

        return new BadRequestObjectResult(problem);
    };
});


builder.Services.AddHealthChecks();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    string archivo = $"{typeof(Program).Assembly.GetName().Name}.xml";
    string ruta = Path.Combine(AppContext.BaseDirectory, archivo);

    options.IncludeXmlComments(ruta);
    options.OperationFilter<SwaggerExamplesFilter>();
});


builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();
builder.Services.AddScoped<ProductService>();
builder.Services.AddHttpClient();



// Registramos la base de datos y el repositorio
builder.Services.AddSingleton<DatabaseInitializer>();
builder.Services.AddScoped<ProductRepository>();

var app = builder.Build();

// Crea la tabla automáticamente al arrancar
using (var scope = app.Services.CreateScope())
{
    var initializer = scope.ServiceProvider.GetRequiredService<DatabaseInitializer>();
    initializer.Initialize();
}


app.Use(async (context, next) =>
{
    string correlationId = context.Request.Headers["X-Correlation-Id"].ToString();

    if (string.IsNullOrWhiteSpace(correlationId))
    {
        correlationId = Guid.NewGuid().ToString();
    }

    context.TraceIdentifier = correlationId;
    context.Response.Headers["X-Correlation-Id"] = correlationId;

    using (Serilog.Context.LogContext.PushProperty("CorrelationId", correlationId))
    using (Serilog.Context.LogContext.PushProperty("Endpoint", context.Request.Path.ToString()))
    {
        var inicio = System.Diagnostics.Stopwatch.StartNew();

        Log.Information("Inicio de solicitud {Metodo} {Endpoint}",
            context.Request.Method, context.Request.Path);

        try
        {
            await next(context);
        }
        finally
        {
            inicio.Stop();

            Log.Information(
                "Fin de solicitud {Metodo} {Endpoint} - Estado {StatusCode} - Duración {DuracionMs} ms",
                context.Request.Method,
                context.Request.Path,
                context.Response.StatusCode,
                inicio.ElapsedMilliseconds);
        }
    }
});

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

//comprueba si la bbdd esta disponible
var healthOptions = new HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";

        var resultado = new
        {
            status = report.Status.ToString()
        };

        await context.Response.WriteAsync(
            JsonSerializer.Serialize(resultado));
    }
};

app.MapHealthChecks("/health", healthOptions);
app.MapHealthChecks("/health/ready", healthOptions);
app.MapHealthChecks("/health/live", healthOptions);

app.MapControllers();

app.Run();


public class SwaggerExamplesFilter : Swashbuckle.AspNetCore.SwaggerGen.IOperationFilter
{
    public void Apply(
        Microsoft.OpenApi.OpenApiOperation operation,
        Swashbuckle.AspNetCore.SwaggerGen.OperationFilterContext context)
    {
        foreach (var respuesta in operation.Responses)
        {
            string codigo = respuesta.Key;


            if (codigo == "200" || codigo == "201")
            {
                var contenidoProducto = respuesta.Value.Content;

                if (contenidoProducto != null &&
                    contenidoProducto.ContainsKey("application/json"))
                {
                    var productoEjemplo = new
                    {
                        id = "3fa85f64-5717-4562-b3fc-2c963f66afa6",
                        nombre = "Notebook Lenovo",
                        descripcion = "Notebook de 15 pulgadas",
                        precio = 850000.00m,
                        stock = 10,
                        categoria = "Tecnología",
                        fechaCreacion = "2026-11-06T12:00:00Z"
                    };

                    object ejemplo;

                    if (context.ApiDescription.ActionDescriptor.RouteValues["action"]?.ToString() == "GetAll" ||
                        context.ApiDescription.ActionDescriptor.RouteValues["action"]?.ToString() == "GetByCategory")
                    {
                        ejemplo = new[] { productoEjemplo };
                    }
                    else
                    {
                        ejemplo = productoEjemplo;
                    }

                    contenidoProducto["application/json"].Example =
                        System.Text.Json.JsonSerializer.SerializeToNode(ejemplo);
                }
            }


            if (codigo == "400" || codigo == "404" ||
                codigo == "409" || codigo == "500")
            {
                string titulo = "";
                string detalle = "";
                string errorCode = "";
                string errorMessage = "";

                if (codigo == "400")
                {
                    titulo = "Bad Request";
                    detalle = "Los datos del producto son inválidos.";
                    errorCode = "PRD-002";
                    errorMessage = detalle;
                }
                else if (codigo == "404")
                {
                    titulo = "Not Found";
                    detalle = "El recurso solicitado no fue encontrado.";
                    errorCode = "PRD-001";
                    errorMessage = "Producto no encontrado.";
                }
                else if (codigo == "409")
                {
                    titulo = "Conflict";

                    if (context.ApiDescription.HttpMethod == "DELETE")
                    {
                        detalle = "No se puede eliminar el recurso.";
                        errorCode = "PRD-004";
                        errorMessage = "El producto tiene órdenes activas y no puede eliminarse.";
                    }
                    else
                    {
                        detalle = "Ya existe un recurso con esos datos.";
                        errorCode = "PRD-003";
                        errorMessage = "Ya existe un producto con ese nombre en la categoría.";
                    }
                }
                else
                {
                    titulo = "Internal Server Error";
                    detalle = "Ocurrió un error interno.";
                    errorCode = "PRD-005";
                    errorMessage = "Error interno al procesar el producto.";
                }

                string tipo = "";

                if (codigo == "400")
                    tipo = "https://tools.ietf.org/html/rfc7231#section-6.5.1";
                else if (codigo == "404")
                    tipo = "https://tools.ietf.org/html/rfc7231#section-6.5.4";
                else if (codigo == "409")
                    tipo = "https://tools.ietf.org/html/rfc7231#section-6.5.9";
                else
                    tipo = "https://tools.ietf.org/html/rfc7231#section-6.6.1";

                string ruta = "/" + (context.ApiDescription.RelativePath ?? "");

                var ejemplo = new
                {
                    type = tipo,
                    title = titulo,
                    status = int.Parse(codigo),
                    detail = detalle,
                    instance = ruta,
                    errorCode = errorCode,
                    errorMessage = errorMessage,
                    correlationId = "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx"
                };

                var contenido = respuesta.Value.Content;

                if (contenido == null)
                {
                    continue;
                }

                if (!contenido.ContainsKey("application/json"))
                {
                    contenido["application/json"] = new Microsoft.OpenApi.OpenApiMediaType();
                }

                contenido["application/json"].Example =
                    System.Text.Json.JsonSerializer.SerializeToNode(ejemplo);
            }
        }
    }
}
