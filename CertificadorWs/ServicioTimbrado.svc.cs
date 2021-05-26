using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;
using CertificadorWs.Business;
using CertificadorWs.Business.Retenciones;
using MessagingToolkit.QRCode.Codec;
using ServicioLocal.Business;
using ServicioLocalContract;
using log4net;
using log4net.Config;

using EspacioComercioExterior11;
using Gma.QrCodeNet.Encoding;
using Gma.QrCodeNet.Encoding.Windows.Render;
using System.Security.Cryptography.X509Certificates;
using System.Web.Security;
using ServicioLocal.Business.Hidro;
using ServicioLocal.Business.Hidrocarburos;
using Planesderetiro11;


namespace CertificadorWs
{


    public class ServicioTimbrado : IServicioTimbrado
    {
        /// <summary>
        /// Regresa el saldo de timbrado del cliente
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        private readonly XNamespace _ns = "http://www.sat.gob.mx/cfd/3";

        private readonly XNamespace _ns2 = "http://www.sat.gob.mx/nomina12";

        private readonly XNamespace _ns999 = "http://www.sat.gob.mx/esquemas/retencionpago/1";

        private static readonly ILog Logger = LogManager.GetLogger(typeof(CertificadorService));

        private readonly ValidadorCFDi32 _val;

        private readonly XNamespace _ns3 = "http://www.sat.gob.mx/ComercioExterior11";

        private readonly XNamespace _ns48 = "http://www.sat.gob.mx/esquemas/retencionpago/1/planesderetiro11";
        private readonly XNamespace _ns49 = "http://www.sat.gob.mx/esquemas/retencionpago/1/PlataformasTecnologicas10";

        private readonly XNamespace _ns8 = "http://www.sat.gob.mx/implocal";

        private readonly XNamespace _ns7 = "http://www.sat.gob.mx/Pagos";

        private readonly XNamespace _ns9 = "http://www.sat.gob.mx/ine";

        private readonly XNamespace _ns10 = "http://www.sat.gob.mx/EstadoDeCuentaCombustible12";

        private readonly XNamespace _ns89 = "http://www.sat.gob.mx/IngresosHidrocarburos10";

        private readonly XNamespace _ns39 = "http://www.sat.gob.mx/GastosHidrocarburos10";

        private readonly XNamespace _ns40 = "http://www.sat.gob.mx/CartaPorte";

        private readonly XNamespace _ns13 = "http://www.sat.gob.mx/IdeRecMinGast";

        public int ConsultaSaldo(string userName, string password)
        {
            ServicioTimbrado.Logger.Debug(userName);
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
            NtLinkSistema sis = new NtLinkSistema();
            Sistemas sistema = sis.GetSistema((int)empresa.idSistema.Value);
            return sistema.SaldoTimbrado;
        }

        /// <summary>
        /// Registro de una empresa en el sistema de timbrado
        /// </summary>
        /// <param name="userName">Nombre de usuario (proporcionado por NT LINK)</param>
        /// <param name="password">Contraseña (proporcionado por NT LINK)</param>
        /// <param name="nuevaEmpresa">Objeto de tipo Empresa para dar de alta</param>
        public void RegistraEmpresa(string userName, string password, EmpresaNtLink nuevaEmpresa)
        {
            ServicioTimbrado.Logger.Debug(userName);
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
                ServicioTimbrado.Logger.Info(empresa.RFC + "-> Bloqueado");
                throw new FaultException("El RFC del emisor se encuentra bloqueado, favor de ponerse en contacto con atención al cliente");
            }
            NtLinkSistema nls = new NtLinkSistema();
            Sistemas sistema = nls.GetSistema((int)empresa.idSistema.Value);
            if (sistema.Bloqueado)
            {
                ServicioTimbrado.Logger.Info(sistema.Rfc + "-> Bloqueado");
                throw new FaultException("El RFC del emisor se encuentra bloqueado, favor de ponerse en contacto con atención al cliente");
            }
            NtLinkEmpresa nle = new NtLinkEmpresa();
            try
            {
                empresa emp = new empresa
                {
                    RazonSocial = nuevaEmpresa.RazonSocial,
                    RFC = nuevaEmpresa.Rfc,
                    CURP = nuevaEmpresa.Curp,
                    Ciudad = nuevaEmpresa.Ciudad,
                    Colonia = nuevaEmpresa.Colonia,
                    CP = nuevaEmpresa.Cp,
                    Contacto = nuevaEmpresa.Contacto,
                    Direccion = nuevaEmpresa.Direccion,
                    Email = nuevaEmpresa.Email,
                    Estado = nuevaEmpresa.Estado,
                    Linea = "A",
                    RegimenFiscal = nuevaEmpresa.RegimenFiscal,
                    Telefono = nuevaEmpresa.Telefono,
                    idSistema = new long?(sistema.IdSistema)
                };
                nle.Save(emp, null);
            }
            catch (FaultException ee)
            {
                ServicioTimbrado.Logger.Warn(ee);
                throw;
            }
            catch (Exception ee2)
            {
                ServicioTimbrado.Logger.Error(ee2);
                if (ee2.InnerException != null)
                {
                    ServicioTimbrado.Logger.Error(ee2.InnerException);
                }
                throw new FaultException("Error al registrar la empresa");
            }
        }

        /// <summary>
        /// Para dar de baja una empresa en el sistema de timbrado
        /// </summary>
        /// <param name="userName">Nombre de usuario (proporcionado por NT LINK)</param>
        /// <param name="password">Contraseña (proporcionado por NT LINK)</param>
        /// <param name="rfcEmpresa">RFC de la empresa que se dará de baja</param>
        public void BajaEmpresa(string userName, string password, string rfcEmpresa)
        {
            ServicioTimbrado.Logger.Debug(userName);
            MembershipUser x = NtLinkLogin.ValidateUser(userName, password);
            if (x == null)
            {
                throw new FaultException("Nombre de usuario o contraseña incorrecta");
            }
            empresa empresa = new empresa();
            if (!TimbradoUtils.EmpresaMultipleRFC(rfcEmpresa))
            {
                empresa = TimbradoUtils.ValidarUsuario(rfcEmpresa);
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
                ServicioTimbrado.Logger.Info(empresa.RFC + "-> Bloqueado");
                throw new FaultException("El RFC del emisor se encuentra bloqueado, favor de ponerse en contacto con atención al cliente");
            }
            NtLinkSistema nls = new NtLinkSistema();
            Sistemas sistema = nls.GetSistema((int)empresa.idSistema.Value);
            if (sistema.Bloqueado)
            {
                ServicioTimbrado.Logger.Info(sistema.Rfc + "-> Bloqueado");
                throw new FaultException("El RFC del emisor se encuentra bloqueado, favor de ponerse en contacto con atención al cliente");
            }
            NtLinkEmpresa nle = new NtLinkEmpresa();
            empresa.Baja = true;
            nle.Save(empresa, null);
        }

