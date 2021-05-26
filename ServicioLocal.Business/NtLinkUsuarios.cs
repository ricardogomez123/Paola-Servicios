using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Web.Security;
using System.Web;
using ServicioLocalContract;


namespace ServicioLocal.Business
{
    public class NtLinkUsuarios : NtLinkBusiness
    {

        public bool RecuperarMail(string rfc, string email)
        {
            try
            {
                var usuario = Membership.GetUser(email);
                if (usuario == null)
                    throw new FaultException("El email proporcionado no coincide con el RFC");
                var empresa = GetEmpresaByUserId(usuario.ProviderUserKey.ToString());
                if (empresa == null || empresa.RFC != rfc)
                    throw new FaultException("El email proporcionado no coincide con el RFC");

                var profile = UserProfile.GetUserProfile(email);
                profile.CambiarPassword = "1";
                profile.Save();
                Mailer m = new Mailer();
                List<string> recipients = new List<string>(){email};
                var pass = usuario.ResetPassword("dos");
                m.Send(recipients,new List<string>(), 
                    "El nuevo password es: " + pass + " Recuerda cambiarlo al acceder al sistema",
                    "Cambio de contraseña NtLink Facturación",
                    "soporte@ntlink.com.mx",
                    "Soporte Nt Link" );
                return true;
            }
            catch (Exception ee)
            {
                Logger.Error(ee);
                throw new FaultException("No se encontró el usuario");
            }
           

        }

        public usuarios LoginAdmin(string usuario, string pass)
        {
            try
            {
                var passw = Utils.Sha1Hash(pass);
                using (var db = new NtLinkLocalServiceEntities())
                {
                    var x = db.usuarios.FirstOrDefault(p => p.Nombre == usuario && p.pass == passw);
                    return x;
                }
            }
            catch (Exception ee)
            {
                Logger.Error(ee.Message);
                return null;
            }
        }


        public usuarios AdminById(int idUsuario)
        {
            try
            {
                using (var db = new NtLinkLocalServiceEntities())
                {
                    var x = db.usuarios.FirstOrDefault(p => p.idusuario == idUsuario);
                    return x;
                }
            }
            catch (Exception ee)
            {
                Logger.Error(ee.Message);
                return null;
            }
        }

        //public SidetecUsuarios(int userId)
        //{
        //    if (userId != 0)
        //    {
        //        this.Usuario = Membership.GetUser(userId);
        //        var e = _entities.usuarios_empresas.Where(p => p.IdUsuarios_Empresas == userId).FirstOrDefault();
        //        Empresa = e == null ? new empresa() : _entities.empresa.Where(p => p.IdEmpresa == e.IdEmpresa).FirstOrDefault();
        //    }
        //    else
        //    {

        //    }

        //}



        public void Save()
        {

        }


        public static bool CreateUser(string userName, string password, string eMail, int idEmpresa, string perfil, string nombreCompleto, string iniciales)
        {
            MembershipCreateStatus status = MembershipCreateStatus.ProviderError;
            try
            {
                
                using (var db = new NtLinkLocalServiceEntities())
                {

                    Membership.CreateUser(userName, password, eMail, "uno", "dos", true, out status);
                    Logger.Debug(status.ToString());
                    if (status == MembershipCreateStatus.Success)
                    {
                        UserProfile p = UserProfile.GetUserProfile(userName);
                        p.NombreCompleto = nombreCompleto;
                        p.Iniciales = iniciales;
                        p.Save();                     

                        MembershipUser mu = Membership.GetUser(userName);
                        if (mu != null && mu.ProviderUserKey != null)
                        {
                            usuarios_empresas ue = new usuarios_empresas
                                                       {IdEmpresa = idEmpresa, UserId = mu.ProviderUserKey.ToString()};
                            db.usuarios_empresas.AddObject(ue);
                        }
                        db.SaveChanges();
                        Roles.AddUserToRole(userName, perfil);
                        return true;
                    }
                }
            }
            catch (Exception ee)
            {
                Logger.Error(ee.Message);
            }
            if (status == MembershipCreateStatus.DuplicateEmail)
            {
                throw new FaultException("Email Duplicado");
            }
            if (status == MembershipCreateStatus.DuplicateUserName)
            {
                throw new FaultException("Usuario Duplicado");
            }
            if (status == MembershipCreateStatus.InvalidPassword)
            {
                throw new FaultException("El password no cumple con las politicas de seguridad");
            }
            return false;
        }





        public static bool UpdateUser(string userId, string nombreCompleto, string iniciales, string perfil)
        {
            try
            {

                MembershipUser usuario = Membership.GetUser(new Guid(userId));
               
                if (usuario != null && usuario.ProviderUserKey != null)
                {
                    AddUserToRoles(usuario.UserName, new[] { perfil });
                    UserProfile pr = UserProfile.GetUserProfile(usuario.UserName);
                    pr.NombreCompleto = nombreCompleto;
                    pr.Iniciales = iniciales;
                    pr.Save();
                    return true;
                }
                return false;
            }
            catch (Exception eee)
            {
                Logger.Error(eee.Message);
                return false;
            }
        }


        public static List<MembershipUser> GetUserList()
        {
            var users = Membership.GetAllUsers();
            
            return users.Cast<MembershipUser>().ToList();
        }


        public static List<MembershipUser> GetUserList(int idEmpresa)
        {
            
            try
            {
                var users = Membership.GetAllUsers();
                using (var db = new NtLinkLocalServiceEntities())
                {
                    var us_emp = db.usuarios_empresas.Where(p => p.IdEmpresa == idEmpresa).ToList();
                    var res = users.Cast<MembershipUser>().ToList();
                    var listaUsuarios = from re in res
                                        join u in us_emp on re.ProviderUserKey.ToString() equals u.UserId
                                        select re;
                    return listaUsuarios.ToList();
                }
            }
            catch (Exception eee)
            {
                Logger.Error(eee);
                return null;
            }
        }





