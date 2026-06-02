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
    public class SemillerosContext : DbContext
    {
        public SemillerosContext() : base("name=SemillerosDB")
        {
        }

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
            // Tabla intermedia ProyectosEventos
            modelBuilder.Entity<Proyecto>()
                .HasMany(p => p.Eventos)
                .WithMany(e => e.Proyectos)
                .Map(m =>
                {
                    m.ToTable("ProyectosEventos");
                    m.MapLeftKey("idProyecto");
                    m.MapRightKey("idEvento");
                });

            // Tabla intermedia EventoPatrocinadores
            modelBuilder.Entity<Evento>()
                .HasMany(e => e.Patrocinadores)
                .WithMany(p => p.Eventos)
                .Map(m =>
                {
                    m.ToTable("EventoPatrocinadores");
                    m.MapLeftKey("idEvento");
                    m.MapRightKey("idPatrocinador");
                });

            base.OnModelCreating(modelBuilder);
        }
    }
}