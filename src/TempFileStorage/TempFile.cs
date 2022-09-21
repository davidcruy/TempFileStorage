namespace TempFileStorage;

public class TempFile
{
    public TempFile() : this(TempFileKeyGenerator.Generate())
    {
    }

    public TempFile(string key)
    {
        Key = key;
    }

    public string Key { get; }
    public string Filename { get; set; }
    public long FileSize { get; set; }
    public DateTime CacheTimeout { get; set; }
}