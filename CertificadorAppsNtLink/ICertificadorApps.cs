using System;
using System.ServiceModel;

namespace CertificadorAppsNtLink
{
    [ServiceContract]
    public interface ICertificadorApps
    {
        [OperationContract]
        string TimbraCfdi(string userName, string password, string comprobante);

        [OperationContract]
        string TimbraRetencion(string userName, string password, string comprobante);

        [OperationContract]
        string Ping();
    }
}
