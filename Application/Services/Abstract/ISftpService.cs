namespace Application.Services.Abstract
{
    public interface ISftpService
    {
        Task<bool> UploadXmlToSftpAsync(string xmlContent, string fileName = null);
        Task<bool> UploadFileToSftpAsync(Stream fileStream, string fileName);
        Task<List<string>> ListFilesAsync(string remoteDirectory = null);
        Task<bool> TestConnectionAsync();
    }
}
