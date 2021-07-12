using System;
using System.Collections.Generic;
using System.Linq;

namespace CatalogosSAT
{
    public class OperacionesCatalogos
    {
        public c_Estaciones Consultar_Estaciones(string Clavetransporte,string NumeroEstacion)
        {
          c_Estaciones result;
            try
            {
                using (CatalogosEntities1 db = new CatalogosEntities1())
                {
                    c_Estaciones C = db.c_Estaciones.FirstOrDefault((c_Estaciones p) => p.Clavetransporte == Clavetransporte && p.Claveidentificacion==NumeroEstacion);
                    result = C;
                }
            }
            catch (Exception ex)
            {
                result = null;
            }
            return result;
        }

        public Double Consultar_rangoMensualSemanal(string Periodicidad, decimal BaseRet, string Impuesto)
        {
            try
            {
                if (Periodicidad == "01")
                {
                    using (CatalogosEntities1 db = new CatalogosEntities1())
                    {
                        List<RangoSemanal> C = db.RangoSemanal.ToList();
                        
                        foreach (var c in C)
                        {
                            if (c.Valormaximo != 0)//para el dato de en adelante...
                            {
                                if (c.Valorminimo <= BaseRet && BaseRet <= c.Valormaximo)
                                {
                                    if (Impuesto == "01")
                                        return (double)c.ISR;
                                    if (Impuesto == "02")
                                        return (double)c.IVA;
                                }
                            }
                            else
                            {
                                if (c.Valorminimo <= BaseRet)
                                {
                                    if (Impuesto == "01")
                                        return (double)c.ISR;
                                    if (Impuesto == "02")
                                        return (double)c.IVA;
                                }
                            }
                        }

                    }

                }
                if (Periodicidad == "02")
                {
                    using (CatalogosEntities1 db = new CatalogosEntities1())
                    {
                        List<RangoMensual> C = db.RangoMensual.ToList();
                        foreach (var c in C)
                        {
                            if (c.Valormaximo != 0)//para el dato de en adelante...
                            {

                                if (c.Valorminimo <= BaseRet && BaseRet <= c.Valormaximo)
                                {
                                    if (Impuesto == "01")
                                        return (double)c.ISR;
                                    if (Impuesto == "02")
                                        return (double)c.IVA;
                                }
                            }
                            else{
                                if (c.Valorminimo <= BaseRet )
                                {
                                    if (Impuesto == "01")
                                        return (double)c.ISR;
                                    if (Impuesto == "02")
                                        return (double)c.IVA;
                                }
                            }

                        }

                    }

                }
                return 0;
            }
            catch (Exception ex)
            {
                return 0;
            }

        }

        public c_NumPedimentoAduana Consultar_Aduanas(int Aduanal)
        {
            c_NumPedimentoAduana result;
            try
            {
                using (CatalogosEntities1 db = new CatalogosEntities1())
                {
                    c_NumPedimentoAduana C = db.c_NumPedimentoAduana.FirstOrDefault((c_NumPedimentoAduana p) => p.c_Aduana == (int?)Aduanal);
                    result = C;
                }
            }
            catch (Exception ex)
            {
                result = null;
            }
            return result;
        }

        public c_PatenteAduanal Consultar_Patente(int PatenteAduanal)
        {
            c_PatenteAduanal result;
            try
            {
                using (CatalogosEntities1 db = new CatalogosEntities1())
                {
                    c_PatenteAduanal C = db.c_PatenteAduanal.FirstOrDefault((c_PatenteAduanal p) => p.C_PatenteAduanal1 == (int?)PatenteAduanal);
                    result = C;
                }
            }
            catch (Exception ex_A5)
            {
                result = null;
            }
            return result;
        }

