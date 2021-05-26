using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml;
using System.Xml.Xsl;
using log4net;

namespace ServicioLocal.Business
{

    public class GeneradorCadenasCfdi : ICloneable, IDisposable
    {


        class LocalFileResolver : XmlUrlResolver
        {
            public string DirectorioInicial { get; set; }
            public override Uri ResolveUri(Uri baseUri, string relativeUri)
            {

                return base.ResolveUri(new Uri(DirectorioInicial), relativeUri);
            }
        }

        private XslCompiledTransform xsltTransform = new XslCompiledTransform();
        private static readonly ILog Log = LogManager.GetLogger(typeof(GeneradorCadenasCfdi));


        public GeneradorCadenasCfdi(string version)
        {
            try
            {
                string xsl;
                LocalFileResolver resolver = new LocalFileResolver();
                if (version == "2.2")
                {
                    resolver.DirectorioInicial = Path.Combine(ConfigurationManager.AppSettings["RutaArchivosXsl"], "2.2") + "\\";
                    xsl = File.ReadAllText(Path.Combine(resolver.DirectorioInicial, "cadenaoriginal_2_2.xslt"));
                }
                else
                {
                   // resolver.DirectorioInicial = Path.Combine(ConfigurationManager.AppSettings["RutaArchivosXsl"], "3.2" + "\\");
                   // xsl = File.ReadAllText(Path.Combine(resolver.DirectorioInicial, "cadenaoriginal_3_2.xslt"));
                    resolver.DirectorioInicial = Path.Combine(ConfigurationManager.AppSettings["RutaArchivosXsl"], "3.3" + "\\");
                    xsl = File.ReadAllText(Path.Combine(resolver.DirectorioInicial, "cadenaoriginal_3_3.xslt"));
                
                }
                var xsltInput = new StringReader(xsl);
                var xsltReader = new XmlTextReader(xsltInput);
                xsltTransform.Load(xsltReader, new XsltSettings(false, true), resolver);
            }
            catch (Exception exception)
            {
                Log.Error(exception);
            }
        }

        public string CadenaOriginal(XmlTextReader xmlReader)
        {
            StringWriter stringWriter = new StringWriter();
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
        

        public string CadenaOriginal(string xml)
        {
            if (string.IsNullOrEmpty(xml))
            {
                throw new ArgumentException("Archivo XML Inválido", "xml");
            }
            StringReader xmlInput = new StringReader(xml);
            XmlTextReader xmlReader = new XmlTextReader(xmlInput);
            StringWriter stringWriter = new StringWriter();
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

        public object Clone()
        {
            
            return this.MemberwiseClone();
        }

        public void Dispose()
        {
            this.xsltTransform = null;
            //GC.SuppressFinalize(this);
        }
 

    }
}
