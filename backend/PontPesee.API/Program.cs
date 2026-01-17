using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PontPesee.API.Data;
using PontPesee.API.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Using QuestPDF 2022.x which doesn't require explicit license configuration here.

// Configuration de la base de données MySQL
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySQL(connectionString ?? throw new InvalidOperationException("Connection string not found.")));

// Configuration JWT Authentication
var jwtKeyFromConfig = builder.Configuration["Jwt:Key"];
Console.WriteLine($"DEBUG: Jwt:Key from config = {jwtKeyFromConfig ?? "NULL"}");

var jwtKey = jwtKeyFromConfig ?? throw new InvalidOperationException("JWT Key not configured.");
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];

// Debug: Afficher la configuration JWT au démarrage
Console.WriteLine("=== JWT Configuration ===");
Console.WriteLine($"JWT Key Length: {jwtKey.Length} caractères");
Console.WriteLine($"JWT Key Value: {jwtKey.Substring(0, Math.Min(20, jwtKey.Length))}...");
Console.WriteLine($"JWT Issuer: {jwtIssuer}");
Console.WriteLine($"JWT Audience: {jwtAudience}");
Console.WriteLine("========================");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ClockSkew = TimeSpan.Zero // Pas de tolérance sur l'expiration
        };

        // Events pour debug
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("❌ JWT Authentication FAILED");
                Console.WriteLine($"   Error: {context.Exception.Message}");
                Console.WriteLine($"   Exception Type: {context.Exception.GetType().Name}");
                if (context.Exception.InnerException != null)
                {
                    Console.WriteLine($"   Inner Exception: {context.Exception.InnerException.Message}");
                }
                Console.ResetColor();
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("✅ JWT Token validated successfully");
                var username = context.Principal?.Identity?.Name;
                Console.WriteLine($"   User: {username}");
                Console.ResetColor();
                return Task.CompletedTask;
            },
            OnMessageReceived = context =>
            {
                if (!string.IsNullOrEmpty(context.Token))
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"📨 JWT Token received: {context.Token.Substring(0, Math.Min(50, context.Token.Length))}...");
                    Console.ResetColor();
                }
                return Task.CompletedTask;
            },
            OnChallenge = context =>
            {
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine("⚠️  JWT Challenge triggered");
                Console.WriteLine($"   Error: {context.Error}");
                Console.WriteLine($"   Error Description: {context.ErrorDescription}");
                Console.ResetColor();
                return Task.CompletedTask;
            }
        };
    });

// Configuration CORS
var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? new[] { "*" };
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        if (builder.Environment.IsDevelopment())
        {
            // In development, allow any origin to simplify local testing (vite dev server ports vary)
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        }
        else
        {
            // In non-dev, restrict to configured origins and allow credentials
            policy.WithOrigins(allowedOrigins)
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .AllowCredentials();
        }
    });
});

// Enregistrement des services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IPeseeService, PeseeService>();
builder.Services.AddScoped<IStatisticsService, StatisticsService>();
// Background report queue
builder.Services.AddSingleton<IReportQueue, ReportQueue>();
builder.Services.AddHostedService<ReportBackgroundService>();
// Cleanup old generated report files
builder.Services.AddHostedService<ReportCleanupService>();

// Add controllers
builder.Services.AddControllers();

// Configuration Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Pont Pesée API",
        Version = "v1",
        Description = "API pour la gestion des pesées - Multi-sites (SOPRECI GUTRI & OLODIO)"
    });

    // Configuration JWT dans Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
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
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Pont Pesée API v1");
        c.RoutePrefix = string.Empty; // Swagger accessible à la racine
    });
}

// HTTPS redirection désactivée en dev pour permettre les tests sur HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

// Activer CORS
app.UseCors("AllowFrontend");

// Authentification et autorisation
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
