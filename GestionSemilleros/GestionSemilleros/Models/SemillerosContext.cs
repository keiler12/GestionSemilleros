using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GestionSemilleros.Models.Entidades;

namespace GestionSemilleros.Models.DAO
{
    // Contexto de la base de datos que hereda de DbContext, representa la conexión a la base de datos y proporciona acceso a las tablas a través de DbSet
    public class SemillerosContext : DbContext
    {
        public SemillerosContext() : base("name=SemillerosDB")
        {
            // Configura la inicialización de la base de datos para crearla si no existe y poblarla con datos de ejemplo
        }

        // Propiedades DbSet que representan las tablas de la base de datos
        public DbSet<Semillero> Semilleros { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Reunion> Reuniones { get; set; }
        public DbSet<Proyecto> Proyectos { get; set; }
        public DbSet<Fase> Fases { get; set; }
        public DbSet<Actividad> Actividades { get; set; }
        public DbSet<Evento> Eventos { get; set; }
        public DbSet<Patrocinador> Patrocinadores { get; set; }

        
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Configura las relaciones entre las entidades utilizando Fluent API
            modelBuilder.Entity<Proyecto>()
                // Configura la relación muchos a muchos entre Proyecto y Evento, indicando que un proyecto puede tener muchos eventos y un evento puede estar asociado a muchos proyectos
                .HasMany(proyecto => proyecto.Eventos)
                .WithMany(evento => evento.Proyectos)
                .Map(mapa =>
                {
                    mapa.ToTable("ProyectosEventos");
                    mapa.MapLeftKey("idProyecto");
                    mapa.MapRightKey("idEvento");
                });

            // Configura la relación muchos a muchos entre Evento y Patrocinador, indicando que un evento puede tener muchos patrocinadores y un patrocinador puede estar asociado a muchos eventos
            modelBuilder.Entity<Evento>()
                .HasMany(evento => evento.Patrocinadores)
                .WithMany(patrocinador => patrocinador.Eventos)
                .Map(mapa =>
                {
                    mapa.ToTable("EventoPatrocinadores");
                    mapa.MapLeftKey("idEvento");
                    mapa.MapRightKey("idPatrocinador");
                });

            base.OnModelCreating(modelBuilder);
        }
    }
}