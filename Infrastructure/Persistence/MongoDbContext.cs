using Infrastructure.Configuration;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Infrastructure.Persistence
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;
        private readonly MongoDbSettings _settings;

        public MongoDbContext(IMongoClient mongoClient, IOptions<MongoDbSettings> settings)
        {
            _settings = settings.Value;
            _database = mongoClient.GetDatabase(_settings.DatabaseName);
        }

        public IMongoDatabase Database => _database;

        public IMongoCollection<T> GetCollection<T>(string collectionName)
        {
            return _database.GetCollection<T>(collectionName);
        }

        public IMongoCollection<T> GetCollection<T>()
        {
            var collectionName = typeof(T).Name.ToLowerInvariant();
            if (collectionName.EndsWith("entity"))
            {
                collectionName = collectionName.Substring(0, collectionName.Length - 6) + "s";
            }
            else if (!collectionName.EndsWith("s"))
            {
                collectionName += "s";
            }
            return _database.GetCollection<T>(collectionName);
        }
    }
}

