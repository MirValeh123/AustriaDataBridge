using Refit;
using Application.External.Taxograf.Models;

namespace Application.Persistence.Services
{
    public interface IManufactureApiClient
    {
        [Get("/api/v1/Manufacture/getBatchForManufacturing/api/v1")]
        Task<ManufactureApiResponse> GetBatchForManufacturingAsync();
    }
}
