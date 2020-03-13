namespace CIM.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class editdbrate : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.rate", "ReportID", "dbo.report");
            DropPrimaryKey("dbo.rate");
            AddPrimaryKey("dbo.rate", "ReportID");
            AddForeignKey("dbo.rate", "ReportID", "dbo.report", "ID");
            DropColumn("dbo.rate", "ID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.rate", "ID", c => c.Int(nullable: false, identity: true));
            DropForeignKey("dbo.rate", "ReportID", "dbo.report");
            DropPrimaryKey("dbo.rate");
            AddPrimaryKey("dbo.rate", "ID");
            AddForeignKey("dbo.rate", "ReportID", "dbo.report", "ID", cascadeDelete: true);
        }
    }
}
