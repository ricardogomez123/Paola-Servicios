using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace CertificadorWs
{
    [DataContract]
    public enum StatusComprobante
    {
        [EnumMemberAttribute]
        NoEncontrado,
        [EnumMemberAttribute]
        EnProceso,
        [EnumMemberAttribute]
        Enviado,
        [EnumMemberAttribute]
        Cancelado
    }

    [DataContract]
    public class ResultadoConsulta
    {
        [DataMember]
        public StatusComprobante Status { get; set; }
        [DataMember]
        public string AcuseEnvio { get; set; }
        [DataMember]
        public string AcuseCancelacion { get; set; }
        [DataMember]
        public string Comprobante { get; set; }
    }
}