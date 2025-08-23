using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Mottu.Domain.MotorcycleAggregate;
using Mottu.Domain.OutboxAggregate;
using Mottu.Domain.RentalAggregate;
using Mottu.Domain.SeedWork;
using Mottu.Domain.UserAggregate;
using Mottu.Infra.Data;
using Mottu.Infra.Data.Repositories;
using Mottu.Infra.HostedService;
using Mottu.Infra.Storage;
using Rebus.Config;
using Rebus.Retry.Simple;

namespace Mottu.Infra.CrossCutting.IoC
{
    public static class NativeInjector
    {
        public static void AddCustomServices(this IServiceCollection services, IConfiguration configuration)
        {
            AddLocalServices(services, configuration);
            AddDatabase(services, configuration);
            AddServiceBus(services, configuration);
            AddStorageService(services, configuration);
        }

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

            services.AddHostedService<OutboxProcessor>();

            #region Repositories
            services.AddScoped<IOutboxRepository, OutboxRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRentalRepository, RentalRepository>();
            services.AddScoped<IMotorcycleRepository, MotorcycleRepository>();

            #endregion

            #region Services


            #endregion
        }

        public static void AddStorageService(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<ObjectStorageOptions>(
            configuration.GetSection("ObjectStorage"));

            services.AddSingleton<IObjectStorage, MinioObjectStorage>();
        }


        public static IServiceCollection AddServiceBus(this IServiceCollection services, IConfiguration configuration)
        {
            var rabbitMqConn = configuration.GetConnectionString("RabbitMq");

            services.AddRebus(cfg => cfg
                .Transport(t => t.UseRabbitMq(rabbitMqConn, inputQueueName: "mottu-queue"))
                .Options(o => o.RetryStrategy("mottu-error", maxDeliveryAttempts: 1)));

            return services;
        }

    }
}
