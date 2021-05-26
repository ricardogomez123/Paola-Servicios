using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CertificadorWs.Business
{
    [DataContract]
    public class RespuestaCancelacion
    {
        [DataMember]
        public string Acuse
        {
            get;
            set;
        }

        [DataMember]
        public string MensajeError
        {
            get;
            set;
        }

        [DataMember]
        public List<StatusUuid> StatusUuids
        {
            get;
            set;
        }
    }
}
