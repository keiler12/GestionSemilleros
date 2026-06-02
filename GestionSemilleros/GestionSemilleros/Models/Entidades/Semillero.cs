using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GestionSemilleros.Models.Entidades
{
    public class Semillero
    {
        [Key]
        public decimal IdSemillero { get; set; }
        public string NombreSemillero { get; set; }
        public string LineaSemillero { get; set; }
        public string EnfoqueSemillero { get; set; }

        public virtual ICollection<Usuario> Usuarios { get; set; }
        public virtual ICollection<Proyecto> Proyectos { get; set; }
    }
}