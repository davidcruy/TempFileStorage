using Microsoft.AspNetCore.Builder;

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
                applicationBuilder.UseMiddleware<TempFileDownloadMiddleware>();
            })
            .Map(customBuilder.Options.UploadFilePattern, applicationBuilder =>
            {
                applicationBuilder.UseMiddleware<TempFileUploadMiddleware>();
            });
    }
}