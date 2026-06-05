using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionSemilleros.Models.Entidades
{
    [Table("Reunion")]
    public class Reunion
    {
        [Key]
        public int IdReunion { get; set; }
        public int IdUsuario { get; set; }
        public string TipoReunion { get; set; }
        public TimeSpan HoraReunion { get; set; }
        public string MotivoReunion { get; set; }
        public DateTime FechaReunion { get; set; }

        [ForeignKey("IdUsuario")]
        public virtual Usuario Usuario { get; set; }
    }
}