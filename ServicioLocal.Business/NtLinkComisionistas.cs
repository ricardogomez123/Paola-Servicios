using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServicioLocalContract;
using System.ServiceModel;

namespace ServicioLocal.Business
{
    public class NtLinkComisionistas : NtLinkBusiness
    {
        public Comisionistas GetComisionista(long idComisionista)
        {

            try
            {
                using (var db = new NtLinkLocalServiceEntities())
                {
                    var com = db.Comisionistas.Where(c => c.IdComisionista == idComisionista);
                    return com.FirstOrDefault();
                }
            }
            catch (Exception ee)
            {
                Logger.Error(ee.Message);
                return null;
            }

        }


        private bool Validar(Comisionistas e)
        {
            //TODO: Validar los campos requeridos y generar excepcion
            {
                if (string.IsNullOrEmpty(e.Nombre))
                {
                    throw new FaultException("El nombre es obligatorio");
                }
                if (string.IsNullOrEmpty(e.Email))
                {
                    throw new FaultException("El Email es obligatorio");
                }
                
            }
            return true;
        }


        public bool SaveComisionista(Comisionistas comisionista)
        {

            try
            {
                if (Validar(comisionista))
                {
                    using (var db = new NtLinkLocalServiceEntities())
                    {
                        if (comisionista.IdComisionista == 0)
                        {
                            db.Comisionistas.AddObject(comisionista);
                        }
                        else
                        {
                            var y = db.Comisionistas.Where(p => p.IdComisionista == comisionista.IdComisionista).FirstOrDefault();
                            db.Comisionistas.ApplyCurrentValues(comisionista);
                        }
                        db.SaveChanges();
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
                Logger.Error(ee.Message);
                return false;
            }
        }

        public List<Comisionistas> GetComisionistasLista(int idEmpresa)
        {

            try
            {
                using (var db = new NtLinkLocalServiceEntities())
                {
                    var com = db.Comisionistas.Where(c => c.IdEmpresa == idEmpresa);
                    return com.ToList();
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
