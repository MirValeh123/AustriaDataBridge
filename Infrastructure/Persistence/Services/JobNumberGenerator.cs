using Application.Models.Response;
using Application.Persistence.Repositories;
using Application.Persistence.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Persistence.Services
{
    public class JobNumberService(IServiceScopeFactory serviceScopeFactory, IConfiguration configuration) : IJobNumberService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory = serviceScopeFactory;
        private readonly string _collectionName = configuration["MongoDbSettings:ExportJobLogCollection"];
        private int _lastJobNumber = 0;
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
        private bool _initialized = false;

        public async Task<string> GetNextJobNumberAsync()
        {
            await _semaphore.WaitAsync();
            try
            {
                if (!_initialized)
                {
                    await InitializeAsync();
                    _initialized = true;
                }

                _lastJobNumber++;
                return _lastJobNumber.ToString();
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task InitializeAsync()
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var mongoRepository = scope.ServiceProvider.GetRequiredService<IMongoRepository>();

            var logs = await mongoRepository.GetAllAsync<ExportJobLog>(_collectionName);

            if (logs != null && logs.Any())
            {
                var maxJobNumber = logs
                    .Where(x => !string.IsNullOrEmpty(x.JobNumber))
                    .Select(x => int.TryParse(x.JobNumber, out int num) ? num : 0)
                    .DefaultIfEmpty(0)
                    .Max();

                _lastJobNumber = maxJobNumber;
            }
        }
    }
}
