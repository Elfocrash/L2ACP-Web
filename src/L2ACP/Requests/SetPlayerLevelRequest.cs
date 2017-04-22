namespace L2ACP.Requests
{
    public class SetPlayerLevelRequest : L2Request
    {
        public string PlayerName { get; set; }
        public int Level { get; set; }

        public SetPlayerLevelRequest(string player, int points) : base(30)
        {
            PlayerName = player;
            Level = points;
        }
    }
}