using Mottu.Application.Rent.Models.Request;
using Swashbuckle.AspNetCore.Filters;
using System.Diagnostics.CodeAnalysis;

namespace Mottu.Api.SwaggerExamples.Rentals
{
    [ExcludeFromCodeCoverage]
    public class RentRequestExample : IExamplesProvider<RentRequest>
    {
        public RentRequest GetExamples()
        {
            var today = DateTime.UtcNow.Date;

            return new RentRequest
            {
                IdCourier = 3,
                IdMotorcycle = 2,
                StartDate = DateOnly.FromDateTime(today.AddDays(1)),
                ForecastEndDate = DateOnly.FromDateTime(today.AddDays(7)),
                Plan = 7
            };
        }
    }
}