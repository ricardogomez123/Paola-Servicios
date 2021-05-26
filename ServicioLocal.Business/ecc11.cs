

using System.Xml.Serialization;


/// <comentarios/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://www.sat.gob.mx/EstadoDeCuentaCombustible12")]
[System.Xml.Serialization.XmlRootAttribute(Namespace="http://www.sat.gob.mx/EstadoDeCuentaCombustible12", IsNullable=false)]
public partial class EstadoDeCuentaCombustible {
    
    private EstadoDeCuentaCombustibleConceptoEstadoDeCuentaCombustible[] conceptosField;
    
    private string versionField;
    
    private string tipoOperacionField;
    
    private string numeroDeCuentaField;
    
    private decimal subTotalField;
    
    private decimal totalField;
    
    public EstadoDeCuentaCombustible() {
        this.versionField = "1.2";
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
    public string Version {
        get {
            return this.versionField;
        }
        set {
            this.versionField = value;
        }
    }
    
    /// <comentarios/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string TipoOperacion {
        get {
            return this.tipoOperacionField;
        }
        set {
            this.tipoOperacionField = value;
        }
    }
    
    /// <comentarios/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string NumeroDeCuenta {
        get {
            return this.numeroDeCuentaField;
        }
        set {
            this.numeroDeCuentaField = value;
        }
    }
    
    /// <comentarios/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public decimal SubTotal {
        get {
            return this.subTotalField;
        }
        set {
            this.subTotalField = value;
        }
    }
    
    /// <comentarios/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public decimal Total {
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
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://www.sat.gob.mx/EstadoDeCuentaCombustible12")]
public partial class EstadoDeCuentaCombustibleConceptoEstadoDeCuentaCombustible {
    
    private EstadoDeCuentaCombustibleConceptoEstadoDeCuentaCombustibleTraslado[] trasladosField;
    
    private string identificadorField;
    
    private string fechaField;
    
    private string rfcField;
    
    private string claveEstacionField;
    
    //private c_TAR tARField;
    
    private bool tARFieldSpecified;
    
    private decimal cantidadField;

    private c_ClaveTipoCombustible tipoCombustibleField;
    
    private string unidadField;
    
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
    public string Identificador {
        get {
            return this.identificadorField;
        }
        set {
            this.identificadorField = value;
        }
    }
    
    /// <comentarios/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string Fecha {
        get {
            return this.fechaField;
        }
        set {
            this.fechaField = value;
        }
    }
    
    /// <comentarios/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string Rfc {
        get {
            return this.rfcField;
        }
        set {
            this.rfcField = value;
        }
    }
    
    /// <comentarios/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string ClaveEstacion {
        get {
            return this.claveEstacionField;
        }
        set {
            this.claveEstacionField = value;
        }
    }
    
    ///// <comentarios/>
    //[System.Xml.Serialization.XmlAttributeAttribute()]
    //public c_TAR TAR {
    //    get {
    //        return this.tARField;
    //    }
    //    set {
    //        this.tARField = value;
    //    }
    //}
    
    /// <comentarios/>
    [System.Xml.Serialization.XmlIgnoreAttribute()]
    public bool TARSpecified {
        get {
            return this.tARFieldSpecified;
        }
        set {
            this.tARFieldSpecified = value;
        }
    }
    
    /// <comentarios/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public decimal Cantidad {
        get {
            return this.cantidadField;
        }
        set {
            this.cantidadField = value;
        }
    }
    
    /// <comentarios/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public c_ClaveTipoCombustible TipoCombustible
    {
        get {
            return this.tipoCombustibleField;
        }
        set {
            this.tipoCombustibleField = value;
        }
    }
    
    /// <comentarios/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string Unidad {
        get {
            return this.unidadField;
        }
        set {
            this.unidadField = value;
        }
    }
    
    /// <comentarios/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string NombreCombustible {
        get {
            return this.nombreCombustibleField;
        }
        set {
            this.nombreCombustibleField = value;
        }
    }
    
    /// <comentarios/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string FolioOperacion {
        get {
            return this.folioOperacionField;
        }
        set {
            this.folioOperacionField = value;
        }
    }
    
    /// <comentarios/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public decimal ValorUnitario {
        get {
            return this.valorUnitarioField;
        }
        set {
            this.valorUnitarioField = value;
        }
    }
    
    /// <comentarios/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public decimal Importe {
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
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://www.sat.gob.mx/EstadoDeCuentaCombustible12")]
public partial class EstadoDeCuentaCombustibleConceptoEstadoDeCuentaCombustibleTraslado {
    
