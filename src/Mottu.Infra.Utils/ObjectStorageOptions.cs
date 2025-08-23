namespace Mottu.Infra.Utils
{
    public sealed class ObjectStorageOptions
    {
        public string Endpoint { get; set; } = default!;
        public string AccessKey { get; set; } = default!;
        public string SecretKey { get; set; } = default!;
        public string Bucket { get; set; } = default!;
    }
}
