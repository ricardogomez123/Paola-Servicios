using CatalogosSAT;
using log4net.Config;
using ServicioLocal.Business.Hidro;
using System;
using System.Linq;

namespace ServicioLocal.Business
{
    public class ValidarGastosHidrocarburos : NtLinkBusiness
    {
        public ValidarGastosHidrocarburos()
        {
            XmlConfigurator.Configure();
        }

        public string ProcesarGastosHidrocarburos(GastosHidrocarburos gh, string version, string TipoComprobante)
        {
            string result;
            try
            {
                if (version != "3.3")
                {
                    result = "GCEH101 - El atributo Version no tiene un valor válido.";
                }
                else if (TipoComprobante != "E")
                {
                    result = "GCEH102 - El atributo TipoDeComprobante no cumple con el valor permitido.";
                }
                else
                {
                    foreach (GastosHidrocarburosErogacion en in gh.Erogacion)
                    {
                        if (en.DocumentoRelacionado != null)
                        {
                            foreach (GastosHidrocarburosErogacionDocumentoRelacionado d in en.DocumentoRelacionado)
                            {
                                if (!string.IsNullOrEmpty(d.FolioFiscalVinculado))
                                {
                                    if (d.OrigenErogacion.ToString() != "Nacional")
                                    {
                                        result = "GCEH103 - El atributo FolioFiscalVinculado no se debe registrar cuando el atributo OrigenErogacion es Extranjero.";
                                        return result;
                                    }
                                    if (string.IsNullOrEmpty(d.RFCProveedor))
                                    {
                                        result = "GCEH105 - El atributo RFCProveedor debe registrarse por que existe el atributo FolioFiscalVinculado.";
                                        return result;
                                    }
                                    if (!d.MontoTotalIVASpecified)
                                    {
                                        result = "GCEH107 - El atributo MontoTotalIVA debe registrarse por que existe el atributo FolioFiscalVinculado.";
                                        return result;
                                    }
                                }
                                else
                                {
                                    if (!string.IsNullOrEmpty(d.RFCProveedor))
                                    {
                                        result = "GCEH104 - El atributo RFCProveedor no se debe registrar por que no existe el atributo FolioFiscalVinculado.";
                                        return result;
                                    }
                                    if (d.MontoTotalIVASpecified)
                                    {
                                        result = "GCEH106 - El atributo MontoTotalIVA no se debe registrar por que no existe el atributo FolioFiscalVinculado.";
                                        return result;
                                    }
                                }
                                if (!string.IsNullOrEmpty(d.NumeroPedimentoVinculado))
                                {
                                    if (d.OrigenErogacion.ToString() == "Nacional")
                                    {
                                        result = "GCEH108 - El atributo NumeroPedimentoVinculado no se debe registrar cuando el atributo OrigenErogacion es Nacional.";
                                        return result;
                                    }
                                    if (!d.ClavePedimentoVinculadoSpecified)
                                    {
                                        result = "GCEH110 - El atributo ClavePedimentoVinculado debe registrarse por que existe el atributo NumeroPedimentoVinculado.";
                                        return result;
                                    }
                                    if (!d.ClavePagoPedimentoVinculadoSpecified)
                                    {
                                        result = "GCEH112 - El atributo ClavePagoPedimento debe registrarse por que existe el atributo NumeroPedimentoVinculado.";
                                        return result;
                                    }
                                    if (!d.MontoIVAPedimentoSpecified)
                                    {
                                        result = "GCEH114 - El atributo MontoIVAPedimento debe registrarse por que existe el atributo NumeroPedimentoVinculado.";
                                        return result;
                                    }
                                }
                                else
                                {
                                    if (d.ClavePedimentoVinculadoSpecified)
                                    {
                                        result = "GCEH109 - El atributo ClavePedimentoVinculado no se debe registrar por que no existe el atributo NumeroPedimentoVinculado.";
                                        return result;
                                    }
                                    if (d.ClavePagoPedimentoVinculadoSpecified)
                                    {
                                        result = "GCEH111 - El atributo ClavePagoPedimento no se debe registrar por que no existe el atributo NumeroPedimentoVinculado.";
                                        return result;
                                    }
                                    if (d.MontoIVAPedimentoSpecified)
                                    {
                                        result = "GCEH113 - El atributo MontoIVAPedimento no se debe registrar por que no existe el atributo NumeroPedimentoVinculado.";
                                        return result;
                                    }
                                }
                                if (!string.IsNullOrEmpty(d.FolioFiscalVinculado) && !d.FechaFolioFiscalVinculadoSpecified)
                                {
                                    result = "GCEH132 - El atributo FechaFolioFiscalVinculado debe registrarse por que existe el atributo  FolioFiscalVinculado.";
                                    return result;
                                }
                                int Mes = d.FechaFolioFiscalVinculado.Month;
                                int Mes2 = Mes - 1;
                                if (Mes2 == 0)
                                {
                                    Mes2 = 12;
                                }
                                string strinMes = d.Mes.ToString().Replace("Item", "");
                                int M = (int)Convert.ToInt16(strinMes);
                                if (M != Mes && M != Mes2)
                                {
                                    result = "GCEH115 - El valor del atributo Mes no corresponde al mes registrado en el atributo FechaFolioFiscalVinculado, o al de un mes anterior de calendario respecto a dicho atributo.";
                                    return result;
                                }
                                if (d.NumeroPedimentoVinculado != null)
                                {
                                    string sa = this.validarNumeroPedimento(d.NumeroPedimentoVinculado, "GCEH133");
                                    if (sa != "OK")
                                    {
                                        result = sa;
                                        return result;
                                    }
                                }
                            }
                        }
                        if (en.Porcentaje <= 0m)
                        {
                            result = "GCEH116 - El valor del atributo Porcentaje no es mayor a 0.";
                            return result;
                        }
                        if (en.Actividades != null)
                        {
                            foreach (GastosHidrocarburosErogacionActividades a in en.Actividades)
                            {
                                if (a.SubActividades == null || a.SubActividades.Count == 0)
                                {
                                    if (a.ActividadRelacionadaSpecified)
                                    {
                                        result = "GCEH117 - El atributo ActividadRelacionada no se debe registrar por que no existen los atributos SubActividadRelacionada y TareaRelacionada.";
                                        return result;
                                    }
                                }
                                foreach (GastosHidrocarburosErogacionActividadesSubActividades s in a.SubActividades)
                                {
                                    if (s.SubActividadRelacionadaSpecified && s.Tareas.Count > 0 && !a.ActividadRelacionadaSpecified)
                                    {
                                        result = "GCEH118 - El atributo ActividadRelacionada debe registrarse por que existen los atributos SubActividadRelacionada y TareaRelacionada.";
                                        return result;
                                    }
                                    if (!a.ActividadRelacionadaSpecified && s.Tareas.Count == 0 && s.SubActividadRelacionadaSpecified)
                                    {
                                        result = "GCEH119 - El atributo SubActividadRelacionada no se debe registrar por que no existen los atributos ActividadRelacionada y TareaRelacionada.";
                                        return result;
                                    }
                                    if (a.ActividadRelacionadaSpecified && s.Tareas.Count > 0 && !s.SubActividadRelacionadaSpecified)
                                    {
                                        result = "GCEH120 - El atributo SubActividadRelacionada debe registrarse por que existen los atributos  ActividadRelacionada y TareaRelacionada.";
                                        return result;
                                    }
                                    if (!a.ActividadRelacionadaSpecified && !s.SubActividadRelacionadaSpecified && s.Tareas.Count != 0)
                                    {
                                        result = "GCEH121 - El atributo TareaRelacionada no se debe registrar por que no existen los atributos ActividadRelacionada y SubActividadRelacionada.";
                                        return result;
                                    }
                                    if (a.ActividadRelacionadaSpecified && s.SubActividadRelacionadaSpecified && s.Tareas.Count == 0)
                                    {
                                        result = "GCEH122 - El atributo TareaRelacionada no se debe registrar por que no existen los atributos ActividadRelacionada y SubActividadRelacionada.";
                                        return result;
                                    }
                                    Actividades myTipoActividad;
                                    Enum.TryParse<Actividades>("Item" + a.ActividadRelacionada.ToString(), out myTipoActividad);
                                    if (myTipoActividad.ToString() != "Item" + a.ActividadRelacionada.ToString())
                                    {
                                        result = "GCEH123 - El valor del atributo ActividadRelacionada no contiene una clave del catálogo catCEH:Actividad.";
                                        return result;
                                    }
                                    OperacionesCatalogos o322 = new OperacionesCatalogos();
                                    CatalogosSAT.SubActividad usoSubActividad = o322.Consultar_SubActividad(s.SubActividadRelacionada.ToString());
                                    if (Convert.ToInt16(usoSubActividad.c_Actividad) != Convert.ToInt16(a.ActividadRelacionada.ToString()))
                                    {
                                        result = "GCEH124 - El valor del atributo SubActividadRelacionada no contiene una clave del catálogo catCEH:SubActividad donde la columna c_Actividad sea igual a la clave registrada en el atributo ActividadRelacionada.";
                                        return result;
                                    }
                                    foreach (GastosHidrocarburosErogacionActividadesSubActividadesTareas t in s.Tareas)
                                    {
                                        if (t.TareaRelacionadaSpecified)
                                        {
                                            OperacionesCatalogos o323 = new OperacionesCatalogos();
                                            CatalogosSAT.Tareas usoTarea = o323.Consultar_Tarea(t.TareaRelacionada.ToString());
                                            if (Convert.ToInt16(usoTarea.c_SubActividad) != Convert.ToInt16(s.SubActividadRelacionada.ToString().Replace("Item", "")) || Convert.ToInt16(usoTarea.c_Actividad) != Convert.ToInt16(a.ActividadRelacionada.ToString()))
                                            {
                                                result = "GCEH125 - El valor del atributo TareaRelacionada no contiene una clave del catálogo catCEH:Tarea donde la columna c_Subactividad sea igual a la clave registrada en el atributo SubActividadRelacionada y la columna c_Actividad sea igual a la clave registrada en el atributo ActividadRelacionada.";
                                                return result;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        if (en.CentroCostos != null)
                        {
                            foreach (GastosHidrocarburosErogacionCentroCostos c in en.CentroCostos)
                            {
                                if (c.Yacimientos.Count == 0)
                                {
                                    if (!string.IsNullOrEmpty(c.Campo))
                                    {
                                        result = "GCEH126 - El atributo Campo no se debe registrar por que no existen los atributos Yacimiento y Pozo.";
                                        return result;
                                    }
                                }
                                foreach (GastosHidrocarburosErogacionCentroCostosYacimientos y in c.Yacimientos)
                                {
                                    if (y.Pozos.Count == 0 && string.IsNullOrEmpty(y.Yacimiento) && !string.IsNullOrEmpty(c.Campo))
                                    {
                                        result = "GCEH126 - El atributo Campo no se debe registrar por que no existen los atributos Yacimiento y Pozo.";
                                        return result;
                                    }
                                    if (y.Pozos.Count<GastosHidrocarburosErogacionCentroCostosYacimientosPozos>() > 0 && !string.IsNullOrEmpty(y.Yacimiento) && string.IsNullOrEmpty(c.Campo))
                                    {
                                        result = "GCEH127 - El atributo Campo debe registrarse por que existen los atributos  Yacimiento y Pozo.";
                                        return result;
                                    }
                                    if (y.Pozos.Count == 0 && string.IsNullOrEmpty(c.Campo) && !string.IsNullOrEmpty(y.Yacimiento))
                                    {
                                        result = "GCEH128 - El atributo Yacimiento no se debe registrar por que no existen los atributos Campo y Pozo.";
                                        return result;
                                    }
                                    if (y.Pozos.Count != 0 && !string.IsNullOrEmpty(c.Campo) && string.IsNullOrEmpty(y.Yacimiento))
                                    {
                                        result = "GCEH129 - El atributo Yacimiento debe registrarse por que existen los atributos  Campo y Pozo.";
                                        return result;
                                    }
                                    if (string.IsNullOrEmpty(y.Yacimiento) && string.IsNullOrEmpty(c.Campo) && y.Pozos.Count != 0)
                                    {
                                        result = "GCEH130 - El atributo Pozo no se debe registrar por que no existen los atributos Campo y Yacimiento.";
                                        return result;
                                    }
                                    if (!string.IsNullOrEmpty(y.Yacimiento) && !string.IsNullOrEmpty(c.Campo) && y.Pozos.Count == 0)
                                    {
                                        result = "GCEH131 - El atributo Pozo debe registrarse por que existen los atributos  Campo y Yacimiento.";
                                        return result;
                                    }
                                }
                            }
                        }
                    }
                    result = "0";
                }
            }
            catch (Exception ex)
            {
                result = "GCEH999 - Error no clasificado " + ex.Message;
            }
            return result;
        }

        public string validarNumeroPedimento(string NumeroPedimento, string Error)
        {
            string result;
            try
            {
                if (NumeroPedimento.Length != 21)
                {
                    result = Error + " - El número de pedimento es inválido.";
                    return result;
                }
                string fechaA = DateTime.Today.Year.ToString();
                fechaA = fechaA.Substring(2, 2);
                string UltimoFecha = fechaA.Substring(1, 1);
                string añoActual = NumeroPedimento.Substring(0, 2);
                string Aduanas = NumeroPedimento.Substring(4, 2);
                string PatenteAduanal = NumeroPedimento.Substring(8, 4);
                string cantidad = NumeroPedimento.Substring(14, 7);
                if (Convert.ToInt16(añoActual) > Convert.ToInt16(fechaA))
                {
                    result = Error + " - El número de pedimento es inválido.";
                    return result;
                }
                if (Convert.ToInt16(añoActual) < Convert.ToInt16(fechaA) - 10)
                {
                    result = Error + " - El número de pedimento es inválido.";
                    return result;
                }
                OperacionesCatalogos ox1x = new OperacionesCatalogos();
                c_NumPedimentoAduana x = ox1x.Consultar_Aduanas((int)Convert.ToInt16(Aduanas));
                if (x == null)
                {
                    result = Error + " - El número de pedimento es inválido.";
                    return result;
                }
                OperacionesCatalogos oxx = new OperacionesCatalogos();
                c_PatenteAduanal x2 = oxx.Consultar_Patente((int)Convert.ToInt16(PatenteAduanal));
                if (x2 == null)
                {
                    result = Error + " - El número de pedimento es inválido.";
                    return result;
                }
            }
            catch (Exception ee)
            {
                NtLinkBusiness.Logger.Error("", ee);
                result = Error + " - El número de pedimento es inválido.";
                return result;
            }
            result = "OK";
            return result;
        }
    }
}
