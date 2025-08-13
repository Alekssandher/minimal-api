using System.ComponentModel;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using minimal_api.Domain.DTOs;
using minimal_api.Domain.Entities;
using minimal_api.Domain.Interfaces;
using minimal_api.Domain.ModelViews;
using minimal_api.Domain.Service;
using minimal_api.Infrastructure.Db;
using Scalar.AspNetCore;

namespace minimal_api
{
    public class Startup
    {
        private static string jwtKey = "";
        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;

            jwtKey = _configuration["JwtSettings:Key"] ?? throw new Exception("Jwt key is missing in AppSettings");
        }

        public IConfiguration? _configuration { get; set; }

        
        public void ConfigureServices(IServiceCollection services)
        {
            if (_configuration == null)
                throw new Exception("Configuration is Null");



            _ = services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = "alek@hotmail.com",
                        ValidAudience = "alek@hotmail.com",
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
                    };
                });

            _ = services.AddAuthorization();
            _ = services.AddAuthorization();
           
            _ = services.AddOpenApi(options =>
            {
                _ = options.AddDocumentTransformer((document, context, cancellationToken) =>
                {
                    document.Info = new()
                    {
                        Title = "Minimal API Documentation",
                        Version = "v1",
                        Description = "A Minimal API Example."
                    };

                    document.Components ??= new();

                    document.Components.SecuritySchemes["Bearer"] = new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                    {
                        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
                        Scheme = "bearer",
                        BearerFormat = "JWT",
                        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\""
                    };

                    return Task.CompletedTask;
                });


            });


            _ = services.AddScoped<IAdminService, AdminService>();
            _ = services.AddScoped<IVehicleService, VehicleService>();

            _ = services.AddDbContext<MyDbContext>(options =>
            {
                _ = options.UseMySql(
                    _configuration.GetConnectionString("mysql"),
                    ServerVersion.AutoDetect(_configuration.GetConnectionString("mysql"))
                );
            });

        }


        private static string GenerateJwt(Admin admin)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Email, admin.Email),
                new Claim(JwtRegisteredClaimNames.Profile, admin.Profile)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                    issuer: "alek@hotmail.com",
                    audience: "alek@hotmail.com",
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(30),
                    signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public void Configure(WebApplication app)
        {
            _ = app.MapOpenApi();
            _ = app.MapScalarApiReference("/docs");
            _ = app.UseRouting();
            _ = app.UseAuthorization();
            _ = app.UseAuthentication();
            
            _ = app.UseEndpoints(endpoints =>
            {
                #region Home
                _ = endpoints.MapGet("/", () => Results.Json(new Home())).WithTags("Home");
                #endregion

                #region Admin
                _ = endpoints.MapPost("admin/login", ([FromBody] LoginDto loginDto, IAdminService ias) =>
                {
                    var result = ias.Login(loginDto);

                    if (result == null)
                    {
                        return Results.Unauthorized();
                    }

                    try
                    {
                        var token = GenerateJwt(result);

                        return Results.Ok(token);
                    }
                    catch (Exception)
                    {
                        return Results.Unauthorized();

                    }
                }).WithTags("Admins");


                _ = endpoints.MapGet("admin", [Authorize] ([FromQuery][DefaultValue(1)] int page, IAdminService ias) =>
                {
                    var result = ias.All(page);

                    return Results.Ok(result);
                }).RequireAuthorization().RequireAuthorization(new AuthorizeAttribute { Roles = "Admin" }).WithTags("Admins");

                _ = endpoints.MapGet("admin/{id}", [Authorize] ([FromRoute] int id, IAdminService ias) =>
                {
                    return Results.Ok(ias.FindById(id));
                }).RequireAuthorization().RequireAuthorization(new AuthorizeAttribute { Roles = "Admin" }).WithTags("Admins");

                _ = endpoints.MapPost("admin/create", ([FromBody] AdminDto adminDto, IAdminService ias) =>
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

                }).RequireAuthorization().RequireAuthorization(new AuthorizeAttribute { Roles = "Admin" }).WithTags("Admins");
                #endregion

                #region Vehicles
                static ValidationErrors ValidateVehicle(VehicleDto vehicleDto)
                {
                    var validation = new ValidationErrors
                    {
                        Messages = []
                    };

                    if (string.IsNullOrEmpty(vehicleDto.Name))
                        validation.Messages.Add("Name não pode estar vazio ou nulo");

                    if (string.IsNullOrEmpty(vehicleDto.Brand))
                        validation.Messages.Add("Brand não pode estar vazia ou nula");

                    if (vehicleDto.Year < 1950)
                        validation.Messages.Add("Veículo muito antigo, somente anos superiores a 1950 são permitidos");

                    return validation;
                }
                _ = endpoints.MapPost("/vehicles", ([FromBody] VehicleDto vehicleDto, IVehicleService ivs) =>
                {
                    var v = ValidateVehicle(vehicleDto);
                    if (v.Messages.Count > 0)
                        return Results.BadRequest(v.Messages);

                    var vehicleEntity = vehicleDto.ToEntity();

                    ivs.Include(vehicleEntity);

                    return Results.Created($"/vehicle/{vehicleEntity.Id}", vehicleEntity);
                }
                ).RequireAuthorization().RequireAuthorization(new AuthorizeAttribute { Roles = "Editor" }).WithTags("Vehicles");

                _ = endpoints.MapGet("/vehicles/{id}", ([FromRoute] int id, IVehicleService ivs) =>
                {
                    var vehicle = ivs.FindById(id);

                    if (vehicle == null)
                    {
                        return Results.NotFound();
                    }

                    return Results.Ok(vehicle);
                }
                ).RequireAuthorization().RequireAuthorization(new AuthorizeAttribute { Roles = "Editor" }).WithTags("Vehicles");

                _ = endpoints.MapGet("/vehicles", ([FromQuery][DefaultValue(1)] int page, [FromQuery] string? name, [FromQuery] string? brand, IVehicleService ivs) =>
                {
                    var vehicles = ivs.All(page, name, brand);

                    return Results.Ok(vehicles);
                }
                ).RequireAuthorization().RequireAuthorization(new AuthorizeAttribute { Roles = "Editor" }).WithTags("Vehicles");

                _ = endpoints.MapPut("/vehicles/{id}", ([FromRoute] int id, [FromBody] VehicleDto vehicleDto, IVehicleService ivs) =>
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

                }).RequireAuthorization().RequireAuthorization(new AuthorizeAttribute { Roles = "Admin" }).WithTags("Vehicles");

                _ = endpoints.MapDelete("/vehicles/{id}", ([FromRoute] int id, IVehicleService ivs) =>
                {
                    var vehicle = ivs.FindById(id);
                    if (vehicle == null)
                    {
                        return Results.NotFound();

                    }

                    ivs.Delete(vehicle);

                    return Results.NoContent();
                }).RequireAuthorization().RequireAuthorization(new AuthorizeAttribute { Roles = "Admin" }).WithTags("Vehicles");
                #endregion
            });
            app.Run();
        }
    }
}