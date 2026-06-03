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
            modelBuilder.Entity<Proyecto>()
                .HasMany(proyecto => proyecto.Eventos)
                .WithMany(evento => evento.Proyectos)
                .Map(mapa =>
                {
                    mapa.ToTable("ProyectosEventos");
                    mapa.MapLeftKey("idProyecto");
                    mapa.MapRightKey("idEvento");
                });

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