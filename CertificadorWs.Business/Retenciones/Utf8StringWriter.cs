using System.IO;
using System.Text;

namespace CertificadorWs.Business.Retenciones
{
    public class Utf8StringWriter : StringWriter
    {
        private readonly Encoding _encoding = new UTF8Encoding(false);
        public override Encoding Encoding
        {
            get
            {
                return _encoding;
            }
        }
        
    }
}
