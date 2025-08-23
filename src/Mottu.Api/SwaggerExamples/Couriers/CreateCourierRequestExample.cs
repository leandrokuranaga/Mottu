using Mottu.Application.Courier.Models.Request;
using Mottu.Domain.UserAggregate.Enums;
using Swashbuckle.AspNetCore.Filters;
using System.Diagnostics.CodeAnalysis;

namespace Mottu.Api.SwaggerExamples.Couriers
{
    [ExcludeFromCodeCoverage]
    public class CreateCourierRequestExample : IExamplesProvider<CreateCourierRequest>
    {
        public CreateCourierRequest GetExamples()
        {
            return new CreateCourierRequest
            {
                Name = "João",
                CNPJ = "82959640000198",
                BirthdayDate = DateOnly.FromDateTime(DateTime.Now.AddYears(-18)),
                CNH = "12345678900",
                TypeCNH = ECNH.A
            };
        }
    }
}