/*
 * Copyright (C) 2018 Petr Jasicek
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
using System.Linq;
using System.Threading.Tasks;

namespace L2ACP.Cryptography
{
    public class L2OffCrypto
    {
        public static string EncryptLegacyL2Password(string password)
        {
            var key = new byte[16];
            long one, two, three, four;
            var dst = new byte[16];
            var nBytes = password.Length;

            for (var i = 0; i < nBytes; i++)
            {
                key[i] = System.Text.Encoding.ASCII.GetBytes(password.Substring(i, 1))[0];
                dst[i] = key[i];
            }

            long rslt = @key[0] + @key[1] * 256 + @key[2] * 65536 + @key[3] * 16777216;
            one = rslt * 213119 + 2529077;
            one = one - ToInt32(one / 4294967296) * 4294967296;

            rslt = @key[4] + @key[5] * 256 + @key[6] * 65536 + @key[7] * 16777216;
            two = rslt * 213247 + 2529089;
            two = two - ToInt32(two / 4294967296) * 4294967296;

            rslt = @key[8] + @key[9] * 256 + @key[10] * 65536 + @key[11] * 16777216;
            three = rslt * 213203 + 2529589;
            three = three - ToInt32(three / 4294967296) * 4294967296;

            rslt = @key[12] + @key[13] * 256 + @key[14] * 65536 + @key[15] * 16777216;
            four = rslt * 213821 + 2529997;
            four = four - ToInt32(four / 4294967296) * 4294967296;

            key[3] = ParseInt(one / 16777216);
            key[2] = ParseInt((((Int32)(one - @key[3] * 16777216)) / 65535));
            key[1] = ParseInt((one - @key[3] * 16777216 - @key[2] * 65536) / 256);
            key[0] = ParseInt((one - @key[3] * 16777216 - @key[2] * 65536 - @key[1] * 256));

            key[7] = ParseInt(two / 16777216);
            key[6] = ParseInt((two - @key[7] * 16777216) / 65535);
            key[5] = ParseInt((two - @key[7] * 16777216 - @key[6] * 65536) / 256);
            key[4] = ParseInt((two - @key[7] * 16777216 - @key[6] * 65536 - @key[5] * 256));

            key[11] = ParseInt(three / 16777216);
            key[10] = ParseInt((three - @key[11] * 16777216) / 65535);
            key[9] = ParseInt((three - @key[11] * 16777216 - @key[10] * 65536) / 256);
            key[8] = ParseInt((three - @key[11] * 16777216 - @key[10] * 65536 - @key[9] * 256));

            key[15] = ParseInt(four / 16777216);
            key[14] = ParseInt((four - @key[15] * 16777216) / 65535);
            key[13] = ParseInt((four - @key[15] * 16777216 - @key[14] * 65536) / 256);
            key[12] = ParseInt((four - @key[15] * 16777216 - @key[14] * 65536 - @key[13] * 256));

            dst[0] = ParseInt(dst[0] ^ @key[0]);

            for (var i = 1; i < dst.Length; i++)
                dst[i] = ParseInt(@dst[i] ^ @dst[i - 1] ^ @key[i]);

            for (var i = 0; i < dst.Length; i++)
                if (dst[i] == 0)
                    dst[i] = 102;

            return L2ACP.Extensions.GeneralExtensions.ByteArrayToString(dst);
        }

        private static int ToInt32(long val)
        {
            return Convert.ToInt32(val);
        }

        private static byte ParseInt(long val)
        {
            return BitConverter.GetBytes(val)[0];
        }

        private static string EncryptMD5L2Password(string password)
        {
            var md5Password = password.ToCharArray();
            var s = (EncryptMD5(password) + EncryptMD5(password)).ToCharArray();
            int j = 0;
            for (var i = 0; i < s.Length; i++)
            {
                if (j >= password.Length) j = 0;

                var calcu = s[i] ^ md5Password[j];
                s[i] = (char)calcu;
                j++;
            }
            return EncryptMD5(new string(s));
        }


        public static string EncryptMD5(string originalPassword)
        {
            return BitConverter.ToString(((System.Security.Cryptography.MD5.Create()).ComputeHash(System.Text.Encoding.UTF8.GetBytes(originalPassword)))).Replace("-", "").ToLower();
        }
    }
}
