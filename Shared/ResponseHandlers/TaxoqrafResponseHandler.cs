using System.Net.Http;
using static Shared.Delegates.MappingDelegate;

namespace Shared.ResponseHandlers
{
    /// <summary>
    /// HttpResponseMessage-dən gələn content-i oxuyub Application model-ə map edən generic handler
    /// </summary>
    public class TaxoqrafResponseHandler<TApplicationModel>
    {
        private readonly MapperDelagate<string, TApplicationModel> _contentMapper;

        /// <summary>
        /// Constructor - HTTP content string-i üçün mapper delegate qəbul edir
        /// </summary>
        public TaxoqrafResponseHandler(MapperDelagate<string, TApplicationModel> contentMapper)
        {
            _contentMapper = contentMapper ?? throw new ArgumentNullException(nameof(contentMapper));
        }

        /// <summary>
        /// HttpResponseMessage-dən tək obyekti oxuyub və map edir
        /// </summary>
        public async Task<TApplicationModel> HandleAsync(HttpResponseMessage httpResponse)
        {
            if (httpResponse == null)
                throw new ArgumentNullException(nameof(httpResponse));

            var content = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);

            return _contentMapper(content);
        }

        /// <summary>
        /// HttpResponseMessage-dən collection şəklində modelləri oxuyub və map edir
        /// (məs: JSON array -> List&lt;TApplicationModel&gt;).
        /// Collection map-lama loqikasını parametr olaraq verirsən.
        /// </summary>
        public async Task<List<TApplicationModel>> HandleCollectionAsync(
            HttpResponseMessage httpResponse,
            Func<string, IEnumerable<TApplicationModel>> collectionMapper)
        {
            if (httpResponse == null)
                throw new ArgumentNullException(nameof(httpResponse));

            if (collectionMapper == null)
                throw new ArgumentNullException(nameof(collectionMapper));

            var content = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);

            var models = collectionMapper(content);

            return models?.ToList() ?? new List<TApplicationModel>();
        }

        /// <summary>
        /// HttpResponseMessage-dən oxuyub, map edir və nəticəni validate edir
        /// </summary>
        public async Task<TApplicationModel> HandleWithValidationAsync(
            HttpResponseMessage httpResponse,
            Func<TApplicationModel, bool> validator)
        {
            if (validator == null)
                throw new ArgumentNullException(nameof(validator));

            var model = await HandleAsync(httpResponse).ConfigureAwait(false);

            if (!validator(model))
                throw new InvalidOperationException($"Validation failed for {typeof(TApplicationModel).Name}");

            return model;
        }
    }
}
