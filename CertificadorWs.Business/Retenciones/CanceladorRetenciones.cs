using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Xml;
using EnvioRetenciones;
using CertificadorWs.Business.AutenticaRecepcion;
using System.Net;
using System.Net.Security;
using System.Web;
using System.Xml.Linq;//aqui la clave

namespace CertificadorWs.Business.Retenciones
{
    public class CanceladorRetenciones
    {
        private String token;
         private X509Certificate2 certificadoCliente;
         

        private string Autentica()
        {
           // System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
           
                var clienteAuthCan = new AutenticaCan.AutenticacionClient();
            //clienteAuthCan.ClientCredentials.ClientCertificate.Certificate = certificadoCliente;
            return clienteAuthCan.Autentica();
        }
        /*private string Autentica()
        {
            //aqui es de envio retencion para cancelar
            AutenticaRecepcion.AutenticacionClient auth = new AutenticacionClient();
            // auth.ClientCredentials.ClientCertificate.Certificate = certificadoCliente;
            return auth.Autentica();
        }*/


        //public string CancelaCfdis(List<string> listaUuids, RSACryptoServiceProvider llavePrivada, string rfcEmisor)
        //{


        //    if (token == null)
        //        token = Autentica();
        //    else
        //    {
        //        DateTime fechaExpira = Utils.GetExpiryTime(token);
        //        var dif = fechaExpira - DateTime.Now;
        //        if (dif.TotalSeconds < 100)// quedan menos de 100 segundos
        //        {
        //            token = Autentica();
        //        }
        //    }

        //    var messageCan = new HttpRequestMessage();
        //    MensajeCancelacion can = new MensajeCancelacion();
        //    var canRequest = can.CerarMensajeCancelacion(listaUuids, rfcEmisor, llavePrivada, "csd.cer");
            
        //    messageCan.RequestUri = new Uri("https://cancelaretencion.cloudapp.net/api/cancela");
        //    //content.Add(new StreamContent(ms), "file", UUID + ".xml");
        //    messageCan.Headers.Add("Authorization", "WRAP access_token=\"" + token + "\"");
        //    messageCan.Method = HttpMethod.Post;
        //    messageCan.Content = new StringContent(canRequest, Encoding.UTF8, "application/xml");
        //    messageCan.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/xml");

        //    var clientCan = new HttpClient();
        //    clientCan.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));

        //    var responseCan = clientCan.SendAsync(messageCan);
        //    responseCan.Wait();
        //    var contentAsyncCan = responseCan.Result.Content.ReadAsStringAsync();
        //    contentAsyncCan.Wait();
        //    XmlReader reader = new XmlTextReader(new StringReader(contentAsyncCan.Result));
        //    string folio = null;
        //    while (reader.Read())
        //    {
        //        if (reader.NodeType == XmlNodeType.Element && reader.LocalName == "Folio")
        //        {
        //            reader.Read();
        //            folio = reader.Value;
        //            break;
        //        }

        //    }
        //    return folio;
        //}

        //rgv para que funcionara
      //  public string CancelaCfdi(string uuid, RSACryptoServiceProvider llavePrivada, string rfcEmisor, byte [] rutaCert)
        public string CancelaCfdi(string uuid, RSACryptoServiceProvider llavePrivada, string rfcEmisor, string rutaCert)
            
    {
            
            
            if (token == null)
                token = Autentica();
            else
            {
                DateTime fechaExpira = Utils.GetExpiryTime(token);
                var dif = fechaExpira - DateTime.Now;
                if (dif.TotalSeconds < 100)// quedan menos de 100 segundos
                {
                    token = Autentica();
                }
            }
            
            

           // string action = "api/cancelauno";
            //string baseUrl = "https://prodretencioncancelacion.clouda.sat.gob.mx/";
            //string baseUrl = "https://cancelaretencion.cloudapp.net/";
            var message=new HttpRequestMessage();
            message.Headers.Add("Authorization", string.Format("WRAP access_token=\"{0}\"",token));
            var content = new MultipartFormDataContent();
            message.Method = HttpMethod.Post;
            var uri = ConfigurationManager.AppSettings["UrlCancelaRet"];
            message.RequestUri = new Uri(uri);
           
           
            MensajeCancelacion can = new MensajeCancelacion();
            var canRequest = can.CerarMensajeCancelacion(new List<string>() { uuid }, rfcEmisor, llavePrivada, rutaCert);
           /* using (StreamReader sr = new StreamReader(@"c:\pruebas\rgv.xml"))
            {
                string cadena = sr.ReadToEnd();
                XDocument doc = XDocument.Parse(cadena);
                string cadenaXML = doc.ToString();
                message.Content = new StringContent(cadenaXML, Encoding.UTF8, "application/xml");
            }*/
            
            message.Content = new StringContent(canRequest, Encoding.UTF8, "application/xml");
          
            
            message.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/xml");
            var clientCan = new HttpClient();
            clientCan.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));
            clientCan.Timeout = TimeSpan.Parse("01:30:00");
           
