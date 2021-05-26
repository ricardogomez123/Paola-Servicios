using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace CertificadorWs
{
    /// <summary>
    /// Estructura de regreso del metodo TimbraCfdiQr
    /// </summary>
    [DataContract]
    public class TimbradoResponse
    {
     
        /// <summary>
        /// El comprobante timbrado
        /// </summary>
        [DataMember]
        public string Cfdi { get; set; }
        /// <summary>
        /// Cadena Original del complemento de certificación
        /// </summary>
        [DataMember]
        public string CadenaTimbre { get; set; }

        /// <summary>
        /// QrCode codificado en BMP de 4 bits de color, expresado en BASE64
        /// </summary>
        [DataMember]
        public string QrCodeBase64 { get; set; }


        /// <summary>
        /// Para indicar si el comprobante es válido
        /// </summary>
        [DataMember]
        public bool Valido { get; set; }


        /// <summary>
        /// Descripcion del error 
        /// </summary>
        [DataMember]
        public string DescripcionError { get; set; }

    }
}