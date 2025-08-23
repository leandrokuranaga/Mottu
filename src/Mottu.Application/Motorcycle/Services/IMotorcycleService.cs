using Mottu.Application.Common;
using Mottu.Application.Motorcycle.Models.Request;
using Mottu.Application.Motorcycle.Models.Response;

namespace Mottu.Application.Motorcycle.Services
{
    public interface IMotorcycleService
    {
        Task<MotorcycleResponse> CreateMotorcycle(CreateMotorcycleRequest request);
        Task<List<MotorcycleResponse>> GetMotorcycles(string? licensePlate);
        Task<BaseResponse<object>> UpdateLicensePlate(int id, string newLicensePlate);
        Task<MotorcycleResponse> GetMotorcycle(int id);
        Task<BaseResponse<object>> DeleteMotorcycle(int id);
    }
}
