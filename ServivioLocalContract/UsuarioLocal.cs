using System;
using System.Runtime.Serialization;

namespace ServicioLocalContract
{
    [Serializable]
    public class UsuarioLocal
    {
        [DataMemberAttribute]
        public string RazonSocial { get; set; }
        [DataMemberAttribute]
        public string CambiarPassword { get; set; }
        [DataMemberAttribute]
        public string UserName { get; set; }
        [DataMemberAttribute]
        public string Perfil { get; set; }
        public Guid UserId { get; set; }
        [DataMemberAttribute]
        public string Email { get; set; }
        [DataMemberAttribute]
        public bool Bloqueado { get; set; }
        [DataMemberAttribute]
        public string NombreCompleto { get; set; }
        [DataMemberAttribute]
        public string Iniciales { get; set; }
        [DataMemberAttribute]
        public string IsLockedOut
        {
            get { return Bloqueado ? "Bloqueado" : "Ok"; }
        }
        [DataMemberAttribute]
        public string PasswordS
        {
            get { return "Cambiar..."; }
        }

        [DataMemberAttribute]
        public string Desbloquea
        {
            get { return "Desbloquear..."; }
        }
    }
}
