using L2ACP.Models;

namespace L2ACP.Responses
{
    public class LuckyWheelListResponse : L2Response
    {
        public LuckyWheelItem[] Items { get; set; }
    }
}