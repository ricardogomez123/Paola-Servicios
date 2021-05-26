

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


namespace ServicioLocal.Business
{
    public class ValidarCartaP : NtLinkBusiness
    {


        public ValidarCartaP()
        {
            XmlConfigurator.Configure();
        }
        //---------------------------------------------------------------------------------------------
        public string ProcesarCarta(CartaPorte carta, Comprobante com)
        {
            try
            {
                if (com.Version != "3.3")
                    return ("CP101 - El valor registrado en este atributo es diferente a \"3.3\"");

                if (carta == null)
                    return ("CP113 - El nodo \"CartaPorte\" no se registró como nodo hijo del nodo complemento del CFDI.");

                if (com.TipoDeComprobante == "T")
                {
                    if (Convert.ToDecimal(com.Subtotal) != 0)
                        return ("CP102 - El valor registrado en el atributo \"Subtotal\" es diferente de cero o el valor del atributo \"TipoDeComprobante\" es diferente de \"T\".");
                    if (com.Moneda != "XXX")
                        return ("CP104 - El valor registrado en el atributo \"Moneda\" es diferente de \"XXX\".");
                    if (com.Total != 0)
                        return ("CP106 - El valor registrado en el atributo \"Total\" es diferente de cero o el valor del atributo \"TipoDeComprobante\" es diferente de \"T\".");

                    if (com.Receptor.Rfc != "XAXX010101000")
                    {
                        Operaciones_IRFC r = new Operaciones_IRFC();
                        vI_RFC t = r.Consultar_IRFC(com.Receptor.Rfc);
                        if (t == null)
                        {
                            return "CP110 - El valor registrado en el atributo \"RFC\" no es \"XAXX010101000\" o bien no existe en la lista de RFC inscritos no cancelados en el SAT (l_RFC).";
                        }
                    }

                    if(com.Receptor.UsoCFDI!="P01")
                        return "CP112 - El valor registrado en el atributo \"UsoCFDI\", es diferente de \"P01\" (Por definir).";
        
                }
                if (com.TipoDeComprobante == "I")
                {
                    if (Convert.ToDecimal(com.Subtotal) < 0)
                        return ("CP103 - El valor registrado en el atributo \"TipoDeComprobante\" es diferente de \"I\".");
                    if (com.Moneda == "XXX")
                        return ("CP105 - El valor registrado en el atributo \"Moneda\" es igual a \"XXX\".");
                    if (com.Total < 0)
                        return ("CP107 - El valor registrado en el atributo \"TipoDeComprobante\" es diferente de \"I\".");
                    if (com.CfdiRelacionados == null || com.CfdiRelacionados.CfdiRelacionado == null)
                        return ("CP108 - El valor del atributo \"TipoDeComprobante\" es diferente de \"I\", o el valor registrado en el atributo \"TipoEstacion\" es diferente de \"02\" \"Intermedia\", o el nodo registrado en \"Mercancias\" es diferente de \"TransporteFerroviario\".");
                    foreach (var ubi in carta.Ubicaciones)
                    {
                        if (ubi.TipoEstacionSpecified == true)
                        {
                            if (ubi.TipoEstacion != c_TipoEstacion.Item02)
                                return ("CP108 - El valor del atributo \"TipoDeComprobante\" es diferente de \"I\", o el valor registrado en el atributo \"TipoEstacion\" es diferente de \"02\" \"Intermedia\", o el nodo registrado en \"Mercancias\" es diferente de \"TransporteFerroviario\".");
                        }
                        else
                            return ("CP108 - El valor del atributo \"TipoDeComprobante\" es diferente de \"I\", o el valor registrado en el atributo \"TipoEstacion\" es diferente de \"02\" \"Intermedia\", o el nodo registrado en \"Mercancias\" es diferente de \"TransporteFerroviario\".");

                    }
                    if (carta.Mercancias.TransporteFerroviario == null)
                        return ("CP108 - El valor del atributo \"TipoDeComprobante\" es diferente de \"I\", o el valor registrado en el atributo \"TipoEstacion\" es diferente de \"02\" \"Intermedia\", o el nodo registrado en \"Mercancias\" es diferente de \"TransporteFerroviario\".");

                    if (com.CfdiRelacionados.TipoRelacion != "05")
                        return ("CP109 - El valor registrado en el atributo \"TipoRelacion\" es diferente de \"05\" \"Traslados de mercancías facturados previamente\" y el valor registrado en el atributo \"Fecha\" contiene una diferencia menor a 6:00:00 horas respecto de la factura inicial a la cual se relaciona, siempre que se cuente con dicha información.");

                    if (com.Receptor.Rfc != "XAXX010101000" && com.Receptor.Rfc != "XEXX010101000")
                    {
                        Operaciones_IRFC r = new Operaciones_IRFC();
                        vI_RFC t = r.Consultar_IRFC(com.Receptor.Rfc);
                        if (t == null)
                        {
                            return "CP111 - El valor registrado en el atributo \"RFC\" no es \"XAXX010101000\" o \"XEXX010101000\", o bien no existe en la lista de RFC inscritos no cancelados en el SAT (l_RFC).";
                           
                        }
                    }
                }
                //---------------------------------------
                if(com.Complemento.Any.Count>1)
                {   int d=0; 
                        d= com.Complemento.Any.Where(p => p.LocalName == "CartaPorte").Count();
                if(d>1)
                    return "CP114 - Existe más de un nodo \"CartaPorte\".";

                var x = com.Complemento.Any.FirstOrDefault(p => p.LocalName != "CartaPorte" && p.LocalName != "TimbreFiscalDigital");
                    if(x!=null)
                    return "CP115 - El complemento registrado de manera adicional, no corresponde con los complemento con los cuales puede coexistir.";

                }
                if (com.TipoDeComprobante != "T" && com.TipoDeComprobante != "I")
                    return "CP116 - El valor registrado en \"TipoDeComprobante\" no puede convivir con el complemento CartaPorte.";
                if (carta.TranspInternac == CartaPorteTranspInternac.Sí)
                {
                    if (carta.ViaEntradaSalidaSpecified == false || carta.EntradaSalidaMercSpecified == false)
                        return "CP117 - Se debe registrar información en el atributo \"EntradaSalidaMerc\" y \"ViaEntradaSalida\".";
                    if (com.TipoDeComprobante == "I" && carta.EntradaSalidaMerc == CartaPorteEntradaSalidaMerc.Entrada && carta.Mercancias.AutotransporteFederal != null)
                    {
                        if (com.Conceptos[0].InformacionAduanera == null || com.Impuestos.Retenciones == null || com.Impuestos.Traslados == null)
                            return "CP118 - Se debe registrar información en el nodo \"InformacionAduanera\", nodo \"Impuestos:Traslados\",  y en el nodo \"Impuestos:Retenciones\", siempre que el valor en el atributo \"EntradaSalidaMerc\" sea \"Entrada\", el tipo de comprobante sea \"I\" y exista el nodo \"Mercancias:AutotransporteFederal\".";

                    }
                }
                else
                {
                    if (carta.ViaEntradaSalidaSpecified == true || carta.EntradaSalidaMercSpecified == true)
                        return "CP119 - No se debe registrar información en el atributo \"EntradaSalidaMerc\" y \"ViaEntradaSalida\".";
                    if (com.TipoDeComprobante == "I" && carta.Mercancias.AutotransporteFederal != null)
                    {
                        if ( com.Impuestos.Retenciones == null || com.Impuestos.Traslados == null)
                            return "CP120 - Se debe registrar información en el nodo \"Impuestos:Traslados\",  y en el nodo \"Impuestos:Retenciones\", siempre que  el tipo de comprobante sea \"I\" y exista el nodo \"Mercancias:AutotransporteFederal\".";

                    }
                
                }
                if (carta.Mercancias.AutotransporteFederal == null || carta.Mercancias.TransporteFerroviario == null)
                {
                    if (carta.TotalDistRecSpecified == true)
                        return "CP121 - El valor registrado en el nodo \"Mercancia\" es diferente a \"TransporteFerroviario\" o \"AutotransporteFederal\".";
                }
                else
                {
                    if (carta.TotalDistRecSpecified == false)
                        return "CP121 - El valor registrado en el nodo \"Mercancia\" es diferente a \"TransporteFerroviario\" o \"AutotransporteFederal\".";
                
                }
                //if (carta.TotalDistRecSpecified == true)
                //{
                //    decimal distancia=0;
                //    foreach (var ubi in carta.Ubicaciones)
                //    {
                        
                //        if(ubi.DistanciaRecorridaSpecified)
                //        distancia = distancia + ubi.DistanciaRecorrida;

                //    }
                //}
                if ( carta.Mercancias.TransporteFerroviario != null && com.CfdiRelacionados==null)
                 if( carta.Ubicaciones.Count()!=7)
                     return "CP123 - El número de nodos \"Ubicación\" registrados no corresponde a \"7\".";

                if (carta.Mercancias.TransporteFerroviario != null && com.CfdiRelacionados != null)
                    if (carta.Ubicaciones.Count() != 6)
                        return "CP124 - El número de nodos \"Ubicación\" registrados no corresponde a \"6\".";

                if (carta.TranspInternac == CartaPorteTranspInternac.Sí)
                {
                    foreach (var ubi in carta.Ubicaciones)
                    {
                        if (ubi.TipoEstacionSpecified == true)
                          return "CP127 - El valor registrado en el atributo \"TranspInternac\" contiene un valor \"Sí\", o el valor del país registrado en los nodos \"Ubicacion:Origen\" o \"Ubicacion:Destino\" contiene el valor \"MEX\".";
                        if(ubi.Domicilio.Pais== global::c_Pais.MEX)
                            return "CP127 - El valor registrado en el atributo \"TranspInternac\" contiene un valor \"Sí\", o el valor del país registrado en los nodos \"Ubicacion:Origen\" o \"Ubicacion:Destino\" contiene el valor \"MEX\".";
             
                    }

                }

                if (carta.Mercancias.AutotransporteFederal != null || carta.Mercancias.TransporteMaritimo != null || carta.Mercancias.TransporteAereo != null)
                {
                    if (carta.Ubicaciones.Count() != 2)
                        return "CP125 - El número de nodos de \"Ubicación\" es menor a \"2\", o no existe el nodo \"Origen\" y/o \"Destino\".";
                    foreach (var ubi in carta.Ubicaciones)
                    {

                        if (ubi.TipoEstacionSpecified == true)
                        {
                            c_TipoEstacion myTipoEstacion;
                            Enum.TryParse<c_TipoEstacion>(ubi.TipoEstacion.ToString(), out myTipoEstacion);
                            if (myTipoEstacion.ToString() != ubi.TipoEstacion.ToString())
                                return "CP126 - La clave registrada en el atributo \"TipoEstacion\" es diferente a las contenidas en el catálogo \"c_TipoEstacion\".";

                        }
                        else
                            return "CP126 - La clave registrada en el atributo \"TipoEstacion\" es diferente a las contenidas en el catálogo \"c_TipoEstacion\".";

                    }
                }
                if (carta.Mercancias.AutotransporteFederal != null || carta.Mercancias.TransporteFerroviario != null)
                {
                    foreach (var ubi in carta.Ubicaciones)
                    { 
                       
                       if(ubi.Destino!=null)
                           if(ubi.DistanciaRecorridaSpecified==false)
                           return "CP128 - No se capturo información en el atributo \"DistanciaRecorrida\".";
                      

                    }
                }
                foreach (var mer in carta.Mercancias.Mercancia)
                {
                    if (mer.CantidadTransporta != null)
                    {
                        bool si = false;
                        foreach (var ub in carta.Ubicaciones)
                        {
                            if (!string.IsNullOrEmpty(ub.Origen.IDOrigen))
                            { si = true; break; }
                        }
                        if (si)
                            break;
                        else
                            return "CP130 - Existe el nodo \"Mercancias:Mercancia:CantidadTransporta\" y no existe el atributo \"Ubicaciones:Ubicacion:Origen:IDOrigen\".";

                    }
                }
                foreach (var mer in carta.Mercancias.Mercancia)
                {
                    if (mer.CantidadTransporta != null)
                    {
                        bool si = false;
                        foreach (var ub in carta.Ubicaciones)
                        {
                            if (!string.IsNullOrEmpty(ub.Destino.IDDestino))
                            { si = true; break; }
                        }
                        if (si)
                            break;
                        else
                            return "CP143 - Existe el nodo \"Mercancias:Mercancia:CantidadTransporta\" y no existe el atributo \"Ubicaciones:Ubicacion:Destino:IDDestino\".";

                    }
                }
                foreach (var ubi in carta.Ubicaciones)
                {
                    if (!string.IsNullOrEmpty(ubi.Origen.NumRegIdTrib) && !string.IsNullOrEmpty(ubi.Origen.RFCRemitente))
                        return "CP133 - Existe información en el atributo \"Origen:NumRegIdTrib\" y se captura información en \"Origen:RFCRemitente\".";
                    if (string.IsNullOrEmpty(ubi.Origen.RFCRemitente) && string.IsNullOrEmpty(ubi.Origen.NumRegIdTrib))
                        return "CP134 - El atributo \"Origen:NumRegIdTrib\", esta vacío o ya existe el atributo \"Origen:RFCDestinatario\" con un RFC nacional.";
                    if (!string.IsNullOrEmpty(ubi.Origen.NumRegIdTrib) && ubi.Origen.ResidenciaFiscalSpecified == false)
                        return "CP137 - No existe información en el atributo \"Ubicaciones:Ubicacion:Origen:NumRegIdTrib\" y existe información en el atributo \"Origen:ResidenciaFiscal\".";
                    if (string.IsNullOrEmpty(ubi.Origen.NumRegIdTrib) && ubi.Origen.ResidenciaFiscalSpecified == true)
                        return "CP137 - No existe información en el atributo \"Ubicaciones:Ubicacion:Origen:NumRegIdTrib\" y existe información en el atributo \"Origen:ResidenciaFiscal\".";
                    if (carta.Mercancias.AutotransporteFederal != null && ubi.Origen.NumEstacionSpecified == true)
                        return "CP138 - Se registró información en el atributo \"Origen:NumEstacion\" cuando solo existe un nodo \"Mercancia:AutotransporteFederal\".";

                    if (ubi.Origen.ResidenciaFiscalSpecified && !string.IsNullOrEmpty(ubi.Origen.RFCRemitente))
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
                                    return "CP136 - El atributo \"Origen:NumRegIdTrib\" no cumple con el patrón publicado en la columna \"Formato de registro de identidad tributaria\" del país indicado en el atributo \"Origen:ResidenciaFiscal\".";

                                }
                            }
                            else if (ubi.Origen.NumRegIdTrib != null && pais17.FormatodeRIT != null && !Regex.Match(ubi.Origen.NumRegIdTrib, "^" + pais17.FormatodeRIT + "$").Success)
                            {
                                return "CP136 - El atributo \"Origen:NumRegIdTrib\" no cumple con el patrón publicado en la columna \"Formato de registro de identidad tributaria\" del país indicado en el atributo \"Origen:ResidenciaFiscal\".";

                            }
                        }
                        else
                            return "CP135 - El atributo \"Origen:NumRegIdTrib\" no tiene un valor que exista en el registro del país indicado en el atributo \"Origen:ResidenciaFiscal\".";

                    }
                    if (ubi.Origen.RFCRemitente != com.Emisor.Rfc && com.TipoDeComprobante == "T")
                    {
                        Operaciones_IRFC r = new Operaciones_IRFC();
                        vI_RFC t = r.Consultar_IRFC(ubi.Origen.RFCRemitente);
                        if (t == null)
                        {
                            return "CP131 - El tipo de comprobante no  es \"T\",  el RFC registrado es diferente al emisor del CFDI, o no existe en  la lista de RFC inscritos no cancelados del SAT l_RFC.";

                        }
                    }
                    if (ubi.Origen.RFCRemitente != com.Receptor.Rfc && com.TipoDeComprobante == "I")
                    {
                        Operaciones_IRFC r = new Operaciones_IRFC();
                        vI_RFC t = r.Consultar_IRFC(ubi.Origen.RFCRemitente);
                        if (t == null)
                        {
                            return "CP132 - El tipo de comprobante no  es \"I\",  el RFC registrado es diferente al receptor del CFDI, o no existe en  la lista de RFC inscritos no cancelados del SAT l_RFC.";

                        }
                    }

