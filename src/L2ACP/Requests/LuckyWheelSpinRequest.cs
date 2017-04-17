namespace L2ACP.Requests
{
    public class LuckyWheelSpinRequest : L2Request
    {
        public string PlayerName { get; set; }
        public LuckyWheelSpinRequest() : base(25)
        {

        }
    }
}