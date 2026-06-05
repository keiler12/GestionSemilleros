using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionSemilleros.Models.Entidades
{
    [Table("Actividad")]
    public class Actividad
    {
        [Key]
        public int IdActividad { get; set; }
        public int IdFase { get; set; }
        public string DuracionActividad { get; set; }
        public string NombreActividad { get; set; }
        public DateTime FechaEntregaActividad { get; set; }

        [ForeignKey("IdFase")]
        public virtual Fase Fase { get; set; }
    }
}