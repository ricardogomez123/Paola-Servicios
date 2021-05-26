using System;
using System.Text;
using System.Web;
using System.Xml;
using System.Xml.Xsl;
using System.IO;
using ServicioLocalContract;
using log4net;
using System.Configuration;

namespace CertificadorWs.Business
{
    public class GeneradorCadenasCfd
    {
        private XmlTextReader xsltReader;
        private StringReader xsltInput;
        private XslCompiledTransform xsltTransform = new XslCompiledTransform();
        private static readonly ILog Log = LogManager.GetLogger(typeof(GeneradorCadenasCfd));

        class LocalFileResolver : XmlUrlResolver
        {
            public override Uri ResolveUri(Uri baseUri, string relativeUri)
            {

                return base.ResolveUri(new Uri(ConfigurationManager.AppSettings["RutaXslt"] + "\\"), relativeUri);
            }
        }



        public GeneradorCadenasCfd()
        {

            try
            {
                LocalFileResolver resolver = new LocalFileResolver();
                var xsl = File.ReadAllText(ConfigurationManager.AppSettings["RutaXslt"] + "\\cadenaoriginal_3_3.xslt");
                xsltInput = new StringReader(xsl);
                xsltReader = new XmlTextReader(xsltInput);
                xsltTransform.Load(xsltReader, new XsltSettings(false, true), resolver);
            }
            catch (Exception exception)
            {
                Log.Error("Error(GeneradorCadenas):" + exception);
            }

        }

        public GeneradorCadenasCfd(string path)
        {
            var cwd = Environment.CurrentDirectory;
            try
            {
                var xsl = File.ReadAllText(path + "\\cadenaoriginal_3_3.xslt");
                Environment.CurrentDirectory = path;
                xsltInput = new StringReader(xsl);
                xsltReader = new XmlTextReader(xsltInput);
                xsltTransform.Load(xsltReader);
            }
            catch (Exception exception)
            {
                Log.Error("Error(GeneradorCadenas):" + exception);
            }
            finally
            {
                Environment.CurrentDirectory = cwd;
            }
        }

        public string CadenaOriginal(string xml)
        {
            if (string.IsNullOrEmpty(xml))
            {
                throw new ArgumentException("Archivo XML Inválido", "xml");
            }
            StringReader xmlInput = new StringReader(xml);
            
            XmlTextReader xmlReader = new XmlTextReader(xmlInput);
            SidetecStringWriter stringWriter = new SidetecStringWriter(Encoding.UTF8);
            XmlTextWriter transformedXml = new XmlTextWriter(stringWriter);
            try
            {
                xsltTransform.Transform(xmlReader, transformedXml);
            }
            catch (Exception ex)
            {
                Log.Error("Error(CadenaOriginal)" + ex);
                throw;
            }
            return HttpUtility.HtmlDecode(stringWriter.ToString());
        }
    }
}
