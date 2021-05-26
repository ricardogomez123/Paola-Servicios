

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



namespace ServicioLocal.Business
{
	public class ValidarECC : NtLinkBusiness
	{
		public ValidarECC()
		{
			XmlConfigurator.Configure();
		}

		public string ProcesarECC(EstadoDeCuentaCombustible ecc, string tipoDeComprobante, string version)
		{
			string result;
			try
			{
				if (ecc.Conceptos != null)
				{
					decimal total = 0m;
					EstadoDeCuentaCombustibleConceptoEstadoDeCuentaCombustible[] conceptos = ecc.Conceptos;
					for (int i = 0; i < conceptos.Length; i++)
					{
						EstadoDeCuentaCombustibleConceptoEstadoDeCuentaCombustible en = conceptos[i];
						total += en.Importe;
					}
					if (ecc.SubTotal != total)
					{
						result = "121 - El valor de este atributo debe ser igual a la suma de los valores de los atributos ConceptoEstadoDeCuentaCombustible:Importe.";
						return result;
					}
					decimal totalTraslado = 0m;
					conceptos = ecc.Conceptos;
					for (int i = 0; i < conceptos.Length; i++)
					{
						EstadoDeCuentaCombustibleConceptoEstadoDeCuentaCombustible en = conceptos[i];
						EstadoDeCuentaCombustibleConceptoEstadoDeCuentaCombustibleTraslado[] traslados = en.Traslados;
						for (int j = 0; j < traslados.Length; j++)
						{
							EstadoDeCuentaCombustibleConceptoEstadoDeCuentaCombustibleTraslado tras = traslados[j];
							totalTraslado += tras.Importe;
						}
					}
					totalTraslado += ecc.SubTotal;
					if (ecc.Total != totalTraslado)
					{
						result = "122 - El valor de este atributo debe ser igual a la suma del valor del atributo SubTotal y la suma de los valores de los atributos ConceptoEstadoDeCuentaCombustible:Traslados:Traslado:Importe.";
						return result;
					}
					conceptos = ecc.Conceptos;
					for (int i = 0; i < conceptos.Length; i++)
					{
						EstadoDeCuentaCombustibleConceptoEstadoDeCuentaCombustible en = conceptos[i];
						if (this.ValidaRFCLCO(en.Rfc) == 402)
						{
							result = "123 - Para el atributo Conceptos:ConceptoEstadoDeCuentaCombustible:Rfc ,  Se debe validar la existencia del RFC en la Lista de Contribuyentes Obligados (LCO).";
							return result;
						}
					}
				}
				if (tipoDeComprobante != "I")
				{
					result = "124 - El valor registrado debe ser la clave I que corresponde a Ingreso.";
				}
				else if (version != "3.3")
				{
					result = "125 - El atributo Version debe tener el valor 3.3.";
				}
				else
				{
					result = "0";
				}
			}
			catch (Exception ex)
			{
				result = "999 - Error no clasificado";
			}
			return result;
		}

		public int ValidaRFCLCO(string rfc)
		{
			int result;
			try
			{
				Operaciones_IRFC lcoLogic = new Operaciones_IRFC();
				vLCO lco = lcoLogic.SearchLCOByRFC(rfc);
				result = ((lco == null) ? 402 : 0);
			}
			catch (Exception ee)
			{
				NtLinkBusiness.Logger.Error("", ee);
				result = 402;
			}
			return result;
		}
	}
}
