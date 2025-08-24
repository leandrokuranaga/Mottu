using Microsoft.AspNetCore.Http;
using Mottu.Application.Common;
using Mottu.Application.Courier.Models.Request;
using Mottu.Application.User.Models.Response;

namespace Mottu.Application.Courier.Services
{
    public interface IUserService
    {
        Task<CourierResponse> CreateCourier(CreateCourierRequest request);
        Task<BaseResponse<object>> UploadCNHPhoto(int id, IFormFile file);
        Task<CourierResponse> GetCourier(int id);
    }
}
