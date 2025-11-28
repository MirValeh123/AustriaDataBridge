using Application.External.Taxograf.Models.Base;

namespace Application.External.Taxograf.Models.Dtos
{
    public class DriverCardExportDto
    {
        public DriverCardHolder CardHolder { get; set; }
        public ExportModelCardIssuingAuthority CardIssuingAuthority { get; set; }
        public DrivingLicenceInfo DrivingLicenceInfo { get; set; }
        public ExportModelBinaryData BinaryData { get; set; }
    }

    public class DriverCardHolder : ExportModelCardHolderBase
    {
        public DateOnly CardHolderBirthDate { get; set; }
        public string CardHolderAddressAzari { get; set; }
        public string CardHolderAddressLatin { get; set; }
    }

    public class DrivingLicenceInfo
    {
        public string DrivingLicenceNumber { get; set; }
        public string DrivingLicenceIssuingAuthorityAzari { get; set; }
        public string DrivingLicenceIssuingAuthorityLatin { get; set; }
        public int DrivingLicenceIssuingNation { get; set; }
    }
}

