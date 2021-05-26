using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Xml.Linq;
using Org.BouncyCastle.X509;
using ServicioLocal.Business;
using ServicioLocalContract;
using log4net;
using log4net.Config;
using I_RFC_SAT;

namespace ServicioLocal.Business
{
    public class ValidadorDatos32
    {
        private static ILog Logger = LogManager.GetLogger(typeof (ValidadorDatos32));
       
        public ValidadorDatos32()
        {
            XmlConfigurator.Configure();
            
        }

        private bool VerificaEmisorCertificado(byte[] certificado, byte[] certificadoAC)
        {
            Org.BouncyCastle.X509.X509Certificate cer1 = new X509CertificateParser().ReadCertificate(certificado);
            Org.BouncyCastle.X509.X509Certificate cer2 = new X509CertificateParser().ReadCertificate(certificadoAC);
            try
            {
                cer1.Verify(cer2.GetPublicKey());
                return true;
            }
            catch (Exception ee)
            {
                Logger.Error(ee);
                return false;
            }
        }


        private bool VerifyCertificate(byte[] primaryCertificate, IEnumerable<byte[]> additionalCertificates)
        {
            var chain = new X509Chain();
            foreach (var cert in additionalCertificates.Select(x => new X509Certificate2(x)))
            {
                chain.ChainPolicy.ExtraStore.Add(cert);
            }
            chain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;
            chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllowUnknownCertificateAuthority;
            var primaryCert = new X509Certificate2(primaryCertificate);
            return chain.Build(primaryCert);
        }




        public int ValidaCertificadoAc(X509Certificate2 certificado)
        {

            try
            {
               
                var directorio = ConfigurationManager.AppSettings["CertACSat"];
                var archivosCert = Directory.EnumerateFiles(directorio, "*.cer");
                if (archivosCert.Select(s => File.ReadAllBytes(s)).
                    Any(archivo => VerificaEmisorCertificado(certificado.RawData, archivo)))
                {
                    return 0;
                }
                return 308;
                /*  */
               
            }
            catch (Exception err)
            {
                Logger.Error(err);
                return 308;
            }
        }


        //public int ValidaCertificadoAC(X509Certificate2 x50MCertificado)
        //{
        //    try
        //    {
        //        if (x50MCertificado == null) return 0;
        //        Guid strAleatorio = Guid.NewGuid();

        //        //obtenemos el certificado a validar//
        //        Directory.CreateDirectory("CertificadosTmp\\");

        //        string strCertificadoaaValidar = "CertificadosTmp\\" + strAleatorio.ToString() + ".cer";
        //        File.WriteAllBytes(strCertificadoaaValidar, x50MCertificado.GetRawCertData());

        //        //Función de Cryptosys para la validación del certificado contra su entidad emisora//
        //        int inTmp = 0;
        //        inTmp = CryptoSysPKI.X509.VerifyCert(strCertificadoaaValidar, "Certificados\\AC_SAT.cer");

        //        File.Delete(strCertificadoaaValidar);

        //        return inTmp == -1 ? 308 : 0;
        //    }
        //    catch (Exception err)
        //    {
        //        return 308;
        //    }
        //}


        public int ValidaCertificadoCSDnoFIEL(X509Certificate2 certificado)
        {
            try
            {
                bool valido = false;
                var certificate2 = certificado;
                foreach (var extension in certificate2.Extensions)
                {
                    if (extension.Oid.Value != "2.5.29.15") continue;
                    var exten = (X509KeyUsageExtension) extension;
                    if (exten.KeyUsages.ToString().Equals("NonRepudiation, DigitalSignature"))
                        valido = true;
                }
                return !valido ? 306 : 0;
            }
            catch (Exception ee)
            {
                Logger.Error(ee);
                return 306;
            }
        }

