using System.Security.Cryptography;
using System.Text;

namespace Task3
{
    public static class Hmac
    {
        public static byte[] Calculate(string data, byte[] key)
        {
            using var hmac = new HMACSHA256(key);
            return hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
        }
    }
}
