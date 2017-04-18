using L2ACP.Models;

namespace L2ACP.Responses
{
    public class GetAnalyticsPlayersResponse : L2Response
    {
        public AnalyticsPlayerData[] PlayerData { get; set; }
    }
}