using System.Runtime.Serialization;

namespace ServicioLocalContract
{
    public partial class vventas
    {


        public string StatusFactura
        {
            get
            {
                if (Cancelado.HasValue && Cancelado.Value > 0) return "Cancelado";
                if (FechaPago.HasValue) return "Pagado";
                else return "Pendiente";
            }
        }

        public override string ToString()
        {
            
            return this.Folio + "|" + this.Cliente + "|" + this.Fecha + "|" + this.Importe +"|" +this.Proyecto + "|" + this.Usuario +"|" + this.FolioPreFactura + "|" + this.Nombre +"|" + this.EmpresaEmisora;
        }

        public string StrCancelado { get { return ((Cancelado.HasValue && Cancelado == 1) ? "Cancelado" : "Vigente"); } }


        [DataMember]
        public decimal ImporteAplicar { get; set; }
        [DataMember]
        public decimal NuevoSaldo { get; set; }


        public decimal Res
        {
            get { return this.Importe - (this.Pagado.HasValue ? this.Pagado.Value : 0); }
        }

        public bool Aplicar { get; set; }

        public string Pdf
        {
            get { return "Pdf"; }
        }

        public string Enviar
        {
            get { return "Enviar"; }
        }

        public string Xml
        {
            get { return "Xml"; }
        }

        public string Cancelar
        {
            get { return (Cancelado.HasValue && Cancelado == 0 )? "Cancelar" : "Acuse de Cancelación" ; }
        }
    }
}
