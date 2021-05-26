using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.Text;
using System.Web.Services.Protocols;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using ClienteNtLink;
using Gma.QrCodeNet.Encoding;
using Gma.QrCodeNet.Encoding.Windows.Render;
using MessagingToolkit.QRCode.Codec;
using ServicioLocal.Business.ReportExecution;
using ServicioLocal.Business.ReportService;
using ServicioLocalContract;
using log4net;
using log4net.Config;
using System.Web;

using ParameterValue = ServicioLocal.Business.ReportExecution.ParameterValue;
using Warning = ServicioLocal.Business.ReportExecution.Warning;


namespace ServicioLocal.Business
{
    public class GeneradorCfdi
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(GeneradorCfdi));


        public GeneradorCfdi()
        {
            XmlConfigurator.Configure();
        }

        private string GetXml(Comprobante p, string complemento)
        {
            XmlSerializer ser = new XmlSerializer(typeof(Comprobante));
            using (MemoryStream memStream = new MemoryStream())
            {
                var sw = new StreamWriter(memStream, Encoding.UTF8);
                using (XmlWriter xmlWriter = XmlWriter.Create(sw, new XmlWriterSettings() { Indent = false, Encoding = Encoding.UTF8 }))
                {
                    XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
                    namespaces.Add("xsi", "http://www.w3.org/2001/XMLSchema-instance");
                    namespaces.Add("cfdi", "http://www.sat.gob.mx/cfd/3");

                    ser.Serialize(xmlWriter, p, namespaces);
                    string xml = Encoding.UTF8.GetString(memStream.GetBuffer());
                    xml = xml.Substring(xml.IndexOf(Convert.ToChar(60)));
                    xml = xml.Substring(0, (xml.LastIndexOf(Convert.ToChar(62)) + 1));
                    //if (complemento != null) //modificado
                    {
                        XElement comprobante = XElement.Parse(xml);
                        var comp = comprobante.Elements(_ns + "Complemento").FirstOrDefault();
                        if (comp == null)
                        {
                            comprobante.Add(new XElement(_ns + "Complemento"));
                            comp = comprobante.Elements(_ns + "Complemento").FirstOrDefault();
                        }
                        comp.Add(XElement.Parse(complemento));
                        SidetecStringWriter swriter = new SidetecStringWriter(Encoding.UTF8);
                        comprobante.Save(swriter,SaveOptions.DisableFormatting);
                        return swriter.ToString();
                    }

                 //   return xml; // modificado
                }
            }
        }


        public Byte[] GetPdfFromComprobante(string xmlComprobante)
        {
            Comprobante comprobante = GetComprobanteFromString(xmlComprobante);
            comprobante.CadenaOriginalTimbre = @"||1.0|6D586938-1A02-44A1-B015-CC1B37D56BCF|2012-08-11T21:46:41|kB0Caoa3gtsqo8klGTHaOgDLCOX1mjT84vaTm0l9iM82sSfTlhLrqEd5o+X3lzETlxaLmQogX27N+tD+Izc/BsqFWHax5Ln2krh9ER0feWD4CglUqGZwnu7BWnFcLNgN8OtcmvrRibjBTsAEOvcfZu4q80aXb/b2LxEHbqM3yuw=|00001000000201614141||";
            comprobante.CantidadLetra = "UNO MXN 16/100";
            return this.GetPdfFromComprobante(comprobante, 1, TipoDocumento.FacturaGeneral);
        }

        private byte[] GetQrCode(string cadena)
        {
            QrEncoder encoder = new QrEncoder();
            QrCode qrCode;
            if (!encoder.TryEncode(cadena, out qrCode))
            {
                throw new Exception("Error al generar codigo bidimensional: " + cadena);
            }
            GraphicsRenderer gRenderer = new GraphicsRenderer(new FixedModuleSize(2, QuietZoneModules.Two), Brushes.Black, Brushes.White);

            MemoryStream ms = new MemoryStream();
            gRenderer.WriteToStream(qrCode.Matrix, ImageFormat.Png, ms);
            return ms.GetBuffer();
        }

       

        private void PrintFields(Type tipo, string prefijo, object emisor, ComprobantePdf comp)
        {
            foreach (PropertyInfo pi in tipo.GetProperties())
            {

                if ((pi != null)/* && (pi.PropertyType == typeof(t_UbicacionFiscal) */|| pi.PropertyType == typeof(t_Ubicacion))//)
                    PrintFields(pi.PropertyType, prefijo + pi.Name, pi.GetValue(emisor, null), comp);
                else
                {
                    try
                    {
                        if (emisor != null && !pi.PropertyType.IsArray)
                        {
                            var valor = pi.GetValue(emisor, null) == null
                                   ? ""
                                   : pi.GetValue(emisor, null).ToString();
                            var property = comp.GetType().GetProperty(prefijo + "_" + pi.Name);
                            if (property != null)
                            {
                                property.SetValue(comp, valor, null);
                            }
                        }
                    }
                    catch (Exception eee)
                    {
                        Logger.Error(eee);
                    }


                }

            }
        }

        public Byte[] GetPdfFromComprobante(Comprobante comprobante, int orientacion, TipoDocumento tipo, ref long id, string metodoPago)
        {


            try
            {
                ReportService.ReportingService2005 clt = new ReportingService2005();// servicio para (pdf)
                string userName = ConfigurationManager.AppSettings["RSUserName"];
                string password = ConfigurationManager.AppSettings["RSPass"];
                string url = ConfigurationManager.AppSettings["RSUrlService"];
                //clt.Credentials = System.Net.CredentialCache.DefaultCredentials;
                clt.Credentials = new NetworkCredential(userName, password);
                clt.Url = url;
                CatalogItem[] cats = clt.ListChildren("/", true);
                var rep = GetRutaPdf(tipo);
                var reporte = cats.FirstOrDefault(p => p.Name == comprobante.Emisor.Rfc + "_" + rep);
                if (reporte == null)
                    reporte = cats.FirstOrDefault(p => p.Name == rep);

                if (reporte == null)
                {
                    throw new FaultException("No esta configurada la plantilla para este comprobante");
                }
                if (rep == "PdfHonorarios")
                {
                    comprobante.SubTotal = comprobante.Impuestos.TotalImpuestosTrasladados + comprobante.SubTotal;
                }
                var pdf = GuardaReporte(comprobante, metodoPago);
                id = pdf.IdComprobantePdf;
                string xmlData = null;
                if (tipo == TipoDocumento.Nomina)
                {
                    xmlData = comprobante.XmlNomina;


                }
                if (tipo == TipoDocumento.ConstructorFirmasCustom)
                {
                    xmlData = comprobante.XmlString;
                }
                //-----------------------------------nueva atributo 
                /*
                if (tipo == TipoDocumento.Complementos)//nuevo
                {

                    xmlData = comprobante.XmlString;
                    XmlDocument xm = new XmlDocument();
                    xm.LoadXml(xmlData);
                    int i = 0;
                    XmlNodeList Conceptos = xm.GetElementsByTagName("cfdi:Conceptos");
                    XmlNodeList concepto = ((XmlElement)Conceptos[0]).GetElementsByTagName("cfdi:Concepto");
                    foreach (XmlElement nodo1 in concepto)
                    {

                        XmlNodeList IA = ((XmlElement)nodo1).GetElementsByTagName("cfdi:InformacionAduanera");
                        foreach (XmlElement nodo in IA)
                        {
                            string xfecha = nodo.GetAttribute("fecha");
                            nodo.SetAttribute("fechaIA", xfecha);

                        }

                        if (i == 0 && IA.Count == 0)
                        {
                            // XmlElement el = nodo1.OwnerDocument.CreateElement("cfdi:InformacionAduanera");
                            String testNamespace = "http://www.sat.gob.mx/cfd/3";
                            XmlElement el = nodo1.OwnerDocument.CreateElement("cfdi", "InformacionAduanera", testNamespace);
                            el.SetAttribute("aduana", "");
                            el.SetAttribute("fechaIA", "");
                            el.SetAttribute("numero", "");
                            nodo1.AppendChild(el);
                        }
                        i++;
                    }

                    xmlData = xm.InnerXml.ToString();

                }//--------------------------------------------------
                */
                return GetReport(reporte.Path, pdf.IdEmpresa, pdf.IdComprobantePdf, xmlData);
            }
            catch (Exception ee)
            {
                //System.Diagnostics.Debugger.Launch();
                Logger.Error(ee);
                if (ee.InnerException != null)
                    Logger.Error(ee.InnerException);
                throw;
            }

        }
        //-------------------------------------------------------------------------------------
        private ComprobantePdf GuardaReporte(Comprobante comprobante, string MetodoPago)
        {
            ComprobantePdf pdf = new ComprobantePdf();
            Type tip = typeof(Comprobante);
            var fields = tip.GetProperties();
            foreach (PropertyInfo propertyInfo in fields)
            {
                try
                {
                    if (propertyInfo.Name == "Emisor")
                    {
                        PrintFields(propertyInfo.PropertyType, "Em", comprobante.Emisor, pdf);
                    }
                    else if (propertyInfo.Name == "Receptor")
                    {
                        PrintFields(propertyInfo.PropertyType, "Re", comprobante.Receptor, pdf);
                    }
                    else if (propertyInfo.Name == "Impuestos")
                    {
                        PrintFields(propertyInfo.PropertyType, "Imp", comprobante.Impuestos, pdf);
                    }
                    else if (propertyInfo.Name == "DatosAduana")
                    {
                        PrintFields(propertyInfo.PropertyType, "Aduana", comprobante.DatosAduana, pdf);
                    }
                    else
                    {
                        var valor = propertyInfo.GetValue(comprobante, null) == null
                                        ? ""
                                        : propertyInfo.GetValue(comprobante, null).ToString();
                        var property = pdf.GetType().GetProperty(propertyInfo.Name);
                        if (property != null)
                        {
                            property.SetValue(pdf, valor, null);
                        }
                    }
                }
                catch (Exception eee)
                {
                    Logger.Error(eee);
                }
            }
           // PrintFields(typeof(TimbreFiscalDigital), "timbre", comprobante.Complemento.timbreFiscalDigital, pdf);
            if(comprobante.Complemento.timbreFiscalDigital!=null)//algun error en la linea anteriror
            {
                pdf.timbre_CadenaOriginal = comprobante.Complemento.timbreFiscalDigital.cadenaOriginal;
                pdf.timbre_FechaTimbrado = comprobante.Complemento.timbreFiscalDigital.FechaTimbrado.ToString();
                pdf.timbre_Leyenda = comprobante.Complemento.timbreFiscalDigital.Leyenda;
                pdf.timbre_NoCertificadoSAT = comprobante.Complemento.timbreFiscalDigital.NoCertificadoSAT;
                pdf.timbre_RfcProvCertif = comprobante.Complemento.timbreFiscalDigital.RfcProvCertif;
                pdf.timbre_SelloCFD = comprobante.Complemento.timbreFiscalDigital.SelloCFD;
                pdf.timbre_SelloSAT = comprobante.Complemento.timbreFiscalDigital.SelloSAT;
                pdf.timbre_UUID = comprobante.Complemento.timbreFiscalDigital.UUID;
                pdf.timbre_Version = comprobante.Complemento.timbreFiscalDigital.Version;
            }
            string enteros;
            string decimales;
            string totalLetra = comprobante.Total.ToString();
            if (totalLetra.IndexOf('.') == -1)
            {
                enteros = "0";
                decimales = "0";
            }
            else
            {
                enteros = totalLetra.Substring(0, totalLetra.IndexOf('.'));
                decimales = totalLetra.Substring(totalLetra.IndexOf('.') + 1);
            }

            string total = enteros.PadLeft(18, '0') + "." + decimales.PadRight(6, '0');
            int tam_var = comprobante.Sello.Length;
            string Var_Sub = comprobante.Sello.Substring((tam_var - 8), 8);

            //para CFDI
            string URL = @"https://verificacfdi.facturaelectronica.sat.gob.mx/default.aspx";
            //para retenciones
            //string URL = @"https://prodretencionverificacion.clouda.sat.gob.mx/";


            string cadenaCodigo = URL + "?" + "&id=" + comprobante.Complemento.timbreFiscalDigital.UUID.ToUpper() + "&fe=" + Var_Sub + "&re=" + comprobante.Emisor.Rfc + "&rr=" + comprobante.Receptor.Rfc + "&tt=" + total;

                           

           // string cadenaCodigo = "?re=" + comprobante.Emisor.Rfc + "&rr=" + comprobante.Receptor.Rfc + "&tt=" +
            //                      total + "&id=" +
           //                       comprobante.Complemento.timbreFiscalDigital.UUID.ToUpper();

            byte[] bm = GetQrCode(cadenaCodigo);
            pdf.QrCode = bm;
            string logoEmpresa = Path.Combine(ConfigurationManager.AppSettings["Resources"],
                                              comprobante.Emisor.Rfc, "Logo.png");
            if (!File.Exists(logoEmpresa))
            {
                logoEmpresa = Path.Combine(ConfigurationManager.AppSettings["Resources"], "LogoGenerico.png");
            }

            var db = new NtLinkLocalServiceEntities();
            var empresa = db.empresa.FirstOrDefault(p => p.RFC == comprobante.Emisor.Rfc);
            if (empresa != null && empresa.Logo == null)
            {
                empresa.Logo = File.ReadAllBytes(logoEmpresa);
                db.empresa.ApplyCurrentValues(empresa);

            }
            pdf.IdEmpresa = empresa.IdEmpresa;

            pdf.metodoDePago = MetodoPago;//TipoDEPago(pdf.metodoDePago);//nuevo
            db.ComprobantePdf.AddObject(pdf);
            db.SaveChanges();
            var conceptos = comprobante.Conceptos.Select(p => new ConceptoPdf
            {
                timbre_UUID = pdf.timbre_UUID,
                cantidad = p.Cantidad.ToString(),
               // CuentaPredial = p.CuentaPredial,
                importe = p.Importe.ToString(),
                valorUnitario = p.ValorUnitario.ToString(),
                descripcion = p.Descripcion,
                Detalles = p.Detalles,
                unidad = p.Unidad,
                OrdenCompra = p.OrdenCompra,
                noIdentificacion = p.NoIdentificacion,
                IdComprobantePdf = pdf.IdComprobantePdf
            });
            foreach (ConceptoPdf conceptoPdf in conceptos)
            {
                db.ConceptoPdf.AddObject(conceptoPdf);
            }
            if (comprobante.ConceptosAduana != null)
            {
                var conceptosAddenda = comprobante.ConceptosAduana.Select(p => new ConceptoPdfAddenda()
                {
                    timbre_UUID = pdf.timbre_UUID,
                    cantidad = p.Cantidad.ToString(),
                    //CuentaPredial = p.CuentaPredial,
                    importe = p.Importe.ToString(),
                    valorUnitario = p.ValorUnitario.ToString(),
                    descripcion = p.Descripcion,
                    Detalles = p.Detalles,
                    unidad = p.Unidad,
                    OrdenCompra = p.OrdenCompra,
                    noIdentificacion = p.NoIdentificacion,
                    IdComprobantePdf = pdf.IdComprobantePdf
                });
                foreach (var conceptoPdf in conceptosAddenda)
                {
                    db.ConceptoPdfAddenda.AddObject(conceptoPdf);
                }
            }


            //TODO GUARDAR CONCEPTOS CARTA PORTE
            if (comprobante.ConceptosCartasPortes != null)
            {
                foreach (var cartaPorte in comprobante.ConceptosCartasPortes)
                {
                    //??
                    cartaPorte.idComprobantePdf = pdf.IdComprobantePdf;
                    db.ConceptosCartaPorte.AddObject(cartaPorte);
                }

            }


            db.SaveChanges();
            return pdf;
        }

        //--------------------------------------------------------------------------------------
        public Byte[] GetPdfFromComprobante(Comprobante comprobante, int orientacion, TipoDocumento tipo)
        {
            try
            {
                ReportService.ReportingService2005 clt = new ReportingService2005();
                string userName = ConfigurationManager.AppSettings["RSUserName"];
                string password = ConfigurationManager.AppSettings["RSPass"];
                string url = ConfigurationManager.AppSettings["RSUrlService"];
                //clt.Credentials = System.Net.CredentialCache.DefaultCredentials;
                clt.Credentials = new NetworkCredential(userName, password);
                clt.Url = url;
                CatalogItem[] cats = clt.ListChildren("/", true);
                var rep = GetRutaPdf(tipo);
                var reporte = cats.FirstOrDefault(p => p.Name == comprobante.Emisor.Rfc + "_" + rep);
                if (reporte == null)
                    reporte = cats.FirstOrDefault(p => p.Name == rep);

                if (reporte == null)
                {
                    throw new FaultException("No esta configurada la plantilla para este comprobante");
                }
                if (rep == "PdfHonorarios")
                {
                    comprobante.SubTotal = comprobante.Impuestos.TotalImpuestosTrasladados + comprobante.SubTotal;
                }
                var pdf = GuardaReporte(comprobante);
                string xmlData = null;
                if (tipo == TipoDocumento.Nomina)
                {
                    xmlData = comprobante.XmlNomina;
                }
                return GetReport(reporte.Path, pdf.IdEmpresa, pdf.IdComprobantePdf, xmlData);
            }
            catch (Exception ee)
            {
                //System.Diagnostics.Debugger.Launch();
                Logger.Error(ee);
                if (ee.InnerException != null)
                    Logger.Error(ee.InnerException);
                throw;
            }

        }

        private ComprobantePdf GuardaReporte(Comprobante comprobante)
        {
            ComprobantePdf pdf = new ComprobantePdf();
            Type tip = typeof(Comprobante);
            var fields = tip.GetProperties();
            foreach (PropertyInfo propertyInfo in fields)
            {
                try
                {
                    if (propertyInfo.Name == "Emisor")
                    {
                        PrintFields(propertyInfo.PropertyType, "Em", comprobante.Emisor, pdf);
                    }
                    else if (propertyInfo.Name == "Receptor")
                    {
                        PrintFields(propertyInfo.PropertyType, "Re", comprobante.Receptor, pdf);
                    }
                    else if (propertyInfo.Name == "Impuestos")
                    {
                        PrintFields(propertyInfo.PropertyType, "Imp", comprobante.Impuestos, pdf);
                    }
                    else if (propertyInfo.Name == "DatosAduana")
                    {
                        PrintFields(propertyInfo.PropertyType, "Aduana", comprobante.DatosAduana, pdf);
                    }
                    else
                    {
                        var valor = propertyInfo.GetValue(comprobante, null) == null
                                        ? ""
                                        : propertyInfo.GetValue(comprobante, null).ToString();
                        var property = pdf.GetType().GetProperty(propertyInfo.Name);
                        if (property != null)
                        {
                            property.SetValue(pdf, valor, null);
                        }
                    }
                }
                catch (Exception eee)
                {
                    Logger.Error(eee);
                }
            }
            PrintFields(typeof(TimbreFiscalDigital), "timbre", comprobante.Complemento.timbreFiscalDigital, pdf);
            string enteros;
            string decimales;
            string totalLetra = comprobante.Total.ToString();
            if (totalLetra.IndexOf('.') == -1)
            {
                enteros = "0";
                decimales = "0";
            }
            else
            {
                enteros = totalLetra.Substring(0, totalLetra.IndexOf('.'));
                decimales = totalLetra.Substring(totalLetra.IndexOf('.') + 1);
            }

            string total = enteros.PadLeft(10, '0') + "." + decimales.PadRight(6, '0');

            string cadenaCodigo = "?re=" + comprobante.Emisor.Rfc + "&rr=" + comprobante.Receptor.Rfc + "&tt=" +
                                  total + "&id=" +
                                  comprobante.Complemento.timbreFiscalDigital.UUID.ToUpper();

            byte[] bm = GetQrCode(cadenaCodigo);
            pdf.QrCode = bm;
            string logoEmpresa = Path.Combine(ConfigurationManager.AppSettings["Resources"],
                                              comprobante.Emisor.Rfc, "Logo.png");
            if (!File.Exists(logoEmpresa))
            {
                logoEmpresa = Path.Combine(ConfigurationManager.AppSettings["Resources"], "LogoGenerico.png");
            }

            var db = new NtLinkLocalServiceEntities();
            var empresa = db.empresa.FirstOrDefault(p => p.RFC == comprobante.Emisor.Rfc);
            if (empresa != null && empresa.Logo == null)
            {
                empresa.Logo = File.ReadAllBytes(logoEmpresa);
                db.empresa.ApplyCurrentValues(empresa);

            }
            pdf.IdEmpresa = empresa.IdEmpresa;
            db.ComprobantePdf.AddObject(pdf);
            db.SaveChanges();
            var conceptos = comprobante.Conceptos.Select(p => new ConceptoPdf
            {
                timbre_UUID = pdf.timbre_UUID,
                cantidad = p.Cantidad.ToString(),
             //   CuentaPredial = p.CuentaPredial,
                importe = p.Importe.ToString(),
                valorUnitario = p.ValorUnitario.ToString(),
                descripcion = p.Descripcion,
                Detalles = p.Detalles,
                unidad = p.Unidad,
                OrdenCompra = p.OrdenCompra,
                noIdentificacion = p.NoIdentificacion,
                IdComprobantePdf = pdf.IdComprobantePdf
            });
            foreach (ConceptoPdf conceptoPdf in conceptos)
            {
                db.ConceptoPdf.AddObject(conceptoPdf);
            }
            if (comprobante.ConceptosAduana != null)
            {
                var conceptosAddenda = comprobante.ConceptosAduana.Select(p => new ConceptoPdfAddenda()
                {
                    timbre_UUID = pdf.timbre_UUID,
                    cantidad = p.Cantidad.ToString(),
                   // CuentaPredial = p.CuentaPredial,
                    importe = p.Importe.ToString(),
                    valorUnitario = p.ValorUnitario.ToString(),
                    descripcion = p.Descripcion,
                    Detalles = p.Detalles,
                    unidad = p.Unidad,
                    OrdenCompra = p.OrdenCompra,
                    noIdentificacion = p.NoIdentificacion,
                    IdComprobantePdf = pdf.IdComprobantePdf
                });
                foreach (var conceptoPdf in conceptosAddenda)
                {
                    db.ConceptoPdfAddenda.AddObject(conceptoPdf);
                }
            }


            //TODO GUARDAR CONCEPTOS CARTA PORTE
            if (comprobante.ConceptosCartasPortes != null)
            {
                foreach (var cartaPorte in comprobante.ConceptosCartasPortes)
                {
                    //??
                    cartaPorte.idComprobantePdf = pdf.IdComprobantePdf;
                    db.ConceptosCartaPorte.AddObject(cartaPorte);
                }

            }


            db.SaveChanges();
            return pdf;
        }

        private byte[] GetReport(string report, int empresa, long idPdf, string xmlData)
        {
            Logger.Debug(report + "-" + empresa + "-" + idPdf);
            ReportExecutionService rs = new ReportExecutionService();
            string userName = ConfigurationManager.AppSettings["RSUserName"];
            string password = ConfigurationManager.AppSettings["RSPass"];
            string url = ConfigurationManager.AppSettings["RSUrlExec"];
            rs.Credentials = new NetworkCredential(userName, password);
            rs.Url = url;
            //rs.Credentials = System.Net.CredentialCache.DefaultCredentials;
            string reportPath = report;//"/ReportesNtLink/Pdf";
            string format = "Pdf";
            string devInfo = @"<DeviceInfo> <OutputFormat>PDF</OutputFormat> </DeviceInfo>";
            int parametros = 0;
            if (xmlData == null)
                parametros = 2;
            else parametros = 3;
            ParameterValue[] parameters = new ParameterValue[parametros];
            parameters[0] = new ParameterValue();
            parameters[0].Name = "Empresa";
            parameters[0].Value = empresa.ToString();
            parameters[1] = new ParameterValue();
            parameters[1].Name = "IdPdf";
            parameters[1].Value = idPdf.ToString();
            if (xmlData != null)
            { 
                parameters[2] = new ParameterValue();
                parameters[2].Name = "XmlData";
                parameters[2].Value = xmlData;
                
            }
            //DataSourceCredentials creds = new DataSourceCredentials();

            ////Quitar hardcodeado de base de datos
            //creds.DataSourceName = "DSGAF";
            //creds.UserName = "Admin";
            //creds.Password = "99300055";
            //rs.SetExecutionCredentials(new[] { creds });
            ExecutionHeader execHeader = new ExecutionHeader();
            rs.Timeout = 300000;
            rs.ExecutionHeaderValue = execHeader;
            string historyId = null;
            rs.LoadReport(reportPath, historyId);
            rs.SetExecutionParameters(parameters, "en-US");

            try
            {
                string mimeType;
                string encoding;
                string fileNameExtension;
                Warning[] warnings;
                string[] streamIDs;
                var res = rs.Render(format, devInfo, out fileNameExtension, out mimeType, out encoding, out warnings, out streamIDs);
                Logger.Debug(res.Length);
                return res;
            }
            catch (Exception ee)
            {
                Logger.Error(ee);
                if (ee.InnerException != null)
                    Logger.Error(ee.InnerException);
                return null;
            }
        }




        private string GetRutaPdf(TipoDocumento tipo)
        {
            string ruta = null;
            ruta = "Pdf";
            if (tipo == TipoDocumento.FacturaTransportista)
                ruta = "PdfTransportista";
            else if (tipo == TipoDocumento.FacturaAduanera)
                ruta = "Aduanera";
            else if (tipo == TipoDocumento.Referencia)
                ruta = "PdfReferencia";
            else if (tipo == TipoDocumento.ReciboHonorarios || tipo == TipoDocumento.Arrendamiento)
                ruta = "PdfHonorarios";
            else if (tipo == TipoDocumento.FacturaGeneralFirmas)
                ruta = "PdfFirmas";
            else if (tipo == TipoDocumento.ConstructorFirmas)
                ruta = "ConstructorFirmas";
            else if (tipo == TipoDocumento.Constructor)
                ruta = "Constructor";
            else if (tipo == TipoDocumento.ConstructorFirmasCustom)
                ruta = "ConstructorFirmasCustom";
            else if (tipo == TipoDocumento.FacturaLiverpool)
                ruta = "FacturaLiverpool";
            else if (tipo == TipoDocumento.FacturaMabe)
                ruta = "FacturaMabe";
            else if (tipo == TipoDocumento.FacturaDeloitte)
                ruta = "FacturaDeloitte";
            else if (tipo == TipoDocumento.FacturaSorianaCEDIS)
                ruta = "FacturaSorianaCEDIS";
            else if (tipo == TipoDocumento.FacturaSorianaTienda)
                ruta = "FacturaSorianaTienda";
            else if (tipo == TipoDocumento.FacturaAdo)
                ruta = "FacturaAdo";
            else if (tipo == TipoDocumento.CorporativoAduanal)
                ruta = "CorporativoAduanal";
            else if (tipo == TipoDocumento.FacturaLucent)
                ruta = "PdfLucent";
            else if (tipo == TipoDocumento.CartaPorte)
                ruta = "PdfCartaPorte";
            else if (tipo == TipoDocumento.Nomina)
                ruta = "PdfNomina";
            return ruta;

        }

        private string GetRutaPdfCustomizado(Comprobante comprobante, int orientacion, TipoDocumento tipo)
        {
            string ruta = null;
            ruta = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Reportes", comprobante.Emisor.Rfc, orientacion == 0 ?
                "Pdf.rdlc" : "Horizontal.rdlc");

            if (tipo == TipoDocumento.FacturaTransportista)
                ruta = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Reportes", comprobante.Emisor.Rfc, "PdfTransportista.rdlc");
            else if (tipo == TipoDocumento.FacturaAduanera)
                ruta = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Reportes", comprobante.Emisor.Rfc, "Aduanera.rdlc");
            else if (tipo == TipoDocumento.Referencia)
                ruta = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Reportes", comprobante.Emisor.Rfc, "PdfReferencia.rdlc");
            else if (tipo == TipoDocumento.Constructor)
                ruta = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Reportes", comprobante.Emisor.Rfc, "Constructor.rdlc");
            else if (tipo == TipoDocumento.FacturaGeneralFirmas)
                ruta = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Reportes", comprobante.Emisor.Rfc, "PdfFirmas.rdlc");

            else if (tipo == TipoDocumento.ConstructorFirmas)
                ruta = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Reportes", comprobante.Emisor.Rfc, "ConstructorFirmas.rdlc");


            else if (tipo == TipoDocumento.ReciboHonorarios || tipo == TipoDocumento.Arrendamiento)
            {
                ruta = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Reportes", comprobante.Emisor.Rfc, "PdfHonorarios.rdlc");
                comprobante.SubTotal = comprobante.Impuestos.TotalImpuestosTrasladados + comprobante.SubTotal;
            }

            return ruta;
        }


        public static Comprobante GetComprobanteFromString(string xmlContent)
        {
            XmlSerializer ser = new XmlSerializer(typeof(Comprobante));
            StringReader sr = new StringReader(xmlContent);
            object obj = ser.Deserialize(sr);
            var c = obj as Comprobante;

            if (c != null && c.Complemento != null && c.Complemento.Any.Count() > 0)
            {
                var d = c.Complemento.Any.FirstOrDefault(p => p.LocalName == "TimbreFiscalDigital");
                if (d != null)
                {
                    XmlSerializer des = new XmlSerializer(typeof(TimbreFiscalDigital));
                    TimbreFiscalDigital tim = (TimbreFiscalDigital)des.Deserialize(new XmlTextReader(new StringReader(d.OuterXml)));
                    GeneradorCadenasTimbre gcad = new GeneradorCadenasTimbre();
                    var cadenaTimbre = gcad.CadenaOriginal(xmlContent);
                    c.CadenaOriginalTimbre = cadenaTimbre;
                    c.Complemento.timbreFiscalDigital = tim;
                }
                
            }
            return c;
        }

        public static string Firmar(string cadenaOriginal, string rutaLlave, string pass)
        {
            byte[] llave = File.ReadAllBytes(rutaLlave);
            if (File.Exists(rutaLlave + ".pem"))
            {
                rutaLlave = rutaLlave + ".pem";
            }
            string ext = Path.GetExtension(rutaLlave);
            //if (string.IsNullOrEmpty(pass))
            //    pass = "12345678a";
            RSACryptoServiceProvider privateKey1 = OpensslKey.DecodePrivateKey(llave, pass, ext);
            UTF8Encoding e = new UTF8Encoding(true);
            byte[] signature = privateKey1.SignData(e.GetBytes(cadenaOriginal), "SHA256");
            string sello256 = Convert.ToBase64String(signature);

            return sello256;

            /*
            byte[] llave = File.ReadAllBytes(rutaLlave);
            if (File.Exists(rutaLlave + ".pem"))
            {
                rutaLlave = rutaLlave + ".pem";
            }
            string ext = Path.GetExtension(rutaLlave);
            RSACryptoServiceProvider rsa = OpensslKey.DecodePrivateKey(llave, pass, ext);
           // HashAlgorithm cryp = new SHA1CryptoServiceProvider();
            HashAlgorithm cryp = new SHA256CryptoServiceProvider(); //sha-2 cambio cfdi3.3
            
            byte[] b = rsa.SignData(Encoding.UTF8.GetBytes(cadenaOriginal), cryp);
            return Convert.ToBase64String(b);
            */


        }


        private readonly XNamespace _ns = "http://www.sat.gob.mx/cfd/3";
        private readonly XNamespace _donat = "http://www.sat.gob.mx/cfd/donat";

        private string ConcatenaTimbre(XElement entrada, string xmlTimbre, string xmlDonat, string xmlAddenda, bool addendaRepetida)
        {
            XElement timbre = XElement.Load(new StringReader(xmlTimbre));
            var complemento = entrada.Elements(_ns + "Complemento").FirstOrDefault();
            if (complemento == null)
            {
                entrada.Add(new XElement(_ns + "Complemento"));
                complemento = entrada.Elements(_ns + "Complemento").FirstOrDefault();
            }
            complemento.Add(timbre);
            if (xmlDonat != null)
            {
                XElement donat = XElement.Load(new StringReader(xmlDonat));
                complemento.Add(donat);
            }
            if (xmlAddenda != null)
            {
                XElement add = XElement.Load(new StringReader(xmlAddenda));
                if (addendaRepetida)
                {
                    entrada.Add(add);
                }
                else
                {
                    entrada.Add(new XElement(_ns + "Addenda"));
                    var addenda = entrada.Elements(_ns + "Addenda").FirstOrDefault();
                    addenda.Add(add);
                }
            }


            MemoryStream mem = new MemoryStream();
            StreamWriter tw = new StreamWriter(mem, Encoding.UTF8);
            //XmlWriter xmlWriter = XmlWriter.Create(tw,
            //                                     new XmlWriterSettings() {Indent = false, Encoding = Encoding.UTF8});
            entrada.Save(tw, SaveOptions.DisableFormatting);
            string xml = Encoding.UTF8.GetString(mem.GetBuffer());
            xml = xml.Substring(xml.IndexOf(Convert.ToChar(60)));
            xml = xml.Substring(0, (xml.LastIndexOf(Convert.ToChar(62)) + 1));
            //xml = xml.Replace("xmlns:donat=\"http://www.sat.gob.mx/donat\"", "");

            return xml;
        }



        public string GetXmlAddenda(object addenda, Type tipoAddenda, string prefijo, string ns)
        {
            XmlSerializer ser;
            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();

            if (string.IsNullOrEmpty(prefijo))
            {
                ser = new XmlSerializer(tipoAddenda, ns);
            }
            else
            {
                ser = new XmlSerializer(tipoAddenda);
                namespaces.Add(prefijo, ns);
            }

            try
            {
                using (MemoryStream memStream = new MemoryStream())
                {
                    var sw = new StreamWriter(memStream, Encoding.UTF8);
                    using (
                        XmlWriter xmlWriter = XmlWriter.Create(sw,
                                                               new XmlWriterSettings() { Indent = false, Encoding = Encoding.UTF8 }))
                    {
                        if (namespaces.Count > 0)
                            ser.Serialize(xmlWriter, addenda, namespaces);
                        else
                        {
                            ser.Serialize(xmlWriter, addenda);
                        }
                        string xml = Encoding.UTF8.GetString(memStream.GetBuffer());
                        xml = xml.Substring(xml.IndexOf(Convert.ToChar(60)));
                        xml = xml.Substring(0, (xml.LastIndexOf(Convert.ToChar(62)) + 1));
                        return xml;
                    }
                }
            }
            catch (Exception ee)
            {

                Logger.Error(ee);
                return null;
            }
        }

        public string GetXmlAddendaDeloitte(AddendaDeloitte addenda)
        {
            XmlSerializer ser = new XmlSerializer(typeof(AddendaDeloitte));
            try
            {
                using (MemoryStream memStream = new MemoryStream())
                {
                    var sw = new StreamWriter(memStream, Encoding.UTF8);
                    using (
                        XmlWriter xmlWriter = XmlWriter.Create(sw,
                                                               new XmlWriterSettings() { Indent = false, Encoding = Encoding.UTF8 }))
                    {
                        XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
                        namespaces.Add("del", "http://www.deloitte.com/CFD/Addenda/Receptor");
                        ser.Serialize(xmlWriter, addenda, namespaces);
                        string xml = Encoding.UTF8.GetString(memStream.GetBuffer());
                        xml = xml.Substring(xml.IndexOf(Convert.ToChar(60)));
                        xml = xml.Substring(0, (xml.LastIndexOf(Convert.ToChar(62)) + 1));
                        //xml = xml.Replace("xmlns:donat=\"http://www.sat.gob.mx/donat\"", "");
                        return xml;
                    }
                }
            }
            catch (Exception ee)
            {

                Logger.Error(ee);
                return null;
            }
        }


        public string GetXmlDonat(Donatarias donat)
        {
            XmlSerializer ser = new XmlSerializer(typeof(Donatarias));
            try
            {
                using (MemoryStream memStream = new MemoryStream())
                {
                    var sw = new StreamWriter(memStream, Encoding.UTF8);
                    using (
                        XmlWriter xmlWriter = XmlWriter.Create(sw,
                                                               new XmlWriterSettings() { Indent = false, Encoding = Encoding.UTF8 }))
                    {
                        XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
                        namespaces.Add("donat", "http://www.sat.gob.mx/donat");
                        ser.Serialize(xmlWriter, donat, namespaces);
                        string xml = Encoding.UTF8.GetString(memStream.GetBuffer());
                        xml = xml.Substring(xml.IndexOf(Convert.ToChar(60)));
                        xml = xml.Substring(0, (xml.LastIndexOf(Convert.ToChar(62)) + 1));
                        //xml = xml.Replace("xmlns:donat=\"http://www.sat.gob.mx/donat\"", "");
                        return xml;
                    }
                }
            }
            catch (Exception ee)
            {

                Logger.Error(ee);
                return null;
            }

        }

        public void TimbrarComprobanteNtLink(Comprobante comp)
        {
            bool AddendaRepetida = false;
            ClienteTimbradoNtlink cliente = new ClienteTimbradoNtlink();
            try
            {
                //string complemento = null;
                Logger.Debug("Timbrando comprobante");
                

                XmlSerializer ser = new XmlSerializer(typeof(TimbreFiscalDigital));
                

                //var str = GetXml(comp, complemento);
                string timbreString = cliente.TimbraCfdi(comp.XmlString);
                Logger.Debug(timbreString);
                TimbreFiscalDigital timbre = null;
                try
                {
                    timbre = (TimbreFiscalDigital)ser.Deserialize(new XmlTextReader(new StringReader(timbreString)));

                }
                catch (Exception ee)
                {
                    Logger.Error(ee);
                    throw new FaultException(timbreString);
                }
                if (timbreString == null)
                {
                    throw new Exception("Ocurrió un error en el timbrado");
                }
                GeneradorCadenasTimbre generadorCadenasTimbre = new GeneradorCadenasTimbre();
                comp.CadenaOriginalTimbre = generadorCadenasTimbre.CadenaOriginal(timbreString);
                string addendaXml = null;

                if (comp.TipoAddenda == TipoAddenda.Deloitte)
                {
                    comp.xsiSchemaLocation = comp.xsiSchemaLocation +
                                             " http://www.deloitte.com/CFD/Addenda/Receptor http://www.pegasotecnologia.com/secfd/schemas/receptor/Deloitte_recepcion.xsd";
                    AddendaDeloitte addenda = new AddendaDeloitte()
                    {
                        mailContactoDeloitte = comp.correocontacto,
                        mailProveedor = comp.correoproveedor,
                        moneda = comp.Tipomoneda,
                        noPedido = comp.nopedido,
                        nombreContactoProveedor = comp.nombreproveedor,
                        numeroProveedor = comp.noproveedor,
                        oficina = comp.oficina,
                        origenFactura = comp.origendefactura

                    };
                    addendaXml = GetXmlAddendaDeloitte(addenda);
                }


                else if (comp.TipoAddenda == TipoAddenda.SorianaCedis)
                {
                    comp.xsiSchemaLocation = comp.xsiSchemaLocation +
                                             " http://www.buzonfiscal.com/ns/addenda/bf/2 http://www.buzonfiscal.com/schema/xsd/Addenda_BF_v20.xsd";
                    // Codigo para addenda Soriana Entrega a CEDIS
                    AddendaBuzonFiscalType addenda = new AddendaBuzonFiscalType
                    {
                        Emisor = new EmisorType() { telefono = comp.Telefono },
                        Receptor =
                            new ReceptorType { noProveedor = comp.noproveedor, GLN = comp.Gln },
                        TipoDocumento =
                            new TipoDocumentoType()
                            {
                                descripcion = DescripcionType.Factura,
                                nombreCorto = NombreCortoType.FAC
                            },
                        CFD = new CFDType()
                        {
                            tipoMoneda = comp.MonedatipoMoneda,
                             tipoCambio = Convert.ToDecimal((comp.TipoCambio)),
                            tipoCambioSpecified = true,
                            observaciones = comp.Proyecto,
                            totalConLetra = comp.CantidadLetra
                            
                        },
                        Extra = new ExtraType[]
                                                                         {
                                                                             new ExtraType()
                                                                                 {
                                                                                     valor = comp.Total.ToString(),
                                                                                     atributo = comp.ExtraAtributo
                                                                                 },
                                                                             new ExtraType()
                                                                                 {
                                                                                     valor = comp.Valor1,
                                                                                     atributo = comp.ExtraAtributo1
                                                                                 }
                                                                         }
                                                                         
                    };
                    addendaXml = GetXmlAddenda(addenda, typeof(AddendaBuzonFiscalType), "bfa2", "http://www.buzonfiscal.com/ns/addenda/bf/2");
                }

               //Addenda ADO
                else if (comp.TipoAddenda == TipoAddenda.ADO)
                {
                    
                    Addenda addenda = new Addenda
                    {
                        proveedor = new AddendaProveedor { tipoAddenda = comp.Proveedor },
                        addenda = new AddendaAddenda { valor = comp.Valor }
                    };
                    AddendaRepetida = true;
                    addendaXml = GetXmlAddenda(addenda, typeof(Addenda), "cfdi", "http://recepcioncfd.ekomercio.com/ADO");
                }
                
                    ///termina ADO
                else if (comp.TipoAddenda == TipoAddenda.SorianaTienda)
                {
                    comp.xsiSchemaLocation = comp.xsiSchemaLocation +
                                             " http://www.visual-tech.mx/Apps/v-Fact/Addendas/Emisor___Proalimex/Receptor___Soriana http://www.visual-tech.mx/Apps/v-Fact/Addendas/Emisor___Proalimex/Receptor___Soriana/Addenda_Soriana.xsd";
                    DSCargaRemisionProvRemision addendaRemision = new DSCargaRemisionProvRemision
                    {
                        RowOrder = "0",
                        Id = "Remision0",
                        Proveedor = Convert.ToInt32(comp.ProveedorRemision),
                        Remision = comp.RemisionR,
                        Consecutivo = short.Parse(comp.Consecutivo),
                        FechaRemision = Convert.ToDateTime(comp.FechaRemision),
                        Tienda = short.Parse(comp.TiendaRemision),
                        TipoMoneda = short.Parse(comp.MonedatipoMoneda),
                        TipoBulto = short.Parse(comp.TipoBulto),
                        EntregaMercancia = short.Parse(comp.EntrgaMercancia),
                        CumpleReqFiscales = Convert.ToBoolean((comp.CumpleReqFiscal)),
                        CantidadBultos = short.Parse(comp.CantidadBultos),
                        Subtotal = comp.SubTotal,
                        IEPS = comp.IEPS,
                        IVA = comp.IVA,
                        OtrosImpuestos = Convert.ToDecimal(comp.OtrosImpuestos),
                        Total = comp.Total,
                        CantidadPedidos = Convert.ToInt32(comp.CantidadPedidos),
                        FechaEntregaMercancia = Convert.ToDateTime(comp.FechaEntrgaMercancia),
                        FolioNotaEntrada = comp.FolioNotaEntrada,
                       FolioNotaEntradaSpecified = true //para mostrar folio nota de entrada se agrgo este 
                    };

                    DSCargaRemisionProvPedidos addendaPedido = new DSCargaRemisionProvPedidos
                    {
                        RowOrder = "0",
                        Id = "Pedidos0",
                       
                        Proveedor = Convert.ToInt32(comp.ProveedorRemision),
                        Remision = comp.RemisionR,
                      FolioPedido = comp.FolioNotaEntrada,
                        Tienda = short.Parse(comp.TiendaRemision),
                       CantidadArticulos = Convert.ToInt32(comp.CantidadArticulos)
                    };
                    
                    List<DSCargaRemisionProvArticulos> addendaArticulo = new List<DSCargaRemisionProvArticulos>();

                    int h = 0;
                        foreach (var articulo in comp.Conceptos)

                        {
                            
                            addendaArticulo.Add(new DSCargaRemisionProvArticulos
                                                    {
                                                     
                                                        
                                                        RowOrder = Convert.ToString(h.ToString()),
                                                        Id = "Aritculos"+Convert.ToString(h.ToString()),
                                                        Remision = comp.RemisionR,
                                                        Proveedor =Convert.ToInt32(comp.ProveedorRemision),
                                                        FolioPedido = comp.FolioNotaEntrada,//Convert.ToInt32(articulo.folioPedido), 
                                                        Tienda = short.Parse(comp.TiendaRemision),
                                                        Codigo = Decimal.Parse(articulo.NoIdentificacion),
                                                        CantidadUnidadCompra= articulo.Cantidad,
                                                        CostoNetoUnidadCompra = articulo.ValorUnitario,
                                                        PorcentajeIEPS = comp.PorcentajeIEPS,
                                                        PorcentajeIVA =comp.PorecentajeIVA
                                                    });
                            h++;
                        }

                    DSCargaRemisionProv addenda = new DSCargaRemisionProv();

                    
                    int index = addendaArticulo.Count + 2;
                    addenda.Items = new object[index];
                    for (int i = 0; i < index; i++)
                    {
                        if (i == 0)
                        {
                            addenda.Items[i] = addendaRemision;
                        }
                        else if (i == 1)
                        {
                            addenda.Items[i] = addendaPedido;
                        }
                        else
                        {
                            addenda.Items[i] = addendaArticulo[i - 2];
                        }
                    }

                    addendaXml = GetXmlAddenda(addenda, typeof(DSCargaRemisionProv), "", "http://www.visual-tech.mx/Apps/v-Fact/Addendas/Emisor___Proalimex/Receptor___Soriana");
                 }
                // Addenda PEMEX -- SZ
                else if (comp.TipoAddenda == TipoAddenda.Pemex)
                {
                    comp.xsiSchemaLocation = comp.xsiSchemaLocation + " http://pemex.com/facturaelectronica/addenda/v2 https://pemex.reachcore.com/schemas/addenda-pemex-v2.xsd";
                    AddendaPemex addendaPemex = comp.AddendaPemex;
                    addendaXml = GetXmlAddenda(addendaPemex, typeof(AddendaPemex), "pm", "http://pemex.com/facturaelectronica/addenda/v2");
                }
                else if (comp.TipoAddenda == TipoAddenda.Lucent)
                {

                    XDocument doc = new XDocument(new XElement("ItemsFacturados"));
                    foreach (var c in comp.Conceptos)
                    {
                        XElement el = new XElement("OrdenCompra");
                        el.Value = comp.LucentOrdenCompra;
                        XElement el2 = new XElement("Item");
                        el2.Value = c.Descripcion;// .Detalles;
                        doc.Elements().First().Add(el);
                        doc.Elements().First().Add(el2);
                    }
                    addendaXml = doc.ToString(SaveOptions.OmitDuplicateNamespaces);
                    comp.DonatLeyenda = comp.LucentOrdenCompra;
                    
                }
                
                
                if (comp.AddendaAmece != null)
                {

                    requestForPayment addendaAmece = comp.AddendaAmece;
                    addendaXml = GetXmlAddenda(addendaAmece, typeof(requestForPayment), null, null);
                }
                if (comp.AddendaCoppelObj != null)
                {
                    comp.AddendaCoppelObj.requestForPayment.cadenaOriginal = new AddendaRequestForPaymentCadenaOriginal()
                                                                                 {
                                                                                     Cadena = comp.CadenaOriginal
                                                                                 };
                    addendaXml = GetXmlAddenda(comp.AddendaCoppelObj.requestForPayment,
                                               typeof (AddendaRequestForPayment), null, null);
                }

                string cfdiString = comp.XmlString;
                StringReader sr = new StringReader(cfdiString);
                XElement element = XElement.Load(sr);
                
               

                string xmlFinal = ConcatenaTimbre(element, timbreString, null, addendaXml, AddendaRepetida);
                comp.Complemento = new ComprobanteComplemento() { timbreFiscalDigital = timbre };
                comp.XmlString = xmlFinal;
            }
            catch (FaultException fe)
            {
                Logger.Info(fe);
                throw;
            }
            catch (SoapException exception)
            {
                Logger.Error(exception.Detail.InnerText.Trim());
                throw new ApplicationException("Error al timbrar el comprobante:" + exception.Detail.InnerText.Trim(), exception);
            }
            catch (Exception exception)
            {
                Logger.Error((exception.InnerException == null ? exception.Message : exception.InnerException.Message));
                throw new Exception("Error al timbrar el comprobante", exception);
            }


        }

        public void TimbrarComprobantePreview(Comprobante comp)
        {
            ClienteTimbradoNtlink cliente = new ClienteTimbradoNtlink();
            try
            {
                XmlSerializer ser = new XmlSerializer(typeof(TimbreFiscalDigital));
                TimbreFiscalDigital timbre = null;
                try
                {
                    timbre = new TimbreFiscalDigital()
                    {
                        UUID = "No Timbrado",
                        FechaTimbrado = DateTime.Now,
                        NoCertificadoSAT = "000",
                        SelloCFD = comp.Sello,
                        SelloSAT = "Inválido",
                        Version = "1.0"
                    };

                }
                catch (Exception ee)
                {
                    Logger.Error(ee);
                }
                GeneradorCadenasTimbre generadorCadenasTimbre = new GeneradorCadenasTimbre();
                comp.CadenaOriginalTimbre = "Inválido";
                string cfdiString = GetXml(comp,null);
                StringReader sr = new StringReader(cfdiString);
                var sw = new StringWriter();
                XElement element = XElement.Load(sr);

                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Encoding = new UnicodeEncoding(false, false); // no BOM in a .NET string
                settings.Indent = false;
                settings.OmitXmlDeclaration = false;
                XmlWriter xmlWriter = XmlWriter.Create(sw, settings);
                ser.Serialize(xmlWriter, timbre);
                string xmlFinal = ConcatenaTimbre(element, sw.ToString(), null, null, false);

                comp.Complemento = new ComprobanteComplemento() { timbreFiscalDigital = timbre };
                if (comp.Nomina != null)
                {
                    var complemento = GetXmlAddenda(comp.Nomina, typeof(Nomina), "nomina", "http://www.sat.gob.mx/nomina");
                    comp.XmlNomina = complemento;
                }

                comp.XmlString = xmlFinal;

            }
            catch (FaultException fe)
            {
                throw;
            }
            catch (SoapException exception)
            {
                Logger.Error(exception.Detail.InnerText.Trim());
                throw new ApplicationException("Error al timbrar el comprobante:" + exception.Detail.InnerText.Trim(), exception);
            }
            catch (Exception exception)
            {
                Logger.Error((exception.InnerException == null ? exception.Message : exception.InnerException.Message));
                throw new Exception("Error al timbrar el comprobante", exception);
            }


        }

        public void GenerarCfdPreview(Comprobante comprobante, X509Certificate2 cert, string rutaLlave, string passLlave)
        {
            try
            {
                comprobante.Certificado = Convert.ToBase64String(cert.RawData);
                comprobante.NoCertificado = NoCert(cert.SerialNumber);
                GeneradorCadenas gen = new GeneradorCadenas();
                string comp = GetXml(comprobante, null);
                comprobante.XmlString = comp;
                comprobante.CadenaOriginal = gen.CadenaOriginal(comp);
                comprobante.Sello = Firmar(comprobante.CadenaOriginal, rutaLlave, passLlave);
                TimbrarComprobantePreview(comprobante);
            }
            catch (FaultException fe)
            {
                Logger.Error(fe);
                throw;
            }
            catch (Exception exception)
            {
                Logger.Error((exception.InnerException == null ? exception.Message : exception.InnerException.Message));
                throw;
            }
        }


        public void GenerarCfd(Comprobante comprobante, X509Certificate2 cert, string rutaLlave, string passLlave)
        {
            try
            {
                Logger.Debug("Generando xml");
                comprobante.Certificado = Convert.ToBase64String(cert.RawData);
                comprobante.NoCertificado = NoCert(cert.SerialNumber);
                string complemento = null;
                GeneradorCadenas gen = new GeneradorCadenas();

                if (comprobante.Nomina != null)
                {
                    comprobante.xsiSchemaLocation = comprobante.xsiSchemaLocation + "http://www.sat.gob.mx/nomina http://www.sat.gob.mx/sitio_internet/cfd/nomina/nomina11.xsd";
                    complemento = GetXmlAddenda(comprobante.Nomina, typeof(Nomina), "nomina", "http://www.sat.gob.mx/nomina");
                    comprobante.XmlNomina = complemento;
                }
                if (comprobante.Complemento != null && comprobante.Complemento.Donat != null)
                {

                    complemento = GetXmlDonat(comprobante.Complemento.Donat);
                }
                string comp = GetXml(comprobante, complemento);
                
                comprobante.CadenaOriginal = gen.CadenaOriginal(comp);
                comprobante.Sello = Firmar(comprobante.CadenaOriginal, rutaLlave, passLlave);
                XElement xeComprobante = XElement.Parse(comp);
                xeComprobante.Add(new XAttribute("sello", comprobante.Sello));
                SidetecStringWriter sw = new SidetecStringWriter(Encoding.UTF8);
                xeComprobante.Save(sw,SaveOptions.DisableFormatting);
                comprobante.XmlString = sw.ToString();
                if (ConfigurationManager.AppSettings["Pac"] == "NtLink")
                {
                    TimbrarComprobanteNtLink(comprobante);
                }
                else throw new Exception("No hay un pac configurado");
            }
            catch (FaultException fe)
            {
                Logger.Error(fe);
                throw;
            }
            catch (Exception exception)
            {
                Logger.Error((exception.InnerException == null ? exception.Message : exception.InnerException.Message));
                throw;
            }
        }
        //---------------------------------------------------------------------
        public string GenerarCfdSinTimbre(Comprobante comprobante, X509Certificate2 cert, string rutaLlave, string passLlave,string comp)
        {
            try
            {
                Logger.Debug("Generando xml");
                comprobante.Certificado = Convert.ToBase64String(cert.RawData);
                comprobante.NoCertificado = NoCert(cert.SerialNumber);
                string complemento = null;
                
                GeneradorCadenas gen = new GeneradorCadenas();
                /*
                if (comprobante.Nomina != null)
                {
                    comprobante.xsiSchemaLocation = comprobante.xsiSchemaLocation + "http://www.sat.gob.mx/nomina12 http://www.sat.gob.mx/sitio_internet/cfd/nomina/nomina12.xsd";
                    complemento = GetXmlAddenda(comprobante.Nomina, typeof(Nomina), "nomina12", "http://www.sat.gob.mx/nomina12");
                    comprobante.XmlNomina = complemento;
                }
                if (comprobante.Complemento != null && comprobante.Complemento.Donat != null)
                {

                    complemento = GetXmlDonat(comprobante.Complemento.Donat);
                }
                 
                string comp = GetXml(comprobante, complemento);
                */
                XElement xeComprobante = XElement.Parse(comp);
                xeComprobante.Add(new XAttribute("Certificado", comprobante.Certificado));
                xeComprobante.Add(new XAttribute("NoCertificado", comprobante.NoCertificado));
                SidetecStringWriter sw = new SidetecStringWriter(Encoding.UTF8);
                xeComprobante.Save(sw, SaveOptions.DisableFormatting);

                XElement xeComprobante2 = XElement.Parse(sw.ToString());
                SidetecStringWriter sw2 = new SidetecStringWriter(Encoding.UTF8);
                
                comprobante.CadenaOriginal = gen.CadenaOriginal(sw.ToString());
                comprobante.Sello = Firmar(comprobante.CadenaOriginal, rutaLlave, passLlave);
                xeComprobante2.Add(new XAttribute("Sello", comprobante.Sello));
               // SidetecStringWriter sw = new SidetecStringWriter(Encoding.UTF8);
                xeComprobante2.Save(sw2, SaveOptions.DisableFormatting);
                comprobante.XmlString = sw2.ToString();
                return comprobante.XmlString;
            }
            catch (FaultException fe)
            {
                Logger.Error(fe);
                return null;
                throw;
            }
            catch (Exception exception)
            {
                Logger.Error((exception.InnerException == null ? exception.Message : exception.InnerException.Message));
                return null;
                throw;
            }
        }

        //-------------------------------------------------------------------------------
        private string NoCert(string cert)
        {
            int count = 0;
            StringBuilder sb = new StringBuilder();
            foreach (char c in cert)
            {
                if (count % 2 != 0)
                    sb.Append(c);
                count++;
            }
            return sb.ToString();
        }

    }
}
