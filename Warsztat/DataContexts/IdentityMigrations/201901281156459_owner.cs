namespace Warsztat.DataContexts.IdentityMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class owner : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Cars", "Owner");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Cars", "Owner", c => c.String());
        }
    }
}
