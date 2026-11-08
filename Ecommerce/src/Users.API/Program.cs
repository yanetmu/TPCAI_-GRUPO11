using Microsoft.AspNetCore.Mvc;
using Users.API.Data;
using Users.API.Exceptions;
using Users.API.Services;
using Dapper;
using Serilog;
using Serilog.Context;
using Users.API.Swagger;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Text.Json;

SqlMapper.AddTypeHandler(new GuidHandler());
var builder = WebApplication.CreateBuilder(args);

// Configurar Serilog
builder.Host.UseSerilog((context, configuration) =>
{
    configuration
        .ReadFrom.Configuration(context.Configuration)
        .Enrich.WithProperty("Service", "Users.API")
        .WriteTo.Console(
    outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] [{CorrelationId}] {Message:lj}{NewLine}{Exception}")
        .WriteTo.File(
    new Serilog.Formatting.Compact.CompactJsonFormatter(),
    "logs/users-api-.json",
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
builder.Services.AddEndpointsApiExplorer();


builder.Services.AddSwaggerGen(options =>
{
    var xmlFile = $"{typeof(Program).Assembly.GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

    options.IncludeXmlComments(xmlPath);
    options.OperationFilter<UsersExamples>();
});


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

        string correlationId =
    context.HttpContext.Items["CorrelationId"]?.ToString() ?? "";

        problem.Extensions["correlationId"] = correlationId;

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




app.UseSerilogRequestLogging(options =>
{
    options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
    {
        diagnosticContext.Set(
            "CorrelationId",
            httpContext.Items["CorrelationId"]?.ToString());
    };
});

app.UseExceptionHandler();

using (var scope = app.Services.CreateScope())
{
    var database = scope.ServiceProvider.GetRequiredService<DatabaseInitializer>();
    database.Initialize();
}

// Configurar el canal de solicitudes HTTP
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

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


app.Run();
