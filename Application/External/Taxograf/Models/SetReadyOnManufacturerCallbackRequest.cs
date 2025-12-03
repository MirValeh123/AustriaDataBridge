namespace Application.External.Taxograf.Models
{
    public class SetReadyOnManufacturerCallbackRequest
    {
        public string CardNumber {  get; set; }
        public string CertificateRequestId {  get; set; }
        public string CardStatusIdentifier {  get; set; }
        public string CardStatus { get; set; }

    }
}
