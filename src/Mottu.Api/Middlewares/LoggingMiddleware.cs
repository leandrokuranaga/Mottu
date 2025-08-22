using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Mottu.Api.Middlewares
{
    [ExcludeFromCodeCoverage]
    public class LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger)
    {
        public async Task Invoke(HttpContext context)
        {
            var sw = Stopwatch.StartNew();

            context.Request.EnableBuffering();
            var requestBody = await ReadRequestBodyAsync(context.Request);

            var originalResponseBody = context.Response.Body;
            var newResponseBody = new MemoryStream();
            context.Response.Body = newResponseBody;

            try
            {
                await next(context);
                sw.Stop();

                var responseBody = await ReadResponseBodyAsync(newResponseBody);
                newResponseBody.Seek(0, SeekOrigin.Begin);
                await newResponseBody.CopyToAsync(originalResponseBody);

                logger.LogInformation(
                    "HTTP {Method} {Path} responded {StatusCode} in {Elapsed}ms\nRequest Body: {RequestBody}\nResponse Body: {ResponseBody}",
                    context.Request.Method,
                    context.Request.Path,
                    context.Response.StatusCode,
                    sw.ElapsedMilliseconds,
                    requestBody,
                    responseBody
                );
            }
            catch (Exception ex)
            {
                sw.Stop();
                logger.LogError(ex, "Unhandled exception in {Method} {Path}", context.Request.Method, context.Request.Path);
                throw;
            }
            finally
            {
                context.Response.Body = originalResponseBody;
                newResponseBody.Dispose();
            }
        }

        private async Task<string> ReadRequestBodyAsync(HttpRequest request)
        {
            request.Body.Seek(0, SeekOrigin.Begin);
            using var reader = new StreamReader(request.Body, Encoding.UTF8, detectEncodingFromByteOrderMarks: false, leaveOpen: true);
            var body = await reader.ReadToEndAsync();
            request.Body.Seek(0, SeekOrigin.Begin);
            return body;
        }

        private async Task<string> ReadResponseBodyAsync(Stream responseBodyStream)
        {
            responseBodyStream.Seek(0, SeekOrigin.Begin);
            using var reader = new StreamReader(responseBodyStream, Encoding.UTF8, detectEncodingFromByteOrderMarks: false, leaveOpen: true);
            var body = await reader.ReadToEndAsync();
            responseBodyStream.Seek(0, SeekOrigin.Begin);
            return body;
        }
    }
}
