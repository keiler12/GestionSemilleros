using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GestionSemilleros.Models.Entidades
{
    public class Patrocinador
    {
        [Key]
        public decimal IdPatrocinador { get; set; }
        public string NombrePatrocinador { get; set; }
        public string TipoPatrocinador { get; set; }
        public decimal TelefonoPatrocinador { get; set; }
        public string CorreoPatrocinador { get; set; }

        public virtual ICollection<Evento> Eventos { get; set; }
    }
}