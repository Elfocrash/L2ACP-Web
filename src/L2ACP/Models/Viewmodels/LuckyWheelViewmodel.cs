using System.Collections.Generic;

namespace L2ACP.Models
{
    public class LuckyWheelViewmodel
    {
        public IEnumerable<string> CharacterNames { get; set; }
        public List<LuckyWheelItem> Items { get; set; }
    }
}