using CertificadorWs.Business.Retenciones;
using log4net;
using log4net.Config;
using SAT.CFDI.Cliente.Procesamiento;
using SAT.CFDI.Cliente.Procesamiento.ServicioAceptacionRechazo;
using SAT.CFDI.Cliente.Procesamiento.ServicioCancelacionCFDI;
using SAT.CFDI.Cliente.Procesamiento.ServicioRelacionados;
using ServicioLocal.Business;
using ServicioLocalContract;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.ServiceModel;
using System.Text;
using System.Xml.Serialization;

namespace CertificadorWs.Business
{
    public class Cancelador
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(Cancelador));

        public Cancelador()
        {
            XmlConfigurator.Configure();
        }

        public string ConsultaEstatusCFDI(string expresion)
        {
            AccesoServicios serv = new AccesoServicios();
            return serv.ConsultaEstatusCFDI(expresion);
        }

        public string ConsultaCFDI(string expresion, string uudi, string rfcReceptor)//no necesita asignature
        {

            AccesoServicios serv = new AccesoServicios();
            SAT.CFDI.Cliente.Procesamiento.ServicioRelacionados.SignatureType asignature = new SAT.CFDI.Cliente.Procesamiento.ServicioRelacionados.SignatureType();
            return serv.ConsultaCFDI(expresion, uudi, rfcReceptor, asignature);
        }

        public string ConsultaAceptacionRechazo(string RfcReceptor)
        {
            AccesoServicios ser = new AccesoServicios();
            return ser.ConsultaAceptacionRechazo(RfcReceptor);
        }

        public string ProcesarRespuestaAceptacionRechazo(string RfcReceptor, string RfcPacEnviaSolicitud, List<Folios> F)
        {
            AccesoServicios ser = new AccesoServicios();
            IList uuidsCancelar = new List<string>();
            foreach (Folios f in F)
            {
                uuidsCancelar.Add(f.UUDI.ToUpper());
            }
            List<SolicitudAceptacionRechazoFolios> Folio = new List<SolicitudAceptacionRechazoFolios>();
            foreach (Folios f in F)
            {
                SolicitudAceptacionRechazoFolios x = new SolicitudAceptacionRechazoFolios();
                x.UUID = f.UUDI;
                if (f.Respuesta.ToString() == "Aceptacion")
                {
                    x.Respuesta = TipoAccionPeticionCancelacion.Aceptacion;
                }
                else
                {
                    x.Respuesta = TipoAccionPeticionCancelacion.Rechazo;
                }
                Folio.Add(x);
            }
            string result;
            using (new NtLinkLocalServiceEntities())
            {
                NtLinkEmpresa nle = new NtLinkEmpresa();
                empresa empresa = nle.GetByRfc(RfcReceptor);
                string fecha = this.FechaHoy();
                SAT.CFDI.Cliente.Procesamiento.Encabezado encLMetadata2 = new SAT.CFDI.Cliente.Procesamiento.Encabezado(RfcReceptor, fecha, uuidsCancelar);
                string path = Path.Combine(ConfigurationManager.AppSettings["Resources"], RfcReceptor);
                string pathCer = Path.Combine(path, "Certs", "csd.cer");
                string pathKey = Path.Combine(path, "Certs", "csd.key");
                string pass = empresa.PassKey;
                SAT.CFDI.Cliente.Procesamiento.ServicioAceptacionRechazo.SignatureType asignature = ser.AsignatureProceso(pathKey, encLMetadata2, pass, pathCer);
                result = ser.ProcesarRespuestaAceptacionRechazo(RfcReceptor, fecha, RfcPacEnviaSolicitud, Folio, asignature);
            }
            return result;
        }

        public string ConsultaCFDIRelacionadosRequest(string RfcPacEnviaSolicitud, string RfcReceptor,string RfcEmisor, string Uuid)
        {
            AccesoServicios ser = new AccesoServicios();
            IList uuidsCancelar = new List<string>();
            uuidsCancelar.Add(Uuid.ToUpper());
            string result;
            using (new NtLinkLocalServiceEntities())
            { string path ="";
                NtLinkEmpresa nle = new NtLinkEmpresa();
                if (!string.IsNullOrEmpty(RfcEmisor))
                {
                    empresa empresa = nle.GetByRfc(RfcEmisor);
                    SAT.CFDI.Cliente.Procesamiento.Encabezado encLMetadata2 = new SAT.CFDI.Cliente.Procesamiento.Encabezado(RfcEmisor, this.FechaHoy(), uuidsCancelar);
                    path = Path.Combine(ConfigurationManager.AppSettings["Resources"], RfcEmisor);
                    string pathCer = Path.Combine(path, "Certs", "csd.cer");
                    string pathKey = Path.Combine(path, "Certs", "csd.key");
                    string pass = empresa.PassKey;
                    SAT.CFDI.Cliente.Procesamiento.ServicioRelacionados.SignatureType asignature = ser.Asignature(pathKey, encLMetadata2, pass, pathCer);
                    result = ser.ConsultaCFDIRelacionadosRequest(RfcPacEnviaSolicitud, RfcReceptor, RfcEmisor, Uuid, asignature);
                }
                else
                {
                empresa empresa = nle.GetByRfc(RfcReceptor);
                SAT.CFDI.Cliente.Procesamiento.Encabezado encLMetadata2 = new SAT.CFDI.Cliente.Procesamiento.Encabezado(RfcReceptor, this.FechaHoy(), uuidsCancelar);
                path = Path.Combine(ConfigurationManager.AppSettings["Resources"], RfcReceptor);
                string pathCer = Path.Combine(path, "Certs", "csd.cer");
                string pathKey = Path.Combine(path, "Certs", "csd.key");
                string pass = empresa.PassKey;
                SAT.CFDI.Cliente.Procesamiento.ServicioRelacionados.SignatureType asignature = ser.Asignature(pathKey, encLMetadata2, pass, pathCer);
                result = ser.ConsultaCFDIRelacionadosRequest(RfcPacEnviaSolicitud, RfcReceptor,RfcEmisor, Uuid, asignature);
      
                }
             }
            return result;
        }

        public RespuestaCancelacion Cancelar(string requestCancelacion)
        {
            Cancelador.Logger.Info(requestCancelacion);
            AccesoServicios serv = new AccesoServicios();
            XmlSerializer ser = new XmlSerializer(typeof(SAT.CFDI.Cliente.Procesamiento.ServicioCancelacionCFDI.Cancelacion), "http://cancelacfd.sat.gob.mx");
            SAT.CFDI.Cliente.Procesamiento.ServicioCancelacionCFDI.Cancelacion cancelacion = null;
            RespuestaCancelacion result;
            try
            {
                cancelacion = (ser.Deserialize(new StringReader(requestCancelacion)) as SAT.CFDI.Cliente.Procesamiento.ServicioCancelacionCFDI.Cancelacion);
                NtLinkTimbrado nlt = new NtLinkTimbrado();
            }
            catch (SerializationException ee)
            {
                Cancelador.Logger.Error(ee);
                result = new RespuestaCancelacion
                {
                    Acuse = null,
                    MensajeError = "Request mal formado " + ee.Message,
                    StatusUuids = new List<StatusUuid>()
                };
                return result;
            }
            catch (Exception ee2)
            {
                Cancelador.Logger.Error(ee2);
                result = new RespuestaCancelacion
                {
                    Acuse = null,
                    MensajeError = "Request mal formado ",
                    StatusUuids = new List<StatusUuid>()
                };
                return result;
            }
            SAT.CFDI.Cliente.Procesamiento.ServicioCancelacionCFDI.Acuse respuesta = serv.CancelaCfdi(cancelacion);
            List<StatusUuid> res = new List<StatusUuid>();
            MemoryStream acuseStream = new MemoryStream();
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(SAT.CFDI.Cliente.Procesamiento.ServicioCancelacionCFDI.Acuse));
            xmlSerializer.Serialize(acuseStream, respuesta);
            acuseStream.Seek(0L, SeekOrigin.Begin);
            StreamReader acuseReader = new StreamReader(acuseStream);
            string acuse = acuseReader.ReadToEnd();
            Cancelador.Logger.Info(acuse);
            if (respuesta.Folios != null && respuesta.Folios.Length > 0)
            {
                SAT.CFDI.Cliente.Procesamiento.ServicioCancelacionCFDI.AcuseFolios[] folios = respuesta.Folios;
                for (int i = 0; i < folios.Length; i++)
                {
                    SAT.CFDI.Cliente.Procesamiento.ServicioCancelacionCFDI.AcuseFolios acuseFoliose = folios[i];
                    NtLinkTimbrado tim = new NtLinkTimbrado();
                    TimbreWs33 timbre = tim.ObtenerTimbre(acuseFoliose.UUID);
                    if (timbre == null)
                    {
                        TimbreWsHistorico timbreHist = tim.ObtenerTimbreHist(acuseFoliose.UUID);
                        if (timbreHist != null)
                        {
                            timbre = new TimbreWs33();
                            timbre.RfcEmisor = timbreHist.RfcEmisor;
                            timbre.RfcReceptor = timbreHist.RfcReceptor;
                            timbre.AcuseCancelacion = timbreHist.AcuseCancelacion;
                            timbre.IdTimbre = timbreHist.IdTimbre;
                            timbre.Uuid = timbreHist.Uuid;
                            timbre.StrError = "Hist";
                        }
                    }
                    if (timbre != null)
                    {
                        if (acuseFoliose.EstatusUUID == "201" || acuseFoliose.EstatusUUID == "202")
                        {
                            timbre.Status = new int?(2);
                        }
                        timbre.AcuseCancelacion = acuse;
                        tim.GuardarTimbre(timbre);
                        res.Add(new StatusUuid
                        {
                            Uuid = acuseFoliose.UUID,
                            Status = acuseFoliose.EstatusUUID
                        });
                    }
                }
            }
            RespuestaCancelacion resultado = new RespuestaCancelacion
            {
                Acuse = acuse,
                StatusUuids = res
            };
            result = resultado;
            return result;
        }

        public int CancelarOtrosPACs(string uuid, string rfc, ref string respuesta, ref string acuse, string Base64Cer, string Base64Key, string PasswordKey)
        {
            int result;
            try
            {
                Cancelador.Logger.Info("Cancelando comprobante: " + uuid);
                IList uuidsCancelar = new List<string>();
                uuidsCancelar.Add(uuid.ToUpper());
                using (new NtLinkLocalServiceEntities())
                {
                       NtLinkEmpresa nle = new NtLinkEmpresa();
                    
                                                      
                                string pass = PasswordKey;
                                SAT.CFDI.Cliente.Procesamiento.Encabezado encLMetadata2 = new SAT.CFDI.Cliente.Procesamiento.Encabezado(rfc, this.FechaHoy(), uuidsCancelar);
                                SAT.CFDI.Cliente.Procesamiento.ServicioCancelacionCFDI.Acuse ac2 = null;
                                acuse = this.EnviarCancelacionSATOtrosPACs(encLMetadata2, Base64Key, pass, Base64Cer, ref ac2);
                                if (string.IsNullOrEmpty(acuse))
                                {
                                    acuse = "No se pudo conectar al servicio de cancelación del SAT";
                                }
                                int intEstatus2 = this.TraerEstatusSAT(uuid, ac2);
                                if (intEstatus2 != 201 && intEstatus2 != 202)
                                {
                                    Cancelador.Logger.Error(acuse);
                                    respuesta = intEstatus2 + " - " + Constantes.ErroresValidacion[intEstatus2];
                                    throw new FaultException(respuesta);
                                }
                                Cancelador.Logger.Info(acuse);
                                string directorio = Path.Combine(ConfigurationManager.AppSettings["RutaTimbrado"], ac2.RfcEmisor, ac2.Fecha.ToString("yyyyMMdd"));
                                if (!Directory.Exists(directorio))
                                {
                                    Directory.CreateDirectory(directorio);
                                }
                                string fileName = Path.Combine(directorio, "Cancelacion_" + uuid.ToString() + ".xml");
                                File.WriteAllText(fileName, acuse, Encoding.UTF8);
                                respuesta = intEstatus2 + " - " + Constantes.ErroresValidacion[intEstatus2];
                                result = intEstatus2;
                                return result;
                           
                }
            }
            catch (Exception ex)
            {
                Cancelador.Logger.Error("(cancelar) Error: " + ex.Message + ((ex.InnerException == null) ? "" : ("\nExcepción Interna:" + ex.InnerException.Message)));
                throw;
            }
           
        }


         public int CancelarPorID(string uuid, int idempresa, ref string respuesta, ref string acuse)
        {
            int result;
            try
            {
                Cancelador.Logger.Info("Cancelando comprobante: " + uuid);
                IList uuidsCancelar = new List<string>();
                uuidsCancelar.Add(uuid.ToUpper());
                using (new NtLinkLocalServiceEntities())
                {
                    NtLinkEmpresa nle = new NtLinkEmpresa();
                   // empresa empresa = nle.GetByRfc(rfc);
                     empresa empresa = nle.GetById(idempresa);
                    if (empresa == null)
                    {
                        respuesta = "300 - El usuario con el que se quiere conectar es inválido";
                        acuse = "";
                        result = 300;
                    }
                    else
                    {
                        NtLinkTimbrado tim = new NtLinkTimbrado();
                        TimbreWs33 timbre = tim.ObtenerTimbre(uuid);
                        if (timbre == null)
                        {
                            TimbreWsHistorico timbreHist = tim.ObtenerTimbreHist(uuid);
                            if (timbreHist == null)
                            {
                                string path = Path.Combine(ConfigurationManager.AppSettings["Resources"], empresa.RFC);
                                string pathCer = Path.Combine(path, "Certs", "csd.cer");
                                string pathKey = Path.Combine(path, "Certs", "csd.key");
                                string pass = empresa.PassKey;
                                SAT.CFDI.Cliente.Procesamiento.Encabezado encLMetadata2 = new SAT.CFDI.Cliente.Procesamiento.Encabezado(empresa.RFC, this.FechaHoy(), uuidsCancelar);
                                SAT.CFDI.Cliente.Procesamiento.ServicioCancelacionCFDI.Acuse ac2 = null;
                                acuse = this.EnviarCancelacionSAT(encLMetadata2, pathKey, pass, pathCer, ref ac2);
                                if (string.IsNullOrEmpty(acuse))
                                {
                                    acuse = "No se pudo conectar al servicio de cancelación del SAT";
                                }
                                int intEstatus2 = this.TraerEstatusSAT(uuid, ac2);
                                if (intEstatus2 != 201 && intEstatus2 != 202)
                                {
                                    Cancelador.Logger.Error(acuse);
                                    respuesta = intEstatus2 + " - " + Constantes.ErroresValidacion[intEstatus2];
                                    throw new FaultException(respuesta);
                                }
                                Cancelador.Logger.Info(acuse);
                                string directorio = Path.Combine(ConfigurationManager.AppSettings["RutaTimbrado"], ac2.RfcEmisor, ac2.Fecha.ToString("yyyyMMdd"));
                                if (!Directory.Exists(directorio))
                                {
                                    Directory.CreateDirectory(directorio);
                                }
                                string fileName = Path.Combine(directorio, "Cancelacion_" + uuid.ToString() + ".xml");
                                File.WriteAllText(fileName, acuse, Encoding.UTF8);
                                respuesta = intEstatus2 + " - " + Constantes.ErroresValidacion[intEstatus2];
                                result = intEstatus2;
                                return result;
                            }
                            else
                            {
                                timbre = new TimbreWs33();
                                timbre.RfcEmisor = timbreHist.RfcEmisor;
                                timbre.RfcReceptor = timbreHist.RfcReceptor;
                                timbre.AcuseCancelacion = timbreHist.AcuseCancelacion;
                                timbre.IdTimbre = timbreHist.IdTimbre;
                                timbre.Uuid = timbreHist.Uuid;
                                timbre.StrError = "Hist";
                            }
                        }
                        if (timbre.RfcEmisor != empresa.RFC)
                        {
                            respuesta = "203 - UUID No corresponde el RFC del emisor y de quien solicita la cancelación.";
                            acuse = "";
                            result = 203;
                        }
                        else if (timbre.AcuseCancelacion != null)
                        {
                            respuesta = "202 - UUID Previamente cancelado";
                            acuse = timbre.AcuseCancelacion;
                            result = 202;
                        }
                        else
                        {
                            string path = Path.Combine(ConfigurationManager.AppSettings["Resources"], empresa.RFC);
                            string pathCer = Path.Combine(path, "Certs", "csd.cer");
                            string pathKey = Path.Combine(path, "Certs", "csd.key");
                            string pass = empresa.PassKey;
                            SAT.CFDI.Cliente.Procesamiento.Encabezado encLMetadata3 = new SAT.CFDI.Cliente.Procesamiento.Encabezado(empresa.RFC, this.FechaHoy(), uuidsCancelar);
                            SAT.CFDI.Cliente.Procesamiento.ServicioCancelacionCFDI.Acuse ac3 = null;
                            acuse = this.EnviarCancelacionSAT(encLMetadata3, pathKey, pass, pathCer, ref ac3);
                            if (string.IsNullOrEmpty(acuse))
                            {
                                acuse = "No se pudo conectar al servicio de cancelación del SAT";
                            }
                            int intEstatus3 = this.TraerEstatusSAT(uuid, ac3);
                            if (intEstatus3 != 201 && intEstatus3 != 202)
                            {
                                Cancelador.Logger.Error(acuse);
                                respuesta = intEstatus3 + " - " + Constantes.ErroresValidacion[intEstatus3];
                                throw new FaultException(respuesta);
                            }
                            Cancelador.Logger.Info(acuse);
                            timbre.Status = new int?(2);
                            string directorio = Path.Combine(ConfigurationManager.AppSettings["RutaTimbrado"], timbre.RfcEmisor, timbre.FechaFactura.ToString("yyyyMMdd"));
                            if (!Directory.Exists(directorio))
                            {
                                Directory.CreateDirectory(directorio);
                            }
                            string fileName = Path.Combine(directorio, "Cancelacion_" + timbre.Uuid.ToString() + ".xml");
                            File.WriteAllText(fileName, acuse, Encoding.UTF8);
                            tim.GuardarTimbre(timbre);
                            respuesta = intEstatus3 + " - " + Constantes.ErroresValidacion[intEstatus3];
                            result = intEstatus3;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Cancelador.Logger.Error("(cancelar) Error: " + ex.Message + ((ex.InnerException == null) ? "" : ("\nExcepción Interna:" + ex.InnerException.Message)));
                throw;
            }
            return result;
        }



        public int Cancelar(string uuid, string rfc, ref string respuesta, ref string acuse)
      //  public int Cancelar(string uuid, int idempresa, ref string respuesta, ref string acuse)
        {
            int result;
            try
            {
                Cancelador.Logger.Info("Cancelando comprobante: " + uuid);
                IList uuidsCancelar = new List<string>();
                uuidsCancelar.Add(uuid.ToUpper());
                using (new NtLinkLocalServiceEntities())
                {
                    NtLinkEmpresa nle = new NtLinkEmpresa();
                    empresa empresa = nle.GetByRfc(rfc);
                   // empresa empresa = nle.GetById(idempresa);
                    if (empresa == null)
                    {
                        respuesta = "300 - El usuario con el que se quiere conectar es inválido";
                        acuse = "";
                        result = 300;
                    }
                    else
                    {
                        NtLinkTimbrado tim = new NtLinkTimbrado();
                        TimbreWs33 timbre = tim.ObtenerTimbre(uuid);
                        if (timbre == null)
                        {
                            TimbreWsHistorico timbreHist = tim.ObtenerTimbreHist(uuid);
                            if (timbreHist == null)
                            {
                                string path = Path.Combine(ConfigurationManager.AppSettings["Resources"], empresa.RFC);
                                string pathCer = Path.Combine(path, "Certs", "csd.cer");
                                string pathKey = Path.Combine(path, "Certs", "csd.key");
                                string pass = empresa.PassKey;
                                SAT.CFDI.Cliente.Procesamiento.Encabezado encLMetadata2 = new SAT.CFDI.Cliente.Procesamiento.Encabezado(empresa.RFC, this.FechaHoy(), uuidsCancelar);
                                SAT.CFDI.Cliente.Procesamiento.ServicioCancelacionCFDI.Acuse ac2 = null;
                                acuse = this.EnviarCancelacionSAT(encLMetadata2, pathKey, pass, pathCer, ref ac2);
                                if (string.IsNullOrEmpty(acuse))
                                {
                                    acuse = "No se pudo conectar al servicio de cancelación del SAT";
                                }
                                int intEstatus2 = this.TraerEstatusSAT(uuid, ac2);
                                if (intEstatus2 != 201 && intEstatus2 != 202)
                                {
                                    Cancelador.Logger.Error(acuse);
                                    respuesta = intEstatus2 + " - " + Constantes.ErroresValidacion[intEstatus2];
                                    throw new FaultException(respuesta);
                                }
                                Cancelador.Logger.Info(acuse);
                                string directorio = Path.Combine(ConfigurationManager.AppSettings["RutaTimbrado"], ac2.RfcEmisor, ac2.Fecha.ToString("yyyyMMdd"));
                                if (!Directory.Exists(directorio))
                                {
                                    Directory.CreateDirectory(directorio);
                                }
                                string fileName = Path.Combine(directorio, "Cancelacion_" + uuid.ToString() + ".xml");
                                File.WriteAllText(fileName, acuse, Encoding.UTF8);
                                respuesta = intEstatus2 + " - " + Constantes.ErroresValidacion[intEstatus2];
                                result = intEstatus2;
                                return result;
                            }
                            else
                            {
                                timbre = new TimbreWs33();
                                timbre.RfcEmisor = timbreHist.RfcEmisor;
                                timbre.RfcReceptor = timbreHist.RfcReceptor;
                                timbre.AcuseCancelacion = timbreHist.AcuseCancelacion;
                                timbre.IdTimbre = timbreHist.IdTimbre;
                                timbre.Uuid = timbreHist.Uuid;
                                timbre.StrError = "Hist";
                            }
                        }
                        if (timbre.RfcEmisor !=empresa.RFC)
                        {
                            respuesta = "203 - UUID No corresponde el RFC del emisor y de quien solicita la cancelación.";
                            acuse = "";
                            result = 203;
                        }
                        else if (timbre.AcuseCancelacion != null)
                        {
                            respuesta = "202 - UUID Previamente cancelado";
                            acuse = timbre.AcuseCancelacion;
                            result = 202;
                        }
                        else
                        {
                            string path = Path.Combine(ConfigurationManager.AppSettings["Resources"],empresa.RFC);
                            string pathCer = Path.Combine(path, "Certs", "csd.cer");
                            string pathKey = Path.Combine(path, "Certs", "csd.key");
                            string pass = empresa.PassKey;
                            SAT.CFDI.Cliente.Procesamiento.Encabezado encLMetadata3 = new SAT.CFDI.Cliente.Procesamiento.Encabezado(empresa.RFC, this.FechaHoy(), uuidsCancelar);
                            SAT.CFDI.Cliente.Procesamiento.ServicioCancelacionCFDI.Acuse ac3 = null;
                            acuse = this.EnviarCancelacionSAT(encLMetadata3, pathKey, pass, pathCer, ref ac3);
                            if (string.IsNullOrEmpty(acuse))
                            {
                                acuse = "No se pudo conectar al servicio de cancelación del SAT";
                            }
                            int intEstatus3 = this.TraerEstatusSAT(uuid, ac3);
                            if (intEstatus3 != 201 && intEstatus3 != 202)
                            {
                                Cancelador.Logger.Error(acuse);
                                respuesta = intEstatus3 + " - " + Constantes.ErroresValidacion[intEstatus3];
                                throw new FaultException(respuesta);
                            }
                            Cancelador.Logger.Info(acuse);
                            timbre.Status = new int?(2);
                            string directorio = Path.Combine(ConfigurationManager.AppSettings["RutaTimbrado"], timbre.RfcEmisor, timbre.FechaFactura.ToString("yyyyMMdd"));
                            if (!Directory.Exists(directorio))
                            {
                                Directory.CreateDirectory(directorio);
                            }
                            string fileName = Path.Combine(directorio, "Cancelacion_" + timbre.Uuid.ToString() + ".xml");
                            File.WriteAllText(fileName, acuse, Encoding.UTF8);
                            tim.GuardarTimbre(timbre);
                            respuesta = intEstatus3 + " - " + Constantes.ErroresValidacion[intEstatus3];
                            result = intEstatus3;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Cancelador.Logger.Error("(cancelar) Error: " + ex.Message + ((ex.InnerException == null) ? "" : ("\nExcepción Interna:" + ex.InnerException.Message)));
                throw;
            }
            return result;
        }

        public int CancelarRet(string uuid, string rfc, ref string respuesta, ref string acuse)
        {
            int result;
            try
            {
                Cancelador.Logger.Info("Cancelando comprobante: " + uuid);
                CanceladorRetenciones can = new CanceladorRetenciones();
                IList uuidsCancelar = new List<string>();
                uuidsCancelar.Add(uuid.ToUpper());
                using (new NtLinkLocalServiceEntities())
                {
                    NtLinkEmpresa nle = new NtLinkEmpresa();
                    empresa empresa = nle.GetByRfc(rfc);
                    if (empresa == null)
                    {
                        respuesta = "300 - El usuario con el que se quiere conectar es inválido";
                        acuse = "";
                        result = 300;
                    }
                    else
                    {
                        NtLinkTimbrado tim = new NtLinkTimbrado();
                        TimbreWs33 timbre = tim.ObtenerTimbre(uuid);
                        if (timbre == null)
                        {
                            TimbreWsHistorico timbreHist = tim.ObtenerTimbreHist(uuid);
                            if (timbreHist == null)
                            {
                                string path = Path.Combine(ConfigurationManager.AppSettings["Resources"], rfc);
                                string pathCer = Path.Combine(path, "Certs", "csd.cer");
                                string pathKey = Path.Combine(path, "Certs", "csd.key");
                                string pass = empresa.PassKey;
                                if (File.Exists(pathKey + ".pem"))
                                {
                                    pathKey += ".pem";
                                }
                                string ext = Path.GetExtension(pathKey);
                                RSACryptoServiceProvider key2 = OpensslKey.DecodePrivateKey(File.ReadAllBytes(pathKey), pass, ext);
                                string ac2 = can.CancelaCfdi(uuid, key2, rfc, pathCer);
                                if (string.IsNullOrEmpty(ac2))
                                {
                                    acuse = "No se pudo conectar al servicio de cancelación del SAT";
                                    respuesta = "1205 - No se pudo conectar al servicio de cancelación del SAT";
                                    result = 1205;
                                    return result;
                                }
                                acuse = ac2;
                                AcuseCancelacion acuseCan2 = AcuseCancelacion.Parse(ac2);
                                if (acuseCan2.Status == null)
                                {
                                    Cancelador.Logger.Error(acuse);
                                    respuesta = "1300 - XML mal formado";
                                    result = 1300;
                                    return result;
                                }
                                if (acuseCan2.Status != "1201" && acuseCan2.Status != "1202")
                                {
                                    Cancelador.Logger.Error(acuse);
                                    respuesta = acuseCan2.Status;
                                    throw new FaultException(respuesta);
                                }
                                Cancelador.Logger.Info(acuse);
                                string directorio = Path.Combine(ConfigurationManager.AppSettings["RutaTimbrado"], rfc, acuseCan2.FechaCancelacion);
                                if (!Directory.Exists(directorio))
                                {
                                    Directory.CreateDirectory(directorio);
                                }
                                string fileName = Path.Combine(directorio, "Cancelacion_" + uuid.ToString() + ".xml");
                                File.WriteAllText(fileName, acuse, Encoding.UTF8);
                                respuesta = acuseCan2.Status + " - " + Constantes.ErroresValidacion[int.Parse(acuseCan2.Status)];
                                result = Convert.ToInt32(acuseCan2.Status);
                                return result;
                            }
                            else
                            {
                                timbre = new TimbreWs33();
                                timbre.RfcEmisor = timbreHist.RfcEmisor;
                                timbre.RfcReceptor = timbreHist.RfcReceptor;
                                timbre.AcuseCancelacion = timbreHist.AcuseCancelacion;
                                timbre.IdTimbre = timbreHist.IdTimbre;
                                timbre.Uuid = timbreHist.Uuid;
                                timbre.StrError = "Hist";
                            }
                        }
                        if (timbre.RfcEmisor != rfc)
                        {
                            respuesta = "203 - UUID No corresponde el RFC del emisor y de quien solicita la cancelación.";
                            acuse = "";
                            result = 203;
                        }
                        else if (timbre.AcuseCancelacion != null)
                        {
                            respuesta = "202 - UUID Previamente cancelado";
                            acuse = timbre.AcuseCancelacion;
                            result = 202;
                        }
                        else
                        {
                            string path = Path.Combine(ConfigurationManager.AppSettings["Resources"], rfc);
                            string pathCer = Path.Combine(path, "Certs", "csd.cer");
                            string pathKey = Path.Combine(path, "Certs", "csd.key");
                            string pass = empresa.PassKey;
                            if (File.Exists(pathKey + ".pem"))
                            {
                                pathKey += ".pem";
                            }
                            string ext = Path.GetExtension(pathKey);
                            RSACryptoServiceProvider key3 = OpensslKey.DecodePrivateKey(File.ReadAllBytes(pathKey), pass, ext);
                            string ac3 = can.CancelaCfdi(uuid, key3, rfc, pathCer);
                            if (string.IsNullOrEmpty(ac3))
                            {
                                acuse = "No se pudo conectar al servicio de cancelación del SAT";
                                respuesta = "1205 - No se pudo conectar al servicio de cancelación del SAT";
                                result = 1205;
                            }
                            else
                            {
                                acuse = ac3;
                                AcuseCancelacion acuseCan3 = AcuseCancelacion.Parse(ac3);
                                if (acuseCan3.Status == null)
                                {
                                    Cancelador.Logger.Error(acuse);
                                    respuesta = "1300 - XML mal formado";
                                    result = 1300;
                                }
                                else
                                {
                                    if (acuseCan3.Status != "1201" && acuseCan3.Status != "1202")
                                    {
                                        Cancelador.Logger.Error(acuse);
                                        respuesta = acuseCan3.Status;
                                        throw new FaultException(respuesta);
                                    }
                                    Cancelador.Logger.Info(acuse);
                                    timbre.Status = new int?(2);
                                    string directorio = Path.Combine(ConfigurationManager.AppSettings["RutaTimbrado"], timbre.RfcEmisor, timbre.FechaFactura.ToString("yyyyMMdd"));
                                    if (!Directory.Exists(directorio))
                                    {
                                        Directory.CreateDirectory(directorio);
                                    }
                                    string fileName = Path.Combine(directorio, "Cancelacion_" + timbre.Uuid.ToString() + ".xml");
                                    File.WriteAllText(fileName, acuse, Encoding.UTF8);
                                    tim.GuardarTimbre(timbre);
                                    respuesta = acuseCan3.Status + " - " + Constantes.ErroresValidacion[int.Parse(acuseCan3.Status)];
                                    result = Convert.ToInt32(acuseCan3.Status);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Cancelador.Logger.Error("(cancelar) Error: " + ex.Message + ((ex.InnerException == null) ? "" : ("\nExcepción Interna:" + ex.InnerException.Message)));
                throw;
            }
            return result;
        }

        private string EnviarCancelacionSAT(SAT.CFDI.Cliente.Procesamiento.Encabezado metadata, string privada, string passPrivada, string publica, ref SAT.CFDI.Cliente.Procesamiento.ServicioCancelacionCFDI.Acuse cAcuse)
        {
            string result;
            try
            {
                AccesoServicios accesoServicios = new AccesoServicios();
                SAT.CFDI.Cliente.Procesamiento.ServicioCancelacionCFDI.Acuse acuse = null;
                string acuseRecibo = accesoServicios.CancelarBloqueCfdi(string.Empty, string.Empty, metadata, privada, passPrivada, publica, ref acuse);
                cAcuse = acuse;
                Cancelador.Logger.Info(acuseRecibo);
                result = acuseRecibo;
            }
            catch (Exception ex)
            {
                Cancelador.Logger.Error("(EnviarCancelacionSAT) Error: " + ex.Message + ((ex.InnerException == null) ? "" : ("\nExcepción Interna:" + ex.InnerException.Message)));
                throw ex;
            }
            return result;
        }

        private string EnviarCancelacionSATOtrosPACs(SAT.CFDI.Cliente.Procesamiento.Encabezado metadata, string Base64Key, string passPrivada, string Base64Cer, ref SAT.CFDI.Cliente.Procesamiento.ServicioCancelacionCFDI.Acuse cAcuse)
        {
            string result;
            try
            {
                AccesoServicios accesoServicios = new AccesoServicios();
                SAT.CFDI.Cliente.Procesamiento.ServicioCancelacionCFDI.Acuse acuse = null;
                string acuseRecibo = accesoServicios.CancelarBloqueCfdiOtrosPACs(string.Empty, string.Empty, metadata, Base64Key, passPrivada, Base64Cer, ref acuse);
                cAcuse = acuse;
                Cancelador.Logger.Info(acuseRecibo);
                result = acuseRecibo;
            }
            catch (Exception ex)
            {
                Cancelador.Logger.Error("(EnviarCancelacionSAT) Error: " + ex.Message + ((ex.InnerException == null) ? "" : ("\nExcepción Interna:" + ex.InnerException.Message)));
                throw ex;
            }
            return result;
        }

        private int TraerEstatusSAT(string uuid, SAT.CFDI.Cliente.Procesamiento.ServicioCancelacionCFDI.Acuse acuse)
        {
            int result;
            try
            {
                if (acuse.Folios != null && acuse.Folios.Length > 0)
                {
                    SAT.CFDI.Cliente.Procesamiento.ServicioCancelacionCFDI.AcuseFolios[] folios = acuse.Folios;
                    for (int i = 0; i < folios.Length; i++)
                    {
                        SAT.CFDI.Cliente.Procesamiento.ServicioCancelacionCFDI.AcuseFolios acuseFoliose = folios[i];
                        if (acuseFoliose.UUID == uuid)
                        {
                            result = Convert.ToInt32(acuseFoliose.EstatusUUID);
                            return result;
                        }
                    }
                }
                else if (!string.IsNullOrEmpty(acuse.CodEstatus))
                {
                    result = Convert.ToInt32(acuse.CodEstatus);
                    return result;
                }
                result = 0;
            }
            catch (Exception err)
            {
                Cancelador.Logger.Error("(TraerEstatusSAT) Error al intentar parsear el XML de respuesta SAT, Err:" + err);
                result = 104;
            }
            return result;
        }

        private string FechaHoy()
        {
            return string.Concat(new object[]
			{
				DateTime.Now.Year,
				"-",
				(DateTime.Now.Month.ToString(CultureInfo.InvariantCulture).Length < 2) ? ("0" + DateTime.Now.Month.ToString(CultureInfo.InvariantCulture)) : DateTime.Now.Month.ToString(CultureInfo.InvariantCulture),
				"-",
				(DateTime.Now.Day.ToString(CultureInfo.InvariantCulture).Length < 2) ? ("0" + DateTime.Now.Day.ToString(CultureInfo.InvariantCulture)) : DateTime.Now.Day.ToString(CultureInfo.InvariantCulture),
				"T",
				(DateTime.Now.Hour.ToString(CultureInfo.InvariantCulture).Length < 2) ? ("0" + DateTime.Now.Hour.ToString(CultureInfo.InvariantCulture)) : DateTime.Now.Hour.ToString(CultureInfo.InvariantCulture),
				":",
				(DateTime.Now.Minute.ToString(CultureInfo.InvariantCulture).Length < 2) ? ("0" + DateTime.Now.Minute.ToString(CultureInfo.InvariantCulture)) : DateTime.Now.Minute.ToString(CultureInfo.InvariantCulture),
				":",
				(DateTime.Now.Second.ToString(CultureInfo.InvariantCulture).Length < 2) ? ("0" + DateTime.Now.Second.ToString(CultureInfo.InvariantCulture)) : DateTime.Now.Second.ToString(CultureInfo.InvariantCulture)
			});
        }
    }
}
