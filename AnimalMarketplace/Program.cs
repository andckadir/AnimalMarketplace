using AnimalMarketplace.Extensions;
using FluentValidation;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddValidatorsFromAssemblyContaining<Program>();

// ------------------------ Extensions ------------------------
builder.Services.AddRepositories();                         
builder.Services.AddCustomServices();
builder.Services.AddDatabaseServices(builder.Configuration);
builder.Services.AddCorsConfiguration();
builder.Services.AddSwaggerConfiguration();
builder.Services.AddJwtAuthentication(builder.Configuration);
// ------------------------------------------------------------

builder.Services.AddControllers();
builder.Services.AddAuthorization();

var app = builder.Build();

// --- 6. Middleware Pipeline ---
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
app.UseCors("AllowReactApp");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();