using CertificadorWs.Business;
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
using System.Xml.Linq;
using System.Xml.Serialization;
using Planesderetiro11;
using CertificadorWs.Business.Retenciones;
using System.Web.Security;
using System.Security.Cryptography;

namespace CertificadorWs
{
    public class CertificadorService : ICertificador
    {
        private readonly XNamespace _ns = "http://www.sat.gob.mx/cfd/3";

        private readonly XNamespace _ns2 = "http://www.sat.gob.mx/nomina12";

        protected static ILog Logger = LogManager.GetLogger(typeof(CertificadorService));

        private ValidadorCFDi32 val;

        private readonly XNamespace _ns3 = "http://www.sat.gob.mx/ComercioExterior11";

        private readonly XNamespace _ns8 = "http://www.sat.gob.mx/implocal";

        private readonly XNamespace _ns7 = "http://www.sat.gob.mx/Pagos";

        private readonly XNamespace _ns89 = "http://www.sat.gob.mx/IngresosHidrocarburos10";

        private readonly XNamespace _ns39 = "http://www.sat.gob.mx/GastosHidrocarburos10";
        private readonly XNamespace _ns999 = "http://www.sat.gob.mx/esquemas/retencionpago/1";
        private readonly XNamespace _ns48 = "http://www.sat.gob.mx/esquemas/retencionpago/1/planesderetiro11";
        private readonly XNamespace _ns49 = "http://www.sat.gob.mx/esquemas/retencionpago/1/PlataformasTecnologicas10";

        private string clave = "rgv123";
  
        public CertificadorService()
        {
            XmlConfigurator.Configure();
            try
            {
                this.val = new ValidadorCFDi32();
            }
            catch (Exception ee)
            {
                CertificadorService.Logger.Error(ee);
            }
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
                erroresNom = ex.InnerException.Message;
                result = null;
                return result;
            }
            result = null;
            return result;
        }
        private string ValidarUsuario(string RFCEmisor, MembershipUser x, ref empresa empres)
        {
            //----------------------------------------------------------

            if (!string.IsNullOrEmpty(RFCEmisor))
            {

                empres = NtLinkUsuarios.GetEmpresaByUserId(x.ProviderUserKey.ToString());
                empres = TimbradoUtils.ValidarUsuarioMultiple(empres);

                if (empres == null)
                {

                    return "Error al sellar el comprobante: en los datos de la empresa-usuario";
                }
                else
                {
                    if (empres.RFC.ToUpper() != RFCEmisor.ToUpper())
                    {
                        List<empresa> Lis = NtLinkSistema.ListaEmpresasPorSistema((long)empres.idSistema);
                        if (Lis.Exists(p => p.RFC.ToUpper() == RFCEmisor.ToUpper()))
                        {
                            var em = Lis.Where(p => p.RFC.ToUpper() == RFCEmisor.ToUpper()).FirstOrDefault();
                            empres = em;
                            empres = TimbradoUtils.ValidarUsuarioMultiple(empres);

                        }
                        else
                            return "Error: El RFC de la empresa no corresponde al usuario";

                    }
                }
            }
            else
            {

                return "Error: Los datos del emisor incompletos";


            }
            //---------------------------------------------


            return "OK";
        }

        public string descifrar(string cadena)
        {
            byte[] llave;
            byte[] arreglo = Convert.FromBase64String(cadena); // Arreglo donde guardaremos la cadena descovertida.
            // Ciframos utilizando el Algoritmo MD5.
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            llave = md5.ComputeHash(UTF8Encoding.UTF8.GetBytes(clave));
            md5.Clear();
            //Ciframos utilizando el Algoritmo 3DES.
            TripleDESCryptoServiceProvider tripledes = new TripleDESCryptoServiceProvider();
            tripledes.Key = llave;
            tripledes.Mode = CipherMode.ECB;
            tripledes.Padding = PaddingMode.PKCS7;
            ICryptoTransform convertir = tripledes.CreateDecryptor();
            byte[] resultado = convertir.TransformFinalBlock(arreglo, 0, arreglo.Length);
            tripledes.Clear();
            string cadena_descifrada = UTF8Encoding.UTF8.GetString(resultado); // Obtenemos la cadena
            return cadena_descifrada; // Devolvemos la cadena
        }

