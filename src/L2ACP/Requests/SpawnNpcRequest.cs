namespace L2ACP.Requests
{
    public class SpawnNpcRequest : L2Request
    {
        public int NpcId { get; set; }
        public int X { get; set; }
        public int Y { get; set; }

        public SpawnNpcRequest() : base(22)
        {
        }
    }
}