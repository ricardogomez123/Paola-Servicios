using System.Collections.Generic;

namespace CertificadorWs.Business.Retenciones
{
    public class CanonicalizationMethod
    {
    }

    public class SignatureMethod
    {
    }

    public class Transform
    {
        public object text { get; set; }
        public string algorithm { get; set; }
    }

    public class DigestMethod
    {
    }

    public class Reference
    {
        public List<Transform> transforms { get; set; }
        public DigestMethod digestMethod { get; set; }
        public string digestValue { get; set; }
        public object id { get; set; }
        public string uRI { get; set; }
        public object type { get; set; }
    }

    public class SignedInfo
    {
        public CanonicalizationMethod canonicalizationMethod { get; set; }
        public SignatureMethod signatureMethod { get; set; }
        public Reference reference { get; set; }
        public object id { get; set; }
    }

    public class RSAKeyValueKBackingField
    {
        public string modulus { get; set; }
        public string exponent { get; set; }
    }

    public class KeyValueKBackingField
    {
        public object text { get; set; }
    }

    public class KeyInfo
    {
        public object itemsElementName { get; set; }
        public object text { get; set; }
        public object id { get; set; }
    }

    public class Signature
    {
        public SignedInfo signedInfo { get; set; }
        public string signatureValue { get; set; }
        public KeyInfo keyInfo { get; set; }
        public object objectType { get; set; }
        public string id { get; set; }
    }

    public class ListaIncidencia
    {
        public string MensajeIncidencia { get; set; }
        public string NoCertificadoPac { get; set; }
        public string CodigoError { get; set; }
        public object RfcEmisor { get; set; }
        public object RfcReceptor { get; set; }
        public string IdIncidencia { get; set; }
        public string Uuid { get; set; }
        public string WorkProcessId { get; set; }
        public string FechaRegistro { get; set; }
    }

    public class AcuseRecepcionRetencion
    {
        public string StrAcuse { get; set; }
        public string uuid { get; set; }
        public Signature signature { get; set; }
        public string codEstatus { get; set; }
        public string fecha { get; set; }
        public string noCertificadoSAT { get; set; }
        public List<ListaIncidencia> listaIncidencia { get; set; }
    }

    public class RootObject
    {
        public AcuseRecepcionRetencion AcuseRecepcionRetencion { get; set; }
        
    }
}
