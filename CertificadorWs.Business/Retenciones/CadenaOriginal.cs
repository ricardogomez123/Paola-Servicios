using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Xsl;
using System.IO;
using System.Xml;
using System.Web;
using System.Web.Hosting;
using CertificadorWs.Business.Retenciones;

namespace EnvioRetenciones
{
    public class CadenaOriginal : ICloneable
    {
        private XslCompiledTransform xsltTransform = new XslCompiledTransform();
        private object lockObject = new object();


        public CadenaOriginal(string nombreXsl)
        {
            String rutaXsl;
            if (HostingEnvironment.IsHosted)
                rutaXsl = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"bin", "XslRet", nombreXsl);
            else
                rutaXsl = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "XslRet", nombreXsl);
            var xsl = File.ReadAllText(rutaXsl);
            LocalFileResolver resolver = new LocalFileResolver();
            var xsltInput = new StringReader(xsl);
            var xsltReader = new XmlTextReader(xsltInput);
            xsltReader.XmlResolver = resolver;
            xsltTransform.Load(xsltReader, new XsltSettings(false, true), resolver);
            xsltReader.Close();
        }

        public string GenerarCadenaOriginal(string xml)
        {
            lock (lockObject)
            {
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
                    Console.WriteLine("Error(CadenaOriginal)" + ex);
                }
                return HttpUtility.HtmlDecode(stringWriter.ToString()); 
            }
            
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
