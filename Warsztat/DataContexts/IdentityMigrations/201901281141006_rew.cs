namespace Warsztat.DataContexts.IdentityMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class rew : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Cars", "ApplicationUserID", "dbo.AspNetUsers");
            DropIndex("dbo.Cars", new[] { "ApplicationUserID" });
            AlterColumn("dbo.Cars", "ApplicationUserID", c => c.String(nullable: false, maxLength: 128));
            CreateIndex("dbo.Cars", "ApplicationUserID");
            AddForeignKey("dbo.Cars", "ApplicationUserID", "dbo.AspNetUsers", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Cars", "ApplicationUserID", "dbo.AspNetUsers");
            DropIndex("dbo.Cars", new[] { "ApplicationUserID" });
            AlterColumn("dbo.Cars", "ApplicationUserID", c => c.String(maxLength: 128));
            CreateIndex("dbo.Cars", "ApplicationUserID");
            AddForeignKey("dbo.Cars", "ApplicationUserID", "dbo.AspNetUsers", "Id");
        }
    }
}
