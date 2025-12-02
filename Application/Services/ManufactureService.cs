using Application.Converters;
using Application.External.Taxograf;
using Application.External.Taxograf.Models;
using Application.Persistence.Services;
using Shared.Helpers;
using Shared.ResponseHandlers;

namespace Application.Services
{
    /// <summary>
    /// Manufacture servisi - Refit client və TaxoqrafResponseHandler istifadə edərək
    /// xarici Taxograf API ilə kommunikasiya edir
    /// </summary>
    public class ManufactureService : IManufactureService
    {
        private readonly IManufactureApiClient _manufactureApiClient;
        private readonly TaxoqrafResponseHandler<ManufactureApiResponse, ManufactureApiResponse> _responseHandler;
        private readonly IControlCardRequestXmlConverter _xmlConverter;

        public ManufactureService(
            IManufactureApiClient manufactureApiClient,
            TaxoqrafResponseHandler<ManufactureApiResponse, ManufactureApiResponse> responseHandler,
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
            var apiResponse = await _manufactureApiClient.GetBatchForManufacturingAsync();
            return _responseHandler.Handle(apiResponse);
        }
        /// <summary>
        /// Xarici API-dən manufacture batch məlumatlarını əldə edir və Xml ə convert  edir
        /// </summary>
        public async Task<string> GetBatchForManufacturingAsXMLAsync()
        {
            var apiResponse = await _manufactureApiClient.GetBatchForManufacturingAsync();
            var handledResponse = _responseHandler.Handle(apiResponse);

            return _xmlConverter.ConvertToXml(handledResponse);
        }
    }
}
