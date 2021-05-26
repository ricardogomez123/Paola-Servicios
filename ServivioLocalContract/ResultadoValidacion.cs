using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;


namespace ServicioLocalContract
{
    [DataContract]
    public class Validacion
    {
        [DataMember]
        public string Error { get; set; }
        [DataMember]
        public string Descripcion { get; set; }
        [DataMember]
        public bool Valido { get; set; }
    }
    [DataContract]
    public class ResultadoValidacion
    {
        [DataMember]
        public bool Valido { get; set; }
        [DataMember]
        public List<Validacion> Detalles { get; set; }
        [DataMember]
        public ValidadorContract Entrada { get; set; }
    }
}
