using System;
using System.Linq;

namespace I_RFC_SAT
{
    public class Operaciones_IRFC
    {
        public vI_RFC Consultar_IRFC(string rfc)
        {
            vI_RFC result;
            try
            {
                using (DescargasSatEntities db = new DescargasSatEntities())
                {
                    vI_RFC tabla = db.vI_RFC.FirstOrDefault((vI_RFC p) => p.RFC == rfc);
                    result = tabla;
                }
            }
            catch (Exception ex_A4)
            {
                result = null;
            }
            return result;
        }

        public vLCO RFCValidezObligaciones(string rfc)
        {
            vLCO result;
            try
            {
                using (DescargasSatEntities db = new DescargasSatEntities())
                {
                    vLCO lco = db.vLCO.FirstOrDefault((vLCO p) => p.Rfc == rfc && (p.ValidezObligaciones == "2" || p.ValidezObligaciones == "3"|| p.ValidezObligaciones == "4" ));
                    result = lco;
                }
            }
            catch (Exception ee_E8)
            {
                result = null;
            }
            return result;
        }

        public vLCO RFCValidezObligacionesAll(string rfc)
        {
            vLCO result;
            try
            {
                using (DescargasSatEntities db = new DescargasSatEntities())
                {
                    vLCO lco = db.vLCO.FirstOrDefault((vLCO p) => p.Rfc == rfc );
                    result = lco;
                }
            }
            catch (Exception ee_E02)
            {
                result = null;
            }
            return result;
        }

        public vLCO SearchLCOByNoCertificado(string noCertificado)
        {
            vLCO result;
            try
            {
                using (DescargasSatEntities db = new DescargasSatEntities())
                {
                    vLCO lco = db.vLCO.FirstOrDefault((vLCO p) => p.noCertificado == noCertificado);
                    result = lco;
                }
            }
            catch (Exception ee_A4)
            {
                result = null;
            }
            return result;
        }

        public vLCO SearchLCOByRFC(string rfc)
        {
            vLCO result;
            try
            {
                using (DescargasSatEntities db = new DescargasSatEntities())
                {
                    vLCO lco = db.vLCO.FirstOrDefault((vLCO p) => p.Rfc == rfc);
                    result = lco;
                }
            }
            catch (Exception ee_A4)
            {
                result = null;
            }
            return result;
        }

        public vLCO CSDValidar(string cadena)
        {
            vLCO result;
            try
            {
                using (DescargasSatEntities db = new DescargasSatEntities())
                {
                    result = db.vLCO.FirstOrDefault((vLCO p) => p.noCertificado == cadena);
                }
            }
            catch (Exception ee_A1)
            {
                result = null;
            }
            return result;
        }

        public bool subcontratacion(string rfc)
        {
            vI_RFC t = this.Consultar_IRFC(rfc);
            return t != null && t.SUBCONTRATACION == "1";
        }

        public bool inscrito(string rfc)
        {
            vI_RFC t = this.Consultar_IRFC(rfc);
            return t != null;
        }

        public bool SNFC(string rfc)
        {
            vI_RFC t = this.Consultar_IRFC(rfc);
            return t != null && t.SNCF == "1";
        }
    }
}
