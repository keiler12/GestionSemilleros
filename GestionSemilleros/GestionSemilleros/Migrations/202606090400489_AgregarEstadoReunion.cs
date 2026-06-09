namespace GestionSemilleros.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AgregarEstadoReunion : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Reunion", "EstadoReunion", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Reunion", "EstadoReunion");
        }
    }
}
