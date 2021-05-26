
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.deloitte.com/CFD/Addenda/Receptor")]
[System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.deloitte.com/CFD/Addenda/Receptor", IsNullable = false)]
public partial class AddendaDeloitte
{

    private string noPedidoField;

    private AddendaDeloitteMoneda monedaField;

    private string mailContactoDeloitteField;

    private AddendaDeloitteOficina oficinaField;

    private AddendaDeloitteOrigenFactura origenFacturaField;

    private string numeroProveedorField;

    private string mailProveedorField;

    private string nombreContactoProveedorField;

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute(DataType = "integer")]
    public string noPedido
    {
        get
        {
            return this.noPedidoField;
        }
        set
        {
            this.noPedidoField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public AddendaDeloitteMoneda moneda
    {
        get
        {
            return this.monedaField;
        }
        set
        {
            this.monedaField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string mailContactoDeloitte
    {
        get
        {
            return this.mailContactoDeloitteField;
        }
        set
        {
            this.mailContactoDeloitteField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public AddendaDeloitteOficina oficina
    {
        get
        {
            return this.oficinaField;
        }
        set
        {
            this.oficinaField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public AddendaDeloitteOrigenFactura origenFactura
    {
        get
        {
            return this.origenFacturaField;
        }
        set
        {
            this.origenFacturaField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string numeroProveedor
    {
        get
        {
            return this.numeroProveedorField;
        }
        set
        {
            this.numeroProveedorField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string mailProveedor
    {
        get
        {
            return this.mailProveedorField;
        }
        set
        {
            this.mailProveedorField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string nombreContactoProveedor
    {
        get
        {
            return this.nombreContactoProveedorField;
        }
        set
        {
            this.nombreContactoProveedorField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
[System.SerializableAttribute()]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.deloitte.com/CFD/Addenda/Receptor")]
public enum AddendaDeloitteMoneda
{

    /// <remarks/>
    MXP,

    /// <remarks/>
    USD,
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
[System.SerializableAttribute()]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.deloitte.com/CFD/Addenda/Receptor")]
public enum AddendaDeloitteOficina
{

    /// <remarks/>
    CANC,

    /// <remarks/>
    CHIH,

    /// <remarks/>
    GDL,

    /// <remarks/>
    LEON,

    /// <remarks/>
    MEX,

    /// <remarks/>
    MTY,

    /// <remarks/>
    PUE,

    /// <remarks/>
    QRO,

    /// <remarks/>
    TIJ,
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
[System.SerializableAttribute()]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.deloitte.com/CFD/Addenda/Receptor")]
public enum AddendaDeloitteOrigenFactura
{

    /// <remarks/>
    PEDIDO,

    /// <remarks/>
    CONTRATO,

    /// <remarks/>
    EGRESO,
}
