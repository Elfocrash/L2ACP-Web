namespace L2ACP.Requests
{
    public class DonateRequest : L2Request
    {
        public string AccountName { get; set; }
        public int Amount { get; set; }
        public string TransactionId { get; set; }
        public string VerificationSign { get; set; }

        public DonateRequest() : base(7)
        {

        }
    }
}