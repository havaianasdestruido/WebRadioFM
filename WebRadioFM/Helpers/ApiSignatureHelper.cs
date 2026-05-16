using System;
using System.Collections.Generic;
using System.Linq;
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
            byte[] inputBytes = Encoding.UTF8.GetBytes(input);
            byte[] hashBytes = ComputeMd5(inputBytes);

            var sb = new StringBuilder();
            foreach (byte b in hashBytes)
            {
                sb.Append(b.ToString("X2"));
            }
            return sb.ToString().ToLower();
        }

        private static byte[] ComputeMd5(byte[] input)
        {
            // MD5 implementation for WP 8.0 compatibility
            uint a, b, c, d;
            uint[] x = new uint[16];

            int len = input.Length;
            int paddedLen = ((len + 8) / 64 + 1) * 64;
            byte[] padded = new byte[paddedLen];

            Buffer.BlockCopy(input, 0, padded, 0, len);
            padded[len] = 0x80;
            long bits = (long)len * 8;
            padded[paddedLen - 8] = (byte)(bits & 0xFF);
            padded[paddedLen - 7] = (byte)((bits >> 8) & 0xFF);
            padded[paddedLen - 6] = (byte)((bits >> 16) & 0xFF);
            padded[paddedLen - 5] = (byte)((bits >> 24) & 0xFF);
            padded[paddedLen - 4] = (byte)((bits >> 32) & 0xFF);
            padded[paddedLen - 3] = (byte)((bits >> 40) & 0xFF);
            padded[paddedLen - 2] = (byte)((bits >> 48) & 0xFF);
            padded[paddedLen - 1] = (byte)((bits >> 56) & 0xFF);

            uint h0 = 0x67452301, h1 = 0xEFCDAB89, h2 = 0x98BADCFE, h3 = 0x10325476;

            for (int i = 0; i < paddedLen; i += 64)
            {
                for (int j = 0; j < 16; j++)
                    x[j] = (uint)(padded[i + j * 4] | (padded[i + j * 4 + 1] << 8) |
                                  (padded[i + j * 4 + 2] << 16) | (padded[i + j * 4 + 3] << 24));

                a = h0; b = h1; c = h2; d = h3;

                a = FF(a, b, c, d, x[0], 7, 0xD76AA478); d = FF(d, a, b, c, x[1], 12, 0xE8C7B756);
                c = FF(c, d, a, b, x[2], 17, 0x242070DB); b = FF(b, c, d, a, x[3], 22, 0xC1BDCEEE);
                a = FF(a, b, c, d, x[4], 7, 0xF57C0FAF); d = FF(d, a, b, c, x[5], 12, 0x4787C62A);
                c = FF(c, d, a, b, x[6], 17, 0xA8304613); b = FF(b, c, d, a, x[7], 22, 0xFD469501);
                a = FF(a, b, c, d, x[8], 7, 0x698098D8); d = FF(d, a, b, c, x[9], 12, 0x8B44F7AF);
                c = FF(c, d, a, b, x[10], 17, 0xFFFF5BB1); b = FF(b, c, d, a, x[11], 22, 0x895CD7BE);
                a = FF(a, b, c, d, x[12], 7, 0x6B901122); d = FF(d, a, b, c, x[13], 12, 0xFD987193);
                c = FF(c, d, a, b, x[14], 17, 0xA679438E); b = FF(b, c, d, a, x[15], 22, 0x49B40821);

                a = GG(a, b, c, d, x[1], 5, 0xF61E2562); d = GG(d, a, b, c, x[6], 9, 0xC040B340);
                c = GG(c, d, a, b, x[11], 14, 0x265E5A51); b = GG(b, c, d, a, x[0], 20, 0xE9B6C7AA);
                a = GG(a, b, c, d, x[5], 5, 0xD62F105D); d = GG(d, a, b, c, x[10], 9, 0x02441453);
                c = GG(c, d, a, b, x[15], 14, 0xD8A1E681); b = GG(b, c, d, a, x[4], 20, 0xE7D3FBC8);
                a = GG(a, b, c, d, x[9], 5, 0x21E1CDE6); d = GG(d, a, b, c, x[14], 9, 0xC33707D6);
                c = GG(c, d, a, b, x[3], 14, 0xF4D50D87); b = GG(b, c, d, a, x[8], 20, 0x455A14ED);
                a = GG(a, b, c, d, x[13], 5, 0xA9E3E905); d = GG(d, a, b, c, x[2], 9, 0xFCEFA3F8);
                c = GG(c, d, a, b, x[7], 14, 0x676F02D9); b = GG(b, c, d, a, x[12], 20, 0x8D2A4C8A);

                a = HH(a, b, c, d, x[5], 4, 0xFFFA3942); d = HH(d, a, b, c, x[8], 11, 0x8771F681);
                c = HH(c, d, a, b, x[11], 16, 0x6D9D6122); b = HH(b, c, d, a, x[14], 23, 0xFDE5380C);
                a = HH(a, b, c, d, x[1], 4, 0xA4BEEA44); d = HH(d, a, b, c, x[4], 11, 0x4BDECFA9);
                c = HH(c, d, a, b, x[7], 16, 0xF6BB4B60); b = HH(b, c, d, a, x[10], 23, 0xBEBFBC70);
                a = HH(a, b, c, d, x[13], 4, 0x289B7EC6); d = HH(d, a, b, c, x[0], 11, 0xEAA127FA);
                c = HH(c, d, a, b, x[3], 16, 0xD4EF3085); b = HH(b, c, d, a, x[6], 23, 0x04881D05);
                a = HH(a, b, c, d, x[9], 4, 0xD9D4D039); d = HH(d, a, b, c, x[12], 11, 0xE6DB99E5);
                c = HH(c, d, a, b, x[15], 16, 0x1FA27CF8); b = HH(b, c, d, a, x[2], 23, 0xC4AC5665);

                a = II(a, b, c, d, x[0], 6, 0xF4292244); d = II(d, a, b, c, x[7], 10, 0x432AFF97);
                c = II(c, d, a, b, x[14], 15, 0xAB9423A7); b = II(b, c, d, a, x[5], 21, 0xFC93A039);
                a = II(a, b, c, d, x[12], 6, 0x655B59C3); d = II(d, a, b, c, x[3], 10, 0x8F0CCC92);
                c = II(c, d, a, b, x[10], 15, 0xFFEFF47D); b = II(b, c, d, a, x[1], 21, 0x85845DD1);
                a = II(a, b, c, d, x[8], 6, 0x6FA87E4F); d = II(d, a, b, c, x[15], 10, 0xFE2CE6E0);
                c = II(c, d, a, b, x[6], 15, 0xA3014314); b = II(b, c, d, a, x[13], 21, 0x4E0811A1);
                a = II(a, b, c, d, x[4], 6, 0xF7537E82); d = II(d, a, b, c, x[11], 10, 0xBD3AF235);
                c = II(c, d, a, b, x[2], 15, 0x2AD7D2BB); b = II(b, c, d, a, x[9], 21, 0xEB86D391);

                h0 += a; h1 += b; h2 += c; h3 += d;
            }

            byte[] result = new byte[16];
            for (int i = 0; i < 4; i++) { result[i] = (byte)(h0 >> (i * 8)); }
            for (int i = 0; i < 4; i++) { result[4 + i] = (byte)(h1 >> (i * 8)); }
            for (int i = 0; i < 4; i++) { result[8 + i] = (byte)(h2 >> (i * 8)); }
            for (int i = 0; i < 4; i++) { result[12 + i] = (byte)(h3 >> (i * 8)); }
            return result;
        }

        private static uint F(uint x, uint y, uint z) => (x & y) | (~x & z);
        private static uint G(uint x, uint y, uint z) => (x & z) | (y & ~z);
        private static uint H(uint x, uint y, uint z) => x ^ y ^ z;
        private static uint I(uint x, uint y, uint z) => y ^ (x | ~z);

        private static uint RL(uint x, int n) => (x << n) | (x >> (32 - n));

        private static uint FF(uint a, uint b, uint c, uint d, uint x, int s, uint ac) =>
            b + RL(a + F(b, c, d) + x + ac, s);
        private static uint GG(uint a, uint b, uint c, uint d, uint x, int s, uint ac) =>
            b + RL(a + G(b, c, d) + x + ac, s);
        private static uint HH(uint a, uint b, uint c, uint d, uint x, int s, uint ac) =>
            b + RL(a + H(b, c, d) + x + ac, s);
        private static uint II(uint a, uint b, uint c, uint d, uint x, int s, uint ac) =>
            b + RL(a + I(b, c, d) + x + ac, s);
    }
}
