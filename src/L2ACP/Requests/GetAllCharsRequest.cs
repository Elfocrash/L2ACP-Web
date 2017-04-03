namespace L2ACP.Requests
{

    public class GetAllCharsRequest : L2Request
    {
        public string Username { get; set; }

        public GetAllCharsRequest() : base(3)
        {

        }
    }
}