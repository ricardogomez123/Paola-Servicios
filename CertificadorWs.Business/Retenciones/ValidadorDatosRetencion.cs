using BusinessLogic;
using EnvioRetenciones;
using ServicioLocal.Business;
using ServicioLocal.Business.TimbreRetenciones;
using ServicioLocalContract;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace CertificadorWs.Business.Retenciones
{
    public class ValidadorDatosRetencion : NtLinkBusiness
    {
        private static CadenaOriginal _genCadenas;

        private static ValidadorEstructuraRetenciones _validadorEstructura;

        private readonly object _lock = new object();

        private readonly string _rutaEntrada = ConfigurationManager.AppSettings["rutaEntrada"];

        public ValidadorDatosRetencion()
        {
            lock (this._lock)
            {
                if (ValidadorDatosRetencion._genCadenas == null)
                {
                    ValidadorDatosRetencion._genCadenas = new CadenaOriginal("retenciones.xslt");
                }
                if (ValidadorDatosRetencion._validadorEstructura == null)
                {
                    ValidadorDatosRetencion._validadorEstructura = new ValidadorEstructuraRetenciones();
                }
            }
        }

        public Dictionary<int, string> ProcesarCadena(string cadenaXml, ref string res, ref ServicioLocal.Business.TimbreRetenciones.TimbreFiscalDigital timbre, ref string acuseSat, ref string hash)
        {
            Dictionary<int, string> result;
            try
            {
                string ruta = Path.Combine(this._rutaEntrada, DateTime.Now.Year.ToString(), DateTime.Now.Month.ToString().PadLeft(2, '0'), DateTime.Now.Day.ToString().PadLeft(2, '0'));
                if (!Directory.Exists(ruta))
                {
                    Directory.CreateDirectory(ruta);
                }
                Guid uid = Guid.NewGuid();
                string archivo = Path.Combine(ruta, uid.ToString() + ".xml");
                File.WriteAllText(archivo, cadenaXml, Encoding.UTF8);
                XElement xe = XElement.Load(new StringReader(cadenaXml));
                acuseSat = "";
                Dictionary<int, string> x = this.Procesar(xe, "WS", null, uid, ref res, ref timbre, ref acuseSat, ref hash);
                NtLinkBusiness.Logger.Debug(x);
                result = x;
            }
            catch (Exception eee)
            {
                NtLinkBusiness.Logger.Error(eee);
                result = this.CrearArchivoROE(new List<int>
				{
					666
				}, "WS", eee.Message);
            }
            return result;
        }

        public Dictionary<int, string> ProcesarCadenaRetencion(string cadenaXml, ref string res, ref ServicioLocal.Business.TimbreRetenciones.TimbreFiscalDigital timbre, ref string acuseSat, ref string hash)
        {
            Dictionary<int, string> result;
            try
            {
                string ruta = Path.Combine(this._rutaEntrada, DateTime.Now.Year.ToString(), DateTime.Now.Month.ToString().PadLeft(2, '0'), DateTime.Now.Day.ToString().PadLeft(2, '0'));
                if (!Directory.Exists(ruta))
                {
                    Directory.CreateDirectory(ruta);
                }
                Guid uid = Guid.NewGuid();
                string archivo = Path.Combine(ruta, uid.ToString() + ".xml");
                File.WriteAllText(archivo, cadenaXml, Encoding.UTF8);
                XElement xe = XElement.Load(new StringReader(cadenaXml));
                acuseSat = "";
                Dictionary<int, string> x = this.Procesar(xe, "WS", null, uid, ref res, ref timbre, ref acuseSat, ref hash);
                NtLinkBusiness.Logger.Debug(x);
                result = x;
            }
            catch (Exception eee)
            {
                NtLinkBusiness.Logger.Error(eee);
                result = this.CrearArchivoROE(new List<int>
				{
					666
				}, "WS", eee.Message);
            }
            return result;
        }

        private Dictionary<int, string> Procesar(XElement xe, string archivoEntrada, XElement addenda, Guid uuid, ref string res, ref ServicioLocal.Business.TimbreRetenciones.TimbreFiscalDigital timbre, ref string acuseSat, ref string hash)
        {
            Dictionary<int, string> erroresPac = new Dictionary<int, string>();
            Dictionary<int, string> result;
            try
            {
                string strContent = xe.ToString();
                List<int> errores = new List<int>();
                string version = (xe.Attribute("Version") == null) ? "" : xe.Attribute("Version").Value;
                if (version == "")
                {
                    errores.Add(1002);
                    List<int> errorRoe = (from p in errores
                                          where p != 0
                                          select p).ToList<int>();
                    erroresPac = this.CrearArchivoROE(errorRoe, archivoEntrada, "");
                    result = erroresPac;
                    return result;
                }
                ValidadorEstructuraRetenciones validadorEstructura = ValidadorDatosRetencion._validadorEstructura.Clone() as ValidadorEstructuraRetenciones;
                ValidacionEstructuraRetencion errorXmlValidacion = validadorEstructura.Validar(strContent);
                errores.Add(errorXmlValidacion.Valido ? 0 : 1003);
                if (!errorXmlValidacion.Valido)
                {
                    erroresPac = this.CrearArchivoROE(errores, archivoEntrada, errorXmlValidacion.ErrorList.ToString());
                    result = erroresPac;
                    return result;
                }
                string sello = (xe.Attribute("Sello") == null) ? "" : xe.Attribute("Sello").Value;
                string serieCert = (xe.Attribute("NumCert") == null) ? string.Empty : xe.Attribute("NumCert").Value;
                string fecha = (xe.Attribute("FechaExp") == null) ? "" : xe.Attribute("FechaExp").Value;
                DateTime fechaEmisionCfdi = Convert.ToDateTime(fecha);
                string rfc = (((XElement)xe.FirstNode).Attribute("RFCEmisor") == null) ? "" : ((XElement)xe.FirstNode).Attribute("RFCEmisor").Value;
                string cadenaOriginal;
                lock (this._lock)
                {
                    cadenaOriginal = ValidadorDatosRetencion._genCadenas.GenerarCadenaOriginal(strContent);
                }
                X509Certificate2 certificado = null;
                byte[] cert = (xe.Attribute("Cert") == null || string.IsNullOrEmpty(xe.Attribute("Cert").Value)) ? new byte[0] : Convert.FromBase64String(xe.Attribute("Cert").Value);
                if (cert.Length == 0)
                {
                    errores.Add(399);
                    erroresPac = this.CrearArchivoROE(errores, archivoEntrada, "");
                    result = erroresPac;
                    return result;
                }
                try
                {
                    certificado = new X509Certificate2(cert);
                }
                catch (Exception ex)
                {
                    NtLinkBusiness.Logger.Error(ex);
                    errores.Add(399);
                    erroresPac = this.CrearArchivoROE(errores, archivoEntrada, "");
                    result = erroresPac;
                    return result;
                }
                DateTime datFechaExpiracionCSD = Convert.ToDateTime(certificado.GetExpirationDateString());
                DateTime datFechaEfectivaCSD = Convert.ToDateTime(certificado.GetEffectiveDateString());
                ValidadorDatos validadorDatos = new ValidadorDatos();
                errores.Add(validadorDatos.ValidaCertificadoAc(certificado));
                DateTime fechaFiel = new DateTime(2015, 3, 4);
                if (fechaEmisionCfdi >= fechaFiel)
                {
                    errores.Add(validadorDatos.ValidaCertificadoCSDnoFIEL(certificado));
                }
                byte[] firma = null;
                try
                {
                    firma = Convert.FromBase64String(sello);
                }
                catch (Exception ee)
                {
                    NtLinkBusiness.Logger.Error(ee);
                    errores.Add(302);
                }
                if (firma != null)
                {
                    errores.Add(validadorDatos.ValidarSelloRetencion(cadenaOriginal, firma, certificado, ref hash));
                }
                NtLinkBusiness.Logger.Debug(hash);
                errores.Add(validadorDatos.ValidaRFCEmisor(rfc, certificado.SubjectName.Name));
                errores.Add(validadorDatos.VerificaCSDRevocado(serieCert, fecha));
                errores.Add(validadorDatos.ValidaFechaEmisionXml(fechaEmisionCfdi, datFechaExpiracionCSD, datFechaEfectivaCSD));
                string cfdiTimbrado = null;
                string uuidDuplicado = null;
                int duplicado = validadorDatos.ValidaTimbrePrevio(xe, hash, ref cfdiTimbrado, ref uuidDuplicado);
                errores.Add(duplicado);
                if (duplicado == 307)
                {
                    timbre = new ServicioLocal.Business.TimbreRetenciones.TimbreFiscalDigital
                    {
                        UUID = uuidDuplicado
                    };
                    res = cfdiTimbrado;
                    result = new Dictionary<int, string>();
                    return result;
                }
                errores.Add(validadorDatos.ValidaRangoFecha(fechaEmisionCfdi));
                errores.Add(validadorDatos.ValidaRFCLCO(rfc));
                errores.Add(validadorDatos.ValidaFechaEmision2011(fechaEmisionCfdi));
                IEnumerable<int> erroresReales = from l in errores
                                                 where l != 0
                                                 select l;
                if (erroresReales.Any<int>())
                {
                    erroresPac = this.CrearArchivoROE(erroresReales.ToList<int>(), archivoEntrada, "");
                    result = erroresPac;
                    return result;
                }
                GeneradorTimbreFiscalDigital genTimbre = new GeneradorTimbreFiscalDigital("SLOT", 666);
                string serieCertPac = ConfigurationManager.AppSettings["NoSerieCertPac"];
                string RFCPac = ConfigurationManager.AppSettings["RFCPac"];
                timbre = genTimbre.GeneraTimbreFiscalDigitalRetencionesCadenas(rfc, serieCertPac, sello, xe, uuid);
                if (timbre != null)
                {
                    string strTimbre = this.GetXmlTimbre(timbre);
                    string xmlCompleto = this.ConcatenaTimbreRet(xe, strTimbre, addenda, rfc, uuid.ToString());
                    res = xmlCompleto;
                    result = erroresPac;
                    return result;
                }
            }
            catch (Exception ex)
            {
                NtLinkBusiness.Logger.Error(ex);
                this.CrearArchivoROE(new List<int>
				{
					666
				}, archivoEntrada, ex.Message);
                throw;
            }
            result = erroresPac;
            return result;
        }

        private string ConcatenaTimbreRet(XElement entrada, string xmlFinal, XElement addenda, string rfc, string uuid)
        {
            XElement timbre = XElement.Load(new StringReader(xmlFinal));
            XElement complemento = entrada.Elements(Constantes.RetencionNamesPace + "Complemento").FirstOrDefault<XElement>();
            if (complemento == null)
            {
                entrada.Add(new XElement(Constantes.RetencionNamesPace + "Complemento"));
                complemento = entrada.Elements(Constantes.RetencionNamesPace + "Complemento").FirstOrDefault<XElement>();
            }
            complemento.Add(timbre);
            if (addenda != null)
            {
                entrada.Add(addenda);
            }
            SidetecStringWriter tw = new SidetecStringWriter(Encoding.UTF8);
            entrada.Save(tw, SaveOptions.DisableFormatting);
            return tw.ToString();
        }

        private string GetXmlTimbre(ServicioLocal.Business.TimbreRetenciones.TimbreFiscalDigital p)
        {
            XmlSerializer ser = new XmlSerializer(typeof(ServicioLocal.Business.TimbreRetenciones.TimbreFiscalDigital));
            string result;
            using (MemoryStream memStream = new MemoryStream())
            {
                StreamWriter sw = new StreamWriter(memStream, Encoding.UTF8);
                using (XmlWriter xmlWriter = XmlWriter.Create(sw, new XmlWriterSettings
                {
                    Indent = false,
                    Encoding = Encoding.UTF8
                }))
                {
                    XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
                    namespaces.Add("xsi", "http://www.w3.org/2001/XMLSchema-instance");
                    namespaces.Add("tfd", "http://www.sat.gob.mx/TimbreFiscalDigital");
                    ser.Serialize(xmlWriter, p, namespaces);
                    string xml = Encoding.UTF8.GetString(memStream.GetBuffer());
                    xml = xml.Substring(xml.IndexOf(Convert.ToChar(60)));
                    xml = xml.Substring(0, xml.LastIndexOf(Convert.ToChar(62)) + 1);
                    result = xml;
                }
            }
            return result;
        }

        private Dictionary<int, string> CrearArchivoROE(IEnumerable<int> errores, string archivoEntrada, string extraInfo = "")
        {
            Dictionary<int, string> resultado = new Dictionary<int, string>();
            StringBuilder errorOutput = new StringBuilder();
            errorOutput.AppendLine("Archivo Invalido");
            errorOutput.AppendLine("Path: " + archivoEntrada);
            foreach (int error in errores)
            {
                resultado.Add(error, Constantes.ErroresValidacion[error] + ((extraInfo == "") ? "" : (" - Extra: " + extraInfo)));
            }
            return resultado;
        }
    }
}
