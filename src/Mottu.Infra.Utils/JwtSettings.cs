using System.Diagnostics.CodeAnalysis;

namespace Mottu.Infra.Utils
{
    [ExcludeFromCodeCoverage]
    public class JwtSettings
    {
        public string SecretKey { get; set; }
    }
}
