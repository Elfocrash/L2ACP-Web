namespace L2ACP.Requests
{
    public class RegisterRequest : L2Request
    {
        public string Username { get; set; }

        public string Password { get; set; }

        public RegisterRequest() : base(2)
        {

        }
    }
}