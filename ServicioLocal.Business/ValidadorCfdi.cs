using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;
using ServicioLocal.Business;
using System.Xml;
using System.Configuration;
using log4net.Config;
using ServicioLocalContract;

namespace ServicioLocal.Business
{
    public class ValidadorCfdi : NtLinkBusiness
    {

        private ValidadorCertificado validadorCertificado;
        public ValidadorCfdi()
        {
             XmlConfigurator.Configure();
             validadorCertificado = new ValidadorCertificado();
        }

        private static XNamespace _versionNamespace = XNamespace.Get("http://www.sat.gob.mx/cfd/3");

        public ResultadoValidacion Validar(string xmlFile, string version)
        {
            if (version == null)
            {
                var lista = new List<Validacion>() { new Validacion { Valido = false, Error = "Versión incorrecta", Descripcion = "Versión del archivo" } };
                return new ResultadoValidacion() { Detalles = lista, Valido = false };
            }
            ValidadorEstructura valEstructura = new ValidadorEstructura(version);
            var reader = new StringReader(xmlFile);
            
            var input = valEstructura.Validate2(new XmlTextReader(reader), xmlFile);
            if (input.Valido)
            {
                return Validar(input);
            }
            else
            {
                var lista = input.Errores.Select(p => new Validacion {Valido = false,Error = p, Descripcion = "Estructura inválida"}).ToList();
                return new ResultadoValidacion() {Detalles =lista, Valido = false};
            }

            
                
        }


        private ResultadoValidacion Validar(ValidadorInput entrada)
        {
            var generadorCadenas = new GeneradorCadenasCfdi(entrada.Version);
            try
            {
                
                string cadenaOriginal;
                string strContent = entrada.XmlString;
                var errores = new Dictionary<string,int>();
                errores.Add("Estructura del archivo XML: " ,0);
                string version = entrada.Version;
                if (version == "")
                {
                    errores.Add("Versión del comprobante: " ,301);
                    return CrearArchivoRoe(errores, entrada.FileName, entrada);
                    
                }
                
                lock (generadorCadenas)
                {
                    cadenaOriginal = generadorCadenas.CadenaOriginal(strContent);
                }
                entrada.CadenaOriginal = cadenaOriginal;
                DateTime datFechaExpiracionCSD = entrada.Certificate.NotAfter;
                DateTime datFechaEfectivaCSD = entrada.Certificate.NotBefore;
                var validadorDatos = new ValidadorDatos32();
                var certificado = new X509Certificate2(entrada.Certificate.GetEncoded());

                errores.Add("Certificado emitido por el SAT: ",validadorDatos.ValidaCertificadoAc(certificado));

                errores.Add("El certificado es un CSD: " ,validadorDatos.ValidaCertificadoCSDnoFIEL(certificado));
                //-Que el sello del Emisor sea válido 
                string hash = null;
                byte[] firma = null;
                try
                {
                    firma = Convert.FromBase64String(entrada.Sello);
                }
                catch (Exception ee)
                {
                    Logger.Error(ee);
                    errores.Add("Sello del comprobante válido: ",302);
                }
                if (firma != null)
                {
                    errores.Add("Sello del comprobante válido: ",validadorDatos.ValidarSello(cadenaOriginal, firma, certificado, ref hash));
                }
                Logger.Debug(hash);
                errores.Add("RFC del emisor coincide con el RFC del certificado: ",validadorDatos.ValidaRFCEmisor(entrada.RfcEmisor, certificado.SubjectName.Name));
                
                errores.Add("Fecha de emisión del comprobante dentro del rango de validéz del certificado: ",validadorDatos.ValidaFechaEmisionXml(entrada.Fecha, datFechaExpiracionCSD, datFechaEfectivaCSD));
                if (entrada.Version == "3.2")
                {
                    //errores.Add("Fecha de timbrado del comprobante posterior a la fecha de emisión: ", validadorDatos.ValidaRangoFecha(entrada.Fecha, entrada.FechaTimbrado));
                    errores.Add("Comprobante emitido después de 2011: ", validadorDatos.ValidaFechaEmision2011(entrada.Fecha));
                    errores.Add("No. de Certificado del PAC: " + entrada.NoCertificadoSat, 0);
                    errores.Add("Verificacion del sello del PAC", validadorCertificado.VerificaSelloPac(entrada.NoCertificadoSat, entrada.SelloSat, entrada.CadenaTimbre));
                }
                else
                {
                    errores.Add("Serie y Folio :" , validadorDatos.VerificaFolioSerieCfd(entrada.Folio, entrada.Serie,entrada.NoAprobacion, entrada.AnoAprobacion));
                }
                //int suma = 0;
                //if (Math.Abs(entrada.SubTotal - entrada.SumaConceptos) > 0.01)
                //{
                //    suma = 100;
                //}
                //errores.Add("La suma de conceptos coincide con el subtotal", suma);
                return CrearArchivoRoe(errores, entrada.FileName, entrada);
                  
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                this.CrearArchivoRoe(new Dictionary<string, int> {{"Error de validacion ", 000}}, entrada.FileName,entrada, ex.Message);
                throw;
            }

        }



        private readonly Dictionary<int, string> _erroresValidacion = new Dictionary<int, string>
                                                         {
                                                             {301,"XML mal formado"},
                                                             {302,"Sello mal formado o inválido"},
                                                             {303,"Sello no corresponde a emisor o caduco"},
                                                             {304,"Certificado revocado o caduco"},
                                                             {305,"La fecha de emisión no esta dentro de la vigencia del CSD del Emisor"},
                                                             {306,"EL certificado no es de tipo CSD"},
                                                             {307,"El CFDI contiene un timbre previo"},
                                                             {308,"Certificado no expedido por el SAT"},
                                                             {401,"Fecha y hora de generación fuera de rango"},
                                                             {402,"RFC del emisor no se encuentra en el régimen de contribuyentes"},
                                                             {403,"La fecha de emisión no es posterior al 01 de enero 2011"},
                                                             {409, "El folio y serie no coinciden con el numero y año de aprobación"},
                                                             {404, "Sello SAT inválido"},
                                                             {100, "La suma de conceptos no coincide con el subtotal"}
                                                         };

        private ResultadoValidacion CrearArchivoRoe(Dictionary<string, int> errores, string archivoEntrada, ValidadorInput input, string extraInfo = "")
        {
            var resultado = new List<Validacion>();
            var errorOutput = new StringBuilder();

            errorOutput.AppendLine("Archivo Invalido");
            errorOutput.AppendLine("Path: " + archivoEntrada);
            foreach (KeyValuePair<string, int> error in errores)
            {
                resultado.Add(new Validacion
                                  {
                                      Descripcion = error.Key,
                                      Valido = (error.Value == 0),
                                      Error = (error.Value == 0 ? "OK" : _erroresValidacion[error.Value])
                                  });
            }
            bool valido = errores.All(p => p.Value == 0);
            ValidadorContract c = new ValidadorContract();
            foreach (PropertyInfo propertyInfo in input.GetType().GetProperties())
            {
                var property = c.GetType().GetProperties().Where(p => p.Name == propertyInfo.Name).FirstOrDefault();
                if (property != null)
                {
                    property.SetValue(c,propertyInfo.GetValue(input, new object[0]),new object[0]);
                }
            }
            return new ResultadoValidacion() {Valido = valido, Detalles = resultado, Entrada = c};
        }

    }
}
