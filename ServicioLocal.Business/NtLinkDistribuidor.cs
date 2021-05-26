using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Security;
using ServicioLocalContract;
using I_RFC_SAT;

namespace ServicioLocal.Business
{
    public class NtLinkDistribuidor : NtLinkBusiness
    {
        public bool SaveDistribuidor(Distribuidores distribuidor, ref string resultado, string nombreCompleto,
                                     string iniciales)
        {

            try
            {
                if (Validar(distribuidor))
                {
                    using (var db = new NtLinkLocalServiceEntities())
                    {
                        if (distribuidor.IdDistribuidor == 0)
                        {
                            // Crear random password
                            if (db.aspnet_Membership.Any(p => p.LoweredEmail == distribuidor.Email.ToLower()))
                            {
                                throw new FaultException("El Email ya fue registrado");
                            }
                            db.Distribuidores.AddObject(distribuidor);
                            db.SaveChanges();
                            var password = Membership.GeneratePassword(8, 2);
                            var userName = distribuidor.Email;
                            NtLinkUsuarios.CreateUserDis(userName, password, distribuidor.Email,
                                                         (int) distribuidor.IdDistribuidor, "Administrador",
                                                         nombreCompleto, iniciales);
                            Logger.Info("se creó el usuario: " + userName + " con el password: " + password);
                            try
                            {
                                EnviarCorreo(distribuidor.Email, distribuidor.Nombre, userName, password);
                            }
                            catch (Exception ex)
                            {
                                Logger.Error(ex);
                            }
                            resultado = "se creó el usuario: " + userName + " con el password: " + password;

                        }
                        else
                        {
                            var y =
                                db.Distribuidores.Where(p => p.IdDistribuidor == distribuidor.IdDistribuidor).
                                    FirstOrDefault();
                            db.Distribuidores.ApplyCurrentValues(distribuidor);
                            db.SaveChanges();
                            resultado = "Registro actualizado correctamente";
                        }


                        return true;
                    }
                }
                return false;
            }
            catch (FaultException fe)
            {
                throw;
            }
            catch (Exception ee)
            {
                resultado = "Error al guardar el registro";
                Logger.Error(ee);
                if (ee.InnerException != null)
                    Logger.Error(ee.InnerException);
                return false;
            }
        }

        private bool Validar(Distribuidores d)
        {
            //TODO: Validar los campos requeridos y generar excepcion
            {
                if (string.IsNullOrEmpty(d.Nombre))
                {
                    throw new FaultException("La Razón Social no puede ir vacía");
                }
                if (string.IsNullOrEmpty(d.Email))
                {
                    throw new FaultException("El campo Email es Obligatorio");
                }
                if (string.IsNullOrEmpty(d.Direccion))
                {
                    throw new FaultException("El campo Calle es Obligatorio");
                }
                if (string.IsNullOrEmpty(d.Ciudad))
                {
                    throw new FaultException("El campo Municipio es Obligatorio");
                }
                if (string.IsNullOrEmpty(d.Estado))
                {
                    throw new FaultException("El campo Estado es Obligatorio");
                }
                if (string.IsNullOrEmpty(d.Cp))
                {
                    throw new FaultException("El campo Código Postal es Obligatorio");
                }
                Regex reg = new Regex("^[A-Z,Ñ,&amp;]{3,4}[0-9]{2}[0-1][0-9][0-3][0-9][A-Z,0-9]{2}[0-9,A]$");
                if (!reg.IsMatch(d.Rfc))
                {
                    throw new FaultException<ApplicationException>(new ApplicationException("El RFC es inválido"),
                                                                   "El RFC es inválido");
                }
                Regex regex =
                    new Regex(
                        @"\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*([,;]\s*\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*)*");
                if (!regex.IsMatch(d.Email))
                {
                    throw new FaultException("El campo Email esta mal formado");
                }
                if (d.IdDistribuidor == 0 && Existe(d))
                {
                    throw new FaultException("El RFC ya fue capturado");
                }
               // LcoLogic lco = new LcoLogic();
                Operaciones_IRFC lco = new Operaciones_IRFC();
                var rfcLco = lco.SearchLCOByRFC(d.Rfc);
                if (rfcLco == null)
                {
                    throw new FaultException("El Rfc no se encuentra en la lista de contribuyentes con obligaciones");
                }
            }
            return true;
        }

