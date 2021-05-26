using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace ServicioLocalContract
{
    [Serializable]
    [DataContract]
    public class Percepcion
    {
        [DataMember]
        public int TipoPercepcion { get; set; }
        [DataMember]
        public string Clave { get; set; }

        [DataMember]
        public string Concepto { get; set; }
        [DataMember]
        public decimal ImporteGravado { get; set; }

        [DataMember]
        public decimal ImporteExento { get; set; }
    }
    [Serializable]
    [DataContract]
    public class Percepciones
    {
        [DataMember]
        public List<Percepcion> Percepcion { get; set; }

        [DataMember]
        public decimal TotalGravado { get; set; }

        [DataMember]
        public decimal TotalExento { get; set; }
    }
    [Serializable]
    [DataContract]
    public partial class Deduccion
    {
        [DataMember]
        public int TipoDeduccion { get; set; }

        [DataMember]
        public string Clave { get; set; }

        [DataMember]
        public string Concepto { get; set; }

        [DataMember]
        public decimal ImporteGravado { get; set; }

        [DataMember]
        public decimal ImporteExento { get; set; }
    }
    [Serializable]
    [DataContract]
    public class Deducciones
    {
        [DataMember]
        public List<Deduccion> Deduccion { get; set; }

        [DataMember]
        public decimal TotalGravado { get; set; }

        [DataMember]
        public decimal TotalExento { get; set; }
    }
    [Serializable]
    [DataContract]
    public class Incapacidad
    {
        
        [DataMember]
        public decimal DiasIncapacidad { get; set; }

        
        [DataMember]
        public int TipoIncapacidad { get; set; }

        
        [DataMember]
        public decimal Descuento { get; set; }
    }


    public enum TipoHoras
    {
        Dobles,
        Triples,
    }
    [Serializable]
    [DataContract]
    public class HorasExtra
    {
        [DataMember]
        public int Dias { get; set; }

        [DataMember]
        public TipoHoras TipoHoras { get; set; }
        [DataMember]
        public int NumeroHorasExtra { get; set; }
        [DataMember]
        public decimal ImportePagado { get; set; }
    }
    [Serializable]
    [DataContract]
    public class NominaDto
    {
        public NominaDto()
        {
            this.Version = "1.1";
        }
        [DataMember]
        public Percepciones Percepciones { get; set; }
        [DataMember]
        public Deducciones Deducciones { get; set; }
        [DataMember]
        public List<Incapacidad> Incapacidades { get; set; }
        [DataMember]
        public HorasExtra[] HorasExtras { get; set; }
        [DataMember]
        public string Version { get; set; }
        [DataMember]
        public string RegistroPatronal { get; set; }
        [DataMember]
        public string NumEmpleado { get; set; }
        [DataMember]
        public string CURP { get; set; }
        [DataMember]
        public int TipoRegimen { get; set; }
        [DataMember]
        public string NumSeguridadSocial { get; set; }
        [DataMember]
        public DateTime FechaPago { get; set; }
        [DataMember]
        public DateTime FechaInicialPago { get; set; }
        [DataMember]
        public DateTime FechaFinalPago { get; set; }
        [DataMember]
        public decimal NumDiasPagados { get; set; }
        [DataMember]
        public string Departamento { get; set; }
        [DataMember]
        public string CLABE { get; set; }
        [DataMember]
        public int Banco { get; set; }
        [DataMember]
        public bool BancoSpecified { get; set; }
        [DataMember]
        public DateTime FechaInicioRelLaboral { get; set; }
        [DataMember]
        public bool FechaInicioRelLaboralSpecified { get; set; }
        [DataMember]
        public int Antiguedad { get; set; }
        [DataMember]
        public bool AntiguedadSpecified { get; set; }
        [DataMember]
        public string Puesto { get; set; }
        [DataMember]
        public string TipoContrato { get; set; }
        [DataMember]
        public string TipoJornada { get; set; }
        [DataMember]
        public string PeriodicidadPago { get; set; }
        [DataMember]
        public decimal SalarioBaseCotApor { get; set; }
        [DataMember]
        public bool SalarioBaseCotAporSpecified { get; set; }
        [DataMember]
        public int RiesgoPuesto { get; set; }
        [DataMember]
        public bool RiesgoPuestoSpecified { get; set; }
        [DataMember]
        public decimal SalarioDiarioIntegrado { get; set; }
        [DataMember]
        public bool SalarioDiarioIntegradoSpecified { get; set; }
    }
}