        public int ValidarSello(string cadena, byte[] firma, X509Certificate2 certificado, ref string sha1Hash)
        {
            try
            {
                var rsa = (RSACryptoServiceProvider) certificado.PublicKey.Key;
                var sha = new SHA1Managed();
                byte[] hash = sha.ComputeHash(Encoding.UTF8.GetBytes(cadena));
                sha1Hash = BitConverter.ToString(hash);
                sha1Hash = sha1Hash.Replace("-", "");
                return rsa.VerifyHash(hash, CryptoConfig.MapNameToOID("SHA1"), firma) ? 0 : 302;
            }
            catch (Exception ee)
            {
                Logger.Error("", ee);
                return 302;
            }
        }

        public int ValidaRFCEmisor(string rfc, string name)
        {
            try
            {
                name = name.Replace("\"", "");
                string strLRfc = name.Substring(name.LastIndexOf("2.5.4.45=") + 9, 13).Trim();
                return strLRfc != rfc ? 303 : 0;
            }
            catch (Exception ee)
            {
                Logger.Error("", ee);
                return 303;
            }
        }

        public int VerificaFolioSerieCfd(string folio, string serie, string noAprobacion, string anioAprobacion)
        {
            try
            {
                var lcoLogic = new LcoLogic();
                var lco = lcoLogic.SearchFolioSerie(folio, serie, noAprobacion, anioAprobacion);
                if (lco == null) { return 409; }
                return 0;
            }
            catch (Exception ee)
            {
                Logger.Error("", ee);
                return 309;
            }
        }

        public int VerificaCSDRevocado(string serieCert, DateTime fecha, string version)
        {
            var lcoLogic = new LcoLogic();
            var lcoLogic2 = new Operaciones_IRFC();
            if (version == "3.2")
            {
              
                vLCO lco = lcoLogic2.SearchLCOByNoCertificado(serieCert);
                try
                {
                    if (lco == null) { return 304; }

                    if (lco.ValidezObligaciones.Equals("0", StringComparison.CurrentCultureIgnoreCase) ||
                        (fecha < lco.FechaInicio || fecha > lco.FechaFinal) ||
                        !lco.EstatusCertificado.Contains("A"))
                    {
                        return 304;
                    }
                    return 0;
                }
                catch (Exception ee)
                {
                    Logger.Error("", ee);
                    return 304;
                }
            }
            else
            {
                var lco = lcoLogic.SearchCsdBySerie(serieCert,fecha );
                try
                {
                    if (lco == null) { return 304; }
                    return 0;
                }
                catch (Exception ee)
                {
                    Logger.Error("", ee);
                    return 304;
                }
            }
            
        }

        public int ValidaFechaEmisionXml(DateTime dtFechaEmision, DateTime dtFechaExpiracion, DateTime dtFechaEfectiva)
        {
            if (dtFechaExpiracion < dtFechaEmision || dtFechaEfectiva > dtFechaEmision)
            {
                return 305;
            }
            return 0;
        }
        private static XNamespace _versionNamespace = XNamespace.Get("http://www.sat.gob.mx/cfd/3");

      

        public int ValidaRangoFecha(DateTime fecha, DateTime fechaTimbrado)
        {
            try
            {
                TimeSpan ts = fechaTimbrado - fecha;
                if (ts.TotalMilliseconds < 0)
                    return 401;
                else return 0;
            }
            catch (Exception ee)
            {
                Logger.Error("", ee);
                return 401;
            }
        }

        public int ValidaRFCLCO(string rfc)
        {
            try
            {
               // var lcoLogic = new LcoLogic();
                var lcoLogic = new Operaciones_IRFC();
                vLCO lco = lcoLogic.SearchLCOByRFC(rfc);
                return lco == null ? 402 : 0;

            }
            catch (Exception ee)
            {
                Logger.Error("", ee);
                return 402;
            }
            
        }

        public int ValidaFechaEmision2011(DateTime datFechaEmisionXML)
        {
            try
            {
                if (DateTime.Parse("2011-01-01") > datFechaEmisionXML)
                    return 403;
                return 0;
            }
            catch (Exception ee)
            {
                Logger.Error("", ee);
                return 403;
            }
        }
    }
}