                    if (carta.Mercancias.TransporteFerroviario != null || carta.Mercancias.TransporteMaritimo != null || carta.Mercancias.TransporteAereo != null)
                    {
                        if(ubi.Origen.NumEstacionSpecified==false)
                            return "CP139 - El atributo \"Origen:NumEstacion\" tiene un valor no permitido.";
                         
                             c_Estaciones myTipoEstaciones;
                            Enum.TryParse<c_Estaciones>(ubi.Origen.NumEstacion.ToString(), out myTipoEstaciones);
                            if (myTipoEstaciones.ToString() != ubi.Origen.NumEstacion.ToString())
                                return "CP139 - El atributo \"Origen:NumEstacion\" tiene un valor no permitido.";
                        
                    }
                    if(carta.Mercancias.TransporteMaritimo != null&&ubi.Origen.NavegacionTraficoSpecified==false)
                        return "CP142 - No existe el nodo \"Mercancia:TransporteMaritimo\" y se registra información en el atributo \"Origen:NavegacionTrafico\".";
                    if (carta.Mercancias.TransporteMaritimo == null && ubi.Origen.NavegacionTraficoSpecified == true)
                        return "CP142 - No existe el nodo \"Mercancia:TransporteMaritimo\" y se registra información en el atributo \"Origen:NavegacionTrafico\".";

                    if (!string.IsNullOrEmpty(ubi.Destino.NumRegIdTrib) && !string.IsNullOrEmpty(ubi.Destino.RFCDestinatario))
                        return "CP146 - Existe información en el atributo \"Origen:NumRegIdTrib\" y se captura información en \"Destino:RFCDestinatario\".";
                
                    if (ubi.Destino.RFCDestinatario != com.Emisor.Rfc && com.TipoDeComprobante == "T")
                    {
                        Operaciones_IRFC r = new Operaciones_IRFC();
                        vI_RFC t = r.Consultar_IRFC(ubi.Destino.RFCDestinatario);
                        if (t == null)
                        {
                            return "CP144 - El tipo de comprobante no  es \"T\",  el RFC registrado es diferente al emisor del CFDI, o no existe en  la lista de RFC inscritos no cancelados del SAT l_RFC.";

                        }
                    }
                    if (ubi.Destino.RFCDestinatario != com.Receptor.Rfc && com.TipoDeComprobante == "I")
                    {
                        Operaciones_IRFC r = new Operaciones_IRFC();
                        vI_RFC t = r.Consultar_IRFC(ubi.Destino.RFCDestinatario);
                        if (t == null)
                        {
                            return "CP145 - El tipo de comprobante no  es \"I\",  el RFC registrado es diferente al receptor del CFDI, o no existe en  la lista de RFC inscritos no cancelados del SAT l_RFC.";

                        }
                    }

                    if (!string.IsNullOrEmpty(ubi.Destino.NumRegIdTrib) && ubi.Destino.ResidenciaFiscalSpecified==true)
                        return "CP150 - No existe información en el atributo \"Ubicaciones:Ubicacion:Destino:NumRegIdTrib\" y existe información en el atributo \"Destino:ResidenciaFiscal\".";
                    if (string.IsNullOrEmpty(ubi.Destino.NumRegIdTrib) && ubi.Destino.ResidenciaFiscalSpecified == false)
                        return "CP150 - No existe información en el atributo \"Ubicaciones:Ubicacion:Destino:NumRegIdTrib\" y existe información en el atributo \"Destino:ResidenciaFiscal\".";
                    if (carta.Mercancias.AutotransporteFederal!=null && ubi.Destino.NumEstacionSpecified == true)
                        return "CP151 - Se registró información en el atributo \"Destino:NumEstacion\" cuando solo existe un nodo \"Mercancia:AutotransporteFederal\".";
                    if (carta.Mercancias.TransporteFerroviario != null || carta.Mercancias.TransporteMaritimo != null || carta.Mercancias.TransporteAereo != null)
                    {
                        if (ubi.Destino.NumEstacionSpecified == false)
                            return "CP152 - El atributo \"Destino:NumEstacion\" tiene un valor no permitido.";
                        c_Estaciones myTipoEstaciones;
                        Enum.TryParse<c_Estaciones>(ubi.Origen.NumEstacion.ToString(), out myTipoEstaciones);
                        if (myTipoEstaciones.ToString() != ubi.Origen.NumEstacion.ToString())
                            return "CP152 - El atributo \"Destino:NumEstacion\" tiene un valor no permitido.";

                    }
                      if ( carta.Mercancias.TransporteMaritimo !=null && ubi.Destino.NavegacionTraficoSpecified==false)
                          return "CP155 - No existe el nodo \"Mercancia:TransporteMaritimo\" y se registra información en el atributo \"Destino:NavegacionTrafico\".";

