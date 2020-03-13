namespace CIM.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fixwarranty : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.warranty", "DateWarranty", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.warranty", "DateWarranty", c => c.DateTime(nullable: false));
        }
    }
}
