using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Text.RegularExpressions;
using ServicioLocalContract;


namespace ServicioLocal.Business
{
    public class NtLinkClientes : NtLinkBusiness
    {

        
        public bool BorrarClientesPromotores(int idCP)
        {
            try
            {
                using (var db = new NtLinkLocalServiceEntities())
                {
                    ClientesPromotores cp = db.ClientesPromotores.FirstOrDefault(p => p.IdCP == idCP);

                    db.ClientesPromotores.DeleteObject(cp);
                    db.SaveChanges();
                    return true;
                }
            }
            catch (Exception ee)
            {
                Logger.Error(ee.Message);
                if (ee.InnerException != null)
                    Logger.Error(ee.InnerException);
                return false;
            }
        }

        public bool GuardarClientesPromotores(int idCliente, int idPromotor)
        {
            try
            {
                using (var db = new NtLinkLocalServiceEntities())
                {
                    ClientesPromotores cp = new ClientesPromotores
                                                {

                                                    IdCliente = idCliente,
                                                    IdPromotor = idPromotor
                                                };
                    db.ClientesPromotores.AddObject(cp);
                    db.SaveChanges();
                    return true;
                }
            }
            catch (Exception ee)
            {
                Logger.Error(ee.Message);
                if (ee.InnerException != null)
                    Logger.Error(ee.InnerException);
                return false;
            }
        }


