
using System.Xml.Serialization;

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://www.sat.gob.mx/IdeRecMinGast")]
[System.Xml.Serialization.XmlRootAttribute(Namespace="http://www.sat.gob.mx/IdeRecMinGast", IsNullable=false)]

public partial class IdentificacionDeRecurso {
    
    private IdentificacionDeRecursoDispersionDelRecurso[] dispersionDelRecursoField;
    
    private IdentificacionDeRecursoIdentificacionDelGasto[] identificacionDelGastoField;
    
    private string versionField;
    
    private string tipoOperacionField;
    
    private decimal montoEntregadoField;
    
    private System.DateTime fechaDepField;
    
    private decimal remanenteField;
    
    private bool remanenteFieldSpecified;
    
    private decimal reintegroRemanenteField;
    
    private bool reintegroRemanenteFieldSpecified;
    
    private System.DateTime reintegroRemanFechaField;
    
    private bool reintegroRemanFechaFieldSpecified;
    
    public IdentificacionDeRecurso() {
        this.versionField = "1.0";
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("DispersionDelRecurso")]
    public IdentificacionDeRecursoDispersionDelRecurso[] DispersionDelRecurso {
        get {
            return this.dispersionDelRecursoField;
        }
        set {
            this.dispersionDelRecursoField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("IdentificacionDelGasto")]
    public IdentificacionDeRecursoIdentificacionDelGasto[] IdentificacionDelGasto {
        get {
            return this.identificacionDelGastoField;
        }
        set {
            this.identificacionDelGastoField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string Version {
        get {
            return this.versionField;
        }
        set {
            this.versionField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string TipoOperacion {
        get {
            return this.tipoOperacionField;
        }
        set {
            this.tipoOperacionField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public decimal MontoEntregado {
        get {
            return this.montoEntregadoField;
        }
        set {
            this.montoEntregadoField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute(DataType="date")]
    public System.DateTime FechaDep {
        get {
            return this.fechaDepField;
        }
        set {
            this.fechaDepField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public decimal Remanente {
        get {
            return this.remanenteField;
        }
        set {
            this.remanenteField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlIgnoreAttribute()]
    public bool RemanenteSpecified {
        get {
            return this.remanenteFieldSpecified;
        }
        set {
            this.remanenteFieldSpecified = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public decimal ReintegroRemanente {
        get {
            return this.reintegroRemanenteField;
        }
        set {
            this.reintegroRemanenteField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlIgnoreAttribute()]
    public bool ReintegroRemanenteSpecified {
        get {
            return this.reintegroRemanenteFieldSpecified;
        }
        set {
            this.reintegroRemanenteFieldSpecified = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute(DataType="date")]
    public System.DateTime ReintegroRemanFecha {
        get {
            return this.reintegroRemanFechaField;
        }
        set {
            this.reintegroRemanFechaField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlIgnoreAttribute()]
    public bool ReintegroRemanFechaSpecified {
        get {
            return this.reintegroRemanFechaFieldSpecified;
        }
        set {
            this.reintegroRemanFechaFieldSpecified = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://www.sat.gob.mx/IdeRecMinGast")]
public partial class IdentificacionDeRecursoDispersionDelRecurso {
    
    private string numIdSolicitudField;
    
    private System.DateTime fechaDeIdentField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string NumIdSolicitud {
        get {
            return this.numIdSolicitudField;
        }
        set {
            this.numIdSolicitudField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute(DataType="date")]
    public System.DateTime FechaDeIdent {
        get {
            return this.fechaDeIdentField;
        }
        set {
            this.fechaDeIdentField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://www.sat.gob.mx/IdeRecMinGast")]
public partial class IdentificacionDeRecursoIdentificacionDelGasto {
    
    private string acuerdoGastoField;
    
    private string uUIDField;
    
    private string numFolioDocField;
    
    private System.DateTime fechaDeGastoField;
    
    private bool fechaDeGastoFieldSpecified;
    
    private string descripcionGastoField;
    
    private decimal importeGastoField;
    
    private bool importeGastoFieldSpecified;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string AcuerdoGasto {
        get {
            return this.acuerdoGastoField;
        }
        set {
            this.acuerdoGastoField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string UUID {
        get {
            return this.uUIDField;
        }
        set {
            this.uUIDField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string NumFolioDoc {
        get {
            return this.numFolioDocField;
        }
        set {
            this.numFolioDocField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute(DataType="date")]
    public System.DateTime FechaDeGasto {
        get {
            return this.fechaDeGastoField;
        }
        set {
            this.fechaDeGastoField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlIgnoreAttribute()]
    public bool FechaDeGastoSpecified {
        get {
            return this.fechaDeGastoFieldSpecified;
        }
        set {
            this.fechaDeGastoFieldSpecified = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string DescripcionGasto {
        get {
            return this.descripcionGastoField;
        }
        set {
            this.descripcionGastoField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public decimal ImporteGasto {
        get {
            return this.importeGastoField;
        }
        set {
            this.importeGastoField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlIgnoreAttribute()]
    public bool ImporteGastoSpecified {
        get {
            return this.importeGastoFieldSpecified;
        }
        set {
            this.importeGastoFieldSpecified = value;
        }
    }
}
