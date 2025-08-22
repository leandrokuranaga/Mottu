using Abp.Domain.Entities;

namespace Mottu.Domain.OutboxAggregate;

public class Outbox : Entity<Guid>
{
    public string Type { get; private set; } = null!;
    public string Content { get; private set; } = null!;
    public DateTime OccuredOn { get; private set; }
    public DateTime? ProcessedOn { get; private set; }

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

    public void MarkProcessed()
        => ProcessedOn = DateTime.UtcNow;

    public void MarkFailed(string errorMessage)
    {
        var suffix = $"\n/* ERROR @ {DateTime.UtcNow:O}: {errorMessage} */";
        Content = Content.Length + suffix.Length <= 4000
            ? Content + suffix
            : Content;
    }
}
