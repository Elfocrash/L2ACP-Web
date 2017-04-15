using L2ACP.Models;

namespace L2ACP.Requests
{
    public class SetDonateListRequest : L2Request
    {
        public AdminDonateListViewmodel[] Items { get; set; }

        public SetDonateListRequest(AdminDonateListViewmodel[] items) : base(23)
        {
            Items = items;
        }
    }
}