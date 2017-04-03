namespace L2ACP.Requests
{

    public class LoginRequest : L2Request
    {
        public string Username { get; set; }

        public string Password { get; set; }

        public LoginRequest() : base(1)
        {

        }
    }
}