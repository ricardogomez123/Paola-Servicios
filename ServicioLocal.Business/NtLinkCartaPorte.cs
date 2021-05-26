using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServicioLocalContract;
namespace ServicioLocal.Business
{
    public class NtLinkCartaPorte : NtLinkBusiness
    {
        public bool SaveConceptoCartaPorte(ConceptosCartaPorte concepto)
        {
            try
            {
                using (var db = new NtLinkLocalServiceEntities())
                {
                    db.AddToConceptosCartaPorte(concepto);
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
    }
}
