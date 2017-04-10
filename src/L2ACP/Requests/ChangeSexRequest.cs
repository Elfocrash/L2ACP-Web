namespace L2ACP.Requests
{
    public class ChangeSexRequest : L2Request
    {
        public string Username { get; set; }

        public ChangeSexRequest() : base(14)
        {

        }
    }
}