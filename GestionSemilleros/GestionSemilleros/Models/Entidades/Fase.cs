using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionSemilleros.Models.Entidades
{
    public class Fase
    {
        [Key]
        public decimal IdFase { get; set; }
        public decimal IdProyecto { get; set; }
        public string NombreFase { get; set; }
        public string DuracionFase { get; set; }

        [ForeignKey("IdProyecto")]
        public virtual Proyecto Proyecto { get; set; }
        public virtual ICollection<Actividad> Actividades { get; set; }
    }
}