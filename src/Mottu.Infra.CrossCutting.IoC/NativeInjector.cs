using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Mottu.Domain.OutboxAggregate;
using Mottu.Domain.SeedWork;
using Mottu.Infra.Data;
using Mottu.Infra.Data.Repositories;
using Mottu.Infra.HostedService;
using Mottu.Infra.Utils;
using Rebus.Config;
using System.Text;

namespace Mottu.Infra.CrossCutting.IoC
{
    public static class NativeInjector
    {

        public static void AddDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<Context>(options =>
                options.UseNpgsql(connectionString));
        }

        public static void AddLocalServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<INotification, Notification>();

            services.AddHostedService<OutboxProcessorService>();

            #region Repositories
            services.AddScoped<IOutboxRepository, OutboxRepository>();

            #endregion

            #region Services


            #endregion
        }

        public static void AddCustomAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                var jwtSection = configuration.GetSection(nameof(JwtSettings));
                var secretKey = jwtSection["SecretKey"];

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                };
            });

            services.AddAuthorization();
        }

        public static IServiceCollection AddServiceBus(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddRebus(cfg => cfg
                .Transport(t => t.UseRabbitMqAsOneWayClient(configuration.GetConnectionString("RabbitMq"))));

            return services;
        }

    }
}
