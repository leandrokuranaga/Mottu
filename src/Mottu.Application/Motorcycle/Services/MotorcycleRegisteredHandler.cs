using Microsoft.Extensions.Logging;
using Mottu.Application.Motorcycle.Models.Dto;
using Mottu.Domain.MotorcycleAggregate;
using Rebus.Handlers;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using DomainMotorcycle = Mottu.Domain.MotorcycleAggregate.Motorcycle;


namespace Mottu.Application.Motorcycle.Services
{
    [ExcludeFromCodeCoverage]
    public sealed class MotorcycleRegisteredHandler(
        IMotorcycleRepository repository,
        ILogger<string> logger
        )
        : IHandleMessages<string>
    {

        private static readonly JsonSerializerOptions JsonOpts = new()
        {
            PropertyNameCaseInsensitive = true
        };

        private sealed record Envelope<T>(string type, int version, T data);

        public async Task Handle(string messageJson)
        {
            Envelope<CreateMotorcycleMessage>? env;
            try
            {
                env = JsonSerializer.Deserialize<Envelope<CreateMotorcycleMessage>>(messageJson, JsonOpts);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Invalid JSON: {Json}", messageJson);
                return;
            }

            if (env is null)
            {
                logger.LogWarning("Null envelope. Payload: {Json}", messageJson);
                return;
            }

            if (env.type != "object" || env.version != 1)
            {
                logger.LogWarning("Ignored Message. Payload: {Json}", messageJson);
                return;
            }

            var d = env.data;

            if (d.Year == 2024)
            {
                logger.LogInformation("Special motorcycle registered: {MotorcycleId}", d.Id);
            }

            DomainMotorcycle motorcycle;

            motorcycle = DomainMotorcycle.Create(d.Year, d.Brand, d.LicensePlate);

            await repository.InsertOrUpdateAsync(motorcycle);   
            await repository.SaveChangesAsync();

        }
    }
}           