using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionSemilleros.Models.Entidades
{
    [Table("Evento")]
    public class Evento
    {
        [Key]
        public int IdEvento { get; set; }
        public string LugarEvento { get; set; }
        public string NombreEvento { get; set; }
        public string TipoEvento { get; set; }
        public DateTime FechaEvento { get; set; }
        public string OrganizadorEvento { get; set; }

        public virtual ICollection<Proyecto> Proyectos { get; set; }
        public virtual ICollection<Patrocinador> Patrocinadores { get; set; }
    }
}