using Application.External.Taxograf.Models.Base;

namespace Application.External.Taxograf.Models.Dtos
{
    public class InspectorCardExportDto : ExportModelCardBase
    {
        public InspectorCardHolder CardHolder { get; set; }
        public ExportModelCardIssuingAuthority CardIssuingAuthority { get; set; }
        public ExportModelInspectionAgency ControlBody { get; set; }
    }

    public class InspectorCardHolder : ExportModelCardHolderBase
    {
    }

    public class ExportModelInspectionAgency
    {
        public string ControlBodyNameAzari { get; set; }
        public string ControlBodyNameLatin { get; set; }
        public string ControlBodyAddressAzari { get; set; }
        public string ControlBodyAddressLatin { get; set; }
    }
}