        public c_FormaPago Consultar_FormaPago(string formaPago)
        {
            c_FormaPago result;
            try
            {
                using (CatalogosEntities1 db = new CatalogosEntities1())
                {
                    c_FormaPago C = db.c_FormaPago.FirstOrDefault((c_FormaPago p) => p.c_FormaPago1 == formaPago);
                    result = C;
                }
            }
            catch (Exception ex)
            {
                result = null;
            }
            return result;
        }
        public c_ClaveProdServCP Consultar_ClaveProdServCP(string clave)
        {
            c_ClaveProdServCP result;
            try
            {
                using (CatalogosEntities1 db = new CatalogosEntities1())
                {
                  var  C = db.c_ClaveProdServCP.FirstOrDefault( p=> p.ClaveProdructo == clave);
                    result = C;
                }
            }
            catch (Exception ex)
            {
                result = null;
            }
            return result;
        }
        public Divisas Consultar_TipoDivisa(string ClavePais)
        {
            Divisas result;
            try
            {
                using (CatalogosEntities1 db = new CatalogosEntities1())
                {
                    Divisas C = db.Divisas.FirstOrDefault((Divisas p) => p.Clave == ClavePais);
                    result = C;
                }
            }
            catch (Exception ex_A6)
            {
                result = null;
            }
            return result;
        }

        public List<c_TasaOCuota> Consultar_TasaCuotaTraslado(string Impu, string tipoFactor)
        {
            List<c_TasaOCuota> result;
            try
            {
                if (Impu == "001")
                {
                    Impu = "ISR";
                }
                if (Impu == "002")
                {
                    Impu = "IVA";
                }
                if (Impu == "003")
                {
                    Impu = "IEPS";
                }
                using (CatalogosEntities1 db = new CatalogosEntities1())
                {
                    List<c_TasaOCuota> C = (from p in db.c_TasaOCuota
                                            where p.Impuesto == Impu && p.Traslado == "Sí" && p.Factor == tipoFactor
                                            select p).ToList<c_TasaOCuota>();
                    result = C;
                }
            }
            catch (Exception ex_1A7)
            {
                result = null;
            }
            return result;
        }

        public List<c_TasaOCuota> Consultar_TasaCuotaRetencion(string Impu, string tipoFactor)
        {
            List<c_TasaOCuota> result;
            try
            {
                if (Impu == "001")
                {
                    Impu = "ISR";
                }
                if (Impu == "002")
                {
                    Impu = "IVA";
                }
                if (Impu == "003")
                {
                    Impu = "IEPS";
                }
                using (CatalogosEntities1 db = new CatalogosEntities1())
                {
                    List<c_TasaOCuota> C = (from p in db.c_TasaOCuota
                                            where p.Impuesto == Impu && p.Retencion == "Sí" && p.Factor == tipoFactor
                                            select p).ToList<c_TasaOCuota>();
                    result = C;
                }
            }
            catch (Exception ex_1A7)
            {
                result = null;
            }
            return result;
        }

        public List<c_TasaOCuota> Consultar_TasaCuota(string Impu, string tipoFactor, string tipoReteTras, ref bool rango)
        {
            List<c_TasaOCuota> result;
            try
            {
                if (Impu == "001")
                {
                    Impu = "ISR";
                }
                if (Impu == "002")
                {
                    Impu = "IVA";
                }
                if (Impu == "003")
                {
                    Impu = "IEPS";
                }
                List<c_TasaOCuota> C = null;
                using (CatalogosEntities1 db = new CatalogosEntities1())
                {
                    if (tipoReteTras == "Retenciones")
                    {
                        C = (from p in db.c_TasaOCuota
                             where p.Impuesto == Impu && p.Retencion == "Sí" && p.Factor == tipoFactor
                             orderby p.Maximo descending
                             select p).ToList<c_TasaOCuota>();
                    }
                    if (tipoReteTras == "Traslados")
                    {
                        C = (from p in db.c_TasaOCuota
                             where p.Impuesto == Impu && p.Traslado == "Sí" && p.Factor == tipoFactor
                             orderby p.Maximo descending
                             select p).ToList<c_TasaOCuota>();
                    }
                    if (C != null && C[0].RangoOFijo == "Rango")
                    {
                        rango = true;
                    }
                    result = C;
                }
            }
            catch (Exception ex)
            {
                result = null;
            }
            return result;
        }

        public Tareas Consultar_Tarea(string T)
        {
            Tareas result;
            try
            {
                using (CatalogosEntities1 db = new CatalogosEntities1())
                {
                    Tareas C = db.Tareas.FirstOrDefault((Tareas p) => p.c_Tarea == T);
                    result = C;
                }
            }
            catch (Exception ex_A6)
            {
                result = null;
            }
            return result;
        }

