namespace CIM.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addmail : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.mail_config",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        EmailAddress = c.String(nullable: false, maxLength: 256),
                        Password = c.String(nullable: false, maxLength: 256),
                        Port = c.String(nullable: false, maxLength: 256),
                        Host = c.String(nullable: false, maxLength: 256),
                        EnabledSSL = c.Boolean(nullable: false),
                        Count = c.Int(nullable: false),
                        DateSend = c.DateTime(),
                        TemplateID = c.Int(nullable: false),
                        CreatedAt = c.DateTime(),
                        UpdatedAt = c.DateTime(),
                        Active = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.mail_template",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Template = c.String(nullable: false),
                        CreatedAt = c.DateTime(),
                        UpdatedAt = c.DateTime(),
                        Active = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.mail_template");
            DropTable("dbo.mail_config");
        }
    }
}
