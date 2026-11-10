using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Notifications.API.Data;
using Notifications.API.Exceptions;
using Notifications.API.Middleware;
using Notifications.API.Services;
using Serilog;
using Serilog.Formatting.Compact;
using System.Text.Json;


var builder = WebApplication.CreateBuilder(args);
Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Service", "Notifications.API")
    .WriteTo.Console()
    .WriteTo.File(
    new CompactJsonFormatter(),
    "logs/notifications-.json",
    rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();
builder.Services.AddSingleton<NotificationRepository>();
builder.Services.AddScoped<NotificationService>();
builder.Services.AddHealthChecks();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    string archivoXml = $"{typeof(Program).Assembly.GetName().Name}.xml";
    string rutaXml = Path.Combine(AppContext.BaseDirectory, archivoXml);

    options.IncludeXmlComments(rutaXml);
});

var app = builder.Build();
app.UseExceptionHandler();
app.UseMiddleware<CorrelationIdMiddleware>();
app.UseSerilogRequestLogging();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
var opcionesHealth = new HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";

        await context.Response.WriteAsync(
            JsonSerializer.Serialize(new
            {
                status = report.Status.ToString()
            }));
    }
};

app.MapHealthChecks("/health", opcionesHealth);
app.MapHealthChecks("/health/ready", opcionesHealth);
app.MapHealthChecks("/health/live", opcionesHealth);

app.Run();
