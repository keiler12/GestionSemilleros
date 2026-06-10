using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionSemilleros.Models.Entidades
{
    [Table("Fase")]
    public class Fase
    {
        [Key]
        public int IdFase { get; set; }
        public int IdProyecto { get; set; }
        public string NombreFase { get; set; }
        public int DuracionFase { get; set; }

        [ForeignKey("IdProyecto")]
        public virtual Proyecto Proyecto { get; set; }
        public virtual ICollection<Actividad> Actividades { get; set; }
    }
}