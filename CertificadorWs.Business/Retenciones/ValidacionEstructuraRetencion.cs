using System.Collections.Generic;

namespace CertificadorWs.Business.Retenciones
{
    public class ValidacionEstructuraRetencion
    {
        public bool Valido { get; set; }
        public List<string> ErrorList { get; set; }
    }
}