                    if (ubi.Origen.ResidenciaFiscalSpecified && !string.IsNullOrEmpty(ubi.Destino.RFCDestinatario))
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
                                    return "CP149 - El atributo \"Destino:NumRegIdTrib\" no cumple con el patrón publicado en la columna \"Formato de Registro de Identidad Tributaria\" del país indicado en el atributo \"Destino:ResidenciaFiscal\".";

                                }
                            }
                            else if (ubi.Origen.NumRegIdTrib != null && pais17.FormatodeRIT != null && !Regex.Match(ubi.Origen.NumRegIdTrib, "^" + pais17.FormatodeRIT + "$").Success)
                            {
                                return "CP149 - El atributo \"Destino:NumRegIdTrib\" no cumple con el patrón publicado en la columna \"Formato de Registro de Identidad Tributaria\" del país indicado en el atributo \"Destino:ResidenciaFiscal\".";

                            }
                        }
                        else
                            return "CP148 - El atributo \"Destino:NumRegIdTrib\" no tiene un valor que exista en el registro del país indicado en el atributo \"Destino:ResidenciaFiscal\".";

                    }
                    if (ubi.Origen.ResidenciaFiscalSpecified && string.IsNullOrEmpty(ubi.Destino.RFCDestinatario))
                         return "CP147 - El atributo \"Destino:NumRegIdTrib\", esta vacío o ya existe el atributo \"Destino:RFCDestinatario\" con un RFC nacional.";
                    if(carta.Mercancias.TransporteFerroviario!=null && ubi.TipoEstacionSpecified==true)
                        if(ubi.TipoEstacion==c_TipoEstacion.Item02 && ubi.Domicilio!=null)
                            return "CP156 - El nodo \"Ubicación:Domicilio\" no debe existir.";
                    if( (ubi.TipoEstacion==c_TipoEstacion.Item01|| ubi.TipoEstacion==c_TipoEstacion.Item03)&&ubi.Domicilio==null)
                        return "CP157 - Se debe registrar el nodo \"Ubicación:Domicilio\".";

                    global::c_Pais myPais;
                    Enum.TryParse<global::c_Pais>(ubi.Domicilio.Pais.ToString(), out myPais);
                    if (myPais.ToString() != ubi.Domicilio.Pais.ToString())
                        return "CP163 - El atributo \"Domicilio:Pais\" tiene un valor no permitido.";

                    if (ubi.Domicilio.Pais == global::c_Pais.MEX)
                    {
                        CatalogosSAT.OperacionesCatalogos o = new CatalogosSAT.OperacionesCatalogos();
                        CatalogosSAT.c_Colonia colonia5 = o.Consultar_Colonia(ubi.Domicilio.CodigoPostal.ToString(), ubi.Domicilio.Colonia.ToString());
                        if (colonia5 == null)
                            return ("CP158 - El atributo \"Domicilio:Colonia\" debe contener una clave del catálogo de \"catCFDI:c_Colonia\", donde la columna \"c_CodigoPostal\" debe ser igual a la clave registrada en el atributo \"CodigoPostal\" o la clave del atributo \"Colonia\", no corresponde a la clave del país registrado al ser extranjero.");
                    
                        CatalogosSAT.c_Localidad localidad5 = o.Consultar_Localidad(ubi.Domicilio.Estado.ToString(), ubi.Domicilio.Localidad.ToString());
                        if (localidad5 == null)
                            return ("CP160 - El atributo \"Domicilio:Localidad\" debe contener una clave del catálogo de \"catCFDI:c_Localidad\", donde la columna clave de \"c_Estado\" debe ser igual a la clave registrada en el atributo estado si el atributo \"Pais\" tiene el valor \"MEX\".");

                        CatalogosSAT.c_Municipio municipio5 = o.Consultar_Municipio(ubi.Domicilio.Estado.ToString(),ubi.Domicilio.Municipio.ToString());
                        if (municipio5 == null)
                            return ("CP161 - El atributo \"Domicilio:Municipio\" debe contener una clave del catálogo de \"catCFDI:c_Municipio\" donde la columna clave de \"c_Estado\" debe ser igual a la clave registrada en el atributo \"Estado\" si el atributo \"Pais\" tiene el valor \"MEX\" o la clave del atributo \"Municipio\", no corresponde a la clave del \"Estado\" registrado al ser extranjero.");
                        CatalogosSAT.c_Estado estado5 = o.Consultar_Estados(ubi.Domicilio.Estado.ToString());
                        if (estado5 == null)
                            return ("CP162 - El atributo \"Domicilio:Estado\" debe contener una clave del catálogo \"catCFDI:c_Estado\" donde la columna \"c_Pais\" tenga el valor \"MEX\" si el atributo \"Pais\" tiene el valor \"MEX\" o la clave del atributo \"Estado\", no corresponde a la clave del país registrado al ser extranjero.");

                        CatalogosSAT.c_CP CP5 = o.Consultar_CP(ubi.Domicilio.Estado.ToString(), ubi.Domicilio.Municipio.ToString(), ubi.Domicilio.Localidad.ToString(), ubi.Domicilio.CodigoPostal.ToString());
                        if (CP5 == null)
                            return ("CP164 - El atributo \"Domicilio:CodigoPostal\" debe contener una clave del catálogo de \"catCFDI:c_CodigoPostal\", donde la columna clave de \"c_Estado\" debe ser igual a la clave registrada en el atributo \"Estado\", la columna clave de \"c_Municipio\" debe ser igual a la clave registrada en el atributo \"Municipio\", y si existe el atributo de \"Localidad\", la columna clave de \"c_Localidad\" debe ser igual a la clave registrada en el atributo \"Localidad\" si el atributo \"Pais\" tiene el valor \"MEX\" o la clave del atributo \"CodigoPostal\", no corresponde a la clave del país registrado al ser extranjero.");

                    }
                    else
                    {
                        if(string.IsNullOrEmpty(ubi.Domicilio.Localidad))
                            return ("CP159 - La clave del atributo \"Localidad\" no corresponde a la clave del país registrado.");
                   
                    }
       
                }

                if(carta.Mercancias.TransporteFerroviario!=null && carta.Mercancias.PesoBrutoTotalSpecified==false)
                    return ("CP165 - El atributo \"Mercancia:PesoBrutoTotal\" no debe existir.");
                   if(carta.Mercancias.TransporteFerroviario==null && carta.Mercancias.PesoBrutoTotalSpecified==true)
                    return ("CP165 - El atributo \"Mercancia:PesoBrutoTotal\" no debe existir.");
              if(carta.Mercancias.TransporteFerroviario!=null)
              {
                  if(carta.Mercancias.UnidadPesoSpecified==true)
                  {
                 c_ClaveUnidadPeso myClaveUnidadPeso;
                    Enum.TryParse<c_ClaveUnidadPeso>(carta.Mercancias.UnidadPeso.ToString(), out myClaveUnidadPeso);
                    if (myClaveUnidadPeso.ToString() !=carta.Mercancias.UnidadPeso.ToString())
                    return "CP166 - El atributo \"Mercancias:UnidadPeso\" tiene un valor no permitido..";
                }
                  else
                        return "CP166 - El atributo \"Mercancias:UnidadPeso\" tiene un valor no permitido..";
              
              }
              if (carta.Mercancias.TransporteMaritimo != null)
              {
                  if(carta.Mercancias.PesoNetoTotalSpecified==false)
                      return "CP167 - La suma de los valores registrados en el atributo \"Mercancia:DetalleMercancia:PesoNeto\", no corresponde.";
   
                 decimal sumaPeso = 0;
                  foreach (var mer in carta.Mercancias.Mercancia)
                  {
                   sumaPeso=sumaPeso+   mer.DetalleMercancia.PesoNeto;
                  }
                  if(carta.Mercancias.PesoNetoTotal!=sumaPeso)
                      return "CP167 - La suma de los valores registrados en el atributo \"Mercancia:DetalleMercancia:PesoNeto\", no corresponde.";
   
              }
              if (carta.Mercancias.TransporteFerroviario != null)
              {
                  decimal sumaPeso = 0;
                  foreach (var car in carta.Mercancias.TransporteFerroviario.Carro)
                  {
                      sumaPeso = sumaPeso + car.ToneladasNetasCarro;
                  }
                  if (carta.Mercancias.PesoNetoTotal != sumaPeso)
                      return "CP168 - La suma de los valores registrados en el atributo \"TransporteFerroviario:Carro:ToneladasNetasCarro\", no corresponde.";
              }
               if(carta.Mercancias.Mercancia.Count()!=carta.Mercancias.NumTotalMercancias)
                    return "CP169 - El valor registrado no coincide con el número de elementos \"Mercancia\" que se registraron en el complemento.";

               if (com.TipoDeComprobante != "T")
               {
                   foreach (var mer in carta.Mercancias.Mercancia)
                   {
                       if (mer.BienesTranspSpecified == true)
                       {
                           CatalogosSAT.OperacionesCatalogos o = new CatalogosSAT.OperacionesCatalogos();
                           CatalogosSAT.c_ClaveProdServCP clave = o.Consultar_ClaveProdServCP(mer.BienesTransp.ToString());
                           if (clave != null)
                           {//    return "CP170 - El tipo de comprobante es diferente de \"T\" o el valor del atributo \"Mercancia:BienesTransp\", es diferente a la clave registrada a nivel CFDI en el atributo \"ClaveProdServ\", o contiene el valor \"0\" en la columna \"MaterialPeligroso\" del catálogo \"catCartaPorte:c_ClaveProdServCP\".";

                               if (clave.MaterialPeligroso == "1" || clave.MaterialPeligroso == "0,1")
                               {
                                   if (mer.MaterialPeligrosoSpecified == false)
                                       return "CP174 - El atributo \"Mercancia:BienesTransp\" contiene el valor \"0\" en la columna \"MaterialPeligroso\" del catálogo \"catCartaPorte:c_ClaveProdServCP\".";
                                   if (mer.MaterialPeligroso == CartaPorteMercanciasMercanciaMaterialPeligroso.Sí)
                                   {
                                       c_MaterialPeligroso myMaterialPeligroso;
                                       Enum.TryParse<c_MaterialPeligroso>(mer.CveMaterialPeligroso.ToString(), out myMaterialPeligroso);
                                       if (myMaterialPeligroso.ToString() != mer.CveMaterialPeligroso.ToString())
                                           return "CP175 - El valor registrado en el atributo \"MaterialPeligroso\" es diferente a las establecidas en el catálogo \"catCartaPorte:c_MaterialPeligroso\".";
                                       if (mer.EmbalajeSpecified == false)
                                           return "CP177 - El atributo \"Mercancia:Embalaje\", no debe existir.";

                                   }
                                   else
                                   {
                                       if (mer.CveMaterialPeligrosoSpecified == true)
                                           return "CP176 - El atributo  \"Mercancia:CveMaterialPeligroso\" no debe existir.";
                                       if (mer.EmbalajeSpecified == true)
                                           return "CP177 - El atributo \"Mercancia:Embalaje\", no debe existir.";
                                   }
                                   bool exi = false;
                                   foreach (var con in com.Conceptos)
                                   {
                                       if (con.ClaveProdServ == mer.BienesTransp.ToString())
                                       { exi = true; break; }
                                   }
                                   if (exi == false)
                                       return "CP170 - El tipo de comprobante es diferente de \"T\" o el valor del atributo \"Mercancia:BienesTransp\", es diferente a la clave registrada a nivel CFDI en el atributo \"ClaveProdServ\", o contiene el valor \"0\" en la columna \"MaterialPeligroso\" del catálogo \"catCartaPorte:c_ClaveProdServCP\".";

                               }
                               else
                               {
                                   if (clave.MaterialPeligroso == "0")
                                       return "CP171 - El atributo \"Mercancia:BienesTransp\" contiene el valor \"0\" en la columna \"MaterialPeligroso\" del catálogo \"catCartaPorte:c_ClaveProdServCP\".";
                                   if (mer.MaterialPeligrosoSpecified == true)
                                       return "CP174 - El atributo \"Mercancia:BienesTransp\" contiene el valor \"0\" en la columna \"MaterialPeligroso\" del catálogo \"catCartaPorte:c_ClaveProdServCP\".";

                               }
                           }
                       }
                       else 
                           if(!string.IsNullOrEmpty(mer.Descripcion))
                               return "CP173 - No se debe registrar información en el atributo \"Mercancia:Descripcion\".";

                    }
               }
               if (com.TipoDeComprobante != "I")
               {
                   foreach (var mer in carta.Mercancias.Mercancia)
                   {
                       CatalogosSAT.OperacionesCatalogos o = new CatalogosSAT.OperacionesCatalogos();
                           CatalogosSAT.c_ClaveProdServCP clave = o.Consultar_ClaveProdServCP(mer.BienesTransp.ToString());
                           if (clave == null)
                               return "CP172 - El tipo de comprobante es distinto de \"I\" y el atributo \"Mercancia:BienesTransp\" tiene una clave diferente del catálogo \"catCartaPorte:c_ClaveProdServCP\".";
                           if (clave.MaterialPeligroso == "1" || clave.MaterialPeligroso == "0,1")
                           {
                               if (mer.MaterialPeligrosoSpecified == false)
                                   return "CP174 - El atributo \"Mercancia:BienesTransp\" contiene el valor \"0\" en la columna \"MaterialPeligroso\" del catálogo \"catCartaPorte:c_ClaveProdServCP\".";
                               if (mer.MaterialPeligroso == CartaPorteMercanciasMercanciaMaterialPeligroso.Sí)
                               {
                                   c_MaterialPeligroso myMaterialPeligroso;
                                   Enum.TryParse<c_MaterialPeligroso>(mer.CveMaterialPeligroso.ToString(), out myMaterialPeligroso);
                                   if (myMaterialPeligroso.ToString() != mer.CveMaterialPeligroso.ToString())
                                       return "CP175 - El valor registrado en el atributo \"MaterialPeligroso\" es diferente a las establecidas en el catálogo \"catCartaPorte:c_MaterialPeligroso\".";
                                   if (mer.EmbalajeSpecified == false)
                                       return "CP177 - El atributo \"Mercancia:Embalaje\", no debe existir.";
                                     
                               }
                               else
                               { 
                                   if(mer.CveMaterialPeligrosoSpecified==true)
                                       return "CP176 - El atributo  \"Mercancia:CveMaterialPeligroso\" no debe existir.";
                                   if (mer.EmbalajeSpecified == true)
                                       return "CP177 - El atributo \"Mercancia:Embalaje\", no debe existir.";
                        
                  
                               }

                           }
                           else
                           {
                                   if (mer.MaterialPeligrosoSpecified == true)
                                   return "CP174 - El atributo \"Mercancia:BienesTransp\" contiene el valor \"0\" en la columna \"MaterialPeligroso\" del catálogo \"catCartaPorte:c_ClaveProdServCP\".";

                           }
                   }
               }

               if (carta.TranspInternac == CartaPorteTranspInternac.Sí)
               {
                   foreach (var mer in carta.Mercancias.Mercancia)
                   {
                       if (mer.FraccionArancelariaSpecified == true)
                       {
                           CatalogosSAT.OperacionesCatalogos o13 = new CatalogosSAT.OperacionesCatalogos();
                           CatalogosSAT.c_FraccionArancelaria f = o13.Consultar_FraccionArancelaria(mer.FraccionArancelaria.ToString().Replace("Item", ""));
                           if (f == null)
                               return "CP178 - El atributo \"Mercancia:FraccionArancelaria\" tiene una clave diferente a las establecidas en el catálogo \"catComExt:c_FraccionArancelaria\" o este atributo no debe existir.";

                       }
                       else
                           return "CP178 - El atributo \"Mercancia:FraccionArancelaria\" tiene una clave diferente a las establecidas en el catálogo \"catComExt:c_FraccionArancelaria\" o este atributo no debe existir.";

                   }

               }
               else
               {
                   foreach (var mer in carta.Mercancias.Mercancia)
                   {
                       if (mer.FraccionArancelariaSpecified == true)
                         return "CP178 - El atributo \"Mercancia:FraccionArancelaria\" tiene una clave diferente a las establecidas en el catálogo \"catComExt:c_FraccionArancelaria\" o este atributo no debe existir.";

                   }
               }
               
                List<CartaPorteUbicacionDestino>D=new List<CartaPorteUbicacionDestino> ();
               foreach (var ubi in carta.Ubicaciones)
               {
                   if (ubi.Destino != null)
                       D.Add(ubi.Destino);
                                     
               }
               if (D.Count < 2)
               {
                   foreach (var mer in carta.Mercancias.Mercancia)
                   {
                       if (mer.CantidadTransporta != null)
                           return "CP180 - Solo se tiene un registro en el nodo \"Ubicación:Destino\", por lo que no se debe registrar este nodo.";
                   }
               }
               else
               {
                   foreach (var mer in carta.Mercancias.Mercancia)
                   {
                       if (mer.CantidadTransporta == null)
                           return "CP181 - Se tiene más de un registro del nodo \"Ubicación:Destino\", por lo que se debe registrar el nodo \"CantidadTransporta\".";
                   }
               
               }
               foreach (var ubi in carta.Ubicaciones)
               {
                   if (!string.IsNullOrEmpty(ubi.Origen.IDOrigen))
                   {
                       foreach (var mer in carta.Mercancias.Mercancia)
                       {
                           if(mer.CantidadTransporta!=null)
                           {
                               bool ex = false;
                               foreach (var cat in mer.CantidadTransporta)
                               {
                                   if (cat.IDOrigen == ubi.Origen.IDOrigen)
                                   { ex = true; break; }
                               }
                               if (ex)
                                   break;
                               else
                                   return "CP182 - El valor registrado no coincide con un valor registrado en los atributos \"Ubicacion:IDOrigen\".";
     
                           }
                       }
                   }
                   if (!string.IsNullOrEmpty(ubi.Destino.IDDestino))
                   {
                       foreach (var mer in carta.Mercancias.Mercancia)
                       {
                           if (mer.CantidadTransporta != null)
                           {
                               bool ex = false;
                               foreach (var cat in mer.CantidadTransporta)
                               {
                                   if (cat.IDDestino == ubi.Destino.IDDestino)
                                   { ex = true; break; }
                               }
                               if (ex)
                                   break;
                               else
                                   return "CP183 - El valor registrado no coincide con  un valor registrado en los atributos \"Ubicacion:IDDestino\".";

                           }
                       }
                   }


                }

                   foreach (var mer in carta.Mercancias.Mercancia)
                   {
                       if (mer.CantidadTransporta != null)
                       {
                            foreach (var cat in mer.CantidadTransporta)
                           {
                               if (cat.CvesTransporteSpecified)
                               {
                                   c_CveTransporte myCvetransporte;
                                   Enum.TryParse<c_CveTransporte>(cat.CvesTransporte.ToString(), out myCvetransporte);
                                   if (myCvetransporte.ToString() != cat.CvesTransporte.ToString())
                                       return "CP184 - El atributo \"CantidadTransporta:CvesTransporte\" tiene una clave diferente a las establecidas en el catálogo \"catCartaPorte:c_CveTransporte\" o este no debe existir.";
                       
                                    if (carta.Mercancias.Mercancia.Count() <2)
                                        return "CP184 - El atributo \"CantidadTransporta:CvesTransporte\" tiene una clave diferente a las establecidas en el catálogo \"catCartaPorte:c_CveTransporte\" o este no debe existir.";
                       
              
                               }
                           }
                       
                       
                       }
                       if(mer.DetalleMercancia!=null&&carta.Mercancias.TransporteMaritimo == null)
                           return "CP185 - El atributo \"DetalleMercancia\" no debe existir.";
                       if (mer.DetalleMercancia == null && carta.Mercancias.TransporteMaritimo != null)
                           return "CP185 - El atributo \"DetalleMercancia\" no debe existir.";
               
                       
                   }

                   if (carta.Mercancias.TransporteAereo != null)
                   {
                       if (!string.IsNullOrEmpty(carta.Mercancias.TransporteAereo.RFCTransportista))
                       {
                           if (carta.Mercancias.TransporteAereo.RFCTransportista == com.Emisor.Rfc)
                               return "CP186 - El valor registrado en el atributo \"RFCTransportista\" es el mismo que el RFC emisor.";
                           Operaciones_IRFC r = new Operaciones_IRFC();
                           vI_RFC t = r.Consultar_IRFC(carta.Mercancias.TransporteAereo.RFCTransportista);
                           if (t == null)
                               return "CP187 - El valor registrado en el atributo \"RFCTransportista\" no existe en la lista de RFC inscritos no cancelados en el SAT (l_RFC).";
                           if(carta.Mercancias.TransporteAereo.CodigoTransportistaSpecified==false)
                               return "CP188 - El atributo \"TransporteAereo:CodigoTransportista\" tiene una clave diferente a las establecidas en el catálogo \"catCartaPorte:c_CodigoTransporteAereo\" o el atributo no debe existir.";
                           c_CodigoTransporteAereo myCodigoTransporteAereo;
                           Enum.TryParse<c_CodigoTransporteAereo>(carta.Mercancias.TransporteAereo.CodigoTransportista.ToString(), out myCodigoTransporteAereo);
                           if (myCodigoTransporteAereo.ToString() != carta.Mercancias.TransporteAereo.CodigoTransportista.ToString())
                               return "CP188 - El atributo \"TransporteAereo:CodigoTransportista\" tiene una clave diferente a las establecidas en el catálogo \"catCartaPorte:c_CodigoTransporteAereo\" o el atributo no debe existir.";
                  
                       }
                       else
                           if (carta.Mercancias.TransporteAereo.CodigoTransportistaSpecified == true)
                               return "CP188 - El atributo \"TransporteAereo:CodigoTransportista\" tiene una clave diferente a las establecidas en el catálogo \"catCartaPorte:c_CodigoTransporteAereo\" o el atributo no debe existir.";

                       if (string.IsNullOrEmpty(carta.Mercancias.TransporteAereo.RFCTransportista))
                       {
                           if(carta.Mercancias.TransporteAereo.ResidenciaFiscalTransporSpecified==false)
                               return "CP192 - El atributo  \"TransporteAereo:ResidenciaFiscalTranspor\" tiene una clave diferente a las establecidas en el catálogo \"catCFDI:c_Pais\".";
                           global::c_Pais myPais;
                           Enum.TryParse<global::c_Pais>(carta.Mercancias.TransporteAereo.ResidenciaFiscalTranspor.ToString(), out myPais);
                           if (myPais.ToString() != carta.Mercancias.TransporteAereo.ResidenciaFiscalTranspor.ToString())
                               return "CP192 - El atributo  \"TransporteAereo:ResidenciaFiscalTranspor\" tiene una clave diferente a las establecidas en el catálogo \"catCFDI:c_Pais\".";
                     
                       }  
                       if(string.IsNullOrEmpty(carta.Mercancias.TransporteAereo.NumRegIdTribTranspor)&& string.IsNullOrEmpty(carta.Mercancias.TransporteAereo.RFCTransportista)&& carta.Mercancias.TransporteAereo.ResidenciaFiscalTransporSpecified==true)
                           return "CP189 - El atributo \"TransporteAereo:RFCTransportista\", esta vacío o ya existe el atributo \"RFCTransportista\" con un RFC nacional.";
                       if (!string.IsNullOrEmpty(carta.Mercancias.TransporteAereo.NumRegIdTribTranspor) && !string.IsNullOrEmpty(carta.Mercancias.TransporteAereo.RFCTransportista) && carta.Mercancias.TransporteAereo.ResidenciaFiscalTransporSpecified == true)
                           return "CP189 - El atributo \"TransporteAereo:RFCTransportista\", esta vacío o ya existe el atributo \"RFCTransportista\" con un RFC nacional.";
                       if (!string.IsNullOrEmpty(carta.Mercancias.TransporteAereo.NumRegIdTribTranspor) && string.IsNullOrEmpty(carta.Mercancias.TransporteAereo.RFCTransportista) && carta.Mercancias.TransporteAereo.ResidenciaFiscalTransporSpecified == true)
                       {
                           OperacionesCatalogos o13 = new OperacionesCatalogos();
                           CatalogosSAT.c_Pais pais17 = o13.Consultar_PaisVerificacionLinea(carta.Mercancias.TransporteAereo.ResidenciaFiscalTranspor.ToString());
                           if (pais17 != null)
                           {
                               if (pais17.ValidaciondelRIT != null)
                               {
                                   Operaciones_IRFC r = new Operaciones_IRFC();
                                   vI_RFC t = r.Consultar_IRFC(carta.Mercancias.TransporteAereo.NumRegIdTribTranspor);
                                   if (t == null)
                                   {
                                       return "CP191 - El atributo \"TransporteAereo:NumRegIdTribTranspor\", no cumple con el patrón publicado en la columna \"Formato de registro de identidad tributaria\" del país indicado en el atributo \"TransporteAereo:ResidenciaFiscalTranspor\"";

                                   }
                               }
                               else if (carta.Mercancias.TransporteAereo.NumRegIdTribTranspor != null && pais17.FormatodeRIT != null && !Regex.Match(carta.Mercancias.TransporteAereo.NumRegIdTribTranspor, "^" + pais17.FormatodeRIT + "$").Success)
                               {
                                   return "CP191 - El atributo \"TransporteAereo:NumRegIdTribTranspor\", no cumple con el patrón publicado en la columna \"Formato de registro de identidad tributaria\" del país indicado en el atributo \"TransporteAereo:ResidenciaFiscalTranspor\"";

                               }
                           }
                           else
                               return "CP190 - El atributo \"TransporteAereo:NumRegIdTribTranspor\", no tiene un valor que exista en el registro del país indicado en el atributo \"TransporteAereo:ResidenciaFiscalTranspor\".";

                       }
                        if (string.IsNullOrEmpty(carta.Mercancias.TransporteAereo.RFCEmbarcador))
                       {
                           if(carta.Mercancias.TransporteAereo.ResidenciaFiscalEmbarcSpecified==false)
                               return "CP196 - El atributo  \"TransporteAereo:ResidenciaFiscalEmbarc\" tiene una clave diferente a las establecidas en el catálogo \"catCFDI:c_Pais\".";
                           global::c_Pais myPais;
                           Enum.TryParse<global::c_Pais>(carta.Mercancias.TransporteAereo.ResidenciaFiscalEmbarc.ToString(), out myPais);
                           if (myPais.ToString() != carta.Mercancias.TransporteAereo.ResidenciaFiscalEmbarc.ToString())
                               return "CP196 - El atributo  \"TransporteAereo:ResidenciaFiscalEmbarc\" tiene una clave diferente a las establecidas en el catálogo \"catCFDI:c_Pais\".";
                     
                       }
                        if (string.IsNullOrEmpty(carta.Mercancias.TransporteAereo.NumRegIdTribEmbarc) && string.IsNullOrEmpty(carta.Mercancias.TransporteAereo.RFCEmbarcador) && carta.Mercancias.TransporteAereo.ResidenciaFiscalEmbarcSpecified == true)
                            return "CP193 - El atributo \"TransporteAereo:RFCEmbarcador\", esta vacío o ya existe el atributo \"RFCEmbarcador\" con un RFC nacional.";
                        if (!string.IsNullOrEmpty(carta.Mercancias.TransporteAereo.NumRegIdTribEmbarc) && !string.IsNullOrEmpty(carta.Mercancias.TransporteAereo.RFCEmbarcador) && carta.Mercancias.TransporteAereo.ResidenciaFiscalEmbarcSpecified == true)
                            return "CP193 - El atributo \"TransporteAereo:RFCEmbarcador\", esta vacío o ya existe el atributo \"RFCEmbarcador\" con un RFC nacional.";


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
                                        return "CP195 - El atributo \"TransporteAereo:NumRegIdTribEmbarc\"  no cumple con el patrón publicado en la columna \"Formato de registro de identidad tributaria\" del país indicado en el atributo \"TransporteAereo:ResidenciaFiscalEmbarc\".";

                                    }
                                }
                                else if (carta.Mercancias.TransporteAereo.NumRegIdTribEmbarc != null && pais17.FormatodeRIT != null && !Regex.Match(carta.Mercancias.TransporteAereo.NumRegIdTribEmbarc, "^" + pais17.FormatodeRIT + "$").Success)
                                {
                                    return "CP195 - El atributo \"TransporteAereo:NumRegIdTribEmbarc\"  no cumple con el patrón publicado en la columna \"Formato de registro de identidad tributaria\" del país indicado en el atributo \"TransporteAereo:ResidenciaFiscalEmbarc\".";

                                }
                            }
                            else
                                return "CP194 - El atributo \"TransporteAereo:NumRegIdTribEmbarc\", no tiene un valor que exista en el registro del país indicado en el atributo \"TransporteAereo:ResidenciaFiscalEmbarc\".";

                        }
                   }

                   if (carta.Mercancias.TransporteFerroviario != null)
                   {
                       if(carta.Mercancias.TransporteFerroviario.Concesionario==com.Emisor.Rfc)
                           return "CP197 - El valor registrado en el atributo \"Concesionario\" es el mismo que el RFC emisor del CFDI o no existe en la lista de contribuyentes no cancelados del SAT (l_RFC).";
                       Operaciones_IRFC r = new Operaciones_IRFC();
                       vI_RFC t = r.Consultar_IRFC(carta.Mercancias.TransporteFerroviario.Concesionario);
                       if (t == null)
                           return "CP197 - El valor registrado en el atributo \"Concesionario\" es el mismo que el RFC emisor del CFDI o no existe en la lista de contribuyentes no cancelados del SAT (l_RFC).";

                       if (carta.Mercancias.TransporteFerroviario.Carro != null)
                       {
                           decimal tot = 0; bool exis = false; decimal peso=0;
                           foreach (var car in carta.Mercancias.TransporteFerroviario.Carro)
                           {
                               if(car.Contenedor!=null)
                                   foreach (var con in car.Contenedor)
                                   {
                                       if (con.PesoNetoMercancia != null)
                                       { exis = true;  peso=con.PesoNetoMercancia; break;}
                                   }
                               tot=tot+ car.ToneladasNetasCarro;
                           }
                           if(exis && peso!=tot)
                               return "CP198 - El valor registrado en el atributo \"ToneladasNetasCarro\" no corresponde a la suma de los valores registrados en el atributo \"PesoNetoMercancia\". ";
                                                          
                       }
        
                   }

                   if (carta.FiguraTransporte != null)
                   {

                       if (carta.Mercancias.AutotransporteFederal != null && carta.FiguraTransporte.Operadores == null)
                           return "CP199 - El nodo \"Operadores\" debe existir, siempre que exista el nodo \"Mercancia:AutotransporteFederal\".";
                       if (carta.Mercancias.AutotransporteFederal == null && carta.FiguraTransporte.Operadores != null)
                           return "CP199 - El nodo \"Operadores\" debe existir, siempre que exista el nodo \"Mercancia:AutotransporteFederal\".";
                       if (carta.FiguraTransporte.Operadores != null)
                       {
                           foreach (var oper in carta.FiguraTransporte.Operadores)
                           {
                              
                               foreach (var op in oper.Operador)
                               {
                                   if (!string.IsNullOrEmpty(op.RFCOperador) && op.ResidenciaFiscalOperadorSpecified == true)
                                       return "CP200 - Se debe registrar información en el atributo \"Operador:RFCOperador\" o ya existe información en el atributo \"Operador:NumRegIdTribOperador\".";
                                   
                                   if (!string.IsNullOrEmpty(op.RFCOperador))
                                   {
                                       Operaciones_IRFC r = new Operaciones_IRFC();
                                       vI_RFC t = r.Consultar_IRFC(op.RFCOperador);
                                       if (t == null)
                                           return "CP201 - El valor registrado en el atributo \"RFCOperador\" no existe en la lista de RFC inscritos no cancelados en el SAT (l_RFC).";

                                   }
                                   else
                                   {                                      
                                        if(op.ResidenciaFiscalOperadorSpecified==false)
                                         return "CP205 - El atributo \"Operador:ResidenciaFiscalOperador\" tiene una clave diferente a las establecidas en el catálogo \"catCFDI:c_Pais\".";
                                        global::c_Pais myPais;
                                        Enum.TryParse<global::c_Pais>(op.ResidenciaFiscalOperador.ToString(), out myPais);
                                        if (myPais.ToString() != op.ResidenciaFiscalOperador.ToString())
                                        return "CP205 - El atributo \"Operador:ResidenciaFiscalOperador\" tiene una clave diferente a las establecidas en el catálogo \"catCFDI:c_Pais\".";
                                                          
                                   }
                                   if (string.IsNullOrEmpty(op.NumRegIdTribOperador) && string.IsNullOrEmpty(op.RFCOperador) && op.ResidenciaFiscalOperadorSpecified == true)
                                       return "CP202 - El atributo \"Operadores:Operador:RFCOperador\" esta vacío o ya existe el atributo \"RFCOperador\" con un RFC nacional.";
                                   if (!string.IsNullOrEmpty(op.NumRegIdTribOperador) && !string.IsNullOrEmpty(op.RFCOperador) && op.ResidenciaFiscalOperadorSpecified == true)
                                       return "CP202 - El atributo \"Operadores:Operador:RFCOperador\" esta vacío o ya existe el atributo \"RFCOperador\" con un RFC nacional.";

                                   if (!string.IsNullOrEmpty(op.NumRegIdTribOperador) && string.IsNullOrEmpty(op.RFCOperador) && op.ResidenciaFiscalOperadorSpecified == true)
                                   {
                                       OperacionesCatalogos o13 = new OperacionesCatalogos();
                                       CatalogosSAT.c_Pais pais17 = o13.Consultar_PaisVerificacionLinea(op.ResidenciaFiscalOperador.ToString());
                                       if (pais17 != null)
                                       {
                                           if (pais17.ValidaciondelRIT != null)
                                           {
                                               Operaciones_IRFC r = new Operaciones_IRFC();
                                               vI_RFC t = r.Consultar_IRFC(op.NumRegIdTribOperador);
                                               if (t == null)
                                               {
                                                   return "CP204 - El atributo \"Operador:NumRegIdTribOperador\", no cumple con el patrón publicado en la columna \"Formato de registro de identidad tributaria\" del país indicado en el atributo \"Operador:ResidenciaFiscalOperador\".";
                                               }
                                           }
                                           else if (op.NumRegIdTribOperador != null && pais17.FormatodeRIT != null && !Regex.Match(op.NumRegIdTribOperador, "^" + pais17.FormatodeRIT + "$").Success)
                                           {
                                               return "CP204 - El atributo \"Operador:NumRegIdTribOperador\", no cumple con el patrón publicado en la columna \"Formato de registro de identidad tributaria\" del país indicado en el atributo \"Operador:ResidenciaFiscalOperador\".";
                                           }
                                       }
                                       else
                                           return "CP203 - El atributo \"Operador:NumRegIdTribOperador\", no tiene un valor que exista en el registro del país indicado en el atributo \"Operador:ResidenciaFiscalOperador\".";

                                   }
                                   if (op.Domicilio.Pais == global::c_Pais.MEX)
                                   {
                                       CatalogosSAT.OperacionesCatalogos o = new CatalogosSAT.OperacionesCatalogos();
                                        CatalogosSAT.c_Colonia colonia5 = o.Consultar_Colonia(op.Domicilio.CodigoPostal.ToString(), op.Domicilio.Colonia.ToString());
                                       if (colonia5 == null)
                                           return ("CP207 - El atributo \"Domicilio:Colonia\" debe contener una clave del catálogo de \"catCFDI:c_Colonia\", donde la columna \"c_CodigoPostal\" debe ser igual a la clave registrada en el atributo \"CodigoPostal\".");

                                       CatalogosSAT.c_Localidad localidad5 = o.Consultar_Localidad(op.Domicilio.Estado.ToString(), op.Domicilio.Localidad.ToString());
                                       if (localidad5 == null)
                                           return ("CP209 - El atributo \"Operador:Domicilio:Localidad\" debe contener una clave del catálogo de \"catCFDI:c_Localidad\", donde la columna clave de \"c_Estado\" debe ser igual a la clave registrada en el atributo estado si el atributo \"Pais\" tiene el valor \"MEX\".");

                                       CatalogosSAT.c_Municipio municipio5 = o.Consultar_Municipio(op.Domicilio.Estado.ToString(), op.Domicilio.Municipio.ToString());
                                       if (municipio5 == null)
                                           return ("CP211 - El atributo \"Operador:Domicilio:Municipio\" debe contener una clave del catálogo de \"catCFDI:c_Municipio\" donde la columna clave de \"c_Estado\" debe ser igual a la clave registrada en el atributo \"Estado\" si el atributo \"Pais\" tiene el valor \"MEX\".");
                                       CatalogosSAT.c_Estado estado5 = o.Consultar_Estados(op.Domicilio.Estado.ToString());
                                       if (estado5 == null)
                                           return ("CP213 - El atributo de \"Estado\" debe contener una clave del catálogo \"catCFDI:c_Estado\", donde la columna \"c_Pais\" tenga el valor \"MEX\".");

                                       global::c_Pais myPais;
                                       Enum.TryParse<global::c_Pais>( op.Domicilio.Pais.ToString(), out myPais);
                                       if (myPais.ToString() != op.Domicilio.Pais.ToString())
                                           return "CP214 - El atributo \"Domicilio:Pais\" tiene un valor no permitido.";
                  
                                       CatalogosSAT.c_CP CP5 = o.Consultar_CP(op.Domicilio.Estado.ToString(), op.Domicilio.Municipio.ToString(), op.Domicilio.Localidad.ToString(), op.Domicilio.CodigoPostal.ToString());
                                       if (CP5 == null)
                                           return ("CP216 - El atributo \"Operador:Domicilio:CodigoPostal\" debe contener una clave del catálogo de \"catCFDI:c_CodigoPostal\", donde la columna clave de \"c_Estado\" debe ser igual a la clave registrada en el atributo \"Estado\", la columna clave de \"c_Municipio\" debe ser igual a la clave registrada en el atributo \"Municipio\", y si existe el atributo de \"Localidad\", la columna clave de \"c_Localidad\" debe ser igual a la clave registrada en el atributo \"Localidad\" si el atributo \"Pais\" tiene el valor \"MEX\".");

                                   }
                                   else
                                   {
                                       if (string.IsNullOrEmpty(op.Domicilio.Colonia))
                                           return ("CP206 - La clave del atributo \"Colonia\", no corresponde a la clave del país registrado al ser extranjero.");
                                       if (string.IsNullOrEmpty(op.Domicilio.Localidad))
                                           return ("CP208 - La clave del atributo \"Localidad\" no corresponde a la clave del país registrado.");
                                       if (string.IsNullOrEmpty(op.Domicilio.Municipio))
                                           return ("CP210 - La clave del atributo \"Municipio\", no corresponde a la clave del estado registrado al ser extranjero.");
                                       if (string.IsNullOrEmpty(op.Domicilio.Estado))
                                           return ("CP212 - La clave del atributo \"Estado\", no corresponde a la clave del país registrado al ser extranjero.");
                                       if (string.IsNullOrEmpty(op.Domicilio.CodigoPostal))
                                           return ("CP215 - La clave del atributo \"CodigoPostal\", no corresponde a la clave del país registrado al ser extranjero.");
                   
                                   }
                               }
                           }

                           
                       }//fin-operador
                       if (carta.FiguraTransporte.Propietario != null)
                       { 
                              foreach (var pro in carta.FiguraTransporte.Propietario)
                              {
                                  if (pro.NombrePropietario == com.Emisor.Nombre && com.TipoDeComprobante == "I")
                                      return "CP217 - La información del propietario debe ser distinta a la del emisor del CFDI, o no debe existir este atributo.";
                  
                                  if(pro.RFCPropietario==com.Emisor.Rfc &&  com.TipoDeComprobante=="I" )
                                      return "CP218 - El valor registrado en el atributo \"RFCPropietario\" es el mismo que el RFC emisor del CFDI.";
                                  if (!string.IsNullOrEmpty(pro.RFCPropietario))
                                  {
                                      Operaciones_IRFC r = new Operaciones_IRFC();
                                      vI_RFC t = r.Consultar_IRFC(pro.RFCPropietario);
                                      if (t == null)
                                      {
                                          return "CP219 - El valor capturado en el atributo \"Propietario:RFCPropietario\"  no existe en la lista de contribuyentes no cancelados del SAT (l_RFC).";
                                      }
                                  }
                                  else
                                  {
                                       if(pro.ResidenciaFiscalPropietarioSpecified==false)
                                         return "CP223 - El atributo \"Propietario:ResidenciaFiscalPropietario\" tiene una clave diferente a las establecidas en el catálogo \"catCFDI:c_Pais\".";
                                        global::c_Pais myPais;
                                        Enum.TryParse<global::c_Pais>(pro.ResidenciaFiscalPropietario.ToString(), out myPais);
                                        if (myPais.ToString() != pro.ResidenciaFiscalPropietario.ToString())
                                            return "CP223 - El atributo \"Propietario:ResidenciaFiscalPropietario\" tiene una clave diferente a las establecidas en el catálogo \"catCFDI:c_Pais\".";
                                                        
                                   }
                                   if (string.IsNullOrEmpty(pro.NumRegIdTribPropietario) && string.IsNullOrEmpty(pro.RFCPropietario) && pro.ResidenciaFiscalPropietarioSpecified == true)
                                       return "CP220 - El atributo \"Propietario:RFCPropietario\" esta vacío o ya existe el atributo \"RFCPropietario\" con un RFC nacional.";
                                   if (!string.IsNullOrEmpty(pro.NumRegIdTribPropietario) && !string.IsNullOrEmpty(pro.RFCPropietario) && pro.ResidenciaFiscalPropietarioSpecified == true)
                                       return "CP220 - El atributo \"Propietario:RFCPropietario\" esta vacío o ya existe el atributo \"RFCPropietario\" con un RFC nacional.";

                                   if (!string.IsNullOrEmpty(pro.NumRegIdTribPropietario)&& string.IsNullOrEmpty(pro.RFCPropietario) && pro.ResidenciaFiscalPropietarioSpecified == true)
                                   {
                                       OperacionesCatalogos o13 = new OperacionesCatalogos();
                                       CatalogosSAT.c_Pais pais17 = o13.Consultar_PaisVerificacionLinea(pro.ResidenciaFiscalPropietario.ToString());
                                       if (pais17 != null)
                                       {
                                           if (pais17.ValidaciondelRIT != null)
                                           {
                                               Operaciones_IRFC r = new Operaciones_IRFC();
                                               vI_RFC t = r.Consultar_IRFC(pro.NumRegIdTribPropietario);
                                               if (t == null)
                                               {
                                                   return "CP222 - El atributo \"Propietario:NumRegIdTribPropietario\" no cumple con el patrón publicado en la columna \"Formato de Registro de Identidad Tributaria\" del país indicado en el atributo \"Propietario:ResidenciaFiscalPropietario\".";
                                               }
                                           }
                                           else if (pro.NumRegIdTribPropietario != null && pais17.FormatodeRIT != null && !Regex.Match(pro.NumRegIdTribPropietario, "^" + pais17.FormatodeRIT + "$").Success)
                                           {
                                               return "CP222 - El atributo \"Propietario:NumRegIdTribPropietario\" no cumple con el patrón publicado en la columna \"Formato de Registro de Identidad Tributaria\" del país indicado en el atributo \"Propietario:ResidenciaFiscalPropietario\".";
                                           }
                                       }
                                       else
                                           return "CP221 - El atributo \"Propietario:NumRegIdTribPropietario\" no tiene un valor que exista en el registro del país indicado en el atributo \"Propietario:ResidenciaFiscalPropietario\".";

                                   }

                                  if (pro.Domicilio.Pais == global::c_Pais.MEX)
                                   {
                                       CatalogosSAT.OperacionesCatalogos o = new CatalogosSAT.OperacionesCatalogos();
                                        CatalogosSAT.c_Colonia colonia5 = o.Consultar_Colonia(pro.Domicilio.CodigoPostal.ToString(), pro.Domicilio.Colonia.ToString());
                                       if (colonia5 == null)
                                           return ("CP225 - El atributo \"Domicilio:Colonia\" debe contener una clave del catálogo de \"catCFDI:c_Colonia\", donde la columna \"c_CodigoPostal\" debe ser igual a la clave registrada en el atributo \"CodigoPostal\".");

                                       CatalogosSAT.c_Localidad localidad5 = o.Consultar_Localidad(pro.Domicilio.Estado.ToString(), pro.Domicilio.Localidad.ToString());
                                       if (localidad5 == null)
                                           return ("CP227 - El atributo \"Propietario:Domicilio:Localidad\" debe contener una clave del catálogo de \"catCFDI:c_Localidad\", donde la columna clave de \"c_Estado\" debe ser igual a la clave registrada en el atributo estado si el atributo \"Pais\" tiene el valor \"MEX\".");

                                       CatalogosSAT.c_Municipio municipio5 = o.Consultar_Municipio(pro.Domicilio.Estado.ToString(), pro.Domicilio.Municipio.ToString());
                                       if (municipio5 == null)
                                           return ("CP229 - El atributo \"Propietario:Domicilio:Municipio\" debe contener una clave del catálogo de \"catCFDI:c_Municipio\" donde la columna clave de \"c_Estado\" debe ser igual a la clave registrada en el atributo \"Estado\" si el atributo \"Pais\" tiene el valor \"MEX\".");
                                       CatalogosSAT.c_Estado estado5 = o.Consultar_Estados(pro.Domicilio.Estado.ToString());
                                       if (estado5 == null)
                                           return ("CP231 - El atributo \"Propietario:Domicilio:Estado\" debe contener una clave del catálogo \"catCFDI:c_Estado\" donde la columna \"c_Pais\" tenga el valor \"MEX\" si el atributo \"Pais\" tiene el valor \"MEX\".");

                                       global::c_Pais myPais;
                                       Enum.TryParse<global::c_Pais>( pro.Domicilio.Pais.ToString(), out myPais);
                                       if (myPais.ToString() != pro.Domicilio.Pais.ToString())
                                           return "CP232 - El atributo \"Domicilio:Pais\" tiene un valor no permitido.";
                  
                                       CatalogosSAT.c_CP CP5 = o.Consultar_CP(pro.Domicilio.Estado.ToString(), pro.Domicilio.Municipio.ToString(), pro.Domicilio.Localidad.ToString(), pro.Domicilio.CodigoPostal.ToString());
                                       if (CP5 == null)
                                           return ("CP234 - El atributo \"Propietario:Domicilio:CodigoPostal\" debe contener una clave del catálogo de \"catCFDI:c_CodigoPostal\", donde la columna clave de \"c_Estado\" debe ser igual a la clave registrada en el atributo \"Estado\", la columna clave de \"c_Municipio\" debe ser igual a la clave registrada en el atributo \"Municipio\", y si existe el atributo de \"Localidad\", la columna clave de \"c_Localidad\" debe ser igual a la clave registrada en el atributo \"Localidad\" si el atributo \"Pais\" tiene el valor \"MEX\".");

                                   }
                                   else
                                   {
                                       if (string.IsNullOrEmpty(pro.Domicilio.Colonia))
                                           return ("CP224 - La clave del atributo \"Colonia\", no corresponde a la clave del país registrado al ser extranjero.");
                                       if (string.IsNullOrEmpty(pro.Domicilio.Localidad))
                                           return ("CP226 - La clave del atributo \"Localidad\" no corresponde a la clave del país registrado.");
                                       if (string.IsNullOrEmpty(pro.Domicilio.Municipio))
                                           return ("CP228 - La clave del atributo \"Municipio\", no corresponde a la clave del estado registrado al ser extranjero.");
                                       if (string.IsNullOrEmpty(pro.Domicilio.Estado))
                                           return ("CP230 - La clave del atributo \"Estado\", no corresponde a la clave del país registrado al ser extranjero.");
                                       if (string.IsNullOrEmpty(pro.Domicilio.CodigoPostal))
                                           return ("CP233 - La clave del atributo \"CodigoPostal\", no corresponde a la clave del país registrado al ser extranjero.");
                   
                                   }
                              }
                       }//fin-propietario

                       if (carta.FiguraTransporte.Arrendatario != null)
                       {
                           if(carta.FiguraTransporte.Arrendatario!=null)
                               foreach (var arr in carta.FiguraTransporte.Arrendatario)
                               {
                                   if (arr.NombreArrendatario == com.Emisor.Nombre && com.TipoDeComprobante == "I")
                                       return "CP235 - La información del arrendatario es igual a la del emisor del comprobante o no existe el nodo \"Arrendatario\".";

                                   if (arr.RFCArrendatario == com.Emisor.Rfc && com.TipoDeComprobante == "I")
                                       return "CP236 - El valor registrado en el atributo \"RFCArrendatario\" es el mismo que el RFC emisor del CFDI.";
                                   if (!string.IsNullOrEmpty(arr.RFCArrendatario))
                                   {
                                       Operaciones_IRFC r = new Operaciones_IRFC();
                                       vI_RFC t = r.Consultar_IRFC(arr.RFCArrendatario);
                                       if (t == null)
                                       {
                                           return "CP237 - El valor capturado en el atributo \"Arrendatario:RFCArrendatario\" no existe en la lista de RFC inscritos no cancelados del SAT (l_RFC).";
                                       }
                                   }
                                   else
                                   {
                                       if (arr.ResidenciaFiscalArrendatarioSpecified == false)
                                           return "CP241 - El atributo \"Arrendatario:ResidenciaFiscalArrendatario\" tiene una clave diferente a las establecidas en el catálogo \"catCFDI:c_Pais\".";
                                       global::c_Pais myPais;
                                       Enum.TryParse<global::c_Pais>(arr.ResidenciaFiscalArrendatario.ToString(), out myPais);
                                       if (myPais.ToString() != arr.ResidenciaFiscalArrendatario.ToString())
                                           return "CP241 - El atributo \"Arrendatario:ResidenciaFiscalArrendatario\" tiene una clave diferente a las establecidas en el catálogo \"catCFDI:c_Pais\".";

                                   }

                                   if (string.IsNullOrEmpty(arr.NumRegIdTribArrendatario) && string.IsNullOrEmpty(arr.RFCArrendatario) && arr.ResidenciaFiscalArrendatarioSpecified == true)
                                       return "CP238 - El atributo \"Arrendatario:RFCArrendatario\" esta vacío o ya existe el atributo \"RFCArrendatario\" con un RFC nacional.";
                                   if (!string.IsNullOrEmpty(arr.NumRegIdTribArrendatario) && !string.IsNullOrEmpty(arr.RFCArrendatario) && arr.ResidenciaFiscalArrendatarioSpecified == true)
                                       return "CP238 - El atributo \"Arrendatario:RFCArrendatario\" esta vacío o ya existe el atributo \"RFCArrendatario\" con un RFC nacional.";

                                   if (!string.IsNullOrEmpty(arr.NumRegIdTribArrendatario) && string.IsNullOrEmpty(arr.RFCArrendatario) && arr.ResidenciaFiscalArrendatarioSpecified == true)
                                   {
                                       OperacionesCatalogos o13 = new OperacionesCatalogos();
                                       CatalogosSAT.c_Pais pais17 = o13.Consultar_PaisVerificacionLinea(arr.ResidenciaFiscalArrendatario.ToString());
                                       if (pais17 != null)
                                       {
                                           if (pais17.ValidaciondelRIT != null)
                                           {
                                               Operaciones_IRFC r = new Operaciones_IRFC();
                                               vI_RFC t = r.Consultar_IRFC(arr.NumRegIdTribArrendatario);
                                               if (t == null)
                                               {
                                                   return "CP240 - El  atributo \"Arrendatario:NumRegIdTribArrendatario\" no cumple con el patrón publicado en la columna \"Formato de Registro de Identidad Tributaria\" del país indicado en el atributo \"Arrendatario:ResidenciaFiscalArrendatario\".";
                                               }
                                           }
                                           else if (arr.NumRegIdTribArrendatario != null && pais17.FormatodeRIT != null && !Regex.Match(arr.NumRegIdTribArrendatario, "^" + pais17.FormatodeRIT + "$").Success)
                                           {
                                               return "CP240 - El  atributo \"Arrendatario:NumRegIdTribArrendatario\" no cumple con el patrón publicado en la columna \"Formato de Registro de Identidad Tributaria\" del país indicado en el atributo \"Arrendatario:ResidenciaFiscalArrendatario\".";
                                           }
                                       }
                                       else
                                           return "CP239 - El  atributo \"Arrendatario:NumRegIdTribArrendatario\" no tiene un valor que exista en el registro del país indicado en el atributo \"Arrendatario:ResidenciaFiscalArrendatario\".";

                                   }
                                   if (arr.Domicilio.Pais == global::c_Pais.MEX)
                                   {
                                       CatalogosSAT.OperacionesCatalogos o = new CatalogosSAT.OperacionesCatalogos();
                                       CatalogosSAT.c_Colonia colonia5 = o.Consultar_Colonia(arr.Domicilio.CodigoPostal.ToString(), arr.Domicilio.Colonia.ToString());
                                       if (colonia5 == null)
                                           return ("CP243 - El atributo \"Domicilio:Colonia\" debe contener una clave del catálogo de \"catCFDI:c_Colonia\", donde la columna \"c_CodigoPostal\" debe ser igual a la clave registrada en el atributo \"CodigoPostal\".");

                                       CatalogosSAT.c_Localidad localidad5 = o.Consultar_Localidad(arr.Domicilio.Estado.ToString(), arr.Domicilio.Localidad.ToString());
                                       if (localidad5 == null)
                                           return ("CP245 - El atributo \"Arrendatario:Localidad\" debe contener una clave del catálogo de \"catCFDI:c_Localidad\", donde la columna clave de \"c_Estado\" debe ser igual a la clave registrada en el atributo \"Estado\" si el atributo \"Pais\" tiene el valor \"MEX\".");

                                       CatalogosSAT.c_Municipio municipio5 = o.Consultar_Municipio(arr.Domicilio.Estado.ToString(), arr.Domicilio.Municipio.ToString());
                                       if (municipio5 == null)
                                           return ("CP247 - El atributo \"Arrendatario:Municipio\" debe contener una clave del catálogo de \"catCFDI:c_Municipio\" donde la columna clave de \"c_Estado\" debe ser igual a la clave registrada en el atributo \"Estado\" si el atributo \"Pais\" tiene el valor \"MEX\".");
                                       CatalogosSAT.c_Estado estado5 = o.Consultar_Estados(arr.Domicilio.Estado.ToString());
                                       if (estado5 == null)
                                           return ("CP249 - El atributo \"Arrendatario:Estado\" debe contener una clave del catálogo \"catCFDI:c_Estado\" donde la columna \"c_Pais\" tenga el valor \"MEX\" si el atributo \"Pais\" tiene el valor \"MEX\".");

                                       global::c_Pais myPais;
                                       Enum.TryParse<global::c_Pais>(arr.Domicilio.Pais.ToString(), out myPais);
                                       if (myPais.ToString() != arr.Domicilio.Pais.ToString())
                                           return "CP250 - El atributo \"Domicilio:Pais\" tiene un valor no permitido.";

                                       CatalogosSAT.c_CP CP5 = o.Consultar_CP(arr.Domicilio.Estado.ToString(), arr.Domicilio.Municipio.ToString(), arr.Domicilio.Localidad.ToString(), arr.Domicilio.CodigoPostal.ToString());
                                       if (CP5 == null)
                                           return ("CP252 - El atributo \"Arrendatario:CodigoPostal\" debe contener una clave del catálogo de \"catCFDI:c_CodigoPostal\", donde la columna clave de \"c_Estado\" debe ser igual a la clave registrada en el atributo \"Estado\", la columna clave de \"c_Municipio\" debe ser igual a la clave registrada en el atributo \"Municipio\", y si existe el atributo de \"Localidad\", la columna clave de \"c_Localidad\" debe ser igual a la clave registrada en el atributo \"Localidad\" si el atributo Pais tiene el valor \"MEX\".");

                                   }
                                   else
                                   {
                                       if (string.IsNullOrEmpty(arr.Domicilio.Colonia))
                                           return ("CP242 - La clave del atributo \"Colonia\", no corresponde a la clave del país registrado al ser extranjero.");
                                       if (string.IsNullOrEmpty(arr.Domicilio.Localidad))
                                           return ("CP244 - La clave del atributo \"Localidad\" no corresponde a la clave del país registrado.");
                                       if (string.IsNullOrEmpty(arr.Domicilio.Municipio))
                                           return ("CP246 - La clave del atributo \"Municipio\", no corresponde a la clave del estado registrado al ser extranjero.");
                                       if (string.IsNullOrEmpty(arr.Domicilio.Estado))
                                           return ("CP248 - La clave del atributo \"Estado\", no corresponde a la clave del país registrado al ser extranjero.");
                                       if (string.IsNullOrEmpty(arr.Domicilio.CodigoPostal))
                                           return ("CP251 - La clave del atributo \"CodigoPostal\", no corresponde a la clave del país registrado al ser extranjero.");

                                   }
                               }
                       }//fin-arrendatario

                       if (carta.FiguraTransporte.Notificado != null)
                       {
                           

                           if(carta.FiguraTransporte.Notificado!=null)
                           foreach (var not in carta.FiguraTransporte.Notificado)
                           {
                                 if (carta.Mercancias.TransporteMaritimo!=null && string.IsNullOrEmpty(not.NumRegIdTribNotificado)&& string.IsNullOrEmpty(not.RFCNotificado))
                                      return "CP253 - Se debe registrar información en el atributo \"Notificado:RFCNotificado\" o ya existe información en el atributo \"Notificado:NumRegIDTribNotificado\".";
                                     if (carta.Mercancias.TransporteMaritimo!=null && !string.IsNullOrEmpty(not.NumRegIdTribNotificado)&& !string.IsNullOrEmpty(not.RFCNotificado))
                                      return "CP253 - Se debe registrar información en el atributo \"Notificado:RFCNotificado\" o ya existe información en el atributo \"Notificado:NumRegIDTribNotificado\".";
                                   if(!string.IsNullOrEmpty(not.RFCNotificado))
                                   {
                                       Operaciones_IRFC r = new Operaciones_IRFC();
                                               vI_RFC t = r.Consultar_IRFC(not.RFCNotificado);
                                               if (t == null)
                                               {
                                                   return "CP254 - El valor capturado en el atributo \"Notificado:RFCNotificado\" no existe en la lista de RFC inscritos no cancelados del SAT (l_RFC).";
                                               }
                                       
                                   }
                                   else
                                   {
                                      if(not.ResidenciaFiscalNotificadoSpecified==false)
                                          return "CP258 - El atributo \"Notificado:ResidenciaFiscalNotificado\" tiene una clave diferente a las establecidas en el catálogo \"catCFDI:c_Pais\".";
                                         global::c_Pais myPais;
                                        Enum.TryParse<global::c_Pais>(not.ResidenciaFiscalNotificado.ToString(), out myPais);
                                        if (myPais.ToString() != not.ResidenciaFiscalNotificado.ToString())
                                          return "CP258 - El atributo \"Notificado:ResidenciaFiscalNotificado\" tiene una clave diferente a las establecidas en el catálogo \"catCFDI:c_Pais\".";
                                        
                                   }

                                    if (string.IsNullOrEmpty(not.NumRegIdTribNotificado) && string.IsNullOrEmpty(not.RFCNotificado) && not.ResidenciaFiscalNotificadoSpecified == true)
                                                 return "CP255 - El atributo \"Notificado:RFCNotificado\" esta vacío o ya existe el atributo \"RFCNotificado\" con un RFC nacional.";
                                           if (!string.IsNullOrEmpty(not.NumRegIdTribNotificado) && !string.IsNullOrEmpty(not.RFCNotificado) && not.ResidenciaFiscalNotificadoSpecified == true)
                                                        return "CP255 - El atributo \"Notificado:RFCNotificado\" esta vacío o ya existe el atributo \"RFCNotificado\" con un RFC nacional.";
                                   
                                if (!string.IsNullOrEmpty(not.NumRegIdTribNotificado)&& string.IsNullOrEmpty(not.RFCNotificado) && not.ResidenciaFiscalNotificadoSpecified == true)
                                   {
                                       OperacionesCatalogos o13 = new OperacionesCatalogos();
                                       CatalogosSAT.c_Pais pais17 = o13.Consultar_PaisVerificacionLinea(not.ResidenciaFiscalNotificado.ToString());
                                       if (pais17 != null)
                                       {
                                           if (pais17.ValidaciondelRIT != null)
                                           {
                                               Operaciones_IRFC r = new Operaciones_IRFC();
                                               vI_RFC t = r.Consultar_IRFC(not.NumRegIdTribNotificado);
                                               if (t == null)
                                               {
                                                   return "CP257 - El atributo \"NumRegIdTribNotificado\" no cumple con el patrón publicado en la columna \"Formato de registro de identidad tributaria\" del país indicado en el atributo \"Notificado:ResidenciaFiscalNotificado\".";
                                               }
                                           }
                                           else if (not.NumRegIdTribNotificado != null && pais17.FormatodeRIT != null && !Regex.Match(not.NumRegIdTribNotificado, "^" + pais17.FormatodeRIT + "$").Success)
                                           {
                                            return "CP257 - El atributo \"NumRegIdTribNotificado\" no cumple con el patrón publicado en la columna \"Formato de registro de identidad tributaria\" del país indicado en el atributo \"Notificado:ResidenciaFiscalNotificado\".";
                                            }
                                       }
                                       else
                                           return "CP256 - El atributo \"NumRegIdTribNotificado\" no contiene un valor que exista en el registro del país indicado en el atributo \"Notificado:ResidenciaFiscalNotificado\".";

                                   }
                               if (not.Domicilio.Pais == global::c_Pais.MEX)
                                   {
                                       CatalogosSAT.OperacionesCatalogos o = new CatalogosSAT.OperacionesCatalogos();
                                       CatalogosSAT.c_Colonia colonia5 = o.Consultar_Colonia(not.Domicilio.CodigoPostal.ToString(), not.Domicilio.Colonia.ToString());
                                       if (colonia5 == null)
                                           return ("CP260 - El atributo \"Domicilio:Colonia\" debe contener una clave del catálogo de \"catCFDI:c_Colonia\", donde la columna \"c_CodigoPostal\" debe ser igual a la clave registrada en el atributo \"CodigoPostal\".");

                                       CatalogosSAT.c_Localidad localidad5 = o.Consultar_Localidad(not.Domicilio.Estado.ToString(), not.Domicilio.Localidad.ToString());
                                       if (localidad5 == null)
                                           return ("CP262 - El atributo \"Notificado:Domicilio:Localidad\" debe contener una clave del catálogo de \"catCFDI:c_Localidad\", donde la columna clave de \"c_Estado\" debe ser igual a la clave registrada en el atributo \"Estado\" si el atributo \"Pais\" tiene el valor \"MEX\".");

                                       CatalogosSAT.c_Municipio municipio5 = o.Consultar_Municipio(not.Domicilio.Estado.ToString(), not.Domicilio.Municipio.ToString());
                                       if (municipio5 == null)
                                           return ("CP264 - El atributo \"Notificado:Domicilio:Municipio\" debe contener una clave del catálogo de \"catCFDI:c_Municipio\" donde la columna clave de \"c_Estado\" debe ser igual a la clave registrada en el atributo \"Estado\" si el atributo \"Pais\" tiene el valor \"MEX\".");
                                       CatalogosSAT.c_Estado estado5 = o.Consultar_Estados(not.Domicilio.Estado.ToString());
                                       if (estado5 == null)
                                           return ("CP266 - El atributo \"Notificado:Domicilio:Estado\" debe contener una clave del catálogo \"catCFDI:c_Estado\" donde la columna \"c_Pais\" tenga el valor \"MEX\" si el atributo \"Pais\" tiene el valor \"MEX\".");

                                       global::c_Pais myPais;
                                       Enum.TryParse<global::c_Pais>(not.Domicilio.Pais.ToString(), out myPais);
                                       if (myPais.ToString() != not.Domicilio.Pais.ToString())
                                           return "CP267 - El atributo \"Domicilio:Pais\" tiene un valor no permitido.";

                                       CatalogosSAT.c_CP CP5 = o.Consultar_CP(not.Domicilio.Estado.ToString(), not.Domicilio.Municipio.ToString(), not.Domicilio.Localidad.ToString(), not.Domicilio.CodigoPostal.ToString());
                                       if (CP5 == null)
                                           return ("CP269 - El atributo \"Notificado:Domicilio:CodigoPostal\" debe contener una clave del catálogo de \"catCFDI:c_CodigoPostal\", donde la columna clave de \"c_Estado\" debe ser igual a la clave registrada en el atributo \"Estado\", la columna clave de \"c_Municipio\" debe ser igual a la clave registrada en el atributo \"Municipio\", y si existe el atributo de \"Localidad\", la columna clave de \"c_Localidad\" debe ser igual a la clave registrada en el atributo \"Localidad\" si el atributo \"Pais\" tiene el valor \"MEX\".");

                                   }
                                   else
                                   {
                                       if (string.IsNullOrEmpty(not.Domicilio.Colonia))
                                           return ("CP259 - La clave del atributo \"Colonia\", no corresponde a la clave del país registrado al ser extranjero.");
                                       if (string.IsNullOrEmpty(not.Domicilio.Localidad))
                                           return ("CP261 - La clave del atributo \"Localidad\" no corresponde a la clave del país registrado.");
                                       if (string.IsNullOrEmpty(not.Domicilio.Municipio))
                                           return ("CP263 - La clave del atributo \"Municipio\", no corresponde a la clave del estado registrado al ser extranjero.");
                                       if (string.IsNullOrEmpty(not.Domicilio.Estado))
                                           return ("CP265 - La clave del atributo \"Estado\", no corresponde a la clave del país registrado al ser extranjero.");
                                       if (string.IsNullOrEmpty(not.Domicilio.CodigoPostal))
                                           return ("CP268 - La clave del atributo \"CodigoPostal\", no corresponde a la clave del país registrado al ser extranjero.");

                                   }


                           }
                       }//fin-notificado
                   }
                   else
                   {
                       if (carta.Mercancias.AutotransporteFederal != null)
                           return "CP199 - El nodo \"Operadores\" debe existir, siempre que exista el nodo \"Mercancia:AutotransporteFederal\".";
                      

                   }
 

                return "0";
            }
            catch (Exception ex)
            {
                return "CP00 - Error en los datos de la carta porte";
            }
        }
        //-------------------------------------------------------------------------------------
       

    }

}