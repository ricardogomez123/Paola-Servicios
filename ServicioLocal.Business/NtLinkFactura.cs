using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using MessagingToolkit.QRCode.Codec;
using ServicioLocal.Business.ReportExecution;
using ServicioLocal.Business.ReportService;
using ServicioLocalContract;
using ParameterValue = ServicioLocal.Business.ReportService.ParameterValue;
using Warning = ServicioLocal.Business.ReportService.Warning;


namespace ServicioLocal.Business
{
    public class NtLinkFactura : NtLinkBusiness
    {
        public string Uuid { get; set; }
        private facturas _factura;
        public Comprobante Cfdi { get; set; }


        public List<facturasdetalle> Detalles { get; set; }

        public facturas Factura
        {
            get { return _factura; }
            set { _factura = value; }
        }

        public clientes Receptor { get; set; }

        public empresa Emisor { get; set; }



        public static byte[] GetXmlData(string uuid)
        {
            return GetData(uuid, "xml");
        }

        public static byte[] GetPdfData(string uuid)
        {
            return GetData(uuid, "pdf");
        }



        public static byte[] GetData(string uuid, string tipo)
        {
            try
            {
                using (var db = new NtLinkLocalServiceEntities())
                {
                    var venta = db.facturas.Where(p => p.Uid == uuid).FirstOrDefault();
                    if (venta == null)
                    {
                        Logger.Error("No se encontró la factura: " + uuid);
                        return null;
                    }
                    var empresa = db.empresa.Where(p => p.IdEmpresa == venta.IdEmpresa).FirstOrDefault();
                    if (empresa == null)
                    {
                        Logger.Error("No se encontró la factura: " + uuid);
                        return null;
                    }
                    string ruta = Path.Combine(ConfigurationManager.AppSettings["Salida"], empresa.RFC);
                    if (File.Exists(Path.Combine(ruta, uuid + ".xml")))
                    {
                        var bytes = File.ReadAllBytes(Path.Combine(ruta, uuid + "." + tipo));
                        return bytes;
                    }
                    else
                    {
                        Logger.Error("No se encontró la factura: " + uuid);
                        return null;
                    }

                }
            }
            catch (Exception ee)
            {
                Logger.Error(ee.Message);
                return null;
            }
        }


        public static Comprobante GeneraCfd(NtLinkFactura factura, bool enviar)
        {
            try
            {
                
                Logger.Debug(factura.Emisor.RFC);
                empresa emp = factura.Emisor;
                clientes cliente = factura.Receptor;
                if (string.IsNullOrEmpty(cliente.RFC))
                {
                    Logger.Error("El rfc es erróneo " + cliente.RFC);
                    throw new ApplicationException("El rfc es erróneo");
                }
                string path = Path.Combine(ConfigurationManager.AppSettings["Resources"], emp.RFC, "Certs");
                X509Certificate2 cert = new X509Certificate2(Path.Combine(path, "csd.cer"));
                string rutaLlave = Path.Combine(path, "csd.key");
                Logger.Debug("Ruta Llave " + rutaLlave);
                var comprobante = GetComprobante(factura, cliente, emp);
                GeneradorCfdi gen = new GeneradorCfdi();

                /*Agregar si hay articulos*/
                comprobante.articulos = factura.Detalles;
                /**/


                gen.GenerarCfd(comprobante, cert, rutaLlave, emp.PassKey);
                string ruta = Path.Combine(ConfigurationManager.AppSettings["Salida"], emp.RFC);
                if (!Directory.Exists(ruta))
                    Directory.CreateDirectory(ruta);
                string xmlFile = Path.Combine(ruta, comprobante.Complemento.timbreFiscalDigital.UUID + ".xml");
                Logger.Debug(comprobante.XmlString);
                StreamWriter sw = new StreamWriter(xmlFile, false,Encoding.UTF8);
                
                sw.Write(comprobante.XmlString);

                sw.Close();
                byte[] pdf = new byte[0];

                try
                {
                    pdf = gen.GetPdfFromComprobante(comprobante, emp.Orientacion, factura.Factura.TipoDocumento);
                    string pdfFile = Path.Combine(ruta, comprobante.Complemento.timbreFiscalDigital.UUID + ".pdf");
                    File.WriteAllBytes(pdfFile, pdf);

                }
                catch (Exception ee)
                {
                    Logger.Error(ee);
                    if (ee.InnerException != null)
                        Logger.Error(ee.InnerException);
                }
                if (enviar)
                {
                    try
                    {
                        Logger.Debug("Enviar Correo");
                        byte[] xmlBytes = Encoding.UTF8.GetBytes(comprobante.XmlString);
                        var atts = new List<EmailAttachment>();
                        atts.Add(new EmailAttachment
                                     {
                                         Attachment = xmlBytes,
                                         Name = comprobante.Complemento.timbreFiscalDigital.UUID + ".xml"
                                     });
                        atts.Add(new EmailAttachment
                                     {
                                         Attachment = pdf,
                                         Name = comprobante.Complemento.timbreFiscalDigital.UUID + ".pdf"
                                     });
                        Mailer m = new Mailer();
                        if (factura.Receptor.Bcc != null)
                            m.Bcc = factura.Receptor.Bcc;
                        List<string> emails = new List<string>();
                        emails.Add(cliente.Email);

                        m.Send(emails, atts,
                               "Se envia la factura con folio " + comprobante.Complemento.timbreFiscalDigital.UUID +
                               " en formato XML y PDF.",
                               "Envío de Factura", emp.Email, emp.RazonSocial);
                    }
                    catch (Exception ee)
                    {
                        Logger.Error(ee.Message);
                        if (ee.InnerException != null)
                            Logger.Error(ee.InnerException);
                    }
                }
                return comprobante;
            }
            catch (FaultException fe)
            {
                throw;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                if (ex.InnerException != null)
                    Logger.Error(ex.InnerException);
                return null;
            }
        }



