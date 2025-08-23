using Microsoft.AspNetCore.Http;
using Mottu.Application.Common;
using Mottu.Application.Courier.Models.Request;

namespace Mottu.Application.Courier.Services
{
    public interface IUserService
    {
        Task<BaseResponse<object>> CreateCourier(CreateCourierRequest request);
        Task<BaseResponse<object>> UploadCNHPhoto(int id, IFormFile file);
    }
}
