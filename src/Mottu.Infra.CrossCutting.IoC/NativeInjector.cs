using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Mottu.Domain.OutboxAggregate;
using Mottu.Domain.SeedWork;
using Mottu.Infra.Data;
using Mottu.Infra.HostedService;

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
            services.AddRebus(configure => configure
                .Transport(t => t.UseAmazonSQSAsOneWayClient(
                    accessKeyId: configuration["ServiceBus:AccessKey"],
                    secretAccessKey: configuration["ServiceBus:SecretKey"],
                    regionEndpoint: Amazon.RegionEndpoint.GetBySystemName(configuration["ServiceBus:Region"])))
                .Routing(r => r.TypeBased()
                    .MapFallback(configuration["ServiceBus:QueueName"])));

            return services;
        }

    }
}
