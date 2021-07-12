

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
using System.Web;
using CatalogosSAT;


namespace ServicioLocal.Business
{
    public class ValidarCFDI33 : NtLinkBusiness
    {
        private readonly XNamespace _ns = "http://www.sat.gob.mx/cfd/3";

        public ValidarCFDI33()
        {
            XmlConfigurator.Configure();
        }

        public string ProcesarCFDI33(Comprobante com, string xml, bool pago10, bool ComerExt, ImpuestosLocales IL)
        {
            if (string.IsNullOrEmpty(com.LugarExpedicion))
            {
                return "Error de estructura el campo LugarExpedicion es obligatorio.";
                     
            }

          
            bool FueraLimiteTipoCambio = false;
            bool FueraLimiteTotal = false;
            Regex regex7 = new Regex("(20[1-9][0-9])-(0[1-9]|1[0-2])-(0[1-9]|[12][0-9]|3[01])T(([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9])");
            string result;
            if (!regex7.IsMatch(com.Fecha))
            {
                result = "CFDI33101 - El campo Fecha no cumple con el patrón requerido.";
            }
            else
            {
                string RFCEmisor = com.Emisor.Rfc;
                string NomEmisor = com.Emisor.Nombre;
                string CURP = "";
                string cadenaOriginal;
                if (string.IsNullOrEmpty(com.CadenaOriginal))
                {
                    GeneradorCadenas gen = new GeneradorCadenas();
                    XElement xeComprobante = XElement.Parse(xml);
                    SidetecStringWriter sw = new SidetecStringWriter(Encoding.UTF8);
                    xeComprobante.Save(sw, SaveOptions.DisableFormatting);
                    cadenaOriginal = gen.CadenaOriginal(sw.ToString());
                }
                else
                {
                    cadenaOriginal = com.CadenaOriginal;
                }
              //  string x = "OK";
                string x = this.ValidaSelloDigital(com.Sello, com.Certificado, cadenaOriginal);
                if (x != "OK")
                {
                    result = "CFDI33102 - El resultado de la digestión debe ser igual al resultado de la desencripción del sello.";
                }
                else if (pago10 && com.FormaPagoSpecified)
                {
                    result = "CFDI33103 - Si existe el complemento para recepción de pagos este campo FormaPago no debe existir.";
                }
                else
                {
                    string s = this.validarCertificado(com.Certificado, com.NoCertificado, Convert.ToDateTime(com.Fecha), RFCEmisor, com.Version, NomEmisor, CURP);
                    if (s != "OK")
                    {
                        result = s;
                    }
                    else
                    {
                        c_Moneda myTipoMoneda;
                        Enum.TryParse<c_Moneda>(com.Moneda, out myTipoMoneda);
                        if (myTipoMoneda.ToString() != com.Moneda)
                        {
                            result = "CFDI33112 - El campo Moneda no contiene un valor del catálogo c_Moneda.";
                        }
                        else
                        {
                            OperacionesCatalogos o9 = new OperacionesCatalogos();
                            CatalogosSAT.c_Moneda mone = o9.Consultar_Moneda(com.Moneda);
                            string sub = com.SubTotal.ToString();
                            if(com.SubTotal!=0 && mone.Decimales!=0)
                            if (sub != null)
                            {
                                string[] split = sub.Split(".".ToCharArray());
                                if (split.Count<string>() > 1)
                                {
                                    if (split[1].Count<char>() > (int)Convert.ToInt16(mone.Decimales))
                                    {
                                        result = "CFDI33106 - El valor de este campo SubTotal excede la cantidad de decimales que soporta la moneda.";
                                        return result;
                                    }
                                }
                                else if (0 != Convert.ToInt16(mone.Decimales))
                                {
                                    result = "CFDI33106 - El valor de este campo SubTotal excede la cantidad de decimales que soporta la moneda.";
                                    return result;
                                }
                            }
                            foreach (ComprobanteConcepto con in com.Conceptos)
                            {
                                if (con.DescuentoSpecified && con.Descuento > con.Importe)
                                {
                                    result = "CFDI33151 - El valor del campo Descuento es mayor que el campo Importe.";
                                    return result;
                                }
                            }
                            if (com.Descuento > com.SubTotal)
                            {
                                result = "CFDI33109 - El valor registrado en el campo Descuento no es menor o igual que el campo Subtotal.";
                            }
                            else
                            {
                                if (com.TipoDeComprobante == "T" || com.TipoDeComprobante == "P" || com.TipoDeComprobante == "N")
                                {
                                    if (!string.IsNullOrEmpty(com.CondicionesDePago)) //no se que numero le corresponde
                                    {
                                        result = "CFDI33200 - El TipoDeComprobante es T,P o N, CondicionesDePago no debe existir.";
                                        return result;
                                    }
                                }

                                if (com.TipoDeComprobante == "I" || com.TipoDeComprobante == "E" || com.TipoDeComprobante == "N")
                                {
                                    if (com.FormaPagoSpecified == false) //no se que numero le corresponde
                                    {
                                        result = "CFDI33197 - El TipoDeComprobante es I,E o N, forma de pago debe existir.";
                                        return result;
                                    }
                                    else
                                    {
                                        c_FormaPago myFormaPago;
                                        Enum.TryParse<c_FormaPago>(com.FormaPago, out myFormaPago);
                                        if (myFormaPago.ToString() != com.FormaPago)//no se que numero le corresponde
                                        {
                                            result = "CFDI33199 - El campo FormaPago, no contiene un valor del catálogo c_FormaPago.";
                                        }
                                    }
                                    if (com.MetodoPagoSpecified == false) //no se que numero le corresponde
                                    {
                                        result = "CFDI33198 - El TipoDeComprobante es I,E o N, metodo de pago debe existir.";
                                        return result;
                                    }
                                    

                                    decimal sumaconceptos = 0m;
                                    decimal sumadescuento = 0m;
                                    foreach (ComprobanteConcepto con in com.Conceptos)
                                    {
                                        sumaconceptos += con.Importe;
                                        if (con.DescuentoSpecified)
                                        {
                                            sumadescuento += con.Descuento;
                                        }
                                    }
                                    decimal uno = decimal.Round(sumaconceptos, mone.Decimales.Value, MidpointRounding.AwayFromZero);
                                    decimal desc = decimal.Round(sumadescuento, mone.Decimales.Value, MidpointRounding.AwayFromZero);
                                    decimal subt=decimal.Round(com.SubTotal, mone.Decimales.Value, MidpointRounding.AwayFromZero);
                                    if (uno != subt)
                                    {
                                        result = "CFDI33107 - El TipoDeComprobante es I,E o N, el importe registrado en el campo no es igual a la suma de los importes de los conceptos registrados.";
                                        return result;
                                    }
                                    if (desc != com.Descuento)
                                    {
                                        result = "CFDI33110 - El TipoDeComprobante NO es I,E o N, y un concepto incluye el campo descuento.";
                                        return result;
                                    }
                                }
                                if (com.TipoDeComprobante == "T" || com.TipoDeComprobante == "P")
                                {
                                    if (com.SubTotal != 0m)
                                    {
                                        result = "CFDI33108 - El TipoDeComprobante es T o P y el importe no es igual a 0, o cero con decimales.";
                                        return result;
                                    }
                                }
                                if (com.TipoDeComprobante == "T" || com.TipoDeComprobante == "P")
                                {
                                    if (com.Impuestos != null)
                                    {
                                        result = "CFDI33179 - Cuando el TipoDeComprobante sea T o P, este elemento Impuestos no debe existir";
                                        return result;
                                    }
                                }
                                string des = com.Descuento.ToString();
                                if (des != null)
                                {
                                    if (des != "0")
                                    {
                                        string[] split = des.Split(".".ToCharArray());
                                        if (split.Count<string>() <= 1)
                                        {
                                            result = "CFDI33111 - El valor del campo Descuento excede la cantidad de decimales que soporta la moneda.";
                                            return result;
                                        }
                                        if (split[1].Count<char>() > (int)Convert.ToInt16(mone.Decimales))
                                        {
                                            result = "CFDI33111 - El valor del campo Descuento excede la cantidad de decimales que soporta la moneda.";
                                            return result;
                                        }
                                    }
                                }
                                if (com.Moneda == "MXN")
                                {
                                    if (com.TipoCambioSpecified)
                                    {
                                        if (com.TipoCambio != 1m)
                                        {
                                            result = "CFDI33113 - El campo TipoCambio no tiene el valor \"1\" y la moneda indicada es MXN.";
                                            return result;
                                        }
                                        XElement element = XElement.Load(new StringReader(xml));
                                        string tipocambio = element.Attribute("TipoCambio").Value;
                                        if (tipocambio != "1")
                                        {
                                            result = "CFDI33113 - El campo TipoCambio no tiene el valor \"1\" y la moneda indicada es MXN.";
                                            return result;
                                        }
                                    }
                                }
                                else if (com.Moneda != "XXX")
                                {
                                    if (!com.TipoCambioSpecified)
                                    {
                                        result = "CFDI33114 - El campo TipoCambio se debe registrar cuando el campo Moneda tiene un valor distinto de MXN y XXX.";
                                        return result;
                                    }
                                }
                                else if (com.TipoCambioSpecified)
                                {
                                    result = "CFDI33115 - El campo TipoCambio no se debe registrar cuando el campo Moneda tiene el valor XXX.";
                                    return result;
                                }
                                string tip = com.TipoCambio.ToString();
                                if (tip != null)
                                {
                                    if (tip != "0")
                                    {
                                        string[] split = tip.Split(".".ToCharArray());
                                        if (split.Count<string>() > 1)
                                        {
                                            if (split[1].Count<char>() < 1 || split[1].Count<char>() > 6)
                                            {
                                                result = "CFDI33116 - El campo TipoCambio no cumple con el patrón requerido.";
                                                return result;
                                            }
                                        }
                                        if (split[0].Count<char>() < 1 || split[0].Count<char>() > 18)
                                        {
                                            result = "CFDI33116 - El campo TipoCambio no cumple con el patrón requerido.";
                                            return result;
                                        }
                                    }
                                }
                                if (mone != null)
                                {
                                    string varia = mone.Variacion;
                                    OperacionesCatalogos o10 = new OperacionesCatalogos();
                                    Divisas divisa = o10.Consultar_TipoDivisa(com.Moneda);
                                    if (divisa != null)
                                    {
                                        decimal inferior = this.CalculoInferiorPorcentajeMoneda(divisa.PesosDivisa, (int)Convert.ToInt16(varia));
                                        decimal superior = this.CalculoSuperiorPorcentajeMoneda(divisa.PesosDivisa, (int)Convert.ToInt16(varia));
                                        if (com.TipoCambio < inferior)
                                        {
                                            FueraLimiteTipoCambio = true;
                                            if (string.IsNullOrEmpty(com.Confirmacion))
                                            {
                                                NtLinkEmpresa Empresa = new NtLinkEmpresa();
                                                ConfirmacionTimbreWs33 Confir = new ConfirmacionTimbreWs33();
                                                Confir.Error = "CFDI33117 - Cuando el valor del campo TipoCambio se encuentre fuera de los límites establecidos, debe existir el campo Confirmación.";
                                                Confir.FechaFactura = Convert.ToDateTime(com.Fecha);
                                                Confir.Folio = com.Folio;
                                                Confir.procesado = new bool?(false);
                                                if (com.Receptor != null)
                                                {
                                                    Confir.RfcReceptor = com.Receptor.Rfc;
                                                }
                                                if (com.Emisor != null)
                                                {
                                                    Confir.RfcEmisor = com.Emisor.Rfc;
                                                }
                                                Confir.Xml = xml;
                                                bool Conf = Empresa.SaveConfirmacion(Confir);
                                                result = "CFDI33117 - Cuando el valor del campo TipoCambio se encuentre fuera de los límites establecidos, debe existir el campo Confirmación.(Solicitar en el portal de facturación NT Link menú Reportes/Confirmaciones)";
                                                return result;
                                            }
                                        }
                                        if (com.TipoCambio > superior)
                                        {
                                            FueraLimiteTipoCambio = true;
                                            if (string.IsNullOrEmpty(com.Confirmacion))
                                            {
                                                NtLinkEmpresa Empresa = new NtLinkEmpresa();
                                                ConfirmacionTimbreWs33 Confir = new ConfirmacionTimbreWs33();
                                                Confir.Error = "CFDI33117 - Cuando el valor del campo TipoCambio se encuentre fuera de los límites establecidos, debe existir el campo Confirmación.";
                                                Confir.FechaFactura = Convert.ToDateTime(com.Fecha);
                                                Confir.Folio = com.Folio;
                                                Confir.procesado = new bool?(false);
                                                if (com.Receptor != null)
                                                {
                                                    Confir.RfcReceptor = com.Receptor.Rfc;
                                                }
                                                if (com.Emisor != null)
                                                {
                                                    Confir.RfcEmisor = com.Emisor.Rfc;
                                                }
                                                Confir.Xml = xml;
                                                bool Conf = Empresa.SaveConfirmacion(Confir);
                                                result = "CFDI33117 - Cuando el valor del campo TipoCambio se encuentre fuera de los límites establecidos, debe existir el campo Confirmación.(Solicitar en el portal de facturación NT Link menú Reportes/Confirmaciones)";
                                                return result;
                                            }
                                        }
                                    }
                                }
                                decimal retenciones = 0m;
                                decimal traslados = 0m;
                                decimal descuento = 0m;
                                foreach (ComprobanteConcepto con in com.Conceptos)
                                {
                                    if (con.DescuentoSpecified)
                                    {
                                        descuento += con.Descuento;
                                    }
                                    if (con.Impuestos != null)
                                    {
                                        if (con.Impuestos.Retenciones != null)
                                        {
                                            ComprobanteConceptoImpuestosRetencion[] retenciones2 = con.Impuestos.Retenciones;
                                            for (int j = 0; j < retenciones2.Length; j++)
                                            {
                                                ComprobanteConceptoImpuestosRetencion ret5 = retenciones2[j];
                                                decimal arg_9DB_0 = ret5.Importe;
                                                bool flag = 1 == 0;
                                                retenciones += ret5.Importe;
                                            }
                                        }
                                        if (con.Impuestos.Traslados != null)
                                        {
                                            ComprobanteConceptoImpuestosTraslado[] traslados2 = con.Impuestos.Traslados;
                                            for (int j = 0; j < traslados2.Length; j++)
                                            {
                                                ComprobanteConceptoImpuestosTraslado ret2 = traslados2[j];
                                                if (ret2.ImporteSpecified)
                                                {
                                                    traslados += ret2.Importe;
                                                }
                                            }
                                        }
                                    }
                                }
                                decimal ILR = 0m;
                                decimal ILT = 0m;
                                if (IL != null)
                                {
                                    if (IL.RetencionesLocales != null)
                                    {
                                        ImpuestosLocalesRetencionesLocales[] retencionesLocales = IL.RetencionesLocales;
                                        for (int j = 0; j < retencionesLocales.Length; j++)
                                        {
                                            ImpuestosLocalesRetencionesLocales ret3 = retencionesLocales[j];
                                            ILR += ret3.Importe;
                                        }
                                    }
                                    if (IL.TrasladosLocales != null)
                                    {
                                        ImpuestosLocalesTrasladosLocales[] trasladosLocales = IL.TrasladosLocales;
                                        for (int j = 0; j < trasladosLocales.Length; j++)
                                        {
                                            ImpuestosLocalesTrasladosLocales ret4 = trasladosLocales[j];
                                            ILT += ret4.Importe;
                                        }
                                    }
                                }
                                retenciones += ILR;
                                traslados += ILT;
                             //nuevo-----
                                traslados = decimal.Round(traslados, mone.Decimales.Value);
                                retenciones = decimal.Round(retenciones, mone.Decimales.Value);
                                descuento = decimal.Round(descuento, mone.Decimales.Value);
                               // ---
                                decimal subtotal = com.SubTotal + traslados - retenciones - descuento;
                                decimal subT = decimal.Round(subtotal, mone.Decimales.Value, MidpointRounding.AwayFromZero);
                                if (subT != com.Total)
                                {
                                    result = "CFDI33118 - El campo Total no corresponde con la suma del subtotal, menos los descuentos aplicables, más las contribuciones recibidas (impuestos trasladados - federales o locales, derechos, productos, aprovechamientos, aportaciones de seguridad social, contribuciones de mejoras) menos los impuestos retenidos.";
                                }
                                else
                                {
                                    OperacionesCatalogos o11 = new OperacionesCatalogos();
                                    CatalogosSAT.c_TipoDeComprobante TCP = o11.Consultar_TipoDeComprobante(com.TipoDeComprobante);
                                    if (TCP != null)
                                    {
                                        decimal total = com.Total;
                                        long? valor_máximo = TCP.Valor_máximo;
                                        if (total > valor_máximo.GetValueOrDefault() && valor_máximo.HasValue)
                                        {
                                            FueraLimiteTotal = true;
                                            if (string.IsNullOrEmpty(com.Confirmacion))
                                            {
                                                NtLinkEmpresa Empresa = new NtLinkEmpresa();
                                                ConfirmacionTimbreWs33 Confir = new ConfirmacionTimbreWs33();
                                                Confir.Error = "CFDI33119 - Cuando el valor del campo Total se encuentre fuera de los límites establecidos, debe existir el campo Confirmacion.";
                                                Confir.FechaFactura = Convert.ToDateTime(com.Fecha);
                                                Confir.Folio = com.Folio;
                                                Confir.procesado = new bool?(false);
                                                if (com.Receptor != null)
                                                {
                                                    Confir.RfcReceptor = com.Receptor.Rfc;
                                                }
                                                if (com.Emisor != null)
                                                {
                                                    Confir.RfcEmisor = com.Emisor.Rfc;
                                                }
                                                Confir.Xml = xml;
                                                bool Conf = Empresa.SaveConfirmacion(Confir);
                                                result = "CFDI33119 - Cuando el valor del campo Total se encuentre fuera de los límites establecidos, debe existir el campo Confirmacion.";
                                                return result;
                                            }
                                        }
                                    }
                                    c_TipoDeComprobante myTipoComprobante;
                                    Enum.TryParse<c_TipoDeComprobante>(com.TipoDeComprobante, out myTipoComprobante);
                                    if (myTipoComprobante.ToString() != com.TipoDeComprobante)
                                    {
                                        result = "CFDI33120 - El campo TipoDeComprobante, no contiene un valor del catálogo c_TipoDeComprobante.";
                                    }
                                    else
                                    {
                                        if (com.MetodoPagoSpecified)
                                        {
                                            c_MetodoPago myTipoMetodoPago;
                                            Enum.TryParse<c_MetodoPago>(com.MetodoPago, out myTipoMetodoPago);
                                            if (myTipoMetodoPago.ToString() != com.MetodoPago)
                                            {
                                                result = "CFDI33121 - El campo MetodoPago, no contiene un valor del catálogo c_MetodoPago.";
                                                return result;
                                            }
                                            if (com.MetodoPago == "PIP")
                                            {
                                                if (!pago10)
                                                {
                                                    result = "CFDI33122 - Cuando se tiene el valor PIP en el campo MetodoPago y el valor en el campo TipoDeComprobante es I ó E, el CFDI debe contener un complemento de recibo de pago.";
                                                    return result;
                                                }
                                            }
                                        }
                                        if (com.TipoDeComprobante == "T" || com.TipoDeComprobante == "P")
                                        {
                                            if (com.MetodoPagoSpecified)
                                            {
                                                result = "CFDI33123 - Se debe omitir el campo MetodoPago cuando el TipoDeComprobante es T o P.";
                                                return result;
                                            }
                                        }
                                        if (pago10)
                                        {
                                            if (com.MetodoPagoSpecified)
                                            {
                                                result = "CFDI33124 - Si existe el complemento para recepción de pagos en este CFDI este campo MetodoPago no debe existir.";
                                                return result;
                                            }
                                        }
                                        if (!string.IsNullOrEmpty(com.LugarExpedicion))
                                        {
                                            ServicioLocal.Business.CatalogosGrandes.c_CodigoPostal myTipoCodigoPostal;
                                            Enum.TryParse<ServicioLocal.Business.CatalogosGrandes.c_CodigoPostal>("Item" + com.LugarExpedicion.ToString(), out myTipoCodigoPostal);
                                            if (myTipoCodigoPostal.ToString() != "Item" + com.LugarExpedicion.ToString())
                                            {
                                                result = "CFDI33125 - El campo LugarExpedicion, no contiene un valor del catálogo c_LugarExpedicion.";
                                                return result;
                                            }
                                        }
                                        if (!FueraLimiteTipoCambio && !FueraLimiteTotal && !string.IsNullOrEmpty(com.Confirmacion))
                                        {
                                            result = "CFDI33126 - El campo Confirmacion no debe existir cuando los atributos TipoCambio y/o Total están dentro del rango permitido.";
                                        }
                                        else
                                        {
                                            if (!string.IsNullOrEmpty(com.Confirmacion))
                                            {
                                                NtLinkEmpresa Empresa = new NtLinkEmpresa();
                                                ConfirmacionTimbreWs33 Confirmado = Empresa.GetConfirmacion(com.Emisor.Rfc, com.Receptor.Rfc, com.Folio);
                                                if (Empresa.GetConfirmacionExiste(com.Confirmacion))
                                                {
                                                    result = "CFDI33128 - Número de confirmación utilizado previamente.";
                                                    return result;
                                                }
                                                if (Confirmado == null)
                                                {
                                                    result = "CFDI33127 - Número de confirmación inválido.";
                                                    return result;
                                                }
                                                if (Confirmado.Confirmacion != com.Confirmacion)
                                                {
                                                    result = "CFDI33127 - Número de confirmación inválido.";
                                                    return result;
                                                }
                                            }
                                            if (com.CfdiRelacionados != null)
                                            {
                                                c_TipoRelacion myTipoTipoRelacion;
                                                Enum.TryParse<c_TipoRelacion>("Item" + com.CfdiRelacionados.TipoRelacion.ToString(), out myTipoTipoRelacion);
                                                if (myTipoTipoRelacion.ToString() != "Item" + com.CfdiRelacionados.TipoRelacion.ToString())
                                                {
                                                    result = "CFDI33129 - El campo TipoRelacion, no contiene un valor del catálogo c_TipoRelacion.";
                                                    return result;
                                                }
                                            }
                                            c_RegimenFiscal myTipoRegimenFiscal;
                                            Enum.TryParse<c_RegimenFiscal>("Item" + com.Emisor.RegimenFiscal, out myTipoRegimenFiscal);
                                            if (myTipoRegimenFiscal.ToString() != "Item" + com.Emisor.RegimenFiscal)
                                            {
                                                result = "CFDI33130 - El campo RegimenFiscal, no contiene un valor del catálogo c_RegimenFiscal.";
                                            }
                                            else
                                            {
                                                OperacionesCatalogos o12 = new OperacionesCatalogos();
                                                CatalogosSAT.c_RegimenFiscal rf = o12.Consultar_RegimenFiscal(com.Emisor.RegimenFiscal);
                                                if (rf != null)
                                                {
                                                    if (!string.IsNullOrEmpty(com.Emisor.Rfc))
                                                    {
                                                        if (com.Emisor.Rfc.Count<char>() == 13)
                                                        {
                                                            if (rf.Física == "No")
                                                            {
                                                                result = "CFDI33131 - La clave del campo RegimenFiscal debe corresponder con el tipo de persona (fisica o moral).";
                                                                return result;
                                                            }
                                                        }
                                                        else if (rf.Moral == "No")
                                                        {
                                                            result = "CFDI33131 - La clave del campo RegimenFiscal debe corresponder con el tipo de persona (fisica o moral).";
                                                            return result;
                                                        }
                                                    }
                                                }
                                                if (com.Receptor.Rfc != "XAXX010101000" && com.Receptor.Rfc != "XEXX010101000")
                                                {
                                                    Operaciones_IRFC r = new Operaciones_IRFC();
                                                    vI_RFC t = r.Consultar_IRFC(com.Receptor.Rfc);
                                                    if (t == null)
                                                    {
                                                        result = "CFDI33132 - Este RFC del receptor no existe en la lista de RFC inscritos no cancelados del SAT.";
                                                        return result;
                                                    }
                                                }
                                                if (com.Receptor != null)
                                                {
                                                    if (com.Receptor.ResidenciaFiscalSpecified)
                                                    {
                                                        c_Pais myTipoRecidenciaFiscal;
                                                        Enum.TryParse<c_Pais>(com.Receptor.ResidenciaFiscal.ToString(), out myTipoRecidenciaFiscal);
                                                        if (myTipoRecidenciaFiscal.ToString() != com.Receptor.ResidenciaFiscal.ToString())
                                                        {
                                                            result = "CFDI33133 - El campo ResidenciaFiscal, no contiene un valor del catálogo c_Pais.";
                                                            return result;
                                                        }
                                                    }
                                                    if (com.Receptor.ResidenciaFiscalSpecified && com.Receptor.Rfc != "XEXX010101000")
                                                    {
                                                        result = "CFDI33134 - El RFC del receptor es de un RFC registrado en el SAT o un RFC genérico nacional y EXISTE el campo ResidenciaFiscal.";
                                                        return result;
                                                    }
                                                    if (com.Receptor.ResidenciaFiscalSpecified && com.Receptor.ResidenciaFiscal.ToString() == "MEX")
                                                    {
                                                        result = "CFDI33135 - El valor del campo ResidenciaFiscal no puede ser MEX.";
                                                        return result;
                                                    }
                                                    if (((com.Receptor.Rfc == "XEXX010101000" && ComerExt) || (com.Receptor.Rfc == "XEXX010101000" && !string.IsNullOrEmpty(com.Receptor.NumRegIdTrib))) && !com.Receptor.ResidenciaFiscalSpecified)
                                                    {
                                                        result = "CFDI33136 - Se debe registrar un valor de acuerdo al catálogo c_Pais en en el campo ResidenciaFiscal, cuando en el en el campo NumRegIdTrib se registre información.";
                                                        return result;
                                                    }
                                                    if (com.Receptor.Rfc != "XEXX010101000" && !string.IsNullOrEmpty(com.Receptor.NumRegIdTrib))
                                                    {
                                                        result = "CFDI33137 - El valor del campo es un RFC inscrito no cancelado en el SAT o un RFC genérico nacional, y se registró el campo NumRegIdTrib.";
                                                        return result;
                                                    }
                                                    if (com.Receptor.Rfc == "XEXX010101000" && ComerExt && string.IsNullOrEmpty(com.Receptor.NumRegIdTrib))
                                                    {
                                                        result = "CFDI33138 - Para registrar el campo NumRegIdTrib, el CFDI debe contener el complemento de comercio exterior y el RFC del receptor debe ser un RFC genérico extranjero.";
                                                        return result;
                                                    }
                                                    if (com.Receptor.ResidenciaFiscalSpecified)
                                                    {
                                                        OperacionesCatalogos o13 = new OperacionesCatalogos();
                                                        CatalogosSAT.c_Pais pais17 = o13.Consultar_PaisVerificacionLinea(com.Receptor.ResidenciaFiscal.ToString());
                                                        if (pais17 != null)
                                                        {
                                                            if (pais17.ValidaciondelRIT != null)
                                                            {
                                                                Operaciones_IRFC r = new Operaciones_IRFC();
                                                                vI_RFC t = r.Consultar_IRFC(com.Receptor.NumRegIdTrib);
                                                                if (t == null)
                                                                {
                                                                    result = "CFDI33139 - El campo NumRegIdTrib no cumple con el patrón correspondiente.";
                                                                    return result;
                                                                }
                                                            }
                                                            else if (com.Receptor.NumRegIdTrib != null && pais17.FormatodeRIT != null && !Regex.Match(com.Receptor.NumRegIdTrib, "^" + pais17.FormatodeRIT + "$").Success)
                                                            {
                                                                result = "CFDI33139 - El campo NumRegIdTrib no cumple con el patrón correspondiente.";
                                                                return result;
                                                            }
                                                        }
                                                    }
                                                    c_UsoCFDI myTipoUsoCFDI;
                                                    Enum.TryParse<c_UsoCFDI>(com.Receptor.UsoCFDI.ToString(), out myTipoUsoCFDI);
                                                    if (myTipoUsoCFDI.ToString() != com.Receptor.UsoCFDI.ToString())
                                                    {
                                                        result = "CFDI33140 - El campo UsoCFDI, no contiene un valor del catálogo c_UsoCFDI.";
                                                        return result;
                                                    }
                                                    OperacionesCatalogos o14 = new OperacionesCatalogos();
                                                    CatalogosSAT.c_UsoCFDI uso_CFDI = o14.Consultar_USOCFDI(com.Receptor.UsoCFDI.ToString());
                                                    if (uso_CFDI != null)
                                                    {
                                                        if (!string.IsNullOrEmpty(com.Receptor.Rfc))
                                                        {
                                                            if (com.Receptor.Rfc.Count<char>() == 13)
                                                            {
                                                                if (uso_CFDI.Física == "No")
                                                                {
                                                                    result = "CFDI33141 - La clave del campo UsoCFDI debe corresponder con el tipo de persona (fisica o moral).";
                                                                    return result;
                                                                }
                                                            }
                                                            else if (uso_CFDI.Moral == "No")
                                                            {
                                                                result = "CFDI33141 - La clave del campo UsoCFDI debe corresponder con el tipo de persona (fisica o moral).";
                                                                return result;
                                                            }
                                                        }
                                                    }
                                                }
                                                int Estimulo = 0;int Estimulo2=0;
                                                foreach (ComprobanteConcepto con in com.Conceptos)
                                                {
                                                    ServicioLocal.Business.CatalogosGrandes.c_ClaveProdServ myTipoClaveProdServ;
                                                    Enum.TryParse<ServicioLocal.Business.CatalogosGrandes.c_ClaveProdServ>("Item" + con.ClaveProdServ, out myTipoClaveProdServ);
                                                    if (myTipoClaveProdServ.ToString() != "Item" + con.ClaveProdServ)
                                                    {
                                                        result = "CFDI33142 - El campo ClaveProdServ, no contiene un valor del catálogo c_ClaveProdServ.";
                                                        return result;
                                                    }
                                                    OperacionesCatalogos o15 = new OperacionesCatalogos();
                                                    CatalogosSAT.c_ClaveProdServ CPS = o15.Consultar_ClaveProdServ(Convert.ToInt64(con.ClaveProdServ));
                                                    if (CPS != null)
                                                    {
                                                        if (!string.IsNullOrEmpty(CPS.Complemento) && !CPS.Complemento.Contains("Opcional:"))
                                                        {
                                                            if (!xml.Contains("<" + CPS.Complemento + ":"))
                                                            {
                                                                result = "CFDI33143 - No existe el complemento requerido para el valor de ClaveProdServ.";
                                                                return result;
                                                            }
                                                        }
                                                        if (CPS.IncluirIVAtrasladado == "Sí")
                                                        {
                                                            bool vandera = false;
                                                            if (con.Impuestos != null)
                                                            {
                                                                ComprobanteConceptoImpuestosTraslado[] traslados2 = con.Impuestos.Traslados;
                                                                for (int j = 0; j < traslados2.Length; j++)
                                                                {
                                                                    ComprobanteConceptoImpuestosTraslado tras = traslados2[j];
                                                                    if (tras.Impuesto == "002")
                                                                    {
                                                                        vandera = true;
                                                                    }
                                                                }
                                                            }
                                                            if (!vandera)
                                                            {
                                                                result = "CFDI33144 - No está declarado el impuesto relacionado con el valor de ClaveProdServ.";
                                                                return result;
                                                            }
                                                        }
                                                    }
                                                    OperacionesCatalogos o16 = new OperacionesCatalogos();
                                                    CatalogosSAT.c_ClaveUnidad CL = o16.ConsultarClaveUnidad(con.ClaveUnidad);
                                                    if (CL == null)
                                                    {
                                                        result = "CFDI33145 - El campo ClaveUnidad no contiene un valor del catálogo c_ClaveUnidad.";
                                                        return result;
                                                    }
                                                    if ((com.TipoDeComprobante == "I" || com.TipoDeComprobante == "E" || com.TipoDeComprobante == "N") && con.ValorUnitario <= 0m)
                                                    {
                                                        result = "CFDI33147 - El valor valor del campo ValorUnitario debe ser mayor que cero (0) cuando el tipo de comprobante es Ingreso, Egreso o Nomina.";
                                                        return result;
                                                    }
                                                    decimal limiteInferior = this.CalculoInferiorConcepto(con.Cantidad, con.ValorUnitario, (int)Convert.ToInt16(mone.Decimales));
                                                    decimal limiteSupeior = this.CalculoSuperiorValorConcepto(con.Cantidad, con.ValorUnitario, (int)Convert.ToInt16(mone.Decimales));
                                                    if (con.Importe < limiteInferior)
                                                    {
                                                        result = "CFDI33149 - El valor del campo Importe no se encuentra entre el limite inferior y superior permitido.";
                                                        return result;
                                                    }
                                                    if (con.Importe > limiteSupeior)
                                                    {
                                                        result = "CFDI33149 - El valor del campo Importe no se encuentra entre el limite inferior y superior permitido.";
                                                        return result;
                                                    }
                                                    if (con.DescuentoSpecified)
                                                    {/*
                                                        string desc2 = con.Descuento.ToString();
                                                        if (desc2 != null)
                                                        {
                                                            if (desc2 != "0")
                                                            {
                                                                string[] split = desc2.Split(".".ToCharArray());
                                                                if (split.Count<string>() <= 1)
                                                                {
                                                                    result = "CFDI33150 - El valor del campo Descuento debe tener hasta la cantidad de decimales que tenga registrado el atributo importe del concepto.";
                                                                    return result;
                                                                }
                                                                if (split[1].Count<char>() > (int)Convert.ToInt16(mone.Decimales))
                                                                {
                                                                    result = "CFDI33150 - El valor del campo Descuento debe tener hasta la cantidad de decimales que tenga registrado el atributo importe del concepto.";
                                                                    return result;
                                                                }
                                                            }
                                                        }*/
                                                        if (con.Descuento > con.Importe)
                                                        {
                                                            result = "CFDI33151 - El valor del campo Descuento es mayor que el campo Importe.";
                                                            return result;
                                                        }
                                                    }
                                                    if (con.Impuestos != null)
                                                    {
                                                        if (con.Impuestos.Retenciones == null && con.Impuestos.Traslados == null)
                                                        {
                                                            result = "CFDI33152 - En caso de utilizar el nodo Impuestos en un concepto, se deben incluir impuestos  de traslado y/o retenciones.";
                                                            return result;
                                                        }
                                                    }
                                                    if (con.Impuestos != null && con.Impuestos.Traslados != null)
                                                    {
                                                        ComprobanteConceptoImpuestosTraslado[] traslados2 = con.Impuestos.Traslados;
                                                        for (int j = 0; j < traslados2.Length; j++)
                                                        {
                                                            ComprobanteConceptoImpuestosTraslado tras = traslados2[j];
                                                            if (tras.Base <= 0m)
                                                            {
                                                                result = "CFDI33154 - El valor del campo Base que corresponde a Traslado debe ser mayor que cero.";
                                                                return result;
                                                            }
                                                            c_Impuesto myTipoImpuesto;
                                                            Enum.TryParse<c_Impuesto>("Item" + tras.Impuesto.ToString(), out myTipoImpuesto);
                                                            if (myTipoImpuesto.ToString() != "Item" + tras.Impuesto.ToString())
                                                            {
                                                                result = "CFDI33155 - El valor del campo Impuesto que corresponde a Traslado no contiene un valor del catálogo c_Impuesto.";
                                                                return result;
                                                            }
                                                            c_TipoFactor myTipoFactor;
                                                            Enum.TryParse<c_TipoFactor>(tras.TipoFactor.ToString(), out myTipoFactor);
                                                            if (myTipoFactor.ToString() != tras.TipoFactor.ToString())
                                                            {
                                                                result = "CFDI33156 - El valor del campo TipoFactor que corresponde a Traslado no contiene un valor del catálogo c_TipoFactor.";
                                                                return result;
                                                            }
                                                            if (tras.TipoFactor.ToString() == "Exento")
                                                            {
                                                                if (tras.TasaOCuotaSpecified || tras.ImporteSpecified)
                                                                {
                                                                    result = "CFDI33157 - Si el valor registrado en el campo TipoFactor que corresponde a Traslado es Exento no se deben registrar los campos TasaOCuota ni Importe.";
                                                                    return result;
                                                                }
                                                            }
                                                            else if (!tras.TasaOCuotaSpecified || !tras.ImporteSpecified)
                                                            {
                                                                result = "CFDI33158 - Si el valor registrado en el campo TipoFactor que corresponde a Traslado es Tasa o Cuota, se deben registrar los campos TasaOCuota e Importe.";
                                                                return result;
                                                            }
                                                            if (tras.TasaOCuotaSpecified)
                                                            {
                                                                bool rango = false;
                                                                OperacionesCatalogos o17 = new OperacionesCatalogos();
                                                                List<CatalogosSAT.c_TasaOCuota> tasas = o17.Consultar_TasaCuota(tras.Impuesto, tras.TipoFactor, "Traslados", ref rango);
                                                                if (rango)
                                                                {
                                                                    if (Convert.ToDecimal(tasas[0].Minimo) > Convert.ToDecimal(tras.TasaOCuota))
                                                                    {
                                                                        result = "CFDI33159 - El valor del campo TasaOCuota que corresponde a Traslado no contiene un valor del catálogo c_TasaOcuota o se encuentra fuera de rango";
                                                                        return result;
                                                                    }
                                                                    if (Convert.ToDecimal(tasas[0].Maximo) < Convert.ToDecimal(tras.TasaOCuota))
                                                                    {
                                                                        result = "CFDI33159 - El valor del campo TasaOCuota que corresponde a Traslado no contiene un valor del catálogo c_TasaOcuota o se encuentra fuera de rango";
                                                                        return result;
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    bool exis = false;
                                                                    foreach (CatalogosSAT.c_TasaOCuota tas in tasas)
                                                                    {
                                                                        if (Convert.ToDecimal(tas.Maximo) == Convert.ToDecimal(tras.TasaOCuota))
                                                                        {
                                                                            exis = true;
                                                                            break;
                                                                        }
                                                                    }
                                                                    if (!exis)
                                                                    {
                                                                        result = "CFDI33159 - El valor del campo TasaOCuota que corresponde a Traslado no contiene un valor del catálogo c_TasaOcuota o se encuentra fuera de rango";
                                                                        return result;
                                                                    }
                                                                }
                                                                if (Convert.ToDecimal(tras.TasaOCuota) == 0.08m && tras.Impuesto == "002")
                                                                
                                                                {
                                                                    if (!(con.ClaveProdServ == "01010101") || !(con.Cantidad == 1m) || !(con.ClaveUnidad == "ACT") || !(com.TipoDeComprobante == "I") || !(com.Receptor.UsoCFDI == "P01"))
                                                                    {
                                                                        OperacionesCatalogos o10 = new OperacionesCatalogos();
                                                                        CatalogosSAT.c_ClaveProdServ cla = o10.Consultar_ClaveProdServ(Convert.ToInt64(con.ClaveProdServ));
                                                                        if (cla == null)
                                                                        {
                                                                            result = "CFDI33196 - No aplica Estímulo Franja Fronteriza para la clave de producto o servicio";
                                                                            return result;
                                                                        }
                                                                        if (cla.EstimuloFranjaFronteriza != 1)
                                                                        {
                                                                            result = "CFDI33196 - No aplica Estímulo Franja Fronteriza para la clave de producto o servicio";
                                                                            return result;
                                                                        }
                                                                        if (Estimulo == 0)
                                                                        {
                                                                            Operaciones_IRFC i = new Operaciones_IRFC();
                                                                            vLCO lco = i.RFCValidezObligaciones(com.Emisor.Rfc);
                                                                            if (lco == null)
                                                                            {
                                                                                result = "CFDI33196 - El RFC no se encuentra registrado para aplicar el Estímulo Franja Fronteriza";
                                                                                return result;
                                                                            }

                                                                            OperacionesCatalogos o18 = new OperacionesCatalogos();
                                                                            c_CP cpx = o18.Consultar_CP(com.LugarExpedicion);
                                                                            if (cpx == null)
                                                                            {
                                                                                result = "CFDI33196 - El código postal no corresponde a Franja Fronteriza";
                                                                                return result;
                                                                            }
                                                                            
                                                                            if (lco.ValidezObligaciones=="2" && cpx.EstímuloFranjaFronteriza != 1)
                                                                            {
                                                                                result = "CFDI33196 - El código postal no corresponde a Franja Fronteriza";
                                                                                return result;
                                                                            }
                                                                                if (lco.ValidezObligaciones=="3" && cpx.EstímuloFranjaFronteriza != 2)
                                                                            {
                                                                                result = "CFDI33196 - El código postal no corresponde a Franja Fronteriza";
                                                                                return result;
                                                                            }
                                                                           if (lco.ValidezObligaciones=="4" && (cpx.EstímuloFranjaFronteriza != 1 &&cpx.EstímuloFranjaFronteriza != 2))
                                                                            {
                                                                                result = "CFDI33196 - El código postal no corresponde a Franja Fronteriza";
                                                                                return result;
                                                                            }
                                                                       
                                                                            Estimulo = 1;
                                                                        }
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    if(Estimulo2==0)
                                                                  if (Convert.ToDecimal(tras.TasaOCuota) != 0.08m && tras.Impuesto == "002")
                                                                  {
                                                                            OperacionesCatalogos o18 = new OperacionesCatalogos();
                                                                            c_CP cpx = o18.Consultar_CP(com.LugarExpedicion);
                                                                            if (cpx == null)
                                                                            {
                                                                                result = "CFDI33196 - El código postal no corresponde a Franja Fronteriza";
                                                                                return result;
                                                                            }
                                                                      Estimulo2 = 1;
                                                                  }
                                                                }
                                                            }
                                                            decimal limiteInferiorT = this.CalculoInferiorTraslado(tras.Base, Convert.ToDecimal(tras.TasaOCuota), (int)Convert.ToInt16(mone.Decimales));
                                                            decimal limiteSupeiorT = this.CalculoSuperiorValorTraslado(tras.Base, Convert.ToDecimal(tras.TasaOCuota), (int)Convert.ToInt16(mone.Decimales));
                                                            if (tras.Importe < limiteInferiorT)
                                                            {
                                                                result = "CFDI33161 - El valor del campo Importe o que corresponde a Traslado no se encuentra entre el limite inferior y superior permitido.";
                                                                return result;
                                                            }
                                                            if (tras.Importe > limiteSupeiorT)
                                                            {
                                                                result = "CFDI33161 - El valor del campo Importe o que corresponde a Traslado no se encuentra entre el limite inferior y superior permitido.";
                                                                return result;
                                                            }
                                                        }
                                                    }
                                                    if (con.Impuestos != null && con.Impuestos.Retenciones != null)
                                                    {
                                                        ComprobanteConceptoImpuestosRetencion[] retenciones2 = con.Impuestos.Retenciones;
                                                        for (int j = 0; j < retenciones2.Length; j++)
                                                        {
                                                            ComprobanteConceptoImpuestosRetencion reten = retenciones2[j];
                                                            if (reten.Base <= 0m)
                                                            {
                                                                result = "CFDI33163 - El valor del campo Base que corresponde a Retención debe ser mayor que cero.";
                                                                return result;
                                                            }
                                                            c_Impuesto myTipoImpuesto2;
                                                            Enum.TryParse<c_Impuesto>("Item" + reten.Impuesto.ToString(), out myTipoImpuesto2);
                                                            if (myTipoImpuesto2.ToString() != "Item" + reten.Impuesto.ToString())
                                                            {
                                                                result = "CFDI33164 - El valor del campo Impuesto que corresponde a Retención no contiene un valor del catálogo c_Impuesto.";
                                                                return result;
                                                            }
                                                            c_TipoFactor myTipoFactor2;
                                                            Enum.TryParse<c_TipoFactor>(reten.TipoFactor.ToString(), out myTipoFactor2);
                                                            if (myTipoFactor2.ToString() != reten.TipoFactor.ToString())
                                                            {
                                                                result = "CFDI33165 - El valor del campo TipoFactor que corresponde a Retención no contiene un valor del catálogo c_TipoFactor.";
                                                                return result;
                                                            }
                                                            if (reten.TipoFactor == "Exento")
                                                            {
                                                                result = "CFDI33166 - Si el valor registrado en el campo TipoFactor que corresponde a Retención debe ser distinto de Exento.";
                                                                return result;
                                                            }
                                                            OperacionesCatalogos o17 = new OperacionesCatalogos();
                                                            List<CatalogosSAT.c_TasaOCuota> uso_CFDI2 = o17.Consultar_TasaCuotaRetencion(reten.Impuesto, reten.TipoFactor);
                                                            if (uso_CFDI2 == null)
                                                            {
                                                                result = "CFDI33167 - El valor del campo TasaOCuota que corresponde a Retención no contiene un valor del catálogo c_TasaOcuota o se encuentra fuera de rango.";
                                                                return result;
                                                            }
                                                            bool verdadero = false;
                                                            foreach (CatalogosSAT.c_TasaOCuota cfdi in uso_CFDI2)
                                                            {
                                                                if (cfdi.RangoOFijo == "Rango")
                                                                {
                                                                    if (Convert.ToDecimal(reten.TasaOCuota) >= Convert.ToDecimal(cfdi.Minimo) && Convert.ToDecimal(reten.TasaOCuota) <= Convert.ToDecimal(cfdi.Maximo))
                                                                    {
                                                                        verdadero = true;
                                                                        break;
                                                                    }
                                                                }
                                                                else if (Convert.ToDecimal(reten.TasaOCuota) == Convert.ToDecimal(cfdi.Maximo))
                                                                {
                                                                    verdadero = true;
                                                                    break;
                                                                }
                                                            }
                                                            if (!verdadero)
                                                            {
                                                                result = "CFDI33167 - El valor del campo TasaOCuota que corresponde a Retención no contiene un valor del catálogo c_TasaOcuota o se encuentra fuera de rango.";
                                                                return result;
                                                            }
                                                            decimal limiteInferiorR = this.CalculoInferiorTraslado(reten.Base, reten.TasaOCuota, (int)Convert.ToInt16(mone.Decimales));
                                                            decimal limiteSupeiorR = this.CalculoSuperiorValorTraslado(reten.Base, reten.TasaOCuota, (int)Convert.ToInt16(mone.Decimales));
                                                            if (reten.Importe < limiteInferiorR)
                                                            {
                                                                result = "CFDI33169 - El valor del campo Importe que corresponde a Retención no se encuentra entre el limite inferior y superior permitido.";
                                                                return result;
                                                            }
                                                            if (reten.Importe > limiteSupeiorR)
                                                            {
                                                                result = "CFDI33169 - El valor del campo Importe que corresponde a Retención no se encuentra entre el limite inferior y superior permitido.";
                                                                return result;
                                                            }
                                                        }
                                                    }
                                                    if (con.InformacionAduanera != null)
                                                    {
                                                        ComprobanteConceptoInformacionAduanera[] informacionAduanera = con.InformacionAduanera;
                                                        for (int j = 0; j < informacionAduanera.Length; j++)
                                                        {
                                                            ComprobanteConceptoInformacionAduanera infAdua = informacionAduanera[j];
                                                            if (ComerExt && !string.IsNullOrEmpty(infAdua.NumeroPedimento))
                                                            {
                                                                result = "CFDI33171 - El NumeroPedimento no debe existir si se incluye el complemento de comercio exterior";
                                                                return result;
                                                            }
                                                            string sa = this.validarNumeroPedimento(infAdua.NumeroPedimento, "CFDI33170");
                                                            if (sa != "OK")
                                                            {
                                                                result = sa;
                                                                return result;
                                                            }
                                                        }
                                                    }
                                                    if (con.Parte != null)
                                                    {
                                                        ComprobanteConceptoParte[] parte2 = con.Parte;
                                                        for (int j = 0; j < parte2.Length; j++)
                                                        {
                                                            ComprobanteConceptoParte parte = parte2[j];
                                                            ServicioLocal.catCFDI.c_ClaveProdServ myTipoClaveProdServ2;
                                                            Enum.TryParse<ServicioLocal.catCFDI.c_ClaveProdServ>(parte.ClaveProdServ, out myTipoClaveProdServ2);
                                                            if (myTipoClaveProdServ2.ToString() != parte.ClaveProdServ)
                                                            {
                                                                result = "CFDI33172 - El campo ClaveProdServ, no contiene un valor del catálogo c_ClaveProdServ.";
                                                                return result;
                                                            }
                                                            if (parte.ValorUnitario <= 0m)
                                                            {
                                                                result = "CFDI33174 - El valor del campo ValorUnitario debe ser mayor que cero (0).";
                                                                return result;
                                                            }
                                                            if (parte.ImporteSpecified)
                                                            {
                                                                decimal limiteInferior2 = this.CalculoInferiorConcepto(parte.Cantidad, parte.ValorUnitario, (int)Convert.ToInt16(mone.Decimales));
                                                                decimal limiteSupeior2 = this.CalculoSuperiorValorConcepto(parte.Cantidad, parte.ValorUnitario, (int)Convert.ToInt16(mone.Decimales));
                                                                if (parte.Importe < limiteInferior2)
                                                                {
                                                                    result = "CFDI33176 - El valor del campo Importe no se encuentra entre el limite inferior y superior permitido.";
                                                                    return result;
                                                                }
                                                                if (parte.Importe > limiteSupeior2)
                                                                {
                                                                    result = "CFDI33176 - El valor del campo Importe no se encuentra entre el limite inferior y superior permitido.";
                                                                    return result;
                                                                }
                                                            }
                                                        }
                                                    }
                                                    if (con.Parte != null)
                                                    {
                                                        ComprobanteConceptoParte[] parte2 = con.Parte;
                                                        for (int j = 0; j < parte2.Length; j++)
                                                        {
                                                            ComprobanteConceptoParte parte = parte2[j];
                                                            foreach (ComprobanteConceptoParteInformacionAduanera infAdua2 in parte.InformacionAduanera)
                                                            {
                                                                if (ComerExt && !string.IsNullOrEmpty(infAdua2.NumeroPedimento))
                                                                {
                                                                    result = "CFDI33178 - El NumeroPedimento no debe existir si se incluye el complemento de comercio exterior";
                                                                    return result;
                                                                }
                                                                string sa = this.validarNumeroPedimento(infAdua2.NumeroPedimento, "CFDI33177");
                                                                if (sa != "OK")
                                                                {
                                                                    result = sa;
                                                                    return result;
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                                if (com.Impuestos != null)
                                                {
                                                    if (com.Impuestos.TotalImpuestosRetenidosSpecified)
                                                    {
                                                        string impRe = com.Impuestos.TotalImpuestosRetenidos.ToString();
                                                        if (impRe != null)
                                                        {
                                                            string[] split = impRe.Split(".".ToCharArray());
                                                            if (split.Count<string>() <= 1)
                                                            {
                                                                result = "CFDI33180 - El valor del campo TotalImpuestosRetenidos debe tener hasta la cantidad de decimales que soporte la moneda.";
                                                                return result;
                                                            }
                                                            if (split[1].Count<char>() > (int)Convert.ToInt16(mone.Decimales))
                                                            {
                                                                result = "CFDI33180 - El valor del campo TotalImpuestosRetenidos debe tener hasta la cantidad de decimales que soporte la moneda.";
                                                                return result;
                                                            }
                                                        }
                                                        decimal totalRet = 0m;
                                                        List<string> list = new List<string>();
                                                        if (com.Impuestos != null && com.Impuestos.Retenciones != null)
                                                        {
                                                            ComprobanteImpuestosRetencion[] retenciones3 = com.Impuestos.Retenciones;
                                                            ComprobanteImpuestosRetencion ret;
                                                            for (int j = 0; j < retenciones3.Length; j++)
                                                            {
                                                                ret = retenciones3[j];
                                                                totalRet += ret.Importe;
                                                                c_Impuesto myTipoImpuesto;
                                                                Enum.TryParse<c_Impuesto>("Item" + ret.Impuesto.ToString(), out myTipoImpuesto);
                                                                if (myTipoImpuesto.ToString() != "Item" + ret.Impuesto.ToString())
                                                                {
                                                                    result = "CFDI33185 - El campo Impuesto no contiene un valor del catálogo c_Impuesto.";
                                                                    return result;
                                                                }
                                                                if (list.Any((string z) => z == ret.Impuesto))
                                                                {
                                                                    result = "CFDI33186 - Debe haber sólo un registro por cada tipo de impuesto retenido.";
                                                                    return result;
                                                                }
                                                                list.Add(ret.Impuesto);
                                                                decimal importeTotal = 0m;
                                                                foreach (ComprobanteConcepto con in com.Conceptos)
                                                                {
                                                                    if (con.Impuestos != null)
                                                                    {
                                                                        if (con.Impuestos.Retenciones != null)
                                                                        {
                                                                            ComprobanteConceptoImpuestosRetencion[] retenciones2 = con.Impuestos.Retenciones;
                                                                            for (int k = 0; k < retenciones2.Length; k++)
                                                                            {
                                                                                ComprobanteConceptoImpuestosRetencion re = retenciones2[k];
                                                                                if (re.Impuesto == ret.Impuesto)
                                                                                {
                                                                                    importeTotal += re.Importe;
                                                                                }
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                                if (importeTotal != ret.Importe)
                                                                {
                                                                    result = "CFDI33189 - El campo Importe correspondiente a Retención no es igual a la suma de los importes de los impuestos retenidos registrados en los conceptos donde el impuesto sea igual al campo impuesto de este elemento.";
                                                                    return result;
                                                                }
                                                            }
                                                        }
                                                        decimal TotalRe = decimal.Round(totalRet, mone.Decimales.Value, MidpointRounding.AwayFromZero);
                                                        if (TotalRe != com.Impuestos.TotalImpuestosRetenidos)
                                                        {
                                                            result = "CFDI33181 - El valor del campo TotalImpuestosRetenidos debe ser igual a la suma de los importes registrados en el elemento hijo Retencion.";
                                                            return result;
                                                        }
                                                    }
                                                    if (com.Impuestos.TotalImpuestosTrasladadosSpecified)
                                                    {
                                                        string impTr = com.Impuestos.TotalImpuestosTrasladados.ToString();
                                                        if (impTr != null)
                                                        {
                                                            string[] split = impTr.Split(".".ToCharArray());
                                                            if (split.Count<string>() <= 1)
                                                            {
                                                                result = "CFDI33182 - El valor del campo TotalImpuestosTrasladados debe tener hasta la cantidad de decimales que soporte la moneda.";
                                                                return result;
                                                            }
                                                            if (split[1].Count<char>() > (int)Convert.ToInt16(mone.Decimales))
                                                            {
                                                                result = "CFDI33182 - El valor del campo TotalImpuestosTrasladados debe tener hasta la cantidad de decimales que soporte la moneda.";
                                                                return result;
                                                            }
                                                        }
                                                        decimal totalTras = 0m;
                                                        ComprobanteImpuestosTraslado[] traslados3 = com.Impuestos.Traslados;
                                                        for (int j = 0; j < traslados3.Length; j++)
                                                        {
                                                            ComprobanteImpuestosTraslado tras2 = traslados3[j];
                                                            totalTras += tras2.Importe;
                                                        }
                                                        decimal Totaltras = decimal.Round(totalTras, mone.Decimales.Value, MidpointRounding.AwayFromZero);
                                                        if (Totaltras != com.Impuestos.TotalImpuestosTrasladados)
                                                        {
                                                            result = "CFDI33183 - El valor del campo TotalImpuestosTrasladados no es igual a la suma de los importes registrados en el elemento hijo Traslado.";
                                                            return result;
                                                        }
                                                    }
                                                    if (com.Impuestos != null && com.Impuestos.Retenciones != null)
                                                    {
                                                        if (com.Impuestos.Retenciones.Count<ComprobanteImpuestosRetencion>() > 0)
                                                        {
                                                            if (!com.Impuestos.TotalImpuestosRetenidosSpecified)
                                                            {
                                                                result = "CFDI33184 - Debe existir el campo TotalImpuestosRetenidos.";
                                                                return result;
                                                            }
                                                            decimal arg_28F4_0 = com.Impuestos.Retenciones[0].Importe;
                                                            bool flag = 1 == 0;
                                                            if (!com.Impuestos.TotalImpuestosRetenidosSpecified)
                                                            {
                                                                result = "CFDI33187 - Debe existir el campo TotalImpuestosRetenidos.";
                                                                return result;
                                                            }
                                                        }
                                                    }
                                                    if (com.Impuestos != null && com.Impuestos.Traslados != null)
                                                    {
                                                        if (!com.Impuestos.TotalImpuestosTrasladadosSpecified)
                                                        {
                                                            result = "CFDI33190 - Debe existir el campo TotalImpuestosTrasladados.";
                                                            return result;
                                                        }
                                                        List<DatosTraslados> DT = new List<DatosTraslados>();
                                                        ComprobanteImpuestosTraslado[] traslados3 = com.Impuestos.Traslados;
                                                        for (int j = 0; j < traslados3.Length; j++)
                                                        {
                                                            ComprobanteImpuestosTraslado tras2 = traslados3[j];
                                                            foreach (DatosTraslados dt in DT)
                                                            {
                                                                if (dt.impuesto == tras2.Impuesto && dt.tasa == tras2.TasaOCuota && dt.factor == tras2.TipoFactor)
                                                                {
                                                                    result = "CFDI33192 - Debe haber sólo un registro con la misma combinación de impuesto, factor y tasa por cada traslado.";
                                                                    return result;
                                                                }
                                                            }
                                                            if (!string.IsNullOrEmpty(tras2.TipoFactor) && !string.IsNullOrEmpty(tras2.Impuesto))
                                                            {
                                                                DT.Add(new DatosTraslados
                                                                {
                                                                    factor = tras2.TipoFactor,
                                                                    impuesto = tras2.Impuesto,
                                                                    tasa = tras2.TasaOCuota
                                                                });
                                                            }
                                                        }
                                                        traslados3 = com.Impuestos.Traslados;
                                                        for (int j = 0; j < traslados3.Length; j++)
                                                        {
                                                            ComprobanteImpuestosTraslado tras2 = traslados3[j];
                                                            c_Impuesto myTipoImpuesto;
                                                            Enum.TryParse<c_Impuesto>("Item" + tras2.Impuesto.ToString(), out myTipoImpuesto);
                                                            if (myTipoImpuesto.ToString() != "Item" + tras2.Impuesto.ToString())
                                                            {
                                                                result = "CFDI33191 - El campo Impuesto no contiene un valor del catálogo c_Impuesto.";
                                                                return result;
                                                            }
                                                            OperacionesCatalogos o17 = new OperacionesCatalogos();
                                                            List<CatalogosSAT.c_TasaOCuota> uso_CFDI2 = o17.Consultar_TasaCuotaTraslado(tras2.Impuesto, tras2.TipoFactor);
                                                            if (uso_CFDI2 == null)
                                                            {
                                                                result = "CFDI33193 - El valor seleccionado debe corresponder a un valor del catalogo donde la columna impuesto corresponda con el campo impuesto y la columna factor corresponda con el campo TipoFactor.";
                                                                return result;
                                                            }
                                                            bool verdadero = false;
                                                            foreach (CatalogosSAT.c_TasaOCuota cfdi in uso_CFDI2)
                                                            {
                                                                if (cfdi.RangoOFijo == "Rango")
                                                                {
                                                                    if (Convert.ToDecimal(tras2.TasaOCuota) >= Convert.ToDecimal(cfdi.Minimo) && Convert.ToDecimal(tras2.TasaOCuota) <= Convert.ToDecimal(cfdi.Maximo))
                                                                    {
                                                                        verdadero = true;
                                                                        break;
                                                                    }
                                                                }
                                                                else if (Convert.ToDecimal(tras2.TasaOCuota) == Convert.ToDecimal(cfdi.Maximo))
                                                                {
                                                                    verdadero = true;
                                                                    break;
                                                                }
                                                            }
                                                            if (!verdadero)
                                                            {
                                                                result = "CFDI33193 - El valor seleccionado debe corresponder a un valor del catalogo donde la columna impuesto corresponda con el campo impuesto y la columna factor corresponda con el campo TipoFactor.";
                                                                return result;
                                                            }
                                                            decimal importeTotal = 0m;
                                                            foreach (ComprobanteConcepto con in com.Conceptos)
                                                            {
                                                                if (con.Impuestos != null)
                                                                {
                                                                    if (con.Impuestos.Traslados != null)
                                                                    {
                                                                        ComprobanteConceptoImpuestosTraslado[] traslados2 = con.Impuestos.Traslados;
                                                                        for (int k = 0; k < traslados2.Length; k++)
                                                                        {
                                                                            ComprobanteConceptoImpuestosTraslado tr = traslados2[k];
                                                                            if (tr.ImporteSpecified && tr.TasaOCuotaSpecified && tr.Impuesto == tras2.Impuesto && tr.TipoFactor == tras2.TipoFactor && tr.TasaOCuota == tras2.TasaOCuota)
                                                                            {
                                                                                importeTotal += tr.Importe;
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                            if (importeTotal != tras2.Importe)
                                                            {
                                                                result = "CFDI33195 - El campo Importe correspondiente a Traslado no es igual a la suma de los importes de los impuestos trasladados registrados en los conceptos donde el impuesto del concepto sea igual al campo impuesto de este elemento y la TasaOCuota del concepto sea igual al campo TasaOCuota de este elemento.";
                                                                return result;
                                                            }
                                                        }
                                                    }
                                                }
                                                result = "0";
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return result;

        }
        private decimal CalculoInferiorConcepto(decimal Cantidad, decimal valorUnitario, int moneda)
        {
            int NumDecimalesCantidad = 0;
            int NumDecimalesValorUnitario = 0;
            string NumDecimalesCantidadString = Cantidad.ToString();
            if (NumDecimalesCantidadString != null)
            {
                string[] split = NumDecimalesCantidadString.Split(".".ToCharArray());
                if (split.Count<string>() > 1)
                {
                    NumDecimalesCantidad = split[1].Count<char>();
                }
            }
            string NumDecimalesValorUnitarioString = valorUnitario.ToString();
            if (NumDecimalesValorUnitarioString != null)
            {
                string[] split2 = NumDecimalesValorUnitarioString.Split(".".ToCharArray());
                if (split2.Count<string>() > 1)
                {
                    NumDecimalesValorUnitario = split2[1].Count<char>();
                }
            }
            decimal x = (decimal)Math.Pow(10.0, (double)(-(double)NumDecimalesCantidad));
            decimal x2 = (decimal)Math.Pow(10.0, (double)(-(double)NumDecimalesValorUnitario));
            decimal x3 = (decimal)Math.Pow(10.0, -12.0);
            decimal resultado = (Cantidad - x / 2m) * (valorUnitario - x2 / 2m);
            return this.Truncate(resultado, moneda);
        }

        private decimal CalculoSuperiorValorConcepto(decimal Cantidad, decimal valorUnitario, int moneda)
        {
            int NumDecimalesCantidad = 0;
            int NumDecimalesValorUnitario = 0;
            string NumDecimalesCantidadString = Cantidad.ToString();
            if (NumDecimalesCantidadString != null)
            {
                string[] split = NumDecimalesCantidadString.Split(".".ToCharArray());
                if (split.Count<string>() > 1)
                {
                    NumDecimalesCantidad = split[1].Count<char>();
                }
            }
            string NumDecimalesValorUnitarioString = valorUnitario.ToString();
            if (NumDecimalesValorUnitarioString != null)
            {
                string[] split2 = NumDecimalesValorUnitarioString.Split(".".ToCharArray());
                if (split2.Count<string>() > 1)
                {
                    NumDecimalesValorUnitario = split2[1].Count<char>();
                }
            }
            decimal x = (decimal)Math.Pow(10.0, (double)(-(double)NumDecimalesCantidad));
            decimal x2 = (decimal)Math.Pow(10.0, (double)(-(double)NumDecimalesValorUnitario));
            decimal x3 = (decimal)Math.Pow(10.0, -12.0);
            decimal resultado = (Cantidad + (x / 2m - x3)) * (valorUnitario + (x2 / 2m - x3));
            resultado = this.Truncate(resultado, moneda);
            string resultadoString = resultado.ToString();
            if (resultadoString != null)
            {
                string[] split3 = resultadoString.Split(".".ToCharArray());
                if (split3.Count<string>() > 1)
                {
                    int decimales = Convert.ToInt32(split3[1]);
                    decimales++;
                    if (decimales == 1 || decimales == 10 || decimales == 100 || decimales == 1000 || decimales == 10000 || decimales == 100000)
                    {
                        string dec = decimales.ToString();
                        dec = dec.Replace("1", "");
                        resultadoString = Convert.ToInt64(split3[0]) + 1L + "." + dec;
                    }
                    else
                    {
                        resultadoString = split3[0] + "." + decimales;
                    }
                    resultado = Convert.ToDecimal(resultadoString);
                }
                else
                {
                    resultadoString = this.AgregaCeros(moneda);
                    resultadoString = split3[0] + "." + resultadoString;
                    resultado = Convert.ToDecimal(resultadoString);
                }
            }
            return resultado;
        }

        private decimal CalculoInferiorTraslado(decimal Base, decimal TasaOCuota, int moneda)
        {
            int NumDecimalesBase = 0;
            string NumDecimalesBaseString = Base.ToString();
            if (NumDecimalesBaseString != null)
            {
                string[] split = NumDecimalesBaseString.Split(".".ToCharArray());
                if (split.Count<string>() > 1)
                {
                    NumDecimalesBase = split[1].Count<char>();
                }
            }
            decimal x = (decimal)Math.Pow(10.0, (double)(-(double)NumDecimalesBase));
            decimal resultado = (Base - x / 2m) * TasaOCuota;
            return this.Truncate(resultado, moneda);
        }

        private decimal CalculoSuperiorValorTraslado(decimal Base, decimal TasaOCuota, int moneda)
        {
            int NumDecimalesBase = 0;
            string NumDecimalesBaseString = Base.ToString();
            if (NumDecimalesBaseString != null)
            {
                string[] split = NumDecimalesBaseString.Split(".".ToCharArray());
                if (split.Count<string>() > 1)
                {
                    NumDecimalesBase = split[1].Count<char>();
                }
            }
            decimal x = (decimal)Math.Pow(10.0, (double)(-(double)NumDecimalesBase));
            decimal x2 = (decimal)Math.Pow(10.0, -12.0);
            decimal resultado = (Base + x / 2m - x2) * TasaOCuota;
            resultado = this.Truncate(resultado, moneda);
            string resultadoString = resultado.ToString();
            if (resultadoString != null)
            {
                string[] split2 = resultadoString.Split(".".ToCharArray());
                if (split2.Count<string>() > 1)
                {
                    int decimales = Convert.ToInt32(split2[1]);
                    decimales++;
                    if (decimales == 1 || decimales == 10 || decimales == 100 || decimales == 1000 || decimales == 10000 || decimales == 100000)
                    {
                        string dec = decimales.ToString();
                        dec = dec.Replace("1", "");
                        resultadoString = Convert.ToInt64(split2[0]) + 1L + "." + dec;
                    }
                    else
                    {
                        resultadoString = split2[0] + "." + decimales;
                    }
                    resultado = Convert.ToDecimal(resultadoString);
                }
                else
                {
                    resultadoString = this.AgregaCeros(moneda);
                    resultadoString = split2[0] + "." + resultadoString;
                    resultado = Convert.ToDecimal(resultadoString);
                }
            }
            return resultado;
        }

        private string AgregaCeros(int num)
        {
            string c = "";
            for (int i = 0; i < num; i++)
            {
                if (i == num - 1)
                {
                    c += "1";
                }
                else
                {
                    c += "0";
                }
            }
            return c;
        }

        public decimal Truncate(decimal number, int digits)
        {
            decimal stepper = (decimal)Math.Pow(10.0, (double)digits);
            long temp = (long)(stepper * number);
            return temp / stepper;
        }

        public string ValidaSelloDigital(string selloDigital, string cert, string cadena)
        {
            string result;
            try
            {
                byte[] sello_byte = Convert.FromBase64String(selloDigital);
                byte[] cert_byte = Convert.FromBase64String(cert);
                X509Certificate2 certificado = new X509Certificate2(cert_byte);
                RSACryptoServiceProvider rsa = (RSACryptoServiceProvider)certificado.PublicKey.Key;
                SHA256Managed sha = new SHA256Managed();
                byte[] hash = sha.ComputeHash(Encoding.UTF8.GetBytes(cadena));
                string sha1Hash = BitConverter.ToString(hash);
                sha1Hash = sha1Hash.Replace("-", "");
                int res = rsa.VerifyHash(hash, CryptoConfig.MapNameToOID("SHA256"), sello_byte) ? 0 : 302;
                if (res == 0)
                {
                    result = "OK";
                }
                else
                {
                    result = (rsa.VerifyData(Encoding.UTF8.GetBytes(cadena), new SHA256Managed(), sello_byte) ? "OK" : "302");
                }
            }
            catch (Exception ex)
            {
                result = "Error al desencriptar el sello digital " + ex.ToString();
            }
            return result;
        }

        private string validarCertificado(string cert, string NoCertificado, DateTime FechaEmision, string RFCEmisor, string version, string NomEmisor, string CURP)
        {
            X509Certificate2 certificado;
            string result;
            try
            {
                byte[] cert_byte = Convert.FromBase64String(cert);
                certificado = new X509Certificate2(cert_byte);
            }
            catch (Exception ex_13)
            {
                result = "CFDI33105 - EL certificado no cumple con alguno de los valores permitidos.";
                return result;
            }
            string NoCertificado2 = ValidarCFDI33.FromHexString(certificado.SerialNumber);
            if (NoCertificado2 != NoCertificado)
            {
                result = "CFDI33105 - EL certificado no cumple con alguno de los valores permitidos.(NoCertificado)";
            }
            else if (this.ValidaCertificadoAc(certificado) != 0)
            {
                result = "CFDI33105 - EL certificado no cumple con alguno de los valores permitidos.(CertificadoAc)";
            }
            else
            {
                DateTime datFechaExpiracionCSD = certificado.NotAfter;
                DateTime datFechaEfectivaCSD = certificado.NotBefore;
                if (this.ValidaFechaEmisionXml(FechaEmision, datFechaExpiracionCSD, datFechaEfectivaCSD) != 0)
                {
                    result = "CFDI33105 - EL certificado no cumple con alguno de los valores permitidos(FechaEmision).";
                }
                else if (this.ValidaRFCEmisor(RFCEmisor, certificado.SubjectName.Name) != 0)
                {
                    result = "CFDI33105 - EL certificado no cumple con alguno de los valores permitidos.(RFC)";
                }
                else if (this.VerificaCSDRevocado(NoCertificado, FechaEmision, version) != 0)
                {
                    result = "CFDI33105 - EL certificado no cumple con alguno de los valores permitidos.(CSDRevocado)";
                }
                else
                {
                    if (!string.IsNullOrEmpty(CURP))
                    {
                        string NS = certificado.SubjectName.Name.Replace("/ ", "");
                        NS = NS.Replace("\"", "");
                        string Curp = NS.Substring(NS.LastIndexOf("SERIALNUMBER=") + 13, 19).Trim();
                    }
                    result = "OK";
                }
            }
            return result;
        }

        public int ValidaCertificadoAc(X509Certificate2 certificado)
        {
            int result;
            try
            {
                
                string directorio = ConfigurationManager.AppSettings["CertACSat"];
                IEnumerable<string> archivosCert = Directory.EnumerateFiles(directorio, "*.cer");
                if ((from s in archivosCert
                     select File.ReadAllBytes(s)).Any((byte[] archivo) => this.VerificaEmisorCertificado(certificado.RawData, archivo)))
                {
                    result = 0;
                }
                else
                {
                    result = 308;
                }
                 
            }
            catch (Exception err)
            {
                NtLinkBusiness.Logger.Error(err);
                result = 308;
            }
            return result;
        }

        private bool VerificaEmisorCertificado(byte[] certificado, byte[] certificadoAC)
        {
            Org.BouncyCastle.X509.X509Certificate cer = new X509CertificateParser().ReadCertificate(certificado);
            Org.BouncyCastle.X509.X509Certificate cer2 = new X509CertificateParser().ReadCertificate(certificadoAC);
            bool result;
            try
            {
                cer.Verify(cer2.GetPublicKey());
                result = true;
            }
            catch (Exception ee_2B)
            {
                result = false;
            }
            return result;
        }

        public int ValidaFechaEmisionXml(DateTime dtFechaEmision, DateTime dtFechaExpiracion, DateTime dtFechaEfectiva)
        {
            int result;
            if (dtFechaExpiracion < dtFechaEmision || dtFechaEfectiva > dtFechaEmision)
            {
                result = 305;
            }
            else
            {
                result = 0;
            }
            return result;
        }

        public int ValidaRFCEmisor(string rfc, string name)
        {
            int result;
            try
            {
                name = name.Replace("\"", "");
                string strLRfc = name.Substring(name.LastIndexOf("2.5.4.45=") + 9, 13).Trim();
                result = ((strLRfc != rfc) ? 303 : 0);
            }
            catch (Exception ee)
            {
                NtLinkBusiness.Logger.Error("", ee);
                result = 303;
            }
            return result;
        }

        public int VerificaCSDRevocado(string serieCert, DateTime fecha, string version)
        {
            Operaciones_IRFC lcoLogic = new Operaciones_IRFC();
            LcoLogic lcoLogic2 = new LcoLogic();
            int result;
            if (version == "3.3")
            {
                vLCO lco = lcoLogic.SearchLCOByNoCertificado(serieCert);
                try
                {
                    if (lco == null)
                    {
                        result = 304;
                        return result;
                    }
                    if (lco.ValidezObligaciones.Equals("0", StringComparison.CurrentCultureIgnoreCase) || fecha < lco.FechaInicio || fecha > lco.FechaFinal || !lco.EstatusCertificado.Contains("A"))
                    {
                        result = 304;
                        return result;
                    }
                    result = 0;
                    return result;
                }
                catch (Exception ee)
                {
                    NtLinkBusiness.Logger.Error("", ee);
                    result = 304;
                    return result;
                }
            }
            Csd lco2 = lcoLogic2.SearchCsdBySerie(serieCert, fecha);
            try
            {
                if (lco2 == null)
                {
                    result = 304;
                }
                else
                {
                    result = 0;
                }
            }
            catch (Exception ee)
            {
                NtLinkBusiness.Logger.Error("", ee);
                result = 304;
            }
            return result;
        }

        public static string FromHexString(string hexString)
        {
            string Resultado = "";
            while (hexString.Length > 0)
            {
                string Data = Convert.ToChar(Convert.ToUInt32(hexString.Substring(0, 2), 16)).ToString();
                Resultado += Data;
                hexString = hexString.Substring(2, hexString.Length - 2);
            }
            return Resultado;
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
                CatalogosSAT.c_NumPedimentoAduana x = ox1x.Consultar_Aduanas((int)Convert.ToInt16(Aduanas));
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

        private decimal CalculoInferiorPorcentajeMoneda(double? tipodeCambio, int variacion)
        {
            decimal resultado = 0;
            decimal v = 0;
            if (variacion != null)
            {
                v = 1 - ((decimal)variacion / 100);

                resultado = (decimal)tipodeCambio * v;
            }
            if (resultado < 0)
                resultado = 0;
            return resultado;
        }
        //-----------------------------------------
        //--------------------------------------
        private decimal CalculoSuperiorPorcentajeMoneda(double? tipodeCambio, int variacion)
        {
            decimal resultado = 0;
            decimal v = 0;
            if (variacion != null)
            {
                v = 1 + ((decimal)variacion / 100);
                resultado = (decimal)tipodeCambio * (decimal)v;
            }
            return resultado;
        }
        //-----------------------------------------
    }
}