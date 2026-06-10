namespace GestionSemilleros.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DuracionFaseAInt : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Fase", "DuracionFase", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Fase", "DuracionFase", c => c.String());
        }
    }
}
