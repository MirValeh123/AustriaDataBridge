namespace Shared.Models
{
    public class AppSettings
    {
        public DatabaseSettings DatabaseSettings { get; set; }
        public MongoDbSettings MongoDbSettings { get; set; }

        public FeatureFlags? FeatureFlags { get; set; }
    }

    public class FeatureFlags
    {
        public FeatureFlags()
        {
            UseMassTransit = true;
            UseMongoDb = true;
        }

        public bool UseMassTransit { get; set; }
        public bool UseMongoDb { get; set; }
    }
}
