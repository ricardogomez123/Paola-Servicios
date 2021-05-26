using System.Xml.Serialization;

namespace CertificadorWs.Business.Retenciones
{
    public partial class Retenciones
    {
        [XmlIgnore]
        public string XmlString { get; set; }

        [XmlIgnore]
        public string CadenaOriginal { get; set; }

        [XmlIgnore]
        public string CadenaOriginalTimbre { get; set; }
    }
}