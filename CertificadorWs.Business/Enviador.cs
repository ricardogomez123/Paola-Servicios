using System;
using System.Configuration;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using SAT.CFDI.Cliente.Procesamiento.ServicioRecepcionCFDI;
using ServicioLocal.Business;
using ServicioLocalContract;
using log4net;
using SAT.CFDI.Cliente.Procesamiento;

namespace CertificadorWs.Business
{
    class Enviador
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(Enviador));
        private string _strMRutaValidacion;

        private void GuardarInformacion(TimbreWs33 comprobante, int idErrorSat, string strLAcuseReciboSat, Acuse acuseReciboSAT)
        {
            try
            {
                NtLinkTimbrado t = new NtLinkTimbrado();

                //si por falta de conexión no actualizo//
                if (idErrorSat == 103)
                    return;

                comprobante.Acuse = strLAcuseReciboSat;
                comprobante.FechaEnvio = acuseReciboSAT.Fecha;

                if (idErrorSat != 0)
                {
                    comprobante.Error = idErrorSat;
                    comprobante.StrError = acuseReciboSAT.Incidencia[0].MensajeIncidencia;
                }
                else
                    comprobante.Status = 1;

                t.GuardarTimbre(comprobante);
            }
            catch (Exception ex)
            {
                Log.Error("Error al intentar salvar la información, Err:" + ex.ToString());
            }
        }

        public bool EnvioSAT(Comprobante comprobante, TimbreWs33 topPComprobante, string cadenaOriginal)
        {
            try
            {
                string strLAcuseReciboSAT;
                byte[] result;
                int idErrorSAT;
                Encabezado encLMetadata;
                SHA1 sha = new SHA1CryptoServiceProvider();
                byte[] bytLCadenaOriginal = Encoding.UTF8.GetBytes(cadenaOriginal);
                result = sha.ComputeHash(bytLCadenaOriginal);
                string strLRfcEmisor = comprobante.Emisor.Rfc;
                string strLHash = BitConverter.ToString(result).Replace("-", string.Empty);
               // string version = comprobante.Version;
                string version = "3.3";
               // string strLCertificadoSAT = comprobante.Complemento.timbreFiscalDigital.NoCertificadoSAT;
                string strLCertificadoSAT = ConfigurationManager.AppSettings["NoCertificadoPac"];

                string strLUUID = comprobante.Complemento.timbreFiscalDigital.UUID;
                DateTime datLFechaTimbrado = comprobante.Complemento.timbreFiscalDigital.FechaTimbrado;
                string strLPathArchivo = ConfigurationManager.AppSettings["PathXMLTemporales"] + strLUUID + ".xml";

                //escribimos el archivo en una carpeta para que el proceso de envio SAT lo tome//
                File.WriteAllText(strLPathArchivo, topPComprobante.Xml, Encoding.UTF8);
                //armamos el encabezado//
                encLMetadata = new Encabezado(strLRfcEmisor, version, strLCertificadoSAT, strLUUID, datLFechaTimbrado, strLPathArchivo);

                Log.Info("Enviando CFDI al SAT.");
                Log.Info("Se enviará el comprobante con el identificador: " + topPComprobante.Uuid);

                var acuseReciboSAT = EnviarCFDIalSAT(encLMetadata);
                if (acuseReciboSAT != null)
                {
                    idErrorSAT = acuseReciboSAT.CodEstatus.Equals("Comprobante Rechazado",
                                                                  StringComparison.InvariantCultureIgnoreCase)
                                     ? Convert.ToInt32(acuseReciboSAT.Incidencia[0].CodigoError)
                                     : 0;
                    var acuseStream = new MemoryStream();
                    var xmlSerializer = new XmlSerializer(typeof(SAT.CFDI.Cliente.Procesamiento.ServicioRecepcionCFDI.Acuse));
                    xmlSerializer.Serialize(acuseStream, acuseReciboSAT);
                    acuseStream.Seek(0, SeekOrigin.Begin);
                    var acuseReader = new StreamReader(acuseStream);
                    strLAcuseReciboSAT = acuseReader.ReadToEnd();
                }
                else
                {
                    idErrorSAT = 103;
                    strLAcuseReciboSAT = "";
                }
                //idErrorSAT = TraerEstatusSAT(strLAcuseReciboSAT);

                Log.Info("Código de retorno SAT: " + idErrorSAT);
                GuardarInformacion(topPComprobante, idErrorSAT, strLAcuseReciboSAT, acuseReciboSAT);

                //File.Delete(strLPathArchivo);

                return true;

            }
            catch (Exception ex)
            {
                Log.Error("Error al intentar enviar el CFD al SAT, Err:" + ex.ToString());
                return false;
            }
        }

      
        private Acuse EnviarCFDIalSAT(Encabezado encLMetadata)
        {
            string strMAcuseRecibo = "";
            try
            {
                AccesoServicios accesoServicios = new AccesoServicios();
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
