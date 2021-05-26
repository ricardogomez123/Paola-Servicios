using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Xml.Linq;
using System.Diagnostics;
using CertificadorWs;
using CertificadorWs.Business;
using System.Security.Cryptography;
using ServicioLocal.Business;
using ServicioLocalContract;


namespace BusinessLogic
{
    /// <summary>
    /// Generador de timbre
    /// </summary>
    public class GeneradorTimbreFiscalDigital : NtLinkBusiness
    {
        static GeneradorCadenasTimbre GeneradorCadenas;
        static GeneradorCadenasTimbreRetencion GeneradorCadenasRetencion;
        private readonly SerializadorTimbres _serializadorTimbres = new SerializadorTimbres();
        private readonly Sellador _sellador;
        private static Dictionary<string, string> _salida = new Dictionary<string, string>();
        RSACryptoServiceProvider objCert = null;
        
        public Dictionary<string, string> Salida
        {
            get { return _salida; }
            set { _salida = value; }
        }

        public GeneradorTimbreFiscalDigital(string slotHSM, int llave)
        { 
            _sellador = new Sellador(slotHSM, llave);
            if (GeneradorCadenas == null)
                GeneradorCadenas = new GeneradorCadenasTimbre();
            if (GeneradorCadenasRetencion == null)
                GeneradorCadenasRetencion = new GeneradorCadenasTimbreRetencion();
        }
//----------------------------------
   public ServicioLocal.Business.TimbreRetenciones.TimbreFiscalDigital GeneraTimbreFiscalDigitalRetencionesCadenas(string strRfc, string noCertificado, string selloCfd, XElement entrada, Guid uuid)
{
	ServicioLocal.Business.TimbreRetenciones.TimbreFiscalDigital result;
	try
	{
		ServicioLocal.Business.TimbreRetenciones.TimbreFiscalDigital timbreFiscalDigital = new ServicioLocal.Business.TimbreRetenciones.TimbreFiscalDigital
		{
			FechaTimbrado = Convert.ToDateTime(DateTime.Now.ToString("s")),
			noCertificadoSAT = noCertificado,
			selloCFD = selloCfd,
			selloSAT = "@",
			UUID = uuid.ToString().ToUpper(),
			version = "1.0"
		};
		string timbreXml = this._serializadorTimbres.GetTimbreRenecionesXml(timbreFiscalDigital);
		lock (GeneradorTimbreFiscalDigital.GeneradorCadenas)
		{
			//timbreFiscalDigital.cadenaOriginal = GeneradorTimbreFiscalDigital.GeneradorCadenas.CadenaOriginal(timbreXml);
            timbreFiscalDigital.cadenaOriginal = GeneradorTimbreFiscalDigital.GeneradorCadenasRetencion.CadenaOriginal(timbreXml);
		
        }
		lock (this._sellador)
		{
			if (ConfigurationManager.AppSettings["FirmaLocal"] == "true")
			{
				string certPac = ConfigurationManager.AppSettings["CertPac"];
				if (this.objCert == null)
				{
					this.objCert = OpensslKey.DecodePrivateKey(File.ReadAllBytes(certPac), ConfigurationManager.AppSettings["PassPac"], ".key");
				}
				timbreFiscalDigital.selloSAT = this._sellador.GeneraSelloDigitalRetencion(timbreFiscalDigital.cadenaOriginal, this.objCert);
			}
			else
			{
				timbreFiscalDigital.selloSAT = this._sellador.GeneraSelloDigitalTimbreRetencion(timbreFiscalDigital.cadenaOriginal);
			}
			if (timbreFiscalDigital.selloSAT == null || timbreFiscalDigital.selloSAT == "Error")
			{
				result = null;
				return result;
			}
		}
		result = timbreFiscalDigital;
	}
	catch (Exception ee)
	{
		NtLinkBusiness.Logger.Error(ee);
		result = null;
	}
	return result;
}
        //---------------------------------

