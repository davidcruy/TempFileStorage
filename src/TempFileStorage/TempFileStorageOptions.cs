namespace TempFileStorage;

public class TempFileStorageOptions
{
    public TempFileStorageOptions()
    {
        DownloadFilePattern = "/download-file";
        UploadFilePattern = "/upload-file";
    }

    /// <summary>
    /// Gets or sets the path for downloading a file. (default: "/download-file")
    /// </summary>
    public string DownloadFilePattern { get; set; }

    /// <summary>
    /// Gets or sets the path for file upload. (default: "/upload-file")
    /// </summary>
    public string UploadFilePattern { get; set; }
}