        public string Activar(string Llave, string RFC)
        {
            string LL = descifrar(Llave);
            var datos = LL.Split('|');
            string Mac = datos[0];
            string Key = datos[1];

            ValidadarActivador V = new ValidadarActivador();
            var A = V.GetActivador(Key);
            if (A == null)
                return "La clave no es valida";
            if (A.Mac != null)
                return "La clave ya no es valida";
            ActivacionConvertidor act = new ActivacionConvertidor();
            act.key = Key;
            act.Mac = Mac;
            act.RFC = RFC;
            act.Activo = true;
            act.FechaActivacion = DateTime.Now;
            act.FechaAlta = A.FechaAlta;
            act.Admin = A.Admin;
            act.Id = A.Id;
            int z = V.Activar(act);
            if (z == 1)
                return "OK";
            else
                return "Error de base";
        }
        public bool ValidarLicencia(string Llave)
        {
            string LL = descifrar(Llave);
            var datos = LL.Split('|');
            string Mac = datos[0];
            string Key = datos[1];

            ValidadarActivador V = new ValidadarActivador();
            var s = V.GetActivo(Key, Mac);
            if (s == null)
                return false;
            else
                return true;
        }
        public string ValidaRFC(string RFC)
        {
            ValidadorDatos D = new ValidadorDatos();
            int sa = D.ValidaRFCLCO(RFC);
            string result;
            if (sa == 0)
            {
                result = "OK";
            }
            else
            {
                result = "Error RFC invalido, no se encuentra en la lista del LCO";
            }
            return result;
        }

