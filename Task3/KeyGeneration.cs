using System.Security.Cryptography;

namespace Task3
{
    public static class KeyGeneration
    {
        public static byte[] Generate()
        {
            var key = new byte[32];
            using (var random = RandomNumberGenerator.Create())
                random.GetBytes(key);
            return key;
        }
    }
}
