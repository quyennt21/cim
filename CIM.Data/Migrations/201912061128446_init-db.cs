namespace CIM.Data.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class initdb : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.area",
                c => new
                {
                    ID = c.Int(nullable: false, identity: true),
                    AreaCode = c.String(nullable: false, maxLength: 256),
                    Name = c.String(nullable: false, maxLength: 256),
                    Description = c.String(maxLength: 1000),
                    LocationID = c.Int(nullable: false),
                    CreatedAt = c.DateTime(defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = c.DateTime(defaultValueSql: "GETUTCDATE()"),
                    Active = c.Boolean(nullable: false),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.location", t => t.LocationID, cascadeDelete: true)
                .Index(t => t.LocationID);

            CreateTable(
                "dbo.location",
                c => new
                {
                    ID = c.Int(nullable: false, identity: true),
                    LocationCode = c.String(nullable: false, maxLength: 256),
                    Name = c.String(nullable: false, maxLength: 256),
                    Description = c.String(maxLength: 1000),
                    CampusID = c.Int(nullable: false),
                    CreatedAt = c.DateTime(defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = c.DateTime(defaultValueSql: "GETUTCDATE()"),
                    Active = c.Boolean(nullable: false),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.campus", t => t.CampusID, cascadeDelete: true)
                .Index(t => t.CampusID);

            CreateTable(
                "dbo.campus",
                c => new
                {
                    ID = c.Int(nullable: false, identity: true),
                    Name = c.String(nullable: false, maxLength: 256),
                    Address = c.String(maxLength: 500),
                    Telephone = c.String(maxLength: 20),
                    CreatedAt = c.DateTime(defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = c.DateTime(defaultValueSql: "GETUTCDATE()"),
                    Active = c.Boolean(nullable: false),
                })
                .PrimaryKey(t => t.ID);

            CreateTable(
                "dbo.asset_type_attribute",
                c => new
                {
                    ID = c.Int(nullable: false, identity: true),
                    Name = c.String(nullable: false, maxLength: 256),
                    AssetTypeID = c.Int(nullable: false),
                    CreatedAt = c.DateTime(defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = c.DateTime(defaultValueSql: "GETUTCDATE()"),
                    Active = c.Boolean(nullable: false),
                })
                .PrimaryKey(t => t.ID);

            CreateTable(
                "dbo.asset_attribute_value",
                c => new
                {
                    ID = c.Int(nullable: false, identity: true),
                    Value = c.String(nullable: false, maxLength: 500),
                    AssetID = c.Int(nullable: false),
                    AssetAttributeID = c.Int(nullable: false),
                    CreatedAt = c.DateTime(defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = c.DateTime(defaultValueSql: "GETUTCDATE()"),
                    Active = c.Boolean(nullable: false),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.asset", t => t.AssetID, cascadeDelete: true)
                .ForeignKey("dbo.asset_type_attribute", t => t.AssetAttributeID, cascadeDelete: true)
                .Index(t => t.AssetID)
                .Index(t => t.AssetAttributeID);

            CreateTable(
                "dbo.asset",
                c => new
                {
                    ID = c.Int(nullable: false, identity: true),
                    AssetCode = c.String(nullable: false, maxLength: 256),
                    Name = c.String(nullable: false, maxLength: 256),
                    Description = c.String(maxLength: 1000),
                    Quantity = c.Int(nullable: false),
                    StartDate = c.DateTime(nullable: false),
                    Image = c.String(),
                    AssetTypeID = c.Int(nullable: false),
                    ApplicationUserID = c.Int(nullable: false),
                    AreaID = c.Int(nullable: false),
                    CreatedAt = c.DateTime(defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = c.DateTime(defaultValueSql: "GETUTCDATE()"),
                    Active = c.Boolean(nullable: false),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.user", t => t.ApplicationUserID, cascadeDelete: true)
                .ForeignKey("dbo.area", t => t.AreaID, cascadeDelete: true)
                .ForeignKey("dbo.asset_type", t => t.AssetTypeID, cascadeDelete: true)
                .Index(t => t.AssetTypeID)
                .Index(t => t.ApplicationUserID)
                .Index(t => t.AreaID);

            CreateTable(
                "dbo.user",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    UserName = c.String(nullable: false, maxLength: 256),
                    Email = c.String(nullable: false, maxLength: 256),
                    FullName = c.String(maxLength: 256),
                    CreatedAt = c.DateTime(),
                    UpdatedAt = c.DateTime(),
                    Active = c.Boolean(nullable: false),
                    EmailConfirmed = c.Boolean(nullable: false),
                    PasswordHash = c.String(),
                    SecurityStamp = c.String(),
                    PhoneNumber = c.String(),
                    PhoneNumberConfirmed = c.Boolean(nullable: false),
                    TwoFactorEnabled = c.Boolean(nullable: false),
                    LockoutEndDateUtc = c.DateTime(),
                    LockoutEnabled = c.Boolean(nullable: false),
                    AccessFailedCount = c.Int(nullable: false),
                })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Email, unique: true, name: "EmailIndex");

            CreateTable(
                "dbo.user_claim",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    UserId = c.Int(nullable: false),
                    ClaimType = c.String(),
                    ClaimValue = c.String(),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.user", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);

            CreateTable(
                "dbo.user_login",
                c => new
                {
                    LoginProvider = c.String(nullable: false, maxLength: 128),
                    ProviderKey = c.String(nullable: false, maxLength: 128),
                    UserId = c.Int(nullable: false),
                })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.user", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);

            CreateTable(
                "dbo.user_role",
                c => new
                {
                    UserId = c.Int(nullable: false),
                    RoleId = c.Int(nullable: false),
                })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.user", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.role", t => t.RoleId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);

            CreateTable(
                "dbo.asset_type",
                c => new
                {
                    ID = c.Int(nullable: false, identity: true),
                    Name = c.String(nullable: false, maxLength: 256),
                    CreatedAt = c.DateTime(defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = c.DateTime(defaultValueSql: "GETUTCDATE()"),
                    Active = c.Boolean(nullable: false),
                })
                .PrimaryKey(t => t.ID);

            CreateTable(
                "dbo.asset_log",
                c => new
                {
                    ID = c.Int(nullable: false, identity: true),
                    Message = c.String(nullable: false),
                    ApplicationUserID = c.Int(nullable: false),
                    AssetID = c.Int(),
                    CreatedAt = c.DateTime(defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = c.DateTime(defaultValueSql: "GETUTCDATE()"),
                    Active = c.Boolean(nullable: false),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.user", t => t.ApplicationUserID, cascadeDelete: true)
                .ForeignKey("dbo.asset", t => t.AssetID)
                .Index(t => t.ApplicationUserID)
                .Index(t => t.AssetID);

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
                    CreatedAt = c.DateTime(defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = c.DateTime(defaultValueSql: "GETUTCDATE()"),
                    Active = c.Boolean(nullable: false),
                })
                .PrimaryKey(t => t.ID);

            CreateTable(
                "dbo.mail_template",
                c => new
                {
                    ID = c.Int(nullable: false, identity: true),
                    Template = c.String(nullable: false),
                    CreatedAt = c.DateTime(defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = c.DateTime(defaultValueSql: "GETUTCDATE()"),
                    Active = c.Boolean(nullable: false),
                })
                .PrimaryKey(t => t.ID);

            CreateTable(
                "dbo.maintenance_diary",
                c => new
                {
                    ID = c.Int(nullable: false, identity: true),
                    MaintenanceDate = c.DateTime(nullable: false),
                    Description = c.String(maxLength: 1000),
                    AssetID = c.Int(nullable: false),
                    CreatedAt = c.DateTime(defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = c.DateTime(defaultValueSql: "GETUTCDATE()"),
                    Active = c.Boolean(nullable: false),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.asset", t => t.AssetID, cascadeDelete: true)
                .Index(t => t.AssetID);

            CreateTable(
                "dbo.PIC",
                c => new
                {
                    ID = c.Int(nullable: false, identity: true),
                    StartDate = c.DateTime(nullable: false),
                    EndDate = c.DateTime(nullable: false),
                    AssetTypeID = c.Int(nullable: false),
                    ApplicationUserID = c.Int(nullable: false),
                    AreaID = c.Int(nullable: false),
                    CreatedAt = c.DateTime(defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = c.DateTime(defaultValueSql: "GETUTCDATE()"),
                    Active = c.Boolean(nullable: false),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.user", t => t.ApplicationUserID, cascadeDelete: true)
                .ForeignKey("dbo.area", t => t.AreaID, cascadeDelete: true)
                .ForeignKey("dbo.asset_type", t => t.AssetTypeID, cascadeDelete: true)
                .Index(t => t.AssetTypeID)
                .Index(t => t.ApplicationUserID)
                .Index(t => t.AreaID);

            CreateTable(
                "dbo.rate",
                c => new
                {
                    ReportID = c.Int(nullable: false),
                    Value = c.Decimal(nullable: false, precision: 18, scale: 2),
                    Comment = c.String(maxLength: 500),
                    CreatedAt = c.DateTime(defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = c.DateTime(defaultValueSql: "GETUTCDATE()"),
                    Active = c.Boolean(nullable: false),
                })
                .PrimaryKey(t => t.ReportID)
                .ForeignKey("dbo.report", t => t.ReportID)
                .Index(t => t.ReportID);

            CreateTable(
                "dbo.report",
                c => new
                {
                    ID = c.Int(nullable: false, identity: true),
                    status = c.Int(nullable: false),
                    ReportAt = c.DateTime(nullable: false),
                    Picture = c.String(maxLength: 256),
                    UserReport = c.String(maxLength: 100),
                    Comment = c.String(maxLength: 500),
                    RequestManager = c.String(maxLength: 100),
                    AssetID = c.Int(),
                    CreatedAt = c.DateTime(defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = c.DateTime(defaultValueSql: "GETUTCDATE()"),
                    Active = c.Boolean(nullable: false),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.asset", t => t.AssetID)
                .Index(t => t.AssetID);

            CreateTable(
                "dbo.role",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Name = c.String(nullable: false, maxLength: 256),
                })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");

            CreateTable(
                "dbo.warranty",
                c => new
                {
                    ID = c.Int(nullable: false, identity: true),
                    EndDate = c.DateTime(nullable: false),
                    DateWarranty = c.DateTime(nullable: false),
                    Reason = c.String(nullable: false, maxLength: 1000),
                    Result = c.String(nullable: false, maxLength: 1000),
                    AssetID = c.Int(nullable: false),
                    CreatedAt = c.DateTime(defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = c.DateTime(defaultValueSql: "GETUTCDATE()"),
                    Active = c.Boolean(nullable: false),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.asset", t => t.AssetID, cascadeDelete: true)
                .Index(t => t.AssetID);
        }

        public override void Down()
        {
            DropForeignKey("dbo.warranty", "AssetID", "dbo.asset");
            DropForeignKey("dbo.user_role", "RoleId", "dbo.role");
            DropForeignKey("dbo.rate", "ReportID", "dbo.report");
            DropForeignKey("dbo.report", "AssetID", "dbo.asset");
            DropForeignKey("dbo.PIC", "AssetTypeID", "dbo.asset_type");
            DropForeignKey("dbo.PIC", "AreaID", "dbo.area");
            DropForeignKey("dbo.PIC", "ApplicationUserID", "dbo.user");
            DropForeignKey("dbo.maintenance_diary", "AssetID", "dbo.asset");
            DropForeignKey("dbo.asset_log", "AssetID", "dbo.asset");
            DropForeignKey("dbo.asset_log", "ApplicationUserID", "dbo.user");
            DropForeignKey("dbo.asset_attribute_value", "AssetAttributeID", "dbo.asset_type_attribute");
            DropForeignKey("dbo.asset_attribute_value", "AssetID", "dbo.asset");
            DropForeignKey("dbo.asset", "AssetTypeID", "dbo.asset_type");
            DropForeignKey("dbo.asset", "AreaID", "dbo.area");
            DropForeignKey("dbo.asset", "ApplicationUserID", "dbo.user");
            DropForeignKey("dbo.user_role", "UserId", "dbo.user");
            DropForeignKey("dbo.user_login", "UserId", "dbo.user");
            DropForeignKey("dbo.user_claim", "UserId", "dbo.user");
            DropForeignKey("dbo.area", "LocationID", "dbo.location");
            DropForeignKey("dbo.location", "CampusID", "dbo.campus");
            DropIndex("dbo.warranty", new[] { "AssetID" });
            DropIndex("dbo.role", "RoleNameIndex");
            DropIndex("dbo.report", new[] { "AssetID" });
            DropIndex("dbo.rate", new[] { "ReportID" });
            DropIndex("dbo.PIC", new[] { "AreaID" });
            DropIndex("dbo.PIC", new[] { "ApplicationUserID" });
            DropIndex("dbo.PIC", new[] { "AssetTypeID" });
            DropIndex("dbo.maintenance_diary", new[] { "AssetID" });
            DropIndex("dbo.asset_log", new[] { "AssetID" });
            DropIndex("dbo.asset_log", new[] { "ApplicationUserID" });
            DropIndex("dbo.user_role", new[] { "RoleId" });
            DropIndex("dbo.user_role", new[] { "UserId" });
            DropIndex("dbo.user_login", new[] { "UserId" });
            DropIndex("dbo.user_claim", new[] { "UserId" });
            DropIndex("dbo.user", "UserNameIndex");
            DropIndex("dbo.asset", new[] { "AreaID" });
            DropIndex("dbo.asset", new[] { "ApplicationUserID" });
            DropIndex("dbo.asset", new[] { "AssetTypeID" });
            DropIndex("dbo.asset_attribute_value", new[] { "AssetAttributeID" });
            DropIndex("dbo.asset_attribute_value", new[] { "AssetID" });
            DropIndex("dbo.location", new[] { "CampusID" });
            DropIndex("dbo.area", new[] { "LocationID" });
            DropTable("dbo.warranty");
            DropTable("dbo.role");
            DropTable("dbo.report");
            DropTable("dbo.rate");
            DropTable("dbo.PIC");
            DropTable("dbo.maintenance_diary");
            DropTable("dbo.mail_template");
            DropTable("dbo.mail_config");
            DropTable("dbo.asset_log");
            DropTable("dbo.asset_type");
            DropTable("dbo.user_role");
            DropTable("dbo.user_login");
            DropTable("dbo.user_claim");
            DropTable("dbo.user");
            DropTable("dbo.asset");
            DropTable("dbo.asset_attribute_value");
            DropTable("dbo.asset_type_attribute");
            DropTable("dbo.campus");
            DropTable("dbo.location");
            DropTable("dbo.area");
        }
    }
}