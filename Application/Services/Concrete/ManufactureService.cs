using Application.Converters;
using Application.External.Taxograf;
using Application.External.Taxograf.Models;
using Application.Persistence.Services;
using Application.Services.Abstract;
using Shared.Helpers;
using Shared.ResponseHandlers;
using System.Net.Http;

namespace Application.Services.Concrete
{
    /// <summary>
    /// Manufacture servisi - Refit client və TaxoqrafResponseHandler istifadə edərək
    /// xarici Taxograf API ilə kommunikasiya edir
    /// </summary>
    public class ManufactureService : IManufactureService
    {
        private readonly IManufactureApiClient _manufactureApiClient;
        private readonly TaxoqrafResponseHandler<ManufactureApiResponse> _responseHandler;
        private readonly IControlCardRequestXmlConverter _xmlConverter;

        public ManufactureService(
            IManufactureApiClient manufactureApiClient,
            TaxoqrafResponseHandler<ManufactureApiResponse> responseHandler,
            IControlCardRequestXmlConverter xmlConverter)
        {
            _manufactureApiClient = manufactureApiClient ?? throw new ArgumentNullException(nameof(manufactureApiClient));
            _responseHandler = responseHandler ?? throw new ArgumentNullException(nameof(responseHandler));
            _xmlConverter = xmlConverter ?? throw new ArgumentNullException(nameof(xmlConverter));
        }


        /// <summary>
        /// Xarici API-dən manufacture batch məlumatlarını əldə edir və map edir
        /// </summary>
        public async Task<ManufactureApiResponse> GetBatchForManufacturingAsync()
        {
            var httpResponse = await _manufactureApiClient.GetBatchForManufacturingAsync();
            var handledResponse = await _responseHandler.HandleAsync(httpResponse);
            return handledResponse;
        }
        /// <summary>
        /// Xarici API-dən manufacture batch məlumatlarını əldə edir və Xml ə convert  edir
        /// </summary>
        public async Task<string> GetBatchForManufacturingAsXMLAsync()
        {
            var httpResponse = await _manufactureApiClient.GetBatchForManufacturingAsync();
            var handledResponse = await _responseHandler.HandleAsync(httpResponse);

            return _xmlConverter.ConvertToXml(handledResponse);
        }

        /// <summary>
        /// Xarici API-yə kartın hazır olduğunu bildirən callback-i forward edir
        /// </summary>
        public async Task SetReadyOnManufacturerCallbackAsync(SetReadyOnManufacturerCallbackRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            await _manufactureApiClient.SetReadyOnManufacturerCallbackAsync(request);
        }
    }
}
