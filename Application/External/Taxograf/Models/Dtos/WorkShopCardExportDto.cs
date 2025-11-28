using Application.External.Taxograf.Models.Base;

namespace Application.External.Taxograf.Models.Dtos
{
    public class WorkShopCardExportDto : ExportModelCardBase
    {
        public WorkshopCardHolder CardHolder { get; set; }
        public ExportModelCardIssuingAuthority CardIssuingAuthority { get; set; }
        public ExportModelWorkshop Workshop { get; set; }
        public ExportModelBinaryData BinaryData { get; set; }
    }

    public class WorkshopCardHolder : ExportModelCardHolderBase
    {
        public string CardHolderAddressAzari { get; set; }
        public string CardHolderAddressLatin { get; set; }
    }

    public class ExportModelWorkshop
    {
        public string WorkshopNameAzari { get; set; }
        public string WorkshopNameLatin { get; set; }
        public string WorkshopAddressAzari { get; set; }
        public string WorkshopAddressLatin { get; set; }
    }
}

