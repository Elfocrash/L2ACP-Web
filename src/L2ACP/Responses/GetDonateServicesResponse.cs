using L2ACP.Models;

namespace L2ACP.Responses
{
    public class GetDonateServicesResponse : L2Response
    {
        public DonateService[] DonateServices { get; set; }
    }
}