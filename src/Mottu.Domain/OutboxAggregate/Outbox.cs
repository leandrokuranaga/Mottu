using Abp.Domain.Entities;

namespace Mottu.Domain.OutboxAggregate;

public class Outbox : Entity<Guid>
{
    public string Type { get; private set; } = null!;
    public string Content { get; private set; } = null!;
    public DateTime OccuredOn { get; private set; }
    public DateTime? ProcessedOn { get; set; }

    protected Outbox() { }

    public Outbox(string type, string content, DateTime occuredOnUtc, DateTime? processedOnUtc = null)
    {
        Type = string.IsNullOrWhiteSpace(type) ? throw new ArgumentException("type") : type;
        Content = string.IsNullOrWhiteSpace(content) ? throw new ArgumentException("content") : content;
        OccuredOn = DateTime.SpecifyKind(occuredOnUtc, DateTimeKind.Utc);
        ProcessedOn = processedOnUtc is null ? null : DateTime.SpecifyKind(processedOnUtc.Value, DateTimeKind.Utc);
    }

    public static Outbox Create(string type, string content)
        => new(type, content, DateTime.UtcNow);
}
