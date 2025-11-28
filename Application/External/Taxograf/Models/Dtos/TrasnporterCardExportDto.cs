using Application.External.Taxograf.Models.Base;

namespace Application.External.Taxograf.Models.Dtos
{
    public class TrasnporterCardExportDto : ExportModelCardBase
    {
        public TransporterCardHolder CardHolder { get; set; }
        public ExportModelCardIssuingAuthority CardIssuingAuthority { get; set; }
        public ExportModelTransporter Company { get; set; }
    }

    public class TransporterCardHolder : ExportModelCardHolderBase
    {
    }

    public class ExportModelTransporter
    {
        public string CompanyNameAzari { get; set; }
        public string CompanyNameLatin { get; set; }
        public string CompanyAddressAzari { get; set; }
        public string CompanyAddressLatin { get; set; }
    }
}