        /// <summary>
        /// Obtiene los datos del cliente de timbrado
        /// </summary>
        /// <param name="userName">Nombre de usuario (proporcionado por NT LINK)</param>
        /// <param name="password">Contraseña (proporcionado por NT LINK)</param>
        /// <returns>Un objeto de tipo ClienteNtLink</returns>
        public ClienteNtLink ObtenerDatosCliente(string userName, string password)
        {
            ClienteNtLink result;
            try
            {
                ServicioTimbrado.Logger.Debug(userName);
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
                    ServicioTimbrado.Logger.Info(empresa.RFC + "-> Bloqueado");
                    throw new FaultException("El RFC del emisor se encuentra bloqueado, favor de ponerse en contacto con atención al cliente");
                }
                NtLinkSistema sis = new NtLinkSistema();
                Sistemas sistema = sis.GetSistema((int)empresa.idSistema.Value);
                if (sistema.Bloqueado)
                {
                    ServicioTimbrado.Logger.Info(sistema.Rfc + "-> Bloqueado");
                    throw new FaultException("El RFC del emisor se encuentra bloqueado, favor de ponerse en contacto con atención al cliente");
                }
                if (sistema != null)
                {
                    result = new ClienteNtLink
                    {
                        Ciudad = sistema.Ciudad,
                        Colonia = sistema.Colonia,
                        Estado = sistema.Estado,
                        Contacto = sistema.Contacto,
                        Cp = sistema.Cp,
                        RazonSocial = sistema.RazonSocial,
                        Direccion = sistema.Direccion,
                        Email = sistema.Email,
                        Empresas = (sistema.Empresas.HasValue ? sistema.Empresas.Value : 0),
                        FechaContrato = (sistema.FechaContrato.HasValue ? sistema.FechaContrato.Value : DateTime.MinValue),
                        RegimenFiscal = sistema.RegimenFiscal,
                        Rfc = sistema.Rfc,
                        Sucursales = (sistema.Sucursales.HasValue ? sistema.Sucursales.Value : 0),
                        Telefono = sistema.Telefono,
                        TimbresContratados = (sistema.TimbresContratados.HasValue ? sistema.TimbresContratados.Value : 0),
                        Usuarios = (sistema.Usuarios.HasValue ? sistema.Usuarios.Value : 0),
                        TimbresConsumidos = sistema.TimbresConsumidos
                    };
                }
                else
                {
                    result = null;
                }
            }
            catch (FaultException ee)
            {
                ServicioTimbrado.Logger.Warn(ee);
                throw;
            }
            catch (Exception ee2)
            {
                ServicioTimbrado.Logger.Error(ee2);
                if (ee2.InnerException != null)
                {
                    ServicioTimbrado.Logger.Error(ee2.InnerException);
                }
                result = null;
            }
            return result;
        }

        /// <summary>
        /// Obtiene un listado con las empresas que estan dadas de alta en el sistema de timbrado
        /// </summary>
        /// <param name="userName">Nombre de usuario (proporcionado por NT LINK)</param>
        /// <param name="password">Contraseña (proporcionado por NT LINK)</param>
        /// <returns></returns>
        public List<EmpresaNtLink> ObtenerEmpresas(string userName, string password)
        {
            List<EmpresaNtLink> result;
            try
            {
                ServicioTimbrado.Logger.Debug(userName);
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
                    ServicioTimbrado.Logger.Info(empresa.RFC + "-> Bloqueado");
                    throw new FaultException("El RFC del emisor se encuentra bloqueado, favor de ponerse en contacto con atención al cliente");
                }
                NtLinkSistema sis = new NtLinkSistema();
                Sistemas sistema = sis.GetSistema((int)empresa.idSistema.Value);
                if (sistema.Bloqueado)
                {
                    ServicioTimbrado.Logger.Info(sistema.Rfc + "-> Bloqueado");
                    throw new FaultException("El RFC del emisor se encuentra bloqueado, favor de ponerse en contacto con atención al cliente");
                }
                NtLinkEmpresa nle = new NtLinkEmpresa();
                List<empresa> lista = nle.GetList("Administrador", empresa.IdEmpresa, sistema.IdSistema);
                if (lista != null)
                {
                    result = (from p in lista
                              select new EmpresaNtLink
                              {
                                  Ciudad = p.Ciudad,
                                  Colonia = p.Colonia,
                                  Estado = p.Estado,
                                  Contacto = p.Contacto,
                                  Cp = p.CP,
                                  RazonSocial = p.RazonSocial,
                                  Direccion = p.Direccion,
                                  Email = p.Email,
                                  RegimenFiscal = p.RegimenFiscal,
                                  Rfc = p.RFC,
                                  Telefono = p.Telefono,
                                  Folios = empresa.TimbresConsumidos
                              }).ToList<EmpresaNtLink>();
                }
                else
                {
                    result = null;
                }
            }
            catch (FaultException ee)
            {
                ServicioTimbrado.Logger.Error(ee);
                if (ee.InnerException != null)
                {
                    ServicioTimbrado.Logger.Error(ee.InnerException);
                }
                throw;
            }
            catch (Exception ee2)
            {
                ServicioTimbrado.Logger.Error(ee2);
                if (ee2.InnerException != null)
                {
                    ServicioTimbrado.Logger.Error(ee2.InnerException);
                }
                result = null;
            }
            return result;
        }

        public ServicioTimbrado()
        {
            XmlConfigurator.Configure();
            try
            {
                this._val = new ValidadorCFDi32();
            }
            catch (Exception ee)
            {
                ServicioTimbrado.Logger.Error(ee);
            }
        }



        private string CadenaCodigoRet(Retenciones comp, string UUID)
        {

            string totalLetra = comp.Totales.montoTotRet.ToString(CultureInfo.InvariantCulture);
            string enteros;
            string decimales;
            if (totalLetra.IndexOf('.') == -1)
            {
                enteros = totalLetra;
                decimales = "0";
            }
            else
            {
                enteros = totalLetra.Substring(0, totalLetra.IndexOf('.'));
                decimales = totalLetra.Substring(totalLetra.IndexOf('.') + 1);
            }
            string cantidadletra = CantidadLetra.Enletras(totalLetra, "MXN");
            string total = enteros + "." + decimales.PadRight(2, '0');
            string cadenaCodigo;
            if (comp.Receptor.Nacionalidad == RetencionesReceptorNacionalidad.Nacional)
            {
                string rfcRec = ((RetencionesReceptorNacional)comp.Receptor.Item).RFCRecep;
                cadenaCodigo = string.Concat(new string[]
								{
									"?re=",
									comp.Emisor.RFCEmisor,
									"&rr=",
									rfcRec,
									"&tt=",
									total,
									"&id=",
									UUID.ToUpper()
								});
            }
            else
            {
                string rfcRec = ((RetencionesReceptorExtranjero)comp.Receptor.Item).NumRegIdTrib;
                cadenaCodigo = string.Concat(new string[]
								{
									"?re=",
									comp.Emisor.RFCEmisor,
									"&nr=",
									rfcRec,
									"&tt=",
									total,
									"&id=",
									UUID.ToUpper()
								});
            }
            int tam_var = comp.Sello.Length;
            string Var_Sub = comp.Sello.Substring(tam_var - 8, 8);

            return cadenaCodigo;
        }                    
                                      

        private string CadenaCodigo(Comprobante comp, string UUID)
        {
            
            string totalLetra = comp.Total.ToString(CultureInfo.InvariantCulture);
            string enteros;
            string decimales;
            if (totalLetra.IndexOf('.') == -1)
            {
                enteros = totalLetra;
                decimales = "0";
            }
            else
            {
                enteros = totalLetra.Substring(0, totalLetra.IndexOf('.'));
                decimales = totalLetra.Substring(totalLetra.IndexOf('.') + 1);
            }
            
	

            string total = enteros + "." + decimales.PadRight(2, '0');
            int tam_var = comp.Sello.Length;
            string Var_Sub = comp.Sello.Substring(tam_var - 8, 8);
            string URL = "https://verificacfdi.facturaelectronica.sat.gob.mx/default.aspx";
            string cadenaCodigo = string.Concat(new string[]
										{
											URL,
											"?&id=",
											UUID.ToUpper(),
											"&re=",
											comp.Emisor.Rfc,
											"&rr=",
											comp.Receptor.Rfc,
											"&tt=",
											total,
											"&fe=",
											Var_Sub
										});

            return cadenaCodigo;
        }
        private string ValidarUsuario(string RFCEmisor, MembershipUser x, ref empresa empres)
        {
                //----------------------------------------------------------

               if (!string.IsNullOrEmpty( RFCEmisor))
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
                                empres =em;
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
      
        /// <summary>
        /// Timbra un comprobante  
        /// </summary>
        /// <param name="userName">Nombre de usuario (proporcionado por NT LINK)</param>
        /// <param name="password">Contraseña (proporcionado por NT LINK)</param>
        /// <param name="comprobante">String UTF-8 con el contenido del comprobante</param>
        /// <returns>El CFDi Timbrado, la cadena original del complemento de certificación y el QrCode codificado en 4 bits</returns>
        public TimbradoResponse TimbraCfdiQr(string userName, string password, string comprobante)
        {
            TimbradoResponse result2;
            try
            {



                string erroresNomina = "0";
                ServicioTimbrado.Logger.Debug(userName);
                MembershipUser x = NtLinkLogin.ValidateUser(userName, password);
                if (x == null)
                {
                    throw new FaultException("Nombre de usuario o contraseña incorrecta");
                }
                XElement element = XElement.Load(new StringReader(comprobante));
                ServicioLocal.Business.Comprobante comp = this.DesSerializar(element);
                //--------------------------
                empresa empres = new empresa();
                if (comp.Emisor != null && comp.Emisor.Rfc != null)
                {
                    string vemp = ValidarUsuario(comp.Emisor.Rfc, x, ref empres);
                    if (vemp != "OK")
                    {
                        result2 = new TimbradoResponse
                        {
                            Valido = false,
                            DescripcionError = vemp
                        };
                        return result2;
                    }
                }
                else
                {
                    result2 = new TimbradoResponse
                    {
                        Valido = false,
                        DescripcionError ="Error: Los datos del emisor incompletos"
                    };
                    return result2;
                
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
                        result2 = new TimbradoResponse
                        {
                            Valido = false,
                            DescripcionError = erroIH
                        };
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
                        result2 = new TimbradoResponse
                        {
                            Valido = false,
                            DescripcionError = erroGH
                        };
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
                        result2 = new TimbradoResponse
                        {
                            Valido = false,
                            DescripcionError = erroINE
                        };
                        return result2;
                    }
                }
                ImpuestosLocales IL = null;
                if (comprobante.Contains("<implocal:ImpuestosLocales"))
                {
                    IL = this.DesSerializarImpuestosLocales(element);
                }
                //-------------------
                bool pago10 = comprobante.Contains("pago10:Pagos");
                if (comp.TipoDeComprobante == "P" && !pago10)
                {
                    result2 = new TimbradoResponse
                    {
                        Valido = false,
                        DescripcionError = "CFDI no contiene el complemento PAGO"
                    };
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
                            result2 = new TimbradoResponse
                            {
                                Valido = false,
                                DescripcionError = ErrorPagos
                            };
                            return result2;
                        }
                    }
                //-------------------------------------
                    if (comprobante.Contains("<ecc12:EstadoDeCuentaCombustible"))
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
                    bool ComerExt = comprobante.Contains("cce11:ComercioExterior");
                    ValidarCFDI33 valida = new ValidarCFDI33();
                    string errorCFDI33 = valida.ProcesarCFDI33(comp, comprobante, pago10, ComerExt, IL);
                    if (errorCFDI33 != "0")
                    {
                        result2 = new TimbradoResponse
                        {
                            Valido = false,
                            DescripcionError = errorCFDI33
                        };
                        return result2;
                    }
                   
                     bool nomin12 = comprobante.Contains("nomina12:Nomina");
                        List<Nomina> nom = null;
                      
                        if (nomin12)
                        {
                            string erroresNom = null;
                            nom = this.DesSerializarNomina12(element, ref erroresNom);
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

                       
                            
                                string result = null;
                                ServicioLocal.Business.TimbreFiscalDigital timbre = null;
                                string acuseSat = "";
                                string hash = null;
                                if (ComerExt && erroresNomina == "0")
                                {
                                    string erroresComer = null;
                                    ValidarComercioExterior val = new ValidarComercioExterior();
                                    ComercioExterior Comer = this.DesSerializarComercioExterior(element, ref erroresComer);
                                    if (erroresComer != null)
                                    {
                                        ServicioTimbrado.Logger.Error("Error al abrir el comprobante: " + erroresComer);
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
                                
                                   if (erroresNomina != "0")
                                    {

                                    ServicioTimbrado.Logger.Error("Error al abrir el comprobante: " + erroresNomina);
                                    result2 = new TimbradoResponse
                                      {
                                        Valido = false,
                                        DescripcionError = erroresNomina
                                      };
                                    return result2;
                                    }

                                }
                                    Dictionary<int, string> dict = this._val.ProcesarCadena(comp.Emisor.Rfc, comprobante, ref result, ref timbre, ref acuseSat, ref hash);
                                    if (timbre != null && timbre.SelloSAT != null && dict.Count == 0)
                                    {
                                        if (ConfigurationManager.AppSettings["Pruebas"] == "true")
                                        {
                                            timbre.SelloSAT = "Inválido, Ambiente de pruebas";
                                        }
                                        string cfdiTimbrado = result;
                                        if (ConfigurationManager.AppSettings["EnvioSat"] == "false")
                                        {
                                            if (!TimbradoUtils.GuardaFactura(comp.Fecha, comp.Emisor.Rfc, comp.Receptor.Rfc, timbre.UUID, cfdiTimbrado, hash, empres, true, false))
                                            {
                                                throw new Exception("Error al abrir el comprobante");
                                            }
                                        }
                                        string cadenaCodigo= CadenaCodigo(comp, timbre.UUID);
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
                                        string cadenaCodigo = CadenaCodigo(comp, timbre.UUID);
                                 
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
                                        ServicioTimbrado.Logger.Error("Error al abrir el comprobante:" + comprobante);
                                        result2 = new TimbradoResponse
                                        {
                                            Valido = false,
                                            DescripcionError = "Error al abrir el comprobante"
                                        };
                                    }
                              
                            
                     
                    
                
            }
            catch (FaultException ex)
            {
                ServicioTimbrado.Logger.Error(ex);
                result2 = new TimbradoResponse
                {
                    Valido = false,
                    DescripcionError = ex.Message
                };
            }
            catch (Exception ex2)
            {
                ServicioTimbrado.Logger.Error(ex2);
                throw new FaultException("Error al abrir el comprobante");
            }
            return result2;
        }

        /// <summary>
        /// Timbra un comprobante  sin sello
        /// </summary>
        /// <param name="userName">Nombre de usuario (proporcionado por NT LINK)</param>
        /// <param name="password">Contraseña (proporcionado por NT LINK)</param>
        /// <param name="comprobante">String UTF-8 con el contenido del comprobante</param>
        /// <returns>El CFDi Timbrado, la cadena original del complemento de certificación y el QrCode codificado en 4 bits</returns>
        public TimbradoResponse TimbraCfdiQrSinSello(string userName, string password, string comprobante)
        {
            TimbradoResponse result2;
            try
            {
                string erroresNomina = "0";
                ServicioTimbrado.Logger.Debug(userName);
                MembershipUser x = NtLinkLogin.ValidateUser(userName, password);
                if (x == null)
                {
                    throw new FaultException("Nombre de usuario o contraseña incorrecta");
                }
                XElement element = XElement.Load(new StringReader(comprobante));
                ServicioLocal.Business.Comprobante comp = this.DesSerializar(element);
                //--------------------------
                empresa empres = new empresa();
                if (comp.Emisor != null && comp.Emisor.Rfc != null)
                {
                    string vemp = ValidarUsuario(comp.Emisor.Rfc, x, ref empres);
                    if (vemp != "OK")
                    {
                        result2 = new TimbradoResponse
                        {
                            Valido = false,
                            DescripcionError = vemp
                        };
                        return result2;
                    }
                }
                else
                {
                    result2 = new TimbradoResponse
                    {
                        Valido = false,
                        DescripcionError = "Error: Los datos del emisor incompletos"
                    };
                    return result2;
                }
                //----------------------
                          
                    if (comprobante.Contains("<ieeh:IngresosHidrocarburos"))
                    {
                        string erroIH = "";
                        IngresosHidrocarburos I = this.DesSerializarIH(element, ref erroIH);
                        ValidarIngresoHidrocarburos VI = new ValidarIngresoHidrocarburos();
                        erroIH = VI.ProcesarIngresoHidrocarburos(I, comp.Version, comp.TipoDeComprobante, comp.Total);
                        if (erroIH != "0")
                        {
                            result2 = new TimbradoResponse
                            {
                                Valido = false,
                                DescripcionError = erroIH
                            };
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
                            result2 = new TimbradoResponse
                            {
                                Valido = false,
                                DescripcionError = erroGH
                            };
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
                            result2 = new TimbradoResponse
                            {
                                Valido = false,
                                DescripcionError = erroINE
                            };
                            return result2;
                        }
                    }
                    ImpuestosLocales IL = null;
                    if (comprobante.Contains("<implocal:ImpuestosLocales"))
                    {
                        IL = this.DesSerializarImpuestosLocales(element);
                    }
                   //-------------
                    bool pago10 = comprobante.Contains("pago10:Pagos");
                    if (comp.TipoDeComprobante == "P" && !pago10)
                    {
                        result2 = new TimbradoResponse
                        {
                            Valido = false,
                            DescripcionError = "CFDI no contiene el complemento PAGO"
                        };
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
                                result2 = new TimbradoResponse
                                {
                                    Valido = false,
                                    DescripcionError = ErrorPagos
                                };
                                return result2;
                            }
                        }
                //-----
                        if (comprobante.Contains("<ecc12:EstadoDeCuentaCombustible"))
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
                        bool ComerExt = comprobante.Contains("cce11:ComercioExterior");
                        bool nomin12 = comprobante.Contains("nomina12:Nomina");
                        List<Nomina> nom = null;
                        if (nomin12)
                        {
                            string erroresNom = null;
                            nom = this.DesSerializarNomina12(element, ref erroresNom);
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
                        string path = Path.Combine(ConfigurationManager.AppSettings["Resources"], empres.RFC, "Certs");
                        X509Certificate2 cert = new X509Certificate2(Path.Combine(path, "csd.cer"));
                        string rutaLlave = Path.Combine(path, "csd.key");
                        if (File.Exists(rutaLlave + ".pem"))
                        {
                            rutaLlave += ".pem";
                        }
                        ServicioTimbrado.Logger.Debug("Ruta Llave " + rutaLlave);
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
                            string sellado = gen.GenerarCfdSinTimbre(comp, cert, rutaLlave, empres.PassKey, comprobante);
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
                                if (ComerExt && erroresNomina == "0")
                                {
                                    string erroresComer = null;
                                    ValidarComercioExterior val = new ValidarComercioExterior();
                                    ComercioExterior Comer = this.DesSerializarComercioExterior(element, ref erroresComer);
                                    if (erroresComer != null)
                                    {
                                        ServicioTimbrado.Logger.Error("Error al abrir el comprobante: " + erroresComer);
                                        result2 = new TimbradoResponse
                                        {
                                            Valido = false,
                                            DescripcionError = erroresComer
                                        };
                                        return result2;
                                    }
                                    erroresNomina = val.ProcesarComercioExterior(Comer, comp);
                                    if (erroresNomina != "0")
                                    {
                                        ServicioTimbrado.Logger.Error("Error al abrir el comprobante: " + erroresNomina);
                                        result2 = new TimbradoResponse
                                        {
                                            Valido = false,
                                            DescripcionError = erroresNomina
                                        };
                                        return result2;
                                    }
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
                                    return result2;
                                }

                               
                                    string result = null;
                                    ServicioLocal.Business.TimbreFiscalDigital timbre = null;
                                    string acuseSat = "";
                                    string hash = null;
                                    if (nomin12 && erroresNomina == "0")
                                    {
                                        erroresNomina = this._val.ProcesarNomina(nom, comp);
                                    
                                       if (erroresNomina != "0")
                                       {
                                             ServicioTimbrado.Logger.Error("Error al abrir el comprobante: " + erroresNomina);
                                        result2 = new TimbradoResponse
                                        {
                                            Valido = false,
                                            DescripcionError = erroresNomina
                                        };
                                       return  result2;
                                       }
                                    }
                                        Dictionary<int, string> dict = this._val.ProcesarCadena(comp.Emisor.Rfc, comprobante, ref result, ref timbre, ref acuseSat, ref hash);
                                        if (timbre != null && timbre.SelloSAT != null && dict.Count == 0)
                                        {
                                            if (ConfigurationManager.AppSettings["Pruebas"] == "true")
                                            {
                                                timbre.SelloSAT = "Inválido, Ambiente de pruebas";
                                            }
                                            string cfdiTimbrado = result;
                                            if (ConfigurationManager.AppSettings["EnvioSat"] == "false")
                                            {
                                                if (!TimbradoUtils.GuardaFactura(comp.Fecha, comp.Emisor.Rfc, comp.Receptor.Rfc, timbre.UUID, cfdiTimbrado, hash, empres, true, false))
                                                {
                                                    throw new Exception("Error al abrir el comprobante");
                                                }
                                            }
                                            string cadenaCodigo = CadenaCodigo(comp, timbre.UUID);
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
                                            string cadenaCodigo = CadenaCodigo(comp, timbre.UUID);
                                     
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
                                            ServicioTimbrado.Logger.Error("Error al abrir el comprobante:" + comprobante);
                                            result2 = new TimbradoResponse
                                            {
                                                Valido = false,
                                                DescripcionError = "Error al abrir el comprobante"
                                            };
                                        }
                                  
                        
                    
                
            }
            catch (FaultException ex)
            {
                ServicioTimbrado.Logger.Error(ex);
                result2 = new TimbradoResponse
                {
                    Valido = false,
                    DescripcionError = ex.Message
                };
            }
            catch (Exception ex2)
            {
                ServicioTimbrado.Logger.Error(ex2);
                throw new FaultException("Error al abrir el comprobante");
            }
            return result2;
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
      
        private List<Nomina> DesSerializarNomina12(XElement element, ref string erroresNom)
        {
            List<Nomina> result = new List<Nomina>();
            try
            {
                IEnumerable<XElement> nomina12 = element.Elements(this._ns + "Complemento");
                if (nomina12 != null)
                {
                    nomina12 = nomina12.Elements(this._ns2 + "Nomina");
                    if (nomina12 != null)
                    {
                        //if (nomina12.Count() > 1)
                        //{
                        //    XElement nomina121 = nomina12.First();
                        //    XElement nomina122 = nomina12.Last();
                        //}
                        foreach (XElement nomi in nomina12)
                        {

                            IEnumerable<XAttribute> version = nomi.Attributes("Version");
                            foreach (XAttribute att in version)
                            {
                                if (att.Value == "1.2")
                                {
                                    XmlSerializer ser = new XmlSerializer(typeof(Nomina));
                                    string xml = nomi.ToString();
                                    StringReader reader = new StringReader(xml);
                                    Nomina comLXMLComprobante = (Nomina)ser.Deserialize(reader);
                                    result.Add(comLXMLComprobante);



                                }
                            }
                        }
                        return result;

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

      public IdentificacionDeRecurso DesSerializarIDR(XElement element, ref string erroECC)
     {
	IdentificacionDeRecurso result;
	try
	{
		IEnumerable<XElement> ine = element.Elements(this._ns + "Complemento");
		if (ine != null)
		{
			ine = ine.Elements(this._ns13 + "IdentificacionDeRecurso");
			if (ine != null)
			{
				using (IEnumerator<XElement> enumerator = ine.GetEnumerator())
				{
					if (enumerator.MoveNext())
					{
						XElement e = enumerator.Current;
						XmlSerializer ser = new XmlSerializer(typeof(IdentificacionDeRecurso));
						string xml = e.ToString();
						StringReader reader = new StringReader(xml);
						IdentificacionDeRecurso comLXMLComprobante = (IdentificacionDeRecurso)ser.Deserialize(reader);
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
      public ServiciosPlataformasTecnologicas DesSerializarSPT(XElement element, ref string erroECC)
      {
          try{
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

      /// <summary>
        
        /// <summary>
        /// Timbra un comprobante
        /// </summary>
        /// <param name="userName">Nombre de usuario (proporcionado por NT LINK)</param>
        /// <param name="password">Contraseña (proporcionado por NT LINK)</param>
        /// <param name="comprobante">String con el contenido del CFDi codificado en UTF-8</param>
        /// <returns>El String con el complemento de certificacion (TimbreFiscalDigital)</returns>
        public string TimbraCfdi(string userName, string password, string comprobante)
        {
            string result;
            try
            {
                string erroresNomina = "0";
                ServicioTimbrado.Logger.Debug(userName);
                MembershipUser x = NtLinkLogin.ValidateUser(userName, password);
                if (x == null)
                {
                    throw new FaultException("Nombre de usuario o contraseña incorrecta");
                }
                XElement element = XElement.Load(new StringReader(comprobante));
                ServicioLocal.Business.Comprobante comp = this.DesSerializar(element);
                //--------------------------
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

                ImpuestosLocales IL = null;
                if (comprobante.Contains("<implocal:ImpuestosLocales"))
                {
                    IL = this.DesSerializarImpuestosLocales(element);
                }
                bool pago10 = comprobante.Contains("pago10:Pagos");
                if (comp.TipoDeComprobante == "P" && !pago10)
                {
                    result = "CFDI no contiene el complemento PAGO";
                    return result;
                }
                if (pago10)
                    {
                        ServicioLocal.Business.Pagoo.Comprobante pagoDatos = this.DesSerializarP(element);
                        ServicioLocal.Business.Complemento.Pagos pagoss = this.DesSerializarPagos(element);
                        ValidarPago VP = new ValidarPago();
                        string ErrorPagos = VP.ProcesarPago(comp, pagoss, pagoDatos);
                        if (ErrorPagos != "0")
                        {
                            result = ErrorPagos;
                            return result;
                        }
                    }

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
                    if (comprobante.Contains("<ine:INE "))
                    {
                        string erroINE = "";
                        INE I3 = this.DesSerializarINE(element, ref erroINE);
                        ValidarINE VI3 = new ValidarINE();
                        erroINE = VI3.ProcesarINE(I3);
                        if (erroINE != "0")
                        {
                            result = erroINE;
                            return result;
                        }
                    }
                    bool ComerExt = comprobante.Contains("cce11:ComercioExterior");
                    ValidarCFDI33 valida = new ValidarCFDI33();
                    string errorCFDI33 = valida.ProcesarCFDI33(comp, comprobante, pago10, ComerExt, IL);
                  
                    if (errorCFDI33 != "0")
                    {
                        ServicioTimbrado.Logger.Error("Error al abrir el comprobante: " + errorCFDI33);
                        result = errorCFDI33;
                        return result;
                    }
                  
                    if (comprobante.Contains("<ecc12:EstadoDeCuentaCombustible"))
                        {
                            string erroECC = "";
                            EstadoDeCuentaCombustible E = this.DesSerializarECC(element, ref erroECC);
                            ValidarECC VE = new ValidarECC();
                            erroECC = VE.ProcesarECC(E, comp.TipoDeComprobante, comp.Version);
                            if (erroECC != "0")
                            {
                                result = erroECC;
                                return result;
                            }
                        }
                        bool nomin12 = comprobante.Contains("nomina12:Nomina");
                        List<Nomina> nom = null;
                        if (nomin12)
                        {
                            string erroresNom = null;
                            nom = this.DesSerializarNomina12(element, ref erroresNom);
                            if (erroresNom != null)
                            {
                                result = erroresNom;
                                return result;
                            }
                        }
                       
                       if (ComerExt && erroresNomina == "0")
                            {
                                string erroresComer = null;
                                ValidarComercioExterior val = new ValidarComercioExterior();
                                ComercioExterior Comer = this.DesSerializarComercioExterior(element, ref erroresComer);
                                if (erroresComer != null)
                                {
                                    result = erroresComer;
                                    return result;
                                }
                                erroresNomina = val.ProcesarComercioExterior(Comer, comp);
                            }

                            if (nomin12 && erroresNomina == "0")
                            {
                                erroresNomina = this._val.ProcesarNomina(nom, comp);
                            }
                            if (erroresNomina != "0")
                            {
                                ServicioTimbrado.Logger.Error("Error al abrir el comprobante: " + erroresNomina);
                                result = erroresNomina;
                                return result;
                            }   
                               result = TimbradoUtils.TimbraCfdiString(comprobante, empres);
                                
                                             
                
            }
            catch (FaultException ex)
            {
                ServicioTimbrado.Logger.Error(ex);
                result = ex.Message;
            }
            catch (Exception ex2)
            {
                ServicioTimbrado.Logger.Error("Error al abrir el comprobante:" + comprobante, ex2);
                result = "Error al abrir el comprobante";
            }
            return result;
        }

        /// <summary>
        /// Timbra un comprobante sin sello
        /// </summary>
        /// <param name="userName">Nombre de usuario (proporcionado por NT LINK)</param>
        /// <param name="password">Contraseña (proporcionado por NT LINK)</param>
        /// <param name="comprobante">String con el contenido del CFDi codificado en UTF-8</param>
        /// <returns>El String con el complemento de certificacion (TimbreFiscalDigital)</returns>
        public string TimbraCfdiSinSello(string userName, string password, string comprobante)
        {
            string result;
            try
            {


                string erroresNomina = "0";
                ServicioTimbrado.Logger.Debug(userName);
                MembershipUser x = NtLinkLogin.ValidateUser(userName, password);
                if (x == null)
                {
                    throw new FaultException("Nombre de usuario o contraseña incorrecta");
                }
                XElement element = XElement.Load(new StringReader(comprobante));
                ServicioLocal.Business.Comprobante comp = this.DesSerializar(element);
                //--------------------------
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
                if (comprobante.Contains("<cartaporte:CartaPorte"))
                {
                    string erroGH = "";
                    CartaPorte I2 = this.DesSerializarCARTP(element, ref erroGH);
                    ValidarCartaP VI2 = new ValidarCartaP();
                    erroGH = VI2.ProcesarCarta(I2, comp);
                    if (erroGH != "0")
                    {
                        result = erroGH;
                        return result;
                    }
                }
            
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
                    //_--
                    bool pago10 = comprobante.Contains("pago10:Pagos");
                    if (comp.TipoDeComprobante == "P" && !pago10)
                    {
                        result = "CFDI no contiene el complemento PAGO";
                        return result;
                    }
                    
                        if (pago10)
                        {
                            ServicioLocal.Business.Pagoo.Comprobante pagoDatos = this.DesSerializarP(element);
                            ServicioLocal.Business.Complemento.Pagos pagoss = this.DesSerializarPagos(element);
                            ValidarPago VP = new ValidarPago();
                            string ErrorPagos = VP.ProcesarPago(comp, pagoss, pagoDatos);
                            if (ErrorPagos != "0")
                            {
                                result = ErrorPagos;
                                return result;
                            }
                        }
                    //----
                   
                   if (comprobante.Contains("<ine:INE "))
                        {
                            string erroINE = "";
                            INE I3 = this.DesSerializarINE(element, ref erroINE);
                            ValidarINE VI3 = new ValidarINE();
                            erroINE = VI3.ProcesarINE(I3);
                            if (erroINE != "0")
                            {
                                result = erroINE;
                                return result;
                            }
                        }
                        if (comprobante.Contains("<ecc12:EstadoDeCuentaCombustible"))
                        {
                            string erroECC = "";
                            EstadoDeCuentaCombustible E = this.DesSerializarECC(element, ref erroECC);
                            ValidarECC VE = new ValidarECC();
                            erroECC = VE.ProcesarECC(E, comp.TipoDeComprobante, comp.Version);
                            if (erroECC != "0")
                            {
                                result = erroECC;
                                return result;
                            }
                        }
                        //----------------verificar este metodo ya que esta fallando por catalogod de sat
                        if (comprobante.Contains("<IRMGCT:IdentificacionDeRecurso"))
                        {
                            string erroECC = "";
                            IdentificacionDeRecurso E2 = this.DesSerializarIDR(element, ref erroECC);
                            ValidarIDR VE2 = new ValidarIDR();
                            erroECC = VE2.ProcesarIDR(E2, comp.TipoDeComprobante, comp.Version);
                            if (erroECC != "0")
                            {
                                result = erroECC;
                                return result;
                            }
                        }
                        //--------------
                        
                        bool ComerExt = comprobante.Contains("cce11:ComercioExterior");
                        bool nomin12 = comprobante.Contains("nomina12:Nomina");
                        List<Nomina> nom = null;
                        if (nomin12)
                        {
                            string erroresNom = null;
                            nom = this.DesSerializarNomina12(element, ref erroresNom);
                            if (erroresNom != null)
                            {
                                result = erroresNom;
                                return result;
                            }
                        }
                       
                          if (ComerExt && erroresNomina == "0")
                            {
                                string erroresComer = null;
                                ValidarComercioExterior val = new ValidarComercioExterior();
                                ComercioExterior Comer = this.DesSerializarComercioExterior(element, ref erroresComer);
                                if (erroresComer != null)
                                {
                                    result = erroresComer;
                                    return result;
                                }
                                erroresNomina = val.ProcesarComercioExterior(Comer, comp);
                            }
                            if (nomin12 && erroresNomina == "0")
                            {
                                erroresNomina = this._val.ProcesarNomina(nom, comp);
                                if (erroresNomina != "0")
                                {
                                    ServicioTimbrado.Logger.Error("Error al abrir el comprobante: " + erroresNomina);
                                    result = erroresNomina;
                                    return result;
                                }  

                            }
                            string path = Path.Combine(ConfigurationManager.AppSettings["Resources"], empres.RFC, "Certs");
                            X509Certificate2 cert = new X509Certificate2(Path.Combine(path, "csd.cer"));
                            string rutaLlave = Path.Combine(path, "csd.key");
                            if (File.Exists(rutaLlave + ".pem"))
                            {
                                rutaLlave += ".pem";
                            }
                            ServicioTimbrado.Logger.Debug("Ruta Llave " + rutaLlave);
                            if (!File.Exists(rutaLlave))
                            {
                                result = "Error certificado de la empresa no está cargado en el sistema";
                                return result;
                            }
                           
                                GeneradorCfdi gen = new GeneradorCfdi();
                                string sellado = gen.GenerarCfdSinTimbre(comp, cert, rutaLlave, empres.PassKey, comprobante);
                                if (sellado == null)
                                {
                                    result = "Error al sellar el comprobante: al sellar";
                                    return result;
                                }
                               
                                    comprobante = sellado;
                                    ValidarCFDI33 valida = new ValidarCFDI33();
                                    string errorCFDI33 = valida.ProcesarCFDI33(comp, comprobante, pago10, ComerExt, IL);
                                    if (errorCFDI33 != "0")
                                    {
                                        ServicioTimbrado.Logger.Error("Error al abrir el comprobante: " + errorCFDI33);
                                        result = errorCFDI33;
                                    }
                                    else if (erroresNomina == "0")
                                    {
                                        result = TimbradoUtils.TimbraCfdiString(comprobante, empres);
                                    }
                                    else
                                    {
                                        ServicioTimbrado.Logger.Error("Error al abrir el comprobante: " + erroresNomina);
                                        result = erroresNomina;
                                    }
                               
                                              
                    
                
            }
            catch (FaultException ex)
            {
                ServicioTimbrado.Logger.Error(ex);
                result = ex.Message;
            }
            catch (Exception ex2)
            {
                ServicioTimbrado.Logger.Error("Error al abrir el comprobante:" + comprobante, ex2);
                result = "Error al abrir el comprobante";
            }
            return result;
        }

        /// <summary>
        /// Timbrar comprobante de retenciones 
        /// </summary>
        /// <param name="userName">Usuario del sistema de timbrado </param>
        /// <param name="password">Contraseña del usuario </param>
        /// <param name="comprobante">Comprobante de retenciones e información de pagos</param>
        /// <returns>Objeto que consta de los siguientes campos
        /// Timbre fiscal digital del comprobante recibido en caso de ser valido, o en su defecto la cadena
        /// </returns>
        public string TimbraRetencion(string userName, string password, string comprobante)
        {
            string result;
            try
            {
                ServicioTimbrado.Logger.Debug(userName);
                MembershipUser x = NtLinkLogin.ValidateUser(userName, password);
                if (x == null)
                {
                    throw new FaultException("Nombre de usuario o contraseña incorrecta");
                }
                XElement element = XElement.Load(new StringReader(comprobante));

                Retenciones comp = TimbradoUtils.DesSerializarRetenciones(element);
                //-----------------------------------------
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
                //------------------------------------------------
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
             
                    
                        result = TimbradoUtils.TimbraRetencionString(comprobante, empres, true,true);
                    
              }
            catch (FaultException ex)
            {
                ServicioTimbrado.Logger.Error(ex);
                result = ex.Message;
            }
            catch (Exception ex2)
            {
                ServicioTimbrado.Logger.Error("Error al abrir el comprobante:" + comprobante, ex2);
                result = "Error al abrir el comprobante";
            }
            return result;
        }

        /// <summary>
        /// Timbrar comprobante de retenciones 
        /// </summary>
        /// <param name="userName">Usuario del sistema de timbrado </param>
        /// <param name="password">Contraseña del usuario </param>
        /// <param name="comprobante">Comprobante de retenciones e información de pagos</param>
        /// <returns>Objeto que consta de los siguientes campos
        /// <br />
        ///  Valido: Boolean - Si el comprobante es valido, true
        /// <br />
        ///                    QrCodeBase64: Arreglo de bytes del Qr Code en formato png, codificado en base 64
        /// <br />
        ///                    CadenaTimbre: Cadena original del timbre fiscal digital
        /// <br />
        ///                    Cfdi: Comprobante de retenciones con el timbre en el campo Complemento
        /// </returns>
        public TimbradoResponse TimbraRetencionQr(string userName, string password, string comprobante)
        {
            TimbradoResponse result2;
            try
            {
                ServicioTimbrado.Logger.Debug(userName);
                MembershipUser x = NtLinkLogin.ValidateUser(userName, password);
                if (x == null)
                {
                    throw new FaultException("Nombre de usuario o contraseña incorrecta");
                }
                XElement element = XElement.Load(new StringReader(comprobante));
                Retenciones comp = TimbradoUtils.DesSerializarRetenciones(element);
                //-----------------------------------------
                empresa empres = new empresa();
                if (comp.Emisor != null && comp.Emisor.RFCEmisor != null)
                {
                    string vemp = ValidarUsuario(comp.Emisor.RFCEmisor, x, ref empres);
                    if (vemp != "OK")
                    {
                        result2 = new TimbradoResponse
                        {
                            Valido = false,
                            DescripcionError = vemp
                        };
                        return result2;
                     
                    }

                }
                else
                {
                    result2 = new TimbradoResponse
                    {
                        Valido = false,
                        DescripcionError = "Error: Los datos del emisor incompletos"
                    };
                    return result2;
                

                }
                //--------------------------------------------------
        
                        string result = null;
                        ServicioLocal.Business.TimbreRetenciones.TimbreFiscalDigital timbre = null;
                        string acuseSat = "";
                        string hash = null;
                        string rfcReceptor = string.Empty;
                        if (comp.Receptor.Nacionalidad == RetencionesReceptorNacionalidad.Nacional)
                        {
                            RetencionesReceptorNacional rec = (RetencionesReceptorNacional)comp.Receptor.Item;
                            rfcReceptor = rec.RFCRecep;
                        }
                        if (comprobante.Contains("<planesderetiro11:Planesderetiro"))
                        {
                            Planesderetiro Plan = this.DesSerializarPR(element);
                            ValidarPR validar = new ValidarPR();
                            string errorPR = validar.ProcesarPR(Plan);
                            if (errorPR != "0")
                            {
                                result2 = new TimbradoResponse
                                {
                                    Valido = false,
                                    DescripcionError = errorPR
                                };
                                return result2;
                            }
                        }
                        ValidadorDatosRetencion val = new ValidadorDatosRetencion();
                        Dictionary<int, string> dict = val.ProcesarCadena(comprobante, ref result, ref timbre, ref acuseSat, ref hash);
                        if (timbre != null && timbre.selloSAT != null && dict.Count == 0)
                        {
                            if (ConfigurationManager.AppSettings["Pruebas"] == "true")
                            {
                                timbre.selloSAT = "Inválido, Ambiente de pruebas";
                            }
                            string cfdiTimbrado = result;
                            if (ConfigurationManager.AppSettings["EnvioSat"] == "false")
                            {
                                string fechaExp = comp.FechaExp;
                                if (!TimbradoUtils.GuardaFactura(fechaExp, comp.Emisor.RFCEmisor, "", timbre.UUID, cfdiTimbrado, hash, empres, true, true))
                                {
                                    throw new Exception("Error al abrir el comprobante");
                                }
                            }
                            string cadenaCodigo = CadenaCodigoRet(comp, timbre.UUID);
                                     
                            string qr = this.GetQrCode(cadenaCodigo);
                            result2 = new TimbradoResponse
                            {
                                Valido = true,
                                QrCodeBase64 = qr,
                                CadenaTimbre = timbre.cadenaOriginal,
                                Cfdi = cfdiTimbrado
                            };
                        }
                        else if (timbre != null && timbre.selloSAT == null && dict.Count == 0)
                        {
                            string cadenaCodigo = CadenaCodigoRet(comp, timbre.UUID);
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
                            ServicioTimbrado.Logger.Error("Error al abrir el comprobante:" + comprobante);
                            result2 = new TimbradoResponse
                            {
                                Valido = false,
                                DescripcionError = "Error al abrir el comprobante"
                            };
                        }
                    
                
              
            }
            catch (FaultException ex)
            {
                ServicioTimbrado.Logger.Error(ex);
                result2 = new TimbradoResponse
                {
                    Valido = false,
                    DescripcionError = ex.Message
                };
            }
            catch (Exception ex2)
            {
                ServicioTimbrado.Logger.Error(ex2);
                throw new FaultException("Error al abrir el comprobante");
            }
            return result2;
        }

        public string CancelaRetencion(string userName, string password, string uuid, string rfc)
        {
            string result;
            try
            {
                ServicioTimbrado.Logger.Debug(userName);
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
                    ServicioTimbrado.Logger.Info(empresa.RFC + "-> Bloqueado");
                    throw new FaultException("El RFC del emisor se encuentra bloqueado, favor de ponerse en contacto con atención al cliente");
                }
                NtLinkSistema nls = new NtLinkSistema();
                Sistemas sistema = nls.GetSistema((int)empresa.idSistema.Value);
                if (sistema.Bloqueado)
                {
                    ServicioTimbrado.Logger.Info(sistema.Rfc + "-> Bloqueado");
                    throw new FaultException("El RFC del emisor se encuentra dado de baja, favor de ponerse en contacto con atención a clientes");
                }
                Cancelador cancelador = new Cancelador();
                string respuesta = null;
                string acuse = null;
                int resultado = cancelador.CancelarRet(uuid, rfc, ref respuesta, ref acuse);
                ServicioTimbrado.Logger.Info(acuse);
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
                ServicioTimbrado.Logger.Error(ex);
                result = ex.Message;
            }
            catch (Exception ex2)
            {
                ServicioTimbrado.Logger.Error("Error al cancelar el comprobante:" + uuid, ex2);
                result = "Error al abrir el comprobante";
            }
            return result;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="userName">Nombre de usuario (proporcionado por NT LINK)</param>
        /// <param name="password">Contraseña (proporcionado por NT LINK)</param>
        /// <param name="requestCancelacion">Request firmado para cancelar el comprobante, debe seguir el standard 
        /// <br />
        /// https://pruebacfdicancelacion.cloudapp.net/Cancelacion/CancelaCFDService.svc?xsd=xsd0
        /// <br />
        /// https://pruebacfdicancelacion.cloudk,.app.net/Cancelacion/CancelaCFDService.svc?xsd=xsd1
        /// </param>
        /// <returns>Acuse de cancelacion, Mensajes de error y el status de cada UUID enviado</returns>
        public RespuestaCancelacion CancelaCfdiRequest(string userName, string password, string requestCancelacion, string expresion, string uuid, string rfcReceptor)
        {
            if (userName == "abel.balandran@ochoacomercial.com")
                throw new FaultException("Usuario bloqueado(peticiones infinitas), favor de ponerse en contacto con atención al cliente");
            //---------------------------------------------------
            ServicioTimbrado.Logger.Debug(userName);
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
                ServicioTimbrado.Logger.Info(empresa.RFC + "-> Bloqueado");
                throw new FaultException("El RFC del emisor se encuentra bloqueado, favor de ponerse en contacto con atención al cliente");
            }
            NtLinkSistema nls = new NtLinkSistema();
            Sistemas sistema = nls.GetSistema((int)empresa.idSistema.Value);
            if (sistema.Bloqueado)
            {
                ServicioTimbrado.Logger.Info(sistema.Rfc + "-> Bloqueado");
                throw new FaultException("El RFC del emisor se encuentra dado de baja, favor de ponerse en contacto con atención a clientes");
            }
            //------------------------------------------------------
            Cancelador canceladorConsulta = new Cancelador();
            string consulta = canceladorConsulta.ConsultaCFDI(expresion, uuid, rfcReceptor);
            RespuestaCancelacion result;
            if (consulta != "OK")
            {
                result = new RespuestaCancelacion
                {
                    Acuse = null,
                    MensajeError = "Error al cancelar el comprobante: " + consulta,
                    StatusUuids = new List<StatusUuid>()
                };
            }
            else
            {
                try
                {
                    Cancelador cancelador = new Cancelador();
                    RespuestaCancelacion resultado = cancelador.Cancelar(requestCancelacion);
                    ServicioTimbrado.Logger.Info(resultado);
                    result = resultado;
                }
                catch (FaultException eee)
                {
                    ServicioTimbrado.Logger.Error(eee);
                    if (eee.InnerException != null)
                    {
                        ServicioTimbrado.Logger.Error(eee.InnerException);
                    }
                    result = new RespuestaCancelacion
                    {
                        Acuse = null,
                        MensajeError = "Error al cancelar el comprobante: " + eee.Message,
                        StatusUuids = new List<StatusUuid>()
                    };
                }
                catch (Exception eee2)
                {
                    ServicioTimbrado.Logger.Error(eee2);
                    if (eee2.InnerException != null)
                    {
                        ServicioTimbrado.Logger.Error(eee2.InnerException);
                    }
                    result = new RespuestaCancelacion
                    {
                        Acuse = null,
                        MensajeError = "Error al cancelar el comprobante",
                        StatusUuids = new List<StatusUuid>()
                    };
                }
            }
            return result;
        }

        /// <summary>
        /// Regresa el status de un comprobante por su UUID
        /// </summary>
        /// <param name="userName">Usuario de la aplicación</param>
        /// <param name="password">Contraseña de la aplicación</param>
        /// <param name="uuid">UUID del comprobante a consultar</param>
        /// <returns>Status del UUID, acuses de envio y cancelacion en caso de estar cancelado</returns>
        public ResultadoConsulta ObtenerStatusUuid(string userName, string password, string uuid)
        {
            ServicioTimbrado.Logger.Debug(userName);
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
                ServicioTimbrado.Logger.Info(empresa.RFC + "-> Bloqueado");
                throw new FaultException("El RFC del emisor se encuentra bloqueado, favor de ponerse en contacto con atención al cliente");
            }
            NtLinkSistema nls = new NtLinkSistema();
            Sistemas sistema = nls.GetSistema((int)empresa.idSistema.Value);
            if (sistema.Bloqueado)
            {
                ServicioTimbrado.Logger.Info(sistema.Rfc + "-> Bloqueado");
                throw new FaultException("El RFC del emisor se encuentra dado de baja, favor de ponerse en contacto con atención a clientes");
            }
            ResultadoConsulta respuesta = new ResultadoConsulta();
            NtLinkTimbrado tim = new NtLinkTimbrado();
            TimbreWs33 timbre = tim.ObtenerTimbre(uuid);
            ResultadoConsulta result;
            if (timbre == null)
            {
                ServicioTimbrado.Logger.Info(uuid + "No encontrado, buscando en historico");
                TimbreWsHistorico historico = tim.ObtenerTimbreHist(uuid);
                if (historico == null)
                {
                    respuesta.Status = StatusComprobante.NoEncontrado;
                    result = respuesta;
                    return result;
                }
                ServicioTimbrado.Logger.Info("Si se encontró");
                timbre = new TimbreWs33();
                timbre.Acuse = historico.Acuse;
                timbre.AcuseCancelacion = historico.AcuseCancelacion;
                timbre.Error = historico.Error;
                timbre.FechaCancelacion = historico.FechaCancelacion;
                timbre.FechaEnvio = historico.FechaEnvio;
                timbre.FechaFactura = historico.FechaFactura;
                timbre.Hash = historico.Hash;
                timbre.IdTimbre = historico.IdTimbre;
                timbre.RfcEmisor = historico.RfcEmisor;
                timbre.RfcReceptor = historico.RfcReceptor;
                timbre.Status = historico.Status;
                timbre.StrError = historico.StrError;
                timbre.Uuid = historico.Uuid;
                timbre.Xml = historico.Xml;
            }
            respuesta.Comprobante = timbre.Xml;
            if (timbre.Status == 0)
            {
                respuesta.Status = StatusComprobante.EnProceso;
            }
            else if (timbre.Status == 1)
            {
                respuesta.Status = StatusComprobante.Enviado;
                respuesta.AcuseEnvio = timbre.Acuse;
            }
            else if (timbre.Status == 2)
            {
                respuesta.Status = StatusComprobante.Cancelado;
                respuesta.AcuseEnvio = timbre.Acuse;
                respuesta.AcuseCancelacion = timbre.AcuseCancelacion;
            }
            result = respuesta;
            return result;
        }

        /// <summary>
        /// Regresa el status de un comprobante por su Hash
        /// </summary>
        /// <param name="userName">Usuario de la aplicación</param>
        /// <param name="password">Contraseña de la aplicación</param>
        /// <param name="uuid">Hash del comprobante a consultar</param>
        /// <returns>Status del Hash, acuses de envio y cancelacion en caso de estar cancelado</returns>
        public ResultadoConsulta ObtenerStatusHash(string userName, string password, string hash)
        {
            ServicioTimbrado.Logger.Debug(userName);
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
                ServicioTimbrado.Logger.Info(empresa.RFC + "-> Bloqueado");
                throw new FaultException("El RFC del emisor se encuentra bloqueado, favor de ponerse en contacto con atención al cliente");
            }
            NtLinkSistema nls = new NtLinkSistema();
            Sistemas sistema = nls.GetSistema((int)empresa.idSistema.Value);
            if (sistema.Bloqueado)
            {
                ServicioTimbrado.Logger.Info(sistema.Rfc + "-> Bloqueado");
                throw new FaultException("El RFC del emisor se encuentra dado de baja, favor de ponerse en contacto con atención a clientes");
            }
            string strComprobante = "";
            ResultadoConsulta respuesta = new ResultadoConsulta();
            NtLinkTimbrado tim = new NtLinkTimbrado();
            TimbreWs33 timbre = tim.ObtenerTimbreHash(hash);
            ResultadoConsulta result;
            if (timbre == null)
            {
                ServicioTimbrado.Logger.Info(hash + "No encontrado, buscando en historico");
                TimbreWsHistorico historico = tim.ObtenerTimbreHistHash(hash);
                if (historico == null)
                {
                    respuesta.Status = StatusComprobante.NoEncontrado;
                    result = respuesta;
                    return result;
                }
                try
                {
                    string directorio = Path.Combine(ConfigurationManager.AppSettings["RutaTimbrado"], historico.RfcEmisor, historico.FechaFactura.ToString("yyyyMMdd"));
                    if (!Directory.Exists(directorio))
                    {
                        Directory.CreateDirectory(directorio);
                    }
                    string fileName = Path.Combine(directorio, "Comprobante_" + historico.Uuid + ".xml");
                    strComprobante = File.ReadAllText(fileName, Encoding.UTF8);
                }
                catch (Exception ex_1CC)
                {
                }
                ServicioTimbrado.Logger.Info("Si se encontró");
                timbre = new TimbreWs33();
                timbre.Acuse = historico.Acuse;
                timbre.AcuseCancelacion = historico.AcuseCancelacion;
                timbre.Error = historico.Error;
                timbre.FechaCancelacion = historico.FechaCancelacion;
                timbre.FechaEnvio = historico.FechaEnvio;
                timbre.FechaFactura = historico.FechaFactura;
                timbre.Hash = historico.Hash;
                timbre.IdTimbre = historico.IdTimbre;
                timbre.RfcEmisor = historico.RfcEmisor;
                timbre.RfcReceptor = historico.RfcReceptor;
                timbre.Status =  historico.Status;
                timbre.StrError = historico.StrError;
                timbre.Uuid = historico.Uuid;
                timbre.Xml = strComprobante;
            }
            else
            {
                try
                {
                    string directorio = Path.Combine(ConfigurationManager.AppSettings["RutaTimbrado"], timbre.RfcEmisor, timbre.FechaFactura.ToString("yyyyMMdd"));
                    if (!Directory.Exists(directorio))
                    {
                        Directory.CreateDirectory(directorio);
                    }
                    string fileName = Path.Combine(directorio, "Comprobante_" + timbre.Uuid + ".xml");
                    strComprobante = File.ReadAllText(fileName, Encoding.UTF8);
                    timbre.Xml = strComprobante;
                }
                catch (Exception ex)
                {
                }
            }
            respuesta.Comprobante = timbre.Xml;
            if (timbre.Status == 0)
            {
                respuesta.Status = StatusComprobante.EnProceso;
            }
            else if (timbre.Status == 1)
            {
                respuesta.Status = StatusComprobante.Enviado;
                respuesta.AcuseEnvio = timbre.Acuse;
            }
            else if (timbre.Status == 2)
            {
                respuesta.Status = StatusComprobante.Cancelado;
                respuesta.AcuseEnvio = timbre.Acuse;
                respuesta.AcuseCancelacion = timbre.AcuseCancelacion;
            }
            result = respuesta;
            return result;
        }

        public string ConsultaAceptacionRechazo(string userName, string password, string rfcEmisor, string rfcReceptor)
        {
            ServicioTimbrado.Logger.Debug(userName);
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
                ServicioTimbrado.Logger.Info(empresa.RFC + "-> Bloqueado");
                throw new FaultException("El RFC del emisor se encuentra bloqueado, favor de ponerse en contacto con atención al cliente");
            }
            NtLinkSistema nls = new NtLinkSistema();
            Sistemas sistema = nls.GetSistema((int)empresa.idSistema.Value);
            if (sistema.Bloqueado)
            {
                ServicioTimbrado.Logger.Info(sistema.Rfc + "-> Bloqueado");
                throw new FaultException("El RFC del emisor se encuentra dado de baja, favor de ponerse en contacto con atención a clientes");
            }
            Cancelador canceladorConsulta = new Cancelador();
            return canceladorConsulta.ConsultaAceptacionRechazo(rfcReceptor);
        }

        public string ConsultaEstatusCFDI(string userName, string password, string expresion)
        {
            ServicioTimbrado.Logger.Debug(userName);
            MembershipUser x = NtLinkLogin.ValidateUser(userName, password);
            if (x == null)
            {
                throw new FaultException("Nombre de usuario o contraseña incorrecta");
            }
            Cancelador canceladorConsulta = new Cancelador();
            return canceladorConsulta.ConsultaEstatusCFDI(expresion);
        }

        public string ConsultaCFDIRelacionados(string userName, string password, string uuid, string rfcEmisor, string rfcReceptor)
        {
            ServicioTimbrado.Logger.Debug(userName);
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
                ServicioTimbrado.Logger.Info(empresa.RFC + "-> Bloqueado");
                throw new FaultException("El RFC del emisor se encuentra bloqueado, favor de ponerse en contacto con atención al cliente");
            }
            NtLinkSistema nls = new NtLinkSistema();
            Sistemas sistema = nls.GetSistema((int)empresa.idSistema.Value);
            if (sistema.Bloqueado)
            {
                ServicioTimbrado.Logger.Info(sistema.Rfc + "-> Bloqueado");
                throw new FaultException("El RFC del emisor se encuentra dado de baja, favor de ponerse en contacto con atención a clientes");
            }
            Cancelador canceladorConsulta = new Cancelador();
            return canceladorConsulta.ConsultaCFDIRelacionadosRequest("", rfcReceptor,rfcEmisor, uuid);
        }

        public string ProcesarRespuestaAceptacionRechazo(string userName, string password, List<Folios> uuid, string rfcEmisor, string rfcReceptor)
        {
            ServicioTimbrado.Logger.Debug(userName);
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
                ServicioTimbrado.Logger.Info(empresa.RFC + "-> Bloqueado");
                throw new FaultException("El RFC del emisor se encuentra bloqueado, favor de ponerse en contacto con atención al cliente");
            }
            NtLinkSistema nls = new NtLinkSistema();
            Sistemas sistema = nls.GetSistema((int)empresa.idSistema.Value);
            if (sistema.Bloqueado)
            {
                ServicioTimbrado.Logger.Info(sistema.Rfc + "-> Bloqueado");
                throw new FaultException("El RFC del emisor se encuentra dado de baja, favor de ponerse en contacto con atención a clientes");
            }
            string pac = ConfigurationManager.AppSettings["RFCPac"];
            Cancelador canceladorConsulta = new Cancelador();
            return canceladorConsulta.ProcesarRespuestaAceptacionRechazo(rfcReceptor, pac, uuid);
        }

        /// <summary>
        /// Cancela un CFDi
        /// </summary>
        /// <param name="userName">Usuario del sistema (con el que se accede a la pagina de administracion de empresas)</param>
        /// <param name="password">Contraseña del sistema</param>
        /// <param name="uuid">Folio Fiscal (UUID) del comprobante a cancelar</param>
        /// <param name="rfc">RFC del emisor del comprobante a cancelar</param>
        /// <returns></returns>
        public string CancelaCfdi(string userName, string password, string uuid, string rfcEmisor, string expresion, string rfcReceptor)
        {
     
           
            ServicioTimbrado.Logger.Debug(userName);
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
            //if ( empresa.RFC.ToUpper().Trim() != rfcEmisor.ToUpper().Trim())
            //{
            //    throw new FaultException("300 - El usuario con el que se quiere conectar no corresponde con el RFC");
          
            //}
            
            if (empresa.Bloqueado)
            {
                ServicioTimbrado.Logger.Info(empresa.RFC + "-> Bloqueado");
                throw new FaultException("El RFC del emisor se encuentra bloqueado, favor de ponerse en contacto con atención al cliente");
            }
            NtLinkSistema nls = new NtLinkSistema();
            Sistemas sistema = nls.GetSistema((int)empresa.idSistema.Value);
            if (sistema.Bloqueado)
            {
                ServicioTimbrado.Logger.Info(sistema.Rfc + "-> Bloqueado");
                throw new FaultException("El RFC del emisor se encuentra dado de baja, favor de ponerse en contacto con atención a clientes");
            }
             
            Cancelador canceladorConsulta = new Cancelador();
            string consulta = canceladorConsulta.ConsultaCFDI(expresion, uuid, rfcReceptor);
            string result;
            if (consulta != "OK")
            {
                result = "Error al cancelar el comprobante: " + consulta;
            }
            else
            {
                Cancelador cancelador = new Cancelador();
                string respuesta = null;
                string acuse = null;
                int resultado ;
               if( NtLinkUsuarios.GetEmpresaMultipleRFC(empresa.RFC))
                   resultado = cancelador.CancelarPorID(uuid, empresa.IdEmpresa, ref respuesta, ref acuse);//para repetidos rfc
               else
                resultado = cancelador.Cancelar(uuid, rfcEmisor, ref respuesta, ref acuse);
                
                ServicioTimbrado.Logger.Info(acuse);
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

        public string CancelaCfdiOtrosPACs(string uuid, string rfcEmisor, string expresion, string rfcReceptor, string Base64Cer, string Base64Key, string PasswordKey)
        {
            ServicioTimbrado.Logger.Debug(rfcEmisor);
            //MembershipUser x = NtLinkLogin.ValidateUser(userName, password);
            //if (x == null)
            //{
            //    throw new FaultException("Nombre de usuario o contraseña incorrecta");
            //}
            //empresa empresa = NtLinkUsuarios.GetEmpresaByUserId(x.ProviderUserKey.ToString());
            //if (empresa == null)
            //{
            //    throw new FaultException("300 - El usuario con el que se quiere conectar es inválido");
            //}
            //if (empresa.Bloqueado)
            //{
            //    ServicioTimbrado.Logger.Info(empresa.RFC + "-> Bloqueado");
            //    throw new FaultException("El RFC del emisor se encuentra bloqueado, favor de ponerse en contacto con atención al cliente");
            //}
            //NtLinkSistema nls = new NtLinkSistema();
            //Sistemas sistema = nls.GetSistema((int)empresa.idSistema.Value);
            //if (sistema.Bloqueado)
            //{
            //    ServicioTimbrado.Logger.Info(sistema.Rfc + "-> Bloqueado");
            //    throw new FaultException("El RFC del emisor se encuentra dado de baja, favor de ponerse en contacto con atención a clientes");
            //}
            Cancelador canceladorConsulta = new Cancelador();
            string consulta = canceladorConsulta.ConsultaCFDI(expresion, uuid, rfcReceptor);
            string result;
            if (consulta != "OK")
            {
                result = "Error al cancelar el comprobante: " + consulta;
            }
            else
            {
                Cancelador cancelador = new Cancelador();
                string respuesta = null;
                string acuse = null;
                int resultado = cancelador.CancelarOtrosPACs(uuid, rfcEmisor, ref respuesta, ref acuse,Base64Cer,Base64Key,PasswordKey);
                ServicioTimbrado.Logger.Info(acuse);
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

        private ServicioLocal.Business.Comprobante DesSerializar(XElement element)
        {
            XmlSerializer ser = new XmlSerializer(typeof(ServicioLocal.Business.Comprobante));
            string xml = element.ToString();
            StringReader reader = new StringReader(xml);
            return (ServicioLocal.Business.Comprobante)ser.Deserialize(reader);
        }
        private ServicioLocal.Business.ComplementoRetencion.Retenciones DesSerializarRetencion(XElement element)
        {
            XmlSerializer ser = new XmlSerializer(typeof(ServicioLocal.Business.ComplementoRetencion.Retenciones));
            string xml = element.ToString();
            StringReader reader = new StringReader(xml);
            return (ServicioLocal.Business.ComplementoRetencion.Retenciones)ser.Deserialize(reader);
        }
        private ServicioLocal.Business.Pagoo.Comprobante DesSerializarP(XElement element)
        {
            XmlSerializer ser = new XmlSerializer(typeof(ServicioLocal.Business.Pagoo.Comprobante));
            string xml = element.ToString();
            StringReader reader = new StringReader(xml);
            return (ServicioLocal.Business.Pagoo.Comprobante)ser.Deserialize(reader);
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
        public CartaPorte DesSerializarCARTP(XElement element, ref string erroGH)
        {
            CartaPorte result;
            try
            {
                IEnumerable<XElement> cart = element.Elements(this._ns + "Complemento");
                if (cart != null)
                {
                    cart = cart.Elements(this._ns40 + "CartaPorte");
                    if (cart != null)
                    {
                        using (IEnumerator<XElement> enumerator = cart.GetEnumerator())
                        {
                            if (enumerator.MoveNext())
                            {
                                XElement e = enumerator.Current;
                                XmlSerializer ser = new XmlSerializer(typeof(CartaPorte));
                                string xml = e.ToString();
                                StringReader reader = new StringReader(xml);
                                CartaPorte comLXMLComprobante = (CartaPorte)ser.Deserialize(reader);
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
