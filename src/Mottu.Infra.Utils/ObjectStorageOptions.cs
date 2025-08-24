using System.Diagnostics.CodeAnalysis;

namespace Mottu.Infra.Utils
{
    [ExcludeFromCodeCoverage]
    public sealed class ObjectStorageOptions
    {
        public string Endpoint { get; set; } = default!;
        public string AccessKey { get; set; } = default!;
        public string SecretKey { get; set; } = default!;
        public string Bucket { get; set; } = default!;
    }
}
