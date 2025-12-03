using Application.External.Taxograf.Models;

namespace Application.Services.Abstract
{
    /// <summary>
    /// Manufacture servisi üçün interface - xarici Taxograf API ilə işləyir
    /// </summary>
    public interface IManufactureService
    {
        /// <summary>
        /// Xarici API-dən manufacture batch məlumatlarını əldə edir
        /// </summary>
        Task<ManufactureApiResponse> GetBatchForManufacturingAsync();

        Task<string> GetBatchForManufacturingAsXMLAsync();

        Task SetReadyOnManufacturerCallbackAsync(SetReadyOnManufacturerCallbackRequest request);
    }
}
