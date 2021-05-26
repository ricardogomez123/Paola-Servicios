using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceProcess;
using System.Text;
using PACEnviadorSATConsole;
using log4net;

namespace ServicioEnviador
{
    public partial class Enviador77 : ServiceBase
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(Enviador77));
        private ProcesoTimbre proctimbre;

        public Enviador77()
        {
          
            InitializeComponent();
            log4net.Config.XmlConfigurator.Configure();
        }

        protected override void OnStart(string[] args)
        {
            try
            {

//#if DEBUG
//                Debugger.Launch();
//#endif     

                proctimbre = new ProcesoTimbre();
                proctimbre.Iniciar();
            }
            catch (Exception ex)
            {
                Log.Error("Error al iniciar el servicio: "+ ex);
            } 
        }

        protected override void OnStop()
        {
            try
            { 
                proctimbre.Detener();
            }
            catch (Exception ex)
            {
                Log.Error("Error al detener el servicio: " + ex);
            }
        }
    }
}
