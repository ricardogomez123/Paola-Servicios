using System;
using System.Security.Cryptography;
using System.Text;
using HSMXml;
using HSMXml.LunaXml;
using log4net;
using log4net.Config;
using System.Configuration;

//using log4net;

namespace CertificadorWs.Business
{
    public class Sellador
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(Sellador));
       
        private byte[] _padding = new byte[]
                                      {
                                          0x30, 0x21, 0x30, 0x09, 0x06, 0x05, 0x2B, 0x0E, 0x03, 0x02, 0x1A, 0x05, 0x00,
                                          0x04, 0x14
                                      };

        public Sellador(string pin, int inLlave)
        {
            XmlConfigurator.Configure();
        }
        //--------------------------------------
        public string GeneraSelloDigitalTimbreRetencion(string cadenaOriginal)
        {
            string result;
            try
            {
                string usuario = ConfigurationManager.AppSettings["UsuarioHsm"];
                string contraseña = ConfigurationManager.AppSettings["ContraseñaHsm"];
                string aliaspk = ConfigurationManager.AppSettings["AliasPK"];
                HsmXmlConnect hsm = new HsmXmlConnect();
                authTokenType token = hsm.Login(usuario, contraseña);
                string sello = hsm.Firmar(ref token, cadenaOriginal, aliaspk, SignatureModeType.SHA1withRSA);
                hsm.Logout(token);
                result = sello;
            }
            catch (Exception Ex)
            {
                Sellador.Logger.Error(Ex.ToString());
                result = "Error";
            }
            return result;
        }

        public string GeneraSelloDigitalRetencion(string cadenaOriginal, RSACryptoServiceProvider objCert)
        {
            string result;
            try
            {
                /*
                SHA1CryptoServiceProvider hasher = new SHA1CryptoServiceProvider();
                byte[] bytesFirmados = objCert.SignData(Encoding.UTF8.GetBytes(cadenaOriginal), hasher);
                string sello = Convert.ToBase64String(bytesFirmados);
                result = sello;
                 
                UTF8Encoding e = new UTF8Encoding(true);
                byte[] signature = objCert.SignData(e.GetBytes(cadenaOriginal), "SHA256");
                string sello256 = Convert.ToBase64String(signature);
                return sello256;

                	string result;
                */
	
		SHA1CryptoServiceProvider hasher = new SHA1CryptoServiceProvider();
		byte[] bytesFirmados = objCert.SignData(Encoding.UTF8.GetBytes(cadenaOriginal), hasher);
		string sello = Convert.ToBase64String(bytesFirmados);
		result = sello;

            }
            catch (Exception Ex)
            {
                Sellador.Logger.Error(Ex.ToString());
                result = "Error";
            }
           return result;
        }
       //----------------------------------------------------------------------
        public String GeneraSelloDigital(String cadenaOriginal, RSACryptoServiceProvider objCert)
        {
            try
            { /*
                //var hasher = new SHA1CryptoServiceProvider();
                var hasher = new SHA256CryptoServiceProvider();// sha -2 porcambio cfdi 3.3
                Byte[] bytesFirmados = objCert.SignData(Encoding.UTF8.GetBytes(cadenaOriginal), hasher);
                String sello = Convert.ToBase64String(bytesFirmados);
                return sello;
               */

                UTF8Encoding e = new UTF8Encoding(true);
                byte[] signature = objCert.SignData(e.GetBytes(cadenaOriginal), "SHA256");
                string sello256 = Convert.ToBase64String(signature);
                return sello256;
               
            }
            catch (Exception Ex)
            {
                Logger.Error(Ex.ToString());
                return "Error";
            }
        }

        public String GeneraSelloDigitalTimbre(String cadenaOriginal)
        {
            try
            {
                string usuario = ConfigurationManager.AppSettings["UsuarioHsm"];
                string contraseña = ConfigurationManager.AppSettings["ContraseñaHsm"];
                string aliaspk = ConfigurationManager.AppSettings["AliasPK"];
                HsmXmlConnect hsm = new HsmXmlConnect();
                authTokenType token = hsm.Login(usuario, contraseña); 
              //  String sello = hsm.Firmar(ref token, cadenaOriginal, aliaspk, SignatureModeType.SHA1withRSA);
                String sello = hsm.Firmar(ref token, cadenaOriginal, aliaspk, SignatureModeType.SHA256withRSA);
                hsm.Logout(token); 
                return sello;
            }
            catch (Exception Ex)
            {
                Logger.Error(Ex.ToString());
                return "Error";
            }
        }
    }
}