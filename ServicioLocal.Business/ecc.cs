
/*
using System.Xml.Serialization;

/// <comentarios/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://www.sat.gob.mx/ecc")]
[System.Xml.Serialization.XmlRootAttribute(Namespace="http://www.sat.gob.mx/ecc", IsNullable=false)]
public partial class EstadoDeCuentaCombustible {
    
    private EstadoDeCuentaCombustibleConceptoEstadoDeCuentaCombustible[] conceptosField;
    
    private string tipoOperacionField;
    
    private string numeroDeCuentaField;
    
    private decimal subTotalField;
    
    private bool subTotalFieldSpecified;
    
    private decimal totalField;
    
    public EstadoDeCuentaCombustible() {
        this.tipoOperacionField = "Tarjeta";
    }
    
    /// <comentarios/>
    [System.Xml.Serialization.XmlArrayItemAttribute("ConceptoEstadoDeCuentaCombustible", IsNullable=false)]
    public EstadoDeCuentaCombustibleConceptoEstadoDeCuentaCombustible[] Conceptos {
        get {
            return this.conceptosField;
        }
        set {
            this.conceptosField = value;
        }
    }
    
    /// <comentarios/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string tipoOperacion {
        get {
            return this.tipoOperacionField;
        }
        set {
            this.tipoOperacionField = value;
        }
    }
    
    /// <comentarios/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string numeroDeCuenta {
        get {
            return this.numeroDeCuentaField;
        }
        set {
            this.numeroDeCuentaField = value;
        }
    }
    
    /// <comentarios/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public decimal subTotal {
        get {
            return this.subTotalField;
        }
        set {
            this.subTotalField = value;
        }
    }
    
    /// <comentarios/>
    [System.Xml.Serialization.XmlIgnoreAttribute()]
    public bool subTotalSpecified {
        get {
            return this.subTotalFieldSpecified;
        }
        set {
            this.subTotalFieldSpecified = value;
        }
    }
    
    /// <comentarios/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public decimal total {
        get {
            return this.totalField;
        }
        set {
            this.totalField = value;
        }
    }
}

/// <comentarios/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://www.sat.gob.mx/ecc")]
public partial class EstadoDeCuentaCombustibleConceptoEstadoDeCuentaCombustible {
    
    private EstadoDeCuentaCombustibleConceptoEstadoDeCuentaCombustibleTraslado[] trasladosField;
    
    private string identificadorField;
    
    private System.DateTime fechaField;
    
    private string rfcField;
    
    private string claveEstacionField;
    
    private decimal cantidadField;
    
    private string nombreCombustibleField;
    
    private string folioOperacionField;
    
    private decimal valorUnitarioField;
    
    private decimal importeField;
    
    /// <comentarios/>
    [System.Xml.Serialization.XmlArrayItemAttribute("Traslado", IsNullable=false)]
    public EstadoDeCuentaCombustibleConceptoEstadoDeCuentaCombustibleTraslado[] Traslados {
        get {
            return this.trasladosField;
        }
        set {
            this.trasladosField = value;
        }
    }
    
    /// <comentarios/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string identificador {
        get {
            return this.identificadorField;
        }
        set {
            this.identificadorField = value;
        }
    }
    
    /// <comentarios/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public System.DateTime fecha {
        get {
            return this.fechaField;
        }
        set {
            this.fechaField = value;
        }
    }
    
    /// <comentarios/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string rfc {
        get {
            return this.rfcField;
        }
        set {
            this.rfcField = value;
        }
    }
    
    /// <comentarios/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string claveEstacion {
        get {
            return this.claveEstacionField;
        }
        set {
            this.claveEstacionField = value;
        }
    }
    
    /// <comentarios/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public decimal cantidad {
        get {
            return this.cantidadField;
        }
        set {
            this.cantidadField = value;
        }
    }
    
    /// <comentarios/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string nombreCombustible {
        get {
            return this.nombreCombustibleField;
        }
        set {
            this.nombreCombustibleField = value;
        }
    }
    
    /// <comentarios/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string folioOperacion {
        get {
            return this.folioOperacionField;
        }
        set {
            this.folioOperacionField = value;
        }
    }
    
    /// <comentarios/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public decimal valorUnitario {
        get {
            return this.valorUnitarioField;
        }
        set {
            this.valorUnitarioField = value;
        }
    }
    
    /// <comentarios/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public decimal importe {
        get {
            return this.importeField;
        }
        set {
            this.importeField = value;
        }
    }
}

/// <comentarios/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://www.sat.gob.mx/ecc")]
public partial class EstadoDeCuentaCombustibleConceptoEstadoDeCuentaCombustibleTraslado {
    
    private EstadoDeCuentaCombustibleConceptoEstadoDeCuentaCombustibleTrasladoImpuesto impuestoField;
    
    private decimal tasaField;
    
    private decimal importeField;
    
    /// <comentarios/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public EstadoDeCuentaCombustibleConceptoEstadoDeCuentaCombustibleTrasladoImpuesto impuesto {
        get {
            return this.impuestoField;
        }
        set {
            this.impuestoField = value;
        }
    }
    
    /// <comentarios/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public decimal tasa {
        get {
            return this.tasaField;
        }
        set {
            this.tasaField = value;
        }
    }
    
    /// <comentarios/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public decimal importe {
        get {
            return this.importeField;
        }
        set {
            this.importeField = value;
        }
    }
}

/// <comentarios/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
[System.SerializableAttribute()]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://www.sat.gob.mx/ecc")]
public enum EstadoDeCuentaCombustibleConceptoEstadoDeCuentaCombustibleTrasladoImpuesto {
    
    /// <comentarios/>
    IVA,
    
    /// <comentarios/>
    IEPS,
}
*/