        public void EnviarFactura(string rfc, string folioFiscal, List<string> rec, List<string> bcc)
        {
            string ruta = Path.Combine(ConfigurationManager.AppSettings["Salida"], rfc);
            string pdfFile = Path.Combine(ruta, folioFiscal + ".pdf");
            string xmlFile = Path.Combine(ruta, folioFiscal + ".xml");

            if (File.Exists(pdfFile) && File.Exists(xmlFile))
            {
                try
                {
                    using (var db = new NtLinkLocalServiceEntities())
                    {
                        var venta = db.facturas.FirstOrDefault(p => p.Uid == folioFiscal);
                        if (venta == null)
                        {
                            throw new FaultException("No se encontró la factura");
                        }
                        var emp = db.empresa.FirstOrDefault(e => e.IdEmpresa == venta.IdEmpresa);
                        Logger.Debug("Enviar Correo");
                        byte[] xmlBytes = File.ReadAllBytes(xmlFile);
                        var atts = new List<EmailAttachment>();
                        atts.Add(new EmailAttachment
                        {
                            Attachment = xmlBytes,
                            Name = Path.GetFileName(xmlFile)
                        });
                        atts.Add(new EmailAttachment
                        {
                            Attachment = File.ReadAllBytes(pdfFile),
                            Name = Path.GetFileName(pdfFile)
                        });
                        Mailer m = new Mailer();
                        if (bcc != null && bcc.Count > 0)
                        {
                            m.Bcc = bcc[0];
                        }

                        m.Send(rec, atts,
                               "Se envia la factura con folio " + folioFiscal +
                               " en formato XML y PDF.",
                               "Envío de Factura", emp.Email, emp.RazonSocial);
                    }

                }
                catch (FaultException ee)
                {
                    Logger.Error(ee + folioFiscal + " " + rfc);
                    throw;

                }
                catch (Exception ee)
                {
                    Logger.Error(ee.Message);
                    if (ee.InnerException != null)
                        Logger.Error(ee.InnerException);
                }
            }
            else
            {
                throw new FaultException("No se encontró la factura");
            }

        }


