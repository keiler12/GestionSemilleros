using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionSemilleros.Models.Entidades
{
    [Table("Reunion")]
    public class Reunion
    {
        [Key]
        public int IdReunion { get; set; }
        public string TipoReunion { get; set; }
        public TimeSpan HoraReunion { get; set; }
        public string MotivoReunion { get; set; }
        public DateTime FechaReunion { get; set; }
        public string EstadoReunion { get; set; }

        public virtual ICollection<Usuario> Usuarios { get; set; }
    }
}