using Microsoft.AspNetCore.Mvc;
using Users.API.Data;
using Users.API.Exceptions;
using Users.API.Services;
using Dapper;

SqlMapper.AddTypeHandler(new GuidHandler());
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddScoped<DatabaseInitializer>();
builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<UserService>();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

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
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
