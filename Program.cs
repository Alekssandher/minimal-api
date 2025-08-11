using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using minimal_api.Domain.DTOs;
using minimal_api.Domain.Interfaces;
using minimal_api.Domain.Service;
using minimal_api.Infrastructure.Db;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IAdminService, AdminService>();

builder.Services.AddDbContext<MyDbContext>( options =>
{
    options.UseMySql(
        builder.Configuration.GetConnectionString("mysql"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("mysql"))
    );
});

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapPost("/login", ([FromBody] LoginDto loginDto, IAdminService ias) =>
{
    if (ias.Login(loginDto) != null)
    {
        return Results.Ok("Logado com sucesso");
    }

    return Results.Unauthorized();
});
app.Run();
