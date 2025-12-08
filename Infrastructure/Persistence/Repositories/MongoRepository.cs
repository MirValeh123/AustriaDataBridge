using Application.Persistence.Repositories;
using MongoDB.Driver;

namespace Infrastructure.Persistence.Repositories
{
    public class MongoRepository : IMongoRepository
    {
        private readonly MongoDbContext _context;

        public MongoRepository(MongoDbContext context)
        {
            _context = context;
        }

        public async Task<List<T>> GetAllAsync<T>(string table) where T : class
        {
            var collection = _context.GetCollection<T>(table);
            return await collection.Find(_ => true).ToListAsync();
        }

        public async Task<T> InsertRecordAsync<T>(string table, T record) where T : class
        {
            var collection = _context.GetCollection<T>(table);
            await collection.InsertOneAsync(record);
            return record;
        }

        public async Task UpsertRecordAsync<T>(string table, Guid id, T record) where T : class
        {
            var collection = _context.GetCollection<T>(table);
            var filter = Builders<T>.Filter.Eq("_id", id);
            var options = new ReplaceOptions { IsUpsert = true };
            await collection.ReplaceOneAsync(filter, record, options);
        }
    }
}