           /*
            clientCan.SendAsync(message).ContinueWith(task =>
            {
                if (task.Result.IsSuccessStatusCode)
                {
                    var result = task.Result;
                    var resulstream = result.Content.ReadAsStreamAsync().Result;
                    XDocument xdoc = XDocument.Load(resulstream);
                    string nombreFile = @"c:\pruebas\rgv2.xml";
                    xdoc.Save(nombreFile);
                }
            });
            return null;
             */
            var response = clientCan.SendAsync(message);
            response.Wait();
            var contentAsyncCan = response.Result.Content.ReadAsStringAsync();
            contentAsyncCan.Wait();

            return contentAsyncCan.Result;
             
 
            /*
            var messageCan = new HttpRequestMessage();
            MensajeCancelacion can = new MensajeCancelacion();
            var canRequest = can.CerarMensajeCancelacion(new List<string>(){uuid}, rfcEmisor, llavePrivada, rutaCert);
            var uri = ConfigurationManager.AppSettings["UrlCancelaRet"];
            messageCan.RequestUri = new Uri(uri);
            //content.Add(new StreamContent(ms), "file", UUID + ".xml");
          //  messageCan.Headers.Add("Authorization", "WRAP access_token=\"" +HttpUtility.UrlDecode( token )+ "\"");
            messageCan.Headers.Add("Authorization", "WRAP access_token=\"" + token + "\"");
         
            messageCan.Method = HttpMethod.Post;
            messageCan.Content = new StringContent(canRequest, Encoding.UTF8, "application/xml");
            messageCan.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/xml");

            var clientCan = new HttpClient();
            clientCan.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));

            var responseCan = clientCan.SendAsync(messageCan);
            responseCan.Wait();
            var contentAsyncCan = responseCan.Result.Content.ReadAsStringAsync();
            contentAsyncCan.Wait();
            return contentAsyncCan.Result;
              
             */ 
        }



        public string ConsultaStatus(string folio)
        {

            
            if (token == null)
                token = Autentica();
            else
            {
                DateTime fechaExpira = Utils.GetExpiryTime(token);
                var dif = fechaExpira - DateTime.Now;
                if (dif.TotalSeconds < 100)// quedan menos de 100 segundos
                {
                    token = Autentica();
                }
            }

          //  System.Net.ServicePointManager.Expect100Continue = false;
            
            var messageCan = new HttpRequestMessage();
            messageCan.RequestUri = new Uri("https://cancelaretencion.cloudapp.net/api/consultaacuse/" + folio);
            messageCan.Headers.Add("Authorization", "WRAP access_token=\"" + token + "\"");
            messageCan.Method = HttpMethod.Get;
            
            var clientCan = new HttpClient();
            clientCan.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));

            var responseCan = clientCan.SendAsync(messageCan);
            responseCan.Wait();
            var contentAsyncCan = responseCan.Result.Content.ReadAsStringAsync();
            contentAsyncCan.Wait();
            return contentAsyncCan.Result;
        }

        public CanceladorRetenciones()
        {
            //this.certificadoCliente = clientCert;
            var thumb = ConfigurationManager.AppSettings["ThumbPrint"];
            var store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
            store.Open(OpenFlags.ReadOnly);
            foreach (X509Certificate2 x509Certificate2 in store.Certificates)
            {
                if (x509Certificate2.GetSerialNumberString() == thumb)
                    this.certificadoCliente = x509Certificate2;

            }
            store.Close();

        }




    }
}
