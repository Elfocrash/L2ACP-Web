namespace L2ACP.Requests
{
    public class GetPlayerInfoRequest : L2Request
    {
        public string Username { get; set; }

        public GetPlayerInfoRequest() : base(5)
        {

        }
    }
}