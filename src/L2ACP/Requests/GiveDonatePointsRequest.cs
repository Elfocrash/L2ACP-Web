namespace L2ACP.Requests
{
    public class GiveDonatePointsRequest : L2Request
    {
        public string PlayerName { get; set; }
        public int Points { get; set; }

        public GiveDonatePointsRequest(string player, int points) : base(29)
        {
            PlayerName = player;
            Points = points;
        }
    }
}