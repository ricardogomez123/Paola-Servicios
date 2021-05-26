using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ServicioLocal.Business
{
    public class ValidadorInput
    {
        
        public Guid Id { get; set; }
        public string Version { get; set; }
        public string Sello { get; set; }
        public string Certificado { get; set; }
        public Org.BouncyCastle.X509.X509Certificate Certificate { get; set; }
        public DateTime Fecha { get; set; }
        public string RfcEmisor { get; set; }
        public string NoCertificado { get; set; }
        public List<string> Errores { get; set; }
        public bool Valido { get; set; }
        public string CadenaOriginal { get; set; }
        public byte[] Hash { get; set; }
        public bool SelloValido { get; set; }
        public override string ToString()
        {
            return Version + "|" + Sello + "|" + Certificado + "|" + Fecha + "|" + RfcEmisor + "|" + NoCertificado + "|" +
                   Valido;
        }

        public bool CertificadoValido { get; set; }
        public bool CertificadoNoFiel { get; set; }
        public string XmlString { get; set; }
        public string CadenaTimbre { get; set; }
        public string SelloSat { get; set; }
        public string NoCertificadoSat { get; set; }
        public string FileName { get; set; }
        public DateTime FechaTimbrado { get; set; }
        public string Folio { get; set; }
        public string Serie { get; set; }
        public string NoAprobacion { get; set; }
        public string AnoAprobacion { get; set; }
        public double Total { get; set; }
        public double SubTotal { get; set; }
        public double SumaImpuestos { get; set; }
        public double SumaRetenciones { get; set; }
        public double SumaConceptos { get; set; }
    }
}
