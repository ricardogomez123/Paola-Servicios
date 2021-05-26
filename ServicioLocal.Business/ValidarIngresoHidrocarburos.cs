using log4net.Config;
using ServicioLocal.Business.Hidrocarburos;
using System;

namespace ServicioLocal.Business
{
    public class ValidarIngresoHidrocarburos : NtLinkBusiness
    {
        public ValidarIngresoHidrocarburos()
        {
            XmlConfigurator.Configure();
        }

        public string ProcesarIngresoHidrocarburos(IngresosHidrocarburos ih, string version, string TipoComprobante, decimal total)
        {
            string result;
            try
            {
                if (version != "3.3")
                {
                    result = "EEH101 - El atributo Version no tiene un valor válido.";
                }
                else if (TipoComprobante != "I")
                {
                    result = "EEH102 - El atributo TipoDeComprobante no cumple con el valor permitido.";
                }
                else
                {
                    if (ih.DocumentoRelacionado != null)
                    {
                        foreach (IngresosHidrocarburosDocumentoRelacionado en in ih.DocumentoRelacionado)
                        {
                            int Mes = en.FechaFolioFiscalVinculado.Month;
                            int Mes2 = Mes - 1;
                            if (Mes2 == 0)
                            {
                                Mes2 = 12;
                            }
                            string strinMes = en.Mes.ToString().Replace("Item", "");
                            int M = (int)Convert.ToInt16(strinMes);
                            if (M != Mes && M != Mes2)
                            {
                                result = "EEH103 - El valor del atributo Mes no corresponde al mes registrado en el atributo FechaFolioFiscalVinculado, o al de un mes anterior de calendario.";
                                return result;
                            }
                        }
                    }
                    if (ih.Porcentaje <= 0m)
                    {
                        result = "EEH104 - El valor del atributo Porcentaje no es mayor a 0.";
                    }
                    else if (ih.ContraprestacionPagadaOperador != total)
                    {
                        result = "EEH105 - El valor del atributo ContraprestacionPagadaOperador es diferente al valor registrado en el Total del comprobante.";
                    }
                    else
                    {
                        result = "0";
                    }
                }
            }
            catch (Exception ex)
            {
                result = "IEEH999 - Error no clasificado " + ex.Message;
            }
            return result;
        }
    }
}
