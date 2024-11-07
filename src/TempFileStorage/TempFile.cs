namespace TempFileStorage;

public class TempFile(string key)
{
    public TempFile() : this(TempFileKeyGenerator.Generate())
    {
    }

    public string Key { get; } = key;
    public bool IsUpload { get; set; }
    public string Filename { get; set; }
    public long FileSize { get; set; }
    public DateTime CacheTimeout { get; set; }
}