        public string ValidaTimbraCfdi(string comprobante)
        {
            string result;
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
                        result = erroIH;
                        return result;
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
                        result = erroGH;
                        return result;
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
                }
                bool ComerExt = comprobante.Contains("cce11:ComercioExterior");
                ValidarCFDI33 valida = new ValidarCFDI33();
                string errorCFDI33 = valida.ProcesarCFDI33(comp, comprobante, pago10, ComerExt, IL);
                if (errorCFDI33 != "0")
                {
                    CertificadorService.Logger.Error("Error al abrir el comprobante: " + errorCFDI33);
                    result = errorCFDI33;
                }
                else
                {
                    result = "OK";
                }
            }
            catch (Exception ex)
            {
                CertificadorService.Logger.Error(ex);
                result = "Error al abrir el comprobante";
            }
            return result;
        }

        /// <summary>
        /// Timbra un comprobante
        /// </summary>
        /// <param name="comprobante">Documento para validar y timbrar</param>
        /// <returns>Una cadena con el timbre fiscal digital (TFD)</returns>
        public string TimbraCfdi(string comprobante, string userName, string password, string LLave, string aplicacion)
        {
            string result2;
            try
            {
                if (string.IsNullOrEmpty(userName))
                    return "Nombre de usuario o contraseña incorrecta";
                CertificadorService.Logger.Debug(userName);
                if (aplicacion == "CON")
                {
                    var Licencia = ValidarLicencia(LLave);
                    if (Licencia == false)
                        return "Licencia no valida";
                }
                MembershipUser x = NtLinkLogin.ValidateUser(userName, password);
                if (x == null)
                {
                    throw new FaultException("Nombre de usuario o contraseña incorrecta");
                }
           

                XElement element = XElement.Load(new StringReader(comprobante));
                ServicioLocal.Business.Comprobante comp = this.DesSerializar(element);
                empresa empres = new empresa();
                if (comp.Emisor != null && comp.Emisor.Rfc != null)
                {
                    string vemp = ValidarUsuario(comp.Emisor.Rfc, x, ref empres);
                    if (vemp != "OK")
                    {
                        return vemp;
                    }
                }
                else
                {
                    return "Error: Los datos del emisor incompletos";
                }
                //-------------------------

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
                if (comp.TipoDeComprobante == "P" && !pago10)
                {
                    result2 = "CFDI no contiene el complemento PAGO";
                    return result2;
                }
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
                    bool ComerExt = comprobante.Contains("cce11:ComercioExterior");
                    ValidarCFDI33 valida = new ValidarCFDI33();
                    string errorCFDI33 = valida.ProcesarCFDI33(comp, comprobante, pago10, ComerExt, IL);
                    if (errorCFDI33 != "0")
                    {
                        CertificadorService.Logger.Error("Error al abrir el comprobante: " + errorCFDI33);
                        return errorCFDI33;
                    }
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
                 
                                NtLinkSistema nls = new NtLinkSistema();
                                string result = null;
                                TimbreFiscalDigital timbre = null;
                                string acuseSat = "";
                                string hash = null;
                                string erroresNomina = "0";
                                if (ComerExt && erroresNomina == "0")
                                {
                                    string erroresComer = null;
                                    ValidarComercioExterior val = new ValidarComercioExterior();
                                    ComercioExterior Comer = this.DesSerializarComercioExterior(element, ref erroresComer);
                                    if (erroresComer != null)
                                    {
                                        result2 = erroresComer;
                                        return result2;
                                    }
                                    erroresNomina = val.ProcesarComercioExterior(Comer, comp);
                                }
                                if (nomin12 && erroresNomina == "0")
                                {
                                    erroresNomina = this.val.ProcesarNomina(nom, comp);
                                    if (erroresNomina != "0")
                                        return erroresNomina;
                                }
                              
                                    Dictionary<int, string> dict = this.val.ProcesarCadena(comp.Emisor.Rfc, comprobante, ref result, ref timbre, ref acuseSat, ref hash);
                                    if (timbre != null && timbre.SelloSAT != null && dict.Count == 0)
                                    {
                                        SerializadorTimbres sert = new SerializadorTimbres();
                                        if (ConfigurationManager.AppSettings["Pruebas"] == "true")
                                        {
                                            timbre.SelloSAT = "Inválido, Ambiente de pruebas";
                                        }
                                        string res = sert.GetTimbreXml(timbre);
                                        string cfdiTimbrado = result;
                                        if (ConfigurationManager.AppSettings["EnvioSat"] == "false")
                                        {
                                            if (!TimbradoUtils.GuardaFactura(comp.Fecha, comp.Emisor.Rfc, comp.Receptor.Rfc, timbre.UUID, cfdiTimbrado, hash, empres, true, false))
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
                                            CertificadorService.Logger.Error("Error al abrir el comprobante: " + comprobante);
                                            result2 = "Error al abrir el comprobante";
                                        }
                                    }
                                
                              
                                                  
                    
                
            }
            catch (Exception ex)
            {
                CertificadorService.Logger.Error(ex);
                result2 = "Error al abrir el comprobante: "+ex.Message;
            }
            return result2;
        }
        public string TimbraRetencion(string comprobante, string userName, string password, string LLave, string aplicacion)
        {
            string result;
            try
            {
                CertificadorService.Logger.Debug(userName);
                if (aplicacion == "CON")
                {
                    var Licencia = ValidarLicencia(LLave);
                    if (Licencia == false)
                        return "Licencia no valida";
                }
                MembershipUser x = NtLinkLogin.ValidateUser(userName, password);
                if (x == null)
                {
                    throw new FaultException("Nombre de usuario o contraseña incorrecta");
                }
           

                XElement element = XElement.Load(new StringReader(comprobante));
                Retenciones comp = TimbradoUtils.DesSerializarRetenciones(element);
                empresa empres = new empresa();
                if (comp.Emisor != null && comp.Emisor.RFCEmisor != null)
                {
                    string vemp = ValidarUsuario(comp.Emisor.RFCEmisor, x, ref empres);
                    if (vemp != "OK")
                    {
                        return vemp;
                    }
                }
                else
                {
                    return "Error: Los datos del emisor incompletos";
                }
                //-------------------------

                if (comprobante.Contains("<planesderetiro11:Planesderetiro"))
                {
                    Planesderetiro Plan = this.DesSerializarPR(element);
                    ValidarPR validar = new ValidarPR();
                    string errorPR = validar.ProcesarPR(Plan);
                    if (errorPR != "0")
                    {
                        result = errorPR;
                        return result;
                    }
                }
                if (comprobante.Contains("<plataformasTecnologicas:ServiciosPlataformasTecnologicas"))
                {
                    ServicioLocal.Business.ComplementoRetencion.Retenciones retencion = this.DesSerializarRetencion(element);

                    string erroECC = "";
                    ServiciosPlataformasTecnologicas E2 = this.DesSerializarSPT(element, ref erroECC);
                    ValidarSPT VE2 = new ValidarSPT();
                    erroECC = VE2.ProcesarSPT(E2, retencion);
                    if (erroECC != "0")
                    {
                        result = erroECC;
                        return result;
                    }
                }
             
                         
                  result = TimbradoUtils.TimbraRetencionString(comprobante, empres, true, true);
                         
               
            }
            catch (FaultException ex)
            {
                result = ex.Message;
            }
            catch (Exception ex2)
            {
                result = "Error al abrir el comprobante";
            }
            return result;
        }

        private ServicioLocal.Business.ComplementoRetencion.Retenciones DesSerializarRetencion(XElement element)
        {
            XmlSerializer ser = new XmlSerializer(typeof(ServicioLocal.Business.ComplementoRetencion.Retenciones));
            string xml = element.ToString();
            StringReader reader = new StringReader(xml);
            return (ServicioLocal.Business.ComplementoRetencion.Retenciones)ser.Deserialize(reader);
        }
       

        private Planesderetiro DesSerializarPR(XElement element)
        {
            IEnumerable<XElement> ImpL = element.Elements(this._ns999 + "Complemento");
            Planesderetiro result;
            if (ImpL != null)
            {
                IEnumerable<XElement> pag = ImpL.Elements(this._ns48 + "Planesderetiro");
                using (IEnumerator<XElement> enumerator = pag.GetEnumerator())
                {
                    if (enumerator.MoveNext())
                    {
                        XElement e = enumerator.Current;
                        XmlSerializer ser = new XmlSerializer(typeof(Planesderetiro));
                        string xml = e.ToString();
                        StringReader reader = new StringReader(xml);
                        Planesderetiro comLXMLComprobante = (Planesderetiro)ser.Deserialize(reader);
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

        public string ConsultaEstatusCFDI(string expresion)
        {
            Cancelador canceladorConsulta = new Cancelador();
            return canceladorConsulta.ConsultaEstatusCFDI(expresion);
        }

        /// <summary>
        /// Cancela un cfdi
        /// </summary>
        /// <param name="uuid">uuid del comprobante a cancelar</param>
        /// <param name="rfc">RFC del emisor</param>
        /// <returns>Acuse de cancelación del SAT</returns>
        public string CancelaCfdi(string uuid, string rfcEmisor, string expresion, string rfcReceptor)
        {
            Cancelador cancelador = new Cancelador();
            string respuesta = null;
            string acuse = null;
            Cancelador canceladorConsulta = new Cancelador();
            string consulta = canceladorConsulta.ConsultaCFDI(expresion, uuid, rfcReceptor);
            string result;
            if (consulta != "OK")
            {
                result = "Error al cancelar el comprobante: " + consulta;
            }
            else
            {
                NtLinkEmpresa nle = new NtLinkEmpresa(); //
                int resultado;
              
                //int numero = 0;
                //if (!int.TryParse(rfcEmisor, out numero))
                //{
                    empresa empresa = nle.GetByRfc(rfcEmisor);//
                    resultado = cancelador.Cancelar(uuid, rfcEmisor, ref respuesta, ref acuse);

                //}
                //else
                //{
                //    var emp = nle.GetById(numero);
                //    resultado = cancelador.CancelarPorID(uuid, emp.IdEmpresa, ref respuesta, ref acuse);//para repetidos rfc

                //}
                
               

               // int resultado = cancelador.Cancelar(uuid, rfcEmisor, ref respuesta, ref acuse);
               // int resultado = cancelador.Cancelar(uuid, empresa.IdEmpresa, ref respuesta, ref acuse);
               
                CertificadorService.Logger.Info(respuesta);
                CertificadorService.Logger.Info(acuse);
                CertificadorService.Logger.Info(resultado);
                if (resultado == 201 || resultado == 202)
                {
                    result = acuse;
                }
                else
                {
                    result = respuesta;
                }
            }
            return result;
        }

        public string CancelaRetencion(string uuid, string rfc)
        {
            string result;
            try
            {
                Cancelador cancelador = new Cancelador();
                string respuesta = null;
                string acuse = null;
                int resultado = cancelador.CancelarRet(uuid, rfc, ref respuesta, ref acuse);
                CertificadorService.Logger.Info(acuse);
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
                CertificadorService.Logger.Error(ex);
                result = ex.Message;
            }
            catch (Exception ex2)
            {
                CertificadorService.Logger.Error("Error al cancelar el comprobante:" + uuid, ex2);
                result = "Error al abrir el comprobante";
            }
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
        public ServiciosPlataformasTecnologicas DesSerializarSPT(XElement element, ref string erroECC)
        {
            try
            {
                IEnumerable<XElement> ImpL = element.Elements(this._ns999 + "Complemento");
                ServiciosPlataformasTecnologicas result;
                if (ImpL != null)
                {
                    IEnumerable<XElement> pag = ImpL.Elements(this._ns49 + "ServiciosPlataformasTecnologicas");
                    using (IEnumerator<XElement> enumerator = pag.GetEnumerator())
                    {
                        if (enumerator.MoveNext())
                        {
                            XElement e = enumerator.Current;
                            XmlSerializer ser = new XmlSerializer(typeof(ServiciosPlataformasTecnologicas));
                            string xml = e.ToString();
                            StringReader reader = new StringReader(xml);
                            ServiciosPlataformasTecnologicas comLXMLComprobante = (ServiciosPlataformasTecnologicas)ser.Deserialize(reader);
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
            catch (Exception ex)
            {
                erroECC = ex.InnerException.Message;

                return null;
            }

        }

 
    }
}
