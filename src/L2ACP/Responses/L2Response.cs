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
using System.Net.Http;
using System.Text;
using L2ACP.Cryptography;
using L2ACP.Extensions;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace L2ACP.Responses
{
    public class L2Response
    {
        public int ResponseCode { get; set; }

        public string ResponseMessage { get; set; }
    }

    public class JsonContent : StringContent
    {
        public JsonContent(object obj) : base(AesCrypto.EncryptRijndael(JsonConvert.SerializeObject(obj), Startup.Configuration.GetValue<string>("CryptoSalt")), Encoding.UTF8, "application/json")
        { }
    }
}