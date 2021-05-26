using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using log4net.Config;


namespace ServicioLocal.Business
{
    public class NtLinkBusiness
    {
        protected static ILog Logger = LogManager.GetLogger(typeof(NtLinkBusiness));
        protected NtLinkBusiness()
        {
            XmlConfigurator.Configure();
        }

    }
}
