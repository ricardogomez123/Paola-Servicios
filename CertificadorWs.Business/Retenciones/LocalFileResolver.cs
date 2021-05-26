using System;
using System.IO;
using System.Xml;

namespace CertificadorWs.Business.Retenciones
{
    public class LocalFileResolver : XmlUrlResolver
    {



        public override Uri ResolveUri(Uri baseUri, string relativeUri)
        {
            
                return base.ResolveUri(new Uri(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "XslRet") + "\\"), relativeUri);
            
        }
    }
}
