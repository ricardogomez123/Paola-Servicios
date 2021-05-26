using System.Runtime.Serialization;

namespace ServicioLocalContract
{
    public partial class vClientesPromotores
    {
        private string _quitar = "Eliminar";

        [DataMemberAttribute()]
        public string Quitar
        {
            get { return _quitar; }
            set { _quitar = value; }
        }
    }


    public partial class clientes
    {
        

        public override string ToString()
        {
            return this.RFC + "|" + this.RazonSocial;
        }


    }
}
