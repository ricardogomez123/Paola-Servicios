

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
using ServicioLocal.Business.Complemento;
using CatalogosSAT;


namespace ServicioLocal.Business
{
    public class ValidarPago : NtLinkBusiness
    {
   

        public ValidarPago()
        {
            XmlConfigurator.Configure();
        }
        //---------------------------------------------------------------------------------------------
        public string ProcesarPago(Comprobante com, ServicioLocal.Business.Complemento.Pagos Pag, Pagoo.Comprobante c)
{
	string result;
	if (com.TipoDeComprobante != "P")
	{
		result = "CRP101 - El valor del campo TipoDeComprobante debe ser \"P\"";
	}
	else if (c.SubTotal != "0")
	{
		result = "CRP102 - El valor del campo SubTotal debe ser cero \"0\".";
	}
	else if (com.Moneda != "XXX")
	{
		result = "CRP103 - El valor del campo Moneda debe ser \"XXX\".";
	}
	else if (com.FormaPagoSpecified)
	{
		result = "CRP104 - El campo FormaPago no se debe registrar en el CFDI.";
	}
	else if (com.MetodoPagoSpecified)
	{
		result = "CRP105 - El campo MetodoPago no se debe registrar en el CFDI.";
	}
	else if (!string.IsNullOrEmpty(com.CondicionesDePago))
	{
		result = "CRP106 - El campo CondicionesDePago no se debe registrar en el CFDI.";
	}
	else if (com.DescuentoSpecified)
	{
		result = "CRP107 - El campo Descuento no se debe registrar en el CFDI.";
	}
	else if (com.TipoCambioSpecified)
	{
		result = "CRP108 - El campo TipoCambio no se debe registrar en el CFDI.";
	}
	else if (c.Total != "0")
	{
		result = "CRP109 - El valor del campo Total debe ser cero \"0\".";
	}
	else if (com.Receptor != null && com.Receptor.UsoCFDI != "P01")
	{
		result = "CRP110 - El valor del campo UsoCFDI debe ser \"P01\".";
	}
	else
	{
		if (com.Conceptos != null)
		{
			if (com.Conceptos.Count<ComprobanteConcepto>() != 1)
			{
				result = "CRP111 - Solo debe existir un Concepto en el CFDI. ";
				return result;
			}
			foreach (ComprobanteConcepto conp in com.Conceptos)
			{
				if (conp.Impuestos != null)
				{
					result = "CRP112 - No se deben registrar apartados dentro de Conceptos";
					return result;
				}
				if (conp.ComplementoConcepto != null)
				{
					result = "CRP112 - No se deben registrar apartados dentro de Conceptos";
					return result;
				}
				if (conp.CuentaPredial != null)
				{
					result = "CRP112 - No se deben registrar apartados dentro de Conceptos";
					return result;
				}
				if (conp.InformacionAduanera != null)
				{
					result = "CRP112 - No se deben registrar apartados dentro de Conceptos";
					return result;
				}
				if (conp.Parte != null)
				{
					result = "CRP112 - No se deben registrar apartados dentro de Conceptos";
					return result;
				}
				if (conp.ClaveProdServ != "84111506")
				{
					result = "CRP113 - El valor del campo ClaveProdServ debe ser \"84111506\".";
					return result;
				}
				if (!string.IsNullOrEmpty(conp.NoIdentificacion))
				{
					result = "CRP114 - El campo NoIdentificacion no se debe registrar en el CFDI.";
					return result;
				}
				if (conp.Cantidad != 1m)
				{
					result = "CRP115 - El valor del campo Cantidad debe ser \"1\".";
					return result;
				}
				if (conp.ClaveUnidad != "ACT")
				{
					result = "CRP116 - El valor del campo ClaveUnidad debe ser \"ACT\".";
					return result;
				}
				if (!string.IsNullOrEmpty(conp.Unidad))
				{
					result = "CRP117 - El campo Unidad no se debe registrar en el CFDI.";
					return result;
				}
				if (conp.Descripcion != "Pago")
				{
					result = "CRP118 - El valor del campo Descripcion debe ser \"Pago\".";
					return result;
				}
				if (conp.ValorUnitario != 0m)
				{
					result = "CRP119 - El valor del campo ValorUnitario debe ser cero \"0\".";
					return result;
				}
				if (conp.Importe != 0m)
				{
					result = "CRP120 - El valor del campo Importe debe ser cero \"0\".";
					return result;
				}
				if (conp.DescuentoSpecified)
				{
					result = "CRP121 - El campo Descuento no se debe registrar en el CFDI.";
					return result;
				}
			}
			foreach (Pagoo.ComprobanteConcepto conp2 in c.Conceptos)
			{
				if (conp2.ValorUnitario != "0")
				{
					result = "CRP119 - El valor del campo ValorUnitario debe ser cero \"0\".";
					return result;
				}
				if (conp2.Importe != "0")
				{
					result = "CRP120 - El valor del campo Importe debe ser cero \"0\".";
					return result;
				}
				if (conp2.Cantidad != "1")
				{
					result = "CRP115 - El valor del campo Cantidad debe ser \"1\".";
					return result;
				}
			}
		}
		if (com.Impuestos != null)
		{
			result = "CRP122 - No se debe registrar el apartado de Impuestos en el CFDI.";
		}
		else
		{
			foreach (PagosPago pagos in Pag.Pago)
			{
				if (pagos.FormaDePagoP == "99")
				{
					result = "CRP201 - El valor del campo FormaDePagoP debe ser distinto de \"99\".";
					return result;
				}
				if (pagos.MonedaP == "XXX")
				{
					result = "CRP202 - El campo MonedaP debe ser distinto de \"XXX\"";
					return result;
				}
				if (pagos.MonedaP != "MXN")
				{
					if (!pagos.TipoCambioPSpecified)
					{
						result = "CRP203 - El campo TipoCambioP se debe registrar.";
						return result;
					}
				}
				else if (pagos.TipoCambioPSpecified)
				{
					result = "CRP204 - El campo TipoCambioP no se debe registrar.";
					return result;
				}
				OperacionesCatalogos o9 = new OperacionesCatalogos();
				CatalogosSAT.c_Moneda mone = o9.Consultar_Moneda(pagos.MonedaP);
				if (mone != null)
				{
					string varia = mone.Variacion;
					OperacionesCatalogos o10 = new OperacionesCatalogos();
					Divisas divisa = o10.Consultar_TipoDivisa(pagos.MonedaP);
					if (divisa != null)
					{
						decimal inferior = this.CalculoInferiorPorcentajeMoneda(divisa.PesosDivisa, (int)Convert.ToInt16(varia));
						decimal superior = this.CalculoSuperiorPorcentajeMoneda(divisa.PesosDivisa, (int)Convert.ToInt16(varia));
						if (pagos.TipoCambioP < inferior)
						{
							if (string.IsNullOrEmpty(com.Confirmacion))
							{
								result = "CRP205 - Cuando el valor del campo TipoCambio se encuentre fuera de los límites establecidos, debe existir el campo Confirmacion.";
								return result;
							}
						}
						if (pagos.TipoCambioP > superior)
						{
							if (string.IsNullOrEmpty(com.Confirmacion))
							{
								result = "CRP205 - Cuando el valor del campo TipoCambioP se encuentre fuera de los límites establecidos, debe existir el campo Confirmacion";
								return result;
							}
						}
					}
				}
				decimal tot = 0m;
				if (pagos.DoctoRelacionado == null)
				{
					result = "CRP000 - El numero de elementos DoctoRelacionado debe ser mayor a cero";
					return result;
				}
				if (pagos.DoctoRelacionado.Count<PagosPagoDoctoRelacionado>() == 0)
				{
					result = "CRP000 - El numero de elementos DoctoRelacionado debe ser mayor a cero";
					return result;
				}
				PagosPagoDoctoRelacionado[] doctoRelacionado = pagos.DoctoRelacionado;
				for (int i = 0; i < doctoRelacionado.Length; i++)
				{
					PagosPagoDoctoRelacionado doc = doctoRelacionado[i];
					if (doc.ImpPagadoSpecified)
					{
						tot += doc.ImpPagado;
					}
				}
				if (pagos.Monto <= 0m)
				{
					result = "CRP207 - El valor del campo Monto no es mayor que cero \"0\".";
					return result;
				}
				decimal MontoDocumentos = pagos.Monto;
				if (pagos.MonedaP != "MXN")
				{
					MontoDocumentos *= pagos.TipoCambioP;
					MontoDocumentos = decimal.Round(MontoDocumentos, mone.Decimales.Value, MidpointRounding.AwayFromZero);
				}
				if (tot > MontoDocumentos)
				{
					result = "CRP206 - La suma de los valores registrados en el campo ImpPagado de los apartados DoctoRelacionado no es menor o igual que el valor del campo Monto.";
					return result;
				}
				string mon = pagos.Monto.ToString();
				if (mon != null)
				{
					if (mon != "0")
					{
						string[] split = mon.Split(".".ToCharArray());
						if (split.Count<string>() <= 1)
						{
							result = "CRP208 - El valor del campo Monto debe tener hasta la cantidad de decimales que soporte la moneda registrada en el campo MonedaP.";
							return result;
						}
						if (split[1].Count<char>() != (int)Convert.ToInt16(mone.Decimales))
						{
							result = "CRP208 - El valor del campo Monto debe tener hasta la cantidad de decimales que soporte la moneda registrada en el campo MonedaP.";
							return result;
						}
					}
				}
				OperacionesCatalogos o11 = new OperacionesCatalogos();
			   CatalogosSAT.c_TipoDeComprobante TCP = o11.Consultar_TipoDeComprobante(com.TipoDeComprobante);
				if (TCP != null)
				{
					decimal monto = pagos.Monto;
					long? valor_máximo = TCP.Valor_máximo;
					if (monto > valor_máximo.GetValueOrDefault() && valor_máximo.HasValue)
					{
						if (string.IsNullOrEmpty(com.Confirmacion))
						{
							result = "CRP209 - Cuando el valor del campo Monto se encuentre fuera de los límites establecidos, debe existir el campo Confirmacion";
							return result;
						}
					}
				}

				if (!string.IsNullOrEmpty(pagos.RfcEmisorCtaOrd))
				{
					if (pagos.RfcEmisorCtaOrd != "XEXX010101000")
					{
						Operaciones_IRFC r = new Operaciones_IRFC();
						vI_RFC t = r.Consultar_IRFC(pagos.RfcEmisorCtaOrd);
						if (t == null)
						{
							result = "CRP210 - El RFC del campo RfcEmisorCtaOrd no se encuentra en la lista de RFC.";
							return result;
						}
					}
					else if (string.IsNullOrEmpty(pagos.NomBancoOrdExt))
					{
						result = "CRP211 - El campo NomBancoOrdExt se debe registrar.";
						return result;
					}
				}
				if (pagos.FormaDePagoP != "02" && pagos.FormaDePagoP != "03" && pagos.FormaDePagoP != "04" && pagos.FormaDePagoP != "05" && pagos.FormaDePagoP != "06" && pagos.FormaDePagoP != "28" && pagos.FormaDePagoP != "29" && !string.IsNullOrEmpty(pagos.CtaOrdenante))
				{
					result = "CRP212 - El campo CtaOrdenante no se debe registrar.";
					return result;
				}
				if (!string.IsNullOrEmpty(pagos.CtaOrdenante))
				{
					OperacionesCatalogos o10 = new OperacionesCatalogos();
					CatalogosSAT.c_FormaPago formaPago = o10.Consultar_FormaPago(pagos.FormaDePagoP);
					if (formaPago.PatroncuentaBeneficiaria != "No" && formaPago.PatroncuentaBeneficiaria != "Opcional")
					{
						if (!this.validarExpresion(formaPago.PatroncuentaBeneficiaria, pagos.CtaOrdenante))
						{
							result = "CRP213 - El campo CtaOrdenante no cumple con el patrón requerido.";
							return result;
						}
					}
				}
				if (pagos.FormaDePagoP != "02" && pagos.FormaDePagoP != "03" && pagos.FormaDePagoP != "04" && pagos.FormaDePagoP != "05" && pagos.FormaDePagoP != "28" && pagos.FormaDePagoP != "29")
				{
					if (!string.IsNullOrEmpty(pagos.RfcEmisorCtaBen))
					{
						result = "CRP214 - El campo RfcEmisorCtaBen no se debe registrar.";
						return result;
					}
					if (!string.IsNullOrEmpty(pagos.CtaBeneficiario))
					{
						result = "CRP215 - El campo CtaBeneficiario no se debe registrar.";
						return result;
					}
				}
				if (pagos.FormaDePagoP != "03" && pagos.TipoCadPagoSpecified)
				{
					result = "CRP216 - El campo TipoCadPago no se debe registrar. ";
					return result;
				}
				int tDoc = 0;
				if (pagos.DoctoRelacionado != null)
				{
					tDoc = pagos.DoctoRelacionado.Count<PagosPagoDoctoRelacionado>();
				}
				if (pagos.DoctoRelacionado != null)
				{
					doctoRelacionado = pagos.DoctoRelacionado;
					for (int i = 0; i < doctoRelacionado.Length; i++)
					{
						PagosPagoDoctoRelacionado doc = doctoRelacionado[i];
						if (doc.MonedaDR == "XXX")
						{
							result = "CRP217 - El valor del campo MonedaDR debe ser distinto de \"XXX\"";
							return result;
						}
						if (doc.MonedaDR != pagos.MonedaP)
						{
							if (!doc.TipoCambioDRSpecified)
							{
								result = "CRP218 - El campo TipoCambioDR se debe registrar.";
								return result;
							}
						}
						else if (doc.TipoCambioDRSpecified)
						{
							result = "CRP219 - El campo TipoCambioDR no se debe registrar.";
							return result;
						}
						if (doc.MonedaDR == "MXN" && pagos.MonedaP != "MXN" && doc.TipoCambioDR != 1m)
						{
							result = "CRP220 - El campo TipoCambioDR debe ser \"1\".";
							return result;
						}
						if (doc.MetodoDePagoDR == "PPD")
						{
							if (!doc.ImpSaldoAntSpecified)
							{
								result = "CRP234 - El campo ImpSaldoAnt se debe registrar.";
								return result;
							}
						}
						if (tDoc > 1 && !doc.ImpPagadoSpecified)
						{
							result = "CRP235 - El campo ImpPagado se debe registrar. ";
							return result;
						}
						if (tDoc == 1 && doc.TipoCambioDRSpecified && !doc.ImpPagadoSpecified)
						{
							result = "CRP235 - El campo ImpPagado se debe registrar. ";
							return result;
						}
						if (doc.ImpSaldoAntSpecified)
						{
							if (doc.ImpSaldoAnt <= 0m)
							{
								result = "CRP221 - El campo ImpSaldoAnt debe mayor a cero.";
								return result;
							}
							string impSal = doc.ImpSaldoAnt.ToString();
							if (impSal != null)
							{
								if (impSal != "0")
								{
									string[] split = impSal.Split(".".ToCharArray());
									if (split.Count<string>() <= 1)
									{
										result = "CRP222 - El valor del campo ImpSaldoAnt debe tener hasta la cantidad de decimales que soporte la moneda registrada en el campo MonedaDR.";
										return result;
									}
									if (split[1].Count<char>() != (int)Convert.ToInt16(mone.Decimales))
									{
										result = "CRP222 - El valor del campo ImpSaldoAnt debe tener hasta la cantidad de decimales que soporte la moneda registrada en el campo MonedaDR.";
										return result;
									}
								}
							}
						}
						if (doc.ImpPagadoSpecified)
						{
							if (doc.ImpPagado <= 0m)
							{
								result = "CRP223 - El campo ImpPagado debe mayor a cero.";
								return result;
							}
							string impPa = doc.ImpPagado.ToString();
							if (impPa != null)
							{
								if (impPa != "0")
								{
									string[] split = impPa.Split(".".ToCharArray());
									if (split.Count<string>() <= 1)
									{
										result = "CRP224 - El valor del campo ImpPagado debe tener hasta la cantidad de decimales que soporte la moneda registrada en el campo MonedaDR.";
										return result;
									}
									if (split[1].Count<char>() != (int)Convert.ToInt16(mone.Decimales))
									{
										result = "CRP224 - El valor del campo ImpPagado debe tener hasta la cantidad de decimales que soporte la moneda registrada en el campo MonedaDR.";
										return result;
									}
								}
							}
						}
						if (doc.ImpSaldoInsolutoSpecified)
						{
							string impSalI = doc.ImpSaldoInsoluto.ToString();
							if (impSalI != null)
							{
								if (impSalI != "0")
								{
									string[] split = impSalI.Split(".".ToCharArray());
									if (split.Count<string>() <= 1)
									{
										result = "CRP225 - El valor del campo ImpSaldoInsoluto debe tener hasta la cantidad de decimales que soporte la moneda registrada en el campo MonedaDR.";
										return result;
									}
									if (split[1].Count<char>() != (int)Convert.ToInt16(mone.Decimales))
									{
										result = "CRP225 - El valor del campo ImpSaldoInsoluto debe tener hasta la cantidad de decimales que soporte la moneda registrada en el campo MonedaDR.";
										return result;
									}
								}
							}
							if (doc.ImpSaldoInsoluto < 0m)
							{
								result = "CRP226 - El campo ImpSaldoInsoluto debe ser mayor o igual a cero y calcularse con la suma de los campos ImSaldoAnt menos el ImpPagado o el Monto.";
								return result;
							}
							if (doc.ImpSaldoInsoluto != doc.ImpSaldoAnt - doc.ImpPagado)
							{
								result = "CRP226 - El campo ImpSaldoInsoluto debe ser mayor o igual a cero y calcularse con la suma de los campos ImSaldoAnt menos el ImpPagado o el Monto.";
								return result;
							}
						}
					}
				}
				if (pagos.TipoCadPagoSpecified)
				{
					if (pagos.CertPago == null)
					{
						result = "CRP227 - El campo CertPago se debe registrar.";
						return result;
					}
					if (string.IsNullOrEmpty(pagos.CadPago))
					{
						result = "CRP229 - El campo CadPago se debe registrar.";
						return result;
					}
					if (pagos.SelloPago == null)
					{
						result = "CRP231 - El campo SelloPago se debe registrar. ";
						return result;
					}
				}
				else
				{
					if (pagos.CertPago != null)
					{
						result = "CRP228 - El campo CertPago no se debe registrar.";
						return result;
					}
					if (!string.IsNullOrEmpty(pagos.CadPago))
					{
						result = "CRP230 - El campo CadPago no se debe registrar.";
						return result;
					}
					if (pagos.SelloPago != null)
					{
						result = "CRP232 - El campo SelloPago no se debe registrar.";
						return result;
					}
				}
				if (pagos.DoctoRelacionado != null)
				{
					doctoRelacionado = pagos.DoctoRelacionado;
					for (int i = 0; i < doctoRelacionado.Length; i++)
					{
						PagosPagoDoctoRelacionado doc = doctoRelacionado[i];
						if (doc.MetodoDePagoDR == "PPD")
						{
							if (string.IsNullOrEmpty(doc.NumParcialidad))
							{
								result = "CRP233 - El campo NumParcialidad se debe registrar.";
								return result;
							}
							if (!doc.ImpSaldoInsolutoSpecified)
							{
								result = "CRP236 - El campo ImpSaldoInsoluto se debe registrar.";
								return result;
							}
						}
					}
				}
				if (pagos.Impuestos != null)
				{
					result = "CRP237 - No debe existir el apartado de Impuestos.";
					return result;
				}
				if (pagos.FormaDePagoP != "02" && pagos.FormaDePagoP != "03" && pagos.FormaDePagoP != "04" && pagos.FormaDePagoP != "05" && pagos.FormaDePagoP != "06" && pagos.FormaDePagoP != "28" && pagos.FormaDePagoP != "29" && !string.IsNullOrEmpty(pagos.RfcEmisorCtaOrd))
				{
					result = "CRP238 - El campo RfcEmisorCtaOrd no se debe registrar.";
					return result;
				}
				if (!string.IsNullOrEmpty(pagos.CtaBeneficiario))
				{
					OperacionesCatalogos o10 = new OperacionesCatalogos();
					CatalogosSAT.c_FormaPago formaPago = o10.Consultar_FormaPago(pagos.FormaDePagoP);
					if (formaPago.PatroncuentaBeneficiaria != "No" && formaPago.PatroncuentaBeneficiaria != "Opcional")
					{
						if (!this.validarExpresion(formaPago.PatroncuentaBeneficiaria, pagos.CtaBeneficiario))
						{
							result = "CRP239 - El campo CtaBeneficiario no cumple con el patrón requerido.";
							return result;
						}
					}
				}
			}
			result = "0";
		}
	}
	return result;
}//-------------------------------------------------------------------------------------
        //--------------------------------------
        private decimal CalculoInferiorPorcentajeMoneda(double? tipodeCambio, int variacion)
        {//
            decimal resultado = 0;
            decimal v = 0;
            if (variacion != null)
            {
                v =1- ( (decimal)variacion/100);

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
                v = 1 + ( (decimal)variacion/100);
                resultado = (decimal)tipodeCambio * (decimal)v;
            }
            return resultado;
        }
        //-----------------------------------------
        bool validarExpresion(string expresion, string cadena)
        {
            bool s = false;
            String[] expresiones = expresion.Split('|');
            foreach (var expre in expresiones)
            {
                if (Regex.Match(cadena, "^" + expre + "$").Success)
                {
                    s = true; break;
                }
            }
            return s;
        }


    }
}