        private static Comprobante GetComprobante(NtLinkFactura factura, clientes cliente, empresa emp)
        {
            Logger.Debug(factura.Factura.Folio);
            Comprobante comprobante = new Comprobante();
            comprobante.Emisor = new ComprobanteEmisor();
            comprobante.Emisor.Nombre = emp.RazonSocial;
           /* comprobante.Emisor.RegimenFiscal = new[]
                                                   {
                                                       new ComprobanteEmisorRegimenFiscal
                                                           {Regimen = factura.Factura.Regimen}
                                                   };
            */
            comprobante.Emisor.RegimenFiscal = factura.Factura.Regimen;
            comprobante.Emisor.Rfc = emp.RFC;
          /*  comprobante.Emisor.DomicilioFiscal = new t_UbicacionFiscal();
            comprobante.Emisor.DomicilioFiscal.calle = emp.Direccion;
            comprobante.Emisor.DomicilioFiscal.colonia = emp.Colonia;
            comprobante.Emisor.DomicilioFiscal.codigoPostal = emp.CP;
            comprobante.Emisor.DomicilioFiscal.municipio = emp.Ciudad;//
            comprobante.Emisor.DomicilioFiscal.pais = "México";
            comprobante.Emisor.DomicilioFiscal.estado = emp.Estado;
            */

            comprobante.Titulo = "Factura";
            //comprobante.tipoDeComprobante = ComprobanteTipoDeComprobante.ingreso;
            //comprobante.TipoDeComprobante = c_TipoDeComprobante.I;//cambio obligado
            comprobante.TipoDeComprobante = "I";//cambio obligado
            
            if (factura.Factura.NotaCredito)
            {
               // comprobante.tipoDeComprobante = ComprobanteTipoDeComprobante.egreso;
               // comprobante.TipoDeComprobante = c_TipoDeComprobante.E;//cambio obligado
                 comprobante.TipoDeComprobante = "E";//cambio obligado
                 comprobante.Titulo = "Nota de Crédito";
            }
            if (factura.Factura.TipoDocumento == TipoDocumento.ReciboHonorarios)
            {
                comprobante.Titulo = "Recibo de Honorarios";
            }
            if (factura.Factura.TipoDocumento == TipoDocumento.Arrendamiento)
            {
                comprobante.Titulo = "Recibo de Arrendamiento";
            }
            if (factura.Factura.TipoDocumento == TipoDocumento.CartaPorte)
            {
                comprobante.Titulo = "Carta Porte";
            }
            if (factura.Factura.TipoDocumento == TipoDocumento.Donativo)
            {
                //comprobante.tipoDeComprobante = ComprobanteTipoDeComprobante.ingreso;
               // comprobante.TipoDeComprobante = c_TipoDeComprobante.I;//cambio obligado
                comprobante.TipoDeComprobante = "I";//cambio obligado
               
                comprobante.DonatAprobacion = factura.Factura.DonativoAutorizacion;
                comprobante.DonatFecha = factura.Factura.DonativoFechaAutorizacion.ToString("dd/MM/yyyy");
                comprobante.DonatLeyenda =
                    "Este comprobante ampara un donativo, el cual será destinado por la donataria a los fines propios de su objeto social. En el caso de que los bienes donados hayan sido deducidos previamente para los efectos del impuesto sobre la renta, este donativo no es deducible. La reproducción no autorizada de este comprobante constituye un delito en los términos de las disposiciones fiscales.";

                comprobante.Titulo = "Recibo de Donativo";
            }
            else if (factura.Factura.TipoDocumento == TipoDocumento.Nomina)
            {
                //comprobante.tipoDeComprobante = ComprobanteTipoDeComprobante.egreso;
                //comprobante.TipoDeComprobante = c_TipoDeComprobante.E;//cambio obligado
                comprobante.TipoDeComprobante = "E";//cambio obligado
               
            }

            comprobante.Receptor = new ComprobanteReceptor();
            comprobante.Receptor.Nombre = cliente.RazonSocial;
            comprobante.Receptor.Rfc = cliente.RFC;
            /*
            comprobante.Receptor.Domicilio = new t_Ubicacion();
            comprobante.Receptor.Domicilio.pais = cliente.Pais;
            comprobante.Receptor.Domicilio.calle = cliente.Direccion;
            comprobante.Receptor.Domicilio.municipio = cliente.Ciudad;
            comprobante.Receptor.Domicilio.estado = cliente.Estado;
            comprobante.Receptor.Domicilio.colonia = cliente.Colonia;
            comprobante.Receptor.Domicilio.codigoPostal = cliente.CP;
            */
            comprobante.LugarExpedicion = factura.Factura.LugarExpedicion;
           // comprobante.fecha = Convert.ToDateTime(factura.Factura.Fecha.ToString("s"));
            comprobante.Fecha =factura.Factura.Fecha.ToString("s");
            comprobante.Total = Decimal.Round(factura.Factura.Total.Value, 6);
            if (factura.Factura.Folio != null)
            {
                factura.Factura.Folio = GetNextFolio(factura.Factura.IdEmpresa.Value);
            }
            comprobante.Leyenda = factura.Factura.Leyenda;
            comprobante.LeyendaInferior = emp.LeyendaInferior;
            comprobante.LeyendaSuperior = emp.LeyendaSuperior;
            comprobante.Folio = factura.Factura.Folio;
            comprobante.LugarExpedicion = factura.Factura.LugarExpedicion;
           
            /* if(factura.Factura.Metodo=="PUE")
            comprobante.MetodoPago =c_MetodoPago.PUE ;
            if (factura.Factura.Metodo == "PIP")
                comprobante.MetodoPago = c_MetodoPago.PIP;
            if (factura.Factura.Metodo == "PPD")
                comprobante.MetodoPago = c_MetodoPago.PPD;
            */

            comprobante.MetodoPago = factura._factura.Metodo;

          //  comprobante.NumCtaPago = factura.Factura.Cuenta;
            var moneda = "MXN";
            if (factura.Factura.Moneda == 1)
                moneda = "MXN";
            if (factura.Factura.Moneda == 2)
                moneda = "USD";
            if (factura.Factura.Moneda == 3)
                moneda = "EUR";

            comprobante.Moneda = moneda;
            //comprobante.Regimen = comprobante.Emisor.RegimenFiscal[0].;
            comprobante.SubTotal = Decimal.Round(factura.Factura.SubTotal.Value, 6);// factura.Factura.Total.Value - factura.Factura.IVA.Value + factura.Factura.RetenciónIva;
            comprobante.Serie = factura.Factura.Serie;
            comprobante.FormaPago = factura.Factura.FormaPago;
            comprobante.VoBoNombre = factura.Factura.VoBoNombre;
            comprobante.VoBoPuesto = factura.Factura.VoBoPuesto;
            comprobante.VoBoArea = factura.Factura.VoBoArea;
            comprobante.AutorizoNombre = factura.Factura.AutorizoNombre;
            comprobante.AutorizoPuesto = factura.Factura.AutorizoPuesto;
            comprobante.AutorizoArea = factura.Factura.AutorizoArea;
            comprobante.RecibiNombre = factura.Factura.RecibiNombre;
            comprobante.RecibiPuesto = factura.Factura.RecibiPuesto;
            comprobante.RecibiArea = factura.Factura.RecibiArea;
            comprobante.VoBoTitulo = factura.Factura.VoBoTitulo;
            comprobante.RecibiTitulo = factura.Factura.RecibiTitulo;
            comprobante.AutorizoTitulo = factura.Factura.AutorizoTitulo;
            comprobante.AgregadoArea = factura.Factura.AgregadoArea;
            comprobante.AgregadoNombre = factura.Factura.AgregadoNombre;
            comprobante.AgregadoPuesto = factura.Factura.AgregadoPuesto;
            comprobante.AgregadoTitulo = factura.Factura.AgregadoTitulo;
            comprobante.CondicionesDePago = factura.Factura.FormaPago;
            comprobante.FechaPago = factura.Factura.FechaPago;
            comprobante.Proyecto = factura.Factura.Proyecto;//campo nuevo
            comprobante.CURPEmisor = emp.CURP;
            comprobante.TituloOtros = factura.Factura.TituloOtros;
            if (factura.Factura.TipoDocumento == TipoDocumento.FacturaLiverpool)
            {

                comprobante.FormaPago = factura.Factura.FormaPago;
                comprobante.NumeroPedido = factura._factura.DatosAduanera.NumeroPedido;
                comprobante.Buyer = factura._factura.DatosAduanera.Buyer;
                comprobante.Seller = factura._factura.DatosAduanera.Seller;
                comprobante.FechaPedido = factura._factura.DatosAduanera.FechaPedido;
                comprobante.DeptoContacto = factura._factura.DatosAduanera.DeptoContacto;
                comprobante.Proveedor = factura._factura.DatosAduanera.Proveedor;
                comprobante.Aprob = factura._factura.DatosAduanera.Aprob;
                comprobante.Contrarecibo = factura._factura.DatosAduanera.Contrarecibo;
                comprobante.FechaContrarecibo = factura._factura.DatosAduanera.FechaContrarecibo;
            }
            if (factura.Factura.TipoDocumento == TipoDocumento.FacturaMabe)
            {
                comprobante.FormaPago = factura.Factura.FormaPago;
                comprobante.MonedatipoMoneda = factura._factura.DatosAduanera.MonedatipoMoneda;
                comprobante.Proveedorcodigo = factura._factura.DatosAduanera.Provedorcodigo;
                comprobante.EntregaCodigoPostal = factura._factura.DatosAduanera.EntregaCodigoPostal;
                comprobante.noExterior = factura._factura.DatosAduanera.noExterior;
                comprobante.FechaInicial = factura._factura.DatosAduanera.FechaInicial;
                comprobante.PlantaEntrega = factura._factura.DatosAduanera.PlantaEntrega;
                comprobante.Calle = factura._factura.DatosAduanera.Calle;
                comprobante.noInterior = factura._factura.DatosAduanera.noInterior;
                comprobante.FechaFinal = factura._factura.DatosAduanera.FechaFinal;
            }
            if (factura.Factura.TipoDocumento == TipoDocumento.FacturaDeloitte)
            {
                comprobante.TipoAddenda = TipoAddenda.Deloitte;
                comprobante.Proyecto = factura.Factura.Proyecto;
                comprobante.nopedido = factura._factura.DatosAduanera.nopedido;
                comprobante.Tipomoneda = AddendaDeloitteMoneda.MXP;
                comprobante.oficina = AddendaDeloitteOficina.MEX;
                comprobante.origendefactura = AddendaDeloitteOrigenFactura.EGRESO;
                comprobante.correocontacto = factura._factura.DatosAduanera.correocontacto;
                comprobante.noproveedor = factura._factura.DatosAduanera.noproveedor;
                comprobante.correoproveedor = factura._factura.DatosAduanera.correoproveedor;
                comprobante.nombreproveedor = factura._factura.DatosAduanera.nombreproveedor;
            }
            if (factura.Factura.TipoDocumento == TipoDocumento.FacturaSorianaCEDIS)
            {
                comprobante.TipoAddenda = TipoAddenda.SorianaCedis;
                comprobante.Telefono = factura.Factura.DatosAduanera.Telefono;
                comprobante.noproveedor = factura.Factura.DatosAduanera.NoProveedor;
                comprobante.NombreCorto = factura.Factura.DatosAduanera.NombreCorto;
                comprobante.Gln = factura.Factura.DatosAduanera.Gln;
                comprobante.Descripcion = factura.Factura.DatosAduanera.Descripcion;
                comprobante.CantidadLetra = factura.Factura.DatosAduanera.CantidadLetra;
                comprobante.Proyecto = factura.Factura.DatosAduanera.Proyecto;
                comprobante.Observaciones = factura.Factura.DatosAduanera.Observaciones;
                comprobante.TipoCambio =Convert.ToDecimal( factura.Factura.DatosAduanera.tipoCambio);
                comprobante.MonedatipoMoneda = factura.Factura.DatosAduanera.TipoMoneda;
                comprobante.ExtraAtributo = factura.Factura.DatosAduanera.ExtraAtributo;
                comprobante.ExtraAtributo1 = factura.Factura.DatosAduanera.ExtraAtributo1;
                comprobante.Valor = factura.Factura.DatosAduanera.Valor;
                comprobante.Valor1 = factura.Factura.DatosAduanera.Valor1;
            }
            if (factura.Factura.TipoDocumento == TipoDocumento.FacturaSorianaTienda)
            {
                comprobante.TipoAddenda = TipoAddenda.SorianaTienda;
                comprobante.ProveedorRemision = factura.Factura.DatosAduanera.ProveedorRemision;
                comprobante.RemisionR = factura.Factura.DatosAduanera.RemisionR;
                comprobante.Consecutivo = factura.Factura.DatosAduanera.Consecutivo;
                comprobante.FechaRemision = factura.Factura.DatosAduanera.FechaRemision;
                comprobante.TiendaRemision = factura.Factura.DatosAduanera.TiendaRemision;
                comprobante.MonedatipoMoneda = factura.Factura.DatosAduanera.TipoMoneda;
                comprobante.TipoBulto = factura.Factura.DatosAduanera.TipoBulto;
                comprobante.EntrgaMercancia = factura.Factura.DatosAduanera.EntrgaMercancia;
                comprobante.CumpleReqFiscal = factura.Factura.DatosAduanera.CumpleReqFiscal;
                comprobante.CantidadBultos = factura.Factura.DatosAduanera.CantidadBultos;
                comprobante.Subtotal = factura.Factura.DatosAduanera.Subtotal;
                comprobante.Descuentos = factura.Factura.DatosAduanera.Descuentos;
                comprobante.IEPS = factura.Factura.DatosAduanera.IEPS;
                comprobante.IVA = factura.Factura.DatosAduanera.IVA;
                comprobante.OtrosImpuestos = factura.Factura.DatosAduanera.OtrosImpuestos;
                comprobante.Total = Convert.ToDecimal(factura.Factura.DatosAduanera.Total);
                comprobante.CantidadPedidos = factura.Factura.DatosAduanera.CantidadPedidos;
                comprobante.FechaEntrgaMercancia = factura.Factura.DatosAduanera.FechaEntrgaMercancia;
                comprobante.FolioNotaEntrada = factura.Factura.DatosAduanera.FolioNotaEntrada;
                comprobante.Proyecto = factura.Factura.DatosAduanera.Observaciones;
                //Pedido
                comprobante.ProveedorPedido = factura.Factura.DatosAduanera.ProveedorRemision;
                comprobante.RemisionPedido = factura.Factura.DatosAduanera.RemisionR;
                comprobante.FolioNotaEntrada = factura.Factura.DatosAduanera.FolioNotaEntrada;
                comprobante.TiendaPedidos = factura.Factura.DatosAduanera.TiendaPedido;
                comprobante.CantidadArticulos = factura.Factura.DatosAduanera.CantidadArticulos;
                //Articulos
                comprobante.ProveedorArticulos = factura.Factura.DatosAduanera.ProveedorRemision;
                comprobante.RemisionArticulos = factura.Factura.DatosAduanera.RemisionR;
                comprobante.FolioPedido = factura.Factura.DatosAduanera.FolioNotaEntrada;
                comprobante.TiendaArticulos = factura.Factura.DatosAduanera.TiendaArticulos;
                comprobante.Codigo = factura.Factura.DatosAduanera.Codigo;
                comprobante.CantidadUnidadCompra = factura.Factura.DatosAduanera.CantidadUnidadCompra;
                comprobante.CostoNetoUnidadCompra = factura.Factura.DatosAduanera.CostoNetoUnidadCompra;
                comprobante.PorcentajeIEPS = factura.Factura.DatosAduanera.PorcentajeIEPS;
                comprobante.PorecentajeIVA = factura.Factura.DatosAduanera.PorecentajeIVA;
            }
            if (factura.Factura.TipoDocumento == TipoDocumento.FacturaAdo)
            {
                comprobante.TipoAddenda = TipoAddenda.ADO;
                comprobante.Proveedor = factura.Factura.DatosAduanera.Proveedor;
                comprobante.Valor = factura.Factura.DatosAduanera.Valor;

            }
            //Addenda PEMEX -- SZ
            if (factura.Factura.TipoDocumento == TipoDocumento.FacturaPemex)
            {
                comprobante.TipoAddenda = TipoAddenda.Pemex;
                // Cast por reflexion
                AddendaPemex ap = new AddendaPemex();
                Type tipo = typeof(AddendaPemex);

                foreach (PropertyInfo propertyInfo in factura.Factura.FacturasAddendaPemex.GetType().GetProperties())
                {
                    tipo.GetProperty(propertyInfo.Name).SetValue(ap, propertyInfo.GetValue(factura.Factura.FacturasAddendaPemex, new object[] { }), new object[] { });
                }
                comprobante.AddendaPemex = ap;
                // Fin Cast por reflexion
            }
            if (factura.Factura.TipoDocumento == TipoDocumento.FacturaLucent)
            {
                comprobante.TipoAddenda = TipoAddenda.Lucent;
                comprobante.LucentRef = factura.Factura.LucentRef;
                comprobante.LucentOrdenCompra = factura.Factura.LucentOrdenCompra;
            }
            if(factura.Factura.TipoDocumento == TipoDocumento.CartaPorte)
            {
                //VERIFY Agregar los campos de la addenda
                comprobante.ConceptosCartasPortes = new List<ConceptosCartaPorte>();
                foreach (var concepto in factura.Factura.ConceptosCartaPortes)
                {
                    comprobante.ConceptosCartasPortes.Add(concepto);
                }
            }
            if (factura.Factura.TipoDocumento == TipoDocumento.Coppel)
            {
                comprobante.AddendaCoppelObj = factura.Factura.AddendaCoppelObj;
            }
            if (factura.Factura.TipoDocumento == TipoDocumento.Nomina)
            {/*
                var dto = factura.Factura.Nomina;
                if (dto != null)
                {
                    Nomina nomina = new Nomina()
                    {
                        Banco = dto.Banco,
                        CLABE = dto.CLABE,
                        Departamento = dto.Departamento,
                        FechaInicialPago = dto.FechaInicialPago,

                        NumEmpleado = dto.NumEmpleado,
                        NumSeguridadSocial = dto.NumSeguridadSocial,
                        Puesto = dto.Puesto,
                        PeriodicidadPago = dto.PeriodicidadPago,
                        TipoRegimen = dto.TipoRegimen,
                        RiesgoPuesto = dto.RiesgoPuesto,
                        SalarioBaseCotApor = dto.SalarioBaseCotApor,
                        SalarioBaseCotAporSpecified = true,
                        SalarioDiarioIntegrado = dto.SalarioDiarioIntegrado,
                        SalarioDiarioIntegradoSpecified = true,
                        TipoContrato = dto.TipoContrato,
                        TipoJornada = dto.TipoJornada,
                        Antiguedad = dto.Antiguedad,

                        CURP = dto.CURP

                    };
                    if (dto.Percepciones != null && dto.Percepciones.Percepcion != null &&
                        dto.Percepciones.Percepcion.Count > 0)
                    {
                        var percepciones = dto.Percepciones.Percepcion.Select(p => new NominaPercepcionesPercepcion
                        {
                            Clave = p.Clave,
                            Concepto = p.Concepto,
                            ImporteExento =
                                p.ImporteExento,
                            ImporteGravado =
                                p.ImporteGravado,
                            TipoPercepcion =
                                p.TipoPercepcion.ToString().PadLeft(3,'0')
                        }).ToList();


                        NominaPercepciones per = new NominaPercepciones()
                        {
                            Percepcion = percepciones.ToArray(),
                            TotalExento = dto.Percepciones.TotalExento,
                            TotalGravado = dto.Percepciones.TotalGravado
                        };
                        nomina.Percepciones = per;
                    }
                    if (dto.Deducciones != null && dto.Deducciones.Deduccion != null &&
                        dto.Deducciones.Deduccion.Count > 0)
                    {
                        var deducciones = dto.Deducciones.Deduccion.Select(p => new NominaDeduccionesDeduccion()
                        {
                            Clave = p.Clave,
                            Concepto = p.Concepto,
                            ImporteExento = p.ImporteExento,
                            ImporteGravado =
                                p.ImporteGravado,
                            TipoDeduccion = p.TipoDeduccion.ToString().PadLeft(3,'0')
                        }).ToList();

                        NominaDeducciones ded = new NominaDeducciones()
                        {
                            Deduccion = deducciones.ToArray(),
                            TotalExento = dto.Deducciones.TotalExento,
                            TotalGravado = dto.Deducciones.TotalGravado
                        };


                        nomina.Deducciones = ded;
                    }
                    comprobante.Nomina = nomina;
                    comprobante.Titulo = "Recibo de Nomina";

                }
                */
            }


            if (factura.Factura.TipoDocumento == TipoDocumento.Amc71)
            {
                comprobante.AddendaAmece = factura.Factura.AddendaAmece;
            }




            List<ComprobanteConcepto> conceptos = new List<ComprobanteConcepto>();
            foreach (facturasdetalle detalle in factura.Detalles)
            {
                ComprobanteConcepto con = new ComprobanteConcepto();
                con.Descripcion = detalle.Descripcion;
                if (!string.IsNullOrEmpty(detalle.Codigo))
                    con.NoIdentificacion = detalle.Codigo;
                con.Detalles = detalle.Descripcion2;
                con.Cantidad = detalle.Cantidad;
                con.ValorUnitario = detalle.Precio;
                con.Importe = Decimal.Round(detalle.TotalPartida, 6);
                con.Unidad = detalle.Unidad;
                con.OrdenCompra = detalle.OrdenCompra;

                /*
                if (!string.IsNullOrEmpty(detalle.CuentaPredial))
                {
                    con.CuentaPredial = detalle.CuentaPredial;
                    var predial = new ComprobanteConceptoCuentaPredial
                        {
                            Numero = detalle.CuentaPredial
                        };
                    con.Items = new object[] { predial };
                }*/
                conceptos.Add(con);
            }

            comprobante.Conceptos = conceptos;//.ToArray();
            ComprobanteImpuestosTraslado traslado = new ComprobanteImpuestosTraslado();
            traslado.Importe = Decimal.Round(factura.Factura.IVA.Value, 6);
            //traslado.Impuesto = ComprobanteImpuestosTrasladoImpuesto.IVA;
            //traslado.TasaOCuota = new decimal(16);
            List<ComprobanteImpuestosTraslado> listaTraslados = new List<ComprobanteImpuestosTraslado>();
            listaTraslados.Add(traslado);

            ComprobanteImpuestos impuestos = new ComprobanteImpuestos();
            impuestos.Traslados = listaTraslados.ToArray();
            impuestos.TotalImpuestosTrasladados = Decimal.Round(factura.Factura.IVA.Value, 6);
            impuestos.TotalImpuestosTrasladadosSpecified = true;
            if (factura.Factura.TipoDocumento == TipoDocumento.FacturaTransportista ||
                factura.Factura.TipoDocumento == TipoDocumento.FacturaAduanera ||
                factura.Factura.TipoDocumento == TipoDocumento.FacturaLiverpool ||
                factura.Factura.TipoDocumento == TipoDocumento.ReciboHonorarios ||
                factura.Factura.TipoDocumento == TipoDocumento.FacturaDeloitte ||
                factura.Factura.TipoDocumento == TipoDocumento.FacturaMabe ||
                 factura.Factura.TipoDocumento == TipoDocumento.FacturaSorianaCEDIS ||
                  factura.Factura.TipoDocumento == TipoDocumento.FacturaSorianaTienda ||
                  factura.Factura.TipoDocumento == TipoDocumento.FacturaAdo ||
                factura.Factura.TipoDocumento == TipoDocumento.Arrendamiento ||
                  factura.Factura.TipoDocumento == TipoDocumento.CorporativoAduanal ||
                factura.Factura.TipoDocumento == TipoDocumento.ConstructorFirmas || 
                factura.Factura.TipoDocumento == TipoDocumento.CartaPorte)
            {
                AgregaRetencionTransportista(factura, impuestos);
                comprobante.RetencionIsr = factura.Factura.RetenciónIsr;
                comprobante.RetencionIva = factura.Factura.RetenciónIva;
            }
            comprobante.CantidadLetra = CantidadLetra.Enletras(comprobante.Total.ToString(), comprobante.Moneda);
            if (factura.Factura.TipoDocumento == TipoDocumento.FacturaAduanera ||
                factura.Factura.TipoDocumento == TipoDocumento.FacturaLiverpool ||
                 factura.Factura.TipoDocumento == TipoDocumento.FacturaMabe ||
                  factura.Factura.TipoDocumento == TipoDocumento.FacturaSorianaCEDIS ||
                   factura.Factura.TipoDocumento == TipoDocumento.FacturaSorianaTienda ||
                   factura.Factura.TipoDocumento == TipoDocumento.FacturaAdo ||
                  factura.Factura.TipoDocumento == TipoDocumento.FacturaDeloitte ||
                factura.Factura.TipoDocumento == TipoDocumento.Constructor ||
                factura.Factura.TipoDocumento == TipoDocumento.ConstructorFirmas ||
                  factura.Factura.TipoDocumento == TipoDocumento.CorporativoAduanal ||
                factura.Factura.TipoDocumento == TipoDocumento.ConstructorFirmasCustom)
            {
                comprobante.DatosAduana = factura.Factura.DatosAduanera;
                if (!string.IsNullOrEmpty(comprobante.DatosAduana.Saldo))
                {
                    var saldo = decimal.Parse(comprobante.DatosAduana.Saldo, NumberStyles.Currency, new CultureInfo("es-MX"));
                    comprobante.CantidadLetra = CantidadLetra.Enletras(saldo.ToString(),
                                                                                comprobante.Moneda);
                }
                comprobante.ConceptosAduana = factura.Factura.ConceptosAduanera
                    .Select(p => new ComprobanteConcepto
                        {
                            Descripcion = p.Descripcion,
                            ValorUnitario = p.Total,
                            Importe = p.Total,
                            Detalles = p.Descripcion2,NoIdentificacion = p.Codigo
                        }).ToList();
            }

            if (factura.Factura.TipoDocumento == TipoDocumento.Donativo)
            {
                Donatarias donat = new Donatarias
                                       {
                                           fechaAutorizacion = factura.Factura.DonativoFechaAutorizacion,
                                           leyenda =
                                               "Este comprobante ampara un donativo, el cual será destinado por la donataria a los fines propios de su objeto social. En el caso de que los bienes donados hayan sido deducidos previamente para los efectos del impuesto sobre la renta, este donativo no es deducible. La reproducción no autorizada de este comprobante constituye un delito en los términos de las disposiciones fiscales.",
                                           noAutorizacion = factura.Factura.DonativoAutorizacion,
                                           version = "1.1"
                                       };
                if (comprobante.Complemento == null)
                    comprobante.Complemento = new ComprobanteComplemento();
                comprobante.Complemento.Donat = donat;
            }
            if (factura.Factura.TipoDocumento == TipoDocumento.Nomina)
            {
                comprobante.DatosAduana = factura.Factura.DatosAduanera;
                comprobante.ConceptosAduana = factura.Factura.ConceptosAduanera
                    .Select(p => new ComprobanteConcepto
                    {
                        Descripcion = p.Descripcion,
                        ValorUnitario = p.Total,
                        Importe = p.Total,
                        Detalles = p.Descripcion2
                    }).ToList();

            }
            comprobante.Impuestos = impuestos;
            return comprobante;
        }

