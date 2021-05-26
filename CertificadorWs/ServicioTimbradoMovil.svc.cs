using CertificadorWs.Business;
using EspacioComercioExterior11;
using Gma.QrCodeNet.Encoding;
using Gma.QrCodeNet.Encoding.Windows.Render;
using log4net;
using log4net.Config;
using ServicioLocal.Business;
using ServicioLocal.Business.Complemento;
using ServicioLocal.Business.Pagoo;
using ServicioLocalContract;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.Text;
using System.Web.Security;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace CertificadorWs
{
    public class ServicioTimbradoMovil : IServicioTimbradoMovil
    {
        private readonly XNamespace _ns = "http://www.sat.gob.mx/cfd/3";

        private readonly XNamespace _ns2 = "http://www.sat.gob.mx/nomina12";

        /// <summary>
        /// Obtiene un listado con las empresas que estan dadas de alta en el sistema de timbrado
        /// </summary>
        /// <param name="userName">Nombre de usuario (proporcionado por NT LINK)</param>
        /// <param name="password">Contraseña (proporcionado por NT LINK)</param>
        /// <returns></returns>
        private static readonly ILog Logger = LogManager.GetLogger(typeof(CertificadorService));

        private readonly ValidadorCFDi32 _val;

        private readonly XNamespace _ns3 = "http://www.sat.gob.mx/ComercioExterior11";

        private readonly XNamespace _ns9 = "http://www.sat.gob.mx/ine";

        private readonly XNamespace _ns10 = "http://www.sat.gob.mx/EstadoDeCuentaCombustible12";

        /// <summary>
        /// Timbra un comprobante
        /// </summary>
        /// <param name="userName">Nombre de usuario (proporcionado por NT LINK)</param>
        /// <param name="password">Contraseña (proporcionado por NT LINK)</param>
        /// <param name="comprobante">String con el contenido del CFDi codificado en UTF-8</param>
        /// <returns>El String con el complemento de certificacion (TimbreFiscalDigital)</returns>
        private readonly XNamespace _ns7 = "http://www.sat.gob.mx/Pagos";

        private readonly XNamespace _ns8 = "http://www.sat.gob.mx/implocal";

        public string ObtenerEmpresasFolio(string userName, string password, string RFC)
        {
            ServicioTimbradoMovil.Logger.Debug(userName);
            MembershipUser x = NtLinkLogin.ValidateUser(userName, password);
            if (x == null)
            {
                throw new FaultException("Nombre de usuario o contraseña incorrecta");
            }
            empresa empresa = new empresa();
            if (!TimbradoUtils.EmpresaMultipleRFC(RFC))
            {
                empresa = TimbradoUtils.ValidarUsuario(RFC);
            }
            else
            {
                empresa = NtLinkUsuarios.GetEmpresaByUserId(x.ProviderUserKey.ToString());
                empresa = TimbradoUtils.ValidarUsuarioMultiple(empresa);
            }
            if (empresa == null)
            {
                throw new FaultException("300 - El usuario con el que se quiere conectar es inválido");
            }
            if (empresa.Bloqueado)
            {
                ServicioTimbradoMovil.Logger.Info(empresa.RFC + "-> Bloqueado");
                throw new FaultException("El RFC del emisor se encuentra bloqueado, favor de ponerse en contacto con atención al cliente");
            }
            string result;
            if (empresa == null)
            {
                result = null;
            }
            else if (empresa.Folio == null)
            {
                result = "0";
            }
            else
            {
                result = empresa.Folio;
            }
            return result;
        }

        public ServicioTimbradoMovil()
        {
            XmlConfigurator.Configure();
            try
            {
                this._val = new ValidadorCFDi32();
            }
            catch (Exception ee)
            {
                ServicioTimbradoMovil.Logger.Error(ee);
            }
        }

        private Nomina DesSerializarNomina12(XElement element, ref string erroresNom)
        {
            Nomina result;
            try
            {
                IEnumerable<XElement> nomina12 = element.Elements(this._ns + "Complemento");
                if (nomina12 != null)
                {
                    nomina12 = nomina12.Elements(this._ns2 + "Nomina");
                    if (nomina12 != null)
                    {
                        IEnumerable<XAttribute> version = nomina12.Attributes("Version");
                        foreach (XAttribute att in version)
                        {
                            if (att.Value == "1.2")
                            {
                                using (IEnumerator<XElement> enumerator2 = nomina12.GetEnumerator())
                                {
                                    if (enumerator2.MoveNext())
                                    {
                                        XElement e = enumerator2.Current;
                                        XmlSerializer ser = new XmlSerializer(typeof(Nomina));
                                        string xml = e.ToString();
                                        StringReader reader = new StringReader(xml);
                                        Nomina comLXMLComprobante = (Nomina)ser.Deserialize(reader);
                                        result = comLXMLComprobante;
                                        return result;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                erroresNom = "NOM225 - " + ex.InnerException.Message;
                result = null;
                return result;
            }
            result = null;
            return result;
        }

        public ComercioExterior DesSerializarComercioExterior(XElement element, ref string erroresNom)
        {
            ComercioExterior result;
            try
            {
                IEnumerable<XElement> nomina12 = element.Elements(this._ns + "Complemento");
                if (nomina12 != null)
                {
                    string comerci = "";
                    try
                    {
                        comerci = nomina12.First<XElement>().ToString();
                    }
                    catch (Exception e_3D)
                    {
                        erroresNom = "CCE154 - El nodo cce11:ComercioExterior debe registrarse como un nodo hijo del nodo Complemento en el CFDI.";
                        result = null;
                        return result;
                    }
                    if (comerci == "<cfdi:Complemento xmlns:cfdi=\"http://www.sat.gob.mx/cfd/3\" />")
                    {
                        erroresNom = "CCE154 - El nodo cce11:ComercioExterior debe registrarse como un nodo hijo del nodo Complemento en el CFDI.";
                        result = null;
                        return result;
                    }
                    nomina12 = nomina12.Elements(this._ns3 + "ComercioExterior");
                    if (nomina12 != null)
                    {
                        IEnumerable<XAttribute> version = nomina12.Attributes("Version");
                        foreach (XAttribute att in version)
                        {
                            if (nomina12.Count<XElement>() > 1)
                            {
                                erroresNom = "CCE153 - El nodo cce11:ComercioExterior no puede registrarse mas de una vez.";
                                result = null;
                                return result;
                            }
                            using (IEnumerator<XElement> enumerator2 = nomina12.GetEnumerator())
                            {
                                if (enumerator2.MoveNext())
                                {
                                    XElement e = enumerator2.Current;
                                    XmlSerializer ser = new XmlSerializer(typeof(ComercioExterior));
                                    string xml = e.ToString();
                                    StringReader reader = new StringReader(xml);
                                    ComercioExterior comLXMLComprobante = (ComercioExterior)ser.Deserialize(reader);
                                    result = comLXMLComprobante;
                                    return result;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                erroresNom = "CCE218 - " + ex.InnerException.Message;
                result = null;
                return result;
            }
            result = null;
            return result;
        }

        public INE DesSerializarINE(XElement element, ref string erroINE)
        {
            INE result;
            try
            {
                IEnumerable<XElement> ine = element.Elements(this._ns + "Complemento");
                if (ine != null)
                {
                    ine = ine.Elements(this._ns9 + "INE");
                    if (ine != null)
                    {
                        using (IEnumerator<XElement> enumerator = ine.GetEnumerator())
                        {
                            if (enumerator.MoveNext())
                            {
                                XElement e = enumerator.Current;
                                XmlSerializer ser = new XmlSerializer(typeof(INE));
                                string xml = e.ToString();
                                StringReader reader = new StringReader(xml);
                                INE comLXMLComprobante = (INE)ser.Deserialize(reader);
                                result = comLXMLComprobante;
                                return result;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                erroINE = ex.InnerException.Message;
                result = null;
                return result;
            }
            result = null;
            return result;
        }

        public EstadoDeCuentaCombustible DesSerializarECC(XElement element, ref string erroECC)
        {
            EstadoDeCuentaCombustible result;
            try
            {
                IEnumerable<XElement> ine = element.Elements(this._ns + "Complemento");
                if (ine != null)
                {
                    ine = ine.Elements(this._ns10 + "EstadoDeCuentaCombustible");
                    if (ine != null)
                    {
                        using (IEnumerator<XElement> enumerator = ine.GetEnumerator())
                        {
                            if (enumerator.MoveNext())
                            {
                                XElement e = enumerator.Current;
                                XmlSerializer ser = new XmlSerializer(typeof(EstadoDeCuentaCombustible));
                                string xml = e.ToString();
                                StringReader reader = new StringReader(xml);
                                EstadoDeCuentaCombustible comLXMLComprobante = (EstadoDeCuentaCombustible)ser.Deserialize(reader);
                                result = comLXMLComprobante;
                                return result;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                erroECC = ex.InnerException.Message;
                result = null;
                return result;
            }
            result = null;
            return result;
        }

        public TimbradoResponse TimbraCfdiMovil(string userName, string password, string comprobante)
        {
            TimbradoResponse result2;
            try
            {
                string erroresNomina = "0";
                ServicioTimbradoMovil.Logger.Debug(userName);
                MembershipUser x = NtLinkLogin.ValidateUser(userName, password);
                if (x == null)
                {
                    throw new FaultException("Nombre de usuario o contraseña incorrecta");
                }
                XElement element = XElement.Load(new StringReader(comprobante));
                ServicioLocal.Business.Comprobante comp = this.DesSerializar(element);
                ImpuestosLocales IL = null;
                if (comprobante.Contains("<ine:INE "))
                {
                    string erroINE = "";
                    INE I = this.DesSerializarINE(element, ref erroINE);
                    ValidarINE VI = new ValidarINE();
                    erroINE = VI.ProcesarINE(I);
                    if (erroINE != "0")
                    {
                        result2 = new TimbradoResponse
                        {
                            Valido = false,
                            DescripcionError = erroINE
                        };
                        return result2;
                    }
                }
                if (comprobante.Contains("<ecc11:EstadoDeCuentaCombustible"))
                {
                    string erroECC = "";
                    EstadoDeCuentaCombustible E = this.DesSerializarECC(element, ref erroECC);
                    ValidarECC VE = new ValidarECC();
                    erroECC = VE.ProcesarECC(E, comp.TipoDeComprobante, comp.Version);
                    if (erroECC != "0")
                    {
                        result2 = new TimbradoResponse
                        {
                            Valido = false,
                            DescripcionError = erroECC
                        };
                        return result2;
                    }
                }
                if (comprobante.Contains("<implocal:ImpuestosLocales"))
                {
                    IL = this.DesSerializarImpuestosLocales(element);
                }
                bool pago10 = comprobante.Contains("pago10:Pagos");
                if (pago10)
                {
                    ServicioLocal.Business.Pagoo.Comprobante pagoDatos = this.DesSerializarP(element);
                    ServicioLocal.Business.Complemento.Pagos pagoss = this.DesSerializarPagos(element);
                    ValidarPago VP = new ValidarPago();
                    string ErrorPagos = VP.ProcesarPago(comp, pagoss, pagoDatos);
                    if (ErrorPagos != "0")
                    {
                        result2 = new TimbradoResponse
                        {
                            Valido = false,
                            DescripcionError = ErrorPagos
                        };
                        return result2;
                    }
                }
                NtLinkClientes nlc = new NtLinkClientes();
                clientes cliente = nlc.GetCliente(comp.Receptor.Rfc);
                bool nomin12 = comprobante.Contains("nomina12:Nomina");
                List<Nomina> nom = new List<Nomina>();
                
                if (nomin12)
                {
                    string erroresNom = null;
                    var nomx = this.DesSerializarNomina12(element, ref erroresNom);
                    nom.Add(nomx);
                    if (erroresNom != null)
                    {
                        result2 = new TimbradoResponse
                        {
                            Valido = false,
                            DescripcionError = erroresNom
                        };
                        return result2;
                    }
                }
                bool ComerExt = comprobante.Contains("cce11:ComercioExterior");
                if (comp.Emisor != null && comp.Emisor.Rfc != null)
                {
                    empresa empresa = new empresa();
                    if (!TimbradoUtils.EmpresaMultipleRFC(comp.Emisor.Rfc))
                    {
                        empresa = TimbradoUtils.ValidarUsuario(comp.Emisor.Rfc);
                    }
                    else
                    {
                        empresa = NtLinkUsuarios.GetEmpresaByUserId(x.ProviderUserKey.ToString());
                        empresa = TimbradoUtils.ValidarUsuarioMultiple(empresa);
                    }
                    if (empresa == null)
                    {
                        result2 = null;
                    }
                    else
                    {
                        string result = null;
                        TimbreFiscalDigital timbre = null;
                        string acuseSat = "";
                        string hash = null;
                        if (ComerExt && erroresNomina == "0")
                        {
                            string erroresComer = null;
                            ValidarComercioExterior val = new ValidarComercioExterior();
                            ComercioExterior Comer = this.DesSerializarComercioExterior(element, ref erroresComer);
                            if (erroresComer != null)
                            {
                                result2 = new TimbradoResponse
                                {
                                    Valido = false,
                                    DescripcionError = erroresComer
                                };
                                return result2;
                            }
                            erroresNomina = val.ProcesarComercioExterior(Comer, comp);
                        }
                        if (nomin12 && erroresNomina == "0")
                        {
                            erroresNomina = this._val.ProcesarNomina(nom, comp);
                        }
                        if (erroresNomina == "0")
                        {
                            try
                            {
                                string path = Path.Combine(ConfigurationManager.AppSettings["Resources"], empresa.RFC, "Certs");
                                X509Certificate2 cert = new X509Certificate2(Path.Combine(path, "csd.cer"));
                                string rutaLlave = Path.Combine(path, "csd.key");
                                if (File.Exists(rutaLlave + ".pem"))
                                {
                                    rutaLlave += ".pem";
                                }
                                ServicioTimbradoMovil.Logger.Debug("Ruta Llave " + rutaLlave);
                                if (!File.Exists(rutaLlave))
                                {
                                    result2 = new TimbradoResponse
                                    {
                                        Valido = false,
                                        DescripcionError = "Error certificado de la empresa no está cargado en el sistema"
                                    };
                                    return result2;
                                }
                                GeneradorCfdi gen = new GeneradorCfdi();
                                string sellado = gen.GenerarCfdSinTimbre(comp, cert, rutaLlave, empresa.PassKey, comprobante);
                                if (sellado == null)
                                {
                                    result2 = new TimbradoResponse
                                    {
                                        Valido = false,
                                        DescripcionError = "Error al sellar el comprobante: al sellar"
                                    };
                                    return result2;
                                }
                                comprobante = sellado;
                            }
                            catch (FaultException ex)
                            {
                                ServicioTimbradoMovil.Logger.Error(ex);
                                result2 = new TimbradoResponse
                                {
                                    Valido = false,
                                    DescripcionError = ex.Message
                                };
                                return result2;
                            }
                            ValidarCFDI33 valida = new ValidarCFDI33();
                            string errorCFDI33 = valida.ProcesarCFDI33(comp, comprobante, pago10, ComerExt, IL);
                            if (errorCFDI33 != "0")
                            {
                                result2 = new TimbradoResponse
                                {
                                    Valido = false,
                                    DescripcionError = errorCFDI33
                                };
                            }
                            else
                            {
                                Dictionary<int, string> dict = this._val.ProcesarCadena(comp.Emisor.Rfc, comprobante, ref result, ref timbre, ref acuseSat, ref hash);
                                if (timbre != null && timbre.SelloSAT != null && dict.Count == 0)
                                {
                                    if (!string.IsNullOrEmpty(comp.Confirmacion))
                                    {
                                        using (NtLinkLocalServiceEntities db = new NtLinkLocalServiceEntities())
                                        {
                                            ConfirmacionTimbreWs33 C = db.ConfirmacionTimbreWs33.FirstOrDefault((ConfirmacionTimbreWs33 p) => p.Folio == comp.Folio && p.RfcEmisor == comp.Emisor.Rfc && p.RfcReceptor == comp.Receptor.Rfc);
                                            C.procesado = new bool?(true);
                                            db.ConfirmacionTimbreWs33.ApplyCurrentValues(C);
                                            db.SaveChanges();
                                        }
                                    }
                                    if (ConfigurationManager.AppSettings["Pruebas"] == "true")
                                    {
                                        timbre.SelloSAT = "Inválido, Ambiente de pruebas";
                                    }
                                    string cfdiTimbrado = result;
                                    if (ConfigurationManager.AppSettings["EnvioSat"] == "false")
                                    {
                                        if (!TimbradoUtils.GuardaFactura(comp.Fecha, comp.Emisor.Rfc, comp.Receptor.Rfc, timbre.UUID, cfdiTimbrado, hash, empresa, true, false))
                                        {
                                            throw new Exception("Error al abrir el comprobante");
                                        }
                                        NtLinkFactura fac = new NtLinkFactura(0);
                                        if (string.IsNullOrEmpty(empresa.RegimenFiscal))
                                        {
                                            throw new FaultException("Debes capturar el regimen fiscal de la empresa");
                                        }
                                        fac.Emisor = empresa;
                                        fac.Receptor = cliente;
                                        fac.Factura = this.CrearFactura(comp, empresa.IdEmpresa, cliente.idCliente);
                                        fac.Factura.Uid = timbre.UUID;
                                        fac.Save();
                                        comp.Complemento = new ComprobanteComplemento();
                                        comp.Complemento.timbreFiscalDigital = timbre;
                                        comp.XmlString = cfdiTimbrado;
                                        comp.CadenaOriginalTimbre = timbre.cadenaOriginal;
                                        string saldo = comp.Total.ToString(CultureInfo.InvariantCulture);
                                        comp.CantidadLetra = CantidadLetra.Enletras(saldo.ToString(), comp.Moneda);
                                        comp.Regimen = empresa.RegimenFiscal;
                                        if (comp.TipoDeComprobante.ToString() == "ingreso")
                                        {
                                            comp.Titulo = "Factura";
                                        }
                                        else if (comp.TipoDeComprobante.ToString() == "egreso")
                                        {
                                            comp.Titulo = "Nota de Crédito";
                                        }
                                        string ruta = Path.Combine(ConfigurationManager.AppSettings["Salida"], empresa.RFC);
                                        if (!Directory.Exists(ruta))
                                        {
                                            Directory.CreateDirectory(ruta);
                                        }
                                        string xmlFile = Path.Combine(ruta, timbre.UUID + ".xml");
                                        ServicioTimbradoMovil.Logger.Debug(comp.XmlString);
                                        StreamWriter sw = new StreamWriter(xmlFile, false, Encoding.UTF8);
                                        sw.Write(comp.XmlString);
                                        sw.Close();
                                        byte[] pdf = new byte[0];
                                        try
                                        {
                                            long id = 0L;
                                            GeneradorCfdi gen = new GeneradorCfdi();
                                            pdf = gen.GetPdfFromComprobante(comp, empresa.Orientacion, fac.Factura.TipoDocumento, ref id, fac.Factura.Metodo);
                                            string pdfFile = Path.Combine(ruta, timbre.UUID + ".pdf");
                                            File.WriteAllBytes(pdfFile, pdf);
                                        }
                                        catch (Exception ee)
                                        {
                                            ServicioTimbradoMovil.Logger.Error(ee);
                                            if (ee.InnerException != null)
                                            {
                                                ServicioTimbradoMovil.Logger.Error(ee.InnerException);
                                            }
                                        }
                                    }
                                    string totalLetra = comp.Total.ToString(CultureInfo.InvariantCulture);
                                    string enteros;
                                    string decimales;
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
                                    int tam_var = comp.Sello.Length;
                                    string Var_Sub = comp.Sello.Substring(tam_var - 8, 8);
                                    string URL = "https://verificacfdi.facturaelectronica.sat.gob.mx/default.aspx";
                                    string cadenaCodigo = string.Concat(new string[]
									{
										URL,
										"?&id=",
										timbre.UUID,
										"&fe=",
										Var_Sub,
										"&re=",
										comp.Emisor.Rfc,
										"&rr=",
										comp.Receptor.Rfc,
										"&tt=",
										total
									});
                                    string qr = this.GetQrCode(cadenaCodigo);
                                    result2 = new TimbradoResponse
                                    {
                                        Valido = true,
                                        QrCodeBase64 = qr,
                                        CadenaTimbre = timbre.cadenaOriginal,
                                        Cfdi = cfdiTimbrado
                                    };
                                }
                                else if (timbre != null && timbre.SelloSAT == null && dict.Count == 0)
                                {
                                    string totalLetra = comp.Total.ToString(CultureInfo.InvariantCulture);
                                    string enteros;
                                    string decimales;
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
                                    int tam_var = comp.Sello.Length;
                                    string Var_Sub = comp.Sello.Substring(tam_var - 8, 8);
                                    string URL = "https://verificacfdi.facturaelectronica.sat.gob.mx/default.aspx";
                                    string cadenaCodigo = string.Concat(new string[]
									{
										URL,
										"?&id=",
										timbre.UUID,
										"&fe=",
										Var_Sub,
										"&re=",
										comp.Emisor.Rfc,
										"&rr=",
										comp.Receptor.Rfc,
										"&tt=",
										total
									});
                                    string qr = this.GetQrCode(cadenaCodigo);
                                    result2 = new TimbradoResponse
                                    {
                                        Valido = true,
                                        QrCodeBase64 = qr,
                                        CadenaTimbre = timbre.cadenaOriginal,
                                        Cfdi = result
                                    };
                                }
                                else if (dict.Count > 0)
                                {
                                    StringBuilder res = new StringBuilder();
                                    foreach (KeyValuePair<int, string> d in dict)
                                    {
                                        res.AppendLine(d.Key.ToString() + " - " + d.Value.ToString());
                                    }
                                    result2 = new TimbradoResponse
                                    {
                                        Valido = false,
                                        DescripcionError = res.ToString()
                                    };
                                }
                                else
                                {
                                    ServicioTimbradoMovil.Logger.Error("Error al abrir el comprobante:" + comprobante);
                                    result2 = new TimbradoResponse
                                    {
                                        Valido = false,
                                        DescripcionError = "Error al abrir el comprobante"
                                    };
                                }
                            }
                        }
                        else
                        {
                            ServicioTimbradoMovil.Logger.Error("Error al abrir el comprobante: " + erroresNomina);
                            result2 = new TimbradoResponse
                            {
                                Valido = false,
                                DescripcionError = erroresNomina
                            };
                        }
                    }
                }
                else
                {
                    ServicioTimbradoMovil.Logger.Error("Error al abrir el comprobante:" + comprobante);
                    result2 = new TimbradoResponse
                    {
                        Valido = false,
                        DescripcionError = "Error al abrir el comprobante"
                    };
                }
            }
            catch (FaultException ex)
            {
                ServicioTimbradoMovil.Logger.Error(ex);
                result2 = new TimbradoResponse
                {
                    Valido = false,
                    DescripcionError = ex.Message
                };
            }
            catch (Exception ex2)
            {
                ServicioTimbradoMovil.Logger.Error(ex2);
                throw new FaultException("Error al abrir el comprobante");
            }
            return result2;
        }

        private ServicioLocal.Business.Comprobante DesSerializar(XElement element)
        {
            XmlSerializer ser = new XmlSerializer(typeof(ServicioLocal.Business.Comprobante));
            string xml = element.ToString();
            StringReader reader = new StringReader(xml);
            return (ServicioLocal.Business.Comprobante)ser.Deserialize(reader);
        }

        private ServicioLocal.Business.Pagoo.Comprobante DesSerializarP(XElement element)
        {
            XmlSerializer ser = new XmlSerializer(typeof(ServicioLocal.Business.Pagoo.Comprobante));
            string xml = element.ToString();
            StringReader reader = new StringReader(xml);
            return (ServicioLocal.Business.Pagoo.Comprobante)ser.Deserialize(reader);
        }

        private ImpuestosLocales DesSerializarImpuestosLocales(XElement element)
        {
            IEnumerable<XElement> ImpL = element.Elements(this._ns + "Complemento");
            ImpuestosLocales result;
            if (ImpL != null)
            {
                IEnumerable<XElement> pag = ImpL.Elements(this._ns8 + "ImpuestosLocales");
                using (IEnumerator<XElement> enumerator = pag.GetEnumerator())
                {
                    if (enumerator.MoveNext())
                    {
                        XElement e = enumerator.Current;
                        XmlSerializer ser = new XmlSerializer(typeof(ImpuestosLocales));
                        string xml = e.ToString();
                        StringReader reader = new StringReader(xml);
                        ImpuestosLocales comLXMLComprobante = (ImpuestosLocales)ser.Deserialize(reader);
                        result = comLXMLComprobante;
                        return result;
                    }
                }
                result = null;
            }
            else
            {
                result = null;
            }
            return result;
        }

        private ServicioLocal.Business.Complemento.Pagos DesSerializarPagos(XElement element)
        {
            IEnumerable<XElement> ImpL = element.Elements(this._ns + "Complemento");
            ServicioLocal.Business.Complemento.Pagos result;
            if (ImpL != null)
            {
                IEnumerable<XElement> pag = ImpL.Elements(this._ns7 + "Pagos");
                using (IEnumerator<XElement> enumerator = pag.GetEnumerator())
                {
                    if (enumerator.MoveNext())
                    {
                        XElement e = enumerator.Current;
                        XmlSerializer ser = new XmlSerializer(typeof(ServicioLocal.Business.Complemento.Pagos));
                        string xml = e.ToString();
                        StringReader reader = new StringReader(xml);
                        ServicioLocal.Business.Complemento.Pagos comLXMLComprobante = (ServicioLocal.Business.Complemento.Pagos)ser.Deserialize(reader);
                        result = comLXMLComprobante;
                        return result;
                    }
                }
                result = null;
            }
            else
            {
                result = null;
            }
            return result;
        }

        private string GetQrCode(string cadena)
        {
            QrEncoder qrEncoder = new QrEncoder(ErrorCorrectionLevel.M);
            QrCode qrCode = qrEncoder.Encode(cadena);
            MemoryStream salida = new MemoryStream();
            GraphicsRenderer renderer = new GraphicsRenderer(new FixedModuleSize(2, QuietZoneModules.Two), Brushes.Black, Brushes.White);
            renderer.WriteToStream(qrCode.Matrix, ImageFormat.Png, salida);
            salida.Close();
            return Convert.ToBase64String(salida.ToArray());
        }

        private facturas CrearFactura(ServicioLocal.Business.Comprobante comp, int idempresa, int idcliente)
        {
            TipoDocumento t = TipoDocumento.FacturaGeneral;
            if (comp.TipoDeComprobante.ToString() == "ingreso")
            {
                t = TipoDocumento.FacturaGeneral;
            }
            if (comp.TipoDeComprobante.ToString() == "egreso")
            {
                t = TipoDocumento.NotaCredito;
            }
            facturas fact = new facturas();
            fact.TipoDocumento = t;
            fact.IdEmpresa = new int?(idempresa);
            fact.Importe = comp.Total;
            fact.IVA = new decimal?(comp.IVA);
            fact.SubTotal = new decimal?(comp.SubTotal);
            fact.Total = new decimal?(comp.Total);
            fact.idcliente = idcliente;
            fact.Fecha = Convert.ToDateTime(comp.Fecha);
            fact.Folio = comp.Folio;
            fact.Serie = comp.Serie;
            fact.nProducto = new int?(comp.Conceptos.Count<ServicioLocal.Business.ComprobanteConcepto>());
            fact.captura = DateTime.Now;
            fact.Cancelado = new short?(0);
            fact.Usuario = "MOVIL";
            fact.LugarExpedicion = comp.LugarExpedicion;
            fact.Proyecto = comp.Proyecto;
            if (comp.MetodoPago != null)
            {
                fact.Metodo = comp.MetodoPago.ToString();
            }
            fact.MonedaS = comp.Moneda;
            fact.FormaPago = comp.FormaPago;
            if (comp.TipoDeComprobante.ToString() == "Egreso")
            {
                fact.NotaCredito = true;
            }
            return fact;
        }
    }
}
