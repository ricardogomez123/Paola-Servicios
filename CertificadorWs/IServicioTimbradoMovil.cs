using System;
using System.ServiceModel;

namespace CertificadorWs
{
    [ServiceContract(Namespace = "https://ntlink.com.mx/IServicioTimbradoMovil")]
    public interface IServicioTimbradoMovil
    {
        [OperationContract]
        string ObtenerEmpresasFolio(string userName, string password, string RFC);

        [OperationContract]
        TimbradoResponse TimbraCfdiMovil(string userName, string password, string comprobante);
    }
}
