namespace L2ACP.Requests
{
    public class RenamePlayerRequest : L2Request
    {
        public string Username { get; set; }
        public string NewName { get; set; }

        public RenamePlayerRequest() : base(13)
        {

        }
    }
}