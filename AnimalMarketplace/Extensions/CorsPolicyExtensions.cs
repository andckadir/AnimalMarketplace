namespace AnimalMarketplace.Extensions;

public static class CorsPolicyExtensions
{
    public static void AddCorsConfiguration(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("AllowReactApp",
                b => b.WithOrigins(
                        "https://localhost:5173",
                        "http://localhost:3000"
                    )
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());
        });
    }
}