using Mottu.Domain.UserAggregate.Enums;

namespace Mottu.Application.Courier.Models.Request
{
    public record CreateCourierRequest
    {
        public string Name { get; set; }
        public string CNPJ { get; set; }
        public DateOnly BirthdayDate { get; set; }
        public string CNH { get; set; }
        public ECNH TypeCNH { get; set; }
    }
}
