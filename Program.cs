using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using minimal_api.Domain.DTOs;
using minimal_api.Domain.Interfaces;
using minimal_api.Domain.ModelViews;
using minimal_api.Domain.Service;
using minimal_api.Infrastructure.Db;
using Scalar.AspNetCore;

#region Builder
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi(options =>
            {
                options.AddDocumentTransformer((document, context, cancellationToken) =>
                {
                    document.Info = new()
                    {
                        Title = "Minimal API Documentation",
                        Version = "v1",
                        Description = "A Minimal API Example."
                    };
                    return Task.CompletedTask;
                });
            });


builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<IVehicleService, VehicleService>();

builder.Services.AddDbContext<MyDbContext>( options =>
{
    options.UseMySql(
        builder.Configuration.GetConnectionString("mysql"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("mysql"))
    );
});
#endregion
var app = builder.Build();

#region Home
app.MapGet("/", () => Results.Json(new Home())).WithTags("Home");
#endregion



#region Admin
app.MapPost("admin/login", ([FromBody] LoginDto loginDto, IAdminService ias) =>
{
    if (ias.Login(loginDto) != null)
    {
        return Results.Ok("Logado com sucesso");
    }

    return Results.Unauthorized();
}).WithTags("Admins");

app.MapGet("admin", ([FromQuery] [DefaultValue(1)] int page, IAdminService ias) =>
{
    var result = ias.All(page);

    return Results.Ok(result);
}).WithTags("Admins");

app.MapGet("admin/{id}", ([FromRoute] int id, IAdminService ias) =>
{
    return Results.Ok(ias.FindById(id));
}).WithTags("Admins");

app.MapPost("admin/create", ([FromBody] AdminDto adminDto, IAdminService ias) =>
{
    var validation = new ValidationErrors
    {
        Messages = []
    };

    if (string.IsNullOrEmpty(adminDto.Email))
        validation.Messages.Add("Email não pode ser nulo ou vazio");

    if (string.IsNullOrEmpty(adminDto.Password))
        validation.Messages.Add("Passowrd não pode ser nula ou vazia");
    if (string.IsNullOrEmpty(adminDto.Profile.ToString()))
        validation.Messages.Add("Profile não pode ser nulo ou vazio");

    if (validation.Messages.Count > 0)
        return Results.BadRequest(validation.Messages);

    ias.Include(adminDto.ToAdmin());

    return Results.Ok();

}).WithTags("Admins");
#endregion

#region Vehicles
static ValidationErrors ValidateVehicle(VehicleDto vehicleDto)
{
    var validation = new ValidationErrors
    {
        Messages = new List<string>()
    };

    if (string.IsNullOrEmpty(vehicleDto.Name))
        validation.Messages.Add("Name não pode estar vazio ou nulo");

    if (string.IsNullOrEmpty(vehicleDto.Brand))
        validation.Messages.Add("Brand não pode estar vazia ou nula");

    if (vehicleDto.Year < 1950)
        validation.Messages.Add("Veículo muito antigo, somente anos superiores a 1950 são permitidos");

    return validation;
}
app.MapPost("/vehicles", ([FromBody] VehicleDto vehicleDto, IVehicleService ivs) =>
{
    var v = ValidateVehicle(vehicleDto);
    if (v.Messages.Count > 0)
        return Results.BadRequest(v.Messages);

    var vehicleEntity = vehicleDto.ToEntity();

    ivs.Include(vehicleEntity);

    return Results.Created($"/vehicle/{vehicleEntity.Id}", vehicleEntity);
}
).WithTags("Vehicles");

app.MapGet("/vehicles/{id}", ([FromRoute] int id, IVehicleService ivs) =>
{
    var vehicle = ivs.FindById(id);

    if (vehicle == null)
    {
        return Results.NotFound();
    }

    return Results.Ok(vehicle);
}
).WithTags("Vehicles");

app.MapGet("/vehicles", ([FromQuery][DefaultValue(1)] int page, [FromQuery] string? name, [FromQuery] string? brand, IVehicleService ivs) =>
{
    var vehicles = ivs.All(page, name, brand);

    return Results.Ok(vehicles);
}
).WithTags("Vehicles");

app.MapPut("/vehicles/{id}", ([FromRoute] int id, [FromBody] VehicleDto vehicleDto, IVehicleService ivs) =>
{
    var v = ValidateVehicle(vehicleDto);
    if (v.Messages.Count > 0)
        return Results.BadRequest(v.Messages);
        
    var vehicle = ivs.FindById(id);

    if (vehicle == null)
    {
        return Results.NotFound();

    }
    vehicle.Update(vehicleDto);

    ivs.Update(vehicle);

    return Results.Ok();

}).WithTags("Vehicles");

app.MapDelete("/vehicles/{id}", ([FromRoute] int id , IVehicleService ivs) =>
{
    var vehicle = ivs.FindById(id);
    if (vehicle == null)
    {
        return Results.NotFound();

    }

    ivs.Delete(vehicle);

    return Results.NoContent();
}).WithTags("Vehicles");
#endregion

#region App
app.MapOpenApi();
app.MapScalarApiReference("/docs");

app.Run();
#endregion