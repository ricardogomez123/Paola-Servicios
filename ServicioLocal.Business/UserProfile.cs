using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Profile;
using System.Web.Security;

namespace ServicioLocal.Business
{
    public class UserProfile : ProfileBase
    {
        public static UserProfile GetUserProfile(string username)
        {
            return Create(username) as UserProfile;
        }

        public static UserProfile GetUserProfile() {
            return Create(Membership.GetUser().UserName) as UserProfile; 
        }
        [SettingsAllowAnonymous(false)]
        public string NombreCompleto {
            get { return base["NombreCompleto"] as string; }
            set { base["NombreCompleto"] = value; } 
        }
        [SettingsAllowAnonymous(false)]
        public string Iniciales {
            get { return base["Iniciales"] as string; }
            set { base["Iniciales"] = value; } 
        }

        [SettingsAllowAnonymous(false)]
        public string CambiarPassword
        {
            get { return base["CambiarPassword"] as string; }
            set { base["CambiarPassword"] = value; }
        }
      
    }
}
