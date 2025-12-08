namespace Application.Persistence.Repositories
{
    public interface IMongoRepository
    {
        Task<T> InsertRecordAsync<T>(string table, T record) where T : class;
        Task UpsertRecordAsync<T>(string table, Guid id, T record) where T : class;
        Task<List<T>> GetAllAsync<T>(string table) where T : class;
    }
}

