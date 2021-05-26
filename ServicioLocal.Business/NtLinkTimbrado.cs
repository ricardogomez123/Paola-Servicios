using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServicioLocalContract;

namespace ServicioLocal.Business
{
    public class NtLinkTimbrado : NtLinkBusiness
    {

        public bool ExisteTimbreUuid(string uuid)
        {
            try
            {
                using (var db = new NtLinkLocalServiceEntities())
                {
                    var timbre = db.TimbreWs33.Any(p => p.Uuid == uuid);
                    if (!timbre)
                    {
                        Logger.Info("No se encontró en tws, buscando en historico");
                        timbre = db.TimbreWsHistorico.Any(p => p.Uuid == uuid);
                        Logger.Info(timbre);
                    }
                    return timbre;
                }
            }
            catch (Exception ee)
            {
                Logger.Error(ee);
                return false;
            }
        }


        public TimbreWsHistorico ObtenerTimbreHist(string uuid)
         {
             try
             {
                 using (var db = new NtLinkLocalServiceEntities())
                 {
                     var timbre = db.TimbreWsHistorico.FirstOrDefault(p => p.Uuid == uuid);
                     return timbre;
                 }
             }
             catch (Exception ee)
             {
                 Logger.Error(ee);
                 return null;
             }
         }
        public TimbreWsHistorico ObtenerTimbreHistHash(string hash)
        {
            try
            {
                using (var db = new NtLinkLocalServiceEntities())
                {
                    var timbre = db.TimbreWsHistorico.FirstOrDefault(p => p.Hash == hash);
                    return timbre;
                }
            }
            catch (Exception ee)
            {
                Logger.Error(ee);
                return null;
            }
        }
        public TimbreWs33 ObtenerTimbre(int idTimbre)
        {
            try
            {
                using (var db = new NtLinkLocalServiceEntities())
                {
                    var timbre = db.TimbreWs33.FirstOrDefault(p => p.IdTimbre == idTimbre);
                    if (timbre == null)
                    {

                    }
                    return timbre;
                }
            }
            catch (Exception ee)
            {
                Logger.Error(ee);
                return null;
            }
        }
       

         public TimbreWs33 ObtenerTimbre(string uuid)
         {
             try
             {
                 using (var db = new NtLinkLocalServiceEntities())
                 {
                     var timbre = db.TimbreWs33.FirstOrDefault(p => p.Uuid == uuid);
                     if (timbre == null)
                     {
                         
                     }
                     return timbre;
                 }
             }
             catch (Exception ee)
             {
                 Logger.Error(ee);
                 return null;
             }
         }

         public TimbreWs33 ObtenerTimbreHash(string hash)
         {
             try
             {
                 using (var db = new NtLinkLocalServiceEntities())
                 {
                     var timbre = db.TimbreWs33.FirstOrDefault(p => p.Hash == hash);
                     return timbre;
                 }
             }
             catch (Exception ee)
             {
                 Logger.Error(ee);
                 return null;
             }
         }


         public bool ExisteTimbre(string hash)
         {
             try
             {
                 using (var db = new NtLinkLocalServiceEntities())
                 {
                     return db.TimbreWs33.Any(p => p.Hash == hash);
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

        public List<TimbreWs33> ObtenerTimbres()
         {
             try
             {
                 Logger.Info("ObtenerTimbres");
                 using (var db = new NtLinkLocalServiceEntities())
                 {
                     db.CommandTimeout = 3600;
                     var fecha = DateTime.Now.AddDays(-7);
                     var timbre = db.TimbreWs33.Where(p => p.Status == 0 && p.FechaFactura > fecha ).Take(200);
                     return timbre.ToList();
                 }
             }
             catch (Exception ee)
             {
                 Logger.Error(ee);
                 return null;
             }
         }
       
        public bool IncrementaSaldo(int idEmpresa, int idSistema)
        {
            try
            {
                using (var db = new NtLinkLocalServiceEntities())
                {
                    var sistemaMensual = db.TimbradoSistemaMensual.FirstOrDefault(p => p.IdSistema == idSistema && p.Anio == DateTime.Now.Year && p.Mes == DateTime.Now.Month);
                    if (sistemaMensual == null)
                    {
                        sistemaMensual = new TimbradoSistemaMensual()
                                         {
                                             IdSistema = idSistema,
                                             Mes = DateTime.Now.Month,
                                             Anio = DateTime.Now.Year,
                                             Timbres = 1

                                         };
                        db.TimbradoSistemaMensual.AddObject(sistemaMensual);
                    }
                    else
                    {
                        sistemaMensual.Timbres = sistemaMensual.Timbres + 1;
                        db.TimbradoSistemaMensual.ApplyCurrentValues(sistemaMensual);
                    }

                    var empresaMensual =
                        db.TimbradoEmpresaMensual.FirstOrDefault(
                            p => p.IdEmpresa == idEmpresa && p.Anio == DateTime.Now.Year && p.Mes == DateTime.Now.Month);
                    if (empresaMensual == null)
                    {
                        empresaMensual = new TimbradoEmpresaMensual()
                        {
                            IdEmpresa = idEmpresa,
                            Mes = DateTime.Now.Month,
                            Anio = DateTime.Now.Year,
                            Timbres = 1

                        };
                        db.TimbradoEmpresaMensual.AddObject(empresaMensual);
                    }
                    else
                    {
                        empresaMensual.Timbres = empresaMensual.Timbres + 1;
                        db.TimbradoEmpresaMensual.ApplyCurrentValues(empresaMensual);
                    }

                    // Debemos de decrementar el saldo de timbrado que esta en la tabla de Sistema.SaldoTimbrado, ya que este es el que se ejecuta
                    var sistema = db.Sistemas.Single(l => l.IdSistema == idSistema);
                    sistema.SaldoTimbrado = sistema.SaldoTimbrado - 1;
                    if (sistema.ConsumoTimbrado == null)//no tenia pa incrementar el saldo timbrado agregado rgv
                        sistema.ConsumoTimbrado = 1;
                    else
                    sistema.ConsumoTimbrado = sistema.ConsumoTimbrado + 1;
                    db.Sistemas.ApplyCurrentValues(sistema);
                    //rgv
                    db.SaveChanges();
                    return true;
                }

            }
            catch (Exception ee)
            {
                Logger.Error(ee.Message);
                if (ee.InnerException != null)
                    Logger.Error(ee.InnerException);
                //throw;
                return false;
            }
        }

        public bool GuardarTimbre(TimbreWs33 timbre)
        {
            try
            {
                
                using (var db = new NtLinkLocalServiceEntities())
                {
                    db.CommandTimeout = 90;
                   
                    if (timbre.IdTimbre == 0)
                    {
                        
                        db.TimbreWs33.AddObject(timbre);
                    }
                    else
                    {

                        if (timbre.StrError == "Hist")
                        {
                            var thist = db.TimbreWsHistorico.FirstOrDefault(p => p.IdTimbre == timbre.IdTimbre);
                            thist.Status = 2;
                            thist.AcuseCancelacion = timbre.AcuseCancelacion;
                            db.TimbreWsHistorico.ApplyCurrentValues(thist);
                        }
                        else
                        {
                            var t = db.TimbreWs33.FirstOrDefault(p => p.IdTimbre == timbre.IdTimbre);
                            db.TimbreWs33.ApplyCurrentValues(timbre);
                        }
                    }
                    db.SaveChanges();
                    return true;
                }

            }
            catch (Exception ee)
            {
                Logger.Error(ee.Message);
                if (ee.InnerException != null)
                    Logger.Error(ee.InnerException);
                //throw;
                return false;
            }
        }
    }
}
