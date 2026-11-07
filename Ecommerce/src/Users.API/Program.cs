using Microsoft.AspNetCore.Mvc;
using Users.API.Data;
using Users.API.Exceptions;
using Users.API.Services;
using Dapper;
using Serilog;
using Serilog.Context;

SqlMapper.AddTypeHandler(new GuidHandler());
var builder = WebApplication.CreateBuilder(args);

// Configurar Serilog
builder.Host.UseSerilog((context, configuration) =>
{
    configuration
        .ReadFrom.Configuration(context.Configuration)
        .WriteTo.Console(
    outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] [{CorrelationId}] {Message:lj}{NewLine}{Exception}")
        .WriteTo.File(
            "logs/users-api-.txt",
            rollingInterval: RollingInterval.Day);
});

builder.Services.AddScoped<DatabaseInitializer>();
builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<UserService>();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();
builder.Services.AddHealthChecks();

// Agrega servicios al contenedor

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var problem = new ProblemDetails
        {
            Status = 400,
            Title = "Datos inválidos"
        };

        problem.Extensions["errorCode"] = "USR-002";
        problem.Extensions["errorMessage"] = "Los datos ingresados no son válidos";

        return new BadRequestObjectResult(problem);
    };
});


var app = builder.Build();


app.Use(async (context, next) =>
{
    string correlationId = context.Request.Headers["X-Correlation-ID"].ToString();

    if (string.IsNullOrWhiteSpace(correlationId))
    {
        correlationId = Guid.NewGuid().ToString();
    }

    context.Items["CorrelationId"] = correlationId;

    context.Response.OnStarting(() =>
    {
        context.Response.Headers["X-Correlation-ID"] = correlationId;
        return Task.CompletedTask;
    });

    using (LogContext.PushProperty("CorrelationId", correlationId))
    {
        await next();
    }
});


app.UseExceptionHandler();

app.UseSerilogRequestLogging(options =>
{
    options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
    {
        diagnosticContext.Set(
            "CorrelationId",
            httpContext.Items["CorrelationId"]?.ToString());
    };
});

using (var scope = app.Services.CreateScope())
{
    var database = scope.ServiceProvider.GetRequiredService<DatabaseInitializer>();
    database.Initialize();
}

// Configurar el canal de solicitudes HTTP
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");
app.MapHealthChecks("/ready");
app.MapHealthChecks("/live");

app.Run();