        public TimbreFiscalDigital GeneraTimbreFiscalDigitalCadenas(string strRfc, string noCertificado, string selloCfd, XElement entrada, Guid uuid, string RfcProvCertif, string Leyenda)
        {
            try
            {
                var timbreFiscalDigital = new TimbreFiscalDigital();
                    
                if (!string.IsNullOrEmpty(Leyenda))
                {
                     timbreFiscalDigital = new TimbreFiscalDigital
                                                 {
                                                     FechaTimbrado = Convert.ToDateTime(DateTime.Now.ToString("s")),
                                                     NoCertificadoSAT = noCertificado,
                                                     SelloCFD = selloCfd,
                                                     SelloSAT = "@",
                                                     UUID = uuid.ToString().ToUpper(),
                                                     Version = "1.1",
                                                     RfcProvCertif = RfcProvCertif,
                                                     Leyenda = Leyenda
                                                 };
                }
                else
                {
                     timbreFiscalDigital = new TimbreFiscalDigital
                    {
                        FechaTimbrado = Convert.ToDateTime(DateTime.Now.ToString("s")),
                        NoCertificadoSAT = noCertificado,
                        SelloCFD = selloCfd,
                        SelloSAT = "@",
                        UUID = uuid.ToString().ToUpper(),
                        Version = "1.1",
                        RfcProvCertif = RfcProvCertif
                        
                    };
                
                
                }
                string timbreXml = _serializadorTimbres.GetTimbreXml(timbreFiscalDigital);
                lock (GeneradorCadenas)
                {
                    timbreFiscalDigital.cadenaOriginal = GeneradorCadenas.CadenaOriginal(timbreXml);
                }
                
                
                lock (_sellador)
                {
                    if (ConfigurationManager.AppSettings["FirmaLocal"] == "true")
                    {
                        string certPac = ConfigurationManager.AppSettings["CertPac"];
                        if (objCert == null)
                            objCert= OpensslKey.DecodePrivateKey(File.ReadAllBytes(certPac), ConfigurationManager.AppSettings["PassPac"],".key");
                        timbreFiscalDigital.SelloSAT = this._sellador.GeneraSelloDigital(timbreFiscalDigital.cadenaOriginal, objCert);
                        //timbreFiscalDigital.selloSAT = "Inválido, ambiente de pruebas";
                    }
                    else
                    {
                        timbreFiscalDigital.SelloSAT = this._sellador.GeneraSelloDigitalTimbre(timbreFiscalDigital.cadenaOriginal);
                    }
                    if (timbreFiscalDigital.SelloSAT == null || timbreFiscalDigital.SelloSAT == "Error")
                    {
                        return null;
                    }
                }
                return timbreFiscalDigital;
            }
            catch (Exception ee)
            {
                Logger.Error(ee);
                return null;
            }
        }


       

        //private string ConcatenaTimbre(XElement entrada, string xmlFinal)
        //{
        //    XElement timbre = XElement.Load(new StringReader(xmlFinal));
        //    var complemento = entrada.Elements(Constantes.CFDVersionNamespace + "Complemento").FirstOrDefault();
        //    if (complemento == null)
        //    {
        //        entrada.Add(new XElement(Constantes.CFDVersionNamespace + "Complemento"));
        //        complemento = entrada.Elements(Constantes.CFDVersionNamespace + "Complemento").FirstOrDefault();
        //    }
        //    complemento.Add(timbre);
        //    var mem = new MemoryStream();
        //    var tw = new StreamWriter(mem, Encoding.UTF8);
        //    entrada.Save(tw, SaveOptions.DisableFormatting);
        //    string xml = Encoding.UTF8.GetString(mem.GetBuffer());
        //    xml = xml.Substring(xml.IndexOf(Convert.ToChar(60)));
        //    xml = xml.Substring(0, (xml.LastIndexOf(Convert.ToChar(62)) + 1));
        //    return xml;
        //}

       
    }
}
