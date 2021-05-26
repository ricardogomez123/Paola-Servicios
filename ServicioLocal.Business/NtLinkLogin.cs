using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Security;

namespace ServicioLocal.Business
{
    public class NtLinkLogin : NtLinkBusiness
    {
        public static MembershipUser ValidateUser(string userName, string pass)
        {
            try
            {
                if (Membership.ValidateUser(userName, pass))
                    return Membership.GetUser(userName);
                else
                {
                    Logger.Info("Usuario Inválido, " + userName);
                    return null;
                }
            }
            catch (Exception ee)
            {
                Logger.Error(ee.Message);
                return null;
            }
            
        }

        


    }
}
