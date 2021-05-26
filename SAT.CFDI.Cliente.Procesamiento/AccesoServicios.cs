using log4net;
using log4net.Config;
using SAT.CFDI.Cliente.Procesamiento.ServicioAceptacionRechazo;
using SAT.CFDI.Cliente.Procesamiento.ServicioAutenticacionCFDI;
using SAT.CFDI.Cliente.Procesamiento.ServicioCancelacionCFDI;
using SAT.CFDI.Cliente.Procesamiento.ServicioConsultaCFDI;
using SAT.CFDI.Cliente.Procesamiento.ServicioRecepcionCFDI;
using SAT.CFDI.Cliente.Procesamiento.ServicioRelacionados;
using ServicioLocalContract;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Web;
using System.Xml;
using System.Xml.Serialization;

namespace SAT.CFDI.Cliente.Procesamiento
{
    public class AccesoServicios
    {
        private X509Certificate2 cert = null;

        private static readonly ILog Logger = LogManager.GetLogger(typeof(AccesoServicios));

        private RecibeCFDIServiceClient clienteRecepcion;

        private CancelaCFDBindingClient clienteCancelacion;

        private AutenticacionClient clienteAutenticacion;

        private static readonly ILog Log = LogManager.GetLogger(typeof(AccesoServicios));

        private AccesoAlmacenBlob clienteAlmacenBlob;

        public RecibeCFDIServiceClient ClienteRecepcion
        {
            get
            {
                if (this.clienteRecepcion == null)
                {
                    this.GenerarClienteRecepcion();
                }
                return this.clienteRecepcion;
            }
        }

        public CancelaCFDBindingClient ClienteCancelacion
        {
            get
            {
                if (this.clienteCancelacion == null)
                {
                    this.GenerarClienteCancelacion();
                }
                return this.clienteCancelacion;
            }
        }

        public AutenticacionClient ClienteAutenticacion
        {
            get
            {
                if (this.clienteAutenticacion == null)
                {
                    this.GenerarClienteAutenticacion();
                }
                return this.clienteAutenticacion;
            }
        }

        public AccesoAlmacenBlob ClienteAlmacenBlob
        {
            get
            {
                if (this.clienteAlmacenBlob == null)
                {
                    this.GenerarClienteAlmacenBlob();
                }
                return this.clienteAlmacenBlob;
            }
        }

        public SAT.CFDI.Cliente.Procesamiento.ServicioCancelacionCFDI.Acuse CancelaCfdi(Cancelacion cancelacion)
        {
            HttpRequestMessageProperty tokenAutenticacion = this.AutenticaServicio();
            CancelaCFDBindingClient clienteCancelacion = new CancelaCFDBindingClient();
            SAT.CFDI.Cliente.Procesamiento.ServicioCancelacionCFDI.Acuse result;
            using (new OperationContextScope(clienteCancelacion.InnerChannel))
            {
                OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] = tokenAutenticacion;
                SAT.CFDI.Cliente.Procesamiento.ServicioCancelacionCFDI.Acuse acuseCancelacion = clienteCancelacion.CancelaCFD(cancelacion);
                result = acuseCancelacion;
            }
            return result;
        }

        public AccesoServicios()
        {
            XmlConfigurator.Configure();
        }

