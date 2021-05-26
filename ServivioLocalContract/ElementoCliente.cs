using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServicioLocalContract
{
    public class ElementoCliente
    {
        public int IdSistema { get; set; }
        public string RazonSocial { get; set; }
        public string Rfc { get; set; }
        public int Contratados { get; set; }
        public int Comsumidos { get; set; }
        public double Porcentaje { get; set; }
        public int Cancelados { get; set; }
    }
}
