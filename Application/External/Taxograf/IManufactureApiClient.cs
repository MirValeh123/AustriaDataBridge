using System.Net.Http;
using Refit;
using Application.External.Taxograf.Models;

namespace Application.External.Taxograf
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
        Task<HttpResponseMessage> GetBatchForManufacturingAsync();

        [Put("/Manufacture/setReadyOnManufacturerCallback")]
        Task SetReadyOnManufacturerCallbackAsync([Body] SetReadyOnManufacturerCallbackRequest request);
    }
}

