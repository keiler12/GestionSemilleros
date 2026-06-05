namespace GestionSemilleros.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Inicial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Actividad",
                c => new
                    {
                        IdActividad = c.Int(nullable: false, identity: true),
                        IdFase = c.Int(nullable: false),
                        DuracionActividad = c.String(),
                        NombreActividad = c.String(),
                        FechaEntregaActividad = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.IdActividad)
                .ForeignKey("dbo.Fase", t => t.IdFase, cascadeDelete: true)
                .Index(t => t.IdFase);
            
            CreateTable(
                "dbo.Fase",
                c => new
                    {
                        IdFase = c.Int(nullable: false, identity: true),
                        IdProyecto = c.Int(nullable: false),
                        NombreFase = c.String(),
                        DuracionFase = c.String(),
                    })
                .PrimaryKey(t => t.IdFase)
                .ForeignKey("dbo.Proyecto", t => t.IdProyecto, cascadeDelete: true)
                .Index(t => t.IdProyecto);
            
            CreateTable(
                "dbo.Proyecto",
                c => new
                    {
                        IdProyecto = c.Int(nullable: false, identity: true),
                        IdSemillero = c.Int(nullable: false),
                        TituloProyecto = c.String(),
                        ObjetivoProyecto = c.String(),
                        DescripcionProyecto = c.String(),
                        FechaInicioProyecto = c.DateTime(nullable: false),
                        FechaFinProyecto = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.IdProyecto)
                .ForeignKey("dbo.Semillero", t => t.IdSemillero, cascadeDelete: true)
                .Index(t => t.IdSemillero);
            
            CreateTable(
                "dbo.Evento",
                c => new
                    {
                        IdEvento = c.Int(nullable: false, identity: true),
                        LugarEvento = c.String(),
                        NombreEvento = c.String(),
                        TipoEvento = c.String(),
                        FechaEvento = c.DateTime(nullable: false),
                        OrganizadorEvento = c.String(),
                    })
                .PrimaryKey(t => t.IdEvento);
            
            CreateTable(
                "dbo.Patrocinador",
                c => new
                    {
                        IdPatrocinador = c.Int(nullable: false, identity: true),
                        NombrePatrocinador = c.String(),
                        TipoPatrocinador = c.String(),
                        TelefonoPatrocinador = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CorreoPatrocinador = c.String(),
                    })
                .PrimaryKey(t => t.IdPatrocinador);
            
            CreateTable(
                "dbo.Semillero",
                c => new
                    {
                        IdSemillero = c.Int(nullable: false, identity: true),
                        NombreSemillero = c.String(),
                        LineaSemillero = c.String(),
                        EnfoqueSemillero = c.String(),
                    })
                .PrimaryKey(t => t.IdSemillero);
            
            CreateTable(
                "dbo.Usuario",
                c => new
                    {
                        IdUsuario = c.Int(nullable: false, identity: true),
                        IdSemillero = c.Int(),
                        ContraseñaUsuario = c.String(),
                        NombresUsuario = c.String(),
                        RolUsuario = c.String(),
                        TelefonoUsuario = c.Int(nullable: false),
                        CorreoUsuario = c.String(),
                        EdadUsuario = c.Int(nullable: false),
                        GeneroUsuario = c.String(),
                        EstadoUsuario = c.String(),
                    })
                .PrimaryKey(t => t.IdUsuario)
                .ForeignKey("dbo.Semillero", t => t.IdSemillero)
                .Index(t => t.IdSemillero);
            
            CreateTable(
                "dbo.Reunion",
                c => new
                    {
                        IdReunion = c.Int(nullable: false, identity: true),
                        IdUsuario = c.Int(nullable: false),
                        TipoReunion = c.String(),
                        HoraReunion = c.Time(nullable: false, precision: 7),
                        MotivoReunion = c.String(),
                        FechaReunion = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.IdReunion)
                .ForeignKey("dbo.Usuario", t => t.IdUsuario, cascadeDelete: true)
                .Index(t => t.IdUsuario);
            
            CreateTable(
                "dbo.EventoPatrocinadores",
                c => new
                    {
                        idEvento = c.Int(nullable: false),
                        idPatrocinador = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.idEvento, t.idPatrocinador })
                .ForeignKey("dbo.Evento", t => t.idEvento, cascadeDelete: true)
                .ForeignKey("dbo.Patrocinador", t => t.idPatrocinador, cascadeDelete: true)
                .Index(t => t.idEvento)
                .Index(t => t.idPatrocinador);
            
            CreateTable(
                "dbo.ProyectosEventos",
                c => new
                    {
                        idProyecto = c.Int(nullable: false),
                        idEvento = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.idProyecto, t.idEvento })
                .ForeignKey("dbo.Proyecto", t => t.idProyecto, cascadeDelete: true)
                .ForeignKey("dbo.Evento", t => t.idEvento, cascadeDelete: true)
                .Index(t => t.idProyecto)
                .Index(t => t.idEvento);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Usuario", "IdSemillero", "dbo.Semillero");
            DropForeignKey("dbo.Reunion", "IdUsuario", "dbo.Usuario");
            DropForeignKey("dbo.Proyecto", "IdSemillero", "dbo.Semillero");
            DropForeignKey("dbo.Fase", "IdProyecto", "dbo.Proyecto");
            DropForeignKey("dbo.ProyectosEventos", "idEvento", "dbo.Evento");
            DropForeignKey("dbo.ProyectosEventos", "idProyecto", "dbo.Proyecto");
            DropForeignKey("dbo.EventoPatrocinadores", "idPatrocinador", "dbo.Patrocinador");
            DropForeignKey("dbo.EventoPatrocinadores", "idEvento", "dbo.Evento");
            DropForeignKey("dbo.Actividad", "IdFase", "dbo.Fase");
            DropIndex("dbo.ProyectosEventos", new[] { "idEvento" });
            DropIndex("dbo.ProyectosEventos", new[] { "idProyecto" });
            DropIndex("dbo.EventoPatrocinadores", new[] { "idPatrocinador" });
            DropIndex("dbo.EventoPatrocinadores", new[] { "idEvento" });
            DropIndex("dbo.Reunion", new[] { "IdUsuario" });
            DropIndex("dbo.Usuario", new[] { "IdSemillero" });
            DropIndex("dbo.Proyecto", new[] { "IdSemillero" });
            DropIndex("dbo.Fase", new[] { "IdProyecto" });
            DropIndex("dbo.Actividad", new[] { "IdFase" });
            DropTable("dbo.ProyectosEventos");
            DropTable("dbo.EventoPatrocinadores");
            DropTable("dbo.Reunion");
            DropTable("dbo.Usuario");
            DropTable("dbo.Semillero");
            DropTable("dbo.Patrocinador");
            DropTable("dbo.Evento");
            DropTable("dbo.Proyecto");
            DropTable("dbo.Fase");
            DropTable("dbo.Actividad");
        }
    }
}