        private void EnviarCorreo(string email, string nombre, string usuario, string password)
        {
            try
            {
                var archivo = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Resources", "TextoEmail.html");

                var content = File.ReadAllText(archivo, Encoding.UTF8);
                content = content.Replace("[Nombre]", nombre).Replace("[UserName]", usuario).Replace(
                    "[Password]", password);
                Mailer mailer = new Mailer();
                var recipients = new List<string>();
                recipients.Add(email);
                mailer.Send(recipients, new List<string>(), content,
                            "Notificacion: Registro de Solicitud de Usuario - Facturación Electrónica",
                            "facturacion@ntlink.com.mx", "Servicio de Facturación Electrónica Nt Link");
            }
            catch (Exception ee)
            {
                Logger.Error(ee);
                if (ee.InnerException != null)
                    Logger.Error(ee.InnerException);
                throw new FaultException(
                    "Ocurrió un error al enviar el correo electronico, revise el archivo de log para ver los detalles");
            }
        }

        public List<Distribuidores> ListaDistribuidores()
        {
            try
            {
                using (var db = new NtLinkLocalServiceEntities())
                {
                    var con = db.Distribuidores.OrderBy(p => p.Nombre).ToList();
                    return con;
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

        public List<ElementoDist> ListaDisContratos(int idDistribuidor)
        {
            try
            {
                using (var db = new NtLinkLocalServiceEntities())
                {

                    var emp = db.empresa;
                    var fac = db.facturas;
                    if (idDistribuidor == 0)
                    {
                        var dist = db.DistContratos.OrderBy(e => e.IdSistema).ToList();
                        return dist.Select(p =>
                                           new ElementoDist()
                                               {
                                                   Id = Convert.ToInt32(p.IdContrato),
                                                   Fecha = p.FechaContrato.ToString().Substring(0, 10),
                                                   RazonSocial = emp.First(a => a.idSistema == p.IdSistema).RazonSocial,
                                                   Rfc = emp.First(a => a.idSistema == p.IdSistema).RFC,
                                                   Contratados = p.TipoContrato,
                                                   Total = Convert.ToDecimal(p.Costo),
                                                   Porcentaje = Convert.ToInt32(p.Pocentaje),
                                                   Comision = Convert.ToDecimal(p.Comision),
                                                   Status = p.Status,
                                                   Emitidos =
                                                       db.TimbreWs.Count(
                                                           f =>
                                                           f.RfcEmisor.Equals(
                                                               emp.FirstOrDefault(a => a.idSistema == p.IdSistema).RFC,
                                                               StringComparison.
                                                                   InvariantCultureIgnoreCase)),
                                                   Distribuidor =
                                                       db.Distribuidores.First(w => w.IdDistribuidor == p.IdDistribuidor)
                                                       .Nombre,
                                                   Timbre = p.Timbres,
                                                   Observacones = p.Observaciones,
                                                   Cancelados = fac.Where(o => o.Cancelado == 1).Count(
                                                       f =>
                                                       f.IdEmpresa ==
                                                       emp.FirstOrDefault(a => a.idSistema == p.IdSistema).IdEmpresa)
                                               }).ToList();
                    }
                    else
                    {
                        var dist =
                            db.DistContratos.Where(p => p.IdDistribuidor == idDistribuidor).OrderBy(e => e.IdSistema).
                                ToList();
                        return dist.Select(p =>
                                           new ElementoDist()
                                               {
                                                   Id = Convert.ToInt32(p.IdContrato),
                                                   Fecha = p.FechaContrato.ToString().Substring(0, 10),
                                                   RazonSocial = emp.First(a => a.idSistema == p.IdSistema).RazonSocial,
                                                   Rfc = emp.First(a => a.idSistema == p.IdSistema).RFC,
                                                   Contratados = p.TipoContrato,
                                                   Total = Convert.ToDecimal(p.Costo),
                                                   Porcentaje = Convert.ToInt32(p.Pocentaje),
                                                   Comision = Convert.ToDecimal(p.Comision),
                                                   Status = p.Status,
                                                   Emitidos =
                                                       db.TimbreWs.Count(
                                                           f =>
                                                           f.RfcEmisor.Equals(
                                                               emp.FirstOrDefault(a => a.idSistema == p.IdSistema).RFC,
                                                               StringComparison.
                                                                   InvariantCultureIgnoreCase)),
                                                   Distribuidor =
                                                       db.Distribuidores.First(w => w.IdDistribuidor == p.IdDistribuidor)
                                                       .Nombre,
                                                   Timbre = p.Timbres,
                                                   Observacones = p.Observaciones,
                                                   Cancelados = fac.Where(o => o.Cancelado == 1).Count(
                                                       f =>
                                                       f.IdEmpresa ==
                                                       emp.FirstOrDefault(a => a.idSistema == p.IdSistema).IdEmpresa)
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

        public void UpdateDistribuidor(int idContrato)
        {
            try
            {
                using (var db = new NtLinkLocalServiceEntities())
                {
                    DistContratos contratos = new DistContratos();
                    var y = db.DistContratos.Where(p => p.IdContrato == idContrato).FirstOrDefault();
                    y.Status = "Pagado";
                    db.DistContratos.ApplyCurrentValues(y);
                    db.SaveChanges();

                }
            }
            catch (Exception ee)
            {
                Logger.Error(ee.Message);
                if (ee.InnerException != null)
                    Logger.Error(ee.InnerException);
            }

        }

        public DistContratos Contratos(int idContrato)
        {
            try
            {
                using (var db = new NtLinkLocalServiceEntities())
                {
                    var contra = db.DistContratos.FirstOrDefault(p => p.IdContrato == idContrato);
                    return contra;
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

        public bool Save(Distribuidores e)
        {
            try
            {
                using (var db = new NtLinkLocalServiceEntities())
                {
                    if (Validar(e))
                    {
                        if (e.IdDistribuidor == 0)
                        {
                            if (db.Distribuidores.Any(l => l.Rfc.Equals(e.Rfc) && l.IdDistribuidor == e.IdDistribuidor))
                            {
                                throw new FaultException("El RFC ya ha sido dato de alta");
                            }
                            db.Distribuidores.AddObject(e);
                        }
                        else
                        {
                            db.Distribuidores.FirstOrDefault(p => p.IdDistribuidor == e.IdDistribuidor);
                            db.Distribuidores.ApplyCurrentValues(e);
                        }
                        db.SaveChanges();
                        return true;
                    }
                    return false;
                }
            }
            catch (ApplicationException ae)
            {
                throw new FaultException(ae.Message);
            }
            catch (FaultException fe)
            {
                throw;
            }
            catch (Exception ee)
            {
                Logger.Error(ee.Message);
                if (ee.InnerException != null)
                    Logger.Error(ee.InnerException.Message);
                return false;
            }

        }

        private bool Existe(Distribuidores e)
        {
            try
            {
                using (var db = new NtLinkLocalServiceEntities())
                {
                    return db.Distribuidores.Any(p => p.Rfc == e.Rfc);
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

        // llamando gridview de comisison distribuidores 
        public List<Comision_Distribuidores> listaComisiones()
        {

            try
            {
                using (var db = new NtLinkLocalServiceEntities())
                {
                    var comision = db.Comision_Distribuidores.ToList();
                    return comision;
                }
            }
            catch (Exception ee)
            {
                // sipre vair este codigo
                Logger.Error(ee);
                if (ee.InnerException != null)
                    Logger.Error(ee.InnerException);
                return null;
            }

        }
        // comision distribuidores timbredo
        public List<Ctdistribuidores> lisDistribuidores()
        {

            try
            {
                using (var bd = new NtLinkLocalServiceEntities())
                {
                    var Distimbrado = bd.Ctdistribuidores.ToList();
                    return Distimbrado;
                }
            }
            catch (Exception ee)
            {
                // sipre vair este codigo
                Logger.Error(ee);
                if (ee.InnerException != null)
                    Logger.Error(ee.InnerException);
                return null;
            }

        }




    }
}
