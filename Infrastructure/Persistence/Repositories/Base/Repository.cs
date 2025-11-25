using Application.Persistence.Repositories.Base;
using Infrastructure.Persistence;
using MongoDB.Driver;
using System.Linq.Expressions;

namespace Infrastructure.Persistence.Repositories.Base
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly IMongoCollection<T> _collection;

        public Repository(MongoDbContext context)
        {
            _collection = context.GetCollection<T>();
        }

        public virtual async Task<T?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            var filter = Builders<T>.Filter.Eq("_id", id);
            return await _collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _collection.Find(_ => true).ToListAsync(cancellationToken);
        }

        public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await _collection.Find(predicate).ToListAsync(cancellationToken);
        }

        public virtual async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await _collection.Find(predicate).FirstOrDefaultAsync(cancellationToken);
        }

        public virtual async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
        {
            await _collection.InsertOneAsync(entity, cancellationToken: cancellationToken);
            return entity;
        }

        public virtual async Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
        {
            var id = GetIdValue(entity);
            var filter = Builders<T>.Filter.Eq("_id", id);
            await _collection.ReplaceOneAsync(filter, entity, cancellationToken: cancellationToken);
        }

        public virtual async Task DeleteAsync(string id, CancellationToken cancellationToken = default)
        {
            var filter = Builders<T>.Filter.Eq("_id", id);
            await _collection.DeleteOneAsync(filter, cancellationToken);
        }

        public virtual async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        {
            var count = await _collection.CountDocumentsAsync(predicate, cancellationToken: cancellationToken);
            return count > 0;
        }

        public virtual async Task<long> CountAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken cancellationToken = default)
        {
            if (predicate == null)
            {
                return await _collection.CountDocumentsAsync(_ => true, cancellationToken: cancellationToken);
            }
            return await _collection.CountDocumentsAsync(predicate, cancellationToken: cancellationToken);
        }

        protected virtual string GetIdValue(T entity)
        {
            var idProperty = typeof(T).GetProperty("Id") ?? typeof(T).GetProperty("_id");
            if (idProperty == null)
            {
                throw new InvalidOperationException($"Entity {typeof(T).Name} does not have an Id or _id property.");
            }
            return idProperty.GetValue(entity)?.ToString() ?? throw new InvalidOperationException($"Id value is null for entity {typeof(T).Name}.");
        }
    }
}

