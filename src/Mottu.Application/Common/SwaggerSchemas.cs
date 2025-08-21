using System.Diagnostics.CodeAnalysis;

namespace Mottu.Application.Common
{
    [ExcludeFromCodeCoverage]

    public class SuccessResponse<T>
    {
        public bool Success { get; set; } = true;
        public T Data { get; set; } = default!;
        public object? Error { get; set; } = null;

    }
}
