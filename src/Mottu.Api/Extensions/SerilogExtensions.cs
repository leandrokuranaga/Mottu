using Serilog;
using Serilog.Formatting.Json;
using System.Diagnostics.CodeAnalysis;

namespace Mottu.Api.Extensions
{
    [ExcludeFromCodeCoverage]
    public static class SerilogExtensions
    {
        public static void ConfigureSerilog(HostBuilderContext context, IServiceProvider services, LoggerConfiguration configuration)
        {
            configuration
                .MinimumLevel.Information()
                .Enrich.FromLogContext()
                .Enrich.WithProperty("Application", "Mottu.Api")
                .WriteTo.Console()
                .WriteTo.File(
                    new JsonFormatter(renderMessage: true),
                    "logs/log-.json",
                    rollingInterval: RollingInterval.Day);
        }
    }
}
