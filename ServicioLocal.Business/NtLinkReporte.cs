using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServicioLocalContract;

namespace ServicioLocal.Business
{
    public class NtLinkReporte : NtLinkBusiness
    {

       


        public static List<ElementoReporte> ObtenerReporte(int mes, int anio)
        {
            try
            {
                using (var db = new NtLinkLocalServiceEntities())
                {
                    if (mes == 0)
                    {
                        var timbres = db.vTimbresSistemaAnio.Where(p => p.Anio == anio).ToList();
                        return timbres.Select(
                            p =>
                            new ElementoReporte()
                            {
                                Cancelados = db.vventas.Count(
                                                    f =>
                                                    f.Fecha.Year == anio &&
                                                    f.RfcEmisor == p.Rfc &&
                                                    f.Cancelado == 1),
                                Cliente = p.RazonSocial,
                                Emitidos = p.Timbres.Value,
                                Rfc = p.Rfc
                            }).
                            ToList();
                    }
                    else
                    {
                        var timbres =
                            db.vTimbresSistema.Where(p => p.Mes == mes && p.Anio == anio).
                                ToList();
                        
                        return timbres.Select(
                            p =>
                            new ElementoReporte()
                                {Cancelados = db.vventas.Count(
                                                     f =>
                                                     f.Fecha.Month == mes &&
                                                     f.Fecha.Year == anio &&
                                                     f.RfcEmisor == p.Rfc &&
                                                     f.Cancelado == 1), 
                                    Cliente = p.RazonSocial, Emitidos = p.Timbres.Value, Rfc = p.Rfc}).
                            ToList();
                    }
                }
            }
            catch (Exception ee)
            {
                Logger.Error(ee.Message);
                if (ee.InnerException != null)
                    Logger.Error(ee.InnerException);
                return null;
            }
        }

        public static List<ElementoReporte> ObtenerReportePorCliente(int mes, int anio, int idSistema)
        {
            try
            {
                using (var db = new NtLinkLocalServiceEntities())
                {

                    if (mes == 0)
                    {
                        var timbres = db.vTimbresSistemaAnio.Where(p => p.Anio == anio && p.IdSistema == idSistema).ToList();
                        return timbres.Select(
                            p =>
                            new ElementoReporte()
                            {
                                Cancelados = db.vventas.Count(
                                                   f =>
                                                   f.Fecha.Year == anio &&
                                                   f.RfcEmisor == p.Rfc &&
                                                   f.Cancelado == 1),
                                Cliente = p.RazonSocial,
                                Emitidos = p.Timbres.Value,
                                Rfc = p.Rfc
                            }).ToList();
                    }
                    else
                    {
                        var timbres =
                            db.vTimbresSistema.Where(p => p.Mes == mes && p.Anio == anio && p.IdSistema == idSistema).
                                ToList();
                       return timbres.Select(
                            p =>
                            new ElementoReporte()
                            {
                                Cancelados = db.vventas.Count(
                                                   f =>
                                                   f.Fecha.Year == anio &&
                                                   f.RfcEmisor == p.Rfc &&
                                                   f.Cancelado == 1),
                                Cliente = p.RazonSocial,
                                Emitidos = p.Timbres.Value,
                                Rfc = p.Rfc
                            }).ToList();
                    }
                }
            }
            catch (Exception ee)
            {
                Logger.Error(ee.Message);
                if (ee.InnerException != null)
                    Logger.Error(ee.InnerException);
                return null;
            }

        }

        public static List<ElementoReporte> ObtenerReportePorEmisor(int mes, int anio, int idEmpresa)
        {
            var listaReporte = new List<ElementoReporte>();
            using (var db = new NtLinkLocalServiceEntities())
            {
                var emisor = db.empresa.First(e => e.IdEmpresa == idEmpresa);
                var elementoReporte = new ElementoReporte
                                          {
                                              Cancelados = db.vventas.Count(
                                                    f =>
                                                    f.Fecha.Year == anio &&
                                                    f.RfcEmisor == emisor.RFC &&
                                                    f.Cancelado == 1),
                                              Cliente = emisor.RazonSocial,
                                              Rfc = emisor.RFC,
                                              Emitidos =
                                                  db.TimbreWs33.Count(
                                                      f =>
                                                      f.FechaFactura.Month == mes &&
                                                      f.FechaFactura.Year == anio &&
                                                      f.RfcEmisor.Equals(emisor.RFC,
                                                                         StringComparison.
                                                                             InvariantCultureIgnoreCase))
                                          };
                listaReporte.Add(elementoReporte);
            }
            return listaReporte;
        }

        public static List<ElementoReporte> ObtenerReporteFullEmisor(int mes, int anio, int idSistema)
        {
            try
            {
                using (var db = new NtLinkLocalServiceEntities())
                {
                    var emisor = db.empresa.Where(e => e.idSistema == idSistema).ToList();
                    var ventas = db.vventas.OrderBy(p => p.IdEmpresa);
                    if (mes != 0)
                    {
                        return emisor.Select(p =>
                                             new ElementoReporte()
                                                 {
                                                     Cancelados = ventas.Count(
                                                         f =>
                                                         f.Fecha.Year == anio &&
                                                         f.Fecha.Month == mes &&
                                                         f.RfcEmisor.Equals(p.RFC,
                                                                            StringComparison.
                                                                                InvariantCultureIgnoreCase) &&
                                                         f.Cancelado == 1),
                                                     Cliente = p.RazonSocial,
                                                     Rfc = p.RFC,
                                                     Emitidos =
                                                         db.TimbreWs33.Count(
                                                             f =>
                                                             f.FechaFactura.Month == mes &&
                                                             f.FechaFactura.Year == anio &&
                                                             f.RfcEmisor.Equals(p.RFC,
                                                                                StringComparison.
                                                                                    InvariantCultureIgnoreCase))
                                                 }).ToList();
                    }
                    else
                    {
                        return emisor.Select(p =>
                                         new ElementoReporte()
                                         {
                                             Cancelados = ventas.Count(
                                                    f =>
                                                    f.RfcEmisor.Equals(p.RFC,
                                                                        StringComparison.
                                                                            InvariantCultureIgnoreCase) &&
                                                    f.Cancelado == 1),
                                             Cliente = p.RazonSocial,
                                             Rfc = p.RFC,
                                             Emitidos =
                                                 db.TimbreWs33.Count(
                                                     f =>
                                                     f.RfcEmisor.Equals(p.RFC,
                                                                        StringComparison.
                                                                            InvariantCultureIgnoreCase))
                                         }).ToList();
                    }

                }
            }
            catch (Exception ee)
            {
                Logger.Error(ee.Message);
                if (ee.InnerException != null)
                    Logger.Error(ee.InnerException);
                return null;
            }

        }

        
    }
}
