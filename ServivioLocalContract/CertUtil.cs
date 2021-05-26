using System;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace ServicioLocalContract
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

        


        
    }
}
