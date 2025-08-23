using Mottu.Application.Motorcycle.Models.Request;
using Swashbuckle.AspNetCore.Filters;
using System.Diagnostics.CodeAnalysis;

namespace Mottu.Api.SwaggerExamples.Motorcycles
{
    [ExcludeFromCodeCoverage]
    public class CreateMotorcycleRequestExample : IExamplesProvider<CreateMotorcycleRequest>
    {
        public CreateMotorcycleRequest GetExamples()
        {
            return new CreateMotorcycleRequest
            {
                Year = 2020,
                Brand = "Suzuki",
                LicensePlate = "ABD1234"
            };
        }
    }
}