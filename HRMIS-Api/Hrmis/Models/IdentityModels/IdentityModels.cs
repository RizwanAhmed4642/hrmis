using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Data.Entity;

namespace Hrmis.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public string DivisionID { get; set; }
        public string DistrictID { get; set; }
        public string TehsilID { get; set; }
        public string hfmiscode { get; set; }
        public int LevelID { get; set; }
        public int? DesigCode { get; set; }
        public bool isActive { get; set; }
        public string HfmisCodeNew { get; set; }
        public DateTime? CreationDate { get; set; }
        public decimal? CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public decimal? ModifiedBy { get; set; }
        public string UserDetail { get; set; }
        public string responsibleuser { get; set; }
        public string hashynoty { get; set; }
        public object Identity { get; internal set; }
        public string HfTypeCode { get; set; }
        public bool isUpdated { get; set; }
        public string Cnic { get; set; }
        public int ProfileId { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager, string authenticationType)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, authenticationType);
            var hfmisCodeNew = HfmisCodeNew ?? "";

            // Add custom user claims here
            userIdentity.AddClaim(new Claim("HfmisCodeNew", hfmisCodeNew));

            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("ForIdentity", throwIfV1Schema: false)
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // This needs to go before the other rules!

            modelBuilder.Entity<ApplicationUser>().ToTable("_User");
            modelBuilder.Entity<IdentityRole>().ToTable("_Role");
            modelBuilder.Entity<IdentityUserRole>().ToTable("_UserRole");
            modelBuilder.Entity<IdentityUserClaim>().ToTable("_UserClaim");
            modelBuilder.Entity<IdentityUserLogin>().ToTable("_UserLogin");
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}
