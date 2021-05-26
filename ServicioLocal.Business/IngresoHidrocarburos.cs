using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace ServicioLocal.Business.Hidrocarburos
{
    [GeneratedCode("xsd", "4.0.30319.1"), DesignerCategory("code"), DebuggerStepThrough, XmlRoot(Namespace = "http://www.sat.gob.mx/IngresosHidrocarburos10", IsNullable = false), XmlType(AnonymousType = true, Namespace = "http://www.sat.gob.mx/IngresosHidrocarburos10")]
    [Serializable]
    public class IngresosHidrocarburos
    {
        [XmlAttribute("schemaLocation", Namespace = "http://www.w3.org/2001/XMLSchema-instance")]
        public string xsiSchemaLocation = "http://www.sat.gob.mx/IngresosHidrocarburos10 http://www.sat.gob.mx/sitio_internet/cfd/IngresosHidrocarburos10/IngresosHidrocarburos.xsd";

        private List<IngresosHidrocarburosDocumentoRelacionado> documentoRelacionadoField;

        private string versionField;

        private string numeroContratoField;

        private decimal contraprestacionPagadaOperadorField;

        private decimal porcentajeField;

        [XmlElement("DocumentoRelacionado")]
        public List<IngresosHidrocarburosDocumentoRelacionado> DocumentoRelacionado
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
        public decimal ContraprestacionPagadaOperador
        {
            get
            {
                return this.contraprestacionPagadaOperadorField;
            }
            set
            {
                this.contraprestacionPagadaOperadorField = value;
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

        public IngresosHidrocarburos()
        {
            this.versionField = "1.0";
        }
    }

    [GeneratedCode("xsd", "4.0.30319.1"), DesignerCategory("code"), DebuggerStepThrough, XmlType(AnonymousType = true, Namespace = "http://www.sat.gob.mx/IngresosHidrocarburos10")]
    [Serializable]
    public partial class IngresosHidrocarburosDocumentoRelacionado
    {
        private string folioFiscalVinculadoField;

        private DateTime fechaFolioFiscalVinculadoField;

        private Meses mesField;

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
}
