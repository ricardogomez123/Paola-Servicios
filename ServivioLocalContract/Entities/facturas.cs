using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using ServicioLocalContract.Entities;

namespace ServicioLocalContract
{
    public enum TipoDocumento
    {
        FacturaGeneral,
        FacturaTransportista,
        FacturaAduanera,
        Referencia,
        ReciboHonorarios,
        Constructor,
        Donativo,
        NotaCredito,
        Arrendamiento,
        FacturaGeneralFirmas,
        ConstructorFirmas,
        ConstructorFirmasCustom,
        FacturaLiverpool,
        FacturaMabe,
        FacturaDeloitte,
        FacturaSorianaCEDIS,
        FacturaSorianaTienda,
        FacturaAdo,
        CorporativoAduanal,
        FacturaPemex, 
        FacturaLucent, Nomina, Amc71,CartaPorte, Coppel
    }

    public partial class facturas
    {

        [DataMember]
        public AddendaCoppel AddendaCoppelObj { get; set; }
        [DataMember]
        public NominaDto Nomina { get; set; }
        [DataMemberAttribute()]
        public requestForPayment AddendaAmece { get; set; }
        [DataMemberAttribute()]
        public string AgregadoTitulo { get; set; }
        [DataMemberAttribute()]
        public string VoBoTitulo { get; set; }
        [DataMemberAttribute()]
        public string RecibiTitulo { get; set; }
        [DataMemberAttribute()]
        public string AutorizoTitulo { get; set; }
        //!!
        [DataMemberAttribute()]
        public List<ConceptosCartaPorte> ConceptosCartaPortes { get; set; }


        [DataMemberAttribute()]
        public string AgregadoNombre { get; set; }
        [DataMemberAttribute()]
        public string AgregadoPuesto { get; set; }
        [DataMemberAttribute()]
        public string AgregadoArea { get; set; }

        [DataMemberAttribute()]
        public string VoBoNombre { get; set; }
        [DataMemberAttribute()]
        public string VoBoPuesto { get; set; }
        [DataMemberAttribute()]
        public string VoBoArea { get; set; }

        [DataMemberAttribute()]
        public string RecibiNombre { get; set; }
        [DataMemberAttribute()]
        public string RecibiPuesto { get; set; }
        [DataMemberAttribute()]
        public string RecibiArea { get; set; }

        [DataMemberAttribute()]
        public string AutorizoNombre { get; set; }
        [DataMemberAttribute()]
        public string AutorizoPuesto { get; set; }
        [DataMemberAttribute()]
        public string AutorizoArea { get; set; }


        [DataMemberAttribute()]
        public string DonativoVersion { get; set; }
        [DataMemberAttribute()]
        public string DonativoAutorizacion { get; set; }
        [DataMemberAttribute()]
        public DateTime DonativoFechaAutorizacion { get; set; }
        [DataMemberAttribute()]
        public string DonativoLeyenda { get; set; }

        [DataMemberAttribute()]
        public string LucentOrdenCompra { get; set; }
        [DataMemberAttribute()]
        public string LucentRef { get; set; }


        [DataMemberAttribute()]
        public string TituloOtros { get; set; }

        [DataMemberAttribute()]
        public bool NotaCredito { get; set; }

        [DataMemberAttribute()]
        public string LeyendaSuperior { get; set; }

        [DataMemberAttribute()]
        public string FormaPago { get; set; }

        [DataMemberAttribute()]
        public string LeyendaInferior { get; set; }
       

        [DataMemberAttribute()]
        public DatosFacturaAduanera DatosAduanera { get; set; }

        public List<facturasdetalle> ConceptosAduanera { get; set; }

        [DataMemberAttribute()]
        public string Leyenda { get; set; }


        [DataMemberAttribute()]
        public TipoDocumento TipoDocumento;

        [DataMemberAttribute()]
        public string Regimen { get; set; }

        [DataMemberAttribute()]
        public string Metodo { get; set; }
        [DataMemberAttribute()]
        public string Cuenta { get; set; }
        [DataMemberAttribute()]
        public string MonedaS { get; set; }
        [DataMemberAttribute]
        public string LugarExpedicion { get; set; }

        [DataMemberAttribute]
        public string FolioFiscalOriginal { get; set; }

        [DataMemberAttribute]
        public string SerieFolioFiscalOriginal { get; set; }

        [DataMemberAttribute]
        public DateTime? FechaFolioFiscalOriginal { get; set; }

        [DataMemberAttribute]
        public decimal? MontoFolioFiscalOriginal { get; set; }

        [DataMemberAttribute]
        public string CondicionesPado { get; set; }

        [DataMemberAttribute]
        public Decimal PorcentajeRetenciónIva { get; set; }

        [DataMemberAttribute]
        public Decimal PorcentajeRetenciónIsr { get; set; }

        [DataMemberAttribute]
        public Decimal RetenciónIva { get; set; }

        [DataMemberAttribute]
        public Decimal RetenciónIsr { get; set; }

        // Addenda PEMEX -- SZ
        [DataMemberAttribute]
        public FacturasAddendaPemex FacturasAddendaPemex { get; set; }
        // Fin Addenda Pemex -- SZ




        public override string ToString()
        {
            return this.Folio + "|" + this.idcliente + "|" + this.Fecha.ToString("dd/MM/yyyy") + "|" + this.Importe;
        }


    }
}
