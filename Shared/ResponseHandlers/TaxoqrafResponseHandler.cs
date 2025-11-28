using static Shared.Delegates.MappingDelegate;

namespace Shared.ResponseHandlers
{
    /// <summary>
    /// Generic response handler - istənilən API response-nu Application model-ə map edir
    /// </summary>
    public class TaxoqrafResponseHandler<TApiResponse,TApplicationModel>
    {
        private readonly MapperDelagate<TApiResponse,TApplicationModel> _mapperDelagate;

        /// <summary>
        /// Constructor - mapper delegate qəbul edir
        /// </summary>
        /// 
        public TaxoqrafResponseHandler(MapperDelagate<TApiResponse, TApplicationModel> mapperDelagate)
        {
            _mapperDelagate = mapperDelagate ?? throw new ArgumentNullException(nameof(mapperDelagate));
        }

        /// <summary>
        /// Tək obyekti handle və map edir
        /// </summary>
        public TApplicationModel Handle(TApiResponse response)
        {
            if (response == null) return default;

            var mapperResult = _mapperDelagate(response);

            return mapperResult;
        }
        /// <summary>
        /// Collection-u handle və map edir
        /// </summary>
        /// 
        public List<TApplicationModel> HandleCollection(IEnumerable<TApiResponse> responses)
        {
            if (responses == null) return new List<TApplicationModel>();

            var mapperResults = responses.Select(res => _mapperDelagate(res)).ToList();

            return mapperResults;
        }

        /// <summary>
        /// Validation ilə handle edir
        /// </summary>
        public TApplicationModel HandleWithValidation(
            TApiResponse apiResponse,
            Func<TApiResponse, bool> validator)
        {
            if (apiResponse == null)
                throw new ArgumentNullException(nameof(apiResponse), "API response cannot be null");

            // Əvvəlcə validate edir
            if (!validator(apiResponse))
                throw new InvalidOperationException($"Validation failed for {typeof(TApiResponse).Name}");

            // Sonra map edir
            return Handle(apiResponse);
        }
    }
}
