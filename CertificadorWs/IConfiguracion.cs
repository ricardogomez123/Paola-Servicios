using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using CertificadorWs.Business;

namespace CertificadorWs
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IConfiguracion" in both code and config file together.
    [ServiceContract(Namespace = "https://ntlink.com.mx/IConfiguracion")]
    public interface IConfiguracion
    {
        [OperationContract]
        void RegistraEmpresa(EmpresaNtLink empresa, byte[] llave, byte[] certificado, string userName, string password);

        [OperationContract]
        ClienteNtLink ObtenerDatosCliente(string userName, string password);

        [OperationContract]
        List<EmpresaNtLink> ObtenerEmpresas(string userName, string password);
    }
}
