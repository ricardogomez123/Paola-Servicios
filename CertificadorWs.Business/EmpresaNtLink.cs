using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace CertificadorWs.Business
{
    /// <summary>
    /// Tipo de dato que representa una empresa en el sistema de timbrado
    /// </summary>
    [DataContract]
    public class EmpresaNtLink
    {
        [DataMember]
        public string Curp { get; set; }
        [DataMember]
        public String Rfc { get; set; }
        [DataMember]
        public String RazonSocial { get; set; }
        [DataMember]
        public String Direccion { get; set; }
        [DataMember]
        public String Colonia { get; set; }
        [DataMember]
        public String Ciudad { get; set; }
        [DataMember]
        public String Estado { get; set; }
        [DataMember]
        public String Cp { get; set; }
        [DataMember]
        public String Telefono { get; set; }
        [DataMember]
        public String Email { get; set; }
        [DataMember]
        public String Contacto { get; set; }
        [DataMember]
        public String RegimenFiscal { get; set; }
        [DataMember]
        public int Folios { get; set; }
        [DataMember]
        public int Usuarios { get; set; }
        [DataMember]
        public int Sucursales { get; set; }
        [DataMember]
        public int TimbresContratados { get; set; }
        [DataMember]
        public DateTime FechaContrato { get; set; }
        
    }
}