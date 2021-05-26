using log4net.Config;
using System;
using System.Collections.Generic;
using Planesderetiro11;
using ServicioLocal.Business.ComplementoRetencion;
using I_RFC_SAT;
using CatalogosSAT;

namespace ServicioLocal.Business
{
    public class ValidarSPT : NtLinkBusiness
    {
        public ValidarSPT()
        {
            XmlConfigurator.Configure();
        }

        public string ProcesarSPT(ServiciosPlataformasTecnologicas s, ServicioLocal.Business.ComplementoRetencion.Retenciones retencion)
        {
             try
            {
                if(retencion.Version!="1.0")
                    return "SPT101 - El atributo Version tiene un valor inválido.";
                if(retencion.CveRetenc!=ComplementoRetencion.c_Retenciones.Item26)
                    return "SPT102 - El atributo CveRetenc contiene una clave distinta a \"26\" (Servicios mediante Plataformas Tecnológicas)";
                if(!string.IsNullOrEmpty( retencion.DescRetenc))
                    return "SPT103 - Se registró el atributo DescRetenc.";
                if (retencion.Receptor!=null)
                  if(retencion.Receptor.Nacionalidad!=ComplementoRetencion.RetencionesReceptorNacionalidad.Nacional)
                    return "SPT104 - El complemento “Servicios Plataformas Tecnológicas” no debe existir cuando el valor del atributo Nacionalidad es distinto de \"Nacional\".";
                if (retencion.Receptor.Item != null)
                {
                    try
                    {
                        retencion.Receptor.Extranjero = (RetencionesReceptorExtranjero)retencion.Receptor.Item;
                    }
                    catch (Exception ex)
                    { }
                    try
                    {
                        retencion.Receptor.Nacional = (RetencionesReceptorNacional)retencion.Receptor.Item;
                    }
                    catch (Exception ex)
                    { }
                    if (retencion.Receptor.Nacional != null)
                    {
                        Operaciones_IRFC r = new Operaciones_IRFC();
                        vI_RFC t = r.Consultar_IRFC(retencion.Receptor.Nacional.RFCRecep);
                        if (t == null)
                        {
                            return "SPT105 - El atributo RFCReceptor es inválido según la lista de RFC inscritos no cancelados en el SAT (l_RFC).";
                        }
                    }
                }
                 if(retencion.Periodo.MesFin!=retencion.Periodo.MesIni)
                     return "SPT106 - El valor del nodo MesFin es diferente al valor del atributo MesIni o mayor al mes en curso.";
                 var fecha= DateTime.Now;
                  if(retencion.Periodo.MesFin>fecha.Month)
                      return "SPT106 - El valor del nodo MesFin es diferente al valor del atributo MesIni o mayor al mes en curso.";
                if(retencion.Periodo.Ejerc<2019)
                    return "SPT107 - El valor del atributo Ejerc es menor a 2019 o mayor al valor del año en curso.";
                if (retencion.Periodo.Ejerc >fecha.Year)
                    return "SPT107 - El valor del atributo Ejerc es menor a 2019 o mayor al valor del año en curso.";
            if(retencion.Totales.montoTotOperacion!=s.MonTotServSIVA)
                return "SPT108 - El valor del atributo montoTotOperacion es diferente al valor registrado en el atributo MonTotServSIVA.";
            if (retencion.Totales.montoTotGrav != retencion.Totales.montoTotOperacion)
                return "SPT109 - El valor del atributo montoTotGrav es diferente al valor del atributo montoTotOperacion.";
            if (retencion.Totales.montoTotExent != 0)
                return "SPT110 - El valor del atributo montoTotExent es diferente de 0.00.";

            if (retencion.Totales.ImpRetenidos != null)
                if (retencion.Totales.ImpRetenidos.Length>0)
            {
                       OperacionesCatalogos o = new OperacionesCatalogos();
                              

                decimal imp = 0.0M;
                foreach (var i in retencion.Totales.ImpRetenidos)
                {
                    imp = imp + i.montoRet;
                    if(i.ImpuestoSpecified==true)
                        if(i.Impuesto==RetencionesTotalesImpRetenidosImpuesto.Item01 ||i.Impuesto==RetencionesTotalesImpRetenidosImpuesto.Item02)
                            if(retencion.Totales.ImpRetenidos.Length>1)
                                return "SPT112 - Existe más de 1 nodo de ImpRetenidos para cada tipo de impuesto ISR (01) o para IVA (02).";
                   if(i.BaseRetSpecified==true)
                     if (i.BaseRet == retencion.Totales.montoTotOperacion)
                         return "SPT113 - El valor del atributo BaseRet es diferente al valor del atributo montoTotOperacion o es diferente del valor contenido en uno de los rangos establecidos en los catálogos \"c_RangoSemRet \" o \" c_RangoMenRet \".";
                   if (i.ImpuestoSpecified == true && i.BaseRetSpecified==true)
                   {
                       double montoRet = o.Consultar_rangoMensualSemanal(s.Periodicidad, i.BaseRet, i.Impuesto.ToString());
                       if (i.montoRet != (i.BaseRet * (decimal)montoRet))
                           return "SPT114 - El valor del atributo montoRet se encuentra fuera del rango establecido de acuerdo a la clave del atributo “Periodicidad”, registrada en el complemento o no corresponde con el factor aplicable del catálogo correspondiente.";
            
                   }

                }
                if (retencion.Totales.montoTotRet != imp)
                    return "SPT111 - El valor del atributo montoTotRet es diferente de la suma de los atributos montoRet del nodo ImpRetenidos.";
        
                  

           }
                   
           //--------------------------------------------------------
        
                ServicioLocal.Business.c_Periodicidad  myTipoPeriodicidad;
                Enum.TryParse<ServicioLocal.Business.c_Periodicidad>(s.Periodicidad, out myTipoPeriodicidad);
                      if (myTipoPeriodicidad.ToString() != s.Periodicidad)
                                    {
                                        return "SPT115 - El atributo Periodicidad tiene un valor no permitido.";
                                    }
                    if(Convert.ToInt32( s.NumServ)!=s.Servicios.Length)
                       return "SPT116 - El NumServ registrado es diferente de la suma de los elementos hijo del nodo “Servicios”.";
                 decimal precioSinIVA=0;
                 decimal ImpuestosTrasladadosdelServicio = 0;   
                 foreach (var i in s.Servicios)
                     {
                       precioSinIVA=precioSinIVA+i.PrecioServSinIVA;
                     if(i.ImpuestosTrasladadosdelServicio!=null)
                       ImpuestosTrasladadosdelServicio = ImpuestosTrasladadosdelServicio + i.ImpuestosTrasladadosdelServicio.Importe;
                     }

                 if (s.MonTotServSIVA != precioSinIVA)
                     return "SPT117 - El valor del atributo MonTotServSIVA es diferente de la suma de los atributos “PrecioServSinIVA” registrados en los nodos hijos “DetallesDelServicio”.";

                 if (s.TotalIVATrasladado != ImpuestosTrasladadosdelServicio)
                     return "SPT118 - El valor del atributo TotalIVATrasladado es diferente de la suma de los atributos “Importe” del nodo “ImpuestosTrasladadosdelServicio”.";

                 decimal tiVA=0;
                 OperacionesCatalogos o2 = new OperacionesCatalogos();
                 double MonTotServSIVA = o2.Consultar_rangoMensualSemanal(s.Periodicidad, s.MonTotServSIVA, "02");
                 tiVA = s.MonTotServSIVA * (decimal)MonTotServSIVA;
                 if(s.TotalIVARetenido!=tiVA)
                 return "SPT119 - El valor del atributo TotalIVARetenido es diferente el producto obtenido al multiplicar el valor del atributo “MonTotServSIVA” por la tasa de retención de IVA del catálogo “c_RangoMenRet” o “c_RangoSemRet” según corresponda.";

                 decimal tISR = 0;
                 OperacionesCatalogos o3 = new OperacionesCatalogos();
                 double MonTotServSIVA2 = o3.Consultar_rangoMensualSemanal(s.Periodicidad, s.MonTotServSIVA, "01");
                 tISR = s.MonTotServSIVA * (decimal)MonTotServSIVA2;
                 if (s.TotalISRRetenido != tISR)
                     return "SPT120 - El valor del atributo TotalISRRetenido es diferente del producto obtenido al multiplicar el valor del atributo “MonTotServSIVA” por la tasa de retención de ISR del catálogo “c_RangoMenRet” o “c_RangoSemRet” según corresponda de acuerdo al rango en el que se encuentre el valor del atributo “MonTotServSIVA”.";


                 if(s.DifIVAEntregadoPrestServ!=(s.TotalIVATrasladado-s.TotalIVARetenido))
                     return "SPT121 - El valor del atributo DifIVAEntregadoPrestServ es distinto del producto obtenido de la diferencia entre el valor del atributo “TotalIVATrasladado” y el valor de atributo “TotaldeIVARetenido”.";
                 decimal ComisionDelServicio=0;
                 decimal contribucion = 0;
                  foreach (var i in s.Servicios)
                     {
                         ComisionDelServicio = ComisionDelServicio + i.ComisionDelServicio.Importe;
                         if (i.ContribucionGubernamental != null)
                         {
                             if (s.MonTotalContribucionGubernamentalSpecified == false)
                                 return "SPT123 - Se debe registrar el MonTotalContribucionGubernamental siempre que exista el nodo “ContribucionGubernamental” o su valor es diferente al resultado de la suma del atributo “ImpContrib” de los nodos hijos “ContribucionGubernamental” del nodo hijo “DetallesDelServicio”.";
                           contribucion=contribucion+  i.ContribucionGubernamental.ImpContrib;
                         }
                      }
                  if (s.MonTotalporUsoPlataforma != ComisionDelServicio)
                      return "SPT122 - El valor del atributo MonTotalporUsoPlataforma es diferente la suma de los atributos “Importe” de los nodos “ComisiondelServicio”.";
                  if (s.MonTotalContribucionGubernamental != contribucion)
                      return "SPT123 - Se debe registrar el MonTotalContribucionGubernamental siempre que exista el nodo “ContribucionGubernamental” o su valor es diferente al resultado de la suma del atributo “ImpContrib” de los nodos hijos “ContribucionGubernamental” del nodo hijo “DetallesDelServicio”.";
              
                    foreach (var i in s.Servicios)
                     {
                       
                      ServicioLocal.Business.c_FormaPagoServ  myTipoFormaPago;
                      Enum.TryParse<ServicioLocal.Business.c_FormaPagoServ>(i.FormaPagoServ, out myTipoFormaPago);
                      if (myTipoFormaPago.ToString() != i.FormaPagoServ)
                                    {
                                        return "SPT124 - La clave registrada en el atributo FormaPagoServ es diferente a las contenidas en el catálogo c_FormaPagoServ.";
                                    }
                        ServicioLocal.Business.c_TipoDeServ  myTipoServicio;
                      Enum.TryParse<ServicioLocal.Business.c_TipoDeServ>(i.TipoDeServ, out myTipoServicio);
                      if (myTipoServicio.ToString() != i.TipoDeServ)
                                    {
                                        return "SPT125 - El atributo TipoDeServ tiene una clave diferente a las establecidas en el catálogo c_TipoDeServ.";
                                    }

                        if(i.SubTipServSpecified==true)
                        {
                         ServicioLocal.Business.c_SubTipoServ  myTipoSubtipo;
                      Enum.TryParse<ServicioLocal.Business.c_SubTipoServ>(i.SubTipServ, out myTipoSubtipo);
                      if (myTipoSubtipo.ToString() != i.SubTipServ)
                                  return "SPT126 - El valor del atributo SubTipServ es diferente a la relación del catálogo “c_SubTipoServ” con el tipo de servicio.";
                        }
                                               
                        Operaciones_IRFC r2 = new Operaciones_IRFC();
                    vI_RFC t2 = r2.Consultar_IRFC(i.RFCTerceroAutorizado);
                    if (t2 == null)
                    {
                        return "SPT127 - El valor capturado en el atributo RFCTerceroAutorizado es inválido según la lista de RFC inscritos no cancelados en el SAT (l_RFC).";
                    }
                    if(i.FechaServ.Month!=retencion.Periodo.MesIni)       
                       return "SPT128 - El valor del atributo MesIni es diferente al valor registrado en el atributo FechaServ del CFDI de Retenciones o mayor al mes en curso.";
                    if(i.ImpuestosTrasladadosdelServicio.Base!=i.PrecioServSinIVA)
                       return "SPT129 - El valor del atributo Base del nodo “ImpuestosTrasladadosdelServicio” es diferente al valor del atributo “PrecioServSinIVA”.";
               
                        if((i.ImpuestosTrasladadosdelServicio.TasaCuota*i.ImpuestosTrasladadosdelServicio.Base)!=i.ImpuestosTrasladadosdelServicio.Importe)
                     return "SPT130 - El valor del atributo Importe, del nodo “ImpuestosTrasladadosdelServicio es diferente del producto obtenido al multiplicar el valor del atributo “Base” por el valor del atributo “TasaCuota” del nodo hijo “ImpuestosTrasladadosdelServicio”.";
               
                        ServicioLocal.Business.c_EntidadesFederativas  myTipoEntidad;
                      Enum.TryParse<ServicioLocal.Business.c_EntidadesFederativas>(i.ContribucionGubernamental.EntidadDondePagaLaContribucion, out myTipoEntidad);
                      if (myTipoEntidad.ToString() != i.ContribucionGubernamental.EntidadDondePagaLaContribucion)
                                  return "SPT131 - El valor del atributo EntidadDondePagaLaContribucion es diferente de la clave del catálogo c_EntidadesFederativas.";
                      
                        if(i.ComisionDelServicio.Importe<=0)
                             return "SPT132 - El valor del atributo Importe del nodo “ComisiondelServicio” es igual a cero.";
                      
                   }


                     return "0";
               
            }
            catch (Exception ex)
            {
                return "SPT999 - Error no clasificado";
            }
          
        }
    }
}
