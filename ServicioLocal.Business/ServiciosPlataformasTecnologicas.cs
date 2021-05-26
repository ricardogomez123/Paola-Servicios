namespace ServicioLocal.Business
{

    using System.Xml.Serialization;


    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.sat.gob.mx/esquemas/retencionpago/1/PlataformasTecnologicas10")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.sat.gob.mx/esquemas/retencionpago/1/PlataformasTecnologicas10", IsNullable = false)]
    public partial class ServiciosPlataformasTecnologicas
    {

        private ServiciosPlataformasTecnologicasDetallesDelServicio[] serviciosField;

        private string versionField;

        private string periodicidadField;

        private string numServField;

        private decimal monTotServSIVAField;

        private decimal totalIVATrasladadoField;

        private decimal totalIVARetenidoField;

        private decimal totalISRRetenidoField;

        private decimal difIVAEntregadoPrestServField;

        private decimal monTotalporUsoPlataformaField;

        private decimal monTotalContribucionGubernamentalField;

        private bool monTotalContribucionGubernamentalFieldSpecified;

        public ServiciosPlataformasTecnologicas()
        {
            this.versionField = "1.0";
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("DetallesDelServicio", IsNullable = false)]
        public ServiciosPlataformasTecnologicasDetallesDelServicio[] Servicios
        {
            get
            {
                return this.serviciosField;
            }
            set
            {
                this.serviciosField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
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

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Periodicidad
        {
            get
            {
                return this.periodicidadField;
            }
            set
            {
                this.periodicidadField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType = "integer")]
        public string NumServ
        {
            get
            {
                return this.numServField;
            }
            set
            {
                this.numServField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal MonTotServSIVA
        {
            get
            {
                return this.monTotServSIVAField;
            }
            set
            {
                this.monTotServSIVAField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal TotalIVATrasladado
        {
            get
            {
                return this.totalIVATrasladadoField;
            }
            set
            {
                this.totalIVATrasladadoField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal TotalIVARetenido
        {
            get
            {
                return this.totalIVARetenidoField;
            }
            set
            {
                this.totalIVARetenidoField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal TotalISRRetenido
        {
            get
            {
                return this.totalISRRetenidoField;
            }
            set
            {
                this.totalISRRetenidoField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal DifIVAEntregadoPrestServ
        {
            get
            {
                return this.difIVAEntregadoPrestServField;
            }
            set
            {
                this.difIVAEntregadoPrestServField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal MonTotalporUsoPlataforma
        {
            get
            {
                return this.monTotalporUsoPlataformaField;
            }
            set
            {
                this.monTotalporUsoPlataformaField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal MonTotalContribucionGubernamental
        {
            get
            {
                return this.monTotalContribucionGubernamentalField;
            }
            set
            {
                this.monTotalContribucionGubernamentalField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool MonTotalContribucionGubernamentalSpecified
        {
            get
            {
                return this.monTotalContribucionGubernamentalFieldSpecified;
            }
            set
            {
                this.monTotalContribucionGubernamentalFieldSpecified = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.sat.gob.mx/esquemas/retencionpago/1/PlataformasTecnologicas10")]
    public partial class ServiciosPlataformasTecnologicasDetallesDelServicio
    {

        private ServiciosPlataformasTecnologicasDetallesDelServicioImpuestosTrasladadosdelServicio impuestosTrasladadosdelServicioField;

        private ServiciosPlataformasTecnologicasDetallesDelServicioContribucionGubernamental contribucionGubernamentalField;

        private ServiciosPlataformasTecnologicasDetallesDelServicioComisionDelServicio comisionDelServicioField;

        private string formaPagoServField;

        private string tipoDeServField;

        private string subTipServField;

        private bool subTipServFieldSpecified;

        private string rFCTerceroAutorizadoField;

        private System.DateTime fechaServField;

        private decimal precioServSinIVAField;

        /// <remarks/>
        public ServiciosPlataformasTecnologicasDetallesDelServicioImpuestosTrasladadosdelServicio ImpuestosTrasladadosdelServicio
        {
            get
            {
                return this.impuestosTrasladadosdelServicioField;
            }
            set
            {
                this.impuestosTrasladadosdelServicioField = value;
            }
        }

        /// <remarks/>
        public ServiciosPlataformasTecnologicasDetallesDelServicioContribucionGubernamental ContribucionGubernamental
        {
            get
            {
                return this.contribucionGubernamentalField;
            }
            set
            {
                this.contribucionGubernamentalField = value;
            }
        }

        /// <remarks/>
        public ServiciosPlataformasTecnologicasDetallesDelServicioComisionDelServicio ComisionDelServicio
        {
            get
            {
                return this.comisionDelServicioField;
            }
            set
            {
                this.comisionDelServicioField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string FormaPagoServ
        {
            get
            {
                return this.formaPagoServField;
            }
            set
            {
                this.formaPagoServField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string TipoDeServ
        {
            get
            {
                return this.tipoDeServField;
            }
            set
            {
                this.tipoDeServField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string SubTipServ
        {
            get
            {
                return this.subTipServField;
            }
            set
            {
                this.subTipServField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool SubTipServSpecified
        {
            get
            {
                return this.subTipServFieldSpecified;
            }
            set
            {
                this.subTipServFieldSpecified = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string RFCTerceroAutorizado
        {
            get
            {
                return this.rFCTerceroAutorizadoField;
            }
            set
            {
                this.rFCTerceroAutorizadoField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType = "date")]
        public System.DateTime FechaServ
        {
            get
            {
                return this.fechaServField;
            }
            set
            {
                this.fechaServField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal PrecioServSinIVA
        {
            get
            {
                return this.precioServSinIVAField;
            }
            set
            {
                this.precioServSinIVAField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.sat.gob.mx/esquemas/retencionpago/1/PlataformasTecnologicas10")]
    public partial class ServiciosPlataformasTecnologicasDetallesDelServicioImpuestosTrasladadosdelServicio
    {

        private decimal baseField;

        private c_TipoImpuesto impuestoField;

        private string tipoFactorField;

        private decimal tasaCuotaField;

        private decimal importeField;

        public ServiciosPlataformasTecnologicasDetallesDelServicioImpuestosTrasladadosdelServicio()
        {
            this.tipoFactorField = "Tasa";
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal Base
        {
            get
            {
                return this.baseField;
            }
            set
            {
                this.baseField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public c_TipoImpuesto Impuesto
        {
            get
            {
                return this.impuestoField;
            }
            set
            {
                this.impuestoField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string TipoFactor
        {
            get
            {
                return this.tipoFactorField;
            }
            set
            {
                this.tipoFactorField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal TasaCuota
        {
            get
            {
                return this.tasaCuotaField;
            }
            set
            {
                this.tasaCuotaField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal Importe
        {
            get
            {
                return this.importeField;
            }
            set
            {
                this.importeField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.sat.gob.mx/esquemas/retencionpago/1/catalogos")]
    public enum c_TipoImpuesto
    {

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("01")]
        Item01,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("02")]
        Item02,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("03")]
        Item03,
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.sat.gob.mx/esquemas/retencionpago/1/PlataformasTecnologicas10")]
    public partial class ServiciosPlataformasTecnologicasDetallesDelServicioContribucionGubernamental
    {

        private decimal impContribField;

        private string entidadDondePagaLaContribucionField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal ImpContrib
        {
            get
            {
                return this.impContribField;
            }
            set
            {
                this.impContribField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string EntidadDondePagaLaContribucion
        {
            get
            {
                return this.entidadDondePagaLaContribucionField;
            }
            set
            {
                this.entidadDondePagaLaContribucionField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.sat.gob.mx/esquemas/retencionpago/1/catalogos")]
    public enum c_EntidadesFederativas
    {

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("01")]
        Item01,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("02")]
        Item02,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("03")]
        Item03,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("04")]
        Item04,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("05")]
        Item05,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("06")]
        Item06,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("07")]
        Item07,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("08")]
        Item08,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("09")]
        Item09,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("10")]
        Item10,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("11")]
        Item11,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("12")]
        Item12,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("13")]
        Item13,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("14")]
        Item14,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("15")]
        Item15,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("16")]
        Item16,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("17")]
        Item17,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("18")]
        Item18,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("19")]
        Item19,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("20")]
        Item20,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("21")]
        Item21,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("22")]
        Item22,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("23")]
        Item23,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("24")]
        Item24,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("25")]
        Item25,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("26")]
        Item26,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("27")]
        Item27,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("28")]
        Item28,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("29")]
        Item29,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("30")]
        Item30,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("31")]
        Item31,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("32")]
        Item32,
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.sat.gob.mx/esquemas/retencionpago/1/PlataformasTecnologicas10")]
    public partial class ServiciosPlataformasTecnologicasDetallesDelServicioComisionDelServicio
    {

        private decimal baseField;

        private bool baseFieldSpecified;

        private decimal porcentajeField;

        private bool porcentajeFieldSpecified;

        private decimal importeField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal Base
        {
            get
            {
                return this.baseField;
            }
            set
            {
                this.baseField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool BaseSpecified
        {
            get
            {
                return this.baseFieldSpecified;
            }
            set
            {
                this.baseFieldSpecified = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
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

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool PorcentajeSpecified
        {
            get
            {
                return this.porcentajeFieldSpecified;
            }
            set
            {
                this.porcentajeFieldSpecified = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal Importe
        {
            get
            {
                return this.importeField;
            }
            set
            {
                this.importeField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.sat.gob.mx/esquemas/retencionpago/1/PlataformasTecnologicas10/catalogo" +
        "s")]
    public enum c_FormaPagoServ
    {

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("01")]
        Item01,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("02")]
        Item02,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("03")]
        Item03,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("04")]
        Item04,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("05")]
        Item05,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("06")]
        Item06,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("07")]
        Item07,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("08")]
        Item08,
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.sat.gob.mx/esquemas/retencionpago/1/PlataformasTecnologicas10/catalogo" +
        "s")]
    public enum c_TipoDeServ
    {

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("01")]
        Item01,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("02")]
        Item02,
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.sat.gob.mx/esquemas/retencionpago/1/PlataformasTecnologicas10/catalogo" +
        "s")]
    public enum c_SubTipoServ
    {

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("01")]
        Item01,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("02")]
        Item02,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("03")]
        Item03,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("04")]
        Item04,
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.sat.gob.mx/esquemas/retencionpago/1/catalogos")]
    public enum c_Periodicidad
    {

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("01")]
        Item01,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("02")]
        Item02,
    }
}