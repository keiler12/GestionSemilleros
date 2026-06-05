using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionSemilleros.Models.Entidades
{
    [Table("Usuario")]
    public class Usuario
    {
        [Key]
        public int IdUsuario { get; set; }
        public int? IdSemillero { get; set; }
        public string ContraseñaUsuario { get; set; }
        public string NombresUsuario { get; set; }
        public string RolUsuario { get; set; }
        public int TelefonoUsuario { get; set; }
        public string CorreoUsuario { get; set; }
        public int EdadUsuario { get; set; }
        public string GeneroUsuario { get; set; }
        public string EstadoUsuario { get; set; }

        [ForeignKey("IdSemillero")]
        public virtual Semillero Semillero { get; set; }
        public virtual ICollection<Reunion> Reuniones { get; set; }
        
    }
}