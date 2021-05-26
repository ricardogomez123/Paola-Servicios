using System;
using System.CodeDom.Compiler;

namespace Planesderetiro11
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Xml.Serialization;

    [GeneratedCode("xsd", "4.0.30319.1"), DesignerCategory("code"), DebuggerStepThrough, XmlRoot(Namespace = "http://www.sat.gob.mx/esquemas/retencionpago/1/planesderetiro11", IsNullable = false), XmlType(AnonymousType = true, Namespace = "http://www.sat.gob.mx/esquemas/retencionpago/1/planesderetiro11")]

    [Serializable]
    public class Planesderetiro
    {
        [XmlAttribute("schemaLocation", Namespace = "http://www.w3.org/2001/XMLSchema-instance")]
        public string xsiSchemaLocation = "http://www.sat.gob.mx/esquemas/retencionpago/1/planesderetiro11 http://www.sat.gob.mx/esquemas/retencionpago/1/planesderetiro11/planesderetiro.xsd";

        private List<PlanesderetiroAportacionesODepositos> aportacionesODepositosField;

        private string versionField;

        private PlanesderetiroSistemaFinanc sistemaFinancField;

        private decimal montTotAportAnioInmAnteriorField;

        private bool montTotAportAnioInmAnteriorFieldSpecified;

        private decimal montIntRealesDevengAniooInmAntField;

        private PlanesderetiroHuboRetirosAnioInmAntPer huboRetirosAnioInmAntPerField;

        private decimal montTotRetiradoAnioInmAntPerField;

        private bool montTotRetiradoAnioInmAntPerFieldSpecified;

        private decimal montTotExentRetiradoAnioInmAntField;

        private bool montTotExentRetiradoAnioInmAntFieldSpecified;

        private decimal montTotExedenteAnioInmAntField;

        private bool montTotExedenteAnioInmAntFieldSpecified;

        private PlanesderetiroHuboRetirosAnioInmAnt huboRetirosAnioInmAntField;

        private decimal montTotRetiradoAnioInmAntField;

        private bool montTotRetiradoAnioInmAntFieldSpecified;

        private string numReferenciaField;

        [XmlElement("AportacionesODepositos")]
        public List<PlanesderetiroAportacionesODepositos> AportacionesODepositos
        {
            get
            {
                return this.aportacionesODepositosField;
            }
            set
            {
                this.aportacionesODepositosField = value;
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
        public PlanesderetiroSistemaFinanc SistemaFinanc
        {
            get
            {
                return this.sistemaFinancField;
            }
            set
            {
                this.sistemaFinancField = value;
            }
        }

        [XmlAttribute]
        public decimal MontTotAportAnioInmAnterior
        {
            get
            {
                return this.montTotAportAnioInmAnteriorField;
            }
            set
            {
                this.montTotAportAnioInmAnteriorField = value;
            }
        }

        [XmlIgnore]
        public bool MontTotAportAnioInmAnteriorSpecified
        {
            get
            {
                return this.montTotAportAnioInmAnteriorFieldSpecified;
            }
            set
            {
                this.montTotAportAnioInmAnteriorFieldSpecified = value;
            }
        }

        [XmlAttribute]
        public decimal MontIntRealesDevengAniooInmAnt
        {
            get
            {
                return this.montIntRealesDevengAniooInmAntField;
            }
            set
            {
                this.montIntRealesDevengAniooInmAntField = value;
            }
        }

        [XmlAttribute]
        public PlanesderetiroHuboRetirosAnioInmAntPer HuboRetirosAnioInmAntPer
        {
            get
            {
                return this.huboRetirosAnioInmAntPerField;
            }
            set
            {
                this.huboRetirosAnioInmAntPerField = value;
            }
        }

        [XmlAttribute]
        public decimal MontTotRetiradoAnioInmAntPer
        {
            get
            {
                return this.montTotRetiradoAnioInmAntPerField;
            }
            set
            {
                this.montTotRetiradoAnioInmAntPerField = value;
            }
        }

        [XmlIgnore]
        public bool MontTotRetiradoAnioInmAntPerSpecified
        {
            get
            {
                return this.montTotRetiradoAnioInmAntPerFieldSpecified;
            }
            set
            {
                this.montTotRetiradoAnioInmAntPerFieldSpecified = value;
            }
        }

        [XmlAttribute]
        public decimal MontTotExentRetiradoAnioInmAnt
        {
            get
            {
                return this.montTotExentRetiradoAnioInmAntField;
            }
            set
            {
                this.montTotExentRetiradoAnioInmAntField = value;
            }
        }

        [XmlIgnore]
        public bool MontTotExentRetiradoAnioInmAntSpecified
        {
            get
            {
                return this.montTotExentRetiradoAnioInmAntFieldSpecified;
            }
            set
            {
                this.montTotExentRetiradoAnioInmAntFieldSpecified = value;
            }
        }

        [XmlAttribute]
        public decimal MontTotExedenteAnioInmAnt
        {
            get
            {
                return this.montTotExedenteAnioInmAntField;
            }
            set
            {
                this.montTotExedenteAnioInmAntField = value;
            }
        }

        [XmlIgnore]
        public bool MontTotExedenteAnioInmAntSpecified
        {
            get
            {
                return this.montTotExedenteAnioInmAntFieldSpecified;
            }
            set
            {
                this.montTotExedenteAnioInmAntFieldSpecified = value;
            }
        }

        [XmlAttribute]
        public PlanesderetiroHuboRetirosAnioInmAnt HuboRetirosAnioInmAnt
        {
            get
            {
                return this.huboRetirosAnioInmAntField;
            }
            set
            {
                this.huboRetirosAnioInmAntField = value;
            }
        }

        [XmlAttribute]
        public decimal MontTotRetiradoAnioInmAnt
        {
            get
            {
                return this.montTotRetiradoAnioInmAntField;
            }
            set
            {
                this.montTotRetiradoAnioInmAntField = value;
            }
        }

        [XmlIgnore]
        public bool MontTotRetiradoAnioInmAntSpecified
        {
            get
            {
                return this.montTotRetiradoAnioInmAntFieldSpecified;
            }
            set
            {
                this.montTotRetiradoAnioInmAntFieldSpecified = value;
            }
        }

        [XmlAttribute]
        public string NumReferencia
        {
            get
            {
                return this.numReferenciaField;
            }
            set
            {
                this.numReferenciaField = value;
            }
        }

        public Planesderetiro()
        {
            this.versionField = "1.1";
        }
    }


    [GeneratedCode("xsd", "4.0.30319.1"), DesignerCategory("code"), DebuggerStepThrough, XmlType(AnonymousType = true, Namespace = "http://www.sat.gob.mx/esquemas/retencionpago/1/planesderetiro11")]
    [Serializable]
    public partial class PlanesderetiroAportacionesODepositos
    {
        private c_TipoAportODep tipoAportacionODepositoField;

        private decimal montAportODepField;

        private string rFCFiduciariaField;

        public c_TipoAportODep TipoAportacionODeposito
        {
            get
            {
                return this.tipoAportacionODepositoField;
            }
            set
            {
                this.tipoAportacionODepositoField = value;
            }
        }

        public decimal MontAportODep
        {
            get
            {
                return this.montAportODepField;
            }
            set
            {
                this.montAportODepField = value;
            }
        }

        public string RFCFiduciaria
        {
            get
            {
                return this.rFCFiduciariaField;
            }
            set
            {
                this.rFCFiduciariaField = value;
            }
        }
    }

    [GeneratedCode("xsd", "4.0.30319.1"), XmlType(AnonymousType = true, Namespace = "http://www.sat.gob.mx/esquemas/retencionpago/1/planesderetiro11")]
    [Serializable]
    public enum PlanesderetiroSistemaFinanc
    {
        SI,
        NO
    }
    [GeneratedCode("xsd", "4.0.30319.1"), XmlType(AnonymousType = true, Namespace = "http://www.sat.gob.mx/esquemas/retencionpago/1/planesderetiro11")]
    [Serializable]
    public enum PlanesderetiroHuboRetirosAnioInmAntPer
    {
        SI,
        NO
    }

    [GeneratedCode("xsd", "4.0.30319.1"), XmlType(AnonymousType = true, Namespace = "http://www.sat.gob.mx/esquemas/retencionpago/1/planesderetiro11")]
    [Serializable]
    public enum PlanesderetiroHuboRetirosAnioInmAnt
    {
        SI,
        NO
    }
    [GeneratedCode("xsd", "4.0.30319.1"), XmlType(AnonymousType = true, Namespace = "http://www.sat.gob.mx/esquemas/retencionpago/1/planesderetiro11")]
    [Serializable]
    public enum c_TipoAportODep
    {
        a,
        b,
        c
    }
}