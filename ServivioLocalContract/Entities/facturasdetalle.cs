using System.Runtime.Serialization;
namespace ServicioLocalContract
{
    public partial class facturasdetalle
    {
        public decimal TotalPartida {get { return this._Cantidad * this.Precio; }}
        public int Partida { get; set; }
        public int? PorcentajeIva { get; set; }
        public decimal? ImporteIva { get; set; }
        public decimal? PorcentajeRetencionIva { get; set; }
        public decimal? RetencionIva { get; set; }
        [DataMemberAttribute]
        public string CuentaPredial { get; set; }
        //Artuculos para soriana  jajajaj
        [DataMemberAttribute]
        public int Proveedor { get; set; }
        
        public string folioPedido { get; set; }

        public short tienda { get; set; }

        //public decimal codigo { get; set; }

        public decimal cantidadUnidadCompra { get; set; }

        public decimal costoNetoUnidadCompra { get; set; }

        public decimal porcentajeIEPS { get; set; }

        public decimal porcentajeIVA { get; set; }
    
        [DataMemberAttribute]
        public string Remision { get; set; }
        [DataMemberAttribute]
        public string ModeloConcepto { get; set; }
    }
}
