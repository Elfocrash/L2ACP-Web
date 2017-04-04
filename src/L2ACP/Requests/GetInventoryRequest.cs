namespace L2ACP.Requests
{
    public class GetInventoryRequest : L2Request
    {
        public string Username { get; set; }

        public GetInventoryRequest() : base(4)
        {

        }
    }
}