using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using L2ACP.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace L2ACP.Controllers
{
    [Route("ipn")]
    public class IPNController : Controller
    {
        private readonly IRequestService _requestService;
        public IPNController(IRequestService requestService)
        {
            _requestService = requestService;
        }


        [HttpPost]
        [Route("validate")]
        public async Task<ActionResult> Ipn()
        {
            var ipn = Request.Form.Keys.ToDictionary(x => x, x => Request.Form[x].ToString());
            ipn.Add("cmd", "_notify-validate");

            var isIpnValid = await ValidateIpnAsync(ipn);
            if (isIpnValid)
            {
                var transactionId = ipn["txn_id"];
                var accountName = ipn["custom"];
                var amount = int.Parse(ipn["option_selection1"].Replace(" Donate Points",string.Empty));
                var verifySign = ipn["verify_sign"];
                await _requestService.SendDonation(accountName, amount, transactionId, verifySign);
            }

            return new EmptyResult();
        }

        private static async Task<bool> ValidateIpnAsync(IEnumerable<KeyValuePair<string, string>> ipn)
        {
            using (var client = new HttpClient())
            {
                const string PayPalUrl = "https://www.sandbox.paypal.com/cgi-bin/webscr";//"https://www.paypal.com/cgi-bin/webscr";

                // This is necessary in order for PayPal to not resend the IPN.
                await client.PostAsync(PayPalUrl, new StringContent(string.Empty));

                var response = await client.PostAsync(PayPalUrl, new FormUrlEncodedContent(ipn));

                var responseString = await response.Content.ReadAsStringAsync();
                return (responseString == "VERIFIED");
            }
        }
    }
}
