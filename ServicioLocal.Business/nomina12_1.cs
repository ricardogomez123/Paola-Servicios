
namespace ServicioLocal.Business
{
    using System;
    using System.Diagnostics;
    using System.Xml.Serialization;
    using System.Collections;
    using System.Xml.Schema;
    using System.ComponentModel;
    using System.Collections.Generic;


    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.sat.gob.mx/nomina12")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.sat.gob.mx/nomina12", IsNullable = false)]
    public partial class Nomina
    {

        private NominaEmisor emisorField;

        private NominaReceptor receptorField;

        private NominaPercepciones percepcionesField;

        private NominaDeducciones deduccionesField;

        private List<NominaOtroPago> otrosPagosField;

        private List<NominaIncapacidad> incapacidadesField;

        private string versionField;

        private System.String tipoNominaField;

        private System.String fechaPagoField;

        private System.String fechaInicialPagoField;

        private System.String fechaFinalPagoField;

        private decimal numDiasPagadosField;

        private decimal totalPercepcionesField;

        private bool totalPercepcionesFieldSpecified;

        private decimal totalDeduccionesField;

        private bool totalDeduccionesFieldSpecified;

        private decimal totalOtrosPagosField;

        private bool totalOtrosPagosFieldSpecified;

        public Nomina()
        {
            //this.incapacidadesField = new List<NominaIncapacidad>();
            //this.otrosPagosField = new List<NominaOtroPago>();
            //this.deduccionesField = new NominaDeducciones();
            //this.percepcionesField = new NominaPercepciones();
            //this.receptorField = new NominaReceptor();
            //this.emisorField = new NominaEmisor();
            this.versionField = "1.2";
        }

        [XmlAttribute("schemaLocation", Namespace = XmlSchema.InstanceNamespace)]
        public string xsiSchemaLocation ="http://www.sat.gob.mx/nomina12 http://www.sat.gob.mx/sitio_internet/cfd/nomina/nomina12.xsd";


        public NominaEmisor Emisor
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

        public NominaReceptor Receptor
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

        public NominaPercepciones Percepciones
        {
            get
            {
                return this.percepcionesField;
            }
            set
            {
                this.percepcionesField = value;
            }
        }

        public NominaDeducciones Deducciones
        {
            get
            {
                return this.deduccionesField;
            }
            set
            {
                this.deduccionesField = value;
            }
        }

        [System.Xml.Serialization.XmlArrayItemAttribute("OtroPago", IsNullable = false)]
        public List<NominaOtroPago> OtrosPagos
        {
            get
            {
                return this.otrosPagosField;
            }
            set
            {
                this.otrosPagosField = value;
            }
        }

        [System.Xml.Serialization.XmlArrayItemAttribute("Incapacidad", IsNullable = false)]
        public List<NominaIncapacidad> Incapacidades
        {
            get
            {
                return this.incapacidadesField;
            }
            set
            {
                this.incapacidadesField = value;
            }
        }
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
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string TipoNomina
        {
            get
            {
                return this.tipoNominaField;
            }
            set
            {
                this.tipoNominaField = value;
            }
        }
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public System.String FechaPago  //motivos de conversion
        {
            get
            {
                return this.fechaPagoField;
            }
            set
            {
                this.fechaPagoField = value;
            }
        }
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public System.String FechaInicialPago
        {
            get
            {
                return this.fechaInicialPagoField;
            }
            set
            {
                this.fechaInicialPagoField = value;
            }
        }
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public System.String FechaFinalPago
        {
            get
            {
                return this.fechaFinalPagoField;
            }
            set
            {
                this.fechaFinalPagoField = value;
            }
        }
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal NumDiasPagados
        {
            get
            {
                return this.numDiasPagadosField;
            }
            set
            {
                this.numDiasPagadosField = value;
            }
        }
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal TotalPercepciones
        {
            get
            {
                return this.totalPercepcionesField;
            }
            set
            {
                this.totalPercepcionesField = value;
            }
        }

        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool TotalPercepcionesSpecified
        {
            get
            {
                return this.totalPercepcionesFieldSpecified;
            }
            set
            {
                this.totalPercepcionesFieldSpecified = value;
            }
        }
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal TotalDeducciones
        {
            get
            {
                return this.totalDeduccionesField;
            }
            set
            {
                this.totalDeduccionesField = value;
            }
        }

        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool TotalDeduccionesSpecified
        {
            get
            {
                return this.totalDeduccionesFieldSpecified;
            }
            set
            {
                this.totalDeduccionesFieldSpecified = value;
            }
        }
         [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal TotalOtrosPagos
        {
            get
            {
                return this.totalOtrosPagosField;
            }
            set
            {
                this.totalOtrosPagosField = value;
            }
        }

        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool TotalOtrosPagosSpecified
        {
            get
            {
                return this.totalOtrosPagosFieldSpecified;
            }
            set
            {
                this.totalOtrosPagosFieldSpecified = value;
            }
        }
    }

    public partial class NominaEmisor
    {

        private NominaEmisorEntidadSNCF entidadSNCFField;

        private string curpField;

        private string registroPatronalField;

        private string rfcPatronOrigenField;

        public NominaEmisor()
        {
           // this.entidadSNCFField = new NominaEmisorEntidadSNCF();
        }

        public NominaEmisorEntidadSNCF EntidadSNCF
        {
            get
            {
                return this.entidadSNCFField;
            }
            set
            {
                this.entidadSNCFField = value;
            }
        }
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Curp
        {
            get
            {
                return this.curpField;
            }
            set
            {
                this.curpField = value;
            }
        }
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string RegistroPatronal
        {
            get
            {
                return this.registroPatronalField;
            }
            set
            {
                this.registroPatronalField = value;
            }
        }
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string RfcPatronOrigen
        {
            get
            {
                return this.rfcPatronOrigenField;
            }
            set
            {
                this.rfcPatronOrigenField = value;
            }
        }
    }

    public partial class NominaEmisorEntidadSNCF
    {

        private string origenRecursoField;

        private decimal montoRecursoPropioField;

        private bool montoRecursoPropioFieldSpecified;
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string OrigenRecurso
        {
            get
            {
                return this.origenRecursoField;
            }
            set
            {
                this.origenRecursoField = value;
            }
        }
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal MontoRecursoPropio
        {
            get
            {
                return this.montoRecursoPropioField;
            }
            set
            {
                this.montoRecursoPropioField = value;
            }
        }

        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool MontoRecursoPropioSpecified
        {
            get
            {
                return this.montoRecursoPropioFieldSpecified;
            }
            set
            {
                this.montoRecursoPropioFieldSpecified = value;
            }
        }
    }

    public enum c_OrigenRecurso
    {

        /// <comentarios/>
        IP,

        /// <comentarios/>
        IF,

        /// <comentarios/>
        IM,
    }

    public partial class NominaReceptor
    {

        private List<NominaReceptorSubContratacion> subContratacionField;

        private string curpField;

        private string numSeguridadSocialField;

        private System.String fechaInicioRelLaboralField;

        private bool fechaInicioRelLaboralFieldSpecified;

        private string antigüedadField;

        private string tipoContratoField;

        private NominaReceptorSindicalizado sindicalizadoField;

        private bool sindicalizadoFieldSpecified;

        private string tipoJornadaField;

        private bool tipoJornadaFieldSpecified;

        private string tipoRegimenField;

        private string numEmpleadoField;

        private string departamentoField;

        private string puestoField;

        private string riesgoPuestoField;

        private bool riesgoPuestoFieldSpecified;

        private string periodicidadPagoField;

        private string bancoField;

        private bool bancoFieldSpecified;

        private string cuentaBancariaField;

        private decimal salarioBaseCotAporField;

        private bool salarioBaseCotAporFieldSpecified;

        private decimal salarioDiarioIntegradoField;

        private bool salarioDiarioIntegradoFieldSpecified;

        private string claveEntFedField;

        public NominaReceptor()
        {
            //this.subContratacionField = new List<NominaReceptorSubContratacion>();
        }
         [XmlElement("SubContratacion")]
        public List<NominaReceptorSubContratacion> SubContratacion
        {
            get
            {
                return this.subContratacionField;
            }
            set
            {
                this.subContratacionField = value;
            }
        }
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Curp
        {
            get
            {
                return this.curpField;
            }
            set
            {
                this.curpField = value;
            }
        }
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string NumSeguridadSocial
        {
            get
            {
                return this.numSeguridadSocialField;
            }
            set
            {
                this.numSeguridadSocialField = value;
            }
        }
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public System.String FechaInicioRelLaboral
        {
            get
            {
                return this.fechaInicioRelLaboralField;
            }
            set
            {
                this.fechaInicioRelLaboralField = value;
            }
        }

        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool FechaInicioRelLaboralSpecified
        {
            get
            {
                return this.fechaInicioRelLaboralFieldSpecified;
            }
            set
            {
                this.fechaInicioRelLaboralFieldSpecified = value;
            }
        }
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Antigüedad
        {
            get
            {
                return this.antigüedadField;
            }
            set
            {
                this.antigüedadField = value;
            }
        }
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string TipoContrato
        {
            get
            {
                return this.tipoContratoField;
            }
            set
            {
                this.tipoContratoField = value;
            }
        }
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public NominaReceptorSindicalizado Sindicalizado
        {
            get
            {
                return this.sindicalizadoField;
            }
            set
            {
                this.sindicalizadoField = value;
            }
        }

        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool SindicalizadoSpecified
        {
            get
            {
                return this.sindicalizadoFieldSpecified;
            }
            set
            {
                this.sindicalizadoFieldSpecified = value;
            }
        }
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string TipoJornada
        {
            get
            {
                return this.tipoJornadaField;
            }
            set
            {
                this.tipoJornadaField = value;
            }
        }

        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool TipoJornadaSpecified
        {
            get
            {
                return this.tipoJornadaFieldSpecified;
            }
            set
            {
                this.tipoJornadaFieldSpecified = value;
            }
        }
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string TipoRegimen
        {
            get
            {
                return this.tipoRegimenField;
            }
            set
            {
                this.tipoRegimenField = value;
            }
        }
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string NumEmpleado
        {
            get
            {
                return this.numEmpleadoField;
            }
            set
            {
                this.numEmpleadoField = value;
            }
        }
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Departamento
        {
            get
            {
                return this.departamentoField;
            }
            set
            {
                this.departamentoField = value;
            }
        }
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Puesto
        {
            get
            {
                return this.puestoField;
            }
            set
            {
                this.puestoField = value;
            }
        }
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string RiesgoPuesto
        {
            get
            {
                return this.riesgoPuestoField;
            }
            set
            {
                this.riesgoPuestoField = value;
            }
        }

        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool RiesgoPuestoSpecified
        {
            get
            {
                return this.riesgoPuestoFieldSpecified;
            }
            set
            {
                this.riesgoPuestoFieldSpecified = value;
            }
        }
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string PeriodicidadPago
        {
            get
            {
                return this.periodicidadPagoField;
            }
            set
            {
                this.periodicidadPagoField = value;
            }
        }
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Banco
        {
            get
            {
                return this.bancoField;
            }
            set
            {
                this.bancoField = value;
            }
        }

        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool BancoSpecified
        {
            get
            {
                return this.bancoFieldSpecified;
            }
            set
            {
                this.bancoFieldSpecified = value;
            }
        }
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string CuentaBancaria
        {
            get
            {
                return this.cuentaBancariaField;
            }
            set
            {
                this.cuentaBancariaField = value;
            }
        }
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal SalarioBaseCotApor
        {
            get
            {
                return this.salarioBaseCotAporField;
            }
            set
            {
                this.salarioBaseCotAporField = value;
            }
        }

        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool SalarioBaseCotAporSpecified
        {
            get
            {
                return this.salarioBaseCotAporFieldSpecified;
            }
            set
            {
                this.salarioBaseCotAporFieldSpecified = value;
            }
        }
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal SalarioDiarioIntegrado
        {
            get
            {
                return this.salarioDiarioIntegradoField;
            }
            set
            {
                this.salarioDiarioIntegradoField = value;
            }
        }

        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool SalarioDiarioIntegradoSpecified
        {
            get
            {
                return this.salarioDiarioIntegradoFieldSpecified;
            }
            set
            {
                this.salarioDiarioIntegradoFieldSpecified = value;
            }
        }
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string ClaveEntFed
        {
            get
            {
                return this.claveEntFedField;
            }
            set
            {
                this.claveEntFedField = value;
            }
        }
    }
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.sat.gob.mx/nomina12")]
 
    public partial class NominaReceptorSubContratacion
    {

        private string rfcLaboraField;

        private decimal porcentajeTiempoField;
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string RfcLabora
        {
            get
            {
                return this.rfcLaboraField;
            }
            set
            {
                this.rfcLaboraField = value;
            }
        }
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal PorcentajeTiempo
        {
            get
            {
                return this.porcentajeTiempoField;
            }
            set
            {
                this.porcentajeTiempoField = value;
            }
        }
    }

    public enum c_TipoContrato
    {

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("01")]
        Item01=01,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("02")]
        Item02=02,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("03")]
        Item03=03,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("04")]
        Item04=04,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("05")]
        Item05=05,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("06")]
        Item06=06,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("07")]
        Item07=07,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("08")]
        Item08=08,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("09")]
        Item09=09,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("10")]
        Item10=10,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("99")]
        Item99=99,
    }

    public enum NominaReceptorSindicalizado
    {

        /// <comentarios/>
        Sí,

        /// <comentarios/>
        No,
    }

    public enum c_TipoJornada
    {

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("02")]
        Item02=02,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("03")]
        Item03=03,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("01")]
        Item01=01,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("04")]
        Item04=04,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("05")]
        Item05=05,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("06")]
        Item06=06,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("07")]
        Item07=07,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("08")]
        Item08=08,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("99")]
        Item99=99,
    }

    public enum c_TipoRegimen
    {

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("03")]
        Item03=03,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("02")]
        Item02=02,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("04")]
        Item04=04,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("05")]
        Item05=05,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("06")]
        Item06=06,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("07")]
        Item07=07,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("08")]
        Item08=08,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("09")]
        Item09=09,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("10")]
        Item10=10,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("11")]
        Item11=11,
        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("12")]
        Item12 = 12,
        [System.Xml.Serialization.XmlEnumAttribute("13")]
        Item13 = 13,
        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("99")]
        Item99=99,
    }

    public enum c_RiesgoPuesto
    {

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("1")]
        Item1=1,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("2")]
        Item2=2,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("3")]
        Item3=3,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("4")]
        Item4=4,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("5")]
        Item5=5,
    }

    public enum c_PeriodicidadPago
    {

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("01")]
        Item01=01,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("02")]
        Item02=02,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("03")]
        Item03=03,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("04")]
        Item04=04,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("05")]
        Item05=05,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("06")]
        Item06=06,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("07")]
        Item07=07,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("08")]
        Item08=08,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("09")]
        Item09=09,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("10")]
        Item10 = 10,
        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("99")]
        Item99=99,
    }

    public enum c_Banco
    {

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("002")]
         Item002=002,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("006")]
        Item006=006,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("009")]
        Item009=009,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("012")]
        Item012=012,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("014")]
        Item014=014,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("019")]
        Item019=019,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("021")]
        Item021=021,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("030")]
        Item030=030,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("032")]
        Item032=032,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("036")]
        Item036=036,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("037")]
        Item037=037,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("042")]
        Item042=042,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("044")]
        Item044=044,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("058")]
        Item058=058,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("059")]
        Item059=059,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("060")]
        Item060=060,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("062")]
        Item062=062,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("072")]
        Item072=072,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("102")]
        Item102=102,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("103")]
        Item103=103,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("106")]
        Item106=106,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("108")]
        Item108=108,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("110")]
        Item110=110,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("112")]
        Item112=112,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("113")]
        Item113=113,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("116")]
        Item116=116,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("124")]
        Item124=124,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("126")]
        Item126=126,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("127")]
        Item127=127,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("128")]
        Item128=128,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("129")]
        Item129=129,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("130")]
        Item130=130,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("131")]
        Item131=131,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("132")]
        Item132=132,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("133")]
        Item133=133,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("134")]
        Item134=134,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("135")]
        Item135=135,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("136")]
        Item136=136,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("137")]
        Item137=137,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("138")]
        Item138=138,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("139")]
        Item139=139,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("140")]
        Item140=140,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("141")]
        Item141=141,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("143")]
        Item143=143,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("145")]
        Item145=145,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("166")]
        Item166=166,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("168")]
        Item168=168,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("600")]
        Item600=600,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("601")]
        Item601=601,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("602")]
        Item602=602,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("605")]
        Item605=605,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("606")]
        Item606=606,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("607")]
        Item607=607,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("608")]
        Item608=608,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("610")]
        Item610=610,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("614")]
        Item614=614,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("615")]
        Item615=615,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("616")]
        Item616=616,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("617")]
        Item617=617,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("618")]
        Item618=618,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("619")]
        Item619=619,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("620")]
        Item620=620,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("621")]
        Item621=621,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("622")]
        Item622=622,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("623")]
        Item623=623,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("626")]
        Item626=626,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("627")]
        Item627=627,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("628")]
        Item628=628,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("629")]
        Item629=629,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("630")]
        Item630=630,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("631")]
        Item631=631,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("632")]
        Item632=632,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("633")]
        Item633=633,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("634")]
        Item634=634,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("636")]
        Item636=636,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("637")]
        Item637 = 637,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("638")]
        Item638=638,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("640")]
        Item640=640,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("642")]
        Item642=642,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("646")]
        Item646=646,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("647")]
        Item647=647,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("648")]
        Item648=648,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("649")]
        Item649=649,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("651")]
        Item651=651,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("652")]
        Item652=652,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("653")]
        Item653=653,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("655")]
        Item655=655,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("656")]
        Item656=656,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("659")]
        Item659=659,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("901")]
        Item901=901,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("902")]
        Item902=902,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("670")]
        Item670=670,
    }

    public enum c_Estado
    {
        [System.Xml.Serialization.XmlEnumAttribute("AGU")]
        /// <comentarios/>
        AGU,

        [System.Xml.Serialization.XmlEnumAttribute("BCN")]
      
        /// <comentarios/>
        BCN,

        [System.Xml.Serialization.XmlEnumAttribute("BCS")]
        BCS,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("CAM")]
        CAM,

        /// <comentarios/>
        CHP,

        /// <comentarios/>
        CHH,

        /// <comentarios/>
        COA,

        /// <comentarios/>
        COL,

        /// <comentarios/>
        DIF,

        /// <comentarios/>
        DUR,

        /// <comentarios/>
        GUA,

        /// <comentarios/>
        GRO,

        /// <comentarios/>
        HID,

        /// <comentarios/>
        JAL,

        /// <comentarios/>
        MEX,

        /// <comentarios/>
        MIC,

        /// <comentarios/>
        MOR,

        /// <comentarios/>
        NAY,

        /// <comentarios/>
        NLE,

        /// <comentarios/>
        OAX,

        /// <comentarios/>
        PUE,

        /// <comentarios/>
        QUE,

        /// <comentarios/>
        ROO,

        /// <comentarios/>
        SLP,

        /// <comentarios/>
        SIN,

        /// <comentarios/>
        SON,

        /// <comentarios/>
        TAB,

        /// <comentarios/>
        TAM,

        /// <comentarios/>
        TLA,

        /// <comentarios/>
        VER,

        /// <comentarios/>
        YUC,

        /// <comentarios/>
        ZAC,

        /// <comentarios/>
        AL,

        /// <comentarios/>
        AK,

        /// <comentarios/>
        AZ,

        /// <comentarios/>
        AR,

        /// <comentarios/>
        CA,

        /// <comentarios/>
        NC,

        /// <comentarios/>
        SC,

        /// <comentarios/>
        CO,

        /// <comentarios/>
        CT,

        /// <comentarios/>
        ND,

        /// <comentarios/>
        SD,

        /// <comentarios/>
        DE,

        /// <comentarios/>
        FL,

        /// <comentarios/>
        GA,

        /// <comentarios/>
        HI,

        /// <comentarios/>
        ID,

        /// <comentarios/>
        IL,

        /// <comentarios/>
        IN,

        /// <comentarios/>
        IA,

        /// <comentarios/>
        KS,

        /// <comentarios/>
        KY,

        /// <comentarios/>
        LA,

        /// <comentarios/>
        ME,

        /// <comentarios/>
        MD,

        /// <comentarios/>
        MA,

        /// <comentarios/>
        MI,

        /// <comentarios/>
        MN,

        /// <comentarios/>
        MS,

        /// <comentarios/>
        MO,

        /// <comentarios/>
        MT,

        /// <comentarios/>
        NE,

        /// <comentarios/>
        NV,

        /// <comentarios/>
        NJ,

        /// <comentarios/>
        NY,

        /// <comentarios/>
        NH,

        /// <comentarios/>
        NM,

        /// <comentarios/>
        OH,

        /// <comentarios/>
        OK,

        /// <comentarios/>
        OR,

        /// <comentarios/>
        PA,

        /// <comentarios/>
        RI,

        /// <comentarios/>
        TN,

        /// <comentarios/>
        TX,

        /// <comentarios/>
        UT,

        /// <comentarios/>
        VT,

        /// <comentarios/>
        VA,

        /// <comentarios/>
        WV,

        /// <comentarios/>
        WA,

        /// <comentarios/>
        WI,

        /// <comentarios/>
        WY,

        /// <comentarios/>
        ON,

        /// <comentarios/>
        QC,

        /// <comentarios/>
        NS,

        /// <comentarios/>
        NB,

        /// <comentarios/>
        MB,

        /// <comentarios/>
        BC,

        /// <comentarios/>
        PE,

        /// <comentarios/>
        SK,

        /// <comentarios/>
        AB,

        /// <comentarios/>
        NL,

        /// <comentarios/>
        NT,

        /// <comentarios/>
        YT,

        /// <comentarios/>
        UN,
    }

    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.sat.gob.mx/nomina12")]
  
    public partial class NominaPercepciones
    {

        private List<NominaPercepcionesPercepcion> percepcionField;

        private NominaPercepcionesJubilacionPensionRetiro jubilacionPensionRetiroField;

        private NominaPercepcionesSeparacionIndemnizacion separacionIndemnizacionField;

        private decimal totalSueldosField;

        private bool totalSueldosFieldSpecified;

        private decimal totalSeparacionIndemnizacionField;

        private bool totalSeparacionIndemnizacionFieldSpecified;

        private decimal totalJubilacionPensionRetiroField;

        private bool totalJubilacionPensionRetiroFieldSpecified;

        private decimal totalGravadoField;

        private decimal totalExentoField;

        public NominaPercepciones()
        {
           // this.separacionIndemnizacionField = new NominaPercepcionesSeparacionIndemnizacion();
           // this.jubilacionPensionRetiroField = new NominaPercepcionesJubilacionPensionRetiro();
           // this.percepcionField = new List<NominaPercepcionesPercepcion>();
        }
        //[System.Xml.Serialization.XmlArrayItemAttribute("Percepcion", IsNullable = false)]
        [XmlElement("Percepcion")]
        public List<NominaPercepcionesPercepcion> Percepcion
        {
            get
            {
                return this.percepcionField;
            }
            set
            {
                this.percepcionField = value;
            }
        }

        public NominaPercepcionesJubilacionPensionRetiro JubilacionPensionRetiro
        {
            get
            {
                return this.jubilacionPensionRetiroField;
            }
            set
            {
                this.jubilacionPensionRetiroField = value;
            }
        }

        public NominaPercepcionesSeparacionIndemnizacion SeparacionIndemnizacion
        {
            get
            {
                return this.separacionIndemnizacionField;
            }
            set
            {
                this.separacionIndemnizacionField = value;
            }
        }
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal TotalSueldos
        {
            get
            {
                return this.totalSueldosField;
            }
            set
            {
                this.totalSueldosField = value;
            }
        }

        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool TotalSueldosSpecified
        {
            get
            {
                return this.totalSueldosFieldSpecified;
            }
            set
            {
                this.totalSueldosFieldSpecified = value;
            }
        }
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal TotalSeparacionIndemnizacion
        {
            get
            {
                return this.totalSeparacionIndemnizacionField;
            }
            set
            {
                this.totalSeparacionIndemnizacionField = value;
            }
        }

        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool TotalSeparacionIndemnizacionSpecified
        {
            get
            {
                return this.totalSeparacionIndemnizacionFieldSpecified;
            }
            set
            {
                this.totalSeparacionIndemnizacionFieldSpecified = value;
            }
        }
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal TotalJubilacionPensionRetiro
        {
            get
            {
                return this.totalJubilacionPensionRetiroField;
            }
            set
            {
                this.totalJubilacionPensionRetiroField = value;
            }
        }

        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool TotalJubilacionPensionRetiroSpecified
        {
            get
            {
                return this.totalJubilacionPensionRetiroFieldSpecified;
            }
            set
            {
                this.totalJubilacionPensionRetiroFieldSpecified = value;
            }
        }
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal TotalGravado
        {
            get
            {
                return this.totalGravadoField;
            }
            set
            {
                this.totalGravadoField = value;
            }
        }
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal TotalExento
        {
            get
            {
                return this.totalExentoField;
            }
            set
            {
                this.totalExentoField = value;
            }
        }
    }
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.sat.gob.mx/nomina12")]
 
    public partial class NominaPercepcionesPercepcion
    {

        private NominaPercepcionesPercepcionAccionesOTitulos accionesOTitulosField;
       
        private List<NominaPercepcionesPercepcionHorasExtra> horasExtraField;

        private string tipoPercepcionField;

        private string claveField;

        private string conceptoField;

        private decimal importeGravadoField;

        private decimal importeExentoField;


        public NominaPercepcionesPercepcionAccionesOTitulos AccionesOTitulos
        {
            get
            {
                return this.accionesOTitulosField;
            }
            set
            {
                this.accionesOTitulosField = value;
            }
        }
         [XmlElement("HorasExtra")]
        public List<NominaPercepcionesPercepcionHorasExtra> HorasExtra
        {
            get
            {
                return this.horasExtraField;
            }
            set
            {
                this.horasExtraField = value;
            }
        }
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string TipoPercepcion
        {
            get
            {
                return this.tipoPercepcionField;
            }
            set
            {
                this.tipoPercepcionField = value;
            }
        }
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Clave
        {
            get
            {
                return this.claveField;
            }
            set
            {
                this.claveField = value;
            }
        }
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Concepto
        {
            get
            {
                return this.conceptoField;
            }
            set
            {
                this.conceptoField = value;
            }
        }
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal ImporteGravado
        {
            get
            {
                return this.importeGravadoField;
            }
            set
            {
                this.importeGravadoField= value;
            }
        }
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal ImporteExento
        {
            get
            {
                return this.importeExentoField;
            }
            set
            {
                this.importeExentoField = value;
            }
        }
    }

    public partial class NominaPercepcionesPercepcionAccionesOTitulos
    {

        private decimal valorMercadoField;

        private decimal precioAlOtorgarseField;
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal ValorMercado
        {
            get
            {
                return this.valorMercadoField;
            }
            set
            {
                this.valorMercadoField = value;
            }
        }
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal PrecioAlOtorgarse
        {
            get
            {
                return this.precioAlOtorgarseField;
            }
            set
            {
                this.precioAlOtorgarseField = value;
            }
        }
    }
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.sat.gob.mx/nomina12")]
    public partial class NominaPercepcionesPercepcionHorasExtra
    {

        private int diasField;

        private string tipoHorasField;

        private int horasExtraField;

        private decimal importePagadoField;
         [System.Xml.Serialization.XmlAttributeAttribute()]
        public int Dias
        {
            get
            {
                return this.diasField;
            }
            set
            {
                this.diasField = value;
            }
        }
         [System.Xml.Serialization.XmlAttributeAttribute()]
        public string TipoHoras
        {
            get
            {
                return this.tipoHorasField;
            }
            set
            {
                this.tipoHorasField = value;
            }
        }
         [System.Xml.Serialization.XmlAttributeAttribute()]
        public int HorasExtra
        {
            get
            {
                return this.horasExtraField;
            }
            set
            {
                this.horasExtraField = value;
            }
        }
         [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal ImportePagado
        {
            get
            {
                return this.importePagadoField;
            }
            set
            {
                this.importePagadoField = value;
            }
        }
    }

    public enum c_TipoHoras
    {

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("01")]
        Item01=01,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("02")]
        Item02=02,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("03")]
        Item03=03,
    }

    public enum c_TipoPercepcion
    {

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("001")]
        Item001=001,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("002")]
        Item002=002,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("003")]
        Item003=003,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("004")]
        Item004=004,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("005")]
        Item005=005,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("006")]
        Item006=006,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("009")]
        Item009=009,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("010")]
        Item010=010,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("011")]
        Item011=011,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("012")]
        Item012=012,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("013")]
        Item013=013,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("014")]
        Item014=014,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("015")]
        Item015=015,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("019")]
        Item019=019,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("020")]
        Item020=020,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("021")]
        Item021=021,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("022")]
        Item022=022,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("023")]
        Item023=023,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("024")]
        Item024=024,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("025")]
        Item025=025,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("026")]
        Item026=026,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("027")]
        Item027=027,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("028")]
        Item028=028,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("029")]
        Item029=029,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("030")]
        Item030=030,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("031")]
        Item031=031,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("032")]
        Item032=032,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("033")]
        Item033=033,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("034")]
        Item034=034,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("035")]
        Item035=35,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("036")]
        Item036=036,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("037")]
        Item037=037,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("038")]
        Item038=038,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("039")]
        Item039=039,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("044")]
        Item044=044,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("045")]
        Item045=045,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("046")]
        Item046=046,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("047")]
        Item047=047,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("048")]
        Item048=048,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("049")]
        Item049=049,

        [System.Xml.Serialization.XmlEnumAttribute("050")]
        Item050 = 050,
    }

    public partial class NominaPercepcionesJubilacionPensionRetiro
    {

        private decimal totalUnaExhibicionField;

        private bool totalUnaExhibicionFieldSpecified;

        private decimal totalParcialidadField;

        private bool totalParcialidadFieldSpecified;

        private decimal montoDiarioField;

        private bool montoDiarioFieldSpecified;

        private decimal ingresoAcumulableField;

        private decimal ingresoNoAcumulableField;
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal TotalUnaExhibicion
        {
            get
            {
                return this.totalUnaExhibicionField;
            }
            set
            {
                this.totalUnaExhibicionField = value;
            }
        }

        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool TotalUnaExhibicionSpecified
        {
            get
            {
                return this.totalUnaExhibicionFieldSpecified;
            }
            set
            {
                this.totalUnaExhibicionFieldSpecified = value;
            }
        }
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal TotalParcialidad
        {
            get
            {
                return this.totalParcialidadField;
            }
            set
            {
                this.totalParcialidadField = value;
            }
        }

        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool TotalParcialidadSpecified
        {
            get
            {
                return this.totalParcialidadFieldSpecified;
            }
            set
            {
                this.totalParcialidadFieldSpecified = value;
            }
        }
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal MontoDiario
        {
            get
            {
                return this.montoDiarioField;
            }
            set
            {
                this.montoDiarioField = value;
            }
        }

        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool MontoDiarioSpecified
        {
            get
            {
                return this.montoDiarioFieldSpecified;
            }
            set
            {
                this.montoDiarioFieldSpecified = value;
            }
        }
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal IngresoAcumulable
        {
            get
            {
                return this.ingresoAcumulableField;
            }
            set
            {
                this.ingresoAcumulableField = value;
            }
        }
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal IngresoNoAcumulable
        {
            get
            {
                return this.ingresoNoAcumulableField;
            }
            set
            {
                this.ingresoNoAcumulableField = value;
            }
        }
    }

    public partial class NominaPercepcionesSeparacionIndemnizacion
    {

        private decimal totalPagadoField;

        private int numAñosServicioField;

        private decimal ultimoSueldoMensOrdField;

        private decimal ingresoAcumulableField;

        private decimal ingresoNoAcumulableField;
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal TotalPagado
        {
            get
            {
                return this.totalPagadoField;
            }
            set
            {
                this.totalPagadoField = value;
            }
        }
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public int NumAñosServicio
        {
            get
            {
                return this.numAñosServicioField;
            }
            set
            {
                this.numAñosServicioField = value;
            }
        }
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal UltimoSueldoMensOrd
        {
            get
            {
                return this.ultimoSueldoMensOrdField;
            }
            set
            {
                this.ultimoSueldoMensOrdField = value;
            }
        }
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal IngresoAcumulable
        {
            get
            {
                return this.ingresoAcumulableField;
            }
            set
            {
                this.ingresoAcumulableField = value;
            }
        }
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal IngresoNoAcumulable
        {
            get
            {
                return this.ingresoNoAcumulableField;
            }
            set
            {
                this.ingresoNoAcumulableField = value;
            }
        }
    }
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.sat.gob.mx/nomina12")]
 
    public partial class NominaDeducciones
    {

        private List<NominaDeduccionesDeduccion> deduccionField;

        private decimal totalOtrasDeduccionesField;

        private bool totalOtrasDeduccionesFieldSpecified;

        private decimal totalImpuestosRetenidosField;

        private bool totalImpuestosRetenidosFieldSpecified;

        public NominaDeducciones()
        {
            //this.deduccionField = new List<NominaDeduccionesDeduccion>();
        }
         [XmlElement("Deduccion")]
        public List<NominaDeduccionesDeduccion> Deduccion
        {
            get
            {
                return this.deduccionField;
            }
            set
            {
                this.deduccionField = value;
            }
        }
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal TotalOtrasDeducciones
        {
            get
            {
                return this.totalOtrasDeduccionesField;
            }
            set
            {
                this.totalOtrasDeduccionesField = value;
            }
        }

        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool TotalOtrasDeduccionesSpecified
        {
            get
            {
                return this.totalOtrasDeduccionesFieldSpecified;
            }
            set
            {
                this.totalOtrasDeduccionesFieldSpecified = value;
            }
        }
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal TotalImpuestosRetenidos
        {
            get
            {
                return this.totalImpuestosRetenidosField;
            }
            set
            {
                this.totalImpuestosRetenidosField = value;
            }
        }

        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool TotalImpuestosRetenidosSpecified
        {
            get
            {
                return this.totalImpuestosRetenidosFieldSpecified;
            }
            set
            {
                this.totalImpuestosRetenidosFieldSpecified = value;
            }
        }
    }
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.sat.gob.mx/nomina12")]
 
    public partial class NominaDeduccionesDeduccion
    {

        private string tipoDeduccionField;

        private string claveField;

        private string conceptoField;

        private decimal importeField;
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string TipoDeduccion
        {
            get
            {
                return this.tipoDeduccionField;
            }
            set
            {
                this.tipoDeduccionField = value;
            }
        }
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Clave
        {
            get
            {
                return this.claveField;
            }
            set
            {
                this.claveField = value;
            }
        }
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Concepto
        {
            get
            {
                return this.conceptoField;
            }
            set
            {
                this.conceptoField = value;
            }
        }
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

    public enum c_TipoDeduccion
    {

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("001")]
        Item001=001,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("002")]
        Item002=002,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("003")]
        Item003=003,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("004")]
        Item004=004,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("005")]
        Item005=005,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("006")]
        Item006=006,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("007")]
        Item007=007,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("008")]
        Item008=008,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("009")]
        Item009=009,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("010")]
        Item010=010,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("011")]
        Item011=011,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("012")]
        Item012=012,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("013")]
        Item013=013,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("014")]
        Item014=014,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("015")]
        Item015=015,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("016")]
        Item016=016,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("017")]
        Item017=017,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("018")]
        Item018=018,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("019")]
        Item019=019,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("020")]
        Item020=020,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("021")]
        Item021=021,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("022")]
        Item022=022,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("023")]
        Item023=023,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("024")]
        Item024 = 024,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("025")]
        Item025 = 025,
        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("026")]
        Item026 = 026,
        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("027")]
        Item027 = 027,
        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("028")]
        Item028 = 028,
        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("029")]
        Item029 = 029,
        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("030")]
        Item030 = 030,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("031")]
        Item031 = 031,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("032")]
        Item032 = 032,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("033")]
        Item033 = 033,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("034")]
        Item034 = 034,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("035")]
        Item035 = 035,
        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("036")]
        Item036 = 036,
        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("037")]
        Item037 = 037,
        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("038")]
        Item038 = 038,
        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("039")]
        Item039 = 039,
        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("040")]
        Item040 = 040,

        [System.Xml.Serialization.XmlEnumAttribute("041")]
        Item041 = 041,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("042")]
        Item042 = 042,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("043")]
        Item043 = 043,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("044")]
        Item044 = 044,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("045")]
        Item045 = 045,
        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("046")]
        Item046 = 046,
        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("047")]
        Item047 = 047,
        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("048")]
        Item048 = 048,
        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("049")]
        Item049 = 049,
        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("050")]
        Item050 = 050,

        [System.Xml.Serialization.XmlEnumAttribute("051")]
        Item051 = 051,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("052")]
        Item052 = 052,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("053")]
        Item053 = 053,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("054")]
        Item054 = 054,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("055")]
        Item055 = 055,
        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("056")]
        Item056 = 056,
        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("057")]
        Item057 = 057,
        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("058")]
        Item058 = 058,
        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("059")]
        Item059 = 059,
        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("060")]
        Item060 = 060,

        [System.Xml.Serialization.XmlEnumAttribute("061")]
        Item061 = 061,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("062")]
        Item062 = 062,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("063")]
        Item063 = 063,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("064")]
        Item064 = 064,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("065")]
        Item065 = 065,
        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("066")]
        Item066 = 066,
        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("067")]
        Item067 = 067,
        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("068")]
        Item068 = 068,
        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("069")]
        Item069 = 069,
        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("070")]
        Item070 = 070,

        [System.Xml.Serialization.XmlEnumAttribute("071")]
        Item071 = 071,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("072")]
        Item072 = 072,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("073")]
        Item073 = 073,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("074")]
        Item074 = 074,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("075")]
        Item075 = 075,
        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("076")]
        Item076 = 076,
        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("077")]
        Item077 = 077,
        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("078")]
        Item078 = 078,
        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("079")]
        Item079 = 079,
        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("080")]
        Item080 = 080,

        [System.Xml.Serialization.XmlEnumAttribute("081")]
        Item081 = 081,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("082")]
        Item082 = 082,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("083")]
        Item083 = 083,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("084")]
        Item084 = 084,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("085")]
        Item085 = 085,
        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("086")]
        Item086 = 086,
        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("087")]
        Item087 = 087,
        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("088")]
        Item088 = 088,
        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("089")]
        Item089 = 089,
        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("090")]
        Item090 = 090,

        [System.Xml.Serialization.XmlEnumAttribute("091")]
        Item091 = 091,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("092")]
        Item092 = 092,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("093")]
        Item093 = 093,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("094")]
        Item094 = 094,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("095")]
        Item095 = 095,
        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("096")]
        Item096 = 096,
        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("097")]
        Item097 = 097,
        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("098")]
        Item098 = 098,
        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("099")]
        Item099 = 099,
        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("100")]
        Item100 = 100,
        [System.Xml.Serialization.XmlEnumAttribute("101")]
        Item101 = 101,
    }
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.sat.gob.mx/nomina12")]
 
    public partial class NominaOtroPago
    {

        private NominaOtroPagoSubsidioAlEmpleo subsidioAlEmpleoField;

        private NominaOtroPagoCompensacionSaldosAFavor compensacionSaldosAFavorField;

        private string tipoOtroPagoField;

        private string claveField;

        private string conceptoField;

        private decimal importeField;

        public NominaOtroPago()
        {
            //this.compensacionSaldosAFavorField = new NominaOtroPagoCompensacionSaldosAFavor();
           // this.subsidioAlEmpleoField = new NominaOtroPagoSubsidioAlEmpleo();
        }

        public NominaOtroPagoSubsidioAlEmpleo SubsidioAlEmpleo
        {
            get
            {
                return this.subsidioAlEmpleoField;
            }
            set
            {
                this.subsidioAlEmpleoField = value;
            }
        }

        public NominaOtroPagoCompensacionSaldosAFavor CompensacionSaldosAFavor
        {
            get
            {
                return this.compensacionSaldosAFavorField;
            }
            set
            {
                this.compensacionSaldosAFavorField = value;
            }
        }
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string TipoOtroPago
        {
            get
            {
                return this.tipoOtroPagoField;
            }
            set
            {
                this.tipoOtroPagoField = value;
            }
        }
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Clave
        {
            get
            {
                return this.claveField;
            }
            set
            {
                this.claveField = value;
            }
        }
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Concepto
        {
            get
            {
                return this.conceptoField;
            }
            set
            {
                this.conceptoField = value;
            }
        }
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

    public partial class NominaOtroPagoSubsidioAlEmpleo
    {

        private decimal subsidioCausadoField;
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal SubsidioCausado
        {
            get
            {
                return this.subsidioCausadoField;
            }
            set
            {
                this.subsidioCausadoField = value;
            }
        }
    }

    public partial class NominaOtroPagoCompensacionSaldosAFavor
    {

        private decimal saldoAFavorField;

        private short añoField;

        private decimal remanenteSalFavField;
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal SaldoAFavor
        {
            get
            {
                return this.saldoAFavorField;
            }
            set
            {
                this.saldoAFavorField = value;
            }
        }
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public short Año
        {
            get
            {
                return this.añoField;
            }
            set
            {
                this.añoField = value;
            }
        }
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal RemanenteSalFav
        {
            get
            {
                return this.remanenteSalFavField;
            }
            set
            {
                this.remanenteSalFavField = value;
            }
        }
    }

    public enum c_TipoOtroPago
    {

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("001")]
        Item001=001,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("002")]
        Item002=002,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("003")]
        Item003=003,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("004")]
        Item004=004,
        [System.Xml.Serialization.XmlEnumAttribute("005")]
        Item005 = 005,
        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("999")]
        Item999=999,
    }
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.sat.gob.mx/nomina12")]
 
    public partial class NominaIncapacidad
    {

        private int diasIncapacidadField;

        private string tipoIncapacidadField;

        private decimal importeMonetarioField;

        private bool importeMonetarioFieldSpecified;
         [System.Xml.Serialization.XmlAttributeAttribute()]
        public int DiasIncapacidad
        {
            get
            {
                return this.diasIncapacidadField;
            }
            set
            {
                this.diasIncapacidadField = value;
            }
        }
         [System.Xml.Serialization.XmlAttributeAttribute()]
        public string TipoIncapacidad
        {
            get
            {
                return this.tipoIncapacidadField;
            }
            set
            {
                this.tipoIncapacidadField = value;
            }
        }
         [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal ImporteMonetario
        {
            get
            {
                return this.importeMonetarioField;
            }
            set
            {
                this.importeMonetarioField = value;
            }
        }

        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool ImporteMonetarioSpecified
        {
            get
            {
                return this.importeMonetarioFieldSpecified;
            }
            set
            {
                this.importeMonetarioFieldSpecified = value;
            }
        }
    }

    public enum c_TipoIncapacidad
    {

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("01")]
        Item01=01,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("02")]
        Item02=02,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("03")]
        Item03=03,
    }

    public enum c_TipoNomina
    {

        /// <comentarios/>
        O,

        /// <comentarios/>
        E,
    }
}

    