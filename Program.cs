using minimal_api.Domain.DTOs;

var builder = WebApplication.CreateBuilder(args);
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