        public SAT.CFDI.Cliente.Procesamiento.ServicioRecepcionCFDI.Acuse EnviarBloqueCfdi(Encabezado encPMEtadato)
        {
            Stream contenidoArchivo = null;
            SAT.CFDI.Cliente.Procesamiento.ServicioRecepcionCFDI.Acuse result;
            try
            {
                EncabezadoCFDI encabezadoCfdi = new EncabezadoCFDI
                {
                    RfcEmisor = encPMEtadato.RfcEmisor,
                    VersionComprobante = encPMEtadato.version,
                    NumeroCertificado = encPMEtadato.NumeroCertificado,
                    UUID = encPMEtadato.UUID,
                    Fecha = encPMEtadato.Fecha
                };
                AccesoServicios.Log.Info("Autenticacion");
                HttpRequestMessageProperty tokenAutenticacion = this.AutenticaServicio();
                AccesoServicios.Log.Info("Termina Autenticacion");
                using (new OperationContextScope(this.ClienteRecepcion.InnerChannel))
                {
                    try
                    {
                        OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] = tokenAutenticacion;
                        contenidoArchivo = AccesoDisco.RecuperaArchivo(encPMEtadato.Xml);
                        AdministradorLogs.RegistraEntrada(string.Format("Se inicia envío de archivo, Tamaño: {0} bytes", contenidoArchivo.Length));
                        AccesoServicios.Log.Info("Inicia AlmacenarCfdiFramework4");
                        string rutaBlob = this.ClienteAlmacenBlob.AlmacenarCfdiFramework4(contenidoArchivo, File.ReadAllText(encPMEtadato.Xml), Path.GetFileName(encPMEtadato.Xml));
                        AccesoServicios.Log.Info("Termina AlmacenarCfdiFramework4");
                        AccesoServicios.Log.Info("Inicia Recibe");
                        SAT.CFDI.Cliente.Procesamiento.ServicioRecepcionCFDI.Acuse acuseRecepcion = this.ClienteRecepcion.Recibe(encabezadoCfdi, rutaBlob);
                        contenidoArchivo.Close();
                        AccesoServicios.Log.Info("Termina Recibe");
                        result = acuseRecepcion;
                        return result;
                    }
                    catch (Exception exception)
                    {
                        Console.WriteLine(exception.ToString());
                        if (contenidoArchivo != null && contenidoArchivo.CanRead)
                        {
                            contenidoArchivo.Close();
                        }
                        AccesoServicios.Log.Error(string.Format("Se genero un error proceso de recepción: {0}\n\n Stack Trace: {1}", exception.Message, exception.StackTrace));
                    }
                }
            }
            catch (Exception exception)
            {
                if (contenidoArchivo != null && contenidoArchivo.CanRead)
                {
                    contenidoArchivo.Close();
                }
                AccesoServicios.Log.Error(string.Format("Se genero un error proceso de recepción: {0}\n\n Stack Trace: {1}", exception.Message, exception.StackTrace));
            }
            result = null;
            return result;
        }

        public byte[] StreamToByteArray(Stream input)
        {
            byte[] totalStream = new byte[0];
            byte[] buffer = new byte[32];
            int read;
            while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                byte[] streamArray = new byte[totalStream.Length + read];
                totalStream.CopyTo(streamArray, 0);
                Array.Copy(buffer, 0, streamArray, totalStream.Length, read);
                totalStream = streamArray;
            }
            return totalStream;
        }

        public static void SignXmlFile(string FileName, ref string SignedFileName, RSA Key)
        {
            if (FileName == null)
            {
                throw new ArgumentNullException("FileName");
            }
            if (SignedFileName == null)
            {
                throw new ArgumentNullException("SignedFileName");
            }
            if (Key == null)
            {
                throw new ArgumentNullException("Key");
            }
            XmlDocument doc = new XmlDocument();
            doc.PreserveWhitespace = false;
            doc.LoadXml(FileName);
            SignedXml signedXml = new SignedXml(doc);
            signedXml.SigningKey = Key;
            Signature XMLSignature = signedXml.Signature;
            Reference reference = new Reference("");
            XmlDsigEnvelopedSignatureTransform env = new XmlDsigEnvelopedSignatureTransform();
            reference.AddTransform(env);
            XMLSignature.SignedInfo.AddReference(reference);
            KeyInfo keyInfo = new KeyInfo();
            keyInfo.AddClause(new RSAKeyValue(Key));
            XMLSignature.KeyInfo = keyInfo;
            signedXml.ComputeSignature();
            XmlElement xmlDigitalSignature = signedXml.GetXml();
            doc.DocumentElement.AppendChild(doc.ImportNode(xmlDigitalSignature, true));
            if (doc.FirstChild is XmlDeclaration)
            {
                doc.RemoveChild(doc.FirstChild);
            }
            SignedFileName = doc.InnerXml;
        }

        public string ConsultaEstatusCFDI(string expresion)
        {
            string result;
            try
            {
                ConsultaCFDIServiceClient CFDI = new ConsultaCFDIServiceClient();
                int i = expresion.IndexOf("&rr=");
                i += 4;
                int j = expresion.IndexOf("&tt=");
                string c = expresion.Substring(i, j - i);
                string z = c.Replace("&", "&amp;");
                expresion = expresion.Replace(c, z);
                i = expresion.IndexOf("&re=");
                i += 4;
                j = expresion.IndexOf("&rr=");
                c = expresion.Substring(i, j - i);
                z = c.Replace("&", "&amp;");
                expresion = expresion.Replace(c, z);
                SAT.CFDI.Cliente.Procesamiento.ServicioConsultaCFDI.Acuse x = CFDI.Consulta(expresion);
                result = string.Concat(new string[]
				{
					x.Estado,
					"|",
					x.EsCancelable,
					"|",
					x.EstatusCancelacion
				});
            }
            catch (Exception exception)
            {
                AccesoServicios.Log.Error("(ConsultaCFDI) Error al consultar los CFDI's " + exception.ToString());
                result = "Error en la consulta al sat";
            }
            return result;
        }

        public string ConsultaCFDI(string expresion, string uuid, string rfcReceptor, SAT.CFDI.Cliente.Procesamiento.ServicioRelacionados.SignatureType asig)
        {
            string result;
            try
            {
                /*
                if (expresion.Contains("&id="))
                {
                    expresion = expresion.Replace("&id=", "&amp;id=");
                }
                if (expresion.Contains("&re="))
                {
                    expresion = expresion.Replace("&re=", "&amp;re=");
                }
                if (expresion.Contains("&rr="))
                {
                    expresion = expresion.Replace("&rr=", "&amp;rr=");
                }
                if (expresion.Contains("&tt="))
                {
                    expresion = expresion.Replace("&tt=", "&amp;tt=");
                }
                if (expresion.Contains("&fe="))
                {
                    expresion = expresion.Replace("&fe=", "&amp;fe=");
                }
                */
                
                int i = expresion.IndexOf("&rr=");
                i += 4;
                int j = expresion.IndexOf("&tt=");
                string c = expresion.Substring(i, j - i);
                string z = c.Replace("&", "&amp;");
                expresion = expresion.Replace(c, z);
                i = expresion.IndexOf("&re=");
                i += 4;
                j = expresion.IndexOf("&rr=");
                c = expresion.Substring(i, j - i);
                z = c.Replace("&", "&amp;");
                expresion = expresion.Replace(c, z);
                


                ConsultaCFDIServiceClient CFDI = new ConsultaCFDIServiceClient();
                SAT.CFDI.Cliente.Procesamiento.ServicioConsultaCFDI.Acuse x = CFDI.Consulta(expresion);
                if (x.Estado != "No Encontrado")
                {
                    if (x.Estado == "Vigente")
                    {
                        if (x.EsCancelable != "No Cancelable")
                        {
                            result = "OK";
                        }
                        else
                        {
                            result = x.EsCancelable;
                        }
                    }
                    else if (x.Estado == "Cancelado")
                    {
                        result = "OK";
                    }
                    else
                    {
                        result = x.Estado;
                    }
                }
                else
                {
                    result = x.Estado;
                }
            }
            catch (Exception exception)
            {
                AccesoServicios.Log.Error("(ConsultaCFDI) Error al consultar los CFDI's  expresion:" + expresion + "---" + exception.ToString());
                result = null;
            }
            return result;
        }

        public string ConsultaCFDIRelacionados(string RfcPacEnviaSolicitud, string RfcReceptor, string Uuid, SAT.CFDI.Cliente.Procesamiento.ServicioRelacionados.SignatureType asig)
        {
            string result;
            try
            {
                PeticionConsultaRelacionados D = new PeticionConsultaRelacionados();
                CfdiConsultaRelacionadosServiceClient CFDI = new CfdiConsultaRelacionadosServiceClient();
                D.RfcPacEnviaSolicitud = RfcPacEnviaSolicitud;
                D.RfcReceptor = RfcReceptor;
                D.Signature = asig;
                D.Uuid = Uuid;
                HttpRequestMessageProperty tokenAutenticacion = this.AutenticaServicio();
                using (new OperationContextScope(CFDI.InnerChannel))
                {
                    OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] = tokenAutenticacion;
                    ConsultaRelacionados x = CFDI.ProcesarRespuesta(D);
                    if (!x.Resultado.Contains("Clave: 2000"))
                    {
                        result = "Cancelable";
                    }
                    else
                    {
                        result = "No Cancelable";
                    }
                }
            }
            catch (Exception exception)
            {
                AccesoServicios.Log.Error("(ConsultaRelacionados) Error al consultar los CFDI's " + exception.ToString());
                result = null;
            }
            return result;
        }

        public string ConsultaCFDIRelacionadosRequest(string RfcPacEnviaSolicitud, string RfcReceptor,string RfcEmisor, string Uuid, SAT.CFDI.Cliente.Procesamiento.ServicioRelacionados.SignatureType asig)
        {
            string result;
            try
            {
                PeticionConsultaRelacionados D = new PeticionConsultaRelacionados();
                CfdiConsultaRelacionadosServiceClient CFDI = new CfdiConsultaRelacionadosServiceClient();
                D.RfcPacEnviaSolicitud = RfcPacEnviaSolicitud;
                if (!string.IsNullOrEmpty(RfcReceptor))
                D.RfcReceptor = RfcReceptor;
                D.Signature = asig;
                D.Uuid = Uuid;
               if( !string.IsNullOrEmpty(RfcEmisor))
                D.RfcEmisor = RfcEmisor;
                HttpRequestMessageProperty tokenAutenticacion = this.AutenticaServicio();
                using (new OperationContextScope(CFDI.InnerChannel))
                {
                    OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] = tokenAutenticacion;
                    ConsultaRelacionados x = CFDI.ProcesarRespuesta(D);
                    string xml = AccesoServicios.GetXMLFromObject(x);
                    result = xml;
                }
            }
            catch (Exception exception)
            {
                AccesoServicios.Log.Error("(ConsultaRelacionados) Error al consultar los CFDI's " + exception.ToString());
                result = null;
            }
            return result;
        }

        public static string GetXMLFromObject(object o)
        {
            string result;
            try
            {
                XmlSerializer serializer = new XmlSerializer(o.GetType());
                StringWriter sw = new StringWriter();
                XmlTextWriter tw = new XmlTextWriter(sw);
                serializer.Serialize(tw, o);
                string x = sw.ToString();
                sw.Close();
                tw.Close();
                result = x;
            }
            catch (Exception ex_3E)
            {
                result = null;
            }
            return result;
        }

        public string ConsultaAceptacionRechazo(string RfcReceptor)
        {
            string result;
            try
            {
                AceptacionRechazoServiceClient CFDI = new AceptacionRechazoServiceClient();
                HttpRequestMessageProperty tokenAutenticacion = this.AutenticaServicio();
                using (new OperationContextScope(CFDI.InnerChannel))
                {
                    OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] = tokenAutenticacion;
                    AcusePeticionesPendientes x = CFDI.ObtenerPeticionesPendientes(RfcReceptor);
                    string xml = AccesoServicios.GetXMLFromObject(x);
                    result = xml;
                }
            }
            catch (Exception exception)
            {
                AccesoServicios.Log.Error("(Consultar) Error al consultar los CFDI's " + exception.ToString());
                result = null;
            }
            return result;
        }

        public string ProcesarRespuestaAceptacionRechazo(string RfcReceptor, string fecha, string rfcPac, List<SolicitudAceptacionRechazoFolios> F, SAT.CFDI.Cliente.Procesamiento.ServicioAceptacionRechazo.SignatureType asig)
        {
            string result;
            try
            {
                AceptacionRechazoServiceClient CFDI = new AceptacionRechazoServiceClient();
                AcuseAceptacionRechazo A = new AcuseAceptacionRechazo();
                HttpRequestMessageProperty tokenAutenticacion = this.AutenticaServicio();
                using (new OperationContextScope(CFDI.InnerChannel))
                {
                    OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] = tokenAutenticacion;
                    A = CFDI.ProcesarRespuesta(new SolicitudAceptacionRechazo
                    {
                        Fecha = Convert.ToDateTime(fecha),
                        RfcPacEnviaSolicitud = rfcPac,
                        RfcReceptor = RfcReceptor,
                        Signature = asig,
                        Folios = F.ToArray()
                    });
                    result = AccesoServicios.GetXMLFromObject(A);
                }
            }
            catch (Exception exception)
            {
                AccesoServicios.Log.Error("(Envio) Error al enviar los CFDI's " + exception.ToString());
                result = null;
            }
            return result;
        }

        public SAT.CFDI.Cliente.Procesamiento.ServicioAceptacionRechazo.SignatureType AsignatureProceso(string strPPrivada, Encabezado encLMetadata, string pass, string strCertificado)
        {
            SAT.CFDI.Cliente.Procesamiento.ServicioAceptacionRechazo.SignatureType result;
            try
            {
                string strPXmlFirmado = "";
                SAT.CFDI.Cliente.Procesamiento.ServicioAceptacionRechazo.SignatureType Asig = new SAT.CFDI.Cliente.Procesamiento.ServicioAceptacionRechazo.SignatureType();
                Asig.Id = "Signature";
                Asig.SignedInfo = new SAT.CFDI.Cliente.Procesamiento.ServicioAceptacionRechazo.SignedInfoType();
                Asig.SignedInfo.Id = "Signature-SignedInfo";
                Asig.SignedInfo.CanonicalizationMethod = new SAT.CFDI.Cliente.Procesamiento.ServicioAceptacionRechazo.CanonicalizationMethodType();
                Asig.SignedInfo.CanonicalizationMethod.Algorithm = "http://www.w3.org/TR/2001/REC-xml-c14n20010315";
                Asig.SignedInfo.SignatureMethod = new SAT.CFDI.Cliente.Procesamiento.ServicioAceptacionRechazo.SignatureMethodType();
                Asig.SignedInfo.SignatureMethod.Algorithm = "http://www.w3.org/2000/09/xmldsig#rsa-sha1";
                Asig.SignedInfo.Reference = new SAT.CFDI.Cliente.Procesamiento.ServicioAceptacionRechazo.ReferenceType();
                Asig.SignedInfo.Reference.URI = "#Certificate1";
                List<SAT.CFDI.Cliente.Procesamiento.ServicioAceptacionRechazo.TransformType> T = new List<SAT.CFDI.Cliente.Procesamiento.ServicioAceptacionRechazo.TransformType>();
                T.Add(new SAT.CFDI.Cliente.Procesamiento.ServicioAceptacionRechazo.TransformType
                {
                    Algorithm = "http://www.w3.org/2000/09/xmldsig#envelopedsignature"
                });
                Asig.SignedInfo.Reference.Transforms = T.ToArray();
                Asig.SignedInfo.Reference.DigestMethod = new SAT.CFDI.Cliente.Procesamiento.ServicioAceptacionRechazo.DigestMethodType();
                Asig.SignedInfo.Reference.DigestMethod.Algorithm = "http://www.w3.org/2000/09/xmldsig#sha1";
                if (File.Exists(strPPrivada + ".pem"))
                {
                    strPPrivada += ".pem";
                }
                string ext = Path.GetExtension(strPPrivada);
                AccesoServicios.SignXmlFile(this.ArmarXmlPreFirma(encLMetadata), ref strPXmlFirmado, OpensslKey.DecodePrivateKey(File.ReadAllBytes(strPPrivada), pass, ext));
                string hex = strPXmlFirmado.Substring(strPXmlFirmado.IndexOf("<DigestValue>") + 13, strPXmlFirmado.IndexOf("</DigestValue>") - strPXmlFirmado.IndexOf("<DigestValue>") - 13);
                Asig.SignedInfo.Reference.DigestValue = Convert.FromBase64String(hex);
                Asig.SignatureValue = Convert.FromBase64String(strPXmlFirmado.Substring(strPXmlFirmado.IndexOf("<SignatureValue>") + 16, strPXmlFirmado.IndexOf("</SignatureValue>") - strPXmlFirmado.IndexOf("<SignatureValue>") - 16));
                X509Certificate2 x509 = new X509Certificate2(strCertificado);
                Asig.KeyInfo = new SAT.CFDI.Cliente.Procesamiento.ServicioAceptacionRechazo.KeyInfoType();
                Asig.KeyInfo.X509Data = new SAT.CFDI.Cliente.Procesamiento.ServicioAceptacionRechazo.X509DataType();
                Asig.KeyInfo.X509Data.X509Certificate = File.ReadAllBytes(strCertificado);
                Asig.KeyInfo.Id = "Certificate1";
                result = Asig;
            }
            catch (Exception exception)
            {
                AccesoServicios.Log.Error("(Asignature) Error al generar la firma de los CFDI's " + exception.ToString());
                result = null;
            }
            return result;
        }

        public SAT.CFDI.Cliente.Procesamiento.ServicioRelacionados.SignatureType Asignature(string strPPrivada, Encabezado encLMetadata, string pass, string strCertificado)
        {
            SAT.CFDI.Cliente.Procesamiento.ServicioRelacionados.SignatureType result;
            try
            {
                string strPXmlFirmado = "";
                SAT.CFDI.Cliente.Procesamiento.ServicioRelacionados.SignatureType Asig = new SAT.CFDI.Cliente.Procesamiento.ServicioRelacionados.SignatureType();
                Asig.Id = "Signature";
                Asig.SignedInfo = new SAT.CFDI.Cliente.Procesamiento.ServicioRelacionados.SignedInfoType();
                Asig.SignedInfo.Id = "Signature-SignedInfo";
                Asig.SignedInfo.CanonicalizationMethod = new SAT.CFDI.Cliente.Procesamiento.ServicioRelacionados.CanonicalizationMethodType();
                Asig.SignedInfo.CanonicalizationMethod.Algorithm = "http://www.w3.org/TR/2001/REC-xml-c14n20010315";
                Asig.SignedInfo.SignatureMethod = new SAT.CFDI.Cliente.Procesamiento.ServicioRelacionados.SignatureMethodType();
                Asig.SignedInfo.SignatureMethod.Algorithm = "http://www.w3.org/2000/09/xmldsig#rsa-sha1";
                Asig.SignedInfo.Reference = new SAT.CFDI.Cliente.Procesamiento.ServicioRelacionados.ReferenceType();
                Asig.SignedInfo.Reference.URI = "#Certificate1";
                List<SAT.CFDI.Cliente.Procesamiento.ServicioRelacionados.TransformType> T = new List<SAT.CFDI.Cliente.Procesamiento.ServicioRelacionados.TransformType>();
                T.Add(new SAT.CFDI.Cliente.Procesamiento.ServicioRelacionados.TransformType
                {
                    Algorithm = "http://www.w3.org/2000/09/xmldsig#envelopedsignature"
                });
                Asig.SignedInfo.Reference.Transforms = T.ToArray();
                Asig.SignedInfo.Reference.DigestMethod = new SAT.CFDI.Cliente.Procesamiento.ServicioRelacionados.DigestMethodType();
                Asig.SignedInfo.Reference.DigestMethod.Algorithm = "http://www.w3.org/2000/09/xmldsig#sha1";
                if (File.Exists(strPPrivada + ".pem"))
                {
                    strPPrivada += ".pem";
                }
                string ext = Path.GetExtension(strPPrivada);
                AccesoServicios.SignXmlFile(this.ArmarXmlPreFirma(encLMetadata), ref strPXmlFirmado, OpensslKey.DecodePrivateKey(File.ReadAllBytes(strPPrivada), pass, ext));
                string hex = strPXmlFirmado.Substring(strPXmlFirmado.IndexOf("<DigestValue>") + 13, strPXmlFirmado.IndexOf("</DigestValue>") - strPXmlFirmado.IndexOf("<DigestValue>") - 13);
                Asig.SignedInfo.Reference.DigestValue = Convert.FromBase64String(hex);
                Asig.SignatureValue = Convert.FromBase64String(strPXmlFirmado.Substring(strPXmlFirmado.IndexOf("<SignatureValue>") + 16, strPXmlFirmado.IndexOf("</SignatureValue>") - strPXmlFirmado.IndexOf("<SignatureValue>") - 16));
                X509Certificate2 x509 = new X509Certificate2(strCertificado);
                Asig.KeyInfo = new SAT.CFDI.Cliente.Procesamiento.ServicioRelacionados.KeyInfoType();
                Asig.KeyInfo.X509Data = new SAT.CFDI.Cliente.Procesamiento.ServicioRelacionados.X509DataType();
                Asig.KeyInfo.X509Data.X509Certificate = File.ReadAllBytes(strCertificado);
                Asig.KeyInfo.Id = "Certificate1";
                result = Asig;
            }
            catch (Exception exception)
            {
                AccesoServicios.Log.Error("(Asignature) Error al generar la firma de los CFDI's " + exception.ToString());
                result = null;
            }
            return result;
        }

        public string CancelarBloqueCfdiOtrosPACs(string directorioLog, string directorioAcuse, Encabezado encLMetadata, string Base64Key, string pass, string Base64Cer, ref SAT.CFDI.Cliente.Procesamiento.ServicioCancelacionCFDI.Acuse cAcuse)
        {
            string result;
            try
            {

                byte[] Cer = System.Convert.FromBase64String(Base64Cer);
                byte[] Key = System.Convert.FromBase64String(Base64Key);


                AccesoServicios.Logger.Info("Privada: " + Base64Key);
                AccesoServicios.Logger.Info("Certificado: " + Base64Cer);
                AccesoServicios.Logger.Info("rfc:" + encLMetadata.RfcEmisor);
                AccesoServicios.Logger.Info("fecha:" + encLMetadata.Fecha);
                AccesoServicios.Logger.Info("uuid:" + encLMetadata.LisMListaFolios[0].ToString());
                string strPXmlFirmado = "";
               
                //if (File.Exists(strPPrivada + ".pem"))
                //{
                //    strPPrivada += ".pem";
                //}
                //string ext = Path.GetExtension(strPPrivada);
                string ext = "key";

                AccesoServicios.SignXmlFile(this.ArmarXmlPreFirma(encLMetadata), ref strPXmlFirmado, OpensslKey.DecodePrivateKey(Key, pass, ext));
                AccesoServicios.Logger.Info(strPXmlFirmado);
                SAT.CFDI.Cliente.Procesamiento.ServicioCancelacionCFDI.SignatureType a = new SAT.CFDI.Cliente.Procesamiento.ServicioCancelacionCFDI.SignatureType();
                SAT.CFDI.Cliente.Procesamiento.ServicioCancelacionCFDI.ReferenceType reference = new SAT.CFDI.Cliente.Procesamiento.ServicioCancelacionCFDI.ReferenceType
                {
                    URI = ""
                };
                XmlDsigEnvelopedSignatureTransform env = new XmlDsigEnvelopedSignatureTransform();
                a.SignedInfo = new SAT.CFDI.Cliente.Procesamiento.ServicioCancelacionCFDI.SignedInfoType
                {
                    Reference = reference
                };
                string hex = strPXmlFirmado.Substring(strPXmlFirmado.IndexOf("<DigestValue>") + 13, strPXmlFirmado.IndexOf("</DigestValue>") - strPXmlFirmado.IndexOf("<DigestValue>") - 13);
                a.SignedInfo.Reference.DigestValue = Convert.FromBase64String(hex);
                a.SignedInfo.Reference.DigestMethod = new SAT.CFDI.Cliente.Procesamiento.ServicioCancelacionCFDI.DigestMethodType
                {
                    Algorithm = "http://www.w3.org/2000/09/xmldsig#sha1"
                };
                a.SignedInfo.Reference.Transforms = new SAT.CFDI.Cliente.Procesamiento.ServicioCancelacionCFDI.TransformType[]
				{
					new SAT.CFDI.Cliente.Procesamiento.ServicioCancelacionCFDI.TransformType()
				};
                a.SignedInfo.Reference.Transforms[0].Algorithm = "http://www.w3.org/2000/09/xmldsig#enveloped-signature";
                a.SignedInfo.CanonicalizationMethod = new SAT.CFDI.Cliente.Procesamiento.ServicioCancelacionCFDI.CanonicalizationMethodType
                {
                    Algorithm = "http://www.w3.org/TR/2001/REC-xml-c14n-20010315"
                };
                a.SignedInfo.SignatureMethod = new SAT.CFDI.Cliente.Procesamiento.ServicioCancelacionCFDI.SignatureMethodType
                {
                    Algorithm = "http://www.w3.org/2000/09/xmldsig#rsa-sha1"
                };
                a.SignatureValue = Convert.FromBase64String(strPXmlFirmado.Substring(strPXmlFirmado.IndexOf("<SignatureValue>") + 16, strPXmlFirmado.IndexOf("</SignatureValue>") - strPXmlFirmado.IndexOf("<SignatureValue>") - 16));
                X509Certificate2 x509 = new X509Certificate2(Cer);
                a.KeyInfo = new SAT.CFDI.Cliente.Procesamiento.ServicioCancelacionCFDI.KeyInfoType();
                a.KeyInfo.X509Data = new SAT.CFDI.Cliente.Procesamiento.ServicioCancelacionCFDI.X509DataType();
                a.KeyInfo.X509Data.X509IssuerSerial = new SAT.CFDI.Cliente.Procesamiento.ServicioCancelacionCFDI.X509IssuerSerialType();
                a.KeyInfo.X509Data.X509IssuerSerial.X509IssuerName = x509.IssuerName.Name.ToString();
                a.KeyInfo.X509Data.X509IssuerSerial.X509SerialNumber = x509.SerialNumber;
                //a.KeyInfo.X509Data.X509Certificate = File.ReadAllBytes(strCertificado);
                a.KeyInfo.X509Data.X509Certificate = Cer;

                Console.WriteLine("Enviando Cancelación...");
                string a2 = a.ToString();
                Cancelacion mensajeCancelacion = new Cancelacion
                {
                    RfcEmisor = encLMetadata.RfcEmisor,
                    Fecha = Convert.ToDateTime(encLMetadata.Fecha),
                    Signature = a
                };
                mensajeCancelacion.Folios = new CancelacionFolios[encLMetadata.LisMListaFolios.Count];
                for (int i = 0; i < encLMetadata.LisMListaFolios.Count; i++)
                {
                    mensajeCancelacion.Folios[i] = new CancelacionFolios
                    {
                        UUID = encLMetadata.LisMListaFolios[i].ToString()
                    };
                }
                if (mensajeCancelacion.Folios.Count<CancelacionFolios>() > 0)
                {
                    Console.WriteLine("Autenticando...");
                    HttpRequestMessageProperty tokenAutenticacion = this.AutenticaServicio();
                    using (new OperationContextScope(this.ClienteCancelacion.InnerChannel))
                    {
                        OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] = tokenAutenticacion;
                        Console.WriteLine("Cancelando...");
                        SAT.CFDI.Cliente.Procesamiento.ServicioCancelacionCFDI.Acuse acuseCancelacion = this.ClienteCancelacion.CancelaCFD(mensajeCancelacion);
                        MemoryStream acuseStream = new MemoryStream();
                        XmlSerializer xmlSerializer = new XmlSerializer(typeof(SAT.CFDI.Cliente.Procesamiento.ServicioCancelacionCFDI.Acuse));
                        xmlSerializer.Serialize(acuseStream, acuseCancelacion);
                        acuseStream.Seek(0L, SeekOrigin.Begin);
                        StreamReader acuseReader = new StreamReader(acuseStream);
                        AccesoServicios.Log.Info("Terminando el proceso...");
                        cAcuse = acuseCancelacion;
                        result = acuseReader.ReadToEnd();
                        return result;
                    }
                }
                result = string.Empty;
                return result;
            }
            catch (Exception exception)
            {
                AccesoServicios.Log.Error("(CancelarBloqueCfdi) Error al cancelar los CFDI's " + exception.ToString());
            }
            result = "";
            return result;
        }

        public string CancelarBloqueCfdi(string directorioLog, string directorioAcuse, Encabezado encLMetadata, string strPPrivada, string pass, string strCertificado, ref SAT.CFDI.Cliente.Procesamiento.ServicioCancelacionCFDI.Acuse cAcuse)
        {
            string result;
            try
            {
                AccesoServicios.Logger.Info("Privada: " + strPPrivada);
                AccesoServicios.Logger.Info("Certificado: " + strCertificado);
                AccesoServicios.Logger.Info("rfc:" + encLMetadata.RfcEmisor);
                AccesoServicios.Logger.Info("fecha:" + encLMetadata.Fecha);
                AccesoServicios.Logger.Info("uuid:" + encLMetadata.LisMListaFolios[0].ToString());
                string strPXmlFirmado = "";
                if (File.Exists(strPPrivada + ".pem"))
                {
                    strPPrivada += ".pem";
                }
                string ext = Path.GetExtension(strPPrivada);
                AccesoServicios.SignXmlFile(this.ArmarXmlPreFirma(encLMetadata), ref strPXmlFirmado, OpensslKey.DecodePrivateKey(File.ReadAllBytes(strPPrivada), pass, ext));
                AccesoServicios.Logger.Info(strPXmlFirmado);
                SAT.CFDI.Cliente.Procesamiento.ServicioCancelacionCFDI.SignatureType a = new SAT.CFDI.Cliente.Procesamiento.ServicioCancelacionCFDI.SignatureType();
                SAT.CFDI.Cliente.Procesamiento.ServicioCancelacionCFDI.ReferenceType reference = new SAT.CFDI.Cliente.Procesamiento.ServicioCancelacionCFDI.ReferenceType
                {
                    URI = ""
                };
                XmlDsigEnvelopedSignatureTransform env = new XmlDsigEnvelopedSignatureTransform();
                a.SignedInfo = new SAT.CFDI.Cliente.Procesamiento.ServicioCancelacionCFDI.SignedInfoType
                {
                    Reference = reference
                };
                string hex = strPXmlFirmado.Substring(strPXmlFirmado.IndexOf("<DigestValue>") + 13, strPXmlFirmado.IndexOf("</DigestValue>") - strPXmlFirmado.IndexOf("<DigestValue>") - 13);
                a.SignedInfo.Reference.DigestValue = Convert.FromBase64String(hex);
                a.SignedInfo.Reference.DigestMethod = new SAT.CFDI.Cliente.Procesamiento.ServicioCancelacionCFDI.DigestMethodType
                {
                    Algorithm = "http://www.w3.org/2000/09/xmldsig#sha1"
                };
                a.SignedInfo.Reference.Transforms = new SAT.CFDI.Cliente.Procesamiento.ServicioCancelacionCFDI.TransformType[]
				{
					new SAT.CFDI.Cliente.Procesamiento.ServicioCancelacionCFDI.TransformType()
				};
                a.SignedInfo.Reference.Transforms[0].Algorithm = "http://www.w3.org/2000/09/xmldsig#enveloped-signature";
                a.SignedInfo.CanonicalizationMethod = new SAT.CFDI.Cliente.Procesamiento.ServicioCancelacionCFDI.CanonicalizationMethodType
                {
                    Algorithm = "http://www.w3.org/TR/2001/REC-xml-c14n-20010315"
                };
                a.SignedInfo.SignatureMethod = new SAT.CFDI.Cliente.Procesamiento.ServicioCancelacionCFDI.SignatureMethodType
                {
                    Algorithm = "http://www.w3.org/2000/09/xmldsig#rsa-sha1"
                };
                a.SignatureValue = Convert.FromBase64String(strPXmlFirmado.Substring(strPXmlFirmado.IndexOf("<SignatureValue>") + 16, strPXmlFirmado.IndexOf("</SignatureValue>") - strPXmlFirmado.IndexOf("<SignatureValue>") - 16));
                X509Certificate2 x509 = new X509Certificate2(strCertificado);
                a.KeyInfo = new SAT.CFDI.Cliente.Procesamiento.ServicioCancelacionCFDI.KeyInfoType();
                a.KeyInfo.X509Data = new SAT.CFDI.Cliente.Procesamiento.ServicioCancelacionCFDI.X509DataType();
                a.KeyInfo.X509Data.X509IssuerSerial = new SAT.CFDI.Cliente.Procesamiento.ServicioCancelacionCFDI.X509IssuerSerialType();
                a.KeyInfo.X509Data.X509IssuerSerial.X509IssuerName = x509.IssuerName.Name.ToString();
                a.KeyInfo.X509Data.X509IssuerSerial.X509SerialNumber = x509.SerialNumber;
                a.KeyInfo.X509Data.X509Certificate = File.ReadAllBytes(strCertificado);
                Console.WriteLine("Enviando Cancelación...");
                string a2 = a.ToString();
                Cancelacion mensajeCancelacion = new Cancelacion
                {
                    RfcEmisor = encLMetadata.RfcEmisor,
                    Fecha = Convert.ToDateTime(encLMetadata.Fecha),
                    Signature = a
                };
                mensajeCancelacion.Folios = new CancelacionFolios[encLMetadata.LisMListaFolios.Count];
                for (int i = 0; i < encLMetadata.LisMListaFolios.Count; i++)
                {
                    mensajeCancelacion.Folios[i] = new CancelacionFolios
                    {
                        UUID = encLMetadata.LisMListaFolios[i].ToString()
                    };
                }
                if (mensajeCancelacion.Folios.Count<CancelacionFolios>() > 0)
                {
                    Console.WriteLine("Autenticando...");
                    HttpRequestMessageProperty tokenAutenticacion = this.AutenticaServicio();
                    using (new OperationContextScope(this.ClienteCancelacion.InnerChannel))
                    {
                        OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] = tokenAutenticacion;
                        Console.WriteLine("Cancelando...");
                        SAT.CFDI.Cliente.Procesamiento.ServicioCancelacionCFDI.Acuse acuseCancelacion = this.ClienteCancelacion.CancelaCFD(mensajeCancelacion);
                        MemoryStream acuseStream = new MemoryStream();
                        XmlSerializer xmlSerializer = new XmlSerializer(typeof(SAT.CFDI.Cliente.Procesamiento.ServicioCancelacionCFDI.Acuse));
                        xmlSerializer.Serialize(acuseStream, acuseCancelacion);
                        acuseStream.Seek(0L, SeekOrigin.Begin);
                        StreamReader acuseReader = new StreamReader(acuseStream);
                        AccesoServicios.Log.Info("Terminando el proceso...");
                        cAcuse = acuseCancelacion;
                        result = acuseReader.ReadToEnd();
                        return result;
                    }
                }
                result = string.Empty;
                return result;
            }
            catch (Exception exception)
            {
                AccesoServicios.Log.Error("(CancelarBloqueCfdi) Error al cancelar los CFDI's " + exception.ToString());
            }
            result = "";
            return result;
        }

        private string ArmarXmlPreFirma(Encabezado encLMetadata)
        {
            string str = string.Concat(new string[]
			{
				"<?xml version=\"1.0\"?><Cancelacion xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" RfcEmisor=\"",
				HttpUtility.HtmlEncode(encLMetadata.RfcEmisor),
				"\" Fecha=\"",
				encLMetadata.FechaString,
				"\" xmlns=\"http://cancelacfd.sat.gob.mx\">"
			});
            str = encLMetadata.LisMListaFolios.Cast<object>().Aggregate(str, (string current, object elemento) => current + "<Folios><UUID>" + elemento.ToString() + "</UUID></Folios>");
            return str + "</Cancelacion>";
        }

        private static bool ValidarCertificadoRemoto(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors policyErrors)
        {
            return true;
        }

        private static string MakeRequest(string uri, string method, WebProxy proxy)
        {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(uri);
            webRequest.AllowAutoRedirect = true;
            webRequest.Method = method;
            ServicePointManager.ServerCertificateValidationCallback = (RemoteCertificateValidationCallback)Delegate.Combine(ServicePointManager.ServerCertificateValidationCallback, new RemoteCertificateValidationCallback(AccesoServicios.ValidarCertificadoRemoto));
            if (proxy != null)
            {
                webRequest.Proxy = proxy;
            }
            HttpWebResponse response = null;
            string result;
            try
            {
                response = (HttpWebResponse)webRequest.GetResponse();
                using (Stream s = response.GetResponseStream())
                {
                    using (StreamReader sr = new StreamReader(s))
                    {
                        result = sr.ReadToEnd();
                    }
                }
            }
            finally
            {
                if (response != null)
                {
                    response.Close();
                }
            }
            return result;
        }

        private HttpRequestMessageProperty AutenticaServicio()
        {
            string token = this.ClienteAutenticacion.Autentica();
            string headerValue = string.Format("WRAP access_token=\"{0}\"", HttpUtility.UrlDecode(token));
          //  AccesoServicios.Logger.Info(headerValue);
            return new HttpRequestMessageProperty
            {
                Method = "POST",
                Headers = 
				{
					{
						HttpRequestHeader.Authorization,
						headerValue
					}
				}
            };
        }

        private void GenerarClienteRecepcion()
        {
            this.clienteRecepcion = new RecibeCFDIServiceClient();
        }

        private void GenerarClienteAlmacenBlob()
        {
            this.clienteAlmacenBlob = new AccesoAlmacenBlob();
        }

        private void GenerarClienteCancelacion()
        {
            this.clienteCancelacion = new CancelaCFDBindingClient();
        }

        private void GenerarClienteAutenticacion()
        {
            this.clienteAutenticacion = new AutenticacionClient();
        }
    }
}
