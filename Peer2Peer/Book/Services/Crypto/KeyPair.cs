using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Book.Services.Crypto
{
    public sealed class KeyPair
    {
        public KeyPair(string privateKey, string publicKey)
        {
            PrivateKey = privateKey;
            PublicKey = publicKey;

        }

        public string PublicKey { get; set; }
        public string PrivateKey { get; set; }
        public static KeyPair Create()
        {
            byte[] seedbuf = new byte[32];
            byte[] pubbuf;
            byte[] prvbuf;

            RNGCryptoServiceProvider.Create().GetBytes(seedbuf);
            Ed25519.KeyPairFromSeed(out pubbuf, out prvbuf, seedbuf);
            CryptoBytes.Wipe(seedbuf);
            return new KeyPair(Convert.ToBase64String(prvbuf), Convert.ToBase64String(pubbuf));
        }

        internal static bool Verify(string publicKey, string signature, string message)
        {
            return Ed25519.Verify(Convert.FromBase64String(signature), Encoding.Default.GetBytes(message), Convert.FromBase64String(publicKey));
        }

        internal static string Sign(string privateKey, string message)
        {
            return Convert.ToBase64String(Ed25519.Sign(Encoding.Default.GetBytes(message), Convert.FromBase64String(privateKey)));
        }

    }
}
