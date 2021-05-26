using CertificadorWs.Business.Retenciones;
using log4net;
using ServicioLocal.Business;
using ServicioLocal.Business.TimbreRetenciones;
using ServicioLocalContract;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace CertificadorWs.Business
{
	public class TimbradoUtils
	{
		private static readonly ILog Logger = LogManager.GetLogger(typeof(TimbradoUtils));

		private static Comprobante DesSerializar(XElement element)
		{
			XmlSerializer ser = new XmlSerializer(typeof(Comprobante));
			string xml = element.ToString();
			StringReader reader = new StringReader(xml);
			return (Comprobante)ser.Deserialize(reader);
		}

		public static Retenciones.Retenciones DesSerializarRetenciones(string xml)
		{
            XmlSerializer ser = new XmlSerializer(typeof(Retenciones.Retenciones));
			StringReader reader = new StringReader(xml);
            return (Retenciones.Retenciones)ser.Deserialize(reader);
		}

        public static Retenciones.Retenciones DesSerializarRetenciones(XElement element)
		{
            XmlSerializer ser = new XmlSerializer(typeof(Retenciones.Retenciones));
			string xml = element.ToString();
			StringReader reader = new StringReader(xml);
            return (Retenciones.Retenciones)ser.Deserialize(reader);
		}

		public static string TimbraRetencionString(string comprobante, empresa emp, bool consumeSaldo,bool vaidarUsuario)
		{
			ValidadorDatosRetencion val = new ValidadorDatosRetencion();
			string result = null;
			ServicioLocal.Business.TimbreRetenciones.TimbreFiscalDigital timbre = null;
			string acuseSat = "";
			string hash = null;
			XElement element = XElement.Parse(comprobante);
            Retenciones.Retenciones comp = TimbradoUtils.DesSerializarRetenciones(element);
            if(vaidarUsuario)
			TimbradoUtils.ValidarUsuario(comp.Emisor.RFCEmisor);
			Dictionary<int, string> dict = val.ProcesarCadenaRetencion(comprobante, ref result, ref timbre, ref acuseSat, ref hash);
			string result2;
			if (timbre != null && timbre.selloSAT != null && dict.Count == 0)
			{
				SerializadorTimbres sert = new SerializadorTimbres();
				if (ConfigurationManager.AppSettings["Pruebas"] == "true")
				{
					timbre.selloSAT = "Inválido, Ambiente de pruebas";
				}
				string res = sert.GetTimbreRenecionesXml(timbre);
				string cfdiTimbrado = result;
				string fecha = comp.FechaExp;
				string rfcReceptor = string.Empty;
				if (comp.Receptor.Nacionalidad == RetencionesReceptorNacionalidad.Nacional)
				{
					RetencionesReceptorNacional rec = (RetencionesReceptorNacional)comp.Receptor.Item;
					rfcReceptor = rec.RFCRecep;
				}
				else
				{
					rfcReceptor = "XEXX010101000";
				}
				if (!TimbradoUtils.GuardaFactura(fecha, comp.Emisor.RFCEmisor, rfcReceptor, timbre.UUID, cfdiTimbrado, hash, emp, consumeSaldo, true))
				{
					throw new Exception("Error al abrir el comprobante");
				}
				result2 = res;
			}
			else
			{
				if (timbre != null && timbre.selloSAT == null && dict.Count == 0)
				{
					XElement el = XElement.Parse(result);
					XElement complemento = el.Elements(Constantes.RetencionNamesPace + "Complemento").FirstOrDefault<XElement>();
					if (complemento != null)
					{
						XElement t = complemento.Elements(Constantes.CFDTimbreFiscalVersionNamespace + "TimbreFiscalDigital").FirstOrDefault<XElement>();
						if (t != null)
						{
							SidetecStringWriter sw = new SidetecStringWriter(Encoding.UTF8);
							t.Save(sw, SaveOptions.DisableFormatting);
							result2 = sw.ToString();
							return result2;
						}
					}
				}
				if (dict.Count > 0)
				{
					StringBuilder res2 = new StringBuilder();
					foreach (KeyValuePair<int, string> d in dict)
					{
						res2.AppendLine(d.Key.ToString() + " - " + d.Value.ToString());
					}
					result2 = res2.ToString();
				}
				else
				{
					TimbradoUtils.Logger.Error("Error al abrir el comprobante:" + comprobante);
					result2 = "Error al abrir el comprobante";
				}
			}
			return result2;
		}

		public static string TimbraCfdiString(string comprobante, empresa emp)
		{
			ValidadorCFDi32 val = new ValidadorCFDi32();
			string result = null;
			ServicioLocal.Business.TimbreFiscalDigital timbre = null;
			string acuseSat = "";
			string hash = null;
			XElement element = XElement.Parse(comprobante);
			Comprobante comp = TimbradoUtils.DesSerializar(element);
			Dictionary<int, string> dict = val.ProcesarCadena(comp.Emisor.Rfc, comprobante, ref result, ref timbre, ref acuseSat, ref hash);
			string result2;
			if (timbre != null && timbre.SelloSAT != null && dict.Count == 0)
			{
				if (!string.IsNullOrEmpty(comp.Confirmacion))
				{
					using (NtLinkLocalServiceEntities db = new NtLinkLocalServiceEntities())
					{
						ConfirmacionTimbreWs33 C = db.ConfirmacionTimbreWs33.FirstOrDefault((ConfirmacionTimbreWs33 p) => p.Folio == comp.Folio && p.RfcEmisor == comp.Emisor.Rfc && p.RfcReceptor == comp.Receptor.Rfc);
						C.procesado = new bool?(true);
						db.ConfirmacionTimbreWs33.ApplyCurrentValues(C);
						db.SaveChanges();
					}
				}
				SerializadorTimbres sert = new SerializadorTimbres();
				if (ConfigurationManager.AppSettings["Pruebas"] == "true")
				{
					timbre.SelloSAT = "Inválido, Ambiente de pruebas";
				}
				string res = sert.GetTimbreXml(timbre);
				string cfdiTimbrado = result;
				if (ConfigurationManager.AppSettings["EnvioSat"] == "false")
				{
					if (!TimbradoUtils.GuardaFactura(comp.Fecha, comp.Emisor.Rfc, comp.Receptor.Rfc, timbre.UUID, cfdiTimbrado, hash, emp, true, false))
					{
						throw new Exception("Error al abrir el comprobante");
					}
				}
				result2 = res;
			}
			else
			{
				if (timbre != null && timbre.SelloSAT == null && dict.Count == 0)
				{
					XElement el = XElement.Parse(result);
					XElement complemento = el.Elements(Constantes.CFDVersionNamespace + "Complemento").FirstOrDefault<XElement>();
					if (complemento != null)
					{
						XElement t = complemento.Elements(Constantes.CFDTimbreFiscalVersionNamespace + "TimbreFiscalDigital").FirstOrDefault<XElement>();
						if (t != null)
						{
							SidetecStringWriter sw = new SidetecStringWriter(Encoding.UTF8);
							t.Save(sw, SaveOptions.DisableFormatting);
							result2 = sw.ToString();
							return result2;
						}
					}
				}
				if (dict.Count > 0)
				{
					StringBuilder res2 = new StringBuilder();
					foreach (KeyValuePair<int, string> d in dict)
					{
						res2.AppendLine(d.Key.ToString() + " - " + d.Value.ToString());
					}
					result2 = res2.ToString();
				}
				else
				{
					TimbradoUtils.Logger.Error("Error al abrir el comprobante:" + comprobante);
					result2 = "Error al abrir el comprobante";
				}
			}
			return result2;
		}

		public static empresa ValidarAplicacion(string hash, string rfc)
		{
			empresa result;
			if (hash == null)
			{
				result = null;
			}
			else if (hash != ConfigurationManager.AppSettings["HashFacturacion"])
			{
				result = null;
			}
			else
			{
				NtLinkEmpresa em = new NtLinkEmpresa();
				empresa empresa = em.GetByRfc(rfc);
				if (empresa == null)
				{
					TimbradoUtils.Logger.Info(rfc + " No encontrado");
					result = null;
				}
				else
				{
					if (empresa.Bloqueado)
					{
						TimbradoUtils.Logger.Info(empresa.RFC + "-> Bloqueado");
						throw new FaultException("El RFC del emisor se encuentra bloqueado, favor de ponerse en contacto con atención al cliente");
					}
					if (empresa.Baja)
					{
						TimbradoUtils.Logger.Info(empresa.RFC + "-> Baja");
						throw new FaultException("El RFC del emisor se encuentra dado de baja, favor de ponerse en contacto con atención a clientes");
					}
					NtLinkSistema nls = new NtLinkSistema();
					Sistemas sistema = nls.GetSistema((int)empresa.idSistema.Value);
					if (sistema.Bloqueado)
					{
						TimbradoUtils.Logger.Info(sistema.Rfc + "-> Bloqueado");
						throw new FaultException("El RFC del emisor se encuentra bloqueado, favor de ponerse en contacto con atención a clientes");
					}
					if ((sistema.TipoSistema != 3 || sistema.TipoSistema != 0) && sistema.SaldoTimbrado <= 0)
					{
						TimbradoUtils.Logger.Info("Saldo: " + sistema.SaldoTimbrado);
						throw new FaultException("No cuentas con saldo suficiente para timbrar documentos, contacta a tu ejecutivo de ventas");
					}
					result = empresa;
				}
			}
			return result;
		}

		public static empresa ValidarUsuario(string rfc)
		{
			NtLinkEmpresa em = new NtLinkEmpresa();
			empresa empresa = em.GetByRfc(rfc);
			empresa result;
			if (empresa == null)
			{
				TimbradoUtils.Logger.Info(rfc + " No encontrado");
				result = null;
			}
			else
			{
				if (empresa.Bloqueado)
				{
					TimbradoUtils.Logger.Info(empresa.RFC + "-> Bloqueado");
					throw new FaultException("El RFC del emisor se encuentra bloqueado, favor de ponerse en contacto con atención al cliente");
				}
				if (empresa.Baja)
				{
					TimbradoUtils.Logger.Info(empresa.RFC + "-> Baja");
					throw new FaultException("El RFC del emisor se encuentra dado de baja, favor de ponerse en contacto con atención a clientes");
				}
				NtLinkSistema nls = new NtLinkSistema();
				Sistemas sistema = nls.GetSistema((int)empresa.idSistema.Value);
				if (sistema.Bloqueado)
				{
					TimbradoUtils.Logger.Info(sistema.Rfc + "-> Bloqueado");
					throw new FaultException("El RFC del emisor se encuentra dado de baja, favor de ponerse en contacto con atención a clientes");
				}
				if ((sistema.TipoSistema == 1 || sistema.TipoSistema == 0) && sistema.SaldoTimbrado <= 0)
				{
					TimbradoUtils.Logger.Info("Saldo: " + sistema.SaldoTimbrado);
					throw new FaultException("No cuentas con saldo suficiente para timbrar documentos, contacta a tu ejecutivo de ventas");
				}
				result = empresa;
			}
			return result;
		}

		public static empresa ValidarUsuarioSinSaldo(string rfc)
		{
			NtLinkEmpresa em = new NtLinkEmpresa();
			empresa empresa = em.GetByRfc(rfc);
			empresa result;
			if (empresa == null)
			{
				TimbradoUtils.Logger.Info(rfc + " No encontrado");
				result = null;
			}
			else
			{
				if (empresa.Bloqueado)
				{
					TimbradoUtils.Logger.Info(empresa.RFC + "-> Bloqueado");
					throw new FaultException("El RFC del emisor se encuentra bloqueado, favor de ponerse en contacto con atención al cliente");
				}
				if (empresa.Baja)
				{
					TimbradoUtils.Logger.Info(empresa.RFC + "-> Baja");
					throw new FaultException("El RFC del emisor se encuentra dado de baja, favor de ponerse en contacto con atención a clientes");
				}
				NtLinkSistema nls = new NtLinkSistema();
				Sistemas sistema = nls.GetSistema((int)empresa.idSistema.Value);
				if (sistema.Bloqueado)
				{
					TimbradoUtils.Logger.Info(sistema.Rfc + "-> Bloqueado");
					throw new FaultException("El RFC del emisor se encuentra dado de baja, favor de ponerse en contacto con atención a clientes");
				}
				result = empresa;
			}
			return result;
		}

		public static bool EmpresaMultipleRFC(string RFC)
		{
			NtLinkEmpresa em = new NtLinkEmpresa();
			return em.GetByRfcMultiple(RFC);
		}

		public static empresa ValidarUsuarioMultiple(empresa empresa)
		{
			empresa result;
			if (empresa == null)
			{
				TimbradoUtils.Logger.Info(empresa.RFC + " No encontrado");
				result = null;
			}
			else
			{
				if (empresa.Bloqueado)
				{
					TimbradoUtils.Logger.Info(empresa.RFC + "-> Bloqueado");
					throw new FaultException("El RFC del emisor se encuentra bloqueado, favor de ponerse en contacto con atención al cliente");
				}
				if (empresa.Baja)
				{
					TimbradoUtils.Logger.Info(empresa.RFC + "-> Baja");
					throw new FaultException("El RFC del emisor se encuentra dado de baja, favor de ponerse en contacto con atención a clientes");
				}
				NtLinkSistema nls = new NtLinkSistema();
				Sistemas sistema = nls.GetSistema((int)empresa.idSistema.Value);
				if (sistema.Bloqueado)
				{
					TimbradoUtils.Logger.Info(sistema.Rfc + "-> Bloqueado");
					throw new FaultException("El RFC del emisor se encuentra dado de baja, favor de ponerse en contacto con atención a clientes");
				}
				if ((sistema.TipoSistema == 1 || sistema.TipoSistema == 0) && sistema.SaldoTimbrado <= 0)
				{
					TimbradoUtils.Logger.Info("Saldo: " + sistema.SaldoTimbrado);
					throw new FaultException("No cuentas con saldo suficiente para timbrar documentos, contacta a tu ejecutivo de ventas");
				}
				result = empresa;
			}
			return result;
		}

		public static bool GuardaFactura(string fecha, string rfcEmisor, string rfcReceptor, string uudi, string xml, string hash, empresa emp, bool consumeSaldo, bool retenciones = false)
		{
			NtLinkTimbrado tim = new NtLinkTimbrado();
			TimbreWs33 tws = new TimbreWs33
			{
				FechaFactura = Convert.ToDateTime(fecha),
				RfcEmisor = rfcEmisor,
				RfcReceptor = rfcReceptor,
				Xml = string.Empty,
				Status = new int?(0),
				Uuid = uudi,
				Hash = hash.Replace("-", ""),
				Retenciones = new bool?(retenciones)
			};
			bool result = tim.GuardarTimbre(tws);
			if (consumeSaldo)
			{
				if (tim.IncrementaSaldo(emp.IdEmpresa, (int)emp.idSistema.Value))
				{
					TimbradoUtils.Logger.Info("Saldo consumido para IdEmpresa" + emp.IdEmpresa);
				}
				else
				{
					TimbradoUtils.Logger.Error("No se pudo consumir saldo para IdEmpresa" + emp.IdEmpresa);
				}
			}
			if (result)
			{
				string directorio = Path.Combine(ConfigurationManager.AppSettings["RutaTimbrado"], tws.RfcEmisor, tws.FechaFactura.ToString("yyyyMMdd"));
				if (!Directory.Exists(directorio))
				{
					Directory.CreateDirectory(directorio);
				}
				string fileName = Path.Combine(directorio, "Comprobante_" + uudi + ".xml");
				File.WriteAllText(fileName, xml, Encoding.UTF8);
				TimbradoUtils.Logger.Info(hash);
			}
			else
			{
				TimbradoUtils.Logger.Fatal("No se pudo guardar el comprobante con hash: " + hash);
			}
			return result;
		}
	}
}