        private static void AgregaRetencionTransportista(NtLinkFactura factura, ComprobanteImpuestos impuestos)
        {
            ComprobanteImpuestosRetencion retIsr = null;
            ComprobanteImpuestosRetencion retIva = null;
            if (factura.Factura.RetenciónIsr > 0)
            {
                retIsr = new ComprobanteImpuestosRetencion
                             {
                                 Importe = Decimal.Round(factura.Factura.RetenciónIsr, 6),
                           //      Impuesto = ComprobanteImpuestosRetencionImpuesto.ISR
                             };
            }
            if (factura.Factura.RetenciónIva > 0)
            {
                retIva = new ComprobanteImpuestosRetencion
                             {
                                 Importe = Decimal.Round(factura.Factura.RetenciónIva, 6),
                             //    Impuesto = ComprobanteImpuestosRetencionImpuesto.IVA
                             };
            }
            List<ComprobanteImpuestosRetencion> retenciones = new List<ComprobanteImpuestosRetencion>();
            if (retIsr != null)
            {
                retenciones.Add(retIsr);
            }
            if (retIva != null)
            {
                retenciones.Add(retIva);
            }
            if (retenciones.Count > 0)
            {
                impuestos.Retenciones = retenciones.ToArray();
                impuestos.TotalImpuestosRetenidos = Decimal.Round(retenciones.Sum(p => p.Importe), 6);
                impuestos.TotalImpuestosRetenidosSpecified = true;
            }
        }

