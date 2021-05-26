using CertificadorWs;
using CertificadorWs.Business;
using CertificadorWs.Business.Retenciones;
using EspacioComercioExterior11;
using log4net;
using log4net.Config;
using ServicioLocal.Business;
using ServicioLocal.Business.Complemento;
using ServicioLocal.Business.Hidro;
using ServicioLocal.Business.Hidrocarburos;
using ServicioLocal.Business.Pagoo;
using ServicioLocalContract;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Web.Security;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace CertificadorAppsNtLink
{
    public class CertificadorApps : ICertificadorApps
    {
        protected static ILog Logger = LogManager.GetLogger(typeof(CertificadorApps));

        private readonly ValidadorCFDi32 _val;

        private readonly XNamespace _ns7 = "http://www.sat.gob.mx/Pagos";

        private readonly XNamespace _ns = "http://www.sat.gob.mx/cfd/3";

        private readonly XNamespace _ns2 = "http://www.sat.gob.mx/nomina12";

        private readonly XNamespace _ns8 = "http://www.sat.gob.mx/implocal";

        private readonly XNamespace _ns3 = "http://www.sat.gob.mx/ComercioExterior11";

        private readonly XNamespace _ns9 = "http://www.sat.gob.mx/ine";

        private readonly XNamespace _ns10 = "http://www.sat.gob.mx/EstadoDeCuentaCombustible12";

        private readonly XNamespace _ns89 = "http://www.sat.gob.mx/IngresosHidrocarburos10";

        private readonly XNamespace _ns39 = "http://www.sat.gob.mx/GastosHidrocarburos10";

        public string CancelaRetencion(string userName, string password, string uuid, string rfc)
        {
            string result;
            try
            {
                CertificadorApps.Logger.Debug(userName);
                MembershipUser x = NtLinkLogin.ValidateUser(userName, password);
                if (x == null)
                {
                    throw new FaultException("Nombre de usuario o contraseña incorrecta");
                }
                empresa empresa = NtLinkUsuarios.GetEmpresaByUserId(x.ProviderUserKey.ToString());
                if (empresa == null)
                {
                    throw new FaultException("300 - El usuario con el que se quiere conectar es inválido");
                }
                if (empresa.Bloqueado)
                {
                    CertificadorApps.Logger.Info(empresa.RFC + "-> Bloqueado");
                    throw new FaultException("El RFC del emisor se encuentra bloqueado, favor de ponerse en contacto con atención al cliente");
                }
                NtLinkSistema nls = new NtLinkSistema();
                Sistemas sistema = nls.GetSistema((int)empresa.idSistema.Value);
                if (sistema.Bloqueado)
                {
                    CertificadorApps.Logger.Info(sistema.Rfc + "-> Bloqueado");
                    throw new FaultException("El RFC del emisor se encuentra dado de baja, favor de ponerse en contacto con atención a clientes");
                }
                Cancelador cancelador = new Cancelador();
                string respuesta = null;
                string acuse = null;
                int resultado = cancelador.CancelarRet(uuid, rfc, ref respuesta, ref acuse);
                CertificadorApps.Logger.Info(acuse);
                if (resultado == 1201 || resultado == 1202)
                {
                    result = acuse;
                }
                else
                {
                    result = respuesta;
                }
            }
            catch (FaultException ex)
            {
                CertificadorApps.Logger.Error(ex);
                result = ex.Message;
            }
            catch (Exception ex2)
            {
                CertificadorApps.Logger.Error("Error al cancelar el comprobante:" + uuid, ex2);
                result = "Error al abrir el comprobante";
            }
            return result;
        }

        public CertificadorApps()
        {
            XmlConfigurator.Configure();
            try
            {
                this._val = new ValidadorCFDi32();
            }
            catch (Exception ee)
            {
                CertificadorApps.Logger.Error(ee);
            }
        }

        public string TimbraCfdi(string userName, string password, string comprobante)
        {
            string erroresNomina = "0";
            if (!this.ValidCredentials(userName, password))
            {
                throw new UnauthorizedAccessException("Invalid Ntlink internal user and password combination");
            }
            string result2;
            try
            {
                XElement element = XElement.Load(new StringReader(comprobante));
                ServicioLocal.Business.Comprobante comp = this.DesSerializar(element);
                if (comprobante.Contains("<ieeh:IngresosHidrocarburos"))
                {
                    string erroIH = "";
                    IngresosHidrocarburos I = this.DesSerializarIH(element, ref erroIH);
                    ValidarIngresoHidrocarburos VI = new ValidarIngresoHidrocarburos();
                    erroIH = VI.ProcesarIngresoHidrocarburos(I, comp.Version, comp.TipoDeComprobante, comp.Total);
                    if (erroIH != "0")
                    {
                        result2 = erroIH;
                        return result2;
                    }
                }
                if (comprobante.Contains("<gceh:GastosHidrocarburos"))
                {
                    string erroGH = "";
                    GastosHidrocarburos I2 = this.DesSerializarGH(element, ref erroGH);
                    ValidarGastosHidrocarburos VI2 = new ValidarGastosHidrocarburos();
                    erroGH = VI2.ProcesarGastosHidrocarburos(I2, comp.Version, comp.TipoDeComprobante);
                    if (erroGH != "0")
                    {
                        result2 = erroGH;
                        return result2;
                    }
                }
                ImpuestosLocales IL = null;
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
                        result2 = ErrorPagos;
                        return result2;
                    }
                }
                if (comprobante.Contains("<ine:INE "))
                {
                    string erroINE = "";
                    INE I3 = this.DesSerializarINE(element, ref erroINE);
                    ValidarINE VI3 = new ValidarINE();
                    erroINE = VI3.ProcesarINE(I3);
                    if (erroINE != "0")
                    {
                        result2 = erroINE;
                        return result2;
                    }
                }
                bool ComerExt = comprobante.Contains("cce11:ComercioExterior");
                ValidarCFDI33 valida = new ValidarCFDI33();
                string errorCFDI33 = valida.ProcesarCFDI33(comp, comprobante, pago10, ComerExt, IL);
                if (errorCFDI33 != "0")
                {
                    CertificadorApps.Logger.Error("Error al abrir el comprobante: " + errorCFDI33);
                    result2 = errorCFDI33;
                }
                else
                {
                    if (comprobante.Contains("<ecc12:EstadoDeCuentaCombustible"))
                    {
                        string erroECC = "";
                        EstadoDeCuentaCombustible E = this.DesSerializarECC(element, ref erroECC);
                        ValidarECC VE = new ValidarECC();
                        erroECC = VE.ProcesarECC(E, comp.TipoDeComprobante, comp.Version);
                        if (erroECC != "0")
                        {
                            result2 = erroECC;
                            return result2;
                        }
                    }
                    if (comp.Emisor != null && comp.Emisor.Rfc != null)
                    {
                        empresa empresa = TimbradoUtils.ValidarUsuarioSinSaldo(comp.Emisor.Rfc);
                        if (empresa == null)
                        {
                            CertificadorApps.Logger.Info(comp.Emisor.Rfc + " No encontrado");
                            result2 = "300 - El usuario con el que se quiere conectar es inválido";
                        }
                        else
                        {
                            string result = null;
                            TimbreFiscalDigital timbre = null;
                            string acuseSat = "";
                            string hash = null;
                            bool nomin12 = comprobante.Contains("nomina12:Nomina");
                            List<Nomina> nom = new List<Nomina>();
                            if (nomin12)
                            {
                                string erroresNom = null;
                                var nomx = this.DesSerializarNomina12(element, ref erroresNom);
                                nom.Add(nomx);
                                if (erroresNom != null)
                                {
                                    result2 = erroresNom;
                                    return result2;
                                }
                            }
                            if (ComerExt && erroresNomina == "0")
                            {
                                string erroresComer = null;
                                ValidarComercioExterior val = new ValidarComercioExterior();
                                ComercioExterior Comer = this.DesSerializarComercioExterior(element, ref erroresComer);
                                if (erroresComer != null)
                                {
                                    CertificadorApps.Logger.Error("Error al abrir el comprobante: " + erroresComer);
                                    result2 = erroresComer;
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
                                    SerializadorTimbres sert = new SerializadorTimbres();
                                    if (ConfigurationManager.AppSettings["Pruebas"] == "true")
                                    {
                                        timbre.SelloSAT = "Inválido, Ambiente de pruebas";
                                    }
                                    string res = sert.GetTimbreXml(timbre);
                                    string cfdiTimbrado = result;
                                    if (ConfigurationManager.AppSettings["EnvioSat"] == "false")
                                    {
                                        if (!TimbradoUtils.GuardaFactura(comp.Fecha, comp.Emisor.Rfc, comp.Receptor.Rfc, timbre.UUID, cfdiTimbrado, hash, empresa, false, false))
                                        {
                                            throw new Exception("Error al abrir el comprobante");
                                        }
                                    }
                                    result2 = res;
                                }
                                else
                                {
                                    if (timbre != null && timbre.SelloSAT == null && dict.Count == 0)
                                    {
                                        XElement el = XElement.Parse(result);
                                        XElement complemento = el.Elements(Constantes.CFDVersionNamespace + "Complemento").FirstOrDefault<XElement>();
                                        if (complemento != null)
                                        {
                                            XElement t = complemento.Elements(Constantes.CFDTimbreFiscalVersionNamespace + "TimbreFiscalDigital").FirstOrDefault<XElement>();
                                            if (t != null)
                                            {
                                                SidetecStringWriter sw = new SidetecStringWriter(Encoding.UTF8);
                                                t.Save(sw, SaveOptions.DisableFormatting);
                                                result2 = sw.ToString();
                                                return result2;
                                            }
                                        }
                                    }
                                    if (dict.Count > 0)
                                    {
                                        StringBuilder res2 = new StringBuilder();
                                        foreach (KeyValuePair<int, string> d in dict)
                                        {
                                            res2.AppendLine(d.Key.ToString() + " - " + d.Value.ToString());
                                        }
                                        result2 = res2.ToString();
                                    }
                                    else
                                    {
                                        CertificadorApps.Logger.Error("Error al abrir el comprobante: " + comprobante);
                                        result2 = "Error al abrir el comprobante";
                                    }
                                }
                            }
                            else
                            {
                                CertificadorApps.Logger.Error("Error al abrir el comprobante: " + erroresNomina);
                                result2 = erroresNomina;
                            }
                        }
                    }
                    else
                    {
                        CertificadorApps.Logger.Error("Error al abrir el comprobante: " + comprobante);
                        result2 = "Error al abrir el comprobante";
                    }
                }
            }
            catch (Exception ex)
            {
                CertificadorApps.Logger.Error(ex);
                result2 = "Error al abrir el comprobante";
            }
            return result2;
        }

        public string TimbraRetencion(string userName, string password, string comprobante)
        {
            if (!this.ValidCredentials(userName, password))
            {
                throw new UnauthorizedAccessException("Invalid Ntlink internal user and password combination");
            }
            string result;
            try
            {
                CertificadorApps.Logger.Debug(userName);
                Retenciones ret = TimbradoUtils.DesSerializarRetenciones(comprobante);
                empresa empresa = NtLinkUsuarios.GetEmpresaByUserId(ret.Emisor.RFCEmisor);
                result = TimbradoUtils.TimbraRetencionString(comprobante, empresa, false,false);
            }
            catch (FaultException ex)
            {
                CertificadorApps.Logger.Error(ex);
                result = ex.Message;
            }
            catch (Exception ex2)
            {
                CertificadorApps.Logger.Error("Error al abrir el comprobante:" + comprobante, ex2);
                result = "Error al abrir el comprobante";
            }
            return result;
        }

        public string Ping()
        {
            return "Pong";
        }

        private bool ValidCredentials(string userName, string password)
        {
            string configUserName = ConfigurationManager.AppSettings["InternalClientUserName"];
            string configPassword = ConfigurationManager.AppSettings["InternalClientPassword"];
            return configUserName == userName && configPassword == password;
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

        public ComercioExterior DesSerializarComercioExterior(XElement element, ref string erroresNom)
        {
            ComercioExterior result;
            try
            {
                IEnumerable<XElement> nomina12 = element.Elements(this._ns + "Complemento");
                if (nomina12 != null)
                {
                    string comerci = nomina12.First<XElement>().ToString();
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

        public IngresosHidrocarburos DesSerializarIH(XElement element, ref string erroIH)
        {
            IngresosHidrocarburos result;
            try
            {
                IEnumerable<XElement> ine = element.Elements(this._ns + "Complemento");
                if (ine != null)
                {
                    ine = ine.Elements(this._ns89 + "IngresosHidrocarburos");
                    if (ine != null)
                    {
                        using (IEnumerator<XElement> enumerator = ine.GetEnumerator())
                        {
                            if (enumerator.MoveNext())
                            {
                                XElement e = enumerator.Current;
                                XmlSerializer ser = new XmlSerializer(typeof(IngresosHidrocarburos));
                                string xml = e.ToString();
                                StringReader reader = new StringReader(xml);
                                IngresosHidrocarburos comLXMLComprobante = (IngresosHidrocarburos)ser.Deserialize(reader);
                                result = comLXMLComprobante;
                                return result;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                erroIH = ex.InnerException.Message;
                result = null;
                return result;
            }
            result = null;
            return result;
        }

        public GastosHidrocarburos DesSerializarGH(XElement element, ref string erroGH)
        {
            GastosHidrocarburos result;
            try
            {
                IEnumerable<XElement> ine = element.Elements(this._ns + "Complemento");
                if (ine != null)
                {
                    ine = ine.Elements(this._ns39 + "GastosHidrocarburos");
                    if (ine != null)
                    {
                        using (IEnumerator<XElement> enumerator = ine.GetEnumerator())
                        {
                            if (enumerator.MoveNext())
                            {
                                XElement e = enumerator.Current;
                                XmlSerializer ser = new XmlSerializer(typeof(GastosHidrocarburos));
                                string xml = e.ToString();
                                StringReader reader = new StringReader(xml);
                                GastosHidrocarburos comLXMLComprobante = (GastosHidrocarburos)ser.Deserialize(reader);
                                result = comLXMLComprobante;
                                return result;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                erroGH = ex.InnerException.Message;
                result = null;
                return result;
            }
            result = null;
            return result;
        }
    }
}
