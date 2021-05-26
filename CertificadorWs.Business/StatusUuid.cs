using System;
using System.Runtime.Serialization;

namespace CertificadorWs.Business
{
    [DataContract]
    public class StatusUuid
    {
        [DataMember]
        public string Uuid
        {
            get;
            set;
        }

        [DataMember]
        public string Status
        {
            get;
            set;
        }
    }
}