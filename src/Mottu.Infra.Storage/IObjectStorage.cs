using Microsoft.AspNetCore.Http;

namespace Mottu.Infra.Storage
{
    public interface IObjectStorage
    {
        Task<string> UploadAsync(
          IFormFile file,
          string objectName,
          CancellationToken ct = default);

        Task<bool> ExistsAsync(string objectName, CancellationToken ct = default);

        Task DeleteAsync(string objectName, CancellationToken ct = default);

        Task<string> GetPresignedReadUrlAsync(
            string objectName,
            TimeSpan? expiry = null,
            CancellationToken ct = default);
    }
}