        public static byte[] GeneraPreviewRs(NtLinkFactura factura)
        {
            try
            {
                empresa emp = factura.Emisor;
                clientes cliente = factura.Receptor;
                if (string.IsNullOrEmpty(cliente.RFC))
                {
                    throw new ApplicationException("El rfc es erróneo");
                }
                string path = Path.Combine(ConfigurationManager.AppSettings["Resources"], emp.RFC, "Certs");
                X509Certificate2 cert = new X509Certificate2(Path.Combine(path, "csd.cer"));
                string rutaLlave = Path.Combine(path, "csd.key");

                var comprobante = GetComprobante(factura, cliente, emp);
                GeneradorCfdi gen = new GeneradorCfdi();
                gen.GenerarCfdPreview(comprobante, cert, rutaLlave, emp.PassKey);
                string ruta = Path.Combine(ConfigurationManager.AppSettings["Salida"], emp.RFC);
                if (!Directory.Exists(ruta))
                    Directory.CreateDirectory(ruta);
                //comprobante.CantidadLetra = CantidadLetra.Enletras(comprobante.total.ToString(), comprobante.Moneda);
                byte[] pdf = gen.GetPdfFromComprobante(comprobante, emp.Orientacion, factura.Factura.TipoDocumento);
                return pdf;
            }
            catch (FaultException fe)
            {
                throw;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                if (ex.InnerException != null)
                    Logger.Error(ex.InnerException);
                return null;
            }

        }



