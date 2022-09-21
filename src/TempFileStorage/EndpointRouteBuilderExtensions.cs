using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace TempFileStorage;

public static class EndpointRouteBuilderExtensions
{
    /// <summary>
    /// Enable TempFileStorage middleware to download and upload files.
    /// Default patterns:
    /// - /download-file
    /// - /upload-file
    /// </summary>
    public static IEndpointConventionBuilder MapTempFileStorage(this IEndpointRouteBuilder endpoints, string downloadPattern = "/download-file", string uploadPattern = "/upload-file")
    {
        var app = endpoints.CreateApplicationBuilder();

        var downloadPipeline = app
            .UseMiddleware<TempFileDownloadMiddleware>()
            .Build();

        endpoints.Map(downloadPattern, downloadPipeline);

        var uploadPipeline = app
            .UseMiddleware<TempFileUploadMiddleware>()
            .Build();

        return endpoints.Map(uploadPattern, uploadPipeline);
    }
}