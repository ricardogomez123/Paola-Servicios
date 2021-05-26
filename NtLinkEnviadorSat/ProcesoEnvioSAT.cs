using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using ServicioLocal.Business;
using ServicioLocalContract;
using log4net;
using CertificadorWs.Business;

using SAT.CFDI.Cliente.Procesamiento;
using System.Xml.Linq;
using CertificadorWs;
using CertificadorWs.Business.Retenciones;
using Acuse = SAT.CFDI.Cliente.Procesamiento.ServicioRecepcionCFDI.Acuse;
using Encabezado = SAT.CFDI.Cliente.Procesamiento.Encabezado;

namespace PACEnviadorSATConsole
{
    public class ProcesoEnvioSAT
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(ProcesoTimbre));
        private string _strMRutaValidacion;

        private void GuardarInformacion(TimbreWs33 topPComprobante, int idErrorSat, string strLAcuseReciboSat,
            AcuseRecepcionRetencion acuse)
        {
            try
            {
                NtLinkTimbrado t = new NtLinkTimbrado();

                //si por falta de conexión no actualizo//
                if (idErrorSat == 103)
                    return;

                topPComprobante.Acuse = strLAcuseReciboSat;
                topPComprobante.FechaEnvio = Convert.ToDateTime(acuse.fecha);

                if (idErrorSat != 0)
                {
                    topPComprobante.Error = idErrorSat;
                    topPComprobante.StrError = acuse.listaIncidencia[0].MensajeIncidencia;
                    topPComprobante.Status = 90;
                }
                else
                    topPComprobante.Status = 1;

                t.GuardarTimbre(topPComprobante);
            }
            catch (Exception ex)
            {
                Log.Error("Error al intentar salvar la información, Err:" + ex.ToString());
            }
        }

        private void GuardarInformacion(TimbreWs33 topPComprobante, int idErrorSat, string strLAcuseReciboSat,
            Acuse acuseReciboSAT)
        {
            try
            {
                NtLinkTimbrado t = new NtLinkTimbrado();

                //si por falta de conexión no actualizo//
                if (idErrorSat == 103 || idErrorSat == 502 || idErrorSat == 501)
                    return;

                topPComprobante.Acuse = strLAcuseReciboSat;
                topPComprobante.FechaEnvio = acuseReciboSAT.Fecha;

                if (idErrorSat != 0)
                {
                    topPComprobante.Error = idErrorSat;
                    topPComprobante.StrError = acuseReciboSAT.Incidencia[0].MensajeIncidencia;
                    topPComprobante.Status = 90;
                }
                else
                    topPComprobante.Status = 1;

                t.GuardarTimbre(topPComprobante);
            }
            catch (Exception ex)
            {
                Log.Error("Error al intentar salvar la información, Err:" + ex.ToString());
            }
        }

        private EnviadorRetenciones ret = new EnviadorRetenciones();
        public bool EnvioSatRet(TimbreWs33 topPComprobante)
        {
            try
            {
                GeneradorCadenasCfd gen = new GeneradorCadenasCfd();
                string strLAcuseReciboSAT;
                byte[] result;
                int idErrorSAT;
                Encabezado encLMetadata;
                SHA1 sha = new SHA1CryptoServiceProvider();
                string strComprobante = null;
                if (string.IsNullOrEmpty(topPComprobante.Xml))
                {
                    var directorio = Path.Combine(ConfigurationManager.AppSettings["RutaTimbrado"],
                    topPComprobante.RfcEmisor, topPComprobante.FechaFactura.ToString("yyyyMMdd"));
                    if (!Directory.Exists(directorio))
                        Directory.CreateDirectory(directorio);
                    var fileName = Path.Combine(directorio, "Comprobante_" + topPComprobante.Uuid + ".xml");
                    strComprobante = File.ReadAllText(fileName, Encoding.UTF8);
                }
                else
                    strComprobante = topPComprobante.Xml;
                Retenciones comprobante = TimbradoUtils.DesSerializarRetenciones(strComprobante);
                //byte[] bytLCadenaOriginal = Encoding.UTF8.GetBytes(gen.CadenaOriginal(strComprobante));
                //result = sha.ComputeHash(bytLCadenaOriginal);
                string strLRfcEmisor = comprobante.Emisor.RFCEmisor;
                string strLHash = topPComprobante.Hash;
                //Quitar addenda si tiene
                XElement xe = XElement.Load(new StringReader(strComprobante));
                XElement addenda = xe.Elements(Constantes.CFDVersionNamespace + "Addenda").FirstOrDefault();

                if (addenda != null)
                {
                    addenda.Remove();
                }
                var tw = new SidetecStringWriter(Encoding.UTF8);
                xe.Save(tw, SaveOptions.DisableFormatting);
                Log.Info("Enviando CFDI al SAT.");
                Log.Info("Se enviará el comprobante con el identificador: " + topPComprobante.IdTimbre);

                var acuseReciboSAT = ret.EnviarRetencion(strComprobante,topPComprobante.Uuid);
                if (acuseReciboSAT != null)
                {
                    idErrorSAT = acuseReciboSAT.codEstatus.Equals("Comprobante Rechazado",
                                                                  StringComparison.InvariantCultureIgnoreCase)
                                     ? Convert.ToInt32(acuseReciboSAT.listaIncidencia[0].CodigoError)
                                     : 0;
                    var acuseStream = new MemoryStream();
                    var xmlSerializer = new XmlSerializer(typeof(CertificadorWs.Business.Retenciones.AcuseRecepcionRetencion));
                    xmlSerializer.Serialize(acuseStream, acuseReciboSAT);
                    acuseStream.Seek(0, SeekOrigin.Begin);
                    var acuseReader = new StreamReader(acuseStream);
                    strLAcuseReciboSAT = acuseReader.ReadToEnd();
                    var directorio = Path.Combine(ConfigurationManager.AppSettings["RutaTimbrado"],
                    topPComprobante.RfcEmisor, topPComprobante.FechaFactura.ToString("yyyyMMdd"));
                    if (!Directory.Exists(directorio))
                        Directory.CreateDirectory(directorio);
                    var fileName = Path.Combine(directorio, "Acuse_" + topPComprobante.Uuid + ".xml");
                    File.WriteAllText(fileName, strLAcuseReciboSAT);
                }
                else
                {
                    idErrorSAT = 103;
                    strLAcuseReciboSAT = "";
                }
                //idErrorSAT = TraerEstatusSAT(strLAcuseReciboSAT);

                Log.Info("Código de retorno SAT: " + idErrorSAT);
                GuardarInformacion(topPComprobante, idErrorSAT, strLAcuseReciboSAT, acuseReciboSAT);

                return true;

            }
            catch (Exception ex)
            {
                Log.Error("Error al intentar enviar el CFD al SAT, Err:" + ex.ToString());
                return false;
            }
        }



        public bool EnvioSAT(TimbreWs33 topPComprobante)
        {
            try
            {
                GeneradorCadenasCfd gen = new GeneradorCadenasCfd();
                string strLAcuseReciboSAT;
                byte[] result;
                int idErrorSAT;
                Encabezado encLMetadata;
                //SHA1 sha = new SHA1CryptoServiceProvider();
                string strComprobante = null;
                if (string.IsNullOrEmpty(topPComprobante.Xml))
                {
                    var directorio = Path.Combine(ConfigurationManager.AppSettings["RutaTimbrado"],
                    topPComprobante.RfcEmisor, topPComprobante.FechaFactura.ToString("yyyyMMdd"));
                    if (!Directory.Exists(directorio))
                        Directory.CreateDirectory(directorio);
                    var fileName = Path.Combine(directorio, "Comprobante_" + topPComprobante.Uuid + ".xml");
                    strComprobante = File.ReadAllText(fileName, Encoding.UTF8);
                }
                else
                    strComprobante = topPComprobante.Xml;

                Comprobante comprobante = GeneradorCfdi.GetComprobanteFromString(strComprobante);
                //byte[] bytLCadenaOriginal = Encoding.UTF8.GetBytes(gen.CadenaOriginal(strComprobante));
                //result = sha.ComputeHash(bytLCadenaOriginal);
                string strLRfcEmisor = comprobante.Emisor.Rfc;
              //  string strLHash = topPComprobante.Hash;
              //  string version = comprobante.Version;
                string version = "3.3";
             
                //string strLCertificadoSAT = comprobante.Complemento.timbreFiscalDigital.NoCertificadoSAT;
                string strLCertificadoSAT = ConfigurationManager.AppSettings["NoCertificadoPac"];

                string strLUUID = comprobante.Complemento.timbreFiscalDigital.UUID;
                DateTime datLFechaTimbrado =  comprobante.Complemento.timbreFiscalDigital.FechaTimbrado;
                string strLPathArchivo = AppDomain.CurrentDomain.BaseDirectory + 
                                         ConfigurationManager.AppSettings["PathXMLTemporales"] + Guid.NewGuid() + ".xml";
                //Quitar addenda si tiene
                XElement xe = XElement.Load(new StringReader(strComprobante));
                XElement addenda = xe.Elements(Constantes.CFDVersionNamespace + "Addenda").FirstOrDefault();

                if (addenda != null)
                {
                    addenda.Remove();
                }
                var tw = new SidetecStringWriter(Encoding.UTF8);
                xe.Save(tw, SaveOptions.DisableFormatting);
                string xml = tw.ToString();
                //escribimos el archivo en una carpeta para que el proceso de envio SAT lo tome//
                File.WriteAllText(strLPathArchivo, xml, Encoding.UTF8);
                //armamos el encabezado//
                encLMetadata = new Encabezado(strLRfcEmisor, version, strLCertificadoSAT, strLUUID, datLFechaTimbrado, strLPathArchivo);

                Log.Info("Enviando CFDI al SAT.");
                Log.Info("Se enviará el comprobante con el identificador: " + topPComprobante.IdTimbre);
                Log.Info("Encabezado." + encLMetadata.version + "|" + encLMetadata.UUID + "|" + encLMetadata.NumeroCertificado + "|" + encLMetadata.RfcEmisor + "|"+encLMetadata.Xml);
                
                var acuseReciboSAT = EnviarCFDIalSAT(encLMetadata);
                
                if (acuseReciboSAT != null)
                {
                    idErrorSAT = acuseReciboSAT.CodEstatus.Equals("Comprobante Rechazado",
                                                                  StringComparison.InvariantCultureIgnoreCase)
                                     ? Convert.ToInt32(acuseReciboSAT.Incidencia[0].CodigoError)
                                     : 0;
                    if (idErrorSAT != 501 || idErrorSAT != 502)
                    {


                    }
                    var acuseStream = new MemoryStream();
                    var xmlSerializer = new XmlSerializer(typeof(SAT.CFDI.Cliente.Procesamiento.ServicioRecepcionCFDI.Acuse));
                    xmlSerializer.Serialize(acuseStream, acuseReciboSAT);
                    acuseStream.Seek(0, SeekOrigin.Begin);
                    var acuseReader = new StreamReader(acuseStream);
                    strLAcuseReciboSAT = acuseReader.ReadToEnd();
                    var directorio = Path.Combine(ConfigurationManager.AppSettings["RutaTimbrado"],
                    topPComprobante.RfcEmisor, topPComprobante.FechaFactura.ToString("yyyyMMdd"));
                    if (!Directory.Exists(directorio))
                        Directory.CreateDirectory(directorio);
                    var fileName = Path.Combine(directorio, "Acuse_" + topPComprobante.Uuid + ".xml");
                    File.WriteAllText(fileName, strLAcuseReciboSAT);
                }
                else
                {
                    idErrorSAT = 103;
                    strLAcuseReciboSAT = "";
                }
                //idErrorSAT = TraerEstatusSAT(strLAcuseReciboSAT);

                Log.Info("Código de retorno SAT: " + idErrorSAT);
                GuardarInformacion(topPComprobante, idErrorSAT, strLAcuseReciboSAT, acuseReciboSAT);

                File.Delete(strLPathArchivo);

                return true;

            }
            catch (Exception ex)
            {
                Log.Error("Error al intentar enviar el CFD al SAT, Err:" + ex.ToString());
                return false;
            }
        }
        
        private int TraerEstatusSAT(string strPAcuse)
        {
            try
            {
                if (strPAcuse == "")
                    return 103;
                string strLEstatus = "";
                XmlDocument xmlLDoc = new XmlDocument();
                xmlLDoc.LoadXml(strPAcuse);
                XmlNode xmlLNode = xmlLDoc.DocumentElement;
                strLEstatus = xmlLNode.Attributes["CodEstatus"].Value;//N - 503

                Log.Debug("Cadena CodEstatus: " + strLEstatus);
                if (!strLEstatus.StartsWith("S"))
                {
                    if (!strLEstatus.Contains("Comprobante recibido satisfactoriamente"))
                    {
                        var codigoEstatus = strLEstatus.Substring(4, 3);
                        int intCodigoEstatus;
                        if (int.TryParse(codigoEstatus, out intCodigoEstatus))
                            return intCodigoEstatus;
                        return 109;
                    }
                }
                return 0;
            }
            catch (Exception err)
            {
                Log.Error("(TraerEstatusSAT) Error al intentar parsear el XML de respuesta SAT, Err:" + err.ToString() + "xml sat:" + strPAcuse);
                return 103;
            }
        }

        private Acuse EnviarCFDIalSAT(Encabezado encLMetadata)
        {
            string strMAcuseRecibo = "";
            try
            {
                AccesoServicios accesoServicios = new AccesoServicios();
                accesoServicios.ClienteAutenticacion.Endpoint.Address = new System.ServiceModel.EndpointAddress(ConfigurationManager.AppSettings["RutaSATAutenticacion"]);
                accesoServicios.ClienteRecepcion.Endpoint.Address = new System.ServiceModel.EndpointAddress(ConfigurationManager.AppSettings["RutaSATRecepcion"]);
               
                var acuse = accesoServicios.EnviarBloqueCfdi(encLMetadata);
                return acuse;
            }
            catch (Exception err)
            {
                Log.Error("(EnviarCFDIalSAT) Error al intentar enviar la información al SAT, Err:" + err.ToString() + "\r\nAcuse: " + strMAcuseRecibo);
                return null;
            }
        }
    }
}