        public static byte[] GeneraPreview(NtLinkFactura factura)
        {
            try
            {
                empresa emp = factura.Emisor;
                clientes cliente = factura.Receptor;
                if (string.IsNullOrEmpty(cliente.RFC))
                {
                    throw new ApplicationException("El rfc es erróneo");
                }
                string path = Path.Combine(ConfigurationManager.AppSettings["Resources"], emp.RFC, "Certs");
                X509Certificate2 cert = new X509Certificate2(Path.Combine(path, "csd.cer"));
                string rutaLlave = Path.Combine(path, "csd.key");

                var comprobante = GetComprobante(factura, cliente, emp);
                GeneradorCfdi gen = new GeneradorCfdi();
                gen.GenerarCfdPreview(comprobante, cert, rutaLlave, emp.PassKey);
                string ruta = Path.Combine(ConfigurationManager.AppSettings["Salida"], emp.RFC);
                if (!Directory.Exists(ruta))
                    Directory.CreateDirectory(ruta);



                //comprobante.CantidadLetra = CantidadLetra.Enletras(comprobante.total.ToString(), comprobante.Moneda);
                byte[] pdf = gen.GetPdfFromComprobante(comprobante, emp.Orientacion, factura.Factura.TipoDocumento);
                return pdf;
            }
            catch (FaultException fe)
            {
                throw;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                if (ex.InnerException != null)
                    Logger.Error(ex.InnerException);
                return null;
            }

        }



