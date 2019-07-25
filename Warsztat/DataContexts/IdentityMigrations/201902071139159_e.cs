namespace Warsztat.DataContexts.IdentityMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class e : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.CustomerMessages", "ReportId_Id", "dbo.Reports");
            DropIndex("dbo.CustomerMessages", new[] { "ReportId_Id" });
            RenameColumn(table: "dbo.CustomerMessages", name: "ReportId_Id", newName: "ReportId");
            AlterColumn("dbo.CustomerMessages", "ReportId", c => c.Int(nullable: false));
            CreateIndex("dbo.CustomerMessages", "ReportId");
            AddForeignKey("dbo.CustomerMessages", "ReportId", "dbo.Reports", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CustomerMessages", "ReportId", "dbo.Reports");
            DropIndex("dbo.CustomerMessages", new[] { "ReportId" });
            AlterColumn("dbo.CustomerMessages", "ReportId", c => c.Int());
            RenameColumn(table: "dbo.CustomerMessages", name: "ReportId", newName: "ReportId_Id");
            CreateIndex("dbo.CustomerMessages", "ReportId_Id");
            AddForeignKey("dbo.CustomerMessages", "ReportId_Id", "dbo.Reports", "Id");
        }
    }
}
