﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Book.Services.Crypto
{
    static class DHKeyExchange
    {
        public static readonly BigInteger2 p = new BigInteger2("1df7c01a4554fd826c5eb26ed9946a07a8460a36605a26f947a9728db6e47b3132b3520927d7b4009e41bd3be74ed35ca97c5509474b13779f196e7a767402babb2e3e9fcfe60d79dd9db948b3662abdf87153c1206651cdaad3d76dc8abdd05b4a5cb157d15bc9f561a68f0fc4ac4bca0447c445c25904fd5ea2e7ab0fcfba0b4b129bb7fe7bd5a8527887a25195ab6c5cec449e2d24ca87babee526d2e120b672d3663905aa33ed34b73e08072bee519fb7d08514d2e2a012f94506765cc18c27206f0f540fae9203b56cdcc7645ac3d45520e4d2128a5f0b56053fa19188a775793886ea8bf86b8b726748ac9f1ae9610b3c046d229af799da1c059fd76edb", 16);
        public static readonly BigInteger2 g = BigInteger2.ValueOf(5);
        public static readonly int KeyBits = 2048;

        public static byte[] CalculateSharedKey(byte[] receivePublicKey, byte[] publickKey)
        {
            var A = new BigInteger2(1, receivePublicKey);
            var b = new BigInteger2(1, publickKey);
            var sharedKey = A.ModPow(b, p);
            var sharedKeyBytes = sharedKey.ToByteArray();
            var ret = new byte[32];
            Buffer.BlockCopy(sharedKeyBytes, 0, ret, 0, ret.Length);
            return ret;
        }

        public static byte[] GetPrivateKey()
        {
            return BigInteger2.Arbitrary(KeyBits).ToByteArray();
        }
        public static byte[] GetPublicKey(byte[] k)
        {
            var kk = new BigInteger2(1, k);
            var pk = g.ModPow(kk, p);
            return pk.ToByteArray();
        }
    }
}
