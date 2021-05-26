#region

using System;
using System.Configuration;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.Xml;
using log4net;
using log4net.Config;
using PACEnviadorSATConsole;

#endregion

namespace PACEnviadorSATConsole
{
    internal class Program
    {
        private static readonly ILog ilgMLogger =
            LogManager.GetLogger(typeof (Program));
         
        static void Main()
        {
            try
            {
                XmlConfigurator.Configure();
                //string strLUri = ConfigurationManager.AppSettings["UriServicio"];
                //ServicePointManager.DefaultConnectionLimit = 200;
                //var shLServicio = new ServiceHost(typeof(EnviadorSAT), new Uri(strLUri)) { CloseTimeout = TimeSpan.Zero };

                //var ntbLBinding = new NetTcpBinding { MaxBufferSize = 65536, MaxReceivedMessageSize = 65536, MaxBufferPoolSize = 65536 };

                //ntbLBinding.ReaderQuotas.MaxArrayLength = 2147483647;
                //ntbLBinding.ReaderQuotas.MaxBytesPerRead = 2147483647;
                //ntbLBinding.ReaderQuotas.MaxDepth = 2147483647;
                //ntbLBinding.ReaderQuotas.MaxNameTableCharCount = 2147483647;
                //ntbLBinding.ReaderQuotas.MaxStringContentLength = 2147483647;
                 

                //ntbLBinding.Security.Mode = SecurityMode.None;
                //ntbLBinding.Security.Message.ClientCredentialType = MessageCredentialType.None;
                //ntbLBinding.Security.Transport.ClientCredentialType = TcpClientCredentialType.None;
                //ntbLBinding.Security.Transport.ProtectionLevel = ProtectionLevel.EncryptAndSign;

                //ntbLBinding.CloseTimeout = new TimeSpan(0, 1, 10, 0);
                //ntbLBinding.OpenTimeout = new TimeSpan(0, 1, 10, 0);
                //ntbLBinding.ReceiveTimeout = new TimeSpan(0, 1, 10, 0);
                //ntbLBinding.SendTimeout = new TimeSpan(0, 1, 10, 0);

                //shLServicio.AddServiceEndpoint(typeof(IEnviadorSAT), ntbLBinding, "PACEnviadorSAT");

                //var smbLBehavior = new ServiceMetadataBehavior();

                //shLServicio.Description.Behaviors.Add(smbLBehavior);

                //Binding mebLBinding = MetadataExchangeBindings.CreateMexTcpBinding();

                //shLServicio.AddServiceEndpoint(typeof(IMetadataExchange), mebLBinding, "PAC");

                //string strLNoSerie = ConfigurationManager.AppSettings["NoSerieCertificado"];
                ////shLServicio.Credentials.ServiceCertificate.SetCertificate(StoreLocation.CurrentUser, StoreName.My,
                //                                          //            X509FindType.FindBySerialNumber, strLNoSerie);
                //shLServicio.Open();
                
                ProcesoTimbre proctimbre = new ProcesoTimbre();
                proctimbre.Iniciar();
                ilgMLogger.Info("Iniciando");
                Console.ReadLine();
                Console.WriteLine("Ya termine... Ahhh!");
                //shLServicio.Close();
            }
            catch (Exception ex)
            {
                ilgMLogger.Error("Error: " + ex.Message +
                          (ex.InnerException == null ? "" : "\nExcepción Interna:" + ex.InnerException.Message));
            }
        }
    }
}