using System;
using System.Security.Cryptography;
using System.Text;

namespace CertificadorWs.Business
{
    class GeneradorSelloDigital
    {
        public static String Sellar(string cadenaOriginal, string pkPemBase64)
        {
            RSACryptoServiceProvider provider = KeyUtils.DecodePrivateKeyInfo(Convert.FromBase64String(pkPemBase64));
            SHA1Managed man = new SHA1Managed();
            byte[] firma = provider.SignData(Encoding.UTF8.GetBytes(cadenaOriginal), man);
            return Convert.ToBase64String(firma);
        }

    }
}
