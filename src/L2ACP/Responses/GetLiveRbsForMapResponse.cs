using L2ACP.Models;

namespace L2ACP.Responses
{
    public class GetLiveRbsForMapResponse : L2Response
    {
        public MapMob[] MapMobs { get; set; }
    }
}