using Microsoft.EntityFrameworkCore;
using minimal_api.Domain.DTOs;
using minimal_api.Infrastructure.Db;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<MyDbContext>( options =>
{
    options.UseMySql(
        builder.Configuration.GetConnectionString("mysql"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("mysql"))
    );
});

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapPost("/login", (LoginDto loginDto) =>
{
    if (loginDto.Email == "admin@admin.com" && loginDto.Password == "1234")
    {
        return Results.Ok("Logado com sucesso");
    }

    return Results.Unauthorized();
});
app.Run();
