
using System.Xml.Serialization;
using System.Xml.Schema;

// 
// Este código fuente fue generado automáticamente por xsd, Versión=4.0.30319.1.
// 

namespace CertificadorWs.Business.Retenciones
{
    /// <comentarios/>
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

        private string fechaExpField;

        private c_Retenciones cveRetencField;

        private string descRetencField;

        [XmlAttribute("schemaLocation", Namespace = XmlSchema.InstanceNamespace
            )]
        public string schemaLocation = "http://www.sat.gob.mx/esquemas/retencionpago/1 http://www.sat.gob.mx/esquemas/retencionpago/1/retencionpagov1.xsd";


        public Retenciones()
        {
            this.versionField = "1.0";
        }

        /// <comentarios/>
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

        /// <comentarios/>
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

        /// <comentarios/>
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

        /// <comentarios/>
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

        /// <comentarios/>
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

        /// <comentarios/>
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

        /// <comentarios/>
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

        /// <comentarios/>
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

        /// <comentarios/>
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

        /// <comentarios/>
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

        /// <comentarios/>
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

        /// <comentarios/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string FechaExp
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

        /// <comentarios/>
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

        /// <comentarios/>
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

    /// <comentarios/>
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

        /// <comentarios/>
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

        /// <comentarios/>
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

        /// <comentarios/>
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

    /// <comentarios/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.sat.gob.mx/esquemas/retencionpago/1")]
    public partial class RetencionesReceptor
    {

        private object itemField;

        private RetencionesReceptorNacionalidad nacionalidadField;

        /// <comentarios/>
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

        /// <comentarios/>
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

    /// <comentarios/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.sat.gob.mx/esquemas/retencionpago/1")]
    public partial class RetencionesReceptorExtranjero
    {

        private string numRegIdTribField;

        private string nomDenRazSocRField;

        /// <comentarios/>
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

        /// <comentarios/>
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

    /// <comentarios/>
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

        /// <comentarios/>
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

        /// <comentarios/>
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

        /// <comentarios/>
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

    /// <comentarios/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.sat.gob.mx/esquemas/retencionpago/1")]
    public enum RetencionesReceptorNacionalidad
    {

        /// <comentarios/>
        Nacional,

        /// <comentarios/>
        Extranjero,
    }

    /// <comentarios/>
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

        /// <comentarios/>
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

        /// <comentarios/>
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

        /// <comentarios/>
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

    /// <comentarios/>
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

        /// <comentarios/>
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

        /// <comentarios/>
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

        /// <comentarios/>
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

        /// <comentarios/>
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

        /// <comentarios/>
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

    /// <comentarios/>
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

        /// <comentarios/>
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

        /// <comentarios/>
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

        /// <comentarios/>
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

        /// <comentarios/>
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

        /// <comentarios/>
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

        /// <comentarios/>
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

    /// <comentarios/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.sat.gob.mx/esquemas/retencionpago/1/catalogos")]
    public enum RetencionesTotalesImpRetenidosImpuesto
    {

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("01")]
        Item01,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("02")]
        Item02,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("03")]
        Item03,
    }

    /// <comentarios/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.sat.gob.mx/esquemas/retencionpago/1")]
    public enum RetencionesTotalesImpRetenidosTipoPagoRet
    {

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("Pago definitivo")]
        Pagodefinitivo,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("Pago provisional")]
        Pagoprovisional,
    }

    /// <comentarios/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.sat.gob.mx/esquemas/retencionpago/1")]
    public partial class RetencionesComplemento
    {

        private System.Xml.XmlElement[] anyField;

        /// <comentarios/>
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

    /// <comentarios/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.sat.gob.mx/esquemas/retencionpago/1")]
    public partial class RetencionesAddenda
    {

        private System.Xml.XmlElement[] anyField;

        /// <comentarios/>
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

    /// <comentarios/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.sat.gob.mx/esquemas/retencionpago/1/catalogos")]
    public enum c_Retenciones
    {

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("01")]
        Item01,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("02")]
        Item02,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("03")]
        Item03,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("04")]
        Item04,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("05")]
        Item05,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("06")]
        Item06,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("07")]
        Item07,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("08")]
        Item08,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("09")]
        Item09,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("10")]
        Item10,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("11")]
        Item11,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("12")]
        Item12,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("13")]
        Item13,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("14")]
        Item14,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("15")]
        Item15,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("16")]
        Item16,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("17")]
        Item17,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("18")]
        Item18,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("19")]
        Item19,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("20")]
        Item20,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("21")]
        Item21,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("22")]
        Item22,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("23")]
        Item23,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("24")]
        Item24,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("25")]
        Item25,
    }
}