        public static string GetNextFolio(int idEmpresa)
        {
            try
            {
                using (var db = new NtLinkLocalServiceEntities())
                {
                    string folio = db.facturas.Where(p => p.IdEmpresa == idEmpresa).Max(p => p.Folio);
                    int i = 0;
                    if (folio != null)
                    {
                        i = int.Parse(folio);
                    }
                    i++;
                    return i.ToString().PadLeft(4, '0');
                }
            }
            catch (Exception ee)
            {
                Logger.Error(ee.Message);
                return null;
            }
        }


        private bool ValidaFolio(string folio, int idEmpresa)
        {
            if (string.IsNullOrEmpty(folio))
                throw new ApplicationException("El folio de la factura no puede ir vacío");
            return true;

        }



        public static byte[] GetAcuseCancelacion(string report, int idVenta)
        {
            try
            {
                Logger.Debug(report + "->" + idVenta);
                ReportExecutionService rs = new ReportExecutionService();
                string userName = ConfigurationManager.AppSettings["RSUserName"];
                string password = ConfigurationManager.AppSettings["RSPass"];
                string url = ConfigurationManager.AppSettings["RSUrlExec"];
                rs.Credentials = new NetworkCredential(userName, password);
                rs.Url = url;
                string reportPath = report;//"/ReportesNtLink/Pdf";
                string format = "Pdf";
                string devInfo = @"<DeviceInfo> <OutputFormat>PDF</OutputFormat> </DeviceInfo>";
                ReportExecution.ParameterValue[] parameters = new ReportExecution.ParameterValue[1];

                parameters[0] = new ReportExecution.ParameterValue();
                parameters[0].Name = "idVenta";
                parameters[0].Value = idVenta.ToString();

                ExecutionHeader execHeader = new ExecutionHeader();
                rs.Timeout = 300000;
                rs.ExecutionHeaderValue = execHeader;
                rs.LoadReport(reportPath, null);
                rs.SetExecutionParameters(parameters, "en-US");


                string mimeType;
                string encoding;
                string fileNameExtension;
                ReportExecution.Warning[] warnings;
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




        public void CancelarFactura(string uuid, string acuse)
        {
            try
            {
                using (var db = new NtLinkLocalServiceEntities())
                {
                    var fact = db.facturas.FirstOrDefault(p => p.Uid == uuid);
                    if (fact != null)
                    {
                        AcuseCancelacion ac = AcuseCancelacion.Parse(acuse);
                        fact.Observaciones = acuse;
                        fact.Cancelado = 1;
                        fact.EstatusCancelacion = ac.Status;
                        fact.FechaCancelacion = ac.FechaCancelacion;
                        fact.SelloCancelacion = ac.SelloSat;
                        db.facturas.ApplyCurrentValues(fact);
                        db.SaveChanges();
                    }
                }
            }
            catch (FaultException fe)
            {
                throw;
            }
            catch (Exception ee)
            {
                Logger.Error(ee.Message);
                if (ee.InnerException != null)
                    Logger.Error(ee.InnerException);
            }
        }

        public NtLinkFactura(int idFactura)
        {
            try
            {
                using (var db = new NtLinkLocalServiceEntities())
                {
                    if (idFactura == 0)
                    {
                        this.Factura = new facturas();
                        this.Detalles = new List<facturasdetalle>();
                    }
                    else
                    {
                        this.Factura = db.facturas.Where(p => p.idVenta == idFactura).FirstOrDefault();
                        if (Factura == null)
                        {
                            throw new ApplicationException("La factura " + idFactura.ToString() + " No se encontró");
                        }
                        this.Detalles = db.facturasdetalle.Where(p => p.idVenta == idFactura).ToList();
                    }
                }
            }
            catch (Exception ee)
            {
                Logger.Error(ee.Message);
                if (ee.InnerException != null)
                    Logger.Error(ee.InnerException);
            }

        }

        public bool Save()
        {
            try
            {
                 if (_factura.DatosAduanera != null)
                    _factura.Ieps = _factura.DatosAduanera.IEPS;

                using (var db = new NtLinkLocalServiceEntities())
                {
                    empresa em = db.empresa.Where(p => p.IdEmpresa == _factura.IdEmpresa).FirstOrDefault();
                    //if (Factura.Folio != null)
                    em.Folio = _factura.Folio;
                    //  else  em.Folio = "000001";//inicia la factura
                    db.SaveChanges();

                    if (_factura.idVenta == 0 && ValidaFolio(_factura.Folio, _factura.IdEmpresa.Value))
                    {
                        db.facturas.AddObject(_factura);
                        var ee = db.Sistemas.FirstOrDefault(p => p.IdSistema == Emisor.idSistema);
                        ee.SaldoEmision = ee.SaldoEmision - 1;
                        ee.ConsumoEmision = ee.ConsumoEmision + 1;
                        db.SaveChanges();
                    }
                    else
                    {
                        db.facturas.ApplyCurrentValues(_factura);
                    }
                    foreach (facturasdetalle detalle in Detalles)
                    {
                        if (detalle.idproducto == 0)
                        {
                            producto prod = new producto();
                            prod.Unidad = detalle.Unidad;
                            prod.Codigo = detalle.Codigo;
                            prod.Descripcion = detalle.Descripcion;
                            prod.Observaciones = detalle.Descripcion2;
                            prod.PrecioP = detalle.Precio;
                            prod.UltimaVenta = detalle.Precio;
                            prod.IdEmpresa = Factura.IdEmpresa;
                            var ntprod = new NtLinkProducto();
                            ntprod.SaveProducto(prod);
                            detalle.idproducto = prod.IdProducto;
                        }
                        else
                        {
                            producto prod = db.producto.Where(p => p.IdProducto == detalle.idproducto).FirstOrDefault();
                            prod.UltimaVenta = detalle.Precio;
                            prod.Modificacion = DateTime.Now;
                            db.producto.ApplyCurrentValues(prod);
                        }

                        detalle.idVenta = _factura.idVenta;
                        if (detalle.IdFacturaDetalle == 0)
                            db.facturasdetalle.AddObject(detalle);
                        else
                            db.facturasdetalle.ApplyCurrentValues(detalle);

                    }



                    db.SaveChanges();
                    return true;
                }
            }
            catch (Exception ee)
            {
                Logger.Error(ee);
                if (ee.InnerException != null)
                    Logger.Error(ee.InnerException);
                return false;
            }
        }
        /*
        public bool Save()
        {
            try
            {
                using (var db = new NtLinkLocalServiceEntities())
                {
                    if (_factura.idVenta == 0 && ValidaFolio(_factura.Folio, _factura.IdEmpresa.Value))
                    {
                        db.facturas.AddObject(_factura);
                        db.SaveChanges();
                    }
                    else
                    {
                        db.facturas.ApplyCurrentValues(_factura);
                    }
                    foreach (facturasdetalle detalle in Detalles)
                    {
                        if (detalle.idproducto == 0)
                        {
                            producto prod = new producto();
                            prod.Unidad = detalle.Unidad;
                            prod.Codigo = detalle.Codigo;
                            prod.Descripcion = detalle.Descripcion;
                            prod.Observaciones = detalle.Descripcion2;
                            prod.PrecioP = detalle.Precio;
                            prod.UltimaVenta = detalle.Precio;
                            prod.IdEmpresa = Factura.IdEmpresa;
                            var ntprod = new NtLinkProducto();
                            ntprod.SaveProducto(prod);
                            detalle.idproducto = prod.IdProducto;
                        }
                        else
                        {
                            producto prod = db.producto.Where(p => p.IdProducto == detalle.idproducto).FirstOrDefault();
                            prod.UltimaVenta = detalle.Precio;
                            prod.Modificacion = DateTime.Now;
                            db.producto.ApplyCurrentValues(prod);
                        }

                        detalle.idVenta = _factura.idVenta;
                        if (detalle.IdFacturaDetalle == 0)
                            db.facturasdetalle.AddObject(detalle);
                        else
                            db.facturasdetalle.ApplyCurrentValues(detalle);


                    }
                    db.SaveChanges();
                    return true;
                }
            }
            catch (Exception ee)
            {
                Logger.Error(ee);
                if (ee.InnerException != null)
                    Logger.Error(ee.InnerException);
                return false;
            }
        }
        */
        public static void Pagar(int idVenta, DateTime fechaPago, string referenciaPago)
        {
            try
            {
                using (var db = new NtLinkLocalServiceEntities())
                {
                    facturas fac = db.facturas.Single(l => l.idVenta == idVenta);
                    fac.FechaPago = fechaPago;
                    fac.ReferenciaPago = referenciaPago;
                    db.facturas.ApplyCurrentValues(fac);
                    db.SaveChanges();
                }
            }
            catch (Exception ee)
            {
                Logger.Error(ee.Message);
                if (ee.InnerException != null)
                    Logger.Error(ee.InnerException);
                throw;
            }
        }
    }
}
