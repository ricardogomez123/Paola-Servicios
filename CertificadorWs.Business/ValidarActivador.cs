using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Xml.Linq;
using Org.BouncyCastle.X509;
using ServicioLocal.Business;
using ServicioLocalContract;
using log4net;
using log4net.Config;
using I_RFC_SAT;

namespace CertificadorWs.Business
{
    public class ValidadarActivador
    {
        private static ILog Logger = LogManager.GetLogger(typeof(ValidadarActivador));
        public ValidadarActivador()
        {
            XmlConfigurator.Configure();
        }

        public int Activar(ActivacionConvertidor A)
        {

            try
            {
                    using (var db = new NtLinkLocalServiceEntities())
                    {
                        if (A.Id == 0)
                        {
                            db.ActivacionConvertidor.AddObject(A);
                        }
                        else
                        {
                            db.ActivacionConvertidor.Where(p => p.Id == A.Id).FirstOrDefault();
                            db.ActivacionConvertidor.ApplyCurrentValues(A);
                        }
                        db.SaveChanges();
                        return 1;
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

        public  ActivacionConvertidor GetActivador(string key)
        {
            try
            {
                //rgv
                using (var db = new NtLinkLocalServiceEntities())
                {
                    ActivacionConvertidor ac = db.ActivacionConvertidor.Where(p => p.key == key && p.Activo==true).FirstOrDefault();
                    return ac;
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
        public ActivacionConvertidor GetActivo(string key,string Mac)
        {
            try
            {
                //rgv
                using (var db = new NtLinkLocalServiceEntities())
                {
                    ActivacionConvertidor ac = db.ActivacionConvertidor.Where(p => p.key == key && p.Mac==Mac && p.Activo == true).FirstOrDefault();
                    return ac;
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
        

    }
}