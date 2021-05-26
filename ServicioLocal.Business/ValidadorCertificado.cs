
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.X509;
using Org.BouncyCastle.Asn1.X509;


namespace ServicioLocal.Business
{

    public class ValidadorCertificado : NtLinkBusiness, ICloneable, IDisposable
    {

        private Dictionary<string, RSAPKCS1SignatureDeformatter> _certificados;

        public ValidadorCertificado()
        {
            _certificados = new Dictionary<string, RSAPKCS1SignatureDeformatter>();
            var ruta = ConfigurationManager.AppSettings["CertsValidacion"];
            if (Directory.Exists(ruta))
            {
                var certs = Directory.EnumerateFiles(ruta);
                foreach (var cert in certs)
                {
                    X509CertificateParser parser = new X509CertificateParser();
                    Org.BouncyCastle.X509.X509Certificate c = parser.ReadCertificate(File.ReadAllBytes(cert));
                    var pk = c.GetPublicKey() as RsaKeyParameters;
                    var rsaParameters = new RSAParameters();
                    rsaParameters.Modulus = pk.Modulus.ToByteArrayUnsigned();
                    rsaParameters.Exponent = pk.Exponent.ToByteArrayUnsigned();
                    RSACryptoServiceProvider rsa = (RSACryptoServiceProvider) RSA.Create();
                    rsa.ImportParameters(rsaParameters);
                    RSAPKCS1SignatureDeformatter deformatter = new RSAPKCS1SignatureDeformatter(rsa);
                    deformatter.SetHashAlgorithm("SHA1");
                    _certificados.Add(Path.GetFileNameWithoutExtension(cert), deformatter);
                }
            }
        }


        public int VerificaSelloPac(string serieCert, string sello, string cadenaOriginal)
        {
            try
            {
                if (_certificados.ContainsKey(serieCert))
                {
                    byte[] bytesSello = Convert.FromBase64String(sello);
                    RSAPKCS1SignatureDeformatter formatter = _certificados[serieCert];
                    SHA1Managed managed = new SHA1Managed();
                    var bytesCadena = Encoding.UTF8.GetBytes(cadenaOriginal);
                    var hash = managed.ComputeHash(bytesCadena);
                    Console.WriteLine(cadenaOriginal);
                    Console.WriteLine("Hash -> " + BitConverter.ToString(hash).Replace("-",""));
                    return formatter.VerifySignature(hash, bytesSello) ? 0 : 404;
                }

            }
            catch (Exception ee)
            {
                Logger.Error(ee);
                if (ee.InnerException != null)
                    Logger.Error(ee.InnerException);
            }
            return 404;
        }


        public object Clone()
        {
            var c = this.MemberwiseClone() as ValidadorCertificado;

            c._certificados = new Dictionary<string, RSAPKCS1SignatureDeformatter>(_certificados);
            return c;
        }

        public void Dispose()
        {
            _certificados = null;
        }
    }
}


