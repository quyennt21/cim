using CIM.Model.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;

namespace CIM.Data
{
    public class CIMDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, int, ApplicationUserLogin, ApplicationUserRole, ApplicationUserClaim>
    {
        public CIMDbContext() : base("name=CIMConnection")
        {
            this.Configuration.LazyLoadingEnabled = false;
        }

        public DbSet<Campus> Campuses { get; set; }
        public DbSet<Area> Areas { get; set; }
        public DbSet<AssetLog> AssetLogs { get; set; }
        public DbSet<Asset> Assets { get; set; }
        public DbSet<AssetTypeAttribute> AssetAttributes { get; set; }
        public DbSet<AssetType> AssetTypes { get; set; }
        public DbSet<AssetAttributeValue> AssetAttributeValues { get; set; }
        public DbSet<MaintenanceDiary> MaintenanceDiaries { get; set; }
        public DbSet<Warranty> Warranties { get; set; }
        public DbSet<Report> Reports { get; set; }
        public DbSet<Rate> Rates { get; set; }
        public DbSet<PIC> PICs { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<MailConfig> MailConfig { get; set; }
        public DbSet<MailTemplate> MailTemplate { get; set; }

        public static CIMDbContext Create()
        {
            return new CIMDbContext();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ApplicationUser>().ToTable("user", "dbo");
            modelBuilder.Entity<ApplicationRole>().ToTable("role", "dbo");
            modelBuilder.Entity<ApplicationUserLogin>().ToTable("user_login", "dbo");
            modelBuilder.Entity<ApplicationUserRole>().ToTable("user_role", "dbo");
            modelBuilder.Entity<ApplicationUserClaim>().ToTable("user_claim", "dbo");

            // Configure Asp Net Identity Tables
            //modelBuilder.Entity<IdentityUserRole>().HasKey(r => new { r.UserId, r.RoleId });
            //modelBuilder.Entity<IdentityUserLogin>().HasKey(l => l.UserId);
        }
    }
}