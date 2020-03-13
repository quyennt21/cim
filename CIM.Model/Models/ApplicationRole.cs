using Microsoft.AspNet.Identity.EntityFramework;

namespace CIM.Model.Models
{
    public class ApplicationRole : IdentityRole<int, ApplicationUserRole>
    {
        public ApplicationRole()
        {
        }

        public ApplicationRole(string name)
        {
            Name = name;
        }
    }
}