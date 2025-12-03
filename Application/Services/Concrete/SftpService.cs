using Application.External.Taxograf.Models;
using Application.Services.Abstract;
using Microsoft.Extensions.Options;
using Renci.SshNet;
using System.Text;

namespace Application.Services.Concrete
{
    public class SftpService : ISftpService, IDisposable
    {
        private readonly SftpSettings _settings;
        private SftpClient _sftpClient;

        public SftpService(IOptions<SftpSettings> settings)
        {
            _settings = settings.Value;
        }

        public async Task<bool> UploadXmlToSftpAsync(string xmlContent, string fileName = null)
        {

            await EnsureConnectedAsync();

            // Əgər fayl adı verilməyibsə, avtomatik generasiya et
            if (string.IsNullOrEmpty(fileName))
            {
                fileName = $"{_settings.FileNamePrefix}{DateTime.Now:yyyyMMdd_HHmmss}{_settings.FileExtension}";
            }

            // XML content-i memory stream-ə çevir
            using var memoryStream = new MemoryStream();
            using var writer = new StreamWriter(memoryStream, Encoding.UTF8);
            await writer.WriteAsync(xmlContent);
            await writer.FlushAsync();
            memoryStream.Position = 0;

            // Remote fayl yolu
            var remoteFilePath = string.IsNullOrEmpty(_settings.RemoteDirectory)
                ? fileName
                : $"{_settings.RemoteDirectory}/{fileName}";

            // Faylı SFTP-yə yüklə
            _sftpClient.UploadFile(memoryStream, remoteFilePath);

            Console.WriteLine($"XML faylı uğurla yükləndi: {remoteFilePath}");
            return true;

        }

        public async Task<bool> UploadFileToSftpAsync(Stream fileStream, string fileName)
        {

            await EnsureConnectedAsync();

            var remoteFilePath = string.IsNullOrEmpty(_settings.RemoteDirectory)
                ? fileName
                : $"{_settings.RemoteDirectory}/{fileName}";

            _sftpClient.UploadFile(fileStream, remoteFilePath);
            return true;

        }

        public async Task<List<string>> ListFilesAsync(string remoteDirectory = null)
        {

            await EnsureConnectedAsync();

            var directory = remoteDirectory ?? _settings.RemoteDirectory;
            var files = _sftpClient.ListDirectory(directory)
                .Where(f => !f.IsDirectory)
                .Select(f => f.Name)
                .ToList();

            return files;

        }

        public async Task<bool> TestConnectionAsync()
        {
            try
            {
                using var testClient = CreateSftpClient();
                await Task.Run(() => testClient.Connect());

                if (testClient.IsConnected)
                {
                    testClient.Disconnect();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SFTP bağlantı testi xətası: {ex.Message}");
                return false;
            }
        }

        public void Dispose()
        {
            if (_sftpClient != null && _sftpClient.IsConnected)
            {
                _sftpClient.Disconnect();
            }
            _sftpClient?.Dispose();
        }



        private SftpClient CreateSftpClient()
        {
            ConnectionInfo connectionInfo;

            if (!string.IsNullOrEmpty(_settings.PrivateKeyPath))
            {
                // Private key ilə bağlantı
                var privateKeyFile = new PrivateKeyFile(_settings.PrivateKeyPath);
                connectionInfo = new ConnectionInfo(
                    _settings.Host,
                    _settings.Port,
                    _settings.Username,
                    new PrivateKeyAuthenticationMethod(_settings.Username, privateKeyFile)
                );
            }
            else
            {
                // Username/Password ilə bağlantı
                connectionInfo = new ConnectionInfo(
                    _settings.Host,
                    _settings.Port,
                    _settings.Username,
                    new PasswordAuthenticationMethod(_settings.Username, _settings.Password)
                );
            }

            return new SftpClient(connectionInfo);
        }

        private async Task EnsureConnectedAsync()
        {
            if (_sftpClient == null)
            {
                _sftpClient = CreateSftpClient();
            }

            if (!_sftpClient.IsConnected)
            {
                await Task.Run(() => _sftpClient.Connect());

                // Remote directory yoxlamaq/yaratmaq
                if (!string.IsNullOrEmpty(_settings.RemoteDirectory) &&
                    !_sftpClient.Exists(_settings.RemoteDirectory))
                {
                    _sftpClient.CreateDirectory(_settings.RemoteDirectory);
                }
            }
        }
    }
}
