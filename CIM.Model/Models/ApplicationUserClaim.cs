using Microsoft.AspNet.Identity.EntityFramework;

namespace CIM.Model.Models
{
    public class ApplicationUserClaim : IdentityUserClaim<int>
    {
        public ApplicationUserClaim()
        {
        }
    }
}