        public SubActividad Consultar_SubActividad(string T)
        {
            SubActividad result;
            try
            {
                string xx = ((int)Convert.ToInt16(T.ToString().Replace("Item", ""))).ToString();
                using (CatalogosEntities1 db = new CatalogosEntities1())
                {
                    SubActividad C = db.SubActividad.FirstOrDefault((SubActividad p) => p.c_SubActividad == xx);
                    result = C;
                }
            }
            catch (Exception ex_C7)
            {
                result = null;
            }
            return result;
        }

        public c_TipoDeComprobante Consultar_TipoDeComprobante(string TC)
        {
            c_TipoDeComprobante result;
            try
            {
                using (CatalogosEntities1 db = new CatalogosEntities1())
                {
                    c_TipoDeComprobante C = db.c_TipoDeComprobante.FirstOrDefault((c_TipoDeComprobante p) => p.c_TipoDeComprobante1 == TC);
                    result = C;
                }
            }
            catch (Exception ex_A6)
            {
                result = null;
            }
            return result;
        }

        public c_ClaveProdServ Consultar_ClaveProdServ(long Clave)
        {
            c_ClaveProdServ result;
            try
            {
                using (CatalogosEntities1 db = new CatalogosEntities1())
                {
                    c_ClaveProdServ C = db.c_ClaveProdServ.FirstOrDefault((c_ClaveProdServ p) => (long)p.c_ClaveProdServ1 == Clave);
                    result = C;
                }
            }
            catch (Exception ex_A5)
            {
                result = null;
            }
            return result;
        }

        public c_UsoCFDI Consultar_USOCFDI(string uso_CFDI)
        {
            c_UsoCFDI result;
            try
            {
                using (CatalogosEntities1 db = new CatalogosEntities1())
                {
                    c_UsoCFDI pais = db.c_UsoCFDI.FirstOrDefault((c_UsoCFDI p) => p.c_UsoCFDI1 == uso_CFDI);
                    result = pais;
                }
            }
            catch (Exception ex_A6)
            {
                result = null;
            }
            return result;
        }

        public c_NumPedimentoAduana Consultar_NumPedimentoAduana(int c_Aduana)
        {
            c_NumPedimentoAduana result;
            try
            {
                using (CatalogosEntities1 db = new CatalogosEntities1())
                {
                    c_NumPedimentoAduana N = db.c_NumPedimentoAduana.FirstOrDefault((c_NumPedimentoAduana p) => p.c_Aduana == (int?)c_Aduana);
                    result = N;
                }
            }
            catch (Exception ex_A5)
            {
                result = null;
            }
            return result;
        }

        public c_Pais Consultar_Pais(string pais1)
        {
            c_Pais result;
            try
            {
                if (!string.IsNullOrEmpty(pais1))
                {
                    pais1 = pais1.Replace("Item", "");
                }
                using (CatalogosEntities1 db = new CatalogosEntities1())
                {
                    c_Pais pais2 = db.c_Pais.FirstOrDefault((c_Pais p) => p.c_Pais1 == pais1);
                    result = pais2;
                }
            }
            catch (Exception ex_D2)
            {
                result = null;
            }
            return result;
        }

        public c_Estado Consultar_EstadosPais(string E, string pais)
        {
            c_Estado result;
            try
            {
                if (!string.IsNullOrEmpty(E))
                {
                    E = E.Replace("Item", "");
                }
                if (!string.IsNullOrEmpty(pais))
                {
                    pais = pais.Replace("Item", "");
                }
                using (CatalogosEntities1 db = new CatalogosEntities1())
                {
                    c_Estado Estado = db.c_Estado.FirstOrDefault((c_Estado p) => p.c_Estado1 == E && p.c_Pais == pais);
                    result = Estado;
                }
            }
            catch (Exception ex_14A)
            {
                result = null;
            }
            return result;
        }

        public c_Estado Consultar_EstadosPais(string pais)
        {
            c_Estado result;
            try
            {
                if (!string.IsNullOrEmpty(pais))
                {
                    pais = pais.Replace("Item", "");
                }
                using (CatalogosEntities1 db = new CatalogosEntities1())
                {
                    c_Estado Estado = db.c_Estado.FirstOrDefault((c_Estado p) => p.c_Pais == pais);
                    result = Estado;
                }
            }
            catch (Exception ex_D2)
            {
                result = null;
            }
            return result;
        }

