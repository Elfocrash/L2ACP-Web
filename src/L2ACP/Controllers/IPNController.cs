/*
 * Copyright (C) 2017  Nick Chapsas
 * This program is free software: you can redistribute it and/or modify it under
 * the terms of the GNU General Public License as published by the Free Software
 * Foundation, either version 2 of the License, or (at your option) any later
 * version.
 * 
 * L2ACP is distributed in the hope that it will be useful, but WITHOUT
 * ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
 * FOR A PARTICULAR PURPOSE. See the GNU General Public License for more
 * details.
 * 
 * You should have received a copy of the GNU General Public License along with
 * this program. If not, see <http://www.gnu.org/licenses/>.
 */
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
