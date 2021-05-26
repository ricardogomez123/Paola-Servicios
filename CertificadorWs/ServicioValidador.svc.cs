using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Xml;
using ServicioLocal.Business;
using ServicioLocalContract;
using log4net;
using log4net.Config;

namespace CertificadorWs
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "ServicioValidador" in code, svc and config file together.
    public class ServicioValidador : IServicioValidador
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(CertificadorService));

        public ServicioValidador()
        {
            XmlConfigurator.Configure();
        }

        public ResultadoValidacion Validar(string userName, string password, string comprobante)
        {
            Logger.Debug(userName);
            Logger.Info("Timbrando:" + comprobante);
            var x = NtLinkLogin.ValidateUser(userName, password);
            if (x == null)
            {
                throw new FaultException("Nombre de usuario o contraseña incorrecta");
            }
            try
            {
                Logger.Info("Validando: " + comprobante);
                ValidadorCfdi val = new ValidadorCfdi();
                comprobante = comprobante.Replace("\ufeff", "");
                string version = null;
                using (var reader = new XmlTextReader(new StringReader(comprobante)))
                {
                    while (reader.Read())
                    {
                        if (reader.LocalName == "Comprobante")
                        {
                            version = reader.GetAttribute("version");
                            break;
                        }
                    }
                }

                return val.Validar(comprobante, version);
            }
            catch (Exception ee)
            {
                Logger.Error(ee);
                if (ee.InnerException != null)
                    Logger.Error(ee.InnerException);
                return new ResultadoValidacion() { Valido = false, Entrada = null };
            }
        }
    }
}
