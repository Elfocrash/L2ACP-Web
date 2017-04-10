namespace L2ACP.Requests
{
    public class ResetPkRequest : L2Request
    {
        public string Username { get; set; }

        public ResetPkRequest() : base(15)
        {

        }
    }

}