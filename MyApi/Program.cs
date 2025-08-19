using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Npgsql;
using System.Data;
using MyApp.Application.Services;
using MyApp.Domain.Repositories;
using MyApp.Infrastructure.Repositories;
using System.Data.SqlClient;

var builder = WebApplication.CreateBuilder(args);

// 🔹 Conexión a PostgreSQL
builder.Services.AddScoped<IDbConnection>(sp =>
    new NpgsqlConnection(builder.Configuration.GetConnectionString("DefaultConnection")));

// 🔹 Clave secreta para validar el token JWT
var secretKey = builder.Configuration["JwtSettings:SecretKey"];
var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

// 🔹 Configuración de autenticación JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = key,

            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero // Opcional: elimina retrasos en expiración
        };

        options.Events = new JwtBearerEvents
        {
            OnChallenge = context =>
            {
                context.HandleResponse();

                if (!context.Response.HasStarted)
                {
                    context.Response.StatusCode = 401;
                    context.Response.ContentType = "application/json";

                    return context.Response.WriteAsync(Newtonsoft.Json.JsonConvert.SerializeObject(new
                    {
                        message = "No autorizado. Debe proporcionar un token válido en el encabezado Authorization."
                    }));
                }

                return Task.CompletedTask;
            },

            OnAuthenticationFailed = context =>
            {
                if (!context.Response.HasStarted)
                {
                    context.Response.StatusCode = 401;
                    context.Response.ContentType = "application/json";

                    string errorMessage = context.Exception?.Message;

                    if (context.Exception is SecurityTokenExpiredException)
                        errorMessage = "El token ha expirado.";
                    else if (context.Exception is SecurityTokenInvalidSignatureException)
                        errorMessage = "El token tiene una firma inválida.";

                    return context.Response.WriteAsync(Newtonsoft.Json.JsonConvert.SerializeObject(new
                    {
                        message = "Token inválido o expirado.",
                        detail = errorMessage
                    }));
                }

                return Task.CompletedTask;
            }
        };
    });


// 🔹 Configuración de autorizaciones
builder.Services.AddAuthorization();

// 🔹 Política de CORS para permitir solo el acceso desde la app React local
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
        policy =>
        {
            policy.WithOrigins("http://localhost:3000")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

// 🔹 Registrar servicios
//builder.Services.AddScoped<IAuthService, AuthService>();
//builder.Services.AddScoped<ITokenService, TokenService>();
//builder.Services.AddScoped<IProductoService, ProductoService>();
//builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
//builder.Services.AddScoped<IProductoRepository, ProductoRepository>();
builder.Services.AddSingleton<IDbConnection>(sp =>
    new NpgsqlConnection("Host=localhost;Database=mi_basededatos;Username=admin;Password=admin123"));
builder.Services.AddSingleton<IAuthService, AuthService>();
builder.Services.AddSingleton<ITokenService, TokenService>();
builder.Services.AddSingleton<IProductoService, ProductoService>();
builder.Services.AddSingleton<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddSingleton<IProductoRepository, ProductoRepository>();

// 🔹 Configuración de Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Mi API Local", Version = "v1" });

    // Seguridad para Swagger UI
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Ingrese 'Bearer' seguido de un espacio y su token JWT.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

builder.Logging.AddConsole();
builder.Logging.AddDebug();

builder.Services.AddControllers();

var app = builder.Build();

// 🔹 Middleware
app.UseHttpsRedirection();

// 🔹 Manejo global de errores
app.UseMiddleware<ErrorHandlingMiddleware>();

// 🔹 CORS
app.UseCors("AllowReactApp");

// 🔹 Autenticación y autorización
app.UseAuthentication();
app.UseAuthorization();

// 🔹 Swagger siempre habilitado (puedes quitar la condición si no quieres limitarlo a Development)
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Mi API Local v1");
    c.RoutePrefix = "swagger"; // o usa string.Empty si quieres que esté en la raíz
});

// 🔹 Mapear controladores
app.MapControllers();

// 🔹 Ejecutar la aplicación
app.Run();
