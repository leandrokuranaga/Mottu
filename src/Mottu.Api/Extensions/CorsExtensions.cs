using System.Diagnostics.CodeAnalysis;

namespace Mottu.Api.Extensions
{
    [ExcludeFromCodeCoverage]
    public static class CorsExtensions
    {
        public static void AddGlobalCorsPolicy(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigins", builder =>
                {
                    builder.AllowAnyOrigin()
                           .AllowAnyHeader()
                           .AllowAnyMethod();
                });
            });
        }

        public static void UseGlobalCorsPolicy(this IApplicationBuilder app)
        {
            app.UseCors("AllowAllOrigins");
        }
    }
}
