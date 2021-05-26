#region

using System;
using System.Collections.Generic;
using System.ServiceModel.Activation;
 
#endregion

namespace PACEnviadorSATConsole
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class EnviadorSAT : IEnviadorSAT
    { 
        #region Usuario 

        public bool IniciarEnvioSAT()
        {
            return true;
        } 

        #endregion  
       
    }
}