using Refit;
using Application.External.Taxograf.Models;

namespace Application.Persistence.Services
{
    /// <summary>
    /// Taxograf API ilə kommunikasiya üçün Refit client interface
    /// </summary>
    public interface IManufactureApiClient
    {
        /// <summary>
        /// Manufacture batch məlumatlarını əldə edir
        /// </summary>
        [Get("/Manufacture/getBatchForManufacturing")]
        Task<ManufactureApiResponse> GetBatchForManufacturingAsync();
    }
}
