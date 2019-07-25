namespace Warsztat.DataContexts.IdentityMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class user : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CustomerMessages", "User", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.CustomerMessages", "User");
        }
    }
}
