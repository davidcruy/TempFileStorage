using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Options;

namespace TempFileStorage;

public static class MiddlewareExtensions
{
    public static IApplicationBuilder UseTempFiles(this IApplicationBuilder builder)
        => builder.UseTempFiles(tempFileBuilder => { });

    public static IApplicationBuilder UseTempFiles(this IApplicationBuilder builder, Action<ITempFileStorageBuilder> extraOptions)
    {
        var customBuilder = new TempFileStorageBuilder(builder, new TempFileStorageOptions());
        extraOptions.Invoke(customBuilder);

        return builder
            .Map(customBuilder.Options.DownloadFilePattern, applicationBuilder =>
            {
                applicationBuilder.UseMiddleware<TempFileDownloadMiddleware>(Options.Create(customBuilder.Options));
            })
            .Map(customBuilder.Options.UploadFilePattern, applicationBuilder =>
            {
                applicationBuilder.UseMiddleware<TempFileUploadMiddleware>(Options.Create(customBuilder.Options));
            });
    }
}