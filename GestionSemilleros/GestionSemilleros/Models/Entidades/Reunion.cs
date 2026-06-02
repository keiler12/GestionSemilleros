using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionSemilleros.Models.Entidades
{
    public class Reunion
    {
        [Key]
        public decimal IdReunion { get; set; }
        public decimal IdUsuario { get; set; }
        public string TipoReunion { get; set; }
        public TimeSpan HoraReunion { get; set; }
        public string MotivoReunion { get; set; }
        public DateTime FechaReunion { get; set; }

        [ForeignKey("IdUsuario")]
        public virtual Usuario Usuario { get; set; }
    }
}