using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace L2ACP.Extensions
{
    public static class GeneralExtensions
    {
        public static string ToL2Password(this string str)
        {
            SHA1 shA1 = SHA1.Create();
            byte[] bytes = new ASCIIEncoding().GetBytes(str);
            str = Convert.ToBase64String(shA1.ComputeHash(bytes));
            return str;
        }

        public static string GetUsername(this HttpContext context)
        {
            if (context.User.Identity.IsAuthenticated)
            {
                return context.User.Claims.FirstOrDefault(x => x.Type == "username").Value;
            }
            return string.Empty;
        }
    }
}