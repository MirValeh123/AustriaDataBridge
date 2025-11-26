namespace Shared.Constants
{
    public class AppSettingsValues
    {
        public static string MongoDbName => "MongoDbSettings:DatabaseName";
        public static string MongoDbConnectionString => "MongoDbSettings:ConnectionString";
        public static string SendoraSmsSenderSettings => "SendoraSmsSenderSettings";

        public static string AppsettingsFileDirectory { get; set; } = string.Empty;
        public static string AdminAppsettingsFileDirectory { get; set; } = string.Empty;

    }
}