        public static bool UpdateUserPassword(MembershipUser user, string newPass)
        {
            try
            {
                var pass = user.ResetPassword("dos");
                user.ChangePassword(pass, newPass);
                var profile = UserProfile.GetUserProfile(user.UserName);
                profile.CambiarPassword = "";
                profile.Save();
                return true;
            }
            catch (Exception ee)
            {
                Logger.Error(ee);
                return false;
            }

        }


        public static void CreateRole(string roleName)
        {
            Roles.CreateRole(roleName);
        }



        public static MembershipUser GetUser(string idUser)
        {
            return Membership.GetUser(new Guid(idUser.Replace("-", "")));
        }

        public static void DeleteUser(MembershipUser user)
        {
            Membership.DeleteUser(user.UserName);
        }

        public static string[] GetRoles()
        {
            return Roles.GetAllRoles();
        }


        public static string[] GetRolesForUser(string userName)
        {
            return Roles.GetRolesForUser(userName);
        }

        public static void AddUserToRoles(string userName, string[] roles)
        {
            string[] todosRoles = Roles.GetAllRoles();
            foreach (var rol in todosRoles)
            {
                if (Roles.IsUserInRole(userName, rol))
                    Roles.RemoveUserFromRole(userName, rol);
            }

            foreach (var s in roles)
            {
                if (!Roles.IsUserInRole(userName, s))
                {
                    Roles.AddUserToRole(userName, s);
                }
            }
        }

        public static empresa GetEmpresaByUserId(string userId = null)
        {
            try
            { 
                //rgv
                using (var db = new NtLinkLocalServiceEntities())
                {
                    usuarios_empresas ue = db.usuarios_empresas.Where(p => p.UserId == userId).FirstOrDefault();
                    if (ue != null)
                    {
                        empresa emp = db.empresa.Where(p => p.IdEmpresa == ue.IdEmpresa).FirstOrDefault();
                        return emp;
                    }
                    return null;
                }


            }
            catch (Exception eee)
            {
                Logger.Error(eee.Message);
                if (eee.InnerException != null)
                    Logger.Error(eee.InnerException);
                return null;
            }
        }
        public static bool GetEmpresaMultipleRFC(string rfc)
        {
            try
            {
                //rgv
                using (var db = new NtLinkLocalServiceEntities())
                {
                        var emp = db.empresa.Where(p => p.RFC == rfc).ToList();
                        if(emp==null)
                              return false;
                        if (emp.Count > 1)
                            return true;
                        else
                            return false;
                }


            }
            catch (Exception eee)
            {
                Logger.Error(eee.Message);
                if (eee.InnerException != null)
                    Logger.Error(eee.InnerException);
                return false;
            }
        }

        public static void DesbloquearUsuario(string userName)
        {
            try
            {
                var usuario = Membership.GetUser(userName);
                if (usuario != null) usuario.UnlockUser();
            }
            catch(Exception ex)
            {
                Logger.Error(ex.ToString());
            }
        }

        public static Distribuidores GetDisByUserId(string userId = null)
        {
            try
            {
                using (var db = new NtLinkLocalServiceEntities())
                {
                    usuarios_distribuidor ue = db.usuarios_distribuidor.Where(p => p.UserId == userId).FirstOrDefault();
                    if (ue != null)
                    {
                        Distribuidores dis = db.Distribuidores.Where(p => p.IdDistribuidor == ue.IdDistribuidor).FirstOrDefault();
                        return dis;
                    }
                    return null;
                }


            }
            catch (Exception eee)
            {
                Logger.Error(eee.Message);
                if (eee.InnerException != null)
                    Logger.Error(eee.InnerException);
                return null;
            }
        }

        public static bool CreateUserDis(string userName, string password, string eMail, int idDistribuidor, string perfil, string nombreCompleto, string iniciales)
        {
            MembershipCreateStatus status = MembershipCreateStatus.ProviderError;
            try
            {

                using (var db = new NtLinkLocalServiceEntities())
                {

                    Membership.CreateUser(userName, password, eMail, "uno", "dos", true, out status);
                    Logger.Debug(status.ToString());
                    if (status == MembershipCreateStatus.Success)
                    {
                        UserProfile p = UserProfile.GetUserProfile(userName);
                        p.NombreCompleto = nombreCompleto;
                        p.Iniciales = iniciales;
                        p.Save();

                        MembershipUser mu = Membership.GetUser(userName);
                        if (mu != null && mu.ProviderUserKey != null)
                        {
                            usuarios_distribuidor ue = new usuarios_distribuidor { IdDistribuidor = idDistribuidor, UserId = mu.ProviderUserKey.ToString() };
                            db.usuarios_distribuidor.AddObject(ue);
                        }
                        db.SaveChanges();
                        Roles.AddUserToRole(userName, perfil);
                        return true;
                    }
                }
            }
            catch (Exception ee)
            {
                Logger.Error(ee.Message);
            }
            if (status == MembershipCreateStatus.DuplicateEmail)
            {
                throw new FaultException("Email Duplicado");
            }
            if (status == MembershipCreateStatus.DuplicateUserName)
            {
                throw new FaultException("Usuario Duplicado");
            }
            if (status == MembershipCreateStatus.InvalidPassword)
            {
                throw new FaultException("El password no cumple con las politicas de seguridad");
            }
            return false;
        }
    }
}
