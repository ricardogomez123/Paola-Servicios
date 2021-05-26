using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading; 
using SAT.CFDI.Cliente.Procesamiento;
using ServicioLocal.Business;
using ServicioLocalContract;
using log4net;

namespace PACEnviadorSATConsole
{
    public class ProcesoTimbre
    {
        public Thread t;
        private static readonly ILog Log = LogManager.GetLogger(typeof(ProcesoTimbre));
        private bool _activo;
        public void Iniciar()
        {
            _activo = true;

            string rutaBase = AppDomain.CurrentDomain.BaseDirectory +
                              ConfigurationManager.AppSettings["PathXMLTemporales"];
            Log.Info(rutaBase);
            if (!Directory.Exists(rutaBase))
                Directory.CreateDirectory(rutaBase);

            t = new Thread(LecturaBase);          
            t.Start();
        }

        public void LecturaBase()
        {
            try
            { 
                NtLinkTimbrado timbradoData = new NtLinkTimbrado();
                ProcesoEnvioSAT proLEnvioSAT = new ProcesoEnvioSAT();
                string strLRutaValidacion;
                
                int intMRegistros = Convert.ToInt32(ConfigurationManager.AppSettings["NumRegistros"]);
                if (intMRegistros <= 0) intMRegistros = 1000;
                Log.Info("NumRegistros");
                while (_activo)
                {
                    try
                    { 
                        //lectura a base de datos
                        Log.Info("Obtener datos");
                        List<TimbreWs33> topLListaComp = timbradoData.ObtenerTimbres();
          
                        if (topLListaComp.Count() > 0)
                        {
                            Log.Info("Se enviaran " + topLListaComp.Count() + " CFD's al SAT");
                            foreach (var topComprobante in topLListaComp) {
                                if (topComprobante.Retenciones == true)//se agrego para retenciones rgv
                                    proLEnvioSAT.EnvioSatRet(topComprobante);//se agrego para retenciones rgv
                                else//se agrego para retenciones rgv
                                proLEnvioSAT.EnvioSAT(topComprobante); 
                            } 
                        }
                        else
                        {
                            Log.Info("No existen CFD's por enviar el proceso se pausará: "  + 
                                        Convert.ToInt32(ConfigurationManager.AppSettings["TiempoDormido"]) / 1000 + " segundos.");
                             
                            Thread.Sleep(Convert.ToInt32(ConfigurationManager.AppSettings["TiempoDormido"]));
                        }
                    }
                    catch (Exception ex)
                    {
                         Log.Error("(Lectura Base) Error interno: " + ex);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("(Lectura Base) Error de inicio: " + ex);
            }
        }

        public void Detener()
        {
            _activo = false;
            t.Join();
        }
    }
}
