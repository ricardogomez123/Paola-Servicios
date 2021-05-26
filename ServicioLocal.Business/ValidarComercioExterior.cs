

using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;
using MessagingToolkit.QRCode.Codec;
using log4net;
using log4net.Config;
using System;
using EspacioComercioExterior11;
using System.Text.RegularExpressions;
using ServicioLocal.catCFDI;
using I_RFC_SAT;


namespace ServicioLocal.Business
{
   public class ValidarComercioExterior : NtLinkBusiness
    {
        private readonly XNamespace _ns = "http://www.sat.gob.mx/cfd/3";
        private readonly XNamespace _ns2 = "http://www.sat.gob.mx/ComercioExterior11";

    
        public ValidarComercioExterior()
        {
           XmlConfigurator.Configure();
        }
       //---------------------------------------------------------------------------------------------
        public string ProcesarComercioExterior(ComercioExterior comer, Comprobante com)
        {
         //-------------------------------CFDI 3.2----------------------------------------------------
            if(com.Version!="3.2" && com.Version!="3.3")
                return ("CCE101 - El atributo cfdi:Comprobante:version no tiene un valor valido");
           /* if (!string.IsNullOrEmpty(com.Fecha))
            {


                //fecha = fecha.Replace("fecha=", "");
                //fecha = fecha.Replace("\"", "");
                Regex regex7 = new Regex(@"[0-9]{4}-(0[1-9]|1[0-2])-(0[1-9]|[12][0-9]|3[01])T(([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9])");
                if (!regex7.IsMatch(com.Fecha))
                    return ("CCE102 - El atributo cfdi:Comprobante:fecha no cumple con el patrón requerido.");
            }
            decimal sumaconceptos = 0;
            foreach (var con in com.Conceptos)
            {
                sumaconceptos += con.Importe;
            }
            if(sumaconceptos!=com.SubTotal)
                return ("CCE103 - El atributo cfdi:Comprobante:subtotal no coincide con la suma de los atributos importe de los nodos Concepto.");

            if(string.IsNullOrEmpty(com.Moneda))
                return ("CCE104 - El atributo cfdi:Comprobante:Moneda se debe registrar");
            else
            {
                if (com.TipoCambioSpecified==true)
                {
                    string TipoCambio = com.TipoCambio.ToString();
                  
                        string[] split = TipoCambio.Split(".".ToCharArray());
                        long number1 = 0;
                        bool canConvert = long.TryParse(split[0], out number1);
                        if(canConvert==false)
                            return ("CCE109 - El atributo cfdi:Comprobante:TipoCambio no cumple con el patrón requerido.");
                        if (split[0].Length > 14 || split[0].Length<1)
                            return ("CCE109 - El atributo cfdi:Comprobante:TipoCambio no cumple con el patrón requerido.");
                        if (split.Count() > 1)
                        {
                             canConvert = long.TryParse(split[1], out number1);
                            if (canConvert == false)
                                return ("CCE109 - El atributo cfdi:Comprobante:TipoCambio no cumple con el patrón requerido.");
                      
                            if (split[1].Length > 6)
                                return ("CCE109 - El atributo cfdi:Comprobante:TipoCambio no cumple con el patrón requerido.");
                        }
                   

                    //Regex regex1 = new Regex(@"[\d]{1,14}([\.]([\d]{1,6}))?");
                    //if (!regex1.IsMatch(com.TipoCambio))
                    //    return ("CCE109 - El atributo cfdi:Comprobante:TipoCambio no cumple con el patrón requerido.");
                }

            c_Moneda myTipoMoneda;
            Enum.TryParse(com.Moneda, out myTipoMoneda);
            if (myTipoMoneda.ToString() !=  com.Moneda)
                return ("CCE105 - El atributo cfdi:Comprobante:Moneda no contiene un valor del catálogo catCFDI:c_Moneda.");
        
                if(com.Moneda=="MXN")
                { if(com.TipoCambioSpecified==true)
                        if(com.TipoCambio!=1)
                            return ("CCE106 - El atributo TipoCambio no tiene el valor \"1\" y la moneda indicada es MXN.");
                }else
                    if (com.Moneda != "XXX")
                    { 
                       if(com.TipoCambioSpecified==false)
                           return ("CCE107 - El atributo cfdi:Comprobante:TipoCambio se debe registrar cuando el atributo cfdi:Comprobante:Moneda tiene un valor distinto de MXN y XXX.");
                    }
                    else
                        if(com.TipoCambioSpecified==true)
                            return ("CCE108 - El atributo cfdi:Comprobante:TipoCambio no se debe registrar cuando el atributo cfdi:Comprobante:Moneda tiene el valor XXX.");
      
            }
            if(com.TipoDeComprobante==null)
                return ("CCE110 - El atributo cfdi:Comprobante:tipoDeComprobante no cumple con alguno de los valores permitidos.");
            else
                if (com.TipoDeComprobante != "E" &&
                    com.TipoDeComprobante != "I" &&
                    com.TipoDeComprobante != "T")
                    return ("CCE110 - El atributo cfdi:Comprobante:tipoDeComprobante no cumple con alguno de los valores permitidos.");

            if (comer.Propietario != null)
            {
                if (comer.Propietario.Count > 0)
                {
                    if (com.TipoDeComprobante != "T" && (comer.MotivoTraslado.ToString() != "Item05" && comer.MotivoTraslado.ToString() != "05"))
                    {
                        return ("CCE114 - El nodo Propietario no debe existir cuando cfdi:Comprobante:tipoDeComprobante es distinto de \"traslado\" y MotivoTraslado tiene una clave distinta de \"05\".");
                    }
                }
            }
//----------------------se movio para que cuadre
            if (com.Receptor != null)
            {
                if (!string.IsNullOrEmpty(comer.MotivoTraslado))
                {
                    if (com.TipoDeComprobante != "T" && (comer.MotivoTraslado.ToString() != "Item02" && comer.MotivoTraslado.ToString() != "02"))
                    {
                        if (com.Receptor.Rfc != "XEXX010101000")
                            return ("CCE132 - El atributo cfdi:Comprobante:Receptor:rfc no tiene el valor \"XEXX010101000\" y el tipoDeComprobante tiene un valor distinto de \"traslado\" y MotivoTraslado un valor distinto de \"02\".");

                    }
                }
            }
            //----------------------
            if (com.TipoDeComprobante == "T")
            {
                if (comer.MotivoTrasladoSpecified == false)
                    return ("CCE111 - El atributo MotivoTraslado debe registrarse cuando cfdi:Comprobante:tipoDeComprobante tiene el valor \"traslado\".");
                else
                    if (comer.MotivoTraslado.ToString() == "Item05" || comer.MotivoTraslado.ToString() == "05")
                    {
                        if (comer.Propietario == null)
                            return ("CCE112 - El nodo Propietario se debe registrar cuando cfdi:Comprobante:tipoDeComprobante tiene el valor \"traslado\" y MotivoTraslado tiene la clave \"05\".");
                        if (comer.Propietario.Count() == 0)
                            return ("CCE112 - El nodo Propietario se debe registrar cuando cfdi:Comprobante:tipoDeComprobante tiene el valor \"traslado\" y MotivoTraslado tiene la clave \"05\".");

                    }
            }
            else
            {
                 if (comer.MotivoTrasladoSpecified == true)
                     return ("CCE113 - El atributo MotivoTraslado no debe existir cuando cfdi:Comprobante:tipoDeComprobante es distinto de \"traslado\".");
          
            }
            
             
            decimal total=0;
            total=com.SubTotal-com.Descuento;
            if(com.Impuestos!=null){
            if(com.Impuestos.TotalImpuestosTrasladadosSpecified==true )
            total=total+com.Impuestos.TotalImpuestosTrasladados;
                if(com.Impuestos.TotalImpuestosRetenidosSpecified==true)
               total=total-com.Impuestos.TotalImpuestosRetenidos;
           
            }
            
            if(com.Total!=total )
                return ("CCE115 - El atributo cfdi:Comprobante:total no coincide con la suma del cdi:Comprobante:subTotal, menos el cfdi:Comprobante:descuento, más cfdi:Comprobante:Impuestos:totalImpuestosTrasladados menos cfdi:Comprobante:Impuestos:totalImpuestosRetenidos.");
         
             if (!string.IsNullOrEmpty(com.LugarExpedicion))
                {
                    Regex regex1 = new Regex(@"[0-9]{5}");
                    if (!regex1.IsMatch(com.LugarExpedicion))
                        return ("CCE116 - El atributo cfdi:Comprobante:LugarExpedicion no cumple con alguno de los valores permitidos.");

                    CatalogosSAT.OperacionesCatalogos o = new CatalogosSAT.OperacionesCatalogos();
                    CatalogosSAT.c_CP cp = o.Consultar_CP(com.LugarExpedicion);
                 if(cp==null)
                     return ("CCE116 - El atributo cfdi:Comprobante:LugarExpedicion no cumple con alguno de los valores permitidos.");

             }
           
            if(com.Receptor!=null)
            {
                if (!string.IsNullOrEmpty(comer.MotivoTraslado))
                {
                    if (com.TipoDeComprobante == "T" && (comer.MotivoTraslado.ToString() == "Item02" || comer.MotivoTraslado.ToString() == "02"))
                    {
                        if (!string.IsNullOrEmpty(com.Receptor.Rfc))
                        {
                            if (com.Receptor.Rfc != "XEXX010101000")
                            {
                                I_RFC_SAT.Operaciones_IRFC r = new I_RFC_SAT.Operaciones_IRFC();
                                vI_RFC t = r.Consultar_IRFC(com.Receptor.Rfc);
                                if (t == null)
                                {
                                    return ("CCE133 - El atributo cfdi:Comprobante:Receptor:rfc debe tener un RFC válido dentro de la lista de RFC's o el valor \"XEXX010101000\" cuando el tipoDeComprobante es \"traslado\" y MotivoTraslado es \"02\".");

                                }
                            }
                        }
                        else
                            return ("CCE133 - El atributo cfdi:Comprobante:Receptor:rfc debe tener un RFC válido dentro de la lista de RFC's o el valor \"XEXX010101000\" cuando el tipoDeComprobante es \"traslado\" y MotivoTraslado es \"02\".");
                    }
                }
                if(string.IsNullOrEmpty(com.Receptor.Nombre))
                    return ("CCE134 - El atributo cfdi:Comprobante:Receptor:nombre se debe registrar.");
            
                bool activo=false;

                if (com.TipoDeComprobante != "T")
                    activo=true;
                if(comer.MotivoTrasladoSpecified==true)
                    if (comer.MotivoTraslado.ToString() != "Item02" && comer.MotivoTraslado.ToString() != "02")
                  activo=true;
                        }
            */ 
         //-------------------------------Complemento Exterior 1.1---------------------------------------------------------
            if (com.Complemento!=null)
            {
              var comple=  com.Complemento.Any;
              foreach(System.Xml.XmlElement Com in comple)
              {
                  if (Com.Name != "cce11:ComercioExterior" && Com.Name != "leyendasFisc:LeyendasFiscales" && Com.Name != "implocal:ImpuestosLocales"
                      && Com.Name != "pago10:Pagos" && Com.Name != "registrofiscal:registrofiscal")
                     return ("CCE155 - El nodo cce11:ComercioExterior solo puede coexistir con los complementos Timbre Fiscal Digital, otros derechos e impuestos, leyendas fiscales, recepción de pago, CFDI registro fiscal.");
              }
                           
            }
            if (comer.MotivoTrasladoSpecified == true)
            { if(com.CfdiRelacionados!=null)
            {
                if(comer.MotivoTraslado=="01" && com.CfdiRelacionados.TipoRelacion=="05")
                    if(com.CfdiRelacionados.CfdiRelacionado[0].UUID==null)
                        return ("CCE157 - El atributo cfdi:CfdiRelacionados:CfdiRelacionado:UUID se debe registrar si el valor de cce11:ComercioExterior:MotivoTraslado es \"01\" con el tipo de relación \"05\".");
          
             }
            }

            if (comer.TipoOperacion.ToString() ==  "A")
            {
                
                if (comer.MotivoTrasladoSpecified == true || comer.ClaveDePedimentoSpecified == true
                    || comer.CertificadoOrigenSpecified == true || !string.IsNullOrEmpty(comer.NumCertificadoOrigen
                    ) || !string.IsNullOrEmpty(comer.NumeroExportadorConfiable) || comer.IncotermSpecified == true
                    || comer.SubdivisionSpecified == true || comer.TipoCambioUSDSpecified == true
                    || comer.TotalUSDSpecified == true || comer.Mercancias!=null)
                 
                {
                    return ("CCE158 - Los atributos MotivoTraslado, ClaveDePedimento, CertificadoOrigen, NumCertificadoOrigen, NumeroExportadorConfiable, Incoterm, Subdivision, TipoCambioUSD, TotalUSD, Mercancias no deben existir si el valor de cce11:ComercioExterior:TipoOperacion es \"A\".");
     
                }
                
            }
            if (comer.TipoOperacion.ToString() == "1" || comer.TipoOperacion.ToString() == "2")
            {
                
                if (comer.ClaveDePedimentoSpecified==false
                    || comer.CertificadoOrigenSpecified == false || comer.IncotermSpecified == false
                    || comer.SubdivisionSpecified == false || comer.TipoCambioUSDSpecified == false
                    || comer.TotalUSDSpecified == false || comer.Mercancias == null)
                
                {
                    return ("CCE159 - Los atributos ClaveDePedimento, CertificadoOrigen, Incoterm, Subdivision, TipoCambioUSD, TotalUSD ,Mercancias deben registrarse si la clave de cce11:ComercioExterior:TipoOperacion registrada es \"1\" ó \"2\".");

                }

            }
             if (comer.CertificadoOrigen == 0)
            { 
              if( !string.IsNullOrEmpty(comer.NumCertificadoOrigen))
                  return ("CCE160 - El atributo cce11:ComercioExterior:NumCertificadoOrigen no se debe registrar si el valor de cce11:ComercioExterior:CertificadoOrigen es \"0\".");

            }
            
             CatalogosSAT.OperacionesCatalogos o4 = new CatalogosSAT.OperacionesCatalogos();

             if (!string.IsNullOrEmpty(comer.NumeroExportadorConfiable))
             {
                 CatalogosSAT.c_Pais pais2 = o4.Consultar_Pais(comer.Receptor.Domicilio.Pais.ToString());
                 if (pais2 != null)
                 {
                     if (pais2.Agrupaciones != "Unión Europea")
                     {
                         return ("CCE161 - El atributo cce11:ComercioExterior:NumExportadorConfiable no se debe registrar si la clave de país del receptor o del destinatario no corresponde a un país del catálogo catCFDI:c_Pais donde la columna Agrupación tenga el valor Unión Europea.");

                     }

                 }
             }
             
             decimal valorDolar = 0;
             string totalUSD = comer.TotalUSD.ToString();
             if (totalUSD != null)
             {
                 string[] split = totalUSD.Split(".".ToCharArray());
                 if (split.Count() > 1)
                 {
                     if (split[1].Count() != 2)
                         return ("CCE163 - El atributo cce11:ComercioExterior:TotalUSD debe registrarse con dos decimales.");
                 }
                 else
                     return ("CCE163 - El atributo cce11:ComercioExterior:TotalUSD debe registrarse con dos decimales.");

             }
       
             foreach (var m in comer.Mercancias)
             {
                valorDolar=valorDolar+ m.ValorDolares;
             }
             if (comer.TotalUSD != valorDolar)
             {
                 return ("CCE162 - El atributo cce11:ComercioExterior:TotalUSD no coincide con la suma de ValorDolares de las mercancías.");
             }
            
            //----------------------------------------------------------------------------------------------
             if (!string.IsNullOrEmpty(com.Emisor.Rfc))
             {
                 if (com.Emisor.Rfc.Count() == 12)
                 {   if(comer.Emisor!=null)
                     if (!string.IsNullOrEmpty(comer.Emisor.Curp))
                         return ("CCE164 - El atributo cce11:ComercioExterior:Emisor:Curp no se debe registrar si el atributo Rfc del nodo cfdi:Comprobante:Emisor es de longitud 12.");

                 }
                 if (com.Emisor.Rfc.Count() == 13)
                 {
                     if (comer.Emisor != null)
                     {
                         if (string.IsNullOrEmpty(comer.Emisor.Curp))
                             return ("CCE165 - El atributo cce11:ComercioExterior:Emisor:Curp se debe registrar si el atributo Rfc del nodo cfdi:Comprobante:Emisor es de longitud 13.");
                     }
                     else
                         return ("CCE165 - El atributo cce11:ComercioExterior:Emisor:Curp se debe registrar si el atributo Rfc del nodo cfdi:Comprobante:Emisor es de longitud 13.");
      
                 }

             }
             if (com.Version == "3.2")
             {
                 if(comer.MotivoTrasladoSpecified==true)
                     if (comer.MotivoTraslado.ToString() == "Item01" || comer.MotivoTraslado.ToString() == "01")
                 //if(string.IsNullOrEmpty(com.FolioFiscalOrig))
                 //    return ("CCE156 - El atributo cfdi:FolioFiscalOrig se debe registrar si el valor de cce11:ComercioExterior:MotivoTraslado es \"01\".");
      

                 if(comer.Receptor!=null)
                 if (string.IsNullOrEmpty(comer.Receptor.NumRegIdTrib))
                     return ("CCE177 - El atributo cce11:ComercioExterior:Receptor:NumRegIdTrib debe registrarse si la versión de CFDI es 3.2.");
    
                 if(comer.Emisor!=null)
                   if(comer.Emisor.Domicilio!=null)
                    return ("CCE166 - El nodo cce11:ComercioExterior:Emisor:Domicilio no debe registrarse si la versión de CFDI es 3.2.");
                 if(comer.Receptor.Domicilio!=null)
                     return ("CCE180 - El nodo cce11:ComercioExterior:Receptor:Domicilio no debe registrarse si la versión de CFDI es 3.2.");
                /*
                 CatalogosSAT.OperacionesCatalogos o8 = new CatalogosSAT.OperacionesCatalogos();
                
                 CatalogosSAT.c_Pais pais10 = o8.Consultar_PaisVerificacionLinea(com.Receptor.Domicilio.pais.ToString());
                 if (pais10 != null)
                 {
                     if (pais10.ValidaciondelRIT != null)// para MEX
                     {
                         I_RFC_SAT.Operaciones_IRFC r = new I_RFC_SAT.Operaciones_IRFC();
                         vI_RFC t = r.Consultar_IRFC(comer.Receptor.NumRegIdTrib);
                         if (t == null)
                         {
                             return ("CCE178 - El atributo cce11:ComercioExterior:Receptor:NumRegIdTrib no tiene un valor que exista en el registro del país indicado en el atributo cfdi:Comprobante:Receptor:Domicilio:pais.");

                         }

                     }
                     else
                     {
                         if (!Regex.Match(comer.Receptor.NumRegIdTrib, "^" + pais10.FormatodeRIT+"$").Success)
                              return ("CCE179 - El atributo cce11:ComercioExterior:Receptor:NumRegIdTrib no cumple con el patrón publicado en la columna \"Formato de registro de identidad tributaria\" del país indicado en el atributo cfdi:Comprobante:Receptor:Domicilio:pais.");

                     }
                 }
                 */
             }
             if (com.Version == "3.3")
             {
                 
                 if (com.TipoDeComprobante != "I" && com.TipoDeComprobante != "E" && com.TipoDeComprobante != "T")
                     return ("CCE145 - El atributo cfdi:Comprobante:TipoDeComprobante no cumple con alguno de los valores permitidos para este complemento.");
                  if (com.TipoDeComprobante == "T")
                  {  if(comer.MotivoTrasladoSpecified==false)
                          return ("CCE146 - El atributo MotivoTraslado se debe registrar cuando el atributo cfdi:Comprobante:TipoDeComprobante tiene el valor \"T\".");
                  if (comer.MotivoTraslado == "Item05" || comer.MotivoTraslado == "05")
                  {
                      if (comer.Propietario == null)
                          return ("CCE147 - El nodo Propietario se debe registrar cuando el atributo cfdi:Comprobante:TipoDeComprobante tiene el valor \"T\" y MotivoTraslado tiene la clave \"05\".");
                      if (comer.Propietario.Count() == 0)
                          return ("CCE147 - El nodo Propietario se debe registrar cuando el atributo cfdi:Comprobante:TipoDeComprobante tiene el valor \"T\" y MotivoTraslado tiene la clave \"05\".");
               
                  }
                  }
                   if(comer.Propietario!=null)
                       if(comer.Propietario.Count()>0)
                       if(com.TipoDeComprobante!="T" && comer.MotivoTraslado!="Item05")
                      return ("CCE148 - El nodo Propietario no se debe registrar cuando el atributo cfdi:Comprobante:TipoDeComprobante tiene un valor distinto de \"T\" y MotivoTraslado tiene una clave distinta de \"05\".");
            
                 if(com.Emisor!=null)
                     if(string.IsNullOrEmpty( com.Emisor.Nombre))
                         return ("CCE149 - El atributo cfdi:Comprobante:Emisor:Nombre se debe registrar.");
            
                  if (com.TipoDeComprobante != "T" && comer.MotivoTraslado!="Item02")
                      if(com.Receptor.Rfc!="XEXX010101000")
                          return ("CCE150 - El atributo cfd:Comprobante:Receptor:Rfc no tiene el valor \"XEXX010101000\" y el TipoDeComprobante tiene un valor distinto de \"T\" y MotivoTraslado un valor distinto de \"02\".");
                if(com.Receptor!=null)
                {
                 if (!string.IsNullOrEmpty(comer.MotivoTraslado))
                {
                    if (com.TipoDeComprobante == "T" && (comer.MotivoTraslado.ToString() == "Item02" || comer.MotivoTraslado.ToString() == "02"))
                    {
                        if (!string.IsNullOrEmpty(com.Receptor.Rfc))
                        {
                            if (com.Receptor.Rfc != "XEXX010101000")
                            {
                                I_RFC_SAT.Operaciones_IRFC r = new I_RFC_SAT.Operaciones_IRFC();
                                vI_RFC t = r.Consultar_IRFC(com.Receptor.Rfc);
                                if (t == null)
                                {
                                return ("CCE151 - El atributo cfdi:Comprobante:Receptor:Rfc debe tener un RFC válido dentro de la lista de RFC's o el valor \"XEXX010101000\" cuando el TipoDeComprobante es \"T\" y MotivoTraslado es \"02\"..");
                  
                                }
                            }
                        }
                        else
                            return ("CCE151 - El atributo cfdi:Comprobante:Receptor:Rfc debe tener un RFC válido dentro de la lista de RFC's o el valor \"XEXX010101000\" cuando el TipoDeComprobante es \"T\" y MotivoTraslado es \"02\"..");
                    }
                }
                
                     if(string.IsNullOrEmpty( com.Receptor.Nombre))
                         return ("CCE152 - El atributo cfdi:Comprobante:Receptor:Nombre se debe registrar. ");
                }

                 if (comer.Emisor != null)
                 {
                     

                     if (comer.Emisor.Domicilio == null)
                         return ("CCE167 - El nodo cce11:ComercioExterior:Emisor:Domicilio debe registrarse si la versión de CFDI es 3.3.");
                     if (comer.Emisor.Domicilio.Pais.ToString() != "MEX")
                         return ("CCE168 - El atributo cce11:ComercioExterior:Emisor:Domicilio:Pais debe tener la clave \"MEX\".");
              
                     CatalogosSAT.OperacionesCatalogos o5 = new CatalogosSAT.OperacionesCatalogos();

                     if (!string.IsNullOrEmpty(comer.Emisor.Domicilio.Estado.ToString()))
                     {
                         CatalogosSAT.c_Estado estado5 = o5.Consultar_Estados(comer.Emisor.Domicilio.Estado.ToString());
                         if (estado5 == null)
                             return ("CCE169 - El atributo cce11:ComercioExterior:Emisor:Domicilio:Estado debe contener una clave del catálogo de catCFDI:c_Estado donde la columna c_Pais tiene el valor \"MEX\".");

                     }
                     if (!string.IsNullOrEmpty(comer.Emisor.Domicilio.Municipio.ToString()))
                     {
                         CatalogosSAT.c_Municipio municipio5 = o5.Consultar_Municipio(comer.Emisor.Domicilio.Estado.ToString(), comer.Emisor.Domicilio.Municipio.ToString());
                         if (municipio5 == null)
                             return ("CCE170 - El atributo cce11:ComercioExterior:Emisor:Domicilio:Municipio debe contener una clave del catálogo de catCFDI:c_Municipio donde la columna clave de c_Estado debe ser igual a la clave registrada en el atributo Estado.");
                     }
                     //--
                     if (!string.IsNullOrEmpty(comer.Emisor.Domicilio.Localidad))
                     {
                         CatalogosSAT.c_Localidad localidad5 = o5.Consultar_Localidad(comer.Emisor.Domicilio.Estado.ToString(), comer.Emisor.Domicilio.Localidad);
                         if (localidad5 == null)
                             return ("CCE171 - El atributo cce11:ComercioExterior:Emisor:Domicilio:Localidad debe contener una clave del catálogo de catCFDI:c_Localidad donde la columna clave de c_Estado debe ser igual a la clave registrada en el atributo Estado.");
                     }
                     
                     if (comer.Emisor.Domicilio.Colonia.ToString().Count() == 8)
                     {
                         CatalogosSAT.c_Colonia colonia5 = o5.Consultar_Colonia(comer.Emisor.Domicilio.CodigoPostal.ToString(), comer.Emisor.Domicilio.Colonia.ToString());
                         if (colonia5 == null)
                             return ("CCE172 - El atributo cce11:ComercioExterior:Emisor:Domicilio:Colonia debe contener una clave del catálogo de catCFDI:c_Colonia donde la columna c_CodigoPostal debe ser igual a la clave registrada en el atributo CodigoPostal.");
                     }
                     CatalogosSAT.c_CP CP5 = o5.Consultar_CP(comer.Emisor.Domicilio.Estado.ToString(), comer.Emisor.Domicilio.Municipio.ToString(), comer.Emisor.Domicilio.Localidad, comer.Emisor.Domicilio.CodigoPostal.ToString());
                     if (CP5 == null)
                         return ("CCE173 - El atributo cce11:ComercioExterior:Emisor:Domicilio:CodigoPostal debe contener una clave del catálogo catCFDI:c_CodigoPostal donde la columna clave de c_Estado debe ser igual a la clave registrada en el atributo Estado, la columna clave de c_Municipio debe ser igual a la clave registrada en el atributo Municipio y si existe el atributo de Localidad, la columna clave de c_Localidad debe ser igual a la clave registrada en el atributo Localidad.");

                 }
                 if (comer.Receptor != null)
                 {
                     if (!string.IsNullOrEmpty(comer.Receptor.NumRegIdTrib))
                         return ("CCE176 - El atributo cce11:ComercioExterior:Receptor:NumRegIdTrib no debe registrarse si la versión de CFDI es 3.3.");
                     
                     
                     
                     if (comer.Receptor.Domicilio == null)
                     return ("CCE181 - El nodo cce11:ComercioExterior:Receptor:Domicilio debe registrarse si la versión de CFDI es 3.3.");
                   CatalogosSAT.OperacionesCatalogos o6 = new CatalogosSAT.OperacionesCatalogos();
                   if (!string.IsNullOrEmpty(comer.Receptor.Domicilio.CodigoPostal) && !string.IsNullOrEmpty(comer.Receptor.Domicilio.Colonia))
                   {
                       if (comer.Receptor.Domicilio.Pais.ToString() == "MEX")
                       {
                           CatalogosSAT.c_Colonia colonia6 = o6.Consultar_ColoniaMEX(comer.Receptor.Domicilio.CodigoPostal, comer.Receptor.Domicilio.Colonia);
                           if (colonia6 == null)
                               return ("CCE182 - El atributo cce11:ComercioExterior:Receptor:Domicilio:Colonia debe tener un valor del catálogo de colonia donde la columna código postal sea igual a la clave registrada en el atributo CodigoPostal cuando la clave de país es \"MEX\", contiene una cadena numérica de cuatro posiciones y la versión de CFDI es 3.3.");
                       }
                   }
                   if (!string.IsNullOrEmpty(comer.Receptor.Domicilio.Localidad))
                   {
                       if (comer.Receptor.Domicilio.Pais.ToString() == "MEX")
                       {
                           CatalogosSAT.c_Localidad localidad6 = o6.Consultar_LocalidadMEX(comer.Receptor.Domicilio.Estado, comer.Receptor.Domicilio.Localidad);
                           if (localidad6 == null)
                               return ("CCE183 - El atributo cce11:ComercioExterior:Receptor:Domicilio:Localidad debe tener un valor del catálogo de localidades (catCFDI:c_Localidad) donde la columna c_Estado sea igual a la clave registrada en el atributo Estado cuando la clave de país es \"MEX\" y la versión de CFDI es 3.3.");
                       }
                   }
                   if (!string.IsNullOrEmpty(comer.Receptor.Domicilio.Municipio))
                   {
                       if (comer.Receptor.Domicilio.Pais.ToString() == "MEX")
                       {
                           CatalogosSAT.c_Municipio municipio6 = o6.Consultar_MunicipioMEX(comer.Receptor.Domicilio.Estado, comer.Receptor.Domicilio.Municipio);
                           if (municipio6 == null)
                               return ("CCE184 -El atributo cce11:ComercioExterior:Receptor:Domicilio:Municipio debe tener un valor del catálogo de municipios (catCFDI:c_Municipio) donde la columna c_Estado sea igual a la clave registrada en el atributo Estado cuando la clave de país es \"MEX\" y la versión de CFDI es 3.3.");
                       }
                   }
                   if (comer.Receptor.Domicilio.Pais.ToString() == "MEX" || comer.Receptor.Domicilio.Pais.ToString() == "CAN" || comer.Receptor.Domicilio.Pais.ToString() == "USA")
                   {
                       CatalogosSAT.c_Estado estado6 = o6.Consultar_EstadosPais(comer.Receptor.Domicilio.Estado, comer.Receptor.Domicilio.Pais.ToString());
                       if (estado6 == null)
                           return ("CCE185 -El atributo cce11:ComercioExterior:Receptor:Domicilio:Estado debe tener un valor del catálogo de estados catCFDI:c_Estado donde la columna c_Pais sea igual a la clave de país registrada en el atributo Pais y la versión de CFDI es 3.3.");
                   }

                     if (!string.IsNullOrEmpty( comer.Receptor.Domicilio.Pais.ToString() ))
                      if (comer.Receptor.Domicilio.Pais.ToString() != "MEX" )
                        {
                            CatalogosSAT.c_Pais pais6 = o6.Consultar_Pais(comer.Receptor.Domicilio.Pais.ToString());
                            if (pais6 != null)
                            {
                                if (!string.IsNullOrEmpty(pais6.FormatodeCP))
                                {
                                   bool cpV =true;
                                   string postal_code= comer.Receptor.Domicilio.CodigoPostal.Replace(" ", "");

                                   if (pais6.c_Pais1 == "CAN" || pais6.c_Pais1 == "USA")
                                   cpV = IsUsorCanadianZipCode(postal_code, pais6.c_Pais1);
                                   else
                                   {

                                     Regex regex5 = new Regex(pais6.FormatodeCP);
                                      if (!regex5.IsMatch(postal_code))
                                       cpV = false;
                                    }

                                    if(cpV==false)
                                    //Regex regex6 = new Regex(pais6.FormatodeCP);
                                    //if (!regex6.IsMatch(comer.Receptor.Domicilio.CodigoPostal))
                                        return ("CCE186 - El atributo cce11:ComercioExterior:Receptor:Domicilio:CodigoPostal debe cumplir con el patrón especificado para el país cuando es distinta de \"MEX\" y la versión de CFDI es 3.3.");
                                }

                            }
                        }

                     if (!string.IsNullOrEmpty(comer.Receptor.Domicilio.CodigoPostal.ToString()))
                     {
                         if (comer.Receptor.Domicilio.Pais.ToString() == "MEX")
                         {
                             CatalogosSAT.c_CP CP6 = o6.Consultar_CPMEX(comer.Receptor.Domicilio.Estado, comer.Receptor.Domicilio.Municipio, comer.Receptor.Domicilio.Localidad, comer.Receptor.Domicilio.CodigoPostal);
                             if (CP6 == null)
                                 return ("CCE187 - El atributo cce11:ComercioExterior:Receptor:Domicilio:CodigoPostal debe tener un valor del catálogo de códigos postales catCFDI:c_CodigoPostal donde la columna c_Estado sea igual a la clave registrada en el atributo Estado, la columna c_Municipio sea igual a la clave registrada en el atributo Municipio y la columna c_Localidad sea igual a la clave registrada en el atributo Localidad en caso de que se haya registrado cuando la clave de país es \"MEX\" y la versión de CFDI es 3.3.");
                         }
                     }



                 }//comer.receptor
                 
             }//---fin 3.3

            //---------------
                  CatalogosSAT.OperacionesCatalogos o18 = new CatalogosSAT.OperacionesCatalogos();
                  if (comer.Propietario!=null)      
            foreach (var prop in comer.Propietario)
                  {
                      CatalogosSAT.c_Pais pais7 = o18.Consultar_PaisVerificacionLinea(prop.ResidenciaFiscal.ToString());
                      if(pais7!=null)
                      {
                          if (pais7.ValidaciondelRIT != null) //para mex
                          {
                              I_RFC_SAT.Operaciones_IRFC r = new I_RFC_SAT.Operaciones_IRFC();
                              vI_RFC t = r.Consultar_IRFC(prop.NumRegIdTrib);
                              if (t == null)
                              {
                                  return ("CCE174 - El atributo cce11:ComercioExterior:Propietario:NumRegIdTrib no tiene un valor que exista en el registro del país indicado en el atributo cce1:Propietario:ResidenciaFiscal.");

                              }

                          }
                          else  //caso contrario otro pais con RIT
                          {//--  --
                              if (pais7.FormatodeRIT!=null)
                              if (!Regex.Match(prop.NumRegIdTrib, "^" + pais7.FormatodeRIT+"$").Success)
                                  return ("CCE175 - El atributo cce11:ComercioExterior:Propietario:NumRegIdTrib no cumple con el patrón publicado en la columna \"Formato de registro de identidad tributaria\" del país indicado en el atributo cce1:Propietario:ResidenciaFiscal.");

                          }
                      }
                  }
                 //------------------
                   
            //--------------------------------------

                  if (com.TipoDeComprobante == "T" && (com.Version == "3.2" || com.Version == "3.3"))
             { 
               if( comer.Destinatario!=null)
                   if(comer.Destinatario.Count()>1)
                       return ("CCE188 - El campo tipoDeComprobante tiene el valor \"traslado\" por lo tanto sólo podrás registrar un Destinatario.");

             }
             if (comer.Destinatario != null)
             {
                 if (comer.Destinatario.Count>0)
                 foreach (var domi in comer.Destinatario[0].Domicilio)
                 {
                     CatalogosSAT.c_Pais pais17 = o18.Consultar_PaisVerificacionLinea(domi.Pais.ToString());
                     if (pais17 != null)
                     {
                         if (pais17.ValidaciondelRIT != null)
                         {
                             I_RFC_SAT.Operaciones_IRFC r = new I_RFC_SAT.Operaciones_IRFC();
                             vI_RFC t = r.Consultar_IRFC(comer.Destinatario[0].NumRegIdTrib);
                             if (t == null)
                             {
                                 return ("CCE189 - El atributo cce11:ComercioExterior:Destinatario:NumRegIdTrib no tiene un valor que exista en el registro del país indicado en el atributo cce11:ComercioExterior:Destinatario:Domicilio:Pais.");

                             }

                         }
                         else
                         {
                            // Regex regex12 = new Regex(pais17.FormatodeRIT);
                           //  if (!regex12.IsMatch(comer.Destinatario[0].NumRegIdTrib))
                             if (!string.IsNullOrEmpty( pais17.FormatodeRIT))
                             if (!Regex.Match(comer.Destinatario[0].NumRegIdTrib,"^"+ pais17.FormatodeRIT+"$").Success)
                                          return ("CCE190 - El atributo cce11:ComercioExterior:Destinatario:NumRegIdTrib no cumple con el patrón publicado en la columna \"Formato de registro de identidad tributaria\" del país indicado en el atributo cce11:ComercioExterior:Destinatario:Domicilio:Pais.");

                         }
                     }
                 }
             }
            CatalogosSAT.OperacionesCatalogos o7 = new CatalogosSAT.OperacionesCatalogos();

             if (comer.Destinatario != null)
             {
                 if (comer.Destinatario.Count > 0)
                 foreach (var des in comer.Destinatario[0].Domicilio)
                 {
                     if (des.Pais.ToString() == "MEX")
                     {
                         if (!string.IsNullOrEmpty(des.CodigoPostal) && !string.IsNullOrEmpty(des.Colonia))
                         {
                             CatalogosSAT.c_Colonia colonia7 = o7.Consultar_ColoniaMEX(des.CodigoPostal, des.Colonia);
                             if (colonia7 == null)
                                 return ("CCE191 - El atributo cce11:ComercioExterior:Destinatario:Domicilio:Colonia debe tener un valor del catálogo de colonias donde la columna código postal sea igual a la clave registrada en el atributo CodigoPostal cuando la clave de país es \"MEX\" y contiene una cadena numérica de cuatro posiciones.");
                         }
                         if (!string.IsNullOrEmpty(des.Localidad))
                         {
                             CatalogosSAT.c_Localidad localidad7 = o7.Consultar_LocalidadMEX(des.Estado, des.Localidad);
                             if (localidad7 == null)
                                 return ("CCE192 - El atributo cce11:ComercioExterior:Destinatario:Domicilio:Localidad debe tener un valor del catálogo de localidades (catCFDI:c_Localidad) donde la columna c_Estado sea igual a la clave registrada en el atributo Estado cuando la clave de país es \"MEX\".");
                         }
                         if (!string.IsNullOrEmpty(des.Municipio))
                         {

                             CatalogosSAT.c_Municipio municipio7 = o7.Consultar_MunicipioMEX(des.Estado, des.Municipio);
                             if (municipio7 == null)
                                 return ("CCE193 - El atributo cce11:ComercioExterior:Destinatario:Domicilio:Municipio debe tener un valor del catálogo de municipios (catCFDI:c_Municipio) donde la columna c_Estado sea igual a la clave registrada en el atributo Estado cuando la clave de país es \"MEX\".");
                         }
                     }
                     if (!string.IsNullOrEmpty(des.Estado) && des.Estado != "ZZZ")
                     {
                         CatalogosSAT.c_Estado estado777=  o7.Consultar_EstadosPais(des.Pais.ToString());
                         if (estado777 != null)
                         {
                             CatalogosSAT.c_Estado estado7 = o7.Consultar_EstadosPais(des.Estado, des.Pais.ToString());
                             if (estado7 == null)
                                 return ("CCE194 - El atributo cce11:ComercioExterior:Destinatario:Domicilio:Estado debe tener un valor del catálogo de estados catCFDI:c_Estado donde la columna c_Pais sea igual a la clave de país registrada en el atributo Pais cuando la clave de país existe en la columna c_Pais del catálogo catCFDI:c_Estado y es diferente de \"ZZZ\".");
                         }
                     }
                     if (!string.IsNullOrEmpty(des.CodigoPostal) && des.Pais.ToString() != "MEX")
                     {
                         CatalogosSAT.c_Pais pais77 = o7.Consultar_PaisVerificacionLinea(des.Pais.ToString());
                         if (pais77.FormatodeCP != null)
                         {
                             bool cpV = true;
                             string postal_code = des.CodigoPostal.Replace(" ", "");

                             if (pais77.c_Pais1 == "CAN" || pais77.c_Pais1 == "USA")
                                 cpV = IsUsorCanadianZipCode(postal_code, pais77.c_Pais1);
                             else
                             {

                                 Regex regex5 = new Regex(pais77.FormatodeCP);
                                 if (!regex5.IsMatch(postal_code))
                                     cpV = false;
                             }

                             if (cpV == false)
                                 //   Regex regex17 = new Regex(pais77.FormatodeCP);
                                 //   if (!regex17.IsMatch(des.CodigoPostal))
                                 return ("CCE195 - El atributo cce11:ComercioExterior:Destinatario:Domicilio:CodigoPostal debe cumplir con el patrón especificado para el país cuando es distinta de \"MEX\". ");
                         }

                     }
                         if (!string.IsNullOrEmpty(des.CodigoPostal.ToString()) && des.Pais.ToString() == "MEX")
                         {
                             CatalogosSAT.c_CP CP77 = o7.Consultar_CPMEX(des.Estado, des.Municipio, des.Localidad, des.CodigoPostal);
                             if (CP77 == null)
                                 return ("CCE196 - El atributo cce11:ComercioExterior:Destinatario:Domicilio:CodigoPostal debe tener un valor del catálogo de códigos postales catCFDI:c_CodigoPostal donde la columna c_Estado sea igual a la clave registrada en el atributo Estado, la columna c_Municipio sea igual a la clave registrada en el atributo Municipio y la columna c_Localidad sea igual a la clave registrada en el atributo Localidad en caso de que se haya registrado cuando la clave de país es \"MEX\".");
                         }
                     
                 }
             }
            

             foreach (var c in com.Conceptos)
             {
                 if (string.IsNullOrEmpty(c.NoIdentificacion))
                 {
                     return ("CCE197 - El atributo cfdi:Comprobante:Conceptos:Concepto:NoIdentificacion se debe registrar en cada concepto.");
        
                 }
             }
             if (comer.Mercancias != null)
             {
                 int contador = 0;
                 foreach (var c in com.Conceptos)
                 {
                     int s = 0;
                     foreach (var co in comer.Mercancias)
                     {
                         if (co.NoIdentificacion == c.NoIdentificacion)
                         {
                             contador++;
                             break;
                         }
                     }
                  
                 }
                 //if (contador != comer.Mercancias.Count())
                 if (contador == 0)
                  return ("CCE198 - Debe existir al menos un cfdi:Comprobante:Conceptos:Concepto:NoIdentificacion relacionado con cce11:ComercioExterior:Mercancias:Mercancia:NoIdentificacion.");

                 contador = 0;
                 foreach (var co in comer.Mercancias)
                 {
                     int s = 0;
                     foreach (var c in com.Conceptos)
                     {
                         if (co.NoIdentificacion == c.NoIdentificacion)
                         {
                             contador++;
                             break;
                         }
                     }
                 }
                 if (contador != comer.Mercancias.Count())
             //if (contador == 0)
                     return ("CCE199 - Debe existir al menos un concepto en el nodo cfdi:Comprobante:Conceptos por cada mercancía registrada en el elemento cce1:ComercioExterior:Mercancias donde el atributo cce11:ComercioExterior:Mercancias:Mercancia:NoIdentificacion sea igual al atributo cfdi:Comprobante:Conceptos:Concepto:NoIdentificacion.");


                 List<ComercioExteriorMercancia> Lista1 = new List<ComercioExteriorMercancia>();
                // string[] Lista1 = new string[comer.Mercancias.Count()];
               //  string[] Lista2 = new string[comer.Mercancias.Count()];
                 int i = 0;
                 foreach (var co in comer.Mercancias)
                 {
                     var itemExists = Lista1.Exists(element => element.NoIdentificacion == co.NoIdentificacion && element.FraccionArancelaria.ToString() == co.FraccionArancelaria.ToString());
                         if (!itemExists)
                         {
                             Lista1.Add(co);
                                             
                         }
                     else
                             return ("CCE200 - No se deben repetir elementos Mercancia donde el NoIdentificacion y la FraccionArancelaria sean iguales en el elemento cce11:ComercioExterior:Mercancias.");

                     
                     i++;
                 }

             }//mercancias

            //-----------

             bool mercanciaExiste = false;

             List<ComercioExteriorMercancia> Lista3 = new List<ComercioExteriorMercancia>();
             foreach (var co2 in comer.Mercancias)
             {
                 bool existe = Lista3.Exists(element => element.NoIdentificacion == co2.NoIdentificacion);
                 if (existe)
                 { mercanciaExiste = true; break; }
                 else
                 {
                     Lista3.Add(co2);
                 }

             }
             if (mercanciaExiste == false)
             {
                 List<ComprobanteConcepto> Lista4 = new List<ComprobanteConcepto>();
                 foreach (var co2 in com.Conceptos)
                 {
                     bool existe = Lista4.Exists(element => element.NoIdentificacion == co2.NoIdentificacion);
                     if (existe)
                     { mercanciaExiste = true; break; }
                     else
                     {
                         Lista4.Add(co2);
                     }


                 }


             }


             if (mercanciaExiste)
             {
                 foreach (var co2 in comer.Mercancias)
                 {
                     if (co2.UnidadAduanaSpecified == false || co2.ValorUnitarioAduanaSpecified == false || co2.CantidadAduanaSpecified == false)
                         return ("CCE213 - Los atributos CantidadAduana, UnidadAduana y ValorUnitarioAduana deben existir en los registros involucrados si se ha registrado alguno de ellos, si existe más de un concepto con el mismo NoIdentificacion o si existe más de una mercancía con el mismo NoIdentificacion.");

                 }

             }
             
            //-----------------
             CatalogosSAT.OperacionesCatalogos o9 = new CatalogosSAT.OperacionesCatalogos();
              CatalogosSAT.c_Moneda mone= o9.Consultar_Moneda(com.Moneda);

             foreach (var c in com.Conceptos)
             {
                 foreach (var co in comer.Mercancias)
                 {
                                          
                     if (c.NoIdentificacion == co.NoIdentificacion)
                     {
                         if (co.CantidadAduanaSpecified == false)
                         {
                             if (c.Cantidad == null)
                                 return ("CCE201 - El atributo cfdi:Comprobante:Conceptos:Concepto:Cantidad no cumple con alguno de los valores permitidos cuando no se registra el atributo cce11:ComercioExterior:Mercancias:Mercancia:CantidadAduana.");
                             if (c.Cantidad < 0.001M)
                                 return ("CCE201 - El atributo cfdi:Comprobante:Conceptos:Concepto:Cantidad no cumple con alguno de los valores permitidos cuando no se registra el atributo cce11:ComercioExterior:Mercancias:Mercancia:CantidadAduana.");

                             Regex regex2 = new Regex(@"^\d{1,14}(.(\d{1,3}))?$");
                             if (!regex2.IsMatch(c.Cantidad.ToString()))
                                 return ("CCE201 - El atributo cfdi:Comprobante:Conceptos:Concepto:Cantidad no cumple con alguno de los valores permitidos cuando no se registra el atributo cce11:ComercioExterior:Mercancias:Mercancia:CantidadAduana.");

                             CatalogosSAT.OperacionesCatalogos o17 = new CatalogosSAT.OperacionesCatalogos();
                             CatalogosSAT.UnidadMedida UM = o17.Consultar_UnidadMedida(c.Unidad.ToUpper());
                            
                            // c_UnidadAduana myTipoAduana;
                            // Enum.TryParse("Item" + c.unidad, out myTipoAduana);
                            // if (myTipoAduana.ToString() != "Item" + c.unidad)
                             if(UM==null)
                                 return ("CCE202 - El atributo cfdi:Comprobante:Conceptos:Concepto:Unidad no cumple con alguno de los valores permitidos cuando no se registra el atributo cce11:ComercioExterior:Mercancias:Mercancia:CantidadAduana.");
                             if (c.ValorUnitario == null)
                                 return ("CCE203 - El atributo cfdi:Comprobante:Conceptos:Concepto:ValorUnitario no cumple con alguno de los valores permitidos cuando no se registra el atributo cce11:ComercioExterior:Mercancias:Mercancia:CantidadAduana.");
                             if (c.ValorUnitario < 0.0001M)
                                 return ("CCE203 - El atributo cfdi:Comprobante:Conceptos:Concepto:ValorUnitario no cumple con alguno de los valores permitidos cuando no se registra el atributo cce11:ComercioExterior:Mercancias:Mercancia:CantidadAduana.");
                             Regex regex3 = new Regex(@"^\d{1,16}(.(\d{1,4}))?$");
                             if (!regex3.IsMatch(c.ValorUnitario.ToString()))
                                 return ("CCE203 - El atributo cfdi:Comprobante:Conceptos:Concepto:ValorUnitario no cumple con alguno de los valores permitidos cuando no se registra el atributo cce11:ComercioExterior:Mercancias:Mercancia:CantidadAduana.");

                         }

                         if (co.UnidadAduana.ToString() != "Item99" || c.Unidad != "Servicio")
                         {
                             if (co.FraccionArancelariaSpecified == false)
                                 return ("CCE206 - El atributo cce11:ComercioExterior:Mercancias:Mercancia:FraccionArancelaria debe registrarse cuando el atributo cce11:ComercioExterior:Mercancias:Mercancia:UnidadAduana o el atributo cfdi:Comprobante:Conceptos:Concepto:Unidad tienen un valor distinto de \"99\".");

                         }
                         else
                         {
                             if (co.FraccionArancelariaSpecified == true)
                                 return ("CCE207 - El atributo cce11:ComercioExterior:Mercancias:Mercancia:FraccionArancelaria no debe registrarse cuando el atributo cce11:ComercioExterior:Mercancias:Mercancia:UnidadAduana o el atributo cfdi:Comprobante:Conceptos:Concepto:Unidad tienen el valor \"99\".");
                         }
                         if (co.UnidadAduanaSpecified == false)
                         {
                             CatalogosSAT.OperacionesCatalogos o13 = new CatalogosSAT.OperacionesCatalogos();
                             CatalogosSAT.UnidadMedida UM = o13.Consultar_UnidadMedida(c.Unidad.ToUpper());
                             CatalogosSAT.c_FraccionArancelaria f = o13.Consultar_FraccionArancelaria(co.FraccionArancelaria.ToString().Replace("Item", ""));
                             if(UM==null)
                                 return ("CCE210 - El atributo cfdi:Comprobante:Conceptos:Concepto:Unidad del concepto relacionado a la mercncía debe tener el valor especificado en el catálogo catCFDI:c_FraccionArancelaria columna \"UMT\" cuando el atributo cce11:ComercioExterior:Mercancias:Mercancia:FraccionArancelaria está registrado.");
                             if(f==null)
                                 return ("CCE210 - El atributo cfdi:Comprobante:Conceptos:Concepto:Unidad del concepto relacionado a la mercncía debe tener el valor especificado en el catálogo catCFDI:c_FraccionArancelaria columna \"UMT\" cuando el atributo cce11:ComercioExterior:Mercancias:Mercancia:FraccionArancelaria está registrado.");
                    
                             if(Convert.ToInt16(UM.C_UnidadMedida)!=f.Unidad_de_Medida)
                                 return ("CCE210 - El atributo cfdi:Comprobante:Conceptos:Concepto:Unidad del concepto relacionado a la mercncía debe tener el valor especificado en el catálogo catCFDI:c_FraccionArancelaria columna \"UMT\" cuando el atributo cce11:ComercioExterior:Mercancias:Mercancia:FraccionArancelaria está registrado.");
                  
                         }



                     }
                 }

                 decimal limiteInferior = CalculoInferiorConcepto(c.Cantidad, c.ValorUnitario, Convert.ToInt16(mone.Decimales));
                 decimal limiteSupeior = CalculoSuperiorValorConcepto(c.Cantidad, c.ValorUnitario, Convert.ToInt16(mone.Decimales));
                 if (c.Importe < limiteInferior)
                     return ("CCE204 - El atributo cfdi:Comprobante:Conceptos:Concepto:importe debe ser mayor o igual que el límite inferior y menor o igual que el límite superior calculado.");
                 if (c.Importe > limiteSupeior)
                     return ("CCE204 - El atributo cfdi:Comprobante:Conceptos:Concepto:importe debe ser mayor o igual que el límite inferior y menor o igual que el límite superior calculado.");
              
              
             }
            //-------------
             bool mercanciaExiste2 = false;
             mercanciaExiste2 = comer.Mercancias.Exists(element => element.UnidadAduanaSpecified == true
                || element.ValorUnitarioAduanaSpecified == true
                || element.CantidadAduanaSpecified == true);

             if (mercanciaExiste2)
             {
                 foreach (var co2 in comer.Mercancias)
                 {
                     if (co2.UnidadAduanaSpecified == false || co2.ValorUnitarioAduanaSpecified == false || co2.CantidadAduanaSpecified == false)
                         return ("CCE214 - Los atributos CantidadAduana, UnidadAduana y ValorUnitarioAduana deben registrarse en todos los elementos mercancía del comprobante, siempre que uno de ellos los tenga registrados.");

                 }

             }
            //-------------------------------------------
            
           
            //--------------------------
            List<ComercioExteriorMercancia> Lista2 = new List<ComercioExteriorMercancia>();

             foreach (var co2 in comer.Mercancias)
             {
                 foreach (var c in com.Conceptos)
                 {
                     if (c.NoIdentificacion == co2.NoIdentificacion)
                     {
                         if (co2.ValorDolares != 0 && co2.ValorDolares != 1)
                         {
                             if (Lista2 == null)
                                 Lista2.Add(co2);
                             else
                             {
                                 int existe = 0;
                                 foreach (var l in Lista2)
                                 {

                                     if (l.NoIdentificacion == co2.NoIdentificacion)
                                     {
                                         l.ValorDolares = l.ValorDolares + co2.ValorDolares;
                                         l.CantidadAduana = l.CantidadAduana + co2.CantidadAduana;
                                         l.ValorUnitarioAduana = l.ValorUnitarioAduana + co2.ValorUnitarioAduana;
                                         existe = 1;
                                         break;
                                     }
                                 }

                                 if (existe == 0)
                                     Lista2.Add(co2);
                             }
                             break;

                         }
                     }
                 }
             }

                 if(Lista2!=null)
                 {
                   foreach(var l in Lista2)
                   {
                   decimal limiteInferior = CalculoInferiorValorDolares(l.CantidadAduana, l.ValorUnitarioAduana);
                   decimal limiteSupeior = CalculoSupeiorValorDolares(l.CantidadAduana, l.ValorUnitarioAduana);
                   if (l.ValorDolares < limiteInferior)
                    return ("CCE205 - La suma de los campos cce11:ComercioExterior:Mercancias:Mercancia:ValorDolares distintos de \"0\" y \"1\" de todas las mercancías que tengan el mismo NoIdentificacion y éste sea igual al NoIdentificacion del concepto debe ser mayor o igual al valor mínimo y menor o igual al valor máximo calculado.");
                   if (l.ValorDolares> limiteSupeior)
                    return ("CCE205 - La suma de los campos cce11:ComercioExterior:Mercancias:Mercancia:ValorDolares distintos de \"0\" y \"1\" de todas las mercancías que tengan el mismo NoIdentificacion y éste sea igual al NoIdentificacion del concepto debe ser mayor o igual al valor mínimo y menor o igual al valor máximo calculado.");
                   }
                 }
            //-------------
                 decimal SumaValorDolares = 0;
             foreach (var co in comer.Mercancias)
             {
               

                 if (co.FraccionArancelariaSpecified == true)
                 {
                     CatalogosSAT.OperacionesCatalogos o12 = new CatalogosSAT.OperacionesCatalogos();
                     CatalogosSAT.c_FraccionArancelaria f = o12.Consultar_FraccionArancelaria(co.FraccionArancelaria.ToString().Replace("Item", ""));
                    if(f!=null)
                    {
                     if(f.Fecha_de_fin_de_vigencia!=null)
                     {
                         if (Convert.ToDateTime(com.Fecha) >= f.Fecha_de_fin_de_vigencia)
                         {
                                return ("CCE208 - El atributo cce11:ComercioExterior:Mercancias:Mercancia:FraccionArancelaria debe tener un valor vigente del catálogo catCFDI:c_FraccionArancelaria.");
                         }
                     }

                     if (f.Fecha_de_inicio_de_vigencia != null)
                     {
                         if (Convert.ToDateTime(com.Fecha) <= f.Fecha_de_inicio_de_vigencia)
                             return ("CCE208 - El atributo cce11:ComercioExterior:Mercancias:Mercancia:FraccionArancelaria debe tener un valor vigente del catálogo catCFDI:c_FraccionArancelaria.");
                                                   
                     }
                    }

                 }
                 if (co.UnidadAduanaSpecified == true)
                 {
                     CatalogosSAT.OperacionesCatalogos o13 = new CatalogosSAT.OperacionesCatalogos();
                     CatalogosSAT.c_FraccionArancelaria f2 = o13.Consultar_FraccionArancelaria(co.FraccionArancelaria.ToString().Replace("Item", "").ToString());
                      if(f2==null)
                       return ("CCE209 - El atributo cce11:ComercioExterior:Mercancias:Mercancia:UnidadAduana debe tener el valor especificado en el catálogo catCFDI:c_FraccionArancelaria columna \"UMT\" cuando el atributo cce11:ComercioExterior:Mercancias:Mercancia:FraccionArancelaria está registrado.");
                      if (string.IsNullOrEmpty(f2.UMT))
                          f2.UMT = "00";
                    
                      string unidadMedida = f2.UMT.ToString();
                      if (f2.UMT.ToString().Count() < 2)
                          unidadMedida = "0" + unidadMedida;
                     if("Item"+unidadMedida!=co.UnidadAduana.ToString())
                       return ("CCE209 - El atributo cce11:ComercioExterior:Mercancias:Mercancia:UnidadAduana debe tener el valor especificado en el catálogo catCFDI:c_FraccionArancelaria columna \"UMT\" cuando el atributo cce11:ComercioExterior:Mercancias:Mercancia:FraccionArancelaria está registrado.");
            
                 
                 }
                 if (com.Version == "3.2")
                 {
                     if (co.FraccionArancelaria.ToString() == "9801000100")
                     {
                         SumaValorDolares = SumaValorDolares + co.ValorDolares;
                     }
                 }
                 
                 
                 
                  if (co.UnidadAduana.ToString() != "99" )
                   {
                     if(co.ValorUnitarioAduana<=0)
                      return ("CCE215 - El atributo cce11:ComercioExterior:Mercancias:Mercancia:ValorUnitarioAduana debe ser mayor que \"0\" cuando  cce11:ComercioExterior:Mercancias:Mercancia:UnidadAduana es distinto de \"99\".");
    
                   }


                 
             }// leer las mercancias
       


            if (com.Version == "3.2")
               if (SumaValorDolares > 0)
             { decimal descuento=0;
             if (com.DescuentoSpecified == true && com.TipoCambioSpecified==true)
             {
                 if (com.Moneda == "MEX")
                     descuento = com.Descuento;
                 else
                     descuento = com.Descuento * Convert.ToDecimal(com.TipoCambio);
             }
             else
             {
                 if (com.TipoCambioSpecified==false)
                     descuento = com.Descuento;
             }
                 if(comer.TipoCambioUSDSpecified==true)
                 SumaValorDolares = SumaValorDolares * comer.TipoCambioUSD;
                 
                 if(descuento<SumaValorDolares)
                   return ("CCE211 - El atributo cfdi:Comprobante:descuento debe ser mayor o igual que la suma de los atributos cce11:ComercioExterior:Mercancias:Mercancia:ValorDolares de todas las mercancías que tengan la fracción arancelaria \"98010001\" convertida a la moneda del comprobante si la versión del CFDI es 3.2.");
    

             }

            if (com.Version == "3.3")
            {
                decimal descuento = 0;
             
                decimal SumaValorDolares3 = 0;
                foreach (var co2 in comer.Mercancias)
                {

                    foreach (var c in com.Conceptos)
                    {
                        if (c.NoIdentificacion == co2.NoIdentificacion && co2.FraccionArancelaria.ToString() == "9801000100")
                        {
                            SumaValorDolares3 = SumaValorDolares3 + co2.ValorDolares;
                            descuento = descuento + c.Descuento;
                        }
                    }
                }

                /*
                decimal descuento = 0;
                if (com.DescuentoSpecified == true && com.TipoCambioSpecified==true)
                {
                    if (com.Moneda == "MEX")
                        descuento = com.Descuento;
                    else
                        descuento = com.Descuento * Convert.ToDecimal(com.TipoCambio);
                }
                if (comer.TipoCambioUSDSpecified == true)
                    SumaValorDolares3 = SumaValorDolares3 * comer.TipoCambioUSD;
                */
                if (descuento < SumaValorDolares3)
                    return ("CCE212 - La suma de los valores de cfdi:Comprobante:Conceptos:Concepto:Descuento donde el NoIdentificacion es el mismo que el de la mercancía convertida a la moneda del comprobante debe ser mayor o igual que la suma de los valores de cce11:ComercioExterior:Mercancias:Mercancia:ValorDolares de todas las mercancías que tengan la fracción arancelaria \"98010001\" y el NoIdentificacion sea igual al NoIdentificacion del concepto si la versión del CFDI es 3.3.");
    

            }
             

              //-----------------------------------
             List<ComercioExteriorMercancia> Lista5 = new List<ComercioExteriorMercancia>();
             List<string> Lista6 = new List<string>();  
            foreach (var co2 in comer.Mercancias)
             {
                 bool existe = Lista5.Exists(element => element.NoIdentificacion == co2.NoIdentificacion);
                 if (existe)
                 { Lista6.Add(co2.NoIdentificacion); }
                 else
                 {
                     Lista5.Add(co2);
                 }

             }
             List<ComprobanteConcepto> Lista7 = new List<ComprobanteConcepto>();
             List<string> Lista8 = new List<string>();  
          
              foreach (var co2 in com.Conceptos)
                 {
                     bool existe = Lista7.Exists(element => element.NoIdentificacion == co2.NoIdentificacion);
                     if (existe)
                     { Lista8.Add(co2.NoIdentificacion); }
                     else
                     {
                         Lista7.Add(co2);
                     }


                 }


              foreach (var co2 in comer.Mercancias)
              {
                  if (co2.CantidadAduanaSpecified == true)
                  {
                      decimal limiteInferior = CalculoInferiorValorDolares(co2.CantidadAduana, co2.ValorUnitarioAduana);
                      decimal limiteSupeior = CalculoSupeiorValorDolares(co2.CantidadAduana, co2.ValorUnitarioAduana);
                      if (co2.ValorDolares < limiteInferior)
                          return ("CCE216 - El atributo cce11:ComercioExterior:Mercancias:ValorDolares de cada mercancía registrada debe ser mayor o igual que el límite inferior y menor o igual que el límtie superior o uno, cuando la normatividad lo permita y exista el atributo cce11:ComercioExterior:Mercancias:Mercancia:CantidadAduana.");
                      if (co2.ValorDolares > limiteSupeior)
                          return ("CCE216 - El atributo cce11:ComercioExterior:Mercancias:ValorDolares de cada mercancía registrada debe ser mayor o igual que el límite inferior y menor o igual que el límtie superior o uno, cuando la normatividad lo permita y exista el atributo cce11:ComercioExterior:Mercancias:Mercancia:CantidadAduana.");


                  }
              }

             foreach (var co2 in comer.Mercancias)
             {
                 foreach (var c in com.Conceptos)
                 {

                     if (c.NoIdentificacion == co2.NoIdentificacion)
                     {
                         bool existe1 = Lista6.ToList().Contains(co2.NoIdentificacion);
                         bool existe2 = Lista8.ToList().Contains(co2.NoIdentificacion);
                         if (existe1 == false && existe2 == false)
                         {
                             if (co2.CantidadAduanaSpecified == false)
                             {
                                 if (co2.ValorDolares != 1)
                                     return ("CCE217 - El atributo cce11:ComercioExterior:Mercancias:ValorDolares de cada mercancía registrada debe ser igual al producto del valor del atributo cfdi:Comprobante:Conceptos:Concepto:Importe por el valor del atributo cfdi:Comprobante:TipoCambio y dividido entre el valor del atributo cce11:ComercioExterior:TipoDeCambioUSD donde el atributo cfdi:Comprobante:Conceptos:NoIdentificacion es igual al atributo cce11:ComercioExterior:Mercancias:Mercancia:NoIdentificacion, \"0\" cuando el atributo cce11:ComercioExterior:Mercancias:Mercancia:UnidadAduana o el atributo cfdi:Comprobante:Conceptos:Concepto:Unidad tienen el valor \"99\", o \"1\", cuando la normatividad lo permita y no debe existir el atributo cce11:ComercioExterior:Mercancias:Mercancia:CantidadAduana.");
                             }
                             else
                                 if (co2.UnidadAduana.ToString() == "Item99" || c.Unidad == "Servicio")
                                 {
                                     if (co2.ValorDolares != 0)
                                         return ("CCE217 - El atributo cce11:ComercioExterior:Mercancias:ValorDolares de cada mercancía registrada debe ser igual al producto del valor del atributo cfdi:Comprobante:Conceptos:Concepto:Importe por el valor del atributo cfdi:Comprobante:TipoCambio y dividido entre el valor del atributo cce11:ComercioExterior:TipoDeCambioUSD donde el atributo cfdi:Comprobante:Conceptos:NoIdentificacion es igual al atributo cce11:ComercioExterior:Mercancias:Mercancia:NoIdentificacion, \"0\" cuando el atributo cce11:ComercioExterior:Mercancias:Mercancia:UnidadAduana o el atributo cfdi:Comprobante:Conceptos:Concepto:Unidad tienen el valor \"99\", o \"1\", cuando la normatividad lo permita y no debe existir el atributo cce11:ComercioExterior:Mercancias:Mercancia:CantidadAduana.");

                                 }
                                 else
                                 {
                                     decimal ValorDolares = (c.Importe * Convert.ToDecimal(com.TipoCambio)) / comer.TipoCambioUSD;
                                     ValorDolares = Decimal.Round(ValorDolares, 2);

                                     if (co2.ValorDolares != ValorDolares)
                                         return ("CCE217 - El atributo cce11:ComercioExterior:Mercancias:ValorDolares de cada mercancía registrada debe ser igual al producto del valor del atributo cfdi:Comprobante:Conceptos:Concepto:Importe por el valor del atributo cfdi:Comprobante:TipoCambio y dividido entre el valor del atributo cce11:ComercioExterior:TipoDeCambioUSD donde el atributo cfdi:Comprobante:Conceptos:NoIdentificacion es igual al atributo cce11:ComercioExterior:Mercancias:Mercancia:NoIdentificacion, \"0\" cuando el atributo cce11:ComercioExterior:Mercancias:Mercancia:UnidadAduana o el atributo cfdi:Comprobante:Conceptos:Concepto:Unidad tienen el valor \"99\", o \"1\", cuando la normatividad lo permita y no debe existir el atributo cce11:ComercioExterior:Mercancias:Mercancia:CantidadAduana.");
                                 }
                         }
                     
                     }
                 }
             }
            
            return "0";
        }

