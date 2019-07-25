namespace Warsztat.DataContexts.IdentityMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class messages : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CustomerMessages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Message = c.String(),
                        ReportId_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Reports", t => t.ReportId_Id)
                .Index(t => t.ReportId_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CustomerMessages", "ReportId_Id", "dbo.Reports");
            DropIndex("dbo.CustomerMessages", new[] { "ReportId_Id" });
            DropTable("dbo.CustomerMessages");
        }
    }
}
