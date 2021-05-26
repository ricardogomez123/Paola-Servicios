using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using CertificadorWs.Business;
using ServicioLocal.Business;
using ServicioLocalContract;
using log4net;
using log4net.Config;

namespace CertificadorWs
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Configuracion" in code, svc and config file together.
    public class Configuracion : IConfiguracion
    {
        protected static ILog Logger = LogManager.GetLogger(typeof(CertificadorService));

        public Configuracion()
        {
            XmlConfigurator.Configure();
        }

        public void RegistraEmpresa(EmpresaNtLink nuevaEmpresa, byte[] llave, byte[] certificado, string userName, string password)
        {
            Logger.Debug(userName);
            var x = NtLinkLogin.ValidateUser(userName, password);
            if (x == null)
            {
                throw new FaultException("Nombre de usuario o contraseña incorrecta");
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
                                      Telefono = nuevaEmpresa.Telefono
                                  };
                nle.Save(emp, certificado, llave, null, null);
            }
            catch (FaultException ee)
            {
                Logger.Warn(ee);
                throw;
            }
            catch (Exception ee)
            {
                Logger.Error(ee);
                if (ee.InnerException != null)
                    Logger.Error(ee.InnerException);
                throw new FaultException("Error al registrar la empresa");
            }
        }

        public ClienteNtLink ObtenerDatosCliente(string userName, string password)
        {
            try
            {
                Logger.Debug(userName);
                var x = NtLinkLogin.ValidateUser(userName, password);
                if (x == null)
                {
                    throw new FaultException("Nombre de usuario o contraseña incorrecta");
                }
                var empresa = NtLinkUsuarios.GetEmpresaByUserId(x.ProviderUserKey.ToString());
                NtLinkSistema sis = new NtLinkSistema();
                var sistema = sis.GetSistema((int) empresa.idSistema.Value);
                if (sistema != null)
                {
                    return new ClienteNtLink
                               {
                                   Ciudad = sistema.Ciudad,
                                   Colonia = sistema.Colonia,
                                   Estado = sistema.Estado,
                                   Contacto = sistema.Contacto,
                                   Cp = sistema.Cp,
                                   RazonSocial = sistema.RazonSocial,
                                   Direccion = sistema.Direccion,
                                   Email = sistema.Email,
                                   Empresas = sistema.Empresas.HasValue ? sistema.Empresas.Value : 0,
                                   FechaContrato = sistema.FechaContrato.HasValue ? sistema.FechaContrato.Value : DateTime.MinValue,
                                   //Folios = sistema.Folios.HasValue ? sistema.Folios.Value : 0,
                                   RegimenFiscal = sistema.RegimenFiscal,
                                   Rfc = sistema.Rfc,
                                   Sucursales = sistema.Sucursales.HasValue ? sistema.Sucursales.Value : 0,
                                   Telefono = sistema.Telefono,
                                   TimbresContratados = sistema.TimbresContratados.HasValue ? sistema.TimbresContratados.Value : 0,
                                   Usuarios = sistema.Usuarios.HasValue ? sistema.Usuarios.Value : 0
                               };
                }
                return null;
            }
            catch (FaultException ee)
            {
                Logger.Warn(ee);
                throw;
            }
            catch (Exception ee)
            {
                Logger.Error(ee);
                if (ee.InnerException != null)
                    Logger.Error(ee.InnerException);
                return null;
            }
           
        }

        public List<EmpresaNtLink> ObtenerEmpresas(string userName, string password)
        {
            try
            {
                Logger.Debug(userName);
                var x = NtLinkLogin.ValidateUser(userName, password);
                if (x == null)
                {
                    throw new FaultException("Nombre de usuario o contraseña incorrecta");
                }
                var empresa = NtLinkUsuarios.GetEmpresaByUserId(x.ProviderUserKey.ToString());
                NtLinkSistema sis = new NtLinkSistema();
                var sistema = sis.GetSistema((int) empresa.idSistema.Value);
                var nle = new NtLinkEmpresa();
                var lista = nle.GetList("Administrador", empresa.IdEmpresa, sistema.IdSistema);
                if (lista != null)
                {
                    return lista.Select(p => new EmpresaNtLink()
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
                                                     //PassKey = p.PassKey
                                                 }).ToList();
                }
                return null;
            }
            catch (Exception ee)
            {
                Logger.Error(ee);
                if (ee.InnerException != null)
                    Logger.Error(ee.InnerException);
                return null;
            }
        }
    }
}
