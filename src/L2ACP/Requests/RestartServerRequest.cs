namespace L2ACP.Requests
{
    public class RestartServerRequest : L2Request
    {
        public int Seconds { get; set; }

        public RestartServerRequest(int seconds) : base(24)
        {
            Seconds = seconds;
        }
    }
}