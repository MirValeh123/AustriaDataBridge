namespace Application.External.Taxograf.Models
{
    public class SftpSettings
    {
        public string Host { get; set; }
        public int Port { get; set; } = 22;
        public string Username { get; set; }
        public string Password { get; set; }
        public string RemoteDirectory { get; set; }
        public string PrivateKeyPath { get; set; }
        public string FileNamePrefix { get; set; }
        public string FileExtension { get; set; }
    }
}
