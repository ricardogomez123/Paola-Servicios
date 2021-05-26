using System;
using System.ServiceModel;

namespace CertificadorWs
{
    [ServiceContract(Namespace = "https://ntlink.com.mx/ICertificador")]
    public interface ICertificador
    {
        [OperationContract]
        string TimbraCfdi(string comprobante, string userName, string password, string LLave, string aplicacion);
  

        [OperationContract]
        string CancelaCfdi(string uuid, string rfcEmisor, string expresion, string rfcReceptor);

        [OperationContract]
        string CancelaRetencion(string uuid, string rfc);

        [OperationContract]
        string ValidaTimbraCfdi(string comprobante);

        [OperationContract]
        string ValidaRFC(string RFC);

        [OperationContract]
        string ConsultaEstatusCFDI(string expresion);
        [OperationContract]

        string TimbraRetencion(string comprobante, string userName, string password, string LLave, string aplicacion);

        [OperationContract]
        string Activar(string Llave, string RFC);
      
    }
}