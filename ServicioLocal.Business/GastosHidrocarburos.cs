using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace ServicioLocal.Business.Hidro
{
    [GeneratedCode("xsd", "4.0.30319.1"), DesignerCategory("code"), DebuggerStepThrough, XmlRoot(Namespace = "http://www.sat.gob.mx/GastosHidrocarburos10", IsNullable = false), XmlType(AnonymousType = true, Namespace = "http://www.sat.gob.mx/GastosHidrocarburos10")]
    [Serializable]
    public class GastosHidrocarburos
    {
        [XmlAttribute("schemaLocation", Namespace = "http://www.w3.org/2001/XMLSchema-instance")]
        public string xsiSchemaLocation = "http://www.sat.gob.mx/GastosHidrocarburos10 http://www.sat.gob.mx/sitio_internet/cfd/GastosHidrocarburos10/GastosHidrocarburos10.xsd";

        private List<GastosHidrocarburosErogacion> erogacionField;

        private string versionField;

        private string numeroContratoField;

        private string areaContractualField;

        [XmlElement("Erogacion")]
        public List<GastosHidrocarburosErogacion> Erogacion
        {
            get
            {
                return this.erogacionField;
            }
            set
            {
                this.erogacionField = value;
            }
        }

        [XmlAttribute]
        public string Version
        {
            get
            {
                return this.versionField;
            }
            set
            {
                this.versionField = value;
            }
        }

        [XmlAttribute]
        public string NumeroContrato
        {
            get
            {
                return this.numeroContratoField;
            }
            set
            {
                this.numeroContratoField = value;
            }
        }

        [XmlAttribute]
        public string AreaContractual
        {
            get
            {
                return this.areaContractualField;
            }
            set
            {
                this.areaContractualField = value;
            }
        }

        public GastosHidrocarburos()
        {
            this.versionField = "1.0";
        }
    }

    [GeneratedCode("xsd", "4.0.30319.1"), DesignerCategory("code"), DebuggerStepThrough, XmlType(AnonymousType = true, Namespace = "http://www.sat.gob.mx/GastosHidrocarburos10")]
    [Serializable]
    public partial class GastosHidrocarburosErogacion
    {
        private List<GastosHidrocarburosErogacionDocumentoRelacionado> documentoRelacionadoField;

        private List<GastosHidrocarburosErogacionActividades> actividadesField;

        private List<GastosHidrocarburosErogacionCentroCostos> centroCostosField;

        private GastosHidrocarburosErogacionTipoErogacion tipoErogacionField;

        private decimal montocuErogacionField;

        private decimal porcentajeField;

        [XmlElement("DocumentoRelacionado")]
        public List<GastosHidrocarburosErogacionDocumentoRelacionado> DocumentoRelacionado
        {
            get
            {
                return this.documentoRelacionadoField;
            }
            set
            {
                this.documentoRelacionadoField = value;
            }
        }

        [XmlElement("Actividades")]
        public List<GastosHidrocarburosErogacionActividades> Actividades
        {
            get
            {
                return this.actividadesField;
            }
            set
            {
                this.actividadesField = value;
            }
        }

        [XmlElement("CentroCostos")]
        public List<GastosHidrocarburosErogacionCentroCostos> CentroCostos
        {
            get
            {
                return this.centroCostosField;
            }
            set
            {
                this.centroCostosField = value;
            }
        }

        [XmlElement("TipoErogacion")]
        public GastosHidrocarburosErogacionTipoErogacion TipoErogacion
        {
            get
            {
                return this.tipoErogacionField;
            }
            set
            {
                this.tipoErogacionField = value;
            }
        }

        [XmlAttribute]
        public decimal MontocuErogacion
        {
            get
            {
                return this.montocuErogacionField;
            }
            set
            {
                this.montocuErogacionField = value;
            }
        }

        [XmlAttribute]
        public decimal Porcentaje
        {
            get
            {
                return this.porcentajeField;
            }
            set
            {
                this.porcentajeField = value;
            }
        }
    }

    public enum GastosHidrocarburosErogacionTipoErogacion
    {
        Costo,
        Gasto,
        Inversión
    }
    public partial class GastosHidrocarburosErogacionCentroCostos
    {
        private List<GastosHidrocarburosErogacionCentroCostosYacimientos> yacimientosField;

        private string campoField;

        [XmlElement("Yacimientos")]
        public List<GastosHidrocarburosErogacionCentroCostosYacimientos> Yacimientos
        {
            get
            {
                return this.yacimientosField;
            }
            set
            {
                this.yacimientosField = value;
            }
        }

        [XmlAttribute]
        public string Campo
        {
            get
            {
                return this.campoField;
            }
            set
            {
                this.campoField = value;
            }
        }
    }

    public partial class GastosHidrocarburosErogacionActividades
    {
        private List<GastosHidrocarburosErogacionActividadesSubActividades> subActividadesField;

        private string actividadRelacionadaField;

        private bool actividadRelacionadaFieldSpecified;

        [XmlElement("SubActividades")]
        public List<GastosHidrocarburosErogacionActividadesSubActividades> SubActividades
        {
            get
            {
                return this.subActividadesField;
            }
            set
            {
                this.subActividadesField = value;
            }
        }

        [XmlAttribute]
        public string ActividadRelacionada
        {
            get
            {
                return this.actividadRelacionadaField;
            }
            set
            {
                this.actividadRelacionadaField = value;
            }
        }

        [XmlIgnore]
        public bool ActividadRelacionadaSpecified
        {
            get
            {
                return this.actividadRelacionadaFieldSpecified;
            }
            set
            {
                this.actividadRelacionadaFieldSpecified = value;
            }
        }

        public GastosHidrocarburosErogacionActividades()
        {
            this.subActividadesField = new List<GastosHidrocarburosErogacionActividadesSubActividades>();
        }
    }

    [GeneratedCode("xsd", "4.0.30319.1"), DesignerCategory("code"), DebuggerStepThrough, XmlType(AnonymousType = true, Namespace = "http://www.sat.gob.mx/GastosHidrocarburos10")]
    [Serializable]
    public partial class GastosHidrocarburosErogacionDocumentoRelacionado
    {
        private GastosHidrocarburosErogacionDocumentoRelacionadoOrigenErogacion origenErogacionField;

        private string folioFiscalVinculadoField;

        private string rFCProveedorField;

        private decimal montoTotalIVAField;

        private bool montoTotalIVAFieldSpecified;

        private decimal montoRetencionISRField;

        private bool montoRetencionISRFieldSpecified;

        private decimal montoRetencionIVAField;

        private bool montoRetencionIVAFieldSpecified;

        private decimal montoRetencionOtrosImpuestosField;

        private bool montoRetencionOtrosImpuestosFieldSpecified;

        private string numeroPedimentoVinculadoField;

        private ClavePedimento clavePedimentoVinculadoField;

        private bool clavePedimentoVinculadoFieldSpecified;

        private ClavePagoPedimento clavePagoPedimentoVinculadoField;

        private bool clavePagoPedimentoVinculadoFieldSpecified;

        private decimal montoIVAPedimentoField;

        private bool montoIVAPedimentoFieldSpecified;

        private decimal otrosImpuestosPagadosPedimentoField;

        private bool otrosImpuestosPagadosPedimentoFieldSpecified;

        private DateTime fechaFolioFiscalVinculadoField;

        private bool fechaFolioFiscalVinculadoFieldSpecified;

        private Meses mesField;

        private decimal montoTotalErogacionesField;

        [XmlAttribute]
        public GastosHidrocarburosErogacionDocumentoRelacionadoOrigenErogacion OrigenErogacion
        {
            get
            {
                return this.origenErogacionField;
            }
            set
            {
                this.origenErogacionField = value;
            }
        }

        [XmlAttribute]
        public string FolioFiscalVinculado
        {
            get
            {
                return this.folioFiscalVinculadoField;
            }
            set
            {
                this.folioFiscalVinculadoField = value;
            }
        }

        [XmlAttribute]
        public string RFCProveedor
        {
            get
            {
                return this.rFCProveedorField;
            }
            set
            {
                this.rFCProveedorField = value;
            }
        }

        [XmlAttribute]
        public decimal MontoTotalIVA
        {
            get
            {
                return this.montoTotalIVAField;
            }
            set
            {
                this.montoTotalIVAField = value;
            }
        }

        [XmlIgnore]
        public bool MontoTotalIVASpecified
        {
            get
            {
                return this.montoTotalIVAFieldSpecified;
            }
            set
            {
                this.montoTotalIVAFieldSpecified = value;
            }
        }

        [XmlAttribute]
        public decimal MontoRetencionISR
        {
            get
            {
                return this.montoRetencionISRField;
            }
            set
            {
                this.montoRetencionISRField = value;
            }
        }

        [XmlIgnore]
        public bool MontoRetencionISRSpecified
        {
            get
            {
                return this.montoRetencionISRFieldSpecified;
            }
            set
            {
                this.montoRetencionISRFieldSpecified = value;
            }
        }

        [XmlAttribute]
        public decimal MontoRetencionIVA
        {
            get
            {
                return this.montoRetencionIVAField;
            }
            set
            {
                this.montoRetencionIVAField = value;
            }
        }

        [XmlIgnore]
        public bool MontoRetencionIVASpecified
        {
            get
            {
                return this.montoRetencionIVAFieldSpecified;
            }
            set
            {
                this.montoRetencionIVAFieldSpecified = value;
            }
        }

        [XmlAttribute]
        public decimal MontoRetencionOtrosImpuestos
        {
            get
            {
                return this.montoRetencionOtrosImpuestosField;
            }
            set
            {
                this.montoRetencionOtrosImpuestosField = value;
            }
        }

        [XmlIgnore]
        public bool MontoRetencionOtrosImpuestosSpecified
        {
            get
            {
                return this.montoRetencionOtrosImpuestosFieldSpecified;
            }
            set
            {
                this.montoRetencionOtrosImpuestosFieldSpecified = value;
            }
        }

        [XmlAttribute]
        public string NumeroPedimentoVinculado
        {
            get
            {
                return this.numeroPedimentoVinculadoField;
            }
            set
            {
                this.numeroPedimentoVinculadoField = value;
            }
        }

        [XmlAttribute]
        public ClavePedimento ClavePedimentoVinculado
        {
            get
            {
                return this.clavePedimentoVinculadoField;
            }
            set
            {
                this.clavePedimentoVinculadoField = value;
            }
        }

        [XmlIgnore]
        public bool ClavePedimentoVinculadoSpecified
        {
            get
            {
                return this.clavePedimentoVinculadoFieldSpecified;
            }
            set
            {
                this.clavePedimentoVinculadoFieldSpecified = value;
            }
        }

        [XmlAttribute]
        public ClavePagoPedimento ClavePagoPedimentoVinculado
        {
            get
            {
                return this.clavePagoPedimentoVinculadoField;
            }
            set
            {
                this.clavePagoPedimentoVinculadoField = value;
            }
        }

        [XmlIgnore]
        public bool ClavePagoPedimentoVinculadoSpecified
        {
            get
            {
                return this.clavePagoPedimentoVinculadoFieldSpecified;
            }
            set
            {
                this.clavePagoPedimentoVinculadoFieldSpecified = value;
            }
        }

        [XmlAttribute]
        public decimal MontoIVAPedimento
        {
            get
            {
                return this.montoIVAPedimentoField;
            }
            set
            {
                this.montoIVAPedimentoField = value;
            }
        }

        [XmlIgnore]
        public bool MontoIVAPedimentoSpecified
        {
            get
            {
                return this.montoIVAPedimentoFieldSpecified;
            }
            set
            {
                this.montoIVAPedimentoFieldSpecified = value;
            }
        }

        [XmlAttribute]
        public decimal OtrosImpuestosPagadosPedimento
        {
            get
            {
                return this.otrosImpuestosPagadosPedimentoField;
            }
            set
            {
                this.otrosImpuestosPagadosPedimentoField = value;
            }
        }

        [XmlIgnore]
        public bool OtrosImpuestosPagadosPedimentoSpecified
        {
            get
            {
                return this.otrosImpuestosPagadosPedimentoFieldSpecified;
            }
            set
            {
                this.otrosImpuestosPagadosPedimentoFieldSpecified = value;
            }
        }

        [XmlAttribute]
        public DateTime FechaFolioFiscalVinculado
        {
            get
            {
                return this.fechaFolioFiscalVinculadoField;
            }
            set
            {
                this.fechaFolioFiscalVinculadoField = value;
            }
        }

        [XmlIgnore]
        public bool FechaFolioFiscalVinculadoSpecified
        {
            get
            {
                return this.fechaFolioFiscalVinculadoFieldSpecified;
            }
            set
            {
                this.fechaFolioFiscalVinculadoFieldSpecified = value;
            }
        }

        [XmlAttribute]
        public Meses Mes
        {
            get
            {
                return this.mesField;
            }
            set
            {
                this.mesField = value;
            }
        }

        [XmlAttribute]
        public decimal MontoTotalErogaciones
        {
            get
            {
                return this.montoTotalErogacionesField;
            }
            set
            {
                this.montoTotalErogacionesField = value;
            }
        }
    }
    public partial class GastosHidrocarburosErogacionCentroCostosYacimientos
    {
        private List<GastosHidrocarburosErogacionCentroCostosYacimientosPozos> pozosField;

        private string yacimientoField;

        [XmlElement("Pozos")]
        public List<GastosHidrocarburosErogacionCentroCostosYacimientosPozos> Pozos
        {
            get
            {
                return this.pozosField;
            }
            set
            {
                this.pozosField = value;
            }
        }

        [XmlAttribute]
        public string Yacimiento
        {
            get
            {
                return this.yacimientoField;
            }
            set
            {
                this.yacimientoField = value;
            }
        }
    }
    public partial class GastosHidrocarburosErogacionActividadesSubActividades
    {
        private List<GastosHidrocarburosErogacionActividadesSubActividadesTareas> tareasField;

        private SubActividad subActividadRelacionadaField;

        private bool subActividadRelacionadaFieldSpecified;

        [XmlElement("Tareas")]
        public List<GastosHidrocarburosErogacionActividadesSubActividadesTareas> Tareas
        {
            get
            {
                return this.tareasField;
            }
            set
            {
                this.tareasField = value;
            }
        }

        [XmlAttribute]
        public SubActividad SubActividadRelacionada
        {
            get
            {
                return this.subActividadRelacionadaField;
            }
            set
            {
                this.subActividadRelacionadaField = value;
            }
        }

        [XmlIgnore]
        public bool SubActividadRelacionadaSpecified
        {
            get
            {
                return this.subActividadRelacionadaFieldSpecified;
            }
            set
            {
                this.subActividadRelacionadaFieldSpecified = value;
            }
        }

        public GastosHidrocarburosErogacionActividadesSubActividades()
        {
            this.tareasField = new List<GastosHidrocarburosErogacionActividadesSubActividadesTareas>();
        }
    }
    public enum GastosHidrocarburosErogacionDocumentoRelacionadoOrigenErogacion
    {
        Nacional,
        Extranjero
    }
    public partial class GastosHidrocarburosErogacionCentroCostosYacimientosPozos
    {
        private string pozoField;

        [XmlAttribute]
        public string Pozo
        {
            get
            {
                return this.pozoField;
            }
            set
            {
                this.pozoField = value;
            }
        }
    }
    public partial class GastosHidrocarburosErogacionActividadesSubActividadesTareas
    {
        private string tareaRelacionadaField;

        private bool tareaRelacionadaFieldSpecified;

        [XmlAttribute]
        public string TareaRelacionada
        {
            get
            {
                return this.tareaRelacionadaField;
            }
            set
            {
                this.tareaRelacionadaField = value;
            }
        }

        [XmlIgnore]
        public bool TareaRelacionadaSpecified
        {
            get
            {
                return this.tareaRelacionadaFieldSpecified;
            }
            set
            {
                this.tareaRelacionadaFieldSpecified = value;
            }
        }
    }
    public enum SubActividad
    {
        [XmlEnum("001")]
        Item001,
        [XmlEnum("002")]
        Item002,
        [XmlEnum("003")]
        Item003,
        [XmlEnum("004")]
        Item004,
        [XmlEnum("005")]
        Item005,
        [XmlEnum("006")]
        Item006,
        [XmlEnum("007")]
        Item007,
        [XmlEnum("008")]
        Item008,
        [XmlEnum("009")]
        Item009,
        [XmlEnum("010")]
        Item010,
        [XmlEnum("011")]
        Item011,
        [XmlEnum("012")]
        Item012,
        [XmlEnum("013")]
        Item013,
        [XmlEnum("014")]
        Item014,
        [XmlEnum("015")]
        Item015,
        [XmlEnum("016")]
        Item016,
        [XmlEnum("017")]
        Item017,
        [XmlEnum("018")]
        Item018,
        [XmlEnum("019")]
        Item019,
        [XmlEnum("020")]
        Item020,
        [XmlEnum("021")]
        Item021,
        [XmlEnum("022")]
        Item022,
        [XmlEnum("023")]
        Item023,
        [XmlEnum("024")]
        Item024,
        [XmlEnum("025")]
        Item025,
        [XmlEnum("026")]
        Item026,
        [XmlEnum("027")]
        Item027,
        [XmlEnum("028")]
        Item028,
        [XmlEnum("029")]
        Item029,
        [XmlEnum("030")]
        Item030,
        [XmlEnum("031")]
        Item031,
        [XmlEnum("032")]
        Item032,
        [XmlEnum("033")]
        Item033,
        [XmlEnum("034")]
        Item034,
        [XmlEnum("035")]
        Item035,
        [XmlEnum("036")]
        Item036,
        [XmlEnum("037")]
        Item037,
        [XmlEnum("038")]
        Item038,
        [XmlEnum("039")]
        Item039
    }
    public enum Meses
    {
        [XmlEnum("01")]
        Item01,
        [XmlEnum("02")]
        Item02,
        [XmlEnum("03")]
        Item03,
        [XmlEnum("04")]
        Item04,
        [XmlEnum("05")]
        Item05,
        [XmlEnum("06")]
        Item06,
        [XmlEnum("07")]
        Item07,
        [XmlEnum("08")]
        Item08,
        [XmlEnum("09")]
        Item09,
        [XmlEnum("10")]
        Item10,
        [XmlEnum("11")]
        Item11,
        [XmlEnum("12")]
        Item12
    }
    public enum ClavePagoPedimento
    {
        [XmlEnum("00")]
        Item00,
        [XmlEnum("02")]
        Item02,
        [XmlEnum("04")]
        Item04,
        [XmlEnum("05")]
        Item05,
        [XmlEnum("06")]
        Item06,
        [XmlEnum("07")]
        Item07,
        [XmlEnum("08")]
        Item08,
        [XmlEnum("09")]
        Item09,
        [XmlEnum("10")]
        Item10,
        [XmlEnum("11")]
        Item11,
        [XmlEnum("12")]
        Item12,
        [XmlEnum("13")]
        Item13,
        [XmlEnum("14")]
        Item14,
        [XmlEnum("15")]
        Item15,
        [XmlEnum("16")]
        Item16,
        [XmlEnum("18")]
        Item18,
        [XmlEnum("19")]
        Item19,
        [XmlEnum("20")]
        Item20,
        [XmlEnum("21")]
        Item21,
        [XmlEnum("22")]
        Item22
    }
    public enum ClavePedimento
    {
        A1,
        A3,
        C1,
        D1,
        GC,
        K1,
        L1,
        P1,
        S2,
        T1,
        VF,
        VU,
        V1,
        V2,
        V5,
        V6,
        V7,
        V9,
        VD,
        AD,
        AJ,
        BA,
        BB,
        BC,
        BD,
        BE,
        BF,
        BH,
        BI,
        BM,
        BO,
        BP,
        BR,
        H1,
        H8,
        I1,
        F4,
        F5,
        IN,
        AF,
        RT,
        A4,
        E1,
        E2,
        G1,
        C3,
        K2,
        A5,
        E3,
        E4,
        G2,
        K3,
        F2,
        F3,
        V3,
        V4,
        F8,
        F9,
        G6,
        G7,
        V8,
        M1,
        M2,
        J3,
        G8,
        M3,
        M4,
        M5,
        J4,
        T3,
        T6,
        T7,
        T9,
        R1,
        CT
    }
    public enum Actividades
	{
		[XmlEnum("01")]
		Item01,
		[XmlEnum("02")]
		Item02,
		[XmlEnum("03")]
		Item03,
		[XmlEnum("04")]
		Item04,
		[XmlEnum("05")]
		Item05
	}
    public enum Tareas
    {
        [XmlEnum("0001")]
        Item0001,
        [XmlEnum("0002")]
        Item0002,
        [XmlEnum("0003")]
        Item0003,
        [XmlEnum("0004")]
        Item0004,
        [XmlEnum("0005")]
        Item0005,
        [XmlEnum("0006")]
        Item0006,
        [XmlEnum("0007")]
        Item0007,
        [XmlEnum("0008")]
        Item0008,
        [XmlEnum("0009")]
        Item0009,
        [XmlEnum("0010")]
        Item0010,
        [XmlEnum("0011")]
        Item0011,
        [XmlEnum("0012")]
        Item0012,
        [XmlEnum("0013")]
        Item0013,
        [XmlEnum("0014")]
        Item0014,
        [XmlEnum("0015")]
        Item0015,
        [XmlEnum("0016")]
        Item0016,
        [XmlEnum("0017")]
        Item0017,
        [XmlEnum("0018")]
        Item0018,
        [XmlEnum("0019")]
        Item0019,
        [XmlEnum("0020")]
        Item0020,
        [XmlEnum("0021")]
        Item0021,
        [XmlEnum("0022")]
        Item0022,
        [XmlEnum("0023")]
        Item0023,
        [XmlEnum("0024")]
        Item0024,
        [XmlEnum("0025")]
        Item0025,
        [XmlEnum("0026")]
        Item0026,
        [XmlEnum("0027")]
        Item0027,
        [XmlEnum("0028")]
        Item0028,
        [XmlEnum("0029")]
        Item0029,
        [XmlEnum("0030")]
        Item0030,
        [XmlEnum("0031")]
        Item0031,
        [XmlEnum("0032")]
        Item0032,
        [XmlEnum("0033")]
        Item0033,
        [XmlEnum("0034")]
        Item0034,
        [XmlEnum("0035")]
        Item0035,
        [XmlEnum("0036")]
        Item0036,
        [XmlEnum("0037")]
        Item0037,
        [XmlEnum("0038")]
        Item0038,
        [XmlEnum("0039")]
        Item0039,
        [XmlEnum("0040")]
        Item0040,
        [XmlEnum("0041")]
        Item0041,
        [XmlEnum("0042")]
        Item0042,
        [XmlEnum("0043")]
        Item0043,
        [XmlEnum("0044")]
        Item0044,
        [XmlEnum("0045")]
        Item0045,
        [XmlEnum("0046")]
        Item0046,
        [XmlEnum("0047")]
        Item0047,
        [XmlEnum("0048")]
        Item0048,
        [XmlEnum("0049")]
        Item0049,
        [XmlEnum("0050")]
        Item0050,
        [XmlEnum("0051")]
        Item0051,
        [XmlEnum("0052")]
        Item0052,
        [XmlEnum("0053")]
        Item0053,
        [XmlEnum("0054")]
        Item0054,
        [XmlEnum("0055")]
        Item0055,
        [XmlEnum("0056")]
        Item0056,
        [XmlEnum("0057")]
        Item0057,
        [XmlEnum("0058")]
        Item0058,
        [XmlEnum("0059")]
        Item0059,
        [XmlEnum("0060")]
        Item0060,
        [XmlEnum("0061")]
        Item0061,
        [XmlEnum("0062")]
        Item0062,
        [XmlEnum("0063")]
        Item0063,
        [XmlEnum("0064")]
        Item0064,
        [XmlEnum("0065")]
        Item0065,
        [XmlEnum("0066")]
        Item0066,
        [XmlEnum("0067")]
        Item0067,
        [XmlEnum("0068")]
        Item0068,
        [XmlEnum("0069")]
        Item0069,
        [XmlEnum("0070")]
        Item0070,
        [XmlEnum("0071")]
        Item0071,
        [XmlEnum("0072")]
        Item0072,
        [XmlEnum("0073")]
        Item0073,
        [XmlEnum("0074")]
        Item0074,
        [XmlEnum("0075")]
        Item0075,
        [XmlEnum("0076")]
        Item0076,
        [XmlEnum("0077")]
        Item0077,
        [XmlEnum("0078")]
        Item0078,
        [XmlEnum("0079")]
        Item0079,
        [XmlEnum("0080")]
        Item0080,
        [XmlEnum("0081")]
        Item0081,
        [XmlEnum("0082")]
        Item0082,
        [XmlEnum("0083")]
        Item0083,
        [XmlEnum("0084")]
        Item0084,
        [XmlEnum("0085")]
        Item0085,
        [XmlEnum("0086")]
        Item0086,
        [XmlEnum("0087")]
        Item0087,
        [XmlEnum("0088")]
        Item0088,
        [XmlEnum("0089")]
        Item0089,
        [XmlEnum("0090")]
        Item0090,
        [XmlEnum("0091")]
        Item0091,
        [XmlEnum("0092")]
        Item0092,
        [XmlEnum("0093")]
        Item0093,
        [XmlEnum("0094")]
        Item0094,
        [XmlEnum("0095")]
        Item0095,
        [XmlEnum("0096")]
        Item0096,
        [XmlEnum("0097")]
        Item0097,
        [XmlEnum("0098")]
        Item0098,
        [XmlEnum("0099")]
        Item0099,
        [XmlEnum("0100")]
        Item0100,
        [XmlEnum("0101")]
        Item0101,
        [XmlEnum("0102")]
        Item0102,
        [XmlEnum("0103")]
        Item0103,
        [XmlEnum("0104")]
        Item0104,
        [XmlEnum("0105")]
        Item0105,
        [XmlEnum("0106")]
        Item0106,
        [XmlEnum("0107")]
        Item0107,
        [XmlEnum("0108")]
        Item0108,
        [XmlEnum("0109")]
        Item0109,
        [XmlEnum("0110")]
        Item0110,
        [XmlEnum("0111")]
        Item0111,
        [XmlEnum("0112")]
        Item0112,
        [XmlEnum("0113")]
        Item0113,
        [XmlEnum("0114")]
        Item0114,
        [XmlEnum("0115")]
        Item0115,
        [XmlEnum("0116")]
        Item0116,
        [XmlEnum("0117")]
        Item0117,
        [XmlEnum("0118")]
        Item0118,
        [XmlEnum("0119")]
        Item0119,
        [XmlEnum("0120")]
        Item0120,
        [XmlEnum("0121")]
        Item0121,
        [XmlEnum("0122")]
        Item0122,
        [XmlEnum("0123")]
        Item0123,
        [XmlEnum("0124")]
        Item0124,
        [XmlEnum("0125")]
        Item0125,
        [XmlEnum("0126")]
        Item0126,
        [XmlEnum("0127")]
        Item0127,
        [XmlEnum("0128")]
        Item0128,
        [XmlEnum("0129")]
        Item0129,
        [XmlEnum("0130")]
        Item0130,
        [XmlEnum("0131")]
        Item0131,
        [XmlEnum("0132")]
        Item0132,
        [XmlEnum("0133")]
        Item0133,
        [XmlEnum("0134")]
        Item0134,
        [XmlEnum("0135")]
        Item0135,
        [XmlEnum("0136")]
        Item0136,
        [XmlEnum("0137")]
        Item0137,
        [XmlEnum("0138")]
        Item0138,
        [XmlEnum("0139")]
        Item0139,
        [XmlEnum("0140")]
        Item0140,
        [XmlEnum("0141")]
        Item0141,
        [XmlEnum("0142")]
        Item0142,
        [XmlEnum("0143")]
        Item0143,
        [XmlEnum("0144")]
        Item0144,
        [XmlEnum("0145")]
        Item0145,
        [XmlEnum("0146")]
        Item0146,
        [XmlEnum("0147")]
        Item0147,
        [XmlEnum("0148")]
        Item0148,
        [XmlEnum("0149")]
        Item0149,
        [XmlEnum("0150")]
        Item0150,
        [XmlEnum("0151")]
        Item0151,
        [XmlEnum("0152")]
        Item0152,
        [XmlEnum("0153")]
        Item0153,
        [XmlEnum("0154")]
        Item0154,
        [XmlEnum("0155")]
        Item0155
    }
}
