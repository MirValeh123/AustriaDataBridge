using Application.External.Taxograf.Models.Dtos;

namespace Application.External.Taxograf.Models
{
    public class SentToManufacturerCallbackRequest
    {
        public List<string> CardNumbers { get; set; }
        public string JobNumber { get; set; }
    }
}
