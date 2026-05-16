using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace WebRadioFM.Helpers
{
    public static class ApiSignatureHelper
    {
        public static string CreateSignature(IDictionary<string, string> parameters, string secret)
        {
            var signature = new StringBuilder();

            foreach (var kvp in parameters
                .OrderBy(o => o.Key)
                .Where(w => !w.Key.Contains("format")))
            {
                signature.Append(kvp.Key);
                signature.Append(kvp.Value);
            }

            signature.Append(secret);

            return CreateMd5(signature.ToString());
        }

        private static string CreateMd5(string input)
        {
            using (var md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                var sb = new StringBuilder();
                foreach (byte b in hashBytes)
                {
                    sb.Append(b.ToString("X2"));
                }
                return sb.ToString().ToLower();
            }
        }
    }
}
