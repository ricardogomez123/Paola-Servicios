using System.Runtime.Serialization;

namespace ServicioLocalContract
{
    public class SorianaArticulos
    {
        public decimal TotalPartida { get;set;}
        public int Partida { get; set; }
        public int? PorcentajeIva { get; set; }
        public decimal? ImporteIva { get; set; }
        public decimal? PorcentajeRetencionIva { get; set; }
        public decimal? RetencionIva { get; set; }
        public int Proveedor { get; set; }
        public string Remision { get; set; }

        [DataMemberAttribute]
        public string CuentaPredial { get; set; }

    }
}
