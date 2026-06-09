namespace GestionSemilleros.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ReunionMuchosAMuchos : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Reunion", "IdUsuario", "dbo.Usuario");
            DropIndex("dbo.Reunion", new[] { "IdUsuario" });
            CreateTable(
                "dbo.ReunionUsuarios",
                c => new
                {
                    idReunion = c.Int(nullable: false),
                    idUsuario = c.Int(nullable: false),
                })
                .PrimaryKey(t => new { t.idReunion, t.idUsuario })
                .ForeignKey("dbo.Reunion", t => t.idReunion, cascadeDelete: true, name: "FK_ReunionUsuarios_Reunion")
                .ForeignKey("dbo.Usuario", t => t.idUsuario, cascadeDelete: true, name: "FK_ReunionUsuarios_Usuario")
                .Index(t => t.idReunion)
                .Index(t => t.idUsuario);

            DropColumn("dbo.Reunion", "IdUsuario");
        }

        public override void Down()
        {
            AddColumn("dbo.Reuniones", "IdUsuario", c => c.Int(nullable: false));
            DropForeignKey("dbo.ReunionUsuarios", "idUsuario", "dbo.Usuario");
            DropForeignKey("dbo.ReunionUsuarios", "idReunion", "dbo.Reuniones");
            DropIndex("dbo.ReunionUsuarios", new[] { "idUsuario" });
            DropIndex("dbo.ReunionUsuarios", new[] { "idReunion" });
            DropTable("dbo.ReunionUsuarios");
            CreateIndex("dbo.Reuniones", "IdUsuario");
            AddForeignKey("dbo.Reunion", "IdUsuario", "dbo.Usuario", "IdUsuario", cascadeDelete: true);
            RenameTable(name: "dbo.Reuniones", newName: "Reunion");
        }
    }
}
