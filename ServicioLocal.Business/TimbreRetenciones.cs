using System;
using System.Xml.Serialization;

namespace ServicioLocal.Business.TimbreRetenciones
{
    [XmlRoot(Namespace = "http://www.sat.gob.mx/TimbreFiscalDigital", IsNullable = false), XmlType(AnonymousType = true, Namespace = "http://www.sat.gob.mx/TimbreFiscalDigital")]
    [Serializable]
    public class TimbreFiscalDigital
    {
        [XmlAttribute("schemaLocation", Namespace = "http://www.w3.org/2001/XMLSchema-instance")]
        //public string xsiSchemaLocation = "http://www.sat.gob.mx/TimbreFiscalDigital http://www.sat.gob.mx/sitio_internet/cfd/TimbreFiscalDigital/TimbreFiscalDigital.xsd";
        public string xsiSchemaLocation = "http://www.sat.gob.mx/TimbreFiscalDigital http://www.sat.gob.mx/TimbreFiscalDigital/TimbreFiscalDigital.xsd";

        private string versionField;

        private string uUIDField;

        private DateTime fechaTimbradoField;

        private string selloCFDField;

        private string noCertificadoSATField;

        private string selloSATField;

        [XmlIgnore]
        public string cadenaOriginal
        {
            get;
            set;
        }

        [XmlAttribute]
        public string version
        {
            get
            {
                return this.versionField;
            }
            set
            {
                this.versionField = value;
            }
        }

        [XmlAttribute]
        public string UUID
        {
            get
            {
                return this.uUIDField;
            }
            set
            {
                this.uUIDField = value;
            }
        }

        [XmlAttribute]
        public DateTime FechaTimbrado
        {
            get
            {
                return this.fechaTimbradoField;
            }
            set
            {
                this.fechaTimbradoField = value;
            }
        }

        [XmlAttribute]
        public string selloCFD
        {
            get
            {
                return this.selloCFDField;
            }
            set
            {
                this.selloCFDField = value;
            }
        }

        [XmlAttribute]
        public string noCertificadoSAT
        {
            get
            {
                return this.noCertificadoSATField;
            }
            set
            {
                this.noCertificadoSATField = value;
            }
        }

        [XmlAttribute]
        public string selloSAT
        {
            get
            {
                return this.selloSATField;
            }
            set
            {
                this.selloSATField = value;
            }
        }

        public TimbreFiscalDigital()
        {
            this.versionField = "1.0";
        }
    }
}
