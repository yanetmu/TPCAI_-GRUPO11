using Serilog;
var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, configuration) =>
{
    configuration
        .WriteTo.Console()
        .WriteTo.File("logs/orders-.txt", rollingInterval: RollingInterval.Day);
});

builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var problema = new Microsoft.AspNetCore.Mvc.ProblemDetails
            {
                Status = 400,
                Title = "Datos inválidos",
                Detail = "Los datos de la orden son inválidos."
            };

            problema.Extensions["errorCode"] = "ORD-002";
            problema.Extensions["errorMessage"] = "Los datos de la orden son inválidos.";

            return new Microsoft.AspNetCore.Mvc.BadRequestObjectResult(problema);
        };
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    string archivoXml = $"{typeof(Program).Assembly.GetName().Name}.xml";
    string rutaXml = Path.Combine(AppContext.BaseDirectory, archivoXml);

    options.IncludeXmlComments(rutaXml);
});
builder.Services.AddHttpClient();
builder.Services.AddSingleton<Orders.API.Services.OrderService>();
builder.Services.AddExceptionHandler<Orders.API.ExceptionHandlers.OrderExceptionHandler>();
builder.Services.AddProblemDetails();
builder.Services.AddHealthChecks();
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

app.UseMiddleware<Orders.API.Middleware.CorrelationIdMiddleware>();

app.UseSerilogRequestLogging();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");
app.MapHealthChecks("/health/ready");
app.MapHealthChecks("/health/live");

app.Run();