        public c_Estado Consultar_Estados(string E)
        {
            c_Estado result;
            try
            {
                if (!string.IsNullOrEmpty(E))
                {
                    E = E.Replace("Item", "");
                }
                using (CatalogosEntities1 db = new CatalogosEntities1())
                {
                    c_Estado Estado = db.c_Estado.FirstOrDefault((c_Estado p) => p.c_Estado1 == E && p.c_Pais == "MEX");
                    result = Estado;
                }
            }
            catch (Exception ex_116)
            {
                result = null;
            }
            return result;
        }

        public c_Municipio Consultar_Municipio(string E, string M)
        {
            c_Municipio result;
            try
            {
                if (!string.IsNullOrEmpty(E))
                {
                    E = E.Replace("Item", "");
                }
                if (!string.IsNullOrEmpty(M))
                {
                    M = M.Replace("Item", "");
                }
                using (CatalogosEntities1 db = new CatalogosEntities1())
                {
                    c_Municipio Municipio;
                    if (E != "")
                    {
                        Municipio = db.c_Municipio.FirstOrDefault((c_Municipio p) => p.c_Estado == E && p.c_Municipio1 == M);
                    }
                    else
                    {
                        Municipio = db.c_Municipio.FirstOrDefault((c_Municipio p) => p.c_Municipio1 == M);
                    }
                    result = Municipio;
                }
            }
            catch (Exception ex_1DF)
            {
                result = null;
            }
            return result;
        }

        public c_Municipio Consultar_MunicipioMEX(string E, string M)
        {
            c_Municipio result;
            try
            {
                if (!string.IsNullOrEmpty(E))
                {
                    E = E.Replace("Item", "");
                }
                if (!string.IsNullOrEmpty(M))
                {
                    M = M.Replace("Item", "");
                }
                using (CatalogosEntities1 db = new CatalogosEntities1())
                {
                    c_Municipio results = (from p in db.c_Municipio
                                           join d in db.c_Estado on p.c_Estado equals d.c_Estado1
                                           where p.c_Estado == E && p.c_Municipio1 == M && d.c_Pais == "MEX"
                                           select p).First<c_Municipio>();
                    result = results;
                }
            }
            catch (Exception ex_364)
            {
                result = null;
            }
            return result;
        }

        public c_Localidad Consultar_LocalidadMEX(string E, string L)
        {
            c_Localidad result;
            try
            {
                if (!string.IsNullOrEmpty(E))
                {
                    E = E.Replace("Item", "");
                }
                if (!string.IsNullOrEmpty(L))
                {
                    L = L.Replace("Item", "");
                }
                using (CatalogosEntities1 db = new CatalogosEntities1())
                {
                    c_Localidad results = (from p in db.c_Localidad
                                           join d in db.c_Estado on p.c_Estado equals d.c_Estado1
                                           where p.c_Estado == E && p.c_Localidad1 == L && d.c_Pais == "MEX"
                                           select p).First<c_Localidad>();
                    result = results;
                }
            }
            catch (Exception ex_364)
            {
                result = null;
            }
            return result;
        }

        public c_Localidad Consultar_Localidad(string E, string L)
        {
            c_Localidad result;
            try
            {
                if (!string.IsNullOrEmpty(E))
                {
                    E = E.Replace("Item", "");
                }
                if (!string.IsNullOrEmpty(L))
                {
                    L = L.Replace("Item", "");
                }
                using (CatalogosEntities1 db = new CatalogosEntities1())
                {
                    c_Localidad Localidad;
                    if (E != "")
                    {
                        Localidad = db.c_Localidad.FirstOrDefault((c_Localidad p) => p.c_Estado == E && p.c_Localidad1 == L);
                    }
                    else
                    {
                        Localidad = db.c_Localidad.FirstOrDefault((c_Localidad p) => p.c_Localidad1 == L);
                    }
                    result = Localidad;
                }
            }
            catch (Exception ex_1DF)
            {
                result = null;
            }
            return result;
        }

