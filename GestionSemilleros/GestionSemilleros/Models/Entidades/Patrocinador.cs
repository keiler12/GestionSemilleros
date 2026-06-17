using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionSemilleros.Models.Entidades
{
    [Table("Patrocinador")]
    public class Patrocinador
    {
        [Key]
        public int IdPatrocinador { get; set; }
        public string NombrePatrocinador { get; set; }
        public string TipoPatrocinador { get; set; }
        public string TelefonoPatrocinador { get; set; }
        public string CorreoPatrocinador { get; set; }

        public virtual ICollection<Evento> Eventos { get; set; }
    }
}