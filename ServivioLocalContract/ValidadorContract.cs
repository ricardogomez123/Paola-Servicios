using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace ServicioLocalContract
{
    [DataContract]
    public class ValidadorContract
    {
        [DataMember]
        public string Version { get; set; }

        [DataMember]
        public string Sello { get; set; }

        [DataMember]
        public string Certificado { get; set; }

        [DataMember]
        public DateTime Fecha { get; set; }

        [DataMember]
        public string RfcEmisor { get; set; }

        [DataMember]
        public string NoCertificado { get; set; }

        [DataMember]
        public List<string> Errores { get; set; }

        [DataMember]
        public bool Valido { get; set; }

        [DataMember]
        public string CadenaOriginal { get; set; }

        [DataMember]
        public byte[] Hash { get; set; }

        [DataMember]
        public bool SelloValido { get; set; }

        [DataMember]
        public bool CertificadoValido { get; set; }

        [DataMember]
        public bool CertificadoNoFiel { get; set; }

        [DataMember]
        public string XmlString { get; set; }

        [DataMember]
        public string CadenaTimbre { get; set; }

        [DataMember]
        public string SelloSat { get; set; }

        [DataMember]
        public string NoCertificadoSat { get; set; }

        [DataMember]
        public string FileName { get; set; }

        [DataMember]
        public DateTime FechaTimbrado { get; set; }

        [DataMember]
        public string Folio { get; set; }

        [DataMember]
        public string Serie { get; set; }

        [DataMember]
        public string NoAprobacion { get; set; }

        [DataMember]
        public string AnoAprobacion { get; set; }

        [DataMember]
        public double Total { get; set; }

        [DataMember]
        public double SubTotal { get; set; }

        [DataMember]
        public double SumaImpuestos { get; set; }

        [DataMember]
        public double SumaRetenciones { get; set; }

        [DataMember]
        public double SumaConceptos { get; set; }

    }
}
