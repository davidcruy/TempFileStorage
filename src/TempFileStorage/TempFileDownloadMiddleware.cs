using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.WebUtilities;
using System.Text.Json;
using Microsoft.Extensions.Options;

namespace TempFileStorage;

public class TempFileDownloadMiddleware
{
    private readonly RequestDelegate _next;

    public TempFileDownloadMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, ITempFileStorage storage)
    {
        if (context.Request.Method.Equals("GET"))
        {
            if (!context.Request.Query.TryGetValue("key", out var fileKeys) || !await storage.ContainsKey(fileKeys[0]))
            {
                throw new Exception("TempFileStorage download key is required.");
            }

            var fileInfo = await storage.GetFileInfo(fileKeys[0]);
            var content = await storage.Download(fileKeys[0]);

            context.Response.ContentType = "application/octet-stream";
            context.Response.Headers.Add("content-disposition", new[] { $"attachment;filename=\"{fileInfo.Filename}\"" });
            context.Response.ContentLength = content.Length;

            await context.Response.Body.WriteAsync(content);
            return;
        }

        await _next(context);
    }
}