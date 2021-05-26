using System;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using ServicioLocalContract;

namespace CertificadorWs.Business
{
    public class CertUtil
    {
        public static bool ValidaCert(string fileName)
        {
            try
            {
                X509Certificate2 certificate = new X509Certificate2(fileName);
                return true;
            }
            catch (Exception)
            {
                throw new ApplicationException("El archivo no es un certificado válido");
            }
           
        }

        


       

        



        public static string ValidarLongitudCertificado(string serie)
        {
            var result = new StringBuilder();
            if (serie.Length > 20)
            {
                for (int i = 1; i < serie.Length; i++)
                {
                    if (i % 2 != 0)
                        result.Append(serie[i]);
                }
                return result.ToString();
            }
            return serie;
        }

    }
}
