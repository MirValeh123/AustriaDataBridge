using Application.External.Taxograf.Models.Dtos;

namespace Application.External.Taxograf.Models
{
    public class ManufactureApiResponse
    {
        public List<DriverCardExportDto> DriverCardsExportModel { get; set; }
        public List<TrasnporterCardExportDto> TransporterCardsExportModel { get; set; }
        public List<WorkShopCardExportDto> WorkshopCardsExportModel { get; set; }
        public List<InspectorCardExportDto> InspectorCardsExportModel { get; set; }
    }
}

