using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CIM.Model.Models
{
    public class ApplicationUser : IdentityUser<int, ApplicationUserLogin, ApplicationUserRole, ApplicationUserClaim>
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser, int> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);

            // Add custom user claims here
            userIdentity.AddClaim(new Claim("FullName", FullName));

            return userIdentity;
        }

        public override int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public override string UserName { get; set; }

        [Required]
        [MaxLength(256)]
        public override string Email { get; set; }

        [MaxLength(256)]
        public string FullName { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime? CreatedAt { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime? UpdatedAt { get; set; }

        public bool Active { get; set; }

        public virtual IEnumerable<Asset> Asset { get; set; }

        public virtual IEnumerable<AssetLog> AssetLog { get; set; }

        public virtual IEnumerable<PIC> PIC { get; set; }
    }
}