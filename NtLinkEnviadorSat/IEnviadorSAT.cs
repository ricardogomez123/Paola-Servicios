#region

using System;
using System.Collections.Generic;
using System.ServiceModel; 

#endregion


namespace PACEnviadorSATConsole
{
    [ServiceContract]
    public interface IEnviadorSAT
    {
        #region Usuario

        [OperationContract]
        bool IniciarEnvioSAT();

        #endregion  

    }
}