using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace ServicioLocal.Business.Pagoo
{
    [GeneratedCode("xsd", "4.0.30319.1"), DesignerCategory("code"), DebuggerStepThrough, XmlRoot(Namespace = "http://www.sat.gob.mx/cfd/3", IsNullable = false), XmlType(AnonymousType = true, Namespace = "http://www.sat.gob.mx/cfd/3")]
    [Serializable]
    public class Comprobante
    {
        [XmlAttribute("schemaLocation", Namespace = "http://www.w3.org/2001/XMLSchema-instance")]
        public string xsiSchemaLocation = "http://www.sat.gob.mx/cfd/3 http://www.sat.gob.mx/sitio_internet/cfd/3/cfdv33.xsd";

        private string totalField;

        private string subTotalField;

        private ComprobanteComplemento complementoField;

        private List<ComprobanteConcepto> conceptosField;

        [XmlArrayItem("Concepto", IsNullable = false)]
        public List<ComprobanteConcepto> Conceptos
        {
            get
            {
                return this.conceptosField;
            }
            set
            {
                this.conceptosField = value;
            }
        }

        public ComprobanteComplemento Complemento
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

        [XmlAttribute]
        public string SubTotal
        {
            get
            {
                return this.subTotalField;
            }
            set
            {
                this.subTotalField = value;
            }
        }

        [XmlAttribute]
        public string Total
        {
            get
            {
                return this.totalField;
            }
            set
            {
                this.totalField = value;
            }
        }
    }
    [GeneratedCode("xsd", "4.0.30319.1"), DesignerCategory("code"), DebuggerStepThrough, XmlType(AnonymousType = true, Namespace = "http://www.sat.gob.mx/cfd/3")]
    [Serializable]
    public partial class ComprobanteConcepto
    {
        private string claveProdServFieldx;

        private string noIdentificacionFieldx;

        private string cantidadFieldx;

        private string claveUnidadFieldx;

        private string unidadFieldx;

        private string descripcionFieldx;

        private string valorUnitarioFieldx;

        private string importeFieldx;

        private decimal descuentoFieldx;

        private bool descuentoFieldSpecifiedx;

        [XmlAttribute]
        public string ClaveProdServ
        {
            get
            {
                return this.claveProdServFieldx;
            }
            set
            {
                this.claveProdServFieldx = value;
            }
        }

        [XmlAttribute]
        public string NoIdentificacion
        {
            get
            {
                return this.noIdentificacionFieldx;
            }
            set
            {
                this.noIdentificacionFieldx = value;
            }
        }

        [XmlAttribute]
        public string Cantidad
        {
            get
            {
                return this.cantidadFieldx;
            }
            set
            {
                this.cantidadFieldx = value;
            }
        }

        [XmlAttribute]
        public string ClaveUnidad
        {
            get
            {
                return this.claveUnidadFieldx;
            }
            set
            {
                this.claveUnidadFieldx = value;
            }
        }

        [XmlAttribute]
        public string Unidad
        {
            get
            {
                return this.unidadFieldx;
            }
            set
            {
                this.unidadFieldx = value;
            }
        }

        [XmlAttribute]
        public string Descripcion
        {
            get
            {
                return this.descripcionFieldx;
            }
            set
            {
                this.descripcionFieldx = value;
            }
        }

        [XmlAttribute]
        public string ValorUnitario
        {
            get
            {
                return this.valorUnitarioFieldx;
            }
            set
            {
                this.valorUnitarioFieldx = value;
            }
        }

        [XmlAttribute]
        public string Importe
        {
            get
            {
                return this.importeFieldx;
            }
            set
            {
                this.importeFieldx = value;
            }
        }

        [XmlAttribute]
        public decimal Descuento
        {
            get
            {
                return this.descuentoFieldx;
            }
            set
            {
                this.descuentoFieldx = value;
            }
        }

        [XmlIgnore]
        public bool DescuentoSpecified
        {
            get
            {
                return this.descuentoFieldSpecifiedx;
            }
            set
            {
                this.descuentoFieldSpecifiedx = value;
            }
        }
    }
}
