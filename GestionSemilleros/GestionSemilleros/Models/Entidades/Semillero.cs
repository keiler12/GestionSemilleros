using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionSemilleros.Models.Entidades
{
    [Table("Semillero")]
    public class Semillero
    {
        [Key]
        public int IdSemillero { get; set; }
        public string NombreSemillero { get; set; }
        public string LineaSemillero { get; set; }
        public string EnfoqueSemillero { get; set; }

        public virtual ICollection<Usuario> Usuarios { get; set; }
        public virtual ICollection<Proyecto> Proyectos { get; set; }
    }
}