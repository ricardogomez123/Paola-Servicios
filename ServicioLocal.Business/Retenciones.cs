namespace ServicioLocal.Business.ComplementoRetencion
{
    using System.Xml.Serialization;

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.sat.gob.mx/esquemas/retencionpago/1")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.sat.gob.mx/esquemas/retencionpago/1", IsNullable = false)]
    public partial class Retenciones
    {

        private RetencionesEmisor emisorField;

        private RetencionesReceptor receptorField;

        private RetencionesPeriodo periodoField;

        private RetencionesTotales totalesField;

        private RetencionesComplemento complementoField;

        private RetencionesAddenda addendaField;

        private string versionField;

        private string folioIntField;

        private string selloField;

        private string numCertField;

        private string certField;

        private System.DateTime fechaExpField;

        private c_Retenciones cveRetencField;

        private string descRetencField;

        public Retenciones()
        {
            this.versionField = "1.0";
        }

        /// <remarks/>
        public RetencionesEmisor Emisor
        {
            get
            {
                return this.emisorField;
            }
            set
            {
                this.emisorField = value;
            }
        }

        /// <remarks/>
        public RetencionesReceptor Receptor
        {
            get
            {
                return this.receptorField;
            }
            set
            {
                this.receptorField = value;
            }
        }

        /// <remarks/>
        public RetencionesPeriodo Periodo
        {
            get
            {
                return this.periodoField;
            }
            set
            {
                this.periodoField = value;
            }
        }

        /// <remarks/>
        public RetencionesTotales Totales
        {
            get
            {
                return this.totalesField;
            }
            set
            {
                this.totalesField = value;
            }
        }

        /// <remarks/>
        public RetencionesComplemento Complemento
        {
            get
            {
                return this.complementoField;
            }
            set
            {
                this.complementoField = value;
            }
        }

        /// <remarks/>
        public RetencionesAddenda Addenda
        {
            get
            {
                return this.addendaField;
            }
            set
            {
                this.addendaField = value;
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
        public string FolioInt
        {
            get
            {
                return this.folioIntField;
            }
            set
            {
                this.folioIntField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Sello
        {
            get
            {
                return this.selloField;
            }
            set
            {
                this.selloField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string NumCert
        {
            get
            {
                return this.numCertField;
            }
            set
            {
                this.numCertField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Cert
        {
            get
            {
                return this.certField;
            }
            set
            {
                this.certField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public System.DateTime FechaExp
        {
            get
            {
                return this.fechaExpField;
            }
            set
            {
                this.fechaExpField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public c_Retenciones CveRetenc
        {
            get
            {
                return this.cveRetencField;
            }
            set
            {
                this.cveRetencField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string DescRetenc
        {
            get
            {
                return this.descRetencField;
            }
            set
            {
                this.descRetencField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.sat.gob.mx/esquemas/retencionpago/1")]
    public partial class RetencionesEmisor
    {

        private string rFCEmisorField;

        private string nomDenRazSocEField;

        private string cURPEField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string RFCEmisor
        {
            get
            {
                return this.rFCEmisorField;
            }
            set
            {
                this.rFCEmisorField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string NomDenRazSocE
        {
            get
            {
                return this.nomDenRazSocEField;
            }
            set
            {
                this.nomDenRazSocEField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string CURPE
        {
            get
            {
                return this.cURPEField;
            }
            set
            {
                this.cURPEField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.sat.gob.mx/esquemas/retencionpago/1")]
    public partial class RetencionesReceptor
    {

        private object itemField;

        private RetencionesReceptorNacionalidad nacionalidadField;
        
        private RetencionesReceptorNacional nacionalField;
        private RetencionesReceptorExtranjero ExtranjeroField;
            [System.Xml.Serialization.XmlIgnore()]
        public RetencionesReceptorNacional Nacional
        {
            get
            {
                return this.nacionalField;
            }
            set
            {
                this.nacionalField = value;
            }
        }
            [System.Xml.Serialization.XmlIgnore()]
        public RetencionesReceptorExtranjero Extranjero
        {
            get
            {
                return this.ExtranjeroField;
            }
            set
            {
                this.ExtranjeroField = value;
            }
        }
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Extranjero", typeof(RetencionesReceptorExtranjero))]
        [System.Xml.Serialization.XmlElementAttribute("Nacional", typeof(RetencionesReceptorNacional))]
        public object Item
        {
            get
            {
                return this.itemField;
            }
            set
            {
                this.itemField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public RetencionesReceptorNacionalidad Nacionalidad
        {
            get
            {
                return this.nacionalidadField;
            }
            set
            {
                this.nacionalidadField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.sat.gob.mx/esquemas/retencionpago/1")]
    public partial class RetencionesReceptorExtranjero
    {

        private string numRegIdTribField;

        private string nomDenRazSocRField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string NumRegIdTrib
        {
            get
            {
                return this.numRegIdTribField;
            }
            set
            {
                this.numRegIdTribField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string NomDenRazSocR
        {
            get
            {
                return this.nomDenRazSocRField;
            }
            set
            {
                this.nomDenRazSocRField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.sat.gob.mx/esquemas/retencionpago/1")]
    public partial class RetencionesReceptorNacional
    {

        private string rFCRecepField;

        private string nomDenRazSocRField;

        private string cURPRField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string RFCRecep
        {
            get
            {
                return this.rFCRecepField;
            }
            set
            {
                this.rFCRecepField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string NomDenRazSocR
        {
            get
            {
                return this.nomDenRazSocRField;
            }
            set
            {
                this.nomDenRazSocRField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string CURPR
        {
            get
            {
                return this.cURPRField;
            }
            set
            {
                this.cURPRField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.sat.gob.mx/esquemas/retencionpago/1")]
    public enum RetencionesReceptorNacionalidad
    {

        /// <remarks/>
        Nacional,

        /// <remarks/>
        Extranjero,
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.sat.gob.mx/esquemas/retencionpago/1")]
    public partial class RetencionesPeriodo
    {

        private int mesIniField;

        private int mesFinField;

        private int ejercField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public int MesIni
        {
            get
            {
                return this.mesIniField;
            }
            set
            {
                this.mesIniField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public int MesFin
        {
            get
            {
                return this.mesFinField;
            }
            set
            {
                this.mesFinField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public int Ejerc
        {
            get
            {
                return this.ejercField;
            }
            set
            {
                this.ejercField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.sat.gob.mx/esquemas/retencionpago/1")]
    public partial class RetencionesTotales
    {

        private RetencionesTotalesImpRetenidos[] impRetenidosField;

        private decimal montoTotOperacionField;

        private decimal montoTotGravField;

        private decimal montoTotExentField;

        private decimal montoTotRetField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("ImpRetenidos")]
        public RetencionesTotalesImpRetenidos[] ImpRetenidos
        {
            get
            {
                return this.impRetenidosField;
            }
            set
            {
                this.impRetenidosField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal montoTotOperacion
        {
            get
            {
                return this.montoTotOperacionField;
            }
            set
            {
                this.montoTotOperacionField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal montoTotGrav
        {
            get
            {
                return this.montoTotGravField;
            }
            set
            {
                this.montoTotGravField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal montoTotExent
        {
            get
            {
                return this.montoTotExentField;
            }
            set
            {
                this.montoTotExentField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal montoTotRet
        {
            get
            {
                return this.montoTotRetField;
            }
            set
            {
                this.montoTotRetField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.sat.gob.mx/esquemas/retencionpago/1")]
    public partial class RetencionesTotalesImpRetenidos
    {

        private decimal baseRetField;

        private bool baseRetFieldSpecified;

        private RetencionesTotalesImpRetenidosImpuesto impuestoField;

        private bool impuestoFieldSpecified;

        private decimal montoRetField;

        private RetencionesTotalesImpRetenidosTipoPagoRet tipoPagoRetField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal BaseRet
        {
            get
            {
                return this.baseRetField;
            }
            set
            {
                this.baseRetField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool BaseRetSpecified
        {
            get
            {
                return this.baseRetFieldSpecified;
            }
            set
            {
                this.baseRetFieldSpecified = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public RetencionesTotalesImpRetenidosImpuesto Impuesto
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
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool ImpuestoSpecified
        {
            get
            {
                return this.impuestoFieldSpecified;
            }
            set
            {
                this.impuestoFieldSpecified = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal montoRet
        {
            get
            {
                return this.montoRetField;
            }
            set
            {
                this.montoRetField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public RetencionesTotalesImpRetenidosTipoPagoRet TipoPagoRet
        {
            get
            {
                return this.tipoPagoRetField;
            }
            set
            {
                this.tipoPagoRetField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.sat.gob.mx/esquemas/retencionpago/1/catalogos")]
    public enum RetencionesTotalesImpRetenidosImpuesto
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
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.sat.gob.mx/esquemas/retencionpago/1")]
    public enum RetencionesTotalesImpRetenidosTipoPagoRet
    {

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("Pago definitivo")]
        Pagodefinitivo,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("Pago provisional")]
        Pagoprovisional,
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.sat.gob.mx/esquemas/retencionpago/1")]
    public partial class RetencionesComplemento
    {

        private System.Xml.XmlElement[] anyField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAnyElementAttribute()]
        public System.Xml.XmlElement[] Any
        {
            get
            {
                return this.anyField;
            }
            set
            {
                this.anyField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.sat.gob.mx/esquemas/retencionpago/1")]
    public partial class RetencionesAddenda
    {

        private System.Xml.XmlElement[] anyField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAnyElementAttribute()]
        public System.Xml.XmlElement[] Any
        {
            get
            {
                return this.anyField;
            }
            set
            {
                this.anyField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.sat.gob.mx/esquemas/retencionpago/1/catalogos")]
    public enum c_Retenciones
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
    }
}