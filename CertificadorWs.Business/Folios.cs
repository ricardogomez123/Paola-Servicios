using System;

namespace CertificadorWs.Business
{
    public class Folios
    {
        public enum R
        {
            Aceptacion,
            Rechazo
        }

        public string UUDI
        {
            get;
            set;
        }

        public Folios.R Respuesta
        {
            get;
            set;
        }
    }
}
