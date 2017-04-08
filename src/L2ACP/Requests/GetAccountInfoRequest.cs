namespace L2ACP.Requests
{

    public class GetAccountInfoRequest : L2Request
    {
        public string Username { get; set; }

        public GetAccountInfoRequest() : base(3)
        {

        }
    }
}