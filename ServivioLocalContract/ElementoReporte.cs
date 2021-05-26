using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServicioLocalContract
{
    public class ElementoReporte
    {
        public string Rfc { get; set; }
        public string Cliente { get; set; }
        public long Emitidos { get; set; }
        public long Cancelados { get; set; } 
    }
}
