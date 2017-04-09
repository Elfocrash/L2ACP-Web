using L2ACP.Models;

namespace L2ACP.Responses
{
    public class GetStatsResponse : L2Response
    {
        public TopPlayer[] TopPvp { get; set; }
        public TopPlayer[] TopPk { get; set; }
        public TopPlayer[] TopOnline { get; set; }
    }
}