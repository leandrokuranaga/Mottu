using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Mottu.Domain.OutboxAggregate;
using Rebus.Bus;
using System.Text.Json;

namespace Mottu.Infra.HostedService
{
    public class OutboxProcessorService(
           ILogger<OutboxProcessorService> logger,
           IServiceProvider serviceProvider,
           IBus bus,
           IConfiguration configuration
           ) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await ProcessNextOutboxMessage();
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error in outbox processor main loop.");
                }
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }

        private async Task ProcessNextOutboxMessage()
        {
            using var scope = serviceProvider.CreateScope();
            var outboxRepository = scope.ServiceProvider.GetRequiredService<IOutboxRepository>();

            var outboxes = await outboxRepository.GetNoTrackingAsync(x => x.ProcessedOn == null);
            var outbox = outboxes.FirstOrDefault();

            var queueName = configuration["ServiceBus:QueueName"];

            if (outbox == null)
            {
                logger.LogDebug("No unprocessed outbox messages found");
                return;
            }

            logger.LogInformation("Processing outbox message {OutboxId} with type {Type}", outbox.Id, outbox.Type);

            var success = false;

            try
            {
                if (string.IsNullOrWhiteSpace(outbox.Content))
                {
                    logger.LogWarning("Empty content for outbox message {OutboxId}. Marking as processed.", outbox.Id);
                    success = true;
                }
                else if (outbox.Type == "object")
                {
                    try
                    {
                        JsonDocument.Parse(outbox.Content);

                        logger.LogInformation("Publishing generic JSON message {OutboxId} to bus", outbox.Id);
                        await bus.Advanced.Routing.Send(queueName, outbox.Content);
                        logger.LogInformation("Successfully published message {OutboxId}", outbox.Id);

                        success = true;
                    }
                    catch (JsonException)
                    {
                        logger.LogError("Invalid JSON for outbox message {OutboxId}. Marking as processed.", outbox.Id);
                        success = true;
                    }
                }
                else
                {
                    var eventType = Type.GetType(outbox.Type);
                    if (eventType == null)
                    {
                        logger.LogWarning("Event type {EventType} not found for outbox message {OutboxId}. Marking as processed.", outbox.Type, outbox.Id);
                        success = true;
                    }
                    else
                    {
                        var eventMessage = JsonSerializer.Deserialize(outbox.Content, eventType);
                        if (eventMessage == null)
                        {
                            logger.LogWarning("Failed to deserialize content for outbox message {OutboxId}. Marking as processed.", outbox.Id);
                            success = true;
                        }
                        else
                        {
                            await bus.Publish(eventMessage);
                            logger.LogInformation("Successfully published typed message {OutboxId}", outbox.Id);
                            success = true;
                        }
                    }
                }

                if (success)
                {
                    outbox.ProcessedOn = DateTime.UtcNow;
                    await outboxRepository.UpdateAsync(outbox);
                    await outboxRepository.SaveChangesAsync();
                    await outboxRepository.CommitAsync();

                    logger.LogInformation("Message {OutboxId} marked as processed and saved", outbox.Id);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error processing outbox message {OutboxId}. Will retry later.", outbox.Id);
            }
        }
    }
}
