namespace L2ACP.Requests
{

    public abstract class L2Request
    {
        public string ApiKey { get; set; } = "elfocrash";

        public int RequestId { get; set; }

        protected L2Request(int requestId)
        {
            RequestId = requestId;
        }
    }
}