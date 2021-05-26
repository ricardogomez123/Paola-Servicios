using CertificadorWs.Business;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace CertificadorWs
{
    [ServiceContract(Namespace = "https://ntlink.com.mx/IServicioTimbrado")]
    public interface IServicioTimbrado
    {
        [OperationContract]
        int ConsultaSaldo(string userName, string password);

        [OperationContract]
        string ConsultaAceptacionRechazo(string userName, string password, string rfcEmisor, string rfcReceptor);

        [OperationContract]
        string ProcesarRespuestaAceptacionRechazo(string userName, string password, List<Folios> uuid, string rfcEmisor, string rfcReceptor);

        [OperationContract]
        string ConsultaEstatusCFDI(string userName, string password, string expresion);

        [OperationContract]
        string ConsultaCFDIRelacionados(string userName, string password, string uuid, string rfcEmisor, string rfcReceptor);

        [OperationContract]
        void RegistraEmpresa(string userName, string password, EmpresaNtLink empresa);

        [OperationContract]
        void BajaEmpresa(string userName, string password, string rfcEmpresa);

        [OperationContract]
        ClienteNtLink ObtenerDatosCliente(string userName, string password);

        [OperationContract]
        List<EmpresaNtLink> ObtenerEmpresas(string userName, string password);

        [OperationContract]
        TimbradoResponse TimbraCfdiQr(string userName, string password, string comprobante);

        [OperationContract]
        TimbradoResponse TimbraCfdiQrSinSello(string userName, string password, string comprobante);

        [OperationContract]
        string TimbraCfdi(string userName, string password, string comprobante);

        [OperationContract]
        string TimbraCfdiSinSello(string userName, string password, string comprobante);

        [OperationContract]
        string TimbraRetencion(string userName, string password, string comprobante);

        [OperationContract]
        TimbradoResponse TimbraRetencionQr(string userName, string password, string comprobante);

        [OperationContract]
        string CancelaRetencion(string userName, string password, string uuid, string rfc);

        [OperationContract]
        string CancelaCfdi(string userName, string password, string uuid, string rfc, string expresion, string rfcReceptor);
        [OperationContract]
        string CancelaCfdiOtrosPACs(string uuid, string rfcEmisor, string expresion, string rfcReceptor, string Base64Cer, string Base64Key, string PasswordKey);
       
        [OperationContract]
        RespuestaCancelacion CancelaCfdiRequest(string userName, string password, string requestCancelacion, string expresion, string uuid, string rfcReceptor);

        [OperationContract]
        ResultadoConsulta ObtenerStatusUuid(string userName, string password, string uuid);

        [OperationContract]
        ResultadoConsulta ObtenerStatusHash(string userName, string password, string hash);
    }
}
