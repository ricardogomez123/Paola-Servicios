

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
using System.Text.RegularExpressions;
using ServicioLocal.catCFDI;
using System.Security.Cryptography.X509Certificates;
using I_RFC_SAT;
using System.Security.Cryptography;
using Org.BouncyCastle.X509;
using ServicioLocalContract;
using CatalogosSAT;
//using ServicioLocal.Business.Carta;

namespace ServicioLocal.Business.Carta
{
    public class ValidarCartaP : NtLinkBusiness
    {
        static bool exi;

        public ValidarCartaP()
        {
            exi = false;
            XmlConfigurator.Configure();
        }
        //---------------------------------------------------------------------------------------------
        public string ProcesarCarta(CartaPorte carta, Comprobante com)
        {
            try
            {
                if (com.Version != "3.3")
                    return ("CP101 - El valor registrado en este atributo es diferente a \"3.3\"");
                if (carta.Mercancias == null)
                    return ("CP164 - El nodo \"Mercancias\" debe contener por lo menos un nodo \"Mercancias:AutortransporteFederal\", \"Mercancias:TransporteMaritimo\", \"Mercancias:TransporteAereo\" o \"Mercancias:TransporteFerroviario\".");
                 if(carta.Mercancias.TransporteFerroviario==null &&carta.Mercancias.TransporteMaritimo==null &&carta.Mercancias.TransporteAereo==null&& carta.Mercancias.AutotransporteFederal==null)
                     return ("CP164 - El nodo \"Mercancias\" debe contener por lo menos un nodo \"Mercancias:AutortransporteFederal\", \"Mercancias:TransporteMaritimo\", \"Mercancias:TransporteAereo\" o \"Mercancias:TransporteFerroviario\".");
         
                    if (com.TipoDeComprobante == "T")
                {
                    if (Convert.ToDecimal(com.SubTotal) != 0)
                        return ("CP102 - El valor registrado en el atributo \"Subtotal\" es diferente de cero o el valor del atributo \"TipoDeComprobante\" es diferente de \"T\".");
                    if (com.Moneda != "XXX")
                        return ("CP103 - El valor registrado en el atributo \"Moneda\" es diferente de \"XXX\".");
                    if (com.Total != 0)
                        return ("CP105 - El valor registrado en el atributo \"Total\" es diferente de cero o el valor del atributo \"TipoDeComprobante\" es diferente de \"T\".");

                    if (com.Receptor.Rfc != "XAXX010101000")
                    {
                        Operaciones_IRFC r = new Operaciones_IRFC();
                        vI_RFC t = r.Consultar_IRFC(com.Receptor.Rfc);
                        if (t == null)
                        {
                            return "CP107 - El valor registrado en el atributo \"RFC\" no es \"XAXX010101000\" o bien no existe en la lista de RFC inscritos no cancelados en el SAT (l_RFC).";
                        }
                    }

                    if(com.Receptor.UsoCFDI!="P01")
                        return "CP109 - El valor registrado en el atributo \"UsoCFDI\", es diferente de \"P01\" (Por definir).";
        
                }
                if (com.CfdiRelacionados != null)
                {
                    exi = false;
                   if(com.TipoDeComprobante!="I")
                       return ("CP106 - El valor del atributo \"TipoDeComprobante\" es diferente de \"I\", o el valor registrado en el atributo \"TipoEstacion\" es diferente de \"02\" \"Intermedia\", o el nodo registrado en \"Mercancias\" es diferente de \"TransporteFerroviario\", o el valor del atributo \"TipoRelacion\" es diferente de \"05\".");
                   if (carta.Ubicaciones == null)
                       return ("CP106 - El valor del atributo \"TipoDeComprobante\" es diferente de \"I\", o el valor registrado en el atributo \"TipoEstacion\" es diferente de \"02\" \"Intermedia\", o el nodo registrado en \"Mercancias\" es diferente de \"TransporteFerroviario\", o el valor del atributo \"TipoRelacion\" es diferente de \"05\".");
         
                    foreach (var ubi in carta.Ubicaciones)
                   {
                       if (ubi.TipoEstacion == "02")
                       { exi = true; break; }
                   }
                    if (exi == false)
                        return ("CP106 - El valor del atributo \"TipoDeComprobante\" es diferente de \"I\", o el valor registrado en el atributo \"TipoEstacion\" es diferente de \"02\" \"Intermedia\", o el nodo registrado en \"Mercancias\" es diferente de \"TransporteFerroviario\", o el valor del atributo \"TipoRelacion\" es diferente de \"05\".");
                     if (carta.Mercancias!=null )
                         if(carta.Mercancias.TransporteFerroviario == null)
                      return ("CP106 - El valor del atributo \"TipoDeComprobante\" es diferente de \"I\", o el valor registrado en el atributo \"TipoEstacion\" es diferente de \"02\" \"Intermedia\", o el nodo registrado en \"Mercancias\" es diferente de \"TransporteFerroviario\", o el valor del atributo \"TipoRelacion\" es diferente de \"05\".");
                     if(com.CfdiRelacionados.TipoRelacion!="05")
                         return ("CP106 - El valor del atributo \"TipoDeComprobante\" es diferente de \"I\", o el valor registrado en el atributo \"TipoEstacion\" es diferente de \"02\" \"Intermedia\", o el nodo registrado en \"Mercancias\" es diferente de \"TransporteFerroviario\", o el valor del atributo \"TipoRelacion\" es diferente de \"05\".");
               

                }

                if (com.TipoDeComprobante == "I")
                {
           
                    if (com.Moneda == "XXX")
                        return ("CP104 - El valor registrado en el atributo \"Moneda\" es igual a \"XXX\".");
                    if (com.Receptor.Rfc != "XAXX010101000" && com.Receptor.Rfc != "XEXX010101000")
                    {
                        Operaciones_IRFC r = new Operaciones_IRFC();
                        vI_RFC t = r.Consultar_IRFC(com.Receptor.Rfc);
                        if (t == null)
                        {
                            return "CP108 - El RFC receptor no es un genérico o no está en la lista de RFC inscritos no cancelados en el SAT.";

                        }
                    }
                    
                    
                 }
                //---------------------------------------
                if(carta==null)
                    return "CP110 - El nodo  \"CartaPorte\" no se registró como nodo hijo del nodo complemento del CFDI.";
                
                if(com.Complemento.Any.Count>1)
                {   int d=0; 
                        d= com.Complemento.Any.Where(p => p.LocalName == "CartaPorte").Count();
                if(d>1)
                    return "CP111 - Existe más de un nodo \"CartaPorte\".";

                var x = com.Complemento.Any.FirstOrDefault(p => p.LocalName != "CartaPorte" && p.LocalName != "TimbreFiscalDigital");
                    if(x!=null)
                    return "CP112 - El complemento registrado de manera adicional, no corresponde con los complemento con los cuales puede coexistir.";

                }

                if (com.TipoDeComprobante != "T" && com.TipoDeComprobante != "I" )
                          return "CP113 - El valor registrado en \"TipoDeComprobante\" es diferente de \"I\" o \"T\".";
               
                if (carta.TranspInternac == CartaPorteTranspInternac.Sí)
                {
                    if (carta.ViaEntradaSalidaSpecified == false || carta.EntradaSalidaMercSpecified == false)
                        return "CP114 - Se debe registrar información en el atributo \"EntradaSalidaMerc\" y \"ViaEntradaSalida\".";
                
                    if (com.TipoDeComprobante == "I" && carta.Mercancias.AutotransporteFederal != null)
                    {
                        if (com.Impuestos.Retenciones == null || com.Impuestos.Traslados == null)
                            return "CP115 - Se deben relacionar los números de pedimento dentro del nodo \"InformacionAduanera\" a nivel Concepto para los bienes o mercancías que se trasladan asociadas al servicio, siempre que exista el nodo “Mercancias:AutotransporteFederal”, el tipo de comprobante sea \"I\" y deberá registrar información en los nodos \"Impuestos:Traslados\",  e \"Impuestos:Retenciones\" a nivel CFDI.";

                    }
                    if (com.TipoDeComprobante == "T")
                    {
                     foreach (var con in com.Conceptos)
                     {
                         if (con.InformacionAduanera == null)
                           return "CP116 - Se deben relacionar los números de pedimento dentro del nodo \"InformacionAduanera\" a nivel Concepto para los bienes o mercancías que se trasladan asociadas al servicio, siempre que exista el nodo “Mercancias:AutotransporteFederal”, el tipo de comprobante sea \"I\" y deberá registrar información en los nodos \"Impuestos:Traslados\",  e \"Impuestos:Retenciones\" a nivel CFDI.";
                         foreach (var inf in con.InformacionAduanera)
                         {
                             if(string.IsNullOrEmpty(inf.NumeroPedimento))
                             return "CP116 - Siempre que el tipo de comprobante sea \"T\", se debe registrar información del número de pedimento en el nodo \"InformacionAduanera\" a nivel Concepto para los bienes o mercancías registrados a nivel CFDI.";
              
                         }
                     }
                    }
                }
                else
                {
                    if (carta.ViaEntradaSalidaSpecified == true || carta.EntradaSalidaMercSpecified == true)
                        return "CP117 - No se debe registrar información en el atributo \"EntradaSalidaMerc\" y \"ViaEntradaSalida\".";
                    if (com.TipoDeComprobante == "I" && carta.Mercancias.AutotransporteFederal != null)
                    {
                        if ( com.Impuestos.Retenciones == null || com.Impuestos.Traslados == null)
                            return "CP118 - Se debe registrar información en el nodo  \"Impuestos:Traslados\",  y en el nodo \"Impuestos:Retenciones\" a nivel CFDI, siempre que  el tipo de comprobante sea \"I\" y exista el nodo \"Mercancias:AutotransporteFederal\".";

                    }
                
                }
                if (carta.Mercancias.AutotransporteFederal != null || carta.Mercancias.TransporteFerroviario != null)
                    if (carta.TotalDistRecSpecified == false)
                        return "CP119 - El valor registrado en el nodo \"Mercancias\" es diferente a \"TransporteFerroviario\" o \"AutotransporteFederal\".";
                if (carta.Mercancias.AutotransporteFederal == null && carta.Mercancias.TransporteFerroviario == null)
                    if (carta.TotalDistRecSpecified == true)
                        return "CP119 - El valor registrado en el nodo \"Mercancias\" es diferente a \"TransporteFerroviario\" o \"AutotransporteFederal\".";


                int nodoOrigen = 0; int nodoDestino = 0;
                if (carta.TotalDistRecSpecified == true)
                {

                    decimal distancia = 0; 
                    foreach (var ubi in carta.Ubicaciones)
                    {
                        if ( ubi.Destino != null )//no se entiende bien
                        {
                            if (ubi.DistanciaRecorridaSpecified)
                                distancia = distancia + ubi.DistanciaRecorrida;
                        }
                        if (ubi.Destino != null)
                            nodoDestino++;
                        if(ubi.Origen!=null)
                            nodoOrigen++;
                    } 
                    if(carta.TotalDistRec!=distancia)
                        return "CP120 - El valor registrado no coincide con la suma de los atributos \"DistanciaRecorrida\".";
         
                }
                if (carta.Mercancias.TransporteFerroviario != null)
                {
                    if (nodoOrigen > 1)
                        return "CP121 - Existen más de un nodo \"Ubicaciones:Ubicacion:Origen\"";
                    if(com.CfdiRelacionados==null)
                        if (nodoDestino != 6)
                            return "CP122 - El número de nodos \"Ubicacion:Destino\" registrados es menor o mayor que \"6\".";
                    if (com.CfdiRelacionados != null)
                        if (nodoDestino != 5)
                            return "CP123 - El número de nodos \"Ubicacion:Destino\" registrados es menor o mayor que \"5\".";
             
               }
                if (carta.Mercancias.AutotransporteFederal != null || carta.Mercancias.TransporteMaritimo != null || carta.Mercancias.TransporteAereo != null)
                {
                    if (carta.Ubicaciones.Count() < 2)
                        return "CP124 - El número de nodos de \"Ubicacion\" es menor a \"2\", o no existe el nodo \"Origen\" y/o \"Destino\".";
                }
                if (carta.TranspInternac == CartaPorteTranspInternac.No)
                if (carta.Mercancias.TransporteFerroviario != null || carta.Mercancias.TransporteMaritimo != null || carta.Mercancias.TransporteAereo != null)
                {
                    foreach (var ubi in carta.Ubicaciones)
                    {
                        if (ubi.TipoEstacionSpecified == true)
                        {
                            c_TipoEstacion myTipoEstacion;
                            Enum.TryParse<c_TipoEstacion>("Item"+ubi.TipoEstacion, out myTipoEstacion);
                            if (myTipoEstacion.ToString() != "Item"+ubi.TipoEstacion)
                               return "CP125 - La clave registrada en el atributo \"TipoEstacion\" es diferente a las contenidas en el catálogo \"c_TipoEstacion\" o se registro el nodo \"Mercancias:AutotransporteFederal\".";
                        }
                        else
                            return "CP125 - La clave registrada en el atributo \"TipoEstacion\" es diferente a las contenidas en el catálogo \"c_TipoEstacion\" o se registro el nodo \"Mercancias:AutotransporteFederal\".";

                    }
                }

                if (carta.TranspInternac == CartaPorteTranspInternac.Sí)
                {
                    foreach (var ubi in carta.Ubicaciones)
                    {
                        if (ubi.Domicilio != null)
                        {
                            if (ubi.Domicilio.Pais != "MEX")
                                if (ubi.TipoEstacionSpecified == true)
                                    return "CP126 - Este atributo no debe registrarse siempre que el atributo \"TranspInternac\" contenga el valor \"Sí\" y el valor del atributo \"Ubicacion:Domicilio:Pais\" contenga el valor \"MEX\".";
                        }
                    }

                }

                if (carta.Mercancias.AutotransporteFederal != null)
                {
                    string Domicilio1 = ""; string Domicilio2 = "";
                    foreach (var ubi in carta.Ubicaciones)
                    {
                        //-----------
                        /*
                     if(Domicilio1=="")
                        if (ubi.Origen != null && ubi.Domicilio != null)
                        {
                            Domicilio1 = ubi.Domicilio.Municipio + ubi.Domicilio.Localidad + ubi.Domicilio.Colonia + ubi.Domicilio.Calle + ubi.Domicilio.CodigoPostal;

                   
                        }
                     if(Domicilio2=="")
                         if (ubi.Destino != null && ubi.Domicilio != null)
                         {
                             Domicilio2 = ubi.Domicilio.Municipio + ubi.Domicilio.Localidad + ubi.Domicilio.Colonia + ubi.Domicilio.Calle + ubi.Domicilio.CodigoPostal;
                         }

                     if (Domicilio2 != "" && Domicilio1 != "")
                     {
                         if (Domicilio1 == Domicilio2 && ubi.DistanciaRecorridaSpecified == false)
                                return "CP128 - Se debe registrar información en el atributo \"DistanciaRecorrida\" o el nodo de \"Ubicacion:Origen\"  y \"Ubicacion:Destino\", no corresponden al mismo domicilio.";
                          else
                             if (Domicilio1 != Domicilio2 && ubi.DistanciaRecorridaSpecified == true)
                                 return "CP128 - Se debe registrar información en el atributo \"DistanciaRecorrida\" o el nodo de \"Ubicacion:Origen\"  y \"Ubicacion:Destino\", no corresponden al mismo domicilio.";
                         break;
                     }
                        */
                        if (ubi.Origen != null && ubi.Domicilio != null)
                        {
                            if (ubi.DistanciaRecorridaSpecified == false)
                                return "CP128 - Se debe registrar información en el atributo \"DistanciaRecorrida\" o el nodo de \"Ubicacion:Origen\"  y \"Ubicacion:Destino\", no corresponden al mismo domicilio.";

                        }

                        //----------
                    }
                }
                if (carta.Mercancias.AutotransporteFederal != null || carta.Mercancias.TransporteFerroviario != null)
                {
                    foreach (var ubi in carta.Ubicaciones)
                    { 
                       
                       if(ubi.Destino!=null)
                           if(ubi.DistanciaRecorridaSpecified==false)
                           return "CP127 - Se debe registrar información en el atributo \"DistanciaRecorrida\" o se registro el nodo \"Mercancias:TransporteAereo\" o \"Mercancias:TransporteMaritimo\".";
                    }


                }
               

                foreach (var mer in carta.Mercancias.Mercancia)
                {
                    if (mer.CantidadTransporta != null)
                    {
                        bool si = false;
                        foreach (var ub in carta.Ubicaciones)
                        {
                            if(ub.Origen!=null)
                            if (!string.IsNullOrEmpty(ub.Origen.IDOrigen))
                            { si = true; break; }
                        }
                        if (si)
                            break;
                        else
                            return "CP129 - Se debe registrar siempre que exista el nodo \"Mercancias:Mercancia:CantidadTransporta\".";

                    }
                }
                //inicio de ubicaciones multiples--------------------------------------
                foreach (var ubi in carta.Ubicaciones)
                {

                    if (ubi.Origen != null)
                    {
                        if (!string.IsNullOrEmpty(ubi.Origen.RFCRemitente))
                         if (ubi.Origen.RFCRemitente != com.Emisor.Rfc && com.TipoDeComprobante == "T")
                        {
                            Operaciones_IRFC r = new Operaciones_IRFC();
                            vI_RFC t = r.Consultar_IRFC(ubi.Origen.RFCRemitente);
                            if (t == null)
                            {
                                return "CP130 - El tipo de comprobante no es de tipo \"T\" o el RFC del remitente no se encuentra en la lista de RFC inscritos no cancelados del SAT l_RFC.";

                            }
                        }
                        if (!string.IsNullOrEmpty(ubi.Origen.RFCRemitente))
                        if (ubi.Origen.RFCRemitente != com.Receptor.Rfc && com.TipoDeComprobante == "I")
                        {
                            Operaciones_IRFC r = new Operaciones_IRFC();
                            vI_RFC t = r.Consultar_IRFC(ubi.Origen.RFCRemitente);
                            if (t == null)
                            {
                                return "CP131 - El tipo de comprobante no es de tipo \"I\" o el RFC del remitente no se encuentra en la lista de RFC inscritos no cancelados del SAT l_RFC.";

                            }
                        }


                        if (!string.IsNullOrEmpty(ubi.Origen.NumRegIdTrib) && !string.IsNullOrEmpty(ubi.Origen.RFCRemitente))
                            return "CP132 - CP132	Si existe información en el atributo \"Origen:NumRegIdTrib\" no se debe registrar información en el atributo \"Origen:RFCRemitente\".";
                        if (ubi.Origen.RFCRemitente != com.Receptor.Rfc)
                            if (ubi.Origen.ResidenciaFiscalSpecified==true)
                            if(ubi.Origen.ResidenciaFiscal!="MEX" && string.IsNullOrEmpty(ubi.Origen.NumRegIdTrib))
                                return "CP133 - El atributo \"Origen:NumRegIdTrib\", esta vacío o ya existe el atributo \"Origen:RFCRemitente\".";

                        if (!string.IsNullOrEmpty(ubi.Origen.NumRegIdTrib) && ubi.Origen.ResidenciaFiscalSpecified == false)
                            return "CP136 - No existe información en el atributo \"Ubicaciones:Ubicacion:Origen:NumRegIdTrib\".";
                        if (string.IsNullOrEmpty(ubi.Origen.NumRegIdTrib) && ubi.Origen.ResidenciaFiscalSpecified == true)
                            return "CP136 - No existe información en el atributo \"Ubicaciones:Ubicacion:Origen:NumRegIdTrib\".";

                        if (ubi.Origen.ResidenciaFiscalSpecified /*&& !string.IsNullOrEmpty(ubi.Origen.RFCRemitente)*/)
                        {
                            OperacionesCatalogos o13 = new OperacionesCatalogos();
                            CatalogosSAT.c_Pais pais17 = o13.Consultar_PaisVerificacionLinea(ubi.Origen.ResidenciaFiscal.ToString());
                            if (pais17 != null)
                            {
                                if (pais17.ValidaciondelRIT != null)
                                {
                                    Operaciones_IRFC r = new Operaciones_IRFC();
                                    vI_RFC t = r.Consultar_IRFC(ubi.Origen.NumRegIdTrib);
                                    if (t == null)
                                    {
                                        return "CP134 - El atributo \"Origen:NumRegIdTrib\" no tiene un valor que exista en el registro del país indicado en el atributo \"Origen:ResidenciaFiscal\".";

                                    }
                                }
                                else 
                                    if ( pais17.FormatodeRIT != null)
                                    {
                                        if (ubi.Origen.NumRegIdTrib == null)
                                            return "CP135 - El atributo \"Origen:NumRegIdTrib\" no cumple con el patrón publicado en la columna \"Formato de registro de identidad tributaria\" del país indicado en el atributo \"Origen:ResidenciaFiscal\".";
                                          if( !Regex.Match(ubi.Origen.NumRegIdTrib, "^" + pais17.FormatodeRIT + "$").Success)
                                        return "CP135 - El atributo \"Origen:NumRegIdTrib\" no cumple con el patrón publicado en la columna \"Formato de registro de identidad tributaria\" del país indicado en el atributo \"Origen:ResidenciaFiscal\".";
                                    }
                            }
                            else
                                return "CP134 - El atributo \"Origen:NumRegIdTrib\" no tiene un valor que exista en el registro del país indicado en el atributo \"Origen:ResidenciaFiscal\".";

                        }


                        if (carta.Mercancias.AutotransporteFederal != null && ubi.Origen.NumEstacionSpecified == true)
                            return "CP137 - Se registró información en el atributo \"Origen:NumEstacion\" cuando solo existe un nodo \"Mercancias:AutotransporteFederal\".";
                        if (ubi.Origen.NumEstacionSpecified == true)
                        {
                            c_Estaciones myEstacion;
                            Enum.TryParse<c_Estaciones>(ubi.Origen.NumEstacion, out myEstacion);
                            if (myEstacion.ToString() != ubi.Origen.NumEstacion)
                                if (!string.IsNullOrEmpty(ubi.Origen.NombreEstacion))
                                    return "CP139 - No existe \"NumEstacion\" que corresponde al medio de transporte.";
                        }
                        else
                            if (!string.IsNullOrEmpty(ubi.Origen.NombreEstacion))
                                return "CP139 - No existe \"NumEstacion\" que corresponde al medio de transporte.";

                        if (carta.Mercancias.TransporteMaritimo != null || carta.Mercancias.TransporteAereo != null || carta.Mercancias.TransporteFerroviario != null)
                        {
                            string clavetrasp = "";
                            if (carta.Mercancias.TransporteMaritimo != null)
                                clavetrasp = "02";
                            if (carta.Mercancias.TransporteAereo != null)
                                clavetrasp = "03";
                            if (carta.Mercancias.TransporteFerroviario != null)
                                clavetrasp = "04";

                            OperacionesCatalogos o1 = new OperacionesCatalogos();
                            CatalogosSAT.c_Estaciones Esta = o1.Consultar_Estaciones(clavetrasp, ubi.Origen.NumEstacion);                           if (Esta == null)
                            {
                                return "CP138 - El atributo \"Origen:NumEstacion\" tiene un valor no permitido.";

                            }
                                if (Esta.Nacionalidad != "México")
                                    if (ubi.Origen.NombreEstacion == "Extranjera" || string.IsNullOrEmpty(ubi.Origen.NombreEstacion))
                                        return "CP140 - La descripción \"Extranjera\" no es un valor valido para el nombre de la estación.";

                        }
                    
                            if (carta.Mercancias.TransporteMaritimo != null && ubi.Origen.NavegacionTraficoSpecified == false)
                                return "CP141 - No existe el nodo \"Mercancias:TransporteMaritimo\" y se registra información en el atributo \"Origen:NavegacionTrafico\".";
                    }//fin origen
                 
                 bool si = false;
                 if (carta.Mercancias != null && carta.Mercancias.Mercancia != null)
                 {
                     foreach (var mer in carta.Mercancias.Mercancia)
                     {
                         if (mer.CantidadTransporta != null)
                         {
                             { si = true; break; }
                         }
                     }
                 }

                 if (ubi.Destino != null)
                 {
                     if (!string.IsNullOrEmpty(ubi.Destino.IDDestino))
                     {

                         if (si == false)
                             return "CP142 - \"Se debe registrar siempre que exista el nodo \"Mercancias:Mercancia:CantidadTransporta\".";
                     }
                     else
                         if (si == true)
                             return "CP142 - \"Se debe registrar \"Destino:IDDestino\" siempre que exista el nodo \"Mercancias:Mercancia:CantidadTransporta\".";
                     if (!string.IsNullOrEmpty(ubi.Destino.RFCDestinatario))
                     if (ubi.Destino.RFCDestinatario != com.Emisor.Rfc && com.TipoDeComprobante == "T")
                     {
                         Operaciones_IRFC r = new Operaciones_IRFC();
                         vI_RFC t = r.Consultar_IRFC(ubi.Destino.RFCDestinatario);
                         if (t == null)
                         {
                             return "CP143 - El tipo de comprobante no es de tipo \"T\" o el RFC del destinatario no se encuentra en la lista de RFC inscritos no cancelados del SAT l_RFC";

                         }
                     }
                     if(!string.IsNullOrEmpty(ubi.Destino.RFCDestinatario))
                     if (ubi.Destino.RFCDestinatario != com.Receptor.Rfc && com.TipoDeComprobante == "I")
                     {
                         Operaciones_IRFC r = new Operaciones_IRFC();
                         vI_RFC t = r.Consultar_IRFC(ubi.Destino.RFCDestinatario);
                         if (t == null)
                         {
                             return "CP144 - El tipo de comprobante no es de tipo \"I\" o el RFC del destinatario no se encuentra en la lista de RFC inscritos no cancelados del SAT l_RFC";

                         }
                     }
                     if (!string.IsNullOrEmpty(ubi.Destino.NumRegIdTrib) && !string.IsNullOrEmpty(ubi.Destino.RFCDestinatario))
                         return "CP145 - Si existe información en el atributo \"Destino:NumRegIdTrib\" no se debe registrar información en el atributo \"Destino:RFCDestinatario\".";
                     if (ubi.Destino.ResidenciaFiscalSpecified && ubi.Destino.ResidenciaFiscal != "MEX" && string.IsNullOrEmpty(ubi.Destino.NumRegIdTrib))
                         return "CP146 - La clave registrada en el atributo \"ResidenciaFiscal\" es \"MEX\".";

                     if (!string.IsNullOrEmpty(ubi.Destino.NumRegIdTrib) && ubi.Destino.ResidenciaFiscalSpecified == false)
                         return "CP149 - No existe información en el atributo \"Ubicaciones:Ubicacion:Destino:NumRegIdTrib\".";
                     if (string.IsNullOrEmpty(ubi.Destino.NumRegIdTrib) && ubi.Destino.ResidenciaFiscalSpecified == true)
                         return "CP149 - No existe información en el atributo \"Ubicaciones:Ubicacion:Destino:NumRegIdTrib\".";

                     if (ubi.Destino.ResidenciaFiscalSpecified)
                     {
                         OperacionesCatalogos o13 = new OperacionesCatalogos();
                         CatalogosSAT.c_Pais pais17 = o13.Consultar_PaisVerificacionLinea(ubi.Destino.ResidenciaFiscal);
                         if (pais17 != null)
                         {
                             if (pais17.ValidaciondelRIT != null)
                             {
                                 Operaciones_IRFC r = new Operaciones_IRFC();
                                 vI_RFC t = r.Consultar_IRFC(ubi.Destino.NumRegIdTrib);
                                 if (t == null)
                                 {
                                     return "CP147 - El atributo \"Destino:NumRegIdTrib\" no tiene un valor que exista en el registro del país indicado en el atributo \"Destino:ResidenciaFiscal\".";
                                 }
                             }
                             else
                                  if ( pais17.FormatodeRIT != null)
                                    {
                                        if (ubi.Destino.NumRegIdTrib == null)
                                            return "CP148 - El atributo \"Destino:NumRegIdTrib\" no cumple con el patrón publicado en la columna \"Formato de registro de identidad tributaria\" del país indicado en el atributo \"Destino:ResidenciaFiscal\".";
                                         if (  !Regex.Match(ubi.Destino.NumRegIdTrib, "^" + pais17.FormatodeRIT + "$").Success)
                                          return "CP148 - El atributo \"Destino:NumRegIdTrib\" no cumple con el patrón publicado en la columna \"Formato de registro de identidad tributaria\" del país indicado en el atributo \"Destino:ResidenciaFiscal\".";
                                    }
                         }
                         else
                             return "CP147 - El atributo \"Destino:NumRegIdTrib\" no tiene un valor que exista en el registro del país indicado en el atributo \"Destino:ResidenciaFiscal\".";

                     }
                     if (carta.Mercancias.AutotransporteFederal != null && ubi.Destino.NumEstacionSpecified == true)
                         return "CP150 - Se registró información en el atributo \"Destino:NumEstacion\" cuando solo existe un nodo \"Mercancias:AutotransporteFederal\".";
                    
                     if (ubi.Destino.NumEstacionSpecified==true)
                      {
                          c_Estaciones myTipoEstaciones;
                         Enum.TryParse<c_Estaciones>(ubi.Destino.NumEstacion, out myTipoEstaciones);
                         if (myTipoEstaciones.ToString() != ubi.Destino.NumEstacion)
                             return "CP152 - El valor de la descripción no coincide con una clave del catálogo.";
                     }

                 if (carta.Mercancias.TransporteMaritimo != null || carta.Mercancias.TransporteAereo != null || carta.Mercancias.TransporteFerroviario != null)
                 {
                     string clavetrasp = "";
                     if (carta.Mercancias.TransporteMaritimo != null)
                         clavetrasp = "02";
                     if (carta.Mercancias.TransporteAereo != null)
                         clavetrasp = "03";
                     if (carta.Mercancias.TransporteFerroviario != null)
                         clavetrasp = "04";

                     OperacionesCatalogos o1 = new OperacionesCatalogos();
                     CatalogosSAT.c_Estaciones Esta = o1.Consultar_Estaciones(clavetrasp, ubi.Destino.NumEstacion);
                     if (Esta == null)
                     {
                         return "CP151 - El atributo \"Destino:NumEstacion\" tiene un valor no permitido.";

                     }
                     if (Esta.Nacionalidad != "México")
                         if (ubi.Destino.NombreEstacion == "Extranjera" || string.IsNullOrEmpty(ubi.Destino.NombreEstacion))
                             return "CP153 - La descripción \"Extranjera\" no es un valor valido para el nombre de la estación.";

                 }
               

                 if (carta.Mercancias.TransporteMaritimo != null && ubi.Destino.NavegacionTraficoSpecified == false)
                     return "CP154 - No existe el nodo \"Mercancias:TransporteMaritimo\" y se registra información en el atributo \"Destino:NavegacionTrafico\".";
                 }//fin destino  
                     if (carta.Mercancias.TransporteFerroviario != null && ubi.TipoEstacionSpecified == true)
                     if (ubi.TipoEstacion == "02" && ubi.Domicilio != null)
                      return "CP155 - El nodo \"Ubicacion:Domicilio\" no debe existir.";
           
                 if ((ubi.TipoEstacion == "01" || ubi.TipoEstacion == "03") && ubi.Domicilio == null)
                     return "CP156 - Se debe registrar el nodo \"Ubicacion:Domicilio\".";
                 if (ubi.Domicilio != null)
                 {
                     c_Pais myPais;
                     Enum.TryParse<c_Pais>(ubi.Domicilio.Pais, out myPais);
                     if (myPais.ToString() != ubi.Domicilio.Pais)
                         return "CP162 - El atributo \"Domicilio:Pais\" tiene un valor no permitido.";
                    
                     if (ubi.Domicilio.Pais == "MEX")
                     {
                         if(string.IsNullOrEmpty(ubi.Domicilio.Colonia))
                             return ("CP157 - El atributo \"Domicilio:Colonia\" debe contener una clave del catálogo de \"catCFDI:c_Colonia\", donde la columna \"c_CodigoPostal\" debe ser igual a la clave registrada en el atributo \"CodigoPostal\".");
               
                         CatalogosSAT.OperacionesCatalogos o = new CatalogosSAT.OperacionesCatalogos();
                         CatalogosSAT.c_Estado estado5 = o.Consultar_Estados(ubi.Domicilio.Estado);
                         if (estado5 == null)
                             return ("CP161 - El atributo \"Estado\" no existe o no contiene un valor  o no corresponde con una clave del \"catCFDI:c_Estado\".");

                         CatalogosSAT.c_Colonia colonia5 = o.Consultar_Colonia(ubi.Domicilio.CodigoPostal, ubi.Domicilio.Colonia);
                         if (colonia5 == null)
                             return ("CP157 - El atributo \"Domicilio:Colonia\" debe contener una clave del catálogo de \"catCFDI:c_Colonia\", donde la columna \"c_CodigoPostal\" debe ser igual a la clave registrada en el atributo \"CodigoPostal\".");
                        if(string.IsNullOrEmpty(ubi.Domicilio.Localidad))
                            return ("CP159 - El atributo \"Domicilio:Localidad\" debe contener una clave del catálogo de \"catCFDI:c_Localidad\", donde la columna clave de \"c_Estado\" debe ser igual a la clave registrada en el atributo estado si el atributo \"Pais\" tiene el valor \"MEX\".");

                         CatalogosSAT.c_Localidad localidad5 = o.Consultar_Localidad(ubi.Domicilio.Estado.ToString(), ubi.Domicilio.Localidad);
                         if (localidad5 == null)
                             return ("CP159 - El atributo \"Domicilio:Localidad\" debe contener una clave del catálogo de \"catCFDI:c_Localidad\", donde la columna clave de \"c_Estado\" debe ser igual a la clave registrada en el atributo estado si el atributo \"Pais\" tiene el valor \"MEX\".");
                          if(string.IsNullOrEmpty(ubi.Domicilio.Municipio))
                              return ("CP160 - El atributo \"Domicilio:Municipio\" debe contener una clave del catálogo de \"catCFDI:c_Municipio\" donde la columna clave de \"c_Estado\" debe ser igual a la clave registrada en el atributo \"Estado\" si el atributo \"Pais\" tiene el valor \"MEX\".");
                  
                         CatalogosSAT.c_Municipio municipio5 = o.Consultar_Municipio(ubi.Domicilio.Estado, ubi.Domicilio.Municipio);
                         if (municipio5 == null)
                             return ("CP160 - El atributo \"Domicilio:Municipio\" debe contener una clave del catálogo de \"catCFDI:c_Municipio\" donde la columna clave de \"c_Estado\" debe ser igual a la clave registrada en el atributo \"Estado\" si el atributo \"Pais\" tiene el valor \"MEX\".");
                       
                          CatalogosSAT.c_CP CP5 = o.Consultar_CP(ubi.Domicilio.Estado, ubi.Domicilio.Municipio, ubi.Domicilio.Localidad, ubi.Domicilio.CodigoPostal);
                         if (CP5 == null)
                             return ("CP163 - El atributo \"Domicilio:CodigoPostal\" debe contener una clave del catálogo de \"catCFDI:c_CodigoPostal\", donde la columna clave de \"c_Estado\" debe ser igual a la clave registrada en el atributo \"Estado\", la columna clave de \"c_Municipio\" debe ser igual a la clave registrada en el atributo \"Municipio\", y si existe el atributo de \"Localidad\", la columna clave de \"c_Localidad\" debe ser igual a la clave registrada en el atributo \"Localidad\" si el atributo \"Pais\" tiene el valor \"MEX\".");

                     }
                     else
                     {
                         if (!string.IsNullOrEmpty(ubi.Domicilio.Localidad))
                             if (ubi.Domicilio.Localidad.All(char.IsDigit))
                                          return ("CP158 - La clave registrada del país es \"MEX\" y el atributo \"Localidad\" no debe ser texto libre.");

                     }
                 }//findomicilio

                } // fin de ciclo de ubicaciones--------------------------

                if (carta.Mercancias.TransporteAereo != null && carta.Mercancias.PesoBrutoTotalSpecified == false)
                    return "CP167 - El atributo \"Mercancias:PesoBrutoTotal\" no debe existir.";
  
                    if ((carta.Mercancias.TransporteFerroviario != null ||carta.Mercancias.TransporteAereo!=null)&& carta.Mercancias.PesoBrutoTotalSpecified == false)
                    return ("CP165 - El atributo \"Mercancias:PesoBrutoTotal\" no debe existir.");
                if ((carta.Mercancias.TransporteFerroviario == null && carta.Mercancias.TransporteAereo == null) && carta.Mercancias.PesoBrutoTotalSpecified == true)
                    return ("CP165 - El atributo \"Mercancias:PesoBrutoTotal\" no debe existir.");
                decimal sumaPeso = 0;
                if (carta.Mercancias.PesoBrutoTotalSpecified == true)
                {
                    if (carta.Mercancias.TransporteFerroviario != null)
                    {
                        if (carta.Mercancias.TransporteFerroviario.Carro != null)
                            foreach (var mer in carta.Mercancias.TransporteFerroviario.Carro)
                            {
                                if (mer.Contenedor != null)
                                    foreach (var cont in mer.Contenedor)
                                    {
                                        sumaPeso = sumaPeso + cont.PesoContenedorVacio + cont.PesoNetoMercancia;
                                    }
                            }
                        if (carta.Mercancias.PesoBrutoTotal != sumaPeso)
                            return "CP166 - La suma del peso bruto total de los atributos TransporteFerroviario:Carro:Contenedor:PesoContenedorVacio y TransporteFerroviario:Carro:Contenedor:PesoNetoMercancia, no corresponde.";
                    }
                }

             
               // if (carta.Mercancias.TransporteAereo == null && carta.Mercancias.PesoBrutoTotalSpecified == true)
               //     return "CP167 - El atributo \"Mercancias:PesoBrutoTotal\" no debe existir.";
                if (carta.Mercancias.TransporteFerroviario != null || carta.Mercancias.TransporteAereo!=null)
                {
                    if (carta.Mercancias.UnidadPesoSpecified == true)
                    {
                        c_ClaveUnidadPeso myClaveUnidadPeso;
                        Enum.TryParse<c_ClaveUnidadPeso>(carta.Mercancias.UnidadPeso, out myClaveUnidadPeso);
                        if (myClaveUnidadPeso.ToString() != carta.Mercancias.UnidadPeso)
                            return "CP168 - El atributo \"Mercancias:UnidadPeso\" tiene un valor no permitido del catálogo \"catCartaPorte:c_ClaveUnidadPeso\".";
                    }
                    else
                        return "CP168 - El atributo \"Mercancias:UnidadPeso\" tiene un valor no permitido del catálogo \"catCartaPorte:c_ClaveUnidadPeso\".";
                }

                if (carta.Mercancias.TransporteMaritimo != null)
                {
                    if (carta.Mercancias.PesoNetoTotalSpecified == false)
                        return "CP169 - La suma de los valores registrados en el atributo \"Mercancia:DetalleMercancia:PesoNeto\", no corresponde.";

                     sumaPeso = 0;
                    foreach (var mer in carta.Mercancias.Mercancia)
                    {
                        if(mer.DetalleMercancia!=null)
                        sumaPeso = sumaPeso + mer.DetalleMercancia.PesoNeto;
                    }
                    if (carta.Mercancias.PesoNetoTotal != sumaPeso)
                        return "CP169 - La suma de los valores registrados en el atributo \"Mercancia:DetalleMercancia:PesoNeto\", no corresponde.";

                }
                if (carta.Mercancias.TransporteFerroviario != null)
                {
                   if( carta.Mercancias.PesoNetoTotalSpecified==false)
                       return "CP170 - La suma de los valores registrados en el atributo \"TransporteFerroviario:Carro:ToneladasNetasCarro\", no corresponde.";
     
                     sumaPeso = 0;
                    foreach (var car in carta.Mercancias.TransporteFerroviario.Carro)
                    {
                        sumaPeso = sumaPeso + car.ToneladasNetasCarro;
                    }
                    if (carta.Mercancias.PesoNetoTotal != sumaPeso)
                        return "CP170 - La suma de los valores registrados en el atributo \"TransporteFerroviario:Carro:ToneladasNetasCarro\", no corresponde.";
                }
                if (carta.Mercancias.Mercancia.Count() != carta.Mercancias.NumTotalMercancias)
                    return "CP171 - El valor registrado no coincide con el número de elementos \"Mercancia\" que se registraron en el complemento.";
              
                if (com.TipoDeComprobante == "T"  )
                {

                     bool exi = false;
                        foreach (var mer in carta.Mercancias.Mercancia)
                        {
                            if (mer.BienesTranspSpecified == false && !string.IsNullOrEmpty(mer.Descripcion))
                                return "CP174 - No se debe registrar información en el atributo \"Mercancia:Descripcion\".";

                            if (com.Conceptos.Count() > 1)
                            {
                               
                            if (mer.BienesTranspSpecified == false)
                            {
                                if (!string.IsNullOrEmpty( mer.Cantidad) )
                                    return "CP175 - No se debe registrar información en el atributo \"Mercancia:Cantidad\" cuando solo existe un tipo de mercancía a nivel concepto del CFDI y el tipo de comprobante es \"T\".";
                            }
                          if (string.IsNullOrEmpty( mer.Cantidad) )
                                    return "CP175 - No se debe registrar información en el atributo \"Mercancia:Cantidad\" cuando solo existe un tipo de mercancía a nivel concepto del CFDI y el tipo de comprobante es \"T\".";
                           
                                exi = false;
                                foreach (var con in com.Conceptos)
                                {
                                    if (con.ClaveProdServ == mer.BienesTransp)
                                    { exi = true; break; }
                                }
                                if (exi == false)
                                    return "CP172 - No se puede omitir cuando existe más de un tipo de mercancía o la clave registrada en el atributo  \"ClaveProdServ\" a nivel CFDI no corresponde a la registrada en este atributo.";

                              
                             
                            }
                            if (mer.BienesTranspSpecified == false && !string.IsNullOrEmpty(mer.ClaveUnidad) )
                                return "CP177 - No se debe registrar información en el atributo \"Mercancia:ClaveUnidad\".";

                    }
                                      
                       
                    
                }

              

                    foreach (var mer in carta.Mercancias.Mercancia)
                    {
                        if (com.TipoDeComprobante == "I")
                        {
                            CatalogosSAT.OperacionesCatalogos o = new CatalogosSAT.OperacionesCatalogos();
                            CatalogosSAT.c_ClaveProdServCP clave = o.Consultar_ClaveProdServCP(mer.BienesTransp);
                            if (clave == null)
                                return "CP173 - El tipo de comprobante es distinto de \"I\" y el atributo \"Mercancia:BienesTransp\" tiene una clave diferente del catálogo \"catCartaPorte:c_ClaveProdServCP\".";
                        }

                        if (!string.IsNullOrEmpty(mer.Cantidad))
                        { 
                           if(com.TipoDeComprobante != "I")
                               return "CP176 - El atributo \"BienesTransp\" contiene un valor o el tipo de comprobante es diferente de \"I\".";
                           if (mer.BienesTranspSpecified == true )
                               return "CP176 - El atributo \"BienesTransp\" contiene un valor o el tipo de comprobante es diferente de \"I\".";
                   
                        }
                        if (!string.IsNullOrEmpty(mer.ClaveUnidad))
                        {
                            if (com.TipoDeComprobante != "I")
                                return "CP178 - El atributo \"BienesTransp\" contiene un valor o el tipo de comprobante es diferente de \"I\".";
                            if (mer.BienesTranspSpecified == true)
                                return "CP178 - El atributo \"BienesTransp\" contiene un valor o el tipo de comprobante es diferente de \"I\".";
                     
                        }


                        if (mer.BienesTranspSpecified == true)
                        {

                            CatalogosSAT.OperacionesCatalogos o = new CatalogosSAT.OperacionesCatalogos();
                            CatalogosSAT.c_ClaveProdServCP clave = o.Consultar_ClaveProdServCP(mer.BienesTransp);
                            if (clave != null)
                            {
                                if (clave.MaterialPeligroso == "1" || clave.MaterialPeligroso == "0,1")
                                {
                                    if (mer.MaterialPeligrosoSpecified == false)
                                        return "CP179 - Se debe especificar el atributo \"Mercancia:BienesTransp\" se considera material peligroso.";

                                }
                            }
                            if (mer.MaterialPeligroso == "Sí")
                            {
                                c_MaterialPeligroso myMaterialPeligroso;
                                Enum.TryParse<c_MaterialPeligroso>(mer.CveMaterialPeligroso, out myMaterialPeligroso);
                                if (myMaterialPeligroso.ToString() != mer.CveMaterialPeligroso)
                                    return "CP180 - El valor registrado en el atributo \"MaterialPeligroso\" es diferente a las establecidas en el catálogo \"catCartaPorte:c_MaterialPeligroso\".";
                            }
                            else
                            {
                                if (mer.CveMaterialPeligrosoSpecified == true)
                                    return "CP181 - El atributo \"Mercancia:CveMaterialPeligroso\" no debe existir.";
                                if (mer.EmbalajeSpecified == true)
                                    return "CP182 - El atributo \"Mercancia:Embalaje\", no debe existir.";
                            }
                        }
                        if (mer.MaterialPeligroso == "No")
                        {
                            if (mer.CveMaterialPeligrosoSpecified == true)
                                return "CP181 - El atributo \"Mercancia:CveMaterialPeligroso\" no debe existir.";
                            if (mer.EmbalajeSpecified == true)
                                return "CP182 - El atributo \"Mercancia:Embalaje\", no debe existir.";
                        }
                        if(mer.ValorMercanciaSpecified==false && carta.Mercancias.TransporteAereo!=null)
                            return "CP183 - El atributo \"ValorMercancia\" debe contener un valor siempre que se registre el nodo \"Mercancias:TransporteAereo\".";
                         if(mer.MonedaSpecified==false && carta.Mercancias.TransporteAereo!=null)
                            return "CP184 - El atributo \"Mercancia:Moneda\" debe contener un valor siempre que se registre el nodo \"Mercancias:TransporteAereo\".";
                         if(carta.TranspInternac!=null &&mer.FraccionArancelariaSpecified == false)
                          return "CP185 - El atributo \"Mercancia:FraccionArancelaria\" tiene una clave diferente a las establecidas en el catálogo \"catComExt:c_FraccionArancelaria\" o este atributo no debe existir.";
                         if(carta.TranspInternac==null &&mer.FraccionArancelariaSpecified == true)
                          return "CP185 - El atributo \"Mercancia:FraccionArancelaria\" tiene una clave diferente a las establecidas en el catálogo \"catComExt:c_FraccionArancelaria\" o este atributo no debe existir.";
               
                        if (carta.TranspInternac!=null && mer.FraccionArancelariaSpecified == true)
                       {
                           CatalogosSAT.OperacionesCatalogos o13 = new CatalogosSAT.OperacionesCatalogos();
                           CatalogosSAT.c_FraccionArancelaria f = o13.Consultar_FraccionArancelaria(mer.FraccionArancelaria.Replace("Item", ""));
                           if (f == null)
                          return "CP185 - El atributo \"Mercancia:FraccionArancelaria\" tiene una clave diferente a las establecidas en el catálogo \"catComExt:c_FraccionArancelaria\" o este atributo no debe existir.";
  
                       }
                        if (carta.TranspInternac == CartaPorteTranspInternac.Sí && carta.EntradaSalidaMercSpecified == true && carta.EntradaSalidaMerc == CartaPorteEntradaSalidaMerc.Salida)
                        { 
                           if(string.IsNullOrEmpty(mer.UUIDComercioExt))
                               return "CP186 - El atributo \"Mercancia:UUIDComercioExt\", contiene una estructura invalida o no debe existir.";
  
                          }

                        int destino = 0; //string StrDestino="";
                        int origen=0; //string StrOrigen="";
                        foreach (var ubi in carta.Ubicaciones)
                        {
                            if (ubi.Destino != null)
                                destino++;
                            if (ubi.Origen != null)
                                origen++;
                           // if (destino == 1 && string.IsNullOrEmpty(StrDestino))
                           //     StrDestino=ubi.Destino.IDDestino;
                           // if(origen==1&& string.IsNullOrEmpty(StrOrigen))
                           //     StrOrigen=ubi.Origen.IDOrigen;

                        }
                        if (destino < 2 && mer.CantidadTransporta!=null)
                        return "CP187 - Solo se tiene un registro en el nodo \"Ubicacion:Destino\", por lo que no se debe registrar este nodo.";
                        if (destino > 1 && mer.CantidadTransporta==null)
                            return "CP188 - Se tiene más de un registro del nodo \"Ubicacion:Destino\", por lo que se debe registrar el nodo \"CantidadTransporta\".";
                       /* if (mer.CantidadTransporta != null)
                        {
                            bool dis = false;
                            foreach (var ubi in carta.Ubicaciones)
                            {
                                if(ubi.Destino!=null)
                                if (StrDestino != ubi.Destino.IDDestino)
                                { dis = true; break; }
                                if (ubi.Origen != null)
                                    if (StrOrigen != ubi.Origen.IDOrigen)
                                 { dis = true; break; }
                               
                            }
                            if(dis==false)
                        
                                                       
                        }
                        */
                        if (mer.CantidadTransporta != null)
                        {
                           bool sali=false;
                            foreach (var cat in mer.CantidadTransporta)
                            {
                               
                                if (!string.IsNullOrEmpty(cat.IDOrigen))
                                {
                                    sali = false;
                                    foreach (var ubi in carta.Ubicaciones)
                                    {  if(ubi.Origen!=null)
                                        if (!string.IsNullOrEmpty(ubi.Origen.IDOrigen))
                                        {
                                            if (cat.IDOrigen == ubi.Origen.IDOrigen)
                                            { sali = true; break; }
                                        }
                                    }
                                    if(sali==false)
                                        return "CP189 - El valor registrado no coincide con un valor registrado en los atributos \"Ubicacion:IDOrigen\".";
                 
                                }
                                if (!string.IsNullOrEmpty(cat.IDDestino))
                                {
                                    sali = false;
                                    foreach (var ubi in carta.Ubicaciones)
                                    {   if(ubi.Destino!=null)
                                        if (!string.IsNullOrEmpty(ubi.Destino.IDDestino))
                                        {
                                            if (cat.IDDestino == ubi.Destino.IDDestino)
                                            { sali = true; break; }
                                        }
                                    }
                                    if (sali == false)
                                        return "CP190 - El valor registrado no coincide con  un valor registrado en los atributos \"Ubicacion:IDDestino\".";
                 
                                }
                            }
                        }

                        int ntrp = 0;
                        if (carta.Mercancias.AutotransporteFederal != null)
                            ntrp++;
                        if (carta.Mercancias.TransporteFerroviario != null)
                            ntrp++;
                        if (carta.Mercancias.TransporteMaritimo != null)
                            ntrp++;


                        if (carta.Mercancias.Mercancia.Count() > 1 && ntrp>1)
                        {
                            foreach (var cat in mer.CantidadTransporta)
                            {
                                if (cat.CvesTransporteSpecified)
                                {
                                    c_CveTransporte myCvetransporte;
                                    Enum.TryParse<c_CveTransporte>(cat.CvesTransporte, out myCvetransporte);
                                    if (myCvetransporte.ToString() != cat.CvesTransporte)
                                        return "CP191 - El atributo \"CantidadTransporta:CvesTransporte\" tiene una clave diferente a las establecidas en el catálogo o no existe más de un nodo Mercancia, o no existe mas de tipo de transporte.";

                                }
                            }
                        }
                        else
                        {
                            if (carta.Mercancias.Mercancia.Count() >0)
                             foreach (var cat in mer.CantidadTransporta)
                            {
                                if (cat.CvesTransporteSpecified)
                                {
                                   return "CP191 - El atributo \"CantidadTransporta:CvesTransporte\" tiene una clave diferente a las establecidas en el catálogo o no existe más de un nodo Mercancia, o no existe mas de tipo de transporte.";

                                }

                            }
                        }

                        if (mer.DetalleMercancia != null && carta.Mercancias.TransporteMaritimo == null)
                            return "CP192 - El atributo \"DetalleMercancia\" no debe existir.";
                        if (mer.DetalleMercancia == null && carta.Mercancias.TransporteMaritimo != null)
                            return "CP192 - El atributo \"DetalleMercancia\" no debe existir.";

                        if (carta.Mercancias.TransporteAereo != null)
                        {
                            if (string.IsNullOrEmpty(carta.Mercancias.TransporteAereo.NumRegIdTribTranspor) && string.IsNullOrEmpty(carta.Mercancias.TransporteAereo.RFCTransportista) && carta.Mercancias.TransporteAereo.ResidenciaFiscalTransporSpecified == true)
                                return "CP193 - El atributo \"TransporteAereo:RFCTransportista\", esta vacío o ya existe el atributo \"RFCTransportista\".";
                            if (!string.IsNullOrEmpty(carta.Mercancias.TransporteAereo.NumRegIdTribTranspor) && !string.IsNullOrEmpty(carta.Mercancias.TransporteAereo.RFCTransportista) && carta.Mercancias.TransporteAereo.ResidenciaFiscalTransporSpecified == true)
                                return "CP193 - El atributo \"TransporteAereo:RFCTransportista\", esta vacío o ya existe el atributo \"RFCTransportista\".";

                            if (!string.IsNullOrEmpty(carta.Mercancias.TransporteAereo.NumRegIdTribTranspor) && string.IsNullOrEmpty(carta.Mercancias.TransporteAereo.RFCTransportista) && carta.Mercancias.TransporteAereo.ResidenciaFiscalTransporSpecified == true)
                            {
                                OperacionesCatalogos o13 = new OperacionesCatalogos();
                                CatalogosSAT.c_Pais pais17 = o13.Consultar_PaisVerificacionLinea(carta.Mercancias.TransporteAereo.ResidenciaFiscalTranspor);
                                if (pais17 != null)
                                {
                                    if (pais17.ValidaciondelRIT != null)
                                    {
                                        Operaciones_IRFC r = new Operaciones_IRFC();
                                        vI_RFC t = r.Consultar_IRFC(carta.Mercancias.TransporteAereo.NumRegIdTribTranspor);
                                        if (t == null)
                                        {
                                            return "CP194 - El atributo \"TransporteAereo:NumRegIdTribTranspor\" no tiene un valor que exista en el registro del país indicado en el atributo \"TransporteAereo:ResidenciaFiscalTranspor\".";

                                        }
                                    }
                                    else if (carta.Mercancias.TransporteAereo.NumRegIdTribTranspor != null && pais17.FormatodeRIT != null && !Regex.Match(carta.Mercancias.TransporteAereo.NumRegIdTribTranspor, "^" + pais17.FormatodeRIT + "$").Success)
                                    {
                                        return "CP195 - El atributo \"TransporteAereo:NumRegIdTribTranspor\" no cumple con el patrón publicado en la columna \"Formato de registro de identidad tributaria\" del país indicado en el atributo \"TransporteAereo:ResidenciaFiscalTranspor\".";

                                    }
                                }
                                else
                                    return "CP194 - El atributo \"TransporteAereo:NumRegIdTribTranspor\" no tiene un valor que exista en el registro del país indicado en el atributo \"TransporteAereo:ResidenciaFiscalTranspor\".";

                            }

                            if (string.IsNullOrEmpty(carta.Mercancias.TransporteAereo.RFCTransportista))
                            {
                                if (carta.Mercancias.TransporteAereo.ResidenciaFiscalTransporSpecified == false)
                                    return "CP196 - El atributo  \"TransporteAereo:ResidenciaFiscalTranspor\" tiene una clave diferente a las establecidas en el catálogo \"catCFDI:c_Pais\".";
                                c_Pais myPais;
                                Enum.TryParse<c_Pais>(carta.Mercancias.TransporteAereo.ResidenciaFiscalTranspor, out myPais);
                                if (myPais.ToString() != carta.Mercancias.TransporteAereo.ResidenciaFiscalTranspor)
                                    return "CP196 - El atributo  \"TransporteAereo:ResidenciaFiscalTranspor\" tiene una clave diferente a las establecidas en el catálogo \"catCFDI:c_Pais\".";

                            }

                            if (string.IsNullOrEmpty(carta.Mercancias.TransporteAereo.NumRegIdTribEmbarc) && string.IsNullOrEmpty(carta.Mercancias.TransporteAereo.RFCEmbarcador) && carta.Mercancias.TransporteAereo.ResidenciaFiscalEmbarcSpecified == true)
                                return "CP197 - Los atributos \"TransporteAereo:RFCEmbarcador\" y \"TransporteAereo:NumRegIdTribEmbarc\" están vacíos, o ya existe el atributo \"RFCEmbarcador\".";
                            if (!string.IsNullOrEmpty(carta.Mercancias.TransporteAereo.NumRegIdTribEmbarc) && !string.IsNullOrEmpty(carta.Mercancias.TransporteAereo.RFCEmbarcador) && carta.Mercancias.TransporteAereo.ResidenciaFiscalEmbarcSpecified == true)
                                return "CP197 - Los atributos \"TransporteAereo:RFCEmbarcador\" y \"TransporteAereo:NumRegIdTribEmbarc\" están vacíos, o ya existe el atributo \"RFCEmbarcador\".";

                            if (!string.IsNullOrEmpty(carta.Mercancias.TransporteAereo.NumRegIdTribEmbarc) && string.IsNullOrEmpty(carta.Mercancias.TransporteAereo.RFCEmbarcador) && carta.Mercancias.TransporteAereo.ResidenciaFiscalEmbarcSpecified == true)
                            {
                                OperacionesCatalogos o13 = new OperacionesCatalogos();
                                CatalogosSAT.c_Pais pais17 = o13.Consultar_PaisVerificacionLinea(carta.Mercancias.TransporteAereo.ResidenciaFiscalEmbarc.ToString());
                                if (pais17 != null)
                                {
                                    if (pais17.ValidaciondelRIT != null)
                                    {

                                        Operaciones_IRFC r = new Operaciones_IRFC();
                                        vI_RFC t = r.Consultar_IRFC(carta.Mercancias.TransporteAereo.NumRegIdTribEmbarc);
                                        if (t == null)
                                        {
                                            return "CP198 - El atributo \"TransporteAereo:NumRegIdTribEmbarc\" no tiene un valor que exista en el registro del país indicado en el atributo \"TransporteAereo:ResidenciaFiscalEmbarc\".";

                                        }
                                    }
                                    else if (carta.Mercancias.TransporteAereo.NumRegIdTribEmbarc != null && pais17.FormatodeRIT != null && !Regex.Match(carta.Mercancias.TransporteAereo.NumRegIdTribEmbarc, "^" + pais17.FormatodeRIT + "$").Success)
                                    {
                                        return "CP199 - El atributo \"TransporteAereo:NumRegIdTribEmbarc\" no cumple con el patrón publicado en la columna \"Formato de registro de identidad tributaria\" del país indicado en el atributo \"TransporteAereo:ResidenciaFiscalEmbarc\".";

                                    }
                                }
                                else
                                    return "CP198 - El atributo \"TransporteAereo:NumRegIdTribEmbarc\" no tiene un valor que exista en el registro del país indicado en el atributo \"TransporteAereo:ResidenciaFiscalEmbarc\".";

                            }

                            if (string.IsNullOrEmpty(carta.Mercancias.TransporteAereo.RFCEmbarcador))
                            {
                                if (carta.Mercancias.TransporteAereo.ResidenciaFiscalEmbarcSpecified == false)
                                    return "CP200 - El atributo  \"TransporteAereo:ResidenciaFiscalEmbarc\" tiene una clave diferente a las establecidas en el catálogo \"catCFDI:c_Pais\".";
                                c_Pais myPais;
                                Enum.TryParse<c_Pais>(carta.Mercancias.TransporteAereo.ResidenciaFiscalEmbarc, out myPais);
                                if (myPais.ToString() != carta.Mercancias.TransporteAereo.ResidenciaFiscalEmbarc)
                                    return "CP200 - El atributo  \"TransporteAereo:ResidenciaFiscalEmbarc\" tiene una clave diferente a las establecidas en el catálogo \"catCFDI:c_Pais\".";

                            }
                        }
                     
                    }
                    if (carta.Mercancias.TransporteFerroviario != null)
                    {
                        if (carta.Mercancias.TransporteFerroviario.Concesionario == com.Emisor.Rfc)
                            return "CP201 - El valor registrado en el atributo \"Concesionario\" es el mismo que el RFC emisor del CFDI o no existe en la lista de contribuyentes no cancelados del SAT (l_RFC).";
                        Operaciones_IRFC r = new Operaciones_IRFC();
                        vI_RFC t = r.Consultar_IRFC(carta.Mercancias.TransporteFerroviario.Concesionario);
                        if (t == null)
                            return "CP201 - El valor registrado en el atributo \"Concesionario\" es el mismo que el RFC emisor del CFDI o no existe en la lista de contribuyentes no cancelados del SAT (l_RFC).";
                       
                        if (carta.Mercancias.TransporteFerroviario.Carro != null)
                        {
                            decimal tot = 0;  
                            foreach (var car in carta.Mercancias.TransporteFerroviario.Carro)
                            {
                                tot = 0;  
                                if (car.Contenedor != null)
                                    foreach (var con in car.Contenedor)
                                    {
                                        if (con.PesoNetoMercancia != null)
                                        { tot = tot + con.PesoNetoMercancia; }
                                    }
                              
                                if(car.ToneladasNetasCarro!=tot)
                                return "CP202 - El valor registrado en el atributo \"ToneladasNetasCarro\" no corresponde a la suma de los valores registrados en el atributo \"PesoNetoMercancia\".";

                             }
                           
                        }
                    }

                    if (carta.Mercancias.AutotransporteFederal != null && ( carta.FiguraTransporte!=null && carta.FiguraTransporte.Operadores == null))
                        return "CP203 - El nodo \"Operadores\" debe existir, siempre que exista el nodo  \"Mercancias:AutotransporteFederal\".";
                    if (carta.Mercancias.AutotransporteFederal == null && (carta.FiguraTransporte != null && carta.FiguraTransporte.Operadores != null))
                        return "CP203 - El nodo \"Operadores\" debe existir, siempre que exista el nodo  \"Mercancias:AutotransporteFederal\".";

                //inicio operador--------------------------------------------------------
                if(carta.FiguraTransporte!=null)
                if (carta.FiguraTransporte.Operadores != null)
                    {
                        
                        foreach (var oper in carta.FiguraTransporte.Operadores)
                        {

                            foreach (var op in oper.Operador)
                            {
                                //if (!string.IsNullOrEmpty(op.RFCOperador))
                                 //   if (!string.IsNullOrEmpty(op.NumRegIdTribOperador))
                                 //       return "CP206 - Existe información en el atributo \"Operador:RFCOperador\" o la clave del país registrada en el atributo \"Operador:ResidenciaFiscalOperador\" no corresponde con el valor registrado en el atributo \"Operador:NumRegIdTribOperador\".";
                            
                               if(op.ResidenciaFiscalOperadorSpecified==true && op.ResidenciaFiscalOperador=="MEX")
                                    if(string.IsNullOrEmpty(op.RFCOperador))
                                     return "CP204 - Se debe registrar información en el atributo \"Operador:RFCOperador\" o ya existe información en el atributo \"Operador:NumRegIdTribOperador\".";
                                
                                //if (!string.IsNullOrEmpty(op.RFCOperador) && !string.IsNullOrEmpty(op.NumRegIdTribOperador))
                                //    return "CP204 - Se debe registrar información en el atributo \"Operador:RFCOperador\" o ya existe información en el atributo \"Operador:NumRegIdTribOperador\".";
                                
                               // if (string.IsNullOrEmpty(op.RFCOperador) && string.IsNullOrEmpty(op.NumRegIdTribOperador))
                                //    return "CP204 - Se debe registrar información en el atributo \"Operador:RFCOperador\" o ya existe información en el atributo \"Operador:NumRegIdTribOperador\".";

                                if (!string.IsNullOrEmpty(op.RFCOperador))
                                {
                                    Operaciones_IRFC r = new Operaciones_IRFC();
                                    vI_RFC t = r.Consultar_IRFC(op.RFCOperador);
                                    if (t == null)
                                        return "CP205 - El valor registrado en el atributo \"Operador:RFCOperador\" no existe en la lista de RFC inscritos no cancelados en el SAT (l_RFC).";

                                }

                                if (op.ResidenciaFiscalOperadorSpecified == true && op.ResidenciaFiscalOperador != "MEX")
                                    if (!string.IsNullOrEmpty(op.RFCOperador))
                                        return "CP206 - Existe información en el atributo \"Operador:RFCOperador\" o la clave del país registrada en el atributo \"Operador:ResidenciaFiscalOperador\" no corresponde con el valor registrado en el atributo \"Operador:NumRegIdTribOperador\".";

                
                               // if (string.IsNullOrEmpty(op.RFCOperador) && (op.ResidenciaFiscalOperador.ToString() != op.NumRegIdTribOperador))
                               //      return "CP206 - Existe información en el atributo \"Operador:RFCOperador\" o la clave del país registrada en el atributo \"Operador:ResidenciaFiscalOperador\" no corresponde con el valor registrado en el atributo \"Operador:NumRegIdTribOperador\".";

                                 if (!string.IsNullOrEmpty(op.NumRegIdTribOperador) && string.IsNullOrEmpty(op.RFCOperador))
                                 {
                                     OperacionesCatalogos o13 = new OperacionesCatalogos();
                                     CatalogosSAT.c_Pais pais17 = o13.Consultar_PaisVerificacionLinea(op.ResidenciaFiscalOperador);
                                     if (pais17 != null)
                                     {
                                         if (pais17.ValidaciondelRIT != null)
                                         {
                                             Operaciones_IRFC r = new Operaciones_IRFC();
                                             vI_RFC t = r.Consultar_IRFC(op.NumRegIdTribOperador);
                                             if (t == null)
                                             {
                                                 return "CP207 - El atributo \"Operador:NumRegIdTribOperador\", no tiene un valor que exista en el registro del país indicado en el atributo \"Operador:ResidenciaFiscalOperador\".";
                                             }
                                         }
                                         else if (op.NumRegIdTribOperador != null && pais17.FormatodeRIT != null && !Regex.Match(op.NumRegIdTribOperador, "^" + pais17.FormatodeRIT + "$").Success)
                                         {
                                             return "CP208 - El atributo \"Operador:NumRegIdTribOperador\", no cumple con el patrón publicado en la columna \"Formato de registro de identidad tributaria\" del país indicado en el atributo \"Operador:ResidenciaFiscalOperador\".";
                                         }
                                     }
                                     else
                                         return "CP207 - El atributo \"Operador:NumRegIdTribOperador\", no tiene un valor que exista en el registro del país indicado en el atributo \"Operador:ResidenciaFiscalOperador\".";

                                     c_Pais myPais;
                                     Enum.TryParse<c_Pais>(op.ResidenciaFiscalOperador, out myPais);
                                     if (myPais.ToString() != op.ResidenciaFiscalOperador)
                                         return "CP209 - Los atributos \"Operador:ResidenciaFiscalOperador\" y \"Operador:NumRegIdTribOperador\" tienen una clave diferente a las establecidas en el catálogo \"catCFDI:c_Pais\" para su validación.";
                                  

                                 }
                                 CatalogosSAT.OperacionesCatalogos o2 = new CatalogosSAT.OperacionesCatalogos();
                               
                                 if (op.Domicilio != null)
                                {
                
                                 if (op.Domicilio.Pais == "MEX" || op.Domicilio.Pais == "CAN" || op.Domicilio.Pais == "USA")
                                 {
                                     CatalogosSAT.c_Estado estado5 = o2.Consultar_EstadosPais(op.Domicilio.Estado, op.Domicilio.Pais);
                                     if (estado5 == null)
                                         return ("CP214 - El atributo \"Operador:Domicilio:Estado\" no existe o no contiene un valor  o no corresponde con una clave del \"catCFDI:c_Estado\".");
                                 }

                                 c_Pais myPaiss;
                                 Enum.TryParse<c_Pais>(op.Domicilio.Pais, out myPaiss);
                                 if (myPaiss.ToString() != op.Domicilio.Pais)
                                     return "CP215 - El atributo \"Operador:Domicilio:Pais\" tiene un valor no permitido.";

                                 if (op.Domicilio.Pais == "MEX")
                                 {
                                     CatalogosSAT.OperacionesCatalogos o = new CatalogosSAT.OperacionesCatalogos();

                                     CatalogosSAT.c_Municipio municipio5 = o.Consultar_Municipio(op.Domicilio.Estado, op.Domicilio.Municipio);
                                     if (municipio5 == null)
                                         return ("CP213 - El atributo \"Operador:Domicilio:Municipio\" debe contener una clave del catálogo de \"catCFDI:c_Municipio\" donde la columna clave de \"c_Estado\" debe ser igual a la clave registrada en el atributo \"Estado\" si el atributo \"Pais\" tiene el valor \"MEX\".");


                                     if (string.IsNullOrEmpty(op.Domicilio.Colonia))
                                         return ("CP210 - El atributo \"Operador:Domicilio:Colonia\" debe contener una clave del catálogo de \"catCFDI:c_Colonia\", donde la columna \"c_CodigoPostal\" debe ser igual a la clave registrada en el atributo \"CodigoPostal\".");

                                     CatalogosSAT.c_Colonia colonia5 = o.Consultar_Colonia(op.Domicilio.CodigoPostal, op.Domicilio.Colonia);
                                     if (colonia5 == null)
                                         return ("CP210 - El atributo \"Operador:Domicilio:Colonia\" debe contener una clave del catálogo de \"catCFDI:c_Colonia\", donde la columna \"c_CodigoPostal\" debe ser igual a la clave registrada en el atributo \"CodigoPostal\".");
                                     if (string.IsNullOrEmpty(op.Domicilio.Localidad))
                                         return ("CP212 - La clave del atributo \"Operador:Domicilio:Localidad\" no corresponde a la clave del país registrado.");

                                     CatalogosSAT.c_Localidad localidad5 = o.Consultar_Localidad(op.Domicilio.Estado, op.Domicilio.Localidad);
                                     if (localidad5 == null)
                                         return ("CP212 - La clave del atributo \"Operador:Domicilio:Localidad\" no corresponde a la clave del país registrado.");
                                     if (string.IsNullOrEmpty(op.Domicilio.Municipio))
                                         return ("CP213 - El atributo \"Operador:Domicilio:Municipio\" debe contener una clave del catálogo de \"catCFDI:c_Municipio\" donde la columna clave de \"c_Estado\" debe ser igual a la clave registrada en el atributo \"Estado\" si el atributo \"Pais\" tiene el valor \"MEX\".");



                                     CatalogosSAT.c_CP CP5 = o.Consultar_CP(op.Domicilio.Estado, op.Domicilio.Municipio, op.Domicilio.Localidad, op.Domicilio.CodigoPostal);
                                     if (CP5 == null)
                                         return ("CP216 - El atributo \"Operador:Domicilio:CodigoPostal\" debe contener una clave del catálogo de \"catCFDI:c_CodigoPostal\", donde la columna clave de \"c_Estado\" debe ser igual a la clave registrada en el atributo \"Estado\", la columna clave de \"c_Municipio\" debe ser igual a la clave registrada en el atributo \"Municipio\", y si existe el atributo de \"Localidad\", la columna clave de \"c_Localidad\" debe ser igual a la clave registrada en el atributo \"Localidad\" si el atributo \"Pais\" tiene el valor \"MEX\".");
                                 }
                                         else
                                        {           if (!string.IsNullOrEmpty(op.Domicilio.Localidad))
                                                if (op.Domicilio.Localidad.All(char.IsDigit))
                                                    return ("CP211 - La clave del atributo \"Operador:Domicilio:Localidad\" no corresponde a la clave del país registrado.");
                                
                                           }
                                   }
                        
                            }
                       }
                    }//fin operador

                //inicio propietario
                     if (carta.FiguraTransporte!=null)
                      if (carta.FiguraTransporte.Propietario != null)
                       {
                           foreach (var pro in carta.FiguraTransporte.Propietario)
                           {
                              
                               if (pro.RFCPropietario == com.Emisor.Rfc && com.TipoDeComprobante == "I")
                                   return "CP217 - La información del propietario debe ser distinta a la del emisor del CFDI, o no debe existir este nodo.";
                               if (!string.IsNullOrEmpty(pro.RFCPropietario))
                               {
                                   Operaciones_IRFC r = new Operaciones_IRFC();
                                   vI_RFC t = r.Consultar_IRFC(pro.RFCPropietario);
                                   if (t == null)
                                   {
                                       return "CP218 - El valor capturado en el atributo \"Propietario:RFCPropietario\"  no existe en la lista de contribuyentes no cancelados del SAT (l_RFC).";
                                   }

                               }
                               if (!string.IsNullOrEmpty(pro.NumRegIdTribPropietario) && !string.IsNullOrEmpty(pro.RFCPropietario))
                                   return "CP219 - Existe información en el atributo \"Operador:RFCOperador\" o la clave del país registrada en el atributo \"Operador:ResidenciaFiscalOperador\" no corresponde con el valor registrado en el atributo \"Operador:NumRegIdTribOperador\".";
                 //              if (string.IsNullOrEmpty(pro.RFCPropietario) && (pro.ResidenciaFiscalPropietario.ToString() != pro.NumRegIdTribPropietario))
                 //                  return "CP219 - Existe información en el atributo \"Propietario:RFCPropietario\" o la clave del país registrada en el atributo \"Propietario:ResidenciaFiscalPropietario\" no corresponde con el valor registrado en el atributo \"Propietario:NumRegIdTribPropietario\".";

                               if (!string.IsNullOrEmpty(pro.NumRegIdTribPropietario) && string.IsNullOrEmpty(pro.RFCPropietario))
                               {
                                   OperacionesCatalogos o13 = new OperacionesCatalogos();
                                   CatalogosSAT.c_Pais pais17 = o13.Consultar_PaisVerificacionLinea(pro.ResidenciaFiscalPropietario);
                                   if (pais17 != null)
                                   {
                                       if (pais17.ValidaciondelRIT != null)
                                       {
                                           Operaciones_IRFC r = new Operaciones_IRFC();
                                           vI_RFC t = r.Consultar_IRFC(pro.NumRegIdTribPropietario);
                                           if (t == null)
                                           {
                                               return "CP220 - El atributo \"Propietario:NumRegIdTribPropietario\" no tiene un valor que exista en el registro del país indicado en el atributo \"Propietario:ResidenciaFiscalPropietario\".";
                                           }
                                       }
                                       else if (pro.NumRegIdTribPropietario != null && pais17.FormatodeRIT != null && !Regex.Match(pro.NumRegIdTribPropietario, "^" + pais17.FormatodeRIT + "$").Success)
                                       {
                                           return "CP221 - El atributo \"Operador:NumRegIdTribOperador\", no cumple con el patrón publicado en la columna \"Formato de registro de identidad tributaria\" del país indicado en el atributo \"Operador:ResidenciaFiscalOperador\".";
                                       }
                                   }
                                   else
                                       return "CP220 - El atributo \"Propietario:NumRegIdTribPropietario\" no tiene un valor que exista en el registro del país indicado en el atributo \"Propietario:ResidenciaFiscalPropietario\".";
                               }
                                   c_Pais myPais;
                                   Enum.TryParse<c_Pais>(pro.ResidenciaFiscalPropietario, out myPais);
                                   if (myPais.ToString() != pro.ResidenciaFiscalPropietario)
                                       return "CP222 - Los atributos \"Propietario:ResidenciaFiscalPropietario\" y \"Propietario:NumRegIdTribPropietario\" tienen una clave diferente a las establecidas en el catálogo \"catCFDI:c_Pais\" para su validación.";

                                    if (pro.Domicilio != null)
                                 {
                 
                                   if (pro.Domicilio.Pais == "MEX" || pro.Domicilio.Pais == "CAN" || pro.Domicilio.Pais == "USA")
                                   {
                                       CatalogosSAT.OperacionesCatalogos ox = new CatalogosSAT.OperacionesCatalogos();
                                    
                                       CatalogosSAT.c_Estado estado5 = ox.Consultar_EstadosPais(pro.Domicilio.Estado, pro.Domicilio.Pais);
                                       if (estado5 == null)
                                           return ("CP227 - El atributo \"Propietario:Domicilio:Estado\" no existe o no contiene un valor  o no corresponde con una clave del \"catCFDI:c_Estado\".");
                                   }

                                   Enum.TryParse<c_Pais>(pro.Domicilio.Pais, out myPais);
                                   if (myPais.ToString() != pro.Domicilio.Pais)
                                       return "CP228 - El atributo \"Propietario:Domicilio:Pais\" tiene un valor no permitido.";

                                   if (pro.Domicilio.Pais == "MEX")
                                   {
                                       CatalogosSAT.OperacionesCatalogos o = new CatalogosSAT.OperacionesCatalogos();
                                       if (string.IsNullOrEmpty(pro.Domicilio.Colonia))
                                           return ("CP223 - El atributo \"Propietario:Domicilio:Colonia\" debe contener una clave del catálogo de \"catCFDI:c_Colonia\", donde la columna \"c_CodigoPostal\" debe ser igual a la clave registrada en el atributo \"CodigoPostal\".");

                                       CatalogosSAT.c_Colonia colonia5 = o.Consultar_Colonia(pro.Domicilio.CodigoPostal, pro.Domicilio.Colonia);
                                       if (colonia5 == null)
                                           return ("CP223 - El atributo \"Propietario:Domicilio:Colonia\" debe contener una clave del catálogo de \"catCFDI:c_Colonia\", donde la columna \"c_CodigoPostal\" debe ser igual a la clave registrada en el atributo \"CodigoPostal\".");
                                       if (string.IsNullOrEmpty(pro.Domicilio.Localidad))
                                           return ("CP225 - El atributo \"Propietario:Domicilio:Localidad\" debe contener una clave del catálogo de \"catCFDI:c_Localidad\", donde la columna clave de \"c_Estado\" debe ser igual a la clave registrada en el atributo estado si el atributo \"Pais\" tiene el valor \"MEX\".");
                                       CatalogosSAT.c_Localidad localidad5 = o.Consultar_Localidad(pro.Domicilio.Estado, pro.Domicilio.Localidad);
                                       if (localidad5 == null)
                                           return ("CP225 - El atributo \"Propietario:Domicilio:Localidad\" debe contener una clave del catálogo de \"catCFDI:c_Localidad\", donde la columna clave de \"c_Estado\" debe ser igual a la clave registrada en el atributo estado si el atributo \"Pais\" tiene el valor \"MEX\".");
                                       if (string.IsNullOrEmpty(pro.Domicilio.Municipio))
                                           return ("CP226 - El atributo \"Propietario:Domicilio:Municipio\" debe contener una clave del catálogo de \"catCFDI:c_Municipio\" donde la columna clave de \"c_Estado\" debe ser igual a la clave registrada en el atributo \"Estado\" si el atributo \"Pais\" tiene el valor \"MEX\".");

                                       CatalogosSAT.c_Municipio municipio5 = o.Consultar_Municipio(pro.Domicilio.Estado, pro.Domicilio.Municipio);
                                       if (municipio5 == null)
                                           return ("CP226 - El atributo \"Propietario:Domicilio:Municipio\" debe contener una clave del catálogo de \"catCFDI:c_Municipio\" donde la columna clave de \"c_Estado\" debe ser igual a la clave registrada en el atributo \"Estado\" si el atributo \"Pais\" tiene el valor \"MEX\".");


                                       CatalogosSAT.c_CP CP5 = o.Consultar_CP(pro.Domicilio.Estado, pro.Domicilio.Municipio, pro.Domicilio.Localidad, pro.Domicilio.CodigoPostal);
                                       if (CP5 == null)
                                           return ("CP229 - El atributo \"Propietario:Domicilio:CodigoPostal\" debe contener una clave del catálogo de \"catCFDI:c_CodigoPostal\", donde la columna clave de \"c_Estado\" debe ser igual a la clave registrada en el atributo \"Estado\", la columna clave de \"c_Municipio\" debe ser igual a la clave registrada en el atributo \"Municipio\", y si existe el atributo de \"Localidad\", la columna clave de \"c_Localidad\" debe ser igual a la clave registrada en el atributo \"Localidad\" si el atributo \"Pais\" tiene el valor \"MEX\".");
                                   }
                                   else
                                   { 
                                      if (!string.IsNullOrEmpty(pro.Domicilio.Localidad))
                                                if (pro.Domicilio.Localidad.All(char.IsDigit))
                                                    return ("CP224 - La clave del atributo \"Propietario:Domicilio:Localidad\" no corresponde a la clave del país registrado.");
                               
                                   }
                                   }
                    
                           

                      }

                      }//fin propirtario----------------------------
                //inicio arredatario----------------------------------
                if (carta.FiguraTransporte!=null)
                if (carta.FiguraTransporte.Arrendatario != null)
                       {
                           if(carta.FiguraTransporte.Arrendatario!=null)
                               foreach (var arr in carta.FiguraTransporte.Arrendatario)
                               {
                                
                                   if (arr.RFCArrendatario == com.Emisor.Rfc && com.TipoDeComprobante == "I")
                                       return "CP230 - La información del arrendatario es igual a la del emisor del comprobante o no existe el nodo \"Arrendatario\".";
                                   if (!string.IsNullOrEmpty(arr.RFCArrendatario))
                                   {
                                       Operaciones_IRFC r = new Operaciones_IRFC();
                                       vI_RFC t = r.Consultar_IRFC(arr.RFCArrendatario);
                                       if (t == null)
                                       {
                                           return "CP231 - El valor capturado en el atributo \"Arrendatario:RFCArrendatario\" no existe en la lista de RFC inscritos no cancelados del SAT (l_RFC).";
                                       }
                                   }
                                   if (!string.IsNullOrEmpty(arr.NumRegIdTribArrendatario) && !string.IsNullOrEmpty(arr.RFCArrendatario))
                                       return "CP232 - Existe información en el atributo \"Arrendatario:RFCArrendatario\" o la clave del país registrada en el atributo \"Arrendatario:ResidenciaFiscalArrendatario\" no corresponde con el valor registrado en el atributo \"Arrendatario:NumRegIdTribArrendatario\".";
                    //               if (string.IsNullOrEmpty(arr.RFCArrendatario) && (arr.RFCArrendatario.ToString() != arr.NumRegIdTribArrendatario))
                    //                   return "CP232 - Existe información en el atributo \"Arrendatario:RFCArrendatario\" o la clave del país registrada en el atributo \"Arrendatario:ResidenciaFiscalArrendatario\" no corresponde con el valor registrado en el atributo \"Arrendatario:NumRegIdTribArrendatario\".";

                                   if (!string.IsNullOrEmpty(arr.NumRegIdTribArrendatario) && string.IsNullOrEmpty(arr.RFCArrendatario))
                                   {
                                       OperacionesCatalogos o13 = new OperacionesCatalogos();
                                       CatalogosSAT.c_Pais pais17 = o13.Consultar_PaisVerificacionLinea(arr.ResidenciaFiscalArrendatario);
                                       if (pais17 != null)
                                       {
                                           if (pais17.ValidaciondelRIT != null)
                                           {
                                               Operaciones_IRFC r = new Operaciones_IRFC();
                                               vI_RFC t = r.Consultar_IRFC(arr.NumRegIdTribArrendatario);
                                               if (t == null)
                                               {
                                                   return "CP233 - El  atributo \"Arrendatario:NumRegIdTribArrendatario\" no tiene un valor que exista en el registro del país indicado en el atributo \"Arrendatario:ResidenciaFiscalArrendatario\".";
                                               }
                                           }
                                           else if (arr.NumRegIdTribArrendatario != null && pais17.FormatodeRIT != null && !Regex.Match(arr.NumRegIdTribArrendatario, "^" + pais17.FormatodeRIT + "$").Success)
                                           {
                                               return "CP234 - El  atributo \"Arrendatario:NumRegIdTribArrendatario\" no cumple con el patrón publicado en la columna \"Formato de Registro de Identidad Tributaria\" del país indicado en el atributo \"Arrendatario:ResidenciaFiscalArrendatario\".";
                                           }
                                       }
                                       else
                                           return "CP233 - El  atributo \"Arrendatario:NumRegIdTribArrendatario\" no tiene un valor que exista en el registro del país indicado en el atributo \"Arrendatario:ResidenciaFiscalArrendatario\".";
                                   }

                                       c_Pais myPais;
                                       Enum.TryParse<c_Pais>(arr.ResidenciaFiscalArrendatario, out myPais);
                                       if (myPais.ToString() != arr.ResidenciaFiscalArrendatario)
                                           return "CP235 - Los atributos \"Arrendatario:ResidenciaFiscalArrendatario\" y \"Arrendatario:NumRegIdTribArrendatario\" tienen una clave diferente a las establecidas en el catálogo \"catCFDI:c_Pais\" para su validación.";
                                        if (arr.Domicilio != null)
                                           {
                
                                       if (arr.Domicilio.Pais == "MEX" || arr.Domicilio.Pais == "CAN" || arr.Domicilio.Pais == "USA")
                                       {
                                           CatalogosSAT.OperacionesCatalogos ox = new CatalogosSAT.OperacionesCatalogos();

                                           CatalogosSAT.c_Estado estado5 = ox.Consultar_EstadosPais(arr.Domicilio.Estado, arr.Domicilio.Pais);
                                           if (estado5 == null)
                                               return ("CP240 - El atributo \"Arrendatario:Domicilio:Estado\" no existe o no contiene un valor  o no corresponde con una clave del \"catCFDI:c_Estado\".");
                                       }

                                       Enum.TryParse<c_Pais>(arr.Domicilio.Pais, out myPais);
                                       if (myPais.ToString() != arr.Domicilio.Pais)
                                           return "CP241 - El atributo \"Arrendatario:Domicilio:Pais\" tiene un valor no permitido.";

                                       if (arr.Domicilio.Pais == "MEX")
                                       {
                                           CatalogosSAT.OperacionesCatalogos o = new CatalogosSAT.OperacionesCatalogos();

                                           CatalogosSAT.c_Municipio municipio5 = o.Consultar_Municipio(arr.Domicilio.Estado, arr.Domicilio.Municipio);
                                           if (municipio5 == null)
                                               return ("CP239 - El atributo \"Arrendatario:Domicilio:Municipio\" debe contener una clave del catálogo de \"catCFDI:c_Municipio\" donde la columna clave de \"c_Estado\" debe ser igual a la clave registrada en el atributo \"Estado\" si el atributo \"Pais\" tiene el valor \"MEX\".");


                                           if (string.IsNullOrEmpty(arr.Domicilio.Colonia))
                                               return ("CP236 - El atributo \"Arrendatario:Domicilio:Colonia\" debe contener una clave del catálogo de \"catCFDI:c_Colonia\", donde la columna \"c_CodigoPostal\" debe ser igual a la clave registrada en el atributo \"CodigoPostal\".");

                                           CatalogosSAT.c_Colonia colonia5 = o.Consultar_Colonia(arr.Domicilio.CodigoPostal, arr.Domicilio.Colonia);
                                           if (colonia5 == null)
                                               return ("CP236 - El atributo \"Arrendatario:Domicilio:Colonia\" debe contener una clave del catálogo de \"catCFDI:c_Colonia\", donde la columna \"c_CodigoPostal\" debe ser igual a la clave registrada en el atributo \"CodigoPostal\".");
                                           if (string.IsNullOrEmpty(arr.Domicilio.Localidad))
                                               return ("CP238 - El atributo \"Arrendatario:Domicilio:Localidad\" debe contener una clave del catálogo de \"catCFDI:c_Localidad\", donde la columna clave de \"c_Estado\" debe ser igual a la clave registrada en el atributo estado si el atributo \"Pais\" tiene el valor \"MEX\".");

                                           CatalogosSAT.c_Localidad localidad5 = o.Consultar_Localidad(arr.Domicilio.Estado, arr.Domicilio.Localidad);
                                           if (localidad5 == null)
                                               return ("CP238 - El atributo \"Arrendatario:Domicilio:Localidad\" debe contener una clave del catálogo de \"catCFDI:c_Localidad\", donde la columna clave de \"c_Estado\" debe ser igual a la clave registrada en el atributo estado si el atributo \"Pais\" tiene el valor \"MEX\".");
                                           if (string.IsNullOrEmpty(arr.Domicilio.Municipio))
                                               return ("CP239 - El atributo \"Arrendatario:Domicilio:Municipio\" debe contener una clave del catálogo de \"catCFDI:c_Municipio\" donde la columna clave de \"c_Estado\" debe ser igual a la clave registrada en el atributo \"Estado\" si el atributo \"Pais\" tiene el valor \"MEX\".");

                                           CatalogosSAT.c_CP CP5 = o.Consultar_CP(arr.Domicilio.Estado, arr.Domicilio.Municipio, arr.Domicilio.Localidad, arr.Domicilio.CodigoPostal);
                                           if (CP5 == null)
                                               return ("CP242 - El atributo \"Arrendatario:Domicilio:CodigoPostal\" debe contener una clave del catálogo de \"catCFDI:c_CodigoPostal\", donde la columna clave de \"c_Estado\" debe ser igual a la clave registrada en el atributo \"Estado\", la columna clave de \"c_Municipio\" debe ser igual a la clave registrada en el atributo \"Municipio\", y si existe el atributo de \"Localidad\", la columna clave de \"c_Localidad\" debe ser igual a la clave registrada en el atributo \"Localidad\" si el atributo \"Pais\" tiene el valor \"MEX\".");
                                       }
                                       else
                                       { 
                                        if (!string.IsNullOrEmpty(arr.Domicilio.Localidad))
                                                if (arr.Domicilio.Localidad.All(char.IsDigit))
                                                    return ("CP237 - La clave del atributo \"Arrendatario:Domicilio:Localidad\" no corresponde a la clave del país registrado.");

                                       }

                                       }
                                 
                               }

                         }// fin arrendatario-----------------------------------
                       
                              //inicio notificado--------------------------------------
                if (carta.FiguraTransporte!=null)
                       if (carta.FiguraTransporte.Notificado != null)
                       {                         

                               foreach (var not in carta.FiguraTransporte.Notificado)
                               {
                                   if (carta.Mercancias.TransporteMaritimo != null && string.IsNullOrEmpty(not.NumRegIdTribNotificado) && string.IsNullOrEmpty(not.RFCNotificado))
                                       return "CP243 - Se debe registrar información en el atributo \"Notificado:RFCNotificado\" o ya existe información en el atributo \"Notificado:NumRegIDTribNotificado\".";
                                   //if (carta.Mercancias.TransporteMaritimo != null && !string.IsNullOrEmpty(not.NumRegIdTribNotificado) && !string.IsNullOrEmpty(not.RFCNotificado))
                                   //    return "CP243 - Se debe registrar información en el atributo \"Notificado:RFCNotificado\" o ya existe información en el atributo \"Notificado:NumRegIDTribNotificado\".";
                                   if (!string.IsNullOrEmpty(not.RFCNotificado))
                                   {
                                       Operaciones_IRFC r = new Operaciones_IRFC();
                                       vI_RFC t = r.Consultar_IRFC(not.RFCNotificado);
                                       if (t == null)
                                       {
                                           return "CP244 - El valor capturado en el atributo \"Notificado:RFCNotificado\" no existe en la lista de RFC inscritos no cancelados del SAT (l_RFC).";
                                       }

                                   }
                                   if (!string.IsNullOrEmpty(not.NumRegIdTribNotificado) && !string.IsNullOrEmpty(not.RFCNotificado))
                                       return "CP245 - Existe información en el atributo \"Notificado:RFCNotificado\" o la clave del país registrada en el atributo \"ResidenciaFiscalNotificado\" no corresponde con el valor registrado en el atributo \"NumRegIdTribNotificado\".";
                                 //  if (string.IsNullOrEmpty(not.RFCNotificado) && (not.RFCNotificado.ToString() != not.NumRegIdTribNotificado))
                                  //     return "CP245 - Existe información en el atributo \"Notificado:RFCNotificado\" o la clave del país registrada en el atributo \"ResidenciaFiscalNotificado\" no corresponde con el valor registrado en el atributo \"NumRegIdTribNotificado\".";

                                   if (!string.IsNullOrEmpty(not.NumRegIdTribNotificado))
                                   {
                                       OperacionesCatalogos o13 = new OperacionesCatalogos();
                                       CatalogosSAT.c_Pais pais17 = o13.Consultar_PaisVerificacionLinea(not.ResidenciaFiscalNotificado);
                                       if (pais17 != null)
                                       {
                                           if (pais17.ValidaciondelRIT != null)
                                           {
                                               Operaciones_IRFC r = new Operaciones_IRFC();
                                               vI_RFC t = r.Consultar_IRFC(not.NumRegIdTribNotificado);
                                               if (t == null)
                                               {
                                                   return "CP246 - El atributo \"Notificado:NumRegIdTribNotificado\" no contiene un valor que exista en el registro del país indicado en el atributo \"Notificado:ResidenciaFiscalNotificado\".";
                                               }
                                           }
                                           else if (not.NumRegIdTribNotificado != null && pais17.FormatodeRIT != null && !Regex.Match(not.NumRegIdTribNotificado, "^" + pais17.FormatodeRIT + "$").Success)
                                           {
                                               return "CP247 - El atributo \"Notificado:NumRegIdTribNotificado\" no cumple con el patrón publicado en la columna \"Formato de registro de identidad tributaria\" del país indicado en el atributo \"Notificado:ResidenciaFiscalNotificado\".";
                                           }
                                       }
                                       else
                                            return "CP246 - El atributo \"Notificado:NumRegIdTribNotificado\" no contiene un valor que exista en el registro del país indicado en el atributo \"Notificado:ResidenciaFiscalNotificado\".";
                                   }
                                     c_Pais myPais;
                                       Enum.TryParse<c_Pais>(not.ResidenciaFiscalNotificado, out myPais);
                                       if (myPais.ToString() != not.ResidenciaFiscalNotificado)
                                           return "CP248 - Los atributos \"Notificado:ResidenciaFiscalNotificado\" y \"Notificado:NumRegIdTribNotificado\" tienen una clave diferente a las establecidas en el catálogo \"catCFDI:c_Pais\" para su validación.";
                                    if (not.Domicilio != null)
                                 {
                
                                       if (not.Domicilio.Pais == "MEX" || not.Domicilio.Pais == "CAN" || not.Domicilio.Pais == "USA")
                                       {
                                           CatalogosSAT.OperacionesCatalogos ox = new CatalogosSAT.OperacionesCatalogos();

                                           CatalogosSAT.c_Estado estado5 = ox.Consultar_EstadosPais(not.Domicilio.Estado, not.Domicilio.Pais);
                                           if (estado5 == null)
                                               return ("CP253 - El atributo \"Notificado:Domicilio:Estado\" no existe o no contiene un valor  o no corresponde con una clave del \"catCFDI:c_Estado\".");
                                       }

                                       Enum.TryParse<c_Pais>(not.Domicilio.Pais, out myPais);
                                       if (myPais.ToString() != not.Domicilio.Pais.ToString())
                                           return "CP254 - El atributo \"Notificado:Domicilio:Pais\" tiene un valor no permitido.";

                                       if (not.Domicilio.Pais == "MEX")
                                       {
                                           CatalogosSAT.OperacionesCatalogos o = new CatalogosSAT.OperacionesCatalogos();

                                           CatalogosSAT.c_Municipio municipio5 = o.Consultar_Municipio(not.Domicilio.Estado, not.Domicilio.Municipio);
                                           if (municipio5 == null)
                                               return ("CP252 - El atributo \"Notificado:Domicilio:Municipio\" debe contener una clave del catálogo de \"catCFDI:c_Municipio\" donde la columna clave de \"c_Estado\" debe ser igual a la clave registrada en el atributo \"Estado\" si el atributo \"Pais\" tiene el valor \"MEX\".");

                                           if (string.IsNullOrEmpty(not.Domicilio.Colonia))
                                               return ("CP249 - El atributo \"Notificado:Domicilio:Colonia\" debe contener una clave del catálogo de \"catCFDI:c_Colonia\", donde la columna \"c_CodigoPostal\" debe ser igual a la clave registrada en el atributo \"CodigoPostal\".");

                                           CatalogosSAT.c_Colonia colonia5 = o.Consultar_Colonia(not.Domicilio.CodigoPostal.ToString(), not.Domicilio.Colonia);
                                           if (colonia5 == null)
                                               return ("CP249 - El atributo \"Notificado:Domicilio:Colonia\" debe contener una clave del catálogo de \"catCFDI:c_Colonia\", donde la columna \"c_CodigoPostal\" debe ser igual a la clave registrada en el atributo \"CodigoPostal\".");
                                           if (string.IsNullOrEmpty(not.Domicilio.Localidad))
                                               return ("CP251 - El atributo \"Notificado:Domicilio:Localidad\" debe contener una clave del catálogo de \"catCFDI:c_Localidad\", donde la columna clave de \"c_Estado\" debe ser igual a la clave registrada en el atributo estado si el atributo \"Pais\" tiene el valor \"MEX\".");

                                           CatalogosSAT.c_Localidad localidad5 = o.Consultar_Localidad(not.Domicilio.Estado, not.Domicilio.Localidad);
                                           if (localidad5 == null)
                                               return ("CP251 - El atributo \"Notificado:Domicilio:Localidad\" debe contener una clave del catálogo de \"catCFDI:c_Localidad\", donde la columna clave de \"c_Estado\" debe ser igual a la clave registrada en el atributo estado si el atributo \"Pais\" tiene el valor \"MEX\".");
                                           if (string.IsNullOrEmpty(not.Domicilio.Municipio))
                                               return ("CP252 - El atributo \"Notificado:Domicilio:Municipio\" debe contener una clave del catálogo de \"catCFDI:c_Municipio\" donde la columna clave de \"c_Estado\" debe ser igual a la clave registrada en el atributo \"Estado\" si el atributo \"Pais\" tiene el valor \"MEX\".");


                                           CatalogosSAT.c_CP CP5 = o.Consultar_CP(not.Domicilio.Estado, not.Domicilio.Municipio, not.Domicilio.Localidad, not.Domicilio.CodigoPostal);
                                           if (CP5 == null)
                                               return ("CP255 - El atributo \"Notificado:Domicilio:CodigoPostal\" debe contener una clave del catálogo de \"catCFDI:c_CodigoPostal\", donde la columna clave de \"c_Estado\" debe ser igual a la clave registrada en el atributo \"Estado\", la columna clave de \"c_Municipio\" debe ser igual a la clave registrada en el atributo \"Municipio\", y si existe el atributo de \"Localidad\", la columna clave de \"c_Localidad\" debe ser igual a la clave registrada en el atributo \"Localidad\" si el atributo \"Pais\" tiene el valor \"MEX\".");
                                       }
                                       else
                                            if (!string.IsNullOrEmpty(not.Domicilio.Localidad))
                                                if (not.Domicilio.Localidad.All(char.IsDigit))
                                                return ("CP250 - La clave del atributo \"Notificado:Domicilio:Localidad\" no corresponde a la clave del país registrado.");

                                       }
                               }
                       } //fin notificado
                //--------------------------------


                return "0";
            }
            catch (Exception ex)
            {
                return "CP00 - Error en los datos de la carta porte:" +ex.Message;
            }
        }
        //-------------------------------------------------------------------------------------
       

    }

}