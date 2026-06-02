using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionSemilleros.Models.Entidades
{
    public class Proyecto
    {
        [Key]
        public decimal IdProyecto { get; set; }
        public decimal IdSemillero { get; set; }
        public string TituloProyecto { get; set; }
        public string ObjetivoProyecto { get; set; }
        public string DescripcionProyecto { get; set; }
        public DateTime FechaInicioProyecto { get; set; }
        public DateTime FechaFinProyecto { get; set; }

        [ForeignKey("IdSemillero")]
        public virtual Semillero Semillero { get; set; }
        public virtual ICollection<Fase> Fases { get; set; }
        public virtual ICollection<Evento> Eventos { get; set; }
    }
}