using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionSemilleros.Models.Entidades
{
    public class Actividad
    {
        [Key]
        public decimal IdActividad { get; set; }
        public decimal IdFase { get; set; }
        public string DuracionActividad { get; set; }
        public string NombreActividad { get; set; }
        public DateTime FechaEntregaActividad { get; set; }

        [ForeignKey("IdFase")]
        public virtual Fase Fase { get; set; }
    }
}