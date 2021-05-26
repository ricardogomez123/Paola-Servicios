using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServicioLocalContract.Entities
{
    public class DatosFacturaAduanera
    {
        public string SaldoLetra { get; set; }
        public string GuiaMaster { get; set; }
        public string ReferenciaInterna { get; set; }
        public string GuiaHouse { get; set; }
        public string ReferenciaPascal { get; set; }
        public string TipoCambioFletes { get; set; }
        public string Aduana { get; set; }
        public string TipoOperacion { get; set; }
        public string Pedimento { get; set; }
        public string ValorAduana { get; set; }
        public string TipoCambioPedimento { get; set; }
        public string Mercancia { get; set; }
        public string Mterr { get; set; }
        public string NoBultos { get; set; }
        public string DiasCredito { get; set; }
        public string PesoBruto { get; set; }
        public string FechaVencimiento { get; set; }
        public string Contenedores { get; set; }
        public string Facturas { get; set; }
        public string TotalAddenda { get; set; }
        public string Anticipo { get; set; }
        public string Saldo { get; set; }
        public string VoBoNombre { get; set; }
        public string NumeroPedido { get; set; }
        public string Buyer { get; set; }
        public string Seller { get; set; }
        public string FechaPedido { get; set; }
        public string DeptoContacto { get; set; }
        public string Proveedor { get; set; }
        public string Aprob { get; set; }
        public string Contrarecibo { get; set; }
        public string FechaContrarecibo { get; set; }
        public string Provedorcodigo { get; set; }
        public string EntregaCodigoPostal{get; set;}
        public string noExterior { get; set; }
        public string FechaInicial { get; set; }
        public string PlantaEntrega { get; set; }
        public string Calle { get; set; }
        public string noInterior { get; set; }
        public string FechaFinal { get; set; }
        public string correocontacto { get; set; }
        public string oficina { get; set; }
        public string origendefactura { get; set; }
        public string correoproveedor { get; set; }
        public string nopedido { get; set; }
        public string nombreproveedor { get; set; }
        public string MonedatipoMoneda { get; set; }
         public string noproveedor { get; set; }
       
       //soriana cedis
         public string Telefono { get; set; }
         public string Gln { get; set; }
         public string NoProveedor { get; set; }
         public string Descripcion { get; set; }
         public string NombreCorto { get; set; }
         public string Observaciones { get; set; }
         public string Proyecto { get; set; }
         public string tipoCambio { get; set; }
         public string ExtraAtributo { get; set; }
         public string ExtraAtributo1 { get; set; }
         public string Valor { get; set; }
         public string Valor1 { get; set; }
         public string CantidadLetra { get; set; }
         // campos soriana tienda
            //Remision
            public string ProveedorRemision { get; set; }
            public string RemisionR { get; set; }
            public string Consecutivo { get; set; }
            public string FechaRemision { get; set; }
            public string TiendaRemision { get; set; }
            public string TipoMoneda { get; set; }
            public string TipoBulto { get; set; }
            public string EntrgaMercancia { get; set; }
            public string CumpleReqFiscal { get; set; }
            public string CantidadBultos { get; set; }
            public string Subtotal { get; set; }
            public decimal Descuentos { get; set; }
            public decimal IEPS { get; set; }
            public decimal IVA { get; set; }
            public string OtrosImpuestos { get; set; }
            public string Total { get; set; }
            public string CantidadPedidos { get; set; }
            public string FechaEntrgaMercancia { get; set; }
            public int FolioNotaEntrada { get; set; }
            //pedido
           public string ProveedorPedido { get; set; }
           public string RemisionPedido { get; set; }
            public int FolioPedido { get; set; }
            public string TiendaPedido { get; set; }
            public string CantidadArticulos { get; set; }
             //Articulos
            public string ProveedorArticulos { get; set; }
            public string RemisionArticulos { get; set; }
            public string FolioPedidoArticulos { get; set; }
            public string TiendaArticulos { get; set; }
            public string Codigo { get; set; }
            public string CantidadUnidadCompra { get; set; }
            public string CostoNetoUnidadCompra { get; set; }
            public decimal PorcentajeIEPS { get; set; }
            public decimal PorecentajeIVA { get; set; }
   




               
    }


    
}
