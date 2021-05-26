using System;

namespace CertificadorWs.Business.Retenciones
{
    public class RetencionesItem
    {
        public DateTime Fechaexp { get; set; }
        public string Version { get; set; }
        public Int64 Folio { get; set; }
        public string Serie { get; set; }
        public string Sello { get; set; }
        public string EmpresaRfc { get; set; }
        public string ReceptorRfc { get; set; }
        public string Certificado { get; set; }
        public string NoCertificado { get; set; }
    }
}
