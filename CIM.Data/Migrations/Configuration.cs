namespace CIM.Data.Migrations
{
    using CIM.Model.Models;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<CIM.Data.CIMDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(CIM.Data.CIMDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method
            //  to avoid creating duplicate seed data.

            //insert Role
            var roleManager = new RoleManager<ApplicationRole, int>(new RoleStore<ApplicationRole, int, ApplicationUserRole>(new CIMDbContext()));

            if (!roleManager.Roles.Any())
            {
                roleManager.Create(new ApplicationRole { Name = "System Admin" });
                roleManager.Create(new ApplicationRole { Name = "Manager" });
                roleManager.Create(new ApplicationRole { Name = "Technician" });
            }
        }
    }
}