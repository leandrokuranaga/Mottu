using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Mottu.Domain.OutboxAggregate;
using Mottu.Infra.Data;
using Rebus.Bus;
using System.Text.Json;

namespace Mottu.Infra.HostedService
{
    public class OutboxProcessor(IServiceProvider sp, ILogger<OutboxProcessor> log) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                try
                {
                    using var scope = sp.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<Context>();
                    var bus = scope.ServiceProvider.GetRequiredService<IBus>();

                    var msgs = await db.Set<Outbox>()
                        .Where(x => x.ProcessedOn == null)
                        .OrderBy(x => x.OccuredOn)
                        .Take(50)
                        .ToListAsync(ct);

                    foreach (var msg in msgs)
                    {
                        try
                        {
                            await PublishFromOutbox(bus, msg, ct);
                            msg.MarkProcessed();
                            await db.SaveChangesAsync(ct);
                        }
                        catch (Exception ex)
                        {
                            log.LogError(ex, "Erro publicando outbox {Id}", msg.Id);
                            msg.MarkFailed(ex.Message);
                            await db.SaveChangesAsync(ct);
                        }
                    }
                }
                catch (Exception loopEx)
                {
                    log.LogError(loopEx, "Erro no loop do OutboxProcessor");
                }

                await Task.Delay(TimeSpan.FromSeconds(5), ct);
            }
        }

        private static async Task PublishFromOutbox(IBus bus, Outbox msg, CancellationToken ct)
        {
            var type = Type.GetType(msg.Type, throwOnError: true)!;
            var body = JsonSerializer.Deserialize(msg.Content, type)!;

            await bus.Publish(body);
        }
    }

}
