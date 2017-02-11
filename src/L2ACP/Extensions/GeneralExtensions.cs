using System;
using System.Security.Cryptography;
using System.Text;

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
    }
}