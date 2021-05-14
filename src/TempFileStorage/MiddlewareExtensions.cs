using System;
using Microsoft.AspNetCore.Builder;

namespace TempFileStorage
{
    public interface ITempFileStorageBuilder
    {
        IApplicationBuilder Builder { get; }
        TempFileStorageOptions Options { get; }
    }

    internal class TempFileStorageBuilder : ITempFileStorageBuilder
    {
        public TempFileStorageBuilder(IApplicationBuilder builder, TempFileStorageOptions options)
        {
            Builder = builder;
            Options = options;
        }

        public IApplicationBuilder Builder { get; }
        public TempFileStorageOptions Options { get; }
    }

    public class TempFileStorageOptions
    {
        public TempFileStorageOptions()
        {
            DownloadFilePath = "/download-file";
            UploadFilePath = "/upload-file";
        }

        /// <summary>
        /// Gets or sets the path for downloading a file. (default: "/download-file")
        /// </summary>
        public string DownloadFilePath { get; set; }

        /// <summary>
        /// Gets or sets the path for file upload. (default: "/upload-file")
        /// </summary>
        public string UploadFilePath { get; set; }
    }

    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseTempFiles(this IApplicationBuilder builder)
            => builder.UseTempFiles(tempFileBuilder => { });

        public static IApplicationBuilder UseTempFiles(this IApplicationBuilder builder, Action<ITempFileStorageBuilder> extraOptions)
        {
            var customBuilder = new TempFileStorageBuilder(builder, new TempFileStorageOptions());
            extraOptions.Invoke(customBuilder);

            return builder.UseMiddleware<TempFileMiddleware>();
        }
    }
}
