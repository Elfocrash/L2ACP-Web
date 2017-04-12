namespace L2ACP.Requests
{
    public class AnnounceRequest : L2Request
    {
        public string Text { get; set; }

        public AnnounceRequest(string text) : base(19)
        {
            Text = text;
        }
    }
}