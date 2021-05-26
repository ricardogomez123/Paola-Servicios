using System;
using System.IO;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using EnvioRetenciones;
using CertificadorWs.Business.AutenticaRecepcion;
using System.Configuration;
using System.Net;
using log4net;

namespace CertificadorWs.Business.Retenciones
{
    public class EnviadorRetenciones
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(EnviadorRetenciones));

        private String token;
        //private X509Certificate2 certificadoCliente;

        public AcuseRecepcionRetencion EnviarRetencion(string cfdRetencion, string uuid)
        {
            try
            {
                try
                {
                    if (token == null)
                        token = Autentica();
                    //else
                    //{
                    //   var f= Utils.GetExpiryTime2(token);
                    //   if (f != null)
                    //   {
                    //       DateTime fechaExpira = Convert.ToDateTime(f);
                    //       var dif = fechaExpira - DateTime.Now;
                    //       if (dif.TotalSeconds < 100)// quedan menos de 100 segundos
                    //       {
                    //           token = Autentica();
                    //       }
                    //   }
                    //   else
                    //   {
                    //       token = Autentica();
                    //       EnviadorRetenciones.Logger.Info("Error de tiempo expiro token:"+token); 
                    //   }
                    //}
                }
                catch (Exception ex)
                {
                    token = Autentica();
                    EnviadorRetenciones.Logger.Info("Error de tiempo expiro token:" + ex.Message); 
                }

                var message = new HttpRequestMessage();
                var content = new MultipartFormDataContent();
                MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(cfdRetencion));
                message.RequestUri = new Uri(ConfigurationManager.AppSettings["UriRetencion"]);
                // message.RequestUri = new Uri("https://servicioretencion.cloudapp.net/api/Recibe?versionEsquema=1.0");
                //content.Add(new StreamContent(File.OpenRead("RetTimbrado.xml")), "File", UUID + ".xml");
                content.Add(new StreamContent(ms), "file", uuid + ".xml");
                message.Headers.Add("Authorization", "WRAP access_token=\"" + token + "\"");
                message.Method = HttpMethod.Post;
                message.Content = content;
                var client = new HttpClient(new HttpClientHandler());
                Console.WriteLine("----------------------------------- Envio ----------------------------");

                //para validar al certificado del servicio----------------------
                ServicePointManager.ServerCertificateValidationCallback = ((remitente, certificado, cadena, sslPolicyErrors) => true);
                //--------
                EnviadorRetenciones.Logger.Info("Inicia Recibe");
                var response = client.SendAsync(message);
                //response.Wait();
                var contentAsync = response.Result.Content.ReadAsStringAsync();
                contentAsync.Wait();
                EnviadorRetenciones.Logger.Info("Termina Recibe");
                DataContractJsonSerializer resSerializer = new DataContractJsonSerializer(typeof(RootObject));
                var salida = (RootObject)resSerializer.ReadObject(new MemoryStream(Encoding.UTF8.GetBytes(contentAsync.Result), false));
                salida.AcuseRecepcionRetencion.StrAcuse = contentAsync.Result;
                return salida.AcuseRecepcionRetencion;
            }
            catch (Exception ex)
            { 
                 EnviadorRetenciones.Logger.Info("Error inesperado:"+ex.Message);
                 return null;
            }
        }

        private string AutenticaRetenciones()
        {
            SAT.CFDI.Cliente.Procesamiento.AutenticaRetencion.AutenticacionClient auth = new SAT.CFDI.Cliente.Procesamiento.AutenticaRetencion.AutenticacionClient();
            return auth.Autentica();
        }

        private string Autentica()
        {
               //aqui
            AutenticaRecepcion.AutenticacionClient auth = new AutenticacionClient();
           // auth.ClientCredentials.ClientCertificate.Certificate = certificadoCliente;
            EnviadorRetenciones.Logger.Info("Autenticacion");
            return auth.Autentica();
        }


        public EnviadorRetenciones()
        {
            //this.certificadoCliente = clientCert;
                
        }
    }
}
