using Mottu.Application.Common;
using Mottu.Application.Rent.Models.Request;
using Mottu.Application.Rent.Models.Response;

namespace Mottu.Application.Rent.Services
{
    public interface IRentService
    {
        Task<BaseResponse<object>> RentMotorcycle(RentRequest request);
        Task<RentResponse> GetMotorcycleRent(int id);
        Task<BaseResponse<object>> ReturnMotorcycle(int id, DateTime returnDate);
    }
}
