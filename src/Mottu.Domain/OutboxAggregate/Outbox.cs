using Abp.Domain.Entities;

namespace Mottu.Domain.OutboxAggregate
{
    public class Outbox : Entity<Guid>
    {
        public string Type { get; set; }
        public string Content { get; set; }
        public DateTime OccuredOn { get; set; }
        public DateTime? ProcessedOn { get; set; }

        public Outbox()
        {

        }

        public Outbox(string type, string content, DateTime occuredOn, DateTime? processedOn = null)
        {
            Type = type;
            Content = content;
            OccuredOn = occuredOn;
            ProcessedOn = processedOn;
        }
    }
}