    private EstadoDeCuentaCombustibleConceptoEstadoDeCuentaCombustibleTrasladoImpuesto impuestoField;
    
    private decimal tasaOCuotaField;
    
    private decimal importeField;
    
    /// <comentarios/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public EstadoDeCuentaCombustibleConceptoEstadoDeCuentaCombustibleTrasladoImpuesto Impuesto {
        get {
            return this.impuestoField;
        }
        set {
            this.impuestoField = value;
        }
    }
    
    /// <comentarios/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public decimal TasaOCuota {
        get {
            return this.tasaOCuotaField;
        }
        set {
            this.tasaOCuotaField = value;
        }
    }
    
    /// <comentarios/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public decimal Importe {
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
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://www.sat.gob.mx/EstadoDeCuentaCombustible12")]
public enum EstadoDeCuentaCombustibleConceptoEstadoDeCuentaCombustibleTrasladoImpuesto {
    
    /// <comentarios/>
    IVA,
    
    /// <comentarios/>
    IEPS,
}

/// <comentarios/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
[System.SerializableAttribute()]
[System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.sat.gob.mx/EstadoDeCuentaCombustible12")]
public enum c_TAR {
    
    /// <comentarios/>
    [System.Xml.Serialization.XmlEnumAttribute("617")]
    Item617,
    
    /// <comentarios/>
    [System.Xml.Serialization.XmlEnumAttribute("619")]
    Item619,
    
    /// <comentarios/>
    [System.Xml.Serialization.XmlEnumAttribute("624")]
    Item624,
    
    /// <comentarios/>
    [System.Xml.Serialization.XmlEnumAttribute("652")]
    Item652,
    
    /// <comentarios/>
    [System.Xml.Serialization.XmlEnumAttribute("654")]
    Item654,
    
    /// <comentarios/>
    [System.Xml.Serialization.XmlEnumAttribute("655")]
    Item655,
    
    /// <comentarios/>
    [System.Xml.Serialization.XmlEnumAttribute("656")]
    Item656,
    
    /// <comentarios/>
    [System.Xml.Serialization.XmlEnumAttribute("658")]
    Item658,
    
    /// <comentarios/>
    [System.Xml.Serialization.XmlEnumAttribute("659")]
    Item659,
    
    /// <comentarios/>
    [System.Xml.Serialization.XmlEnumAttribute("660")]
    Item660,
    
    /// <comentarios/>
    [System.Xml.Serialization.XmlEnumAttribute("661")]
    Item661,
    
    /// <comentarios/>
    [System.Xml.Serialization.XmlEnumAttribute("667")]
    Item667,
    
    /// <comentarios/>
    [System.Xml.Serialization.XmlEnumAttribute("693")]
    Item693,
    
    /// <comentarios/>
    [System.Xml.Serialization.XmlEnumAttribute("602")]
    Item602,
    
    /// <comentarios/>
    [System.Xml.Serialization.XmlEnumAttribute("603")]
    Item603,
    
    /// <comentarios/>
    [System.Xml.Serialization.XmlEnumAttribute("604")]
    Item604,
    
    /// <comentarios/>
    [System.Xml.Serialization.XmlEnumAttribute("605")]
    Item605,
    
    /// <comentarios/>
    [System.Xml.Serialization.XmlEnumAttribute("606")]
    Item606,
    
    /// <comentarios/>
    [System.Xml.Serialization.XmlEnumAttribute("607")]
    Item607,
    
    /// <comentarios/>
    [System.Xml.Serialization.XmlEnumAttribute("608")]
    Item608,
    
    /// <comentarios/>
    [System.Xml.Serialization.XmlEnumAttribute("609")]
    Item609,
    
    /// <comentarios/>
    [System.Xml.Serialization.XmlEnumAttribute("611")]
    Item611,
    
    /// <comentarios/>
    [System.Xml.Serialization.XmlEnumAttribute("612")]
    Item612,
    
    /// <comentarios/>
    [System.Xml.Serialization.XmlEnumAttribute("613")]
    Item613,
    
    /// <comentarios/>
    [System.Xml.Serialization.XmlEnumAttribute("614")]
    Item614,
    
    /// <comentarios/>
    [System.Xml.Serialization.XmlEnumAttribute("615")]
    Item615,
    
    /// <comentarios/>
    [System.Xml.Serialization.XmlEnumAttribute("620")]
    Item620,
    
    /// <comentarios/>
    [System.Xml.Serialization.XmlEnumAttribute("621")]
    Item621,
    
    /// <comentarios/>
    [System.Xml.Serialization.XmlEnumAttribute("622")]
    Item622,
    
    /// <comentarios/>
    [System.Xml.Serialization.XmlEnumAttribute("623")]
    Item623,
    
    /// <comentarios/>
    [System.Xml.Serialization.XmlEnumAttribute("625")]
    Item625,
    
    /// <comentarios/>
    [System.Xml.Serialization.XmlEnumAttribute("657")]
    Item657,
    
    /// <comentarios/>
    [System.Xml.Serialization.XmlEnumAttribute("664")]
    Item664,
    
    /// <comentarios/>
    [System.Xml.Serialization.XmlEnumAttribute("665")]
    Item665,
    
    /// <comentarios/>
    [System.Xml.Serialization.XmlEnumAttribute("666")]
    Item666,
    
    /// <comentarios/>
    [System.Xml.Serialization.XmlEnumAttribute("669")]
    Item669,
    
    /// <comentarios/>
    [System.Xml.Serialization.XmlEnumAttribute("695")]
    Item695,
    
    /// <comentarios/>
    [System.Xml.Serialization.XmlEnumAttribute("696")]
    Item696,
    
    /// <comentarios/>
    [System.Xml.Serialization.XmlEnumAttribute("697")]
    Item697,
    
    /// <comentarios/>
    [System.Xml.Serialization.XmlEnumAttribute("699")]
    Item699,
    
    /// <comentarios/>
    [System.Xml.Serialization.XmlEnumAttribute("627")]
    Item627,
    
    /// <comentarios/>
    [System.Xml.Serialization.XmlEnumAttribute("628")]
    Item628,
    
    /// <comentarios/>
    [System.Xml.Serialization.XmlEnumAttribute("629")]
    Item629,
    
    /// <comentarios/>
    [System.Xml.Serialization.XmlEnumAttribute("630")]
    Item630,
    
    /// <comentarios/>
    [System.Xml.Serialization.XmlEnumAttribute("631")]
    Item631,
    
    /// <comentarios/>
    [System.Xml.Serialization.XmlEnumAttribute("632")]
    Item632,
    
    /// <comentarios/>
    [System.Xml.Serialization.XmlEnumAttribute("633")]
    Item633,
    
    /// <comentarios/>
    [System.Xml.Serialization.XmlEnumAttribute("636")]
    Item636,
    
    /// <comentarios/>
    [System.Xml.Serialization.XmlEnumAttribute("637")]
    Item637,
    
    /// <comentarios/>
    [System.Xml.Serialization.XmlEnumAttribute("638")]
    Item638,
    
    /// <comentarios/>
    [System.Xml.Serialization.XmlEnumAttribute("639")]
    Item639,
    
    /// <comentarios/>
    [System.Xml.Serialization.XmlEnumAttribute("640")]
    Item640,
    
    /// <comentarios/>
    [System.Xml.Serialization.XmlEnumAttribute("641")]
    Item641,
    
    /// <comentarios/>
    [System.Xml.Serialization.XmlEnumAttribute("644")]
    Item644,
    
    /// <comentarios/>
    [System.Xml.Serialization.XmlEnumAttribute("645")]
    Item645,
    
    /// <comentarios/>
    [System.Xml.Serialization.XmlEnumAttribute("646")]
    Item646,
    
    /// <comentarios/>
    [System.Xml.Serialization.XmlEnumAttribute("647")]
    Item647,
    
    /// <comentarios/>
    [System.Xml.Serialization.XmlEnumAttribute("648")]
    Item648,
    
    /// <comentarios/>
    [System.Xml.Serialization.XmlEnumAttribute("649")]
    Item649,
    
    /// <comentarios/>
    [System.Xml.Serialization.XmlEnumAttribute("650")]
    Item650,
    
    /// <comentarios/>
    [System.Xml.Serialization.XmlEnumAttribute("663")]
    Item663,
    
    /// <comentarios/>
    [System.Xml.Serialization.XmlEnumAttribute("668")]
    Item668,
    
    /// <comentarios/>
    [System.Xml.Serialization.XmlEnumAttribute("672")]
    Item672,
    
    /// <comentarios/>
    [System.Xml.Serialization.XmlEnumAttribute("673")]
    Item673,
    
    /// <comentarios/>
    [System.Xml.Serialization.XmlEnumAttribute("674")]
    Item674,
    
    /// <comentarios/>
    [System.Xml.Serialization.XmlEnumAttribute("675")]
    Item675,
    
    /// <comentarios/>
    [System.Xml.Serialization.XmlEnumAttribute("676")]
    Item676,
    
    /// <comentarios/>
    [System.Xml.Serialization.XmlEnumAttribute("677")]
    Item677,
    
    /// <comentarios/>
    [System.Xml.Serialization.XmlEnumAttribute("678")]
    Item678,
    
    /// <comentarios/>
    [System.Xml.Serialization.XmlEnumAttribute("681")]
    Item681,
    
    /// <comentarios/>
    [System.Xml.Serialization.XmlEnumAttribute("682")]
    Item682,
    
    /// <comentarios/>
    [System.Xml.Serialization.XmlEnumAttribute("683")]
    Item683,
    
    /// <comentarios/>
    [System.Xml.Serialization.XmlEnumAttribute("684")]
    Item684,
    
    /// <comentarios/>
    [System.Xml.Serialization.XmlEnumAttribute("685")]
    Item685,
    
    /// <comentarios/>
    [System.Xml.Serialization.XmlEnumAttribute("688")]
    Item688,
    
    /// <comentarios/>
    [System.Xml.Serialization.XmlEnumAttribute("689")]
    Item689,
    
    /// <comentarios/>
    [System.Xml.Serialization.XmlEnumAttribute("690")]
    Item690,
    
    /// <comentarios/>
    [System.Xml.Serialization.XmlEnumAttribute("698")]
    Item698,
}

/// <comentarios/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
[System.SerializableAttribute()]
[System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.sat.gob.mx/EstadoDeCuentaCombustible12")]
public enum c_claveProducto {
    
    /// <comentarios/>
    [System.Xml.Serialization.XmlEnumAttribute("32011")]
    Item32011,
    
    /// <comentarios/>
    [System.Xml.Serialization.XmlEnumAttribute("32012")]
    Item32012,
    
    /// <comentarios/>
    [System.Xml.Serialization.XmlEnumAttribute("34006")]
    Item34006,
    
    /// <comentarios/>
    [System.Xml.Serialization.XmlEnumAttribute("34008")]
    Item34008,
    
    /// <comentarios/>
    Otros,
}

[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
[System.SerializableAttribute()]
[System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.sat.gob.mx/EstadoDeCuentaCombustible12")]
public enum c_ClaveTipoCombustible
{

    /// <comentarios/>
    [System.Xml.Serialization.XmlEnumAttribute("1")]
    Item1,

    /// <comentarios/>
    [System.Xml.Serialization.XmlEnumAttribute("2")]
    Item2,

    /// <comentarios/>
    [System.Xml.Serialization.XmlEnumAttribute("3")]
    Item3,

    /// <comentarios/>
    [System.Xml.Serialization.XmlEnumAttribute("4")]
    Item4,

    /// <comentarios/>
    [System.Xml.Serialization.XmlEnumAttribute("5")]
    Item5,

}


