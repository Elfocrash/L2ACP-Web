namespace L2ACP.Requests
{
    public class SetNoblessRequest : L2Request
    {
        public string Username { get; set; }

        public SetNoblessRequest() : base(16)
        {

        }
    }
}