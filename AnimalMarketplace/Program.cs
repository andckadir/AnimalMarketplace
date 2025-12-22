using System.Text;
using Microsoft.EntityFrameworkCore;
using AnimalMarketplace.Database.DbContexts;
using AnimalMarketplace.Repositories.Implementations;
using AnimalMarketplace.Repositories.Interfaces;
using AnimalMarketplace.Services;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// --- 1. Servis Kayıtları ---
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ISellerRepository, SellerRepository>();
builder.Services.AddScoped<IAdvertRepository, AdvertRepository>();
builder.Services.AddScoped<IImageService, CloudinaryImageService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ISellerService, SellerService>();

builder.Services.AddControllers();

// --- 2. Veritabanı Bağlantısı ---
var connectionString = builder.Configuration.GetConnectionString("Neon");

Console.WriteLine("--------------------------------------------------");
Console.WriteLine("OKUNAN BAĞLANTI ADRESİ:");
Console.WriteLine(connectionString);
Console.WriteLine("--------------------------------------------------");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

// --- 3. CORS AYARLARI (GÜNCELLENDİ) ---
// Vite varsayılan olarak 5173 portunu kullanır. 
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
        b => b.WithOrigins(
                "http://localhost:5173", // Vite React (ÖNEMLİ: Burası düzeltildi)
                "http://localhost:3000"  // Yedek olarak kalsın (Create-React-App kullanırsan)
             )
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials());
});
// --------------------------------------

// --- 4. Swagger / OpenAPI ---
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT token'ınızı girin"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
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

// --- 5. JWT Authentication ---
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Secret"])),
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["JwtSettings:Audience"],
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// --- 6. Middleware Pipeline ---

// Scalar UI
app.UseSwagger(options => { options.RouteTemplate = "openapi/{documentName}.json"; });
app.MapScalarApiReference(options =>
{
    options.Title = "Animal Marketplace API";
    options.Layout = ScalarLayout.Modern;
    options.Theme = ScalarTheme.Kepler;
    options.OpenApiRoutePattern = "/openapi/v1.json";
    options.Authentication = new ScalarAuthenticationOptions
    {
        PreferredSecurityScheme = "Bearer"
    };
    options.WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
});

app.UseHttpsRedirection();

// --- CORS MİDDLEWARE (AKTİFLEŞTİRME) ---
// Sıralama çok kritiktir: UseCors -> UseAuthentication -> UseAuthorization
app.UseCors("AllowReactApp");
// ---------------------------------------

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();