namespace GestionSemilleros.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AgregarDuracionReunion : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Reunion", "DuracionMinutos", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Reunion", "DuracionMinutos");
        }
    }
}
