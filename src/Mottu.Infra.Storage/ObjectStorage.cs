using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;
using Mottu.Infra.Storage;
using Mottu.Infra.Utils;
using System.Diagnostics.CodeAnalysis;

[ExcludeFromCodeCoverage]
public sealed class MinioObjectStorage : IObjectStorage
{
    private readonly IMinioClient _minio;
    private readonly ObjectStorageOptions _opt;

    public MinioObjectStorage(IOptions<ObjectStorageOptions> options)
    {
        _opt = options.Value;

        _minio = new MinioClient()
            .WithEndpoint(_opt.Endpoint)
            .WithCredentials(_opt.AccessKey, _opt.SecretKey)
            .WithSSL(false)
            .Build();
    }

    private async Task EnsureBucketAsync(CancellationToken ct)
    {
        var be = await _minio.BucketExistsAsync(
            new BucketExistsArgs().WithBucket(_opt.Bucket), ct);

        if (!be)
        {
            await _minio.MakeBucketAsync(
                new MakeBucketArgs().WithBucket(_opt.Bucket), ct);
        }
    }

    public async Task<string> UploadAsync(
        IFormFile file, string objectName, CancellationToken ct = default)
    {
        if (file == null || file.Length == 0)
            throw new ArgumentException("Empty content.");

        await EnsureBucketAsync(ct);

        var putArgs = new PutObjectArgs()
            .WithBucket(_opt.Bucket)
            .WithObject(objectName)
            .WithStreamData(file.OpenReadStream())
            .WithObjectSize(file.Length);

        await _minio.PutObjectAsync(putArgs, ct);

        return objectName;
    }
}
