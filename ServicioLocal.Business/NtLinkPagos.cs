using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServicioLocalContract;

namespace ServicioLocal.Business
{
    public class NtLinkPagos : NtLinkBusiness
    {
        public List<vFacturasPagos>ListaAplicados(int idPago)
        {
            try
            {
                using (var db = new NtLinkLocalServiceEntities())
                {
                    var lista =
                        db.vFacturasPagos.Where(p => p.IdPago == idPago).ToList();
                    return lista;
                }
            }
            catch (Exception ee)
            {
                Logger.Error(ee);
                if (ee.InnerException != null)
                    Logger.Error(ee.InnerException);
                return null;
            }
        }

        public List<Pagos> ListaPagos(int idCliente, DateTime fecha1, DateTime fecha2)
        {
            try
            {
                using (var db = new NtLinkLocalServiceEntities())
                {
                    var lista =
                        db.Pagos.Where(p => p.IdCliente == idCliente && (p.Fecha >= fecha1 && p.Fecha <= fecha2)).ToList();
                    return lista.ToList();
                }
            }
            catch (Exception ee)
            {
                Logger.Error(ee);
                if (ee.InnerException != null)
                    Logger.Error(ee.InnerException);
                return null;
            }
        }

        public int GuardarPago(Pagos pago)
        {
            try
            {
                using (var db = new NtLinkLocalServiceEntities())
                {
                    db.Pagos.AddObject(pago);
                    db.SaveChanges();
                    return pago.IdPago;
                }
            }
            catch (Exception ee)
            {
                Logger.Error(ee);
                if (ee.InnerException != null)
                    Logger.Error(ee.InnerException);
                return 0;
            }
        }


        public int GuardarPago(decimal importe, DateTime fecha, string observaciones, decimal pendiente, string referencia)
        {
            try
            {
                using (var db = new NtLinkLocalServiceEntities())
                {
                    var pago = new Pagos
                    {
                        Fecha = fecha,
                        Importe = importe,
                        Observaciones = observaciones,
                        Referencia = referencia,
                        Pendiente = pendiente

                    };
                    db.Pagos.AddObject(pago);
                    db.SaveChanges();
                    return pago.IdPago;
                }
            }
            catch (Exception ee)
            {
                Logger.Error(ee);
                if (ee.InnerException != null)
                    Logger.Error(ee.InnerException);
                return -1;
            }
        }


        public bool CancelarPago(int idPago)
        {
            try
            {
                using (var db = new NtLinkLocalServiceEntities())
                {
                    var pago = db.Pagos.FirstOrDefault(p => p.IdPago == idPago);
                    pago.Cancelado = true;
                    db.Pagos.ApplyCurrentValues(pago);
                    
                    var facturas = db.FacturasPagos.Where(p => p.IdPago == idPago).ToList();
                    foreach (FacturasPagos fp in facturas)
                    {
                        var factura = db.facturas.Where(p => p.idVenta == fp.IdVenta).FirstOrDefault();
                        if (factura != null)
                        {
                            factura.Pagado = 0;
                            factura.StatusPago = false;
                            factura.FechaPago = null;
                            factura.StatusPago = false;
                            factura.Resta = factura.Resta - fp.Acuenta;
                            fp.IdPago = idPago;
                            db.FacturasPagos.ApplyCurrentValues(fp);
                            db.facturas.ApplyCurrentValues(factura);
                            //db.SaveChanges();
                        }

                    }
                    db.SaveChanges();
                    return true;
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



        public bool AplicarPago(int idPago, List<FacturasPagos> facturas, DateTime fechaPago)
        {
            try
            {
                using (var db = new NtLinkLocalServiceEntities())
                {
                    foreach (FacturasPagos fp in facturas)
                    {
                        var factura = db.facturas.Where(p => p.idVenta == fp.IdVenta).FirstOrDefault();
                        if (factura != null)
                        {
                            factura.Pagado = (factura.Pagado.HasValue ? factura.Pagado.Value : 0) + fp.Acuenta;
                            if (factura.Pagado >= factura.Importe)
                            {
                                factura.StatusPago = true;
                                factura.FechaPago = fechaPago;
                            }
                            else factura.StatusPago = false;

                            factura.Resta = factura.Importe - factura.Pagado;
                            fp.IdPago = idPago;
                            db.FacturasPagos.AddObject(fp);
                            db.facturas.ApplyCurrentValues(factura);                            
                            db.SaveChanges();
                        }

                    } 
                    return true;
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
    }
}
