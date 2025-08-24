using Microsoft.AspNetCore.Http;

namespace Mottu.Infra.Storage
{
    public interface IObjectStorage
    {
        Task<string> UploadAsync(
          IFormFile file,
          string objectName,
          CancellationToken ct = default);
    }
}
