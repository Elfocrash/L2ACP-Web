namespace L2ACP.Requests
{
    public class PunishRequest : L2Request
    {
        public int PunishId { get; set; }
        public string PlayerName { get; set; }
        public int Time { get; set; }

        public PunishRequest(int punishId, string playerName, int time) : base(20)
        {
            PunishId = punishId;
            PlayerName = playerName;
            Time = time;
        }
    }
}