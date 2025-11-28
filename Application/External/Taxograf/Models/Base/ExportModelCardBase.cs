namespace Application.External.Taxograf.Models.Base
{
    public class ExportModelCardBase
    {
        public int Test { get; set; }
        public string CardNumber { get; set; }
        public DateTime CardIssueDate { get; set; }
        public DateTime CardValidityBegin { get; set; }
        public DateTime CardExpiryDate { get; set; }
    }
}

