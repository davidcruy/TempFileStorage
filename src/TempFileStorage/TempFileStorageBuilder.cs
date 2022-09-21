using Microsoft.AspNetCore.Builder;

namespace TempFileStorage;

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