        public c_Colonia Consultar_ColoniaMEX(string CP, string C)
        {
            c_Colonia result;
            try
            {
                if (!string.IsNullOrEmpty(CP))
                {
                    CP = CP.Replace("Item", "");
                }
                if (!string.IsNullOrEmpty(C))
                {
                    C = C.Replace("Item", "");
                }
                using (CatalogosEntities1 db = new CatalogosEntities1())
                {
                    c_Colonia results = (from c in db.c_Colonia
                                         join d in db.c_CP on c.c_CodigoPostal equals d.c_CP1
                                         join p in db.c_Estado on d.c_Estado equals p.c_Estado1
                                         where c.c_Colonia1 == C && c.c_CodigoPostal == CP && p.c_Pais == "MEX"
                                         select c).First<c_Colonia>();
                    result = results;
                }
            }
            catch (Exception ex_505)
            {
                result = null;
            }
            return result;
        }

        public c_Colonia Consultar_Colonia(string CP, string C)
        {
            c_Colonia result;
            try
            {
                if (!string.IsNullOrEmpty(CP))
                {
                    CP = CP.Replace("Item", "");
                }
                if (!string.IsNullOrEmpty(C))
                {
                    C = C.Replace("Item", "");
                }
                using (CatalogosEntities1 db = new CatalogosEntities1())
                {
                    c_Colonia Localidad;
                    if (CP != "")
                    {
                        Localidad = db.c_Colonia.FirstOrDefault((c_Colonia p) => p.c_CodigoPostal == CP && p.c_Colonia1 == C);
                    }
                    else
                    {
                        Localidad = db.c_Colonia.FirstOrDefault((c_Colonia p) => p.c_Colonia1 == C);
                    }
                    result = Localidad;
                }
            }
            catch (Exception ex_1DF)
            {
                result = null;
            }
            return result;
        }

        public c_CP Consultar_CP(string E, string M, string L, string cp)
        {
            c_CP result;
            try
            {
                if (!string.IsNullOrEmpty(cp))
                {
                    cp = cp.Replace("Item", "");
                }
                if (!string.IsNullOrEmpty(L))
                {
                    L = L.Replace("Item", "");
                }
                if (!string.IsNullOrEmpty(E))
                {
                    E = E.Replace("Item", "");
                }
                if (!string.IsNullOrEmpty(M))
                {
                    M = M.Replace("Item", "");
                }
                using (CatalogosEntities1 db = new CatalogosEntities1())
                {
                    if (!string.IsNullOrEmpty(E) && !string.IsNullOrEmpty(M) && !string.IsNullOrEmpty(L))
                    {
                        c_CP CP = db.c_CP.FirstOrDefault((c_CP p) => p.c_Estado == E && p.c_Municipio == M && p.c_Localidad == L && p.c_CP1 == cp);
                        result = CP;
                    }
                    else if (!string.IsNullOrEmpty(E) && !string.IsNullOrEmpty(M))
                    {
                        c_CP CP = db.c_CP.FirstOrDefault((c_CP p) => p.c_Estado == E && p.c_Municipio == M && p.c_CP1 == cp);
                        result = CP;
                    }
                    else if (!string.IsNullOrEmpty(E) && !string.IsNullOrEmpty(L))
                    {
                        c_CP CP = db.c_CP.FirstOrDefault((c_CP p) => p.c_Estado == E && p.c_Localidad == L && p.c_CP1 == cp);
                        result = CP;
                    }
                    else if (!string.IsNullOrEmpty(M) && !string.IsNullOrEmpty(L))
                    {
                        c_CP CP = db.c_CP.FirstOrDefault((c_CP p) => p.c_Municipio == M && p.c_Localidad == L && p.c_CP1 == cp);
                        result = CP;
                    }
                    else if (!string.IsNullOrEmpty(E))
                    {
                        c_CP CP = db.c_CP.FirstOrDefault((c_CP p) => p.c_Estado == E && p.c_CP1 == cp);
                        result = CP;
                    }
                    else if (!string.IsNullOrEmpty(M))
                    {
                        c_CP CP = db.c_CP.FirstOrDefault((c_CP p) => p.c_Municipio == M && p.c_CP1 == cp);
                        result = CP;
                    }
                    else if (!string.IsNullOrEmpty(L))
                    {
                        c_CP CP = db.c_CP.FirstOrDefault((c_CP p) => p.c_Localidad == L && p.c_CP1 == cp);
                        result = CP;
                    }
                    else
                    {
                        result = null;
                    }
                }
            }
            catch (Exception ex_887)
            {
                result = null;
            }
            return result;
        }

