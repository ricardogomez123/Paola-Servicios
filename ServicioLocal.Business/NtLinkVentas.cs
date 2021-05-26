using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServicioLocalContract;


namespace ServicioLocal.Business
{
    public class NtLinkVentas : NtLinkBusiness
    {
        public List<vventas> GetListCuentas(DateTime fechaInicial, DateTime fechaFinal, int idEmpresa, string status, int idCliente = 0, string linea = null)
        {
            try
            {
                List<vventas> lista;
                using (var db = new NtLinkLocalServiceEntities())
                {
                    if (idEmpresa == 0)
                    {
                        if (linea == null)
                        {
                            lista = db.vventas.Where(p => p.Fecha >= fechaInicial &&
                                                          p.Fecha <= fechaFinal).ToList();
                        }
                        else
                        {
                            if(idCliente==0)
                            {
                                lista = db.vventas.Where(p => p.Fecha >= fechaInicial &&
                                                              p.Fecha <= fechaFinal && p.Linea == linea).ToList();    
                            }
                            else
                            {
                                lista = db.vventas.Where(p => p.Fecha >= fechaInicial &&
                                                          p.Fecha <= fechaFinal && p.Linea == linea &&
                                                          p.idcliente == idCliente).ToList();
                            }
                            
                        }


                    }
                    else if (idCliente == 0)
                    {
                        if (linea == null)
                        {
                            lista = db.vventas.Where(p => p.IdEmpresa == idEmpresa && p.Fecha >= fechaInicial &&
                                                          p.Fecha <= fechaFinal).ToList();
                        }
                        else
                        {
                            lista = db.vventas.Where(p => p.IdEmpresa == idEmpresa && p.Fecha >= fechaInicial &&
                                                          p.Fecha <= fechaFinal && p.Linea == linea).ToList();
                        }
                    }
                    else
                    {
                        if (linea == null)
                        {
                            lista =
                           db.vventas.Where(
                               p => p.IdEmpresa == idEmpresa && p.idcliente == idCliente && p.Fecha >= fechaInicial &&
                                    p.Fecha <= fechaFinal).ToList();
                        }
                        else
                        {
                            lista =
                              db.vventas.Where(
                                  p => p.IdEmpresa == idEmpresa && p.idcliente == idCliente && p.Fecha >= fechaInicial &&
                                       p.Fecha <= fechaFinal && p.Linea == linea).ToList();
                        }


                    }
                    if (status == "Todos")
                    {
                        return lista;
                    }
                    return lista.Where(p => p.StatusFactura == status || p.StatusFactura == "Pagado").ToList();
                }
            }
            catch (Exception eee)
            {
                Logger.Error(eee.Message);
                return new List<vventas>();
            }

        }
        public List<vventas> GetList(DateTime fechaInicial, DateTime fechaFinal, int idEmpresa, string status, int idCliente = 0, string linea = null, string iniciales = null)
        {
            try
            {
                List<vventas> lista;
                using (var db = new NtLinkLocalServiceEntities())
                {
                    if (idEmpresa == 0)
                    {
                        if (!string.IsNullOrEmpty(iniciales))
                        {
                            lista = db.vventas.Where(p => p.Fecha >= fechaInicial &&
                                                          p.Fecha <= fechaFinal && p.Usuario == iniciales).OrderBy(p => p.Folio).ToList();
                        }
                        else if (linea == null)
                        {
                            lista = db.vventas.Where(p => p.Fecha >= fechaInicial &&
                                                          p.Fecha <= fechaFinal).OrderBy(p => p.Folio).ToList();
                        }
                        else
                        {
                            if (idCliente == 0)
                            {
                                lista = db.vventas.Where(p => p.Fecha >= fechaInicial &&
                                                              p.Fecha <= fechaFinal && p.Linea == linea).OrderBy(p => p.Folio).ToList();
                            }
                            else
                            {
                                // order by folio
                                lista = db.vventas.Where(p => p.Fecha >= fechaInicial && p.Fecha <= fechaFinal && p.Linea == linea &&
                                                          p.idcliente == idCliente).OrderBy(p => p.Folio).ToList();
                            }

                        }

                        //Lista de REportes
                    }
                    else if (idCliente == 0)
                    {
                        if (!string.IsNullOrEmpty(iniciales))
                        {
                            lista = db.vventas.Where(p => p.IdEmpresa == idEmpresa && p.Fecha >= fechaInicial &&
                                                          p.Fecha <= fechaFinal && p.Usuario == iniciales).OrderByDescending(p => p.Folio).ToList();
                        }
                        else if (linea == null)
                        {
                            lista = db.vventas.Where(p => p.IdEmpresa == idEmpresa && p.Fecha >= fechaInicial &&
                                                          p.Fecha <= fechaFinal).OrderByDescending(p => p.Folio).ToList();
                        }
                        else
                        {
                            lista = db.vventas.Where(p => p.IdEmpresa == idEmpresa && p.Fecha >= fechaInicial &&
                                                          p.Fecha <= fechaFinal && p.Linea == linea).OrderByDescending(p => p.Folio).ToList();
                        }
                    }
                    else
                    {
                        if (linea == null)
                        {
                            lista =
                           db.vventas.Where(
                               p => p.IdEmpresa == idEmpresa && p.idcliente == idCliente && p.Fecha >= fechaInicial &&
                                    p.Fecha <= fechaFinal).ToList();
                        }
                        else
                        {
                            lista =
                              db.vventas.Where(
                                  p => p.IdEmpresa == idEmpresa && p.idcliente == idCliente && p.Fecha >= fechaInicial &&
                                       p.Fecha <= fechaFinal && p.Linea == linea).ToList();
                        }


                    }
                    if (status == "Todos")
                    {
                        return lista;
                    }
                    return lista.Where(p => p.StatusFactura == status).ToList();
                }
            }
            catch (Exception eee)
            {
                Logger.Error(eee.Message);
                return new List<vventas>();
            }
           
        }

