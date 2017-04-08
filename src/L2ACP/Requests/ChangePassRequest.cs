namespace L2ACP.Requests
{
    public class ChangePassRequest : L2Request
    {
        public string Username { get; set; }

        public string CurrentPassword { get; set; }

        public string NewPassword { get; set; }

        public ChangePassRequest() : base(8)
        {

        }
    }
}