        public c_CP Consultar_CPMEX(string E, string M, string L, string cp)
        {
            c_CP result;
            try
            {
                if (!string.IsNullOrEmpty(cp))
                {
                    cp = cp.Replace("Item", "");
                }
                if (!string.IsNullOrEmpty(L))
                {
                    L = L.Replace("Item", "");
                }
                if (!string.IsNullOrEmpty(E))
                {
                    E = E.Replace("Item", "");
                }
                if (!string.IsNullOrEmpty(M))
                {
                    M = M.Replace("Item", "");
                }
                using (CatalogosEntities1 db = new CatalogosEntities1())
                {
                    c_CP results = (from c in db.c_CP
                                    join p in db.c_Estado on c.c_Estado equals p.c_Estado1
                                    where c.c_Estado == E && c.c_Municipio == M && c.c_Localidad == L && c.c_CP1 == cp && p.c_Pais == "MEX"
                                    select c).First<c_CP>();
                    result = results;
                }
            }
            catch (Exception ex_49E)
            {
                result = null;
            }
            return result;
        }

        public c_RegimenFiscal Consultar_RegimenFiscal(string clave)
        {
            c_RegimenFiscal result;
            try
            {
                using (CatalogosEntities1 db = new CatalogosEntities1())
                {
                    c_RegimenFiscal regimen = db.c_RegimenFiscal.FirstOrDefault((c_RegimenFiscal p) => p.c_RegimenFiscal1 == clave);
                    result = regimen;
                }
            }
            catch (Exception ex_A6)
            {
                result = null;
            }
            return result;
        }

        public c_Pais Consultar_PaisVerificacionLinea(string clave)
        {
            c_Pais result;
            try
            {
                using (CatalogosEntities1 db = new CatalogosEntities1())
                {
                    c_Pais pais = db.c_Pais.FirstOrDefault((c_Pais p) => p.c_Pais1 == clave);
                    result = pais;
                }
            }
            catch (Exception ex_A6)
            {
                result = null;
            }
            return result;
        }

        public c_CP Consultar_CP(string cp)
        {
            c_CP result;
            try
            {
                using (CatalogosEntities1 db = new CatalogosEntities1())
                {
                    c_CP CP = db.c_CP.FirstOrDefault((c_CP p) => p.c_CP1 == cp);
                    result = CP;
                }
            }
            catch (Exception ex_A6)
            {
                result = null;
            }
            return result;
        }

        public c_ClaveUnidad ConsultarClaveUnidad(string ClaveUnidad)
        {
            c_ClaveUnidad result;
            try
            {
                using (CatalogosEntities1 db = new CatalogosEntities1())
                {
                    c_ClaveUnidad C = (from p in db.c_ClaveUnidad
                                       where p.c_ClaveUnidad1 == ClaveUnidad
                                       select p).FirstOrDefault<c_ClaveUnidad>();
                    result = C;
                }
            }
            catch (Exception ex_AB)
            {
                result = null;
            }
            return result;
        }

        public c_FraccionArancelaria Consultar_FraccionArancelaria(string f)
        {
            c_FraccionArancelaria result;
            try
            {
                using (CatalogosEntities1 db = new CatalogosEntities1())
                {
                    c_FraccionArancelaria CP = db.c_FraccionArancelaria.FirstOrDefault((c_FraccionArancelaria p) => p.c_FraccionArancelaria1 == f);
                    result = CP;
                }
            }
            catch (Exception ex_A6)
            {
                result = null;
            }
            return result;
        }

        public UnidadMedida Consultar_UnidadMedida(string f)
        {
            UnidadMedida result;
            try
            {
                using (CatalogosEntities1 db = new CatalogosEntities1())
                {
                    UnidadMedida UM = db.UnidadMedida.FirstOrDefault((UnidadMedida p) => p.Descripción == f.ToUpper());
                    result = UM;
                }
            }
            catch (Exception ex_C0)
            {
                result = null;
            }
            return result;
        }

        public c_Moneda Consultar_Moneda(string moneda)
        {
            c_Moneda result;
            try
            {
                using (CatalogosEntities1 db = new CatalogosEntities1())
                {
                    c_Moneda CP = db.c_Moneda.FirstOrDefault((c_Moneda p) => p.c_Moneda1 == moneda);
                    result = CP;
                }
            }
            catch (Exception ex_A6)
            {
                result = null;
            }
            return result;
        }
    }
}
