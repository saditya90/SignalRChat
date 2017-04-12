using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Security.Principal;

namespace WebChat.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            userIdentity.AddClaim(new Claim("UserFullName", string.Format("{0} {1}", FirstName, LastName)));
            return userIdentity;
        }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTimeOffset LastActivity { get; set; }
    }

    public class UsersChat
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public string FromUserId { get; set; }
        public string OtherUserId { get; set; }
        public string RoomId { get; set; }
        public string ClientGuidId { get; set; }
        public bool IsSeen { get; set; }
        public DateTime? CreatedOn { get; set; }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection")
        {
            Database.SetInitializer(new ApplicationSeed());
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<UsersChat> UsersChat { get; set; }
    }

    public static class IdentityExtensions
    {
        public static string FullUserName(this IIdentity identity)
        {
            var claim = identity as ClaimsIdentity;
            return claim.FindFirst("UserFullName") == null ?
                     string.Empty :
                    claim.FindFirst("UserFullName").Value;
        }
    }
}