        public List<vClientesPromotores> ListaClientesPromotores(int idCliente)
        {
            try
            {
                using (var db = new NtLinkLocalServiceEntities())
                {
                    var clientesPromotores = db.vClientesPromotores.Where(p => p.idCliente == idCliente);
                    return  clientesPromotores.ToList();
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

        //Guardar Promotores
        public bool GuardarPromotor(Promotores promotor)
        {

            try
            {
                Regex inv = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
                if (!inv.IsMatch(promotor.Email))
                {
                    throw new FaultException("Email incorrecto");
                }
                
                using (var db = new NtLinkLocalServiceEntities())
                {

                    
                    if (promotor.IdPromotor == 0)
                    {
                        if (db.Promotores.Any(p => p.Email == promotor.Email))
                        {
                            throw new FaultException("Email Duplicado");
                        }
                        db.Promotores.AddObject(promotor);
                    }
                    else
                    {
                        var o = db.Promotores.Where(p => p.IdPromotor == promotor.IdPromotor).FirstOrDefault();
                        db.Promotores.ApplyCurrentValues(promotor);
                    }
                    db.SaveChanges();
                    return true;
                }
            }
            catch(FaultException fe)
            {
                throw;
            }
            catch (Exception ee)
            {
                Logger.Error(ee.Message);
                if (ee.InnerException != null)
                    Logger.Error(ee.InnerException);
                return false;
            }
        }
        //ListarPromotores
        public List<Promotores> ListaPromotores(int idSistema)
        {
            try
            {
                using (var db = new NtLinkLocalServiceEntities())
                {
                    var x = db.Promotores.Where(p => p.IdSistema == idSistema).ToList();
                    Logger.Debug(x.Count);
                    return x;
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
        //Obtener Promotores
        public Promotores ObtenerPromotores(int idPromotor)
        {
            try
            {
                using (var db = new NtLinkLocalServiceEntities())
                {
                    var ObtenerPromotor = db.Promotores.Where(p => p.IdSistema == idPromotor).FirstOrDefault();
                    return ObtenerPromotor;
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

        public clientes GetCliente(int idCliente)
        {

            try
            {
                using (var db = new NtLinkLocalServiceEntities())
                {
                    var cliente = db.clientes.Where(c => c.idCliente == idCliente);
                    return cliente.FirstOrDefault();
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

        public clientes  GetCliente(string rfc)
        {

            try
            {
                using (var db = new NtLinkLocalServiceEntities())
                {
                    var cliente = db.clientes.Where(c => c.RFC == rfc);
                    return cliente.FirstOrDefault();
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


        public List<clientes> GetList(string linea)
        {

            try
            {
                using (var db = new NtLinkLocalServiceEntities())
                {
                    string query = "select Value c from NtLinkLocalServiceEntities.clientes as c inner join NtLinkLocalServiceEntities.empresa as b " +
                        " ON (c.idempresa = b.IdEmpresa) where b.Linea = @linea";
                    ObjectParameter op = new ObjectParameter("linea", linea);
                    var q = db.CreateQuery<clientes>(query, new [] {op}).OrderBy(p=>p.RazonSocial);
                    var result = q.ToList();
                    return result;
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


        public List<clientes> GetList()
        {
            
            try
            {
                using (var db = new NtLinkLocalServiceEntities())
                {

                    var result = db.clientes.OrderBy(l=>l.RazonSocial).ToList();
                    return result;
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


        public List<clientes> GetList(string perfil, int idEmpresa, string filtro = "", bool lista = false)
        {
            List<clientes> result = new List<clientes>();
            try
            {
                using (var db = new NtLinkLocalServiceEntities())
                {
                    if (string.IsNullOrEmpty(filtro))
                    {
                        result =
                            db.clientes.Where(p => p.idempresa == idEmpresa).OrderByDescending(p => p.RazonSocial).
                                ToList();
                    }
                    else
                    {
                        result =
                            db.clientes.Where(
                                p =>
                                p.idempresa == idEmpresa && (p.RazonSocial.Contains(filtro) || p.RFC.Contains(filtro))).
                                OrderByDescending(p => p.RazonSocial).ToList();
                    }
                }
            }
            catch (Exception ee)
            {
                Logger.Error(ee.Message);
                if (ee.InnerException != null)
                    Logger.Error(ee.InnerException);
            }
            if (lista)
                result.Add(new clientes{RazonSocial = "Todos",idCliente = 0});
            result.Reverse();
            return result;
        }


        public bool EliminarCliente(clientes cliente)
        {
            try
            {

                Logger.Info(cliente);
                
                using (var db = new NtLinkLocalServiceEntities())
                {
                    var existe = db.facturas.Any(p => p.idcliente == cliente.idCliente);
                    if (existe)
                        throw new FaultException("El cliente tiene facturas capturadas, no es posible eliminar");
                    var cli = db.clientes.FirstOrDefault(c => c.idCliente == cliente.idCliente);
                    db.clientes.DeleteObject(cli);
                    db.SaveChanges();
                    return true;
                }
            }
            catch (FaultException fe)
            {
                throw;
            }
            catch (Exception ee)
            {
                Logger.Error(ee);
                if (ee.InnerException != null)
                    Logger.Error(ee.InnerException);
                return false;
            }
        }

        public int SaveCliente(clientes cliente)
        {

            try
            {
                if (Validar(cliente))
                {
                    using (var db = new NtLinkLocalServiceEntities())
                    {
                        if (cliente.idCliente == 0)
                        {
                            db.clientes.AddObject(cliente);
                        }
                        else
                        {
                            var y = db.clientes.Where(p => p.idCliente == cliente.idCliente).FirstOrDefault();
                            db.clientes.ApplyCurrentValues(cliente);
                        }
                        db.SaveChanges();
                        return cliente.idCliente;
                    }
                }
                return 0;
            }
            catch (FaultException fe)
            {
                throw;
            }
            catch (Exception ee)
            {
                Logger.Error(ee);
                if (ee.InnerException != null)
                    Logger.Error(ee.InnerException);
                return 0;
            }
        }

        public DatosNomina GetDatosByCliente(int idCliente)
        {
            try
            {

                using (var db = new NtLinkLocalServiceEntities())
                {
                    var datos = db.DatosNomina.FirstOrDefault(p => p.IdCliente == idCliente);
                    return datos;
                }
            }
            catch (FaultException fe)
            {
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

        public bool SaveDatosNomina(DatosNomina datos)
        {
            try
            {
                
                using (var db = new NtLinkLocalServiceEntities())
                {
                    if (datos.IdDatoNomina == 0)
                    {
                        db.DatosNomina.AddObject(datos);
                    }
                    else
                    {
                        var y = db.DatosNomina.FirstOrDefault(p => p.IdDatoNomina == datos.IdDatoNomina);
                        db.DatosNomina.ApplyCurrentValues(datos);
                    }
                    db.SaveChanges();
                    return true;
                }
            }
            catch (FaultException fe)
            {
                throw;
            }
            catch (Exception ee)
            {
                Logger.Error(ee);
                if (ee.InnerException != null)
                    Logger.Error(ee.InnerException);
                return false;
            }
        }


        private bool Validar(clientes e)
        {
            //TODO: Validar los campos requeridos y generar excepcion
            {
                if (string.IsNullOrEmpty(e.RazonSocial))
                {
                    throw new FaultException<ApplicationException>(new ApplicationException("La Razón Social no puede ir vacía"), "La Razón Social no puede ir vacía");
                }
                if (string.IsNullOrEmpty(e.RFC))
                {
                    throw new FaultException<ApplicationException>(new ApplicationException("El RFC no puede ir vacío"), "El RFC no puede ir vacío");
                }
                if (string.IsNullOrEmpty(e.Direccion))
                {
                    throw new FaultException<ApplicationException>(new ApplicationException("El campo Dirección no puede ir vacío"), "El campo Dirección no puede ir vacío");
                }
                if (string.IsNullOrEmpty(e.Colonia))
                {
                    throw new FaultException<ApplicationException>(new ApplicationException("El campo Colonia no puede ir vacío"), "El campo Colonia no puede ir vacío");
                }
                if (string.IsNullOrEmpty(e.Ciudad))
                {
                    throw new FaultException<ApplicationException>(new ApplicationException("El campo Municipio no puede ir vacío"), "El campo Municipio no puede ir vacío");
                }
                if (string.IsNullOrEmpty(e.Estado))
                {
                    throw new FaultException<ApplicationException>(new ApplicationException("El campo Estado no puede ir vacío"),"El campo Estado no puede ir vacío");
                }
                if (string.IsNullOrEmpty(e.Pais))
                {
                    throw new FaultException<ApplicationException>(new ApplicationException("El campo País no puede ir vacío"), "El campo País no puede ir vacío");
                }
                if (string.IsNullOrEmpty(e.CP))
                {
                    throw new FaultException<ApplicationException>(new ApplicationException("El campo CP no puede ir vacío"),"El campo CP no puede ir vacío");
                }
                Regex reg = new Regex("^[A-Z,Ñ,&amp;]{3,4}[0-9]{2}[0-1][0-9][0-3][0-9][A-Z,0-9]{2}[0-9,A]$");
                if (!reg.IsMatch(e.RFC))
                {
                    throw new FaultException<ApplicationException>(new ApplicationException("El RFC es inválido"),"El RFC es inválido");
                }
            }
            return true;
        }
        //
        public List<vClientesPromotores> ListaPromotoresClientes(int idCliente)
        {
            try
            {
                using (var db = new NtLinkLocalServiceEntities())
                {
                    var x = db.vClientesPromotores.Where(p => p.idCliente == idCliente).ToList();
                    Logger.Debug(x.Count);
                    return x;
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
