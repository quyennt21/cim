namespace CIM.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class editdiarytb : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.maintenance_diary", "MaintenanceDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.maintenance_diary", "Description", c => c.String(maxLength: 1000));
            DropColumn("dbo.maintenance_diary", "DateMaintenance");
            DropColumn("dbo.maintenance_diary", "NextDateMaintenance");
        }
        
        public override void Down()
        {
            AddColumn("dbo.maintenance_diary", "NextDateMaintenance", c => c.DateTime());
            AddColumn("dbo.maintenance_diary", "DateMaintenance", c => c.DateTime(nullable: false));
            DropColumn("dbo.maintenance_diary", "Description");
            DropColumn("dbo.maintenance_diary", "MaintenanceDate");
        }
    }
}