       //----------------------------------------------------------------------------------------
       //----------------------------------------------------------------------------------------
        public ComercioExterior  DesSerializarComercioExterior(XElement element, ref string erroresNom)
        {

            //XName nomina = XName.Get("Conceptos");
            // IEnumerable<XNode> dnas = from node in element.DescendantNodes() select node;

            try
            {

                var nomina12 = element.Elements(_ns + "Complemento");
                if (nomina12 != null)
                {
                    string comerci = nomina12.First().ToString();
                    if (comerci == "<cfdi:Complemento xmlns:cfdi=\"http://www.sat.gob.mx/cfd/3\" />")
                    {
                        erroresNom = "CCE154 - El nodo cce11:ComercioExterior debe registrarse como un nodo hijo del nodo Complemento en el CFDI.";
                        return null;
                    }
                    nomina12 = nomina12.Elements(_ns2 + "ComercioExterior");
                    if (nomina12 != null)
                    {
                        IEnumerable<XAttribute> version = nomina12.Attributes("Version");
                        foreach (XAttribute att in version)
                        {

                           // if (att.Value == "3.2")
                            {
                                if (nomina12.Count() > 1)
                                {
                                    erroresNom = "CCE153 - El nodo cce11:ComercioExterior no puede registrarse mas de una vez.";
                                    return null;
                                }
                                foreach (XElement e in nomina12)
                                {
                                    var ser = new XmlSerializer(typeof(ComercioExterior));
                                    string xml = e.ToString();
                                    // xml = xml.Replace("xmlns:nomina12=\"http://www.sat.gob.mx/nomina12\"", "");
                                    var reader = new StringReader(xml);
                                    var comLXMLComprobante = (ComercioExterior)ser.Deserialize(reader);
                                    return comLXMLComprobante;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                erroresNom = "CCE218 - " + ex.InnerException.Message;
                return null;
            }
            return null;

        }

    
        private bool IsUsorCanadianZipCode(string zipCode ,string pais)
        {
            string _usZipRegEx = @"^\d{5}(?:[-\s]\d{4})?$";
            string _caZipRegEx = @"^([ABCEGHJKLMNPRSTVXY]\d[ABCEGHJKLMNPRSTVWXYZ])\ {0,1}(\d[ABCEGHJKLMNPRSTVWXYZ]\d)$";

            string _ZipRegEx = "";
            if (pais == "CAN")
                _ZipRegEx = _caZipRegEx;
            if (pais == "USA")
                _ZipRegEx = _usZipRegEx;
       
            bool validZipCode = true;
            if ((!Regex.Match(zipCode, _ZipRegEx).Success))// && (!Regex.Match(zipCode, _caZipRegEx).Success))
            {
                validZipCode = false;
            }
            return validZipCode;
        }

       //--------------------------------------
        private decimal CalculoInferiorValorDolares(decimal CantidadAduana, decimal ValorUnitarioAduana)
        {

           int NumDecimalesCantidad=0;
           int NumDecimalesValorUnitario=0;
            //--------------
            string NumDecimalesCantidadString = CantidadAduana.ToString();
             if (NumDecimalesCantidadString != null)
             {
                 string[] split = NumDecimalesCantidadString.Split(".".ToCharArray());
                 if (split.Count() > 1)
                 {
                   NumDecimalesCantidad=  split[1].Count();
                 }
             }
            //----------------
            string NumDecimalesValorUnitarioString = ValorUnitarioAduana.ToString();
             if (NumDecimalesValorUnitarioString != null)
             {
                 string[] split2 = NumDecimalesValorUnitarioString.Split(".".ToCharArray());
                 if (split2.Count() > 1)
                 {
                   NumDecimalesValorUnitario=  split2[1].Count();
                 }
             }
            //----------------
             decimal x1 =(decimal)(Math.Pow(10, -NumDecimalesCantidad));
             decimal x2 = (decimal)(Math.Pow(10, -NumDecimalesValorUnitario));

             decimal resultado = (CantidadAduana - (x1 / 2)) * (ValorUnitarioAduana - (x2 / 2));
            resultado=Truncate(resultado,2);
            return resultado;
        }
         //-----------------------------------------

        private decimal CalculoSupeiorValorDolares(decimal CantidadAduana, decimal ValorUnitarioAduana)
        {

           int NumDecimalesCantidad=0;
           int NumDecimalesValorUnitario=0;
            //--------------
            string NumDecimalesCantidadString = CantidadAduana.ToString();
             if (NumDecimalesCantidadString != null)
             {
                 string[] split = NumDecimalesCantidadString.Split(".".ToCharArray());
                 if (split.Count() > 1)
                 {
                   NumDecimalesCantidad=  split[1].Count();
                 }
             }
            //----------------
            string NumDecimalesValorUnitarioString = ValorUnitarioAduana.ToString();
             if (NumDecimalesValorUnitarioString != null)
             {
                 string[] split2 = NumDecimalesValorUnitarioString.Split(".".ToCharArray());
                 if (split2.Count() > 1)
                 {
                   NumDecimalesValorUnitario=  split2[1].Count();
                 }
             }
            //----------------
             int decimales = 0;
             decimal x1 = (decimal)(Math.Pow(10, -NumDecimalesCantidad));
             decimal x2 = (decimal)(Math.Pow(10, -NumDecimalesValorUnitario));
             decimal x3 = (decimal)(Math.Pow(10, -12));


             decimal resultado = (CantidadAduana + ((x1 / 2) - x3)) * (ValorUnitarioAduana + ((x2 / 2) - x3));
             resultado=Truncate(resultado, 2);
             string resultadoString = resultado.ToString();
             if (resultadoString != null)
             {
                 string[] split3 = resultadoString.Split(".".ToCharArray());
                 if (split3.Count() > 1)
                 {
                     decimales = Convert.ToInt32(split3[1]);
                     decimales = decimales + 1;
                     resultadoString = split3[0] + "." + decimales;
                     resultado = Convert.ToDecimal(resultadoString);
                 }
                 else
                 {
                     resultadoString = AgregaCeros(2);
                     resultadoString = split3[0] + "." + resultadoString;
                     resultado = Convert.ToDecimal(resultadoString);
            
                 }
             }
            
           //  if (decimales > 0)
           //      decimales = Math.Ceiling(decimales);

            return resultado;
        }
        //-----------------------------------------
        public decimal Truncate(decimal number, int digits)
        {
            decimal stepper = (decimal)(Math.Pow(10.0, (double)digits));
            int temp = (int)(stepper * number);
            return (decimal)temp / stepper;
        }
        //--------------------------------------
        private decimal CalculoInferiorMercancia(decimal importe, decimal TipoCambio, decimal TipoCambioUSD)
        {

            int NumDecimalesImporte = 0;
            int NumDecimalesTipoCambio = 0;
            //--------------
            string NumDecimalesImporteString = importe.ToString();
            if (NumDecimalesImporteString != null)
            {
                string[] split = NumDecimalesImporteString.Split(".".ToCharArray());
                if (split.Count() > 1)
                {
                    NumDecimalesImporte = split[1].Count();
                }
            }
            //----------------
            string NumDecimalesTipoCambioString = TipoCambio.ToString();
            if (NumDecimalesTipoCambioString != null)
            {
                string[] split2 = NumDecimalesTipoCambioString.Split(".".ToCharArray());
                if (split2.Count() > 1)
                {
                    NumDecimalesTipoCambio = split2[1].Count();
                }
            }
            //----------------
            decimal x1 = (decimal)(Math.Pow(10, -NumDecimalesImporte));
            decimal x2 = (decimal)(Math.Pow(10, -NumDecimalesTipoCambio));
            decimal x3 = (decimal)(Math.Pow(10, -12));



            decimal resultado = (importe - (x1 / 2)) * (TipoCambio - (x2 / 2)) / (TipoCambioUSD + ((x2 / 2) - x3));
            resultado = Truncate(resultado, 2);
            return resultado;
        }
        //-----------------------------------------
        private decimal CalculoSuperiorValorMercancia(decimal importe, decimal TipoCambio, decimal TipoCambioUSD)
        {

            int NumDecimalesImporte = 0;
            int NumDecimalesTipoCambio = 0;
            //--------------
            string NumDecimalesImporteString = importe.ToString();
            if (NumDecimalesImporteString != null)
            {
                string[] split = NumDecimalesImporteString.Split(".".ToCharArray());
                if (split.Count() > 1)
                {
                    NumDecimalesImporte = split[1].Count();
                }
            }
            //----------------
            string NumDecimalesTipoCambioString = TipoCambio.ToString();
            if (NumDecimalesTipoCambioString != null)
            {
                string[] split2 = NumDecimalesTipoCambioString.Split(".".ToCharArray());
                if (split2.Count() > 1)
                {
                    NumDecimalesTipoCambio = split2[1].Count();
                }
            }
            //----------------
            int decimales = 0;
            decimal x1 = (decimal)(Math.Pow(10, -NumDecimalesImporte));
            decimal x2 = (decimal)(Math.Pow(10, -NumDecimalesTipoCambio));
            decimal x3 = (decimal)(Math.Pow(10, -12));

            decimal resultado = (importe + ((x1 / 2) -x3)) * (TipoCambio + ((x2 / 2) -x3)) / (TipoCambioUSD - (x2 / 2));
           
            resultado = Truncate(resultado, 2);
            string resultadoString = resultado.ToString();
            if (resultadoString != null)
            {
                string[] split3 = resultadoString.Split(".".ToCharArray());
                if (split3.Count() > 1)
                {
                    decimales = Convert.ToInt32(split3[1]);
                    decimales = decimales + 1;
                    resultadoString = split3[0] + "." + decimales;
                    resultado = Convert.ToDecimal(resultadoString);
                }
                else
                {
                    resultadoString = AgregaCeros(2);
                    resultadoString = split3[0] + "." + resultadoString;
                    resultado = Convert.ToDecimal(resultadoString);
            
                }
            }

            //  if (decimales > 0)
            //      decimales = Math.Ceiling(decimales);

            return resultado;
        }
        //-----------------------------------------
        //--------------------------------------
        private decimal CalculoInferiorConcepto(decimal Cantidad, decimal valorUnitario, int moneda)
        {

            int NumDecimalesCantidad = 0;
            int NumDecimalesValorUnitario = 0;
            //--------------
            string NumDecimalesCantidadString = Cantidad.ToString();
            if (NumDecimalesCantidadString != null)
            {
                string[] split = NumDecimalesCantidadString.Split(".".ToCharArray());
                if (split.Count() > 1)
                {
                    NumDecimalesCantidad = split[1].Count();
                }
            }
            //----------------
            string NumDecimalesValorUnitarioString = valorUnitario.ToString();
            if (NumDecimalesValorUnitarioString != null)
            {
                string[] split2 = NumDecimalesValorUnitarioString.Split(".".ToCharArray());
                if (split2.Count() > 1)
                {
                    NumDecimalesValorUnitario = split2[1].Count();
                }
            }
            //----------------
            decimal x1 = (decimal)(Math.Pow(10, -NumDecimalesCantidad));
            decimal x2 = (decimal)(Math.Pow(10, -NumDecimalesValorUnitario));
            decimal x3 = (decimal)(Math.Pow(10, -12));


            decimal resultado = (Cantidad - (x1 / 2)) * (valorUnitario - (x2 / 2));
            resultado = Truncate(resultado, moneda);
            return resultado;
        }
        //-----------------------------------------
        private decimal CalculoSuperiorValorConcepto(decimal Cantidad, decimal valorUnitario, int moneda)
        {

            int NumDecimalesCantidad = 0;
            int NumDecimalesValorUnitario = 0;
            //--------------
            string NumDecimalesCantidadString = Cantidad.ToString();
            if (NumDecimalesCantidadString != null)
            {
                string[] split = NumDecimalesCantidadString.Split(".".ToCharArray());
                if (split.Count() > 1)
                {
                    NumDecimalesCantidad = split[1].Count();
                }
            }
            //----------------
            string NumDecimalesValorUnitarioString = valorUnitario.ToString();
            if (NumDecimalesValorUnitarioString != null)
            {
                string[] split2 = NumDecimalesValorUnitarioString.Split(".".ToCharArray());
                if (split2.Count() > 1)
                {
                    NumDecimalesValorUnitario = split2[1].Count();
                }
            }
            //----------------
            int decimales = 0;
            decimal x1 = (decimal)(Math.Pow(10, -NumDecimalesCantidad));
            decimal x2 = (decimal)(Math.Pow(10, -NumDecimalesValorUnitario));
            decimal x3 = (decimal)(Math.Pow(10, -12));

            decimal resultado = (Cantidad + ((x1 / 2) - x3)) * (valorUnitario + ((x2 / 2) - x3)); 
            
            resultado = Truncate(resultado, moneda);
            
            string resultadoString = resultado.ToString();
            if (resultadoString != null)
            {
                string[] split3 = resultadoString.Split(".".ToCharArray());
                if (split3.Count() > 1)
                {
                    decimales = Convert.ToInt32(split3[1]);
                    decimales = decimales + 1;
                    resultadoString = split3[0] + "." + decimales;
                    resultado = Convert.ToDecimal(resultadoString);
                }
                else
                {
                    resultadoString = AgregaCeros( moneda);
                    resultadoString = split3[0] + "." + resultadoString;
                    resultado = Convert.ToDecimal(resultadoString);
             
                }
            }

            //  if (decimales > 0)
            //      decimales = Math.Ceiling(decimales);

            return resultado;
        }
        //-----------------------------------------
       private string AgregaCeros(int num)
       {
           string c = "";
           int i=0;
           while (i < num)
           {
               if(i==num-1)
                   c = c + "1";
               else
               c = c + "0";
               i++;
           }
           return c;
       }
    }
}