        public List<vventas> GetListNomina(DateTime fechaInicial, DateTime fechaFinal, int idEmpresa, string status, int idCliente = 0, string linea = null, string iniciales = null)
        {
            try
            {
                List<vventas> lista;
                using (var db = new NtLinkLocalServiceEntities())
                {
                    if (idEmpresa == 0)
                    {
                        if (!string.IsNullOrEmpty(iniciales))
                        {
                            lista = db.vventas.Where(p => p.Tipo == 1 &&  p.Fecha >= fechaInicial &&
                                                          p.Fecha <= fechaFinal && p.Usuario == iniciales).OrderBy(p => p.Folio).ToList();
                        }
                        else if (linea == null)
                        {
                            lista = db.vventas.Where(p => p.Tipo == 1 && p.Fecha >= fechaInicial &&
                                                          p.Fecha <= fechaFinal).OrderBy(p => p.Folio).ToList();
                        }
                        else
                        {
                            if (idCliente == 0)
                            {
                                lista = db.vventas.Where(p => p.Tipo == 1 && p.Fecha >= fechaInicial &&
                                                              p.Fecha <= fechaFinal && p.Linea == linea).OrderBy(p => p.Folio).ToList();
                            }
                            else
                            {
                                // order by folio
                                lista = db.vventas.Where(p => p.Tipo == 1 && p.Fecha >= fechaInicial && p.Fecha <= fechaFinal && p.Linea == linea &&
                                                          p.idcliente == idCliente).OrderBy(p => p.Folio).ToList();
                            }

                        }

                        //Lista de REportes
                    }
                    else if (idCliente == 0)
                    {
                        if (!string.IsNullOrEmpty(iniciales))
                        {
                            lista = db.vventas.Where(p => p.Tipo == 1 && p.IdEmpresa == idEmpresa && p.Fecha >= fechaInicial &&
                                                          p.Fecha <= fechaFinal && p.Usuario == iniciales).OrderByDescending(p => p.Folio).ToList();
                        }
                        else if (linea == null)
                        {
                            lista = db.vventas.Where(p => p.Tipo == 1 && p.IdEmpresa == idEmpresa && p.Fecha >= fechaInicial &&
                                                          p.Fecha <= fechaFinal).OrderByDescending(p => p.Folio).ToList();
                        }
                        else
                        {
                            lista = db.vventas.Where(p => p.Tipo == 1 && p.IdEmpresa == idEmpresa && p.Fecha >= fechaInicial &&
                                                          p.Fecha <= fechaFinal && p.Linea == linea).OrderByDescending(p => p.Folio).ToList();
                        }
                    }
                    else
                    {
                        if (linea == null)
                        {
                            lista =
                           db.vventas.Where(
                               p => p.Tipo == 1 && p.IdEmpresa == idEmpresa && p.idcliente == idCliente && p.Fecha >= fechaInicial &&
                                    p.Fecha <= fechaFinal).ToList();
                        }
                        else
                        {
                            lista =
                              db.vventas.Where(
                                  p => p.Tipo == 1 && p.IdEmpresa == idEmpresa && p.idcliente == idCliente && p.Fecha >= fechaInicial &&
                                       p.Fecha <= fechaFinal && p.Linea == linea).ToList();
                        }


                    }
                    if (status == "Todos")
                    {
                        return lista;
                    }
                    return lista.Where(p => p.StatusFactura == status).ToList();
                }
            }
            catch (Exception eee)
            {
                Logger.Error(eee.Message);
                return new List<vventas>();
            }

        }


        



        public vventas GetById(int idVenta)
        {
            try
            {
                using (var db = new NtLinkLocalServiceEntities())
                {
                    var x = db.vventas.FirstOrDefault(p => p.idVenta == idVenta);
                    return x;
                }
            }
            catch (Exception eee)
            {
                Logger.Error(eee.Message);
                return null;
            }
        }

        public bool SaveList (List<vventas> lista)
        {
            try
            {
                using (var db = new NtLinkLocalServiceEntities())
                {
                    foreach (vventas ventas in lista)
                    {
                        vventas ventas1 = ventas;
                        facturas f = db.facturas.Where(p => p.idVenta == ventas1.idVenta).First();
                        f.Cancelado = ventas.Cancelado;
                        f.FechaPago = ventas.FechaPago;
                        f.ReferenciaPago = ventas.ReferenciaPago;
                        f.Vencimiento = ventas.Vencimiento;
                        f.Proyecto = ventas.Proyecto;
                        //_entities.facturas.ApplyCurrentValues(f);
                    }
                    db.SaveChanges();
                }
                return true;
            }
            catch (Exception ee)
            {
                Logger.Error(ee.Message);
                return false;
            }
            
        }

        


    }
}
