using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace ServicioLocalContract
{
    public partial class empresa
    {
        private string editar = "Editar";
        [DataMemberAttribute()]
        public string Editar
        {
            get { return editar; }
            set { editar = value; }
        }
        private string sucursales = "Sucursales";
        [DataMemberAttribute()]
        public string Sucursales
        {
            get { return sucursales; }
            set { sucursales = value; }
        }
        private string conceptos = "Conceptos";
        [DataMemberAttribute()]
        public string Conceptos
        {
            get { return conceptos; }
            set { conceptos = value; }
        }

        [DataMemberAttribute()]
        public string VencimientoCert { get; set; }

        

    }
}
