using log4net.Config;
using System;
using System.Collections.Generic;
using Planesderetiro11;

namespace ServicioLocal.Business
{
    public class ValidarPR : NtLinkBusiness
    {
        public ValidarPR()
        {
            XmlConfigurator.Configure();
        }

        public string ProcesarPR(Planesderetiro pr)
        {
            string result;
            try
            {
                if (pr.MontTotRetiradoAnioInmAntPerSpecified && !pr.MontTotExentRetiradoAnioInmAntSpecified)
                {
                    result = "PDR101 - El atributo “MontTotExentRetiradoAnioInmAnt” debe de existir";
                }
                else if (pr.MontTotRetiradoAnioInmAntPerSpecified && !pr.MontTotExedenteAnioInmAntSpecified)
                {
                    result = "PDR102 - El atributo “MontTotExedenteAnioInmAnt” debe de existir";
                }
                else if (pr.MontTotRetiradoAnioInmAnt != pr.MontTotExentRetiradoAnioInmAnt + pr.MontTotExedenteAnioInmAnt)
                {
                    result = "PDR103 - El valor de este campo debe ser igual a la suma de “MontTotExentRetiradoAnioInmAnt” mas “MontTotExedenteAnioInmAnt”";
                }
                else
                {
                    if (pr.AportacionesODepositos != null && pr.AportacionesODepositos.Count > 0)
                    {
                        List<string> L = new List<string>();
                        foreach (PlanesderetiroAportacionesODepositos a in pr.AportacionesODepositos)
                        {
                            if (L.Count == 0)
                            {
                                L.Add(a.TipoAportacionODeposito.ToString());
                            }
                            else
                            {
                                foreach (string i in L)
                                {
                                    if (i == a.TipoAportacionODeposito.ToString())
                                    {
                                        result = "PDR104 - El valor de cada uno de los campos “TipoAportacionODeposito” debe ser diferente entre si.";
                                        return result;
                                    }
                                }
                                L.Add(a.TipoAportacionODeposito.ToString());
                            }
                        }
                    }
                    result = "0";
                }
            }
            catch (Exception ex)
            {
                result = "PDR999 - Error no clasificado";
            }
            return result;
        }
    }
}
