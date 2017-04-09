using System;

namespace L2ACP.Responses
{
    public class GetAccountInfoResponse : L2Response
    {
        public string[] AccountNames { get; set; }
        public int DonatePoints { get; set; }
        public int AccessLevel { get; set; }
    }
}