using System.Data.Entity;
using System.Web;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;
using System.Collections.Generic;
using System;

namespace WebChat.Models
{
    public class ApplicationSeed : DropCreateDatabaseIfModelChanges<ApplicationDbContext>
    {
        protected override void Seed(ApplicationDbContext context)
        {
            var users = DefaultUsers();
            users.ForEach(user =>
            {
                if (UserManager.FindByName(user.Email) == null)
                {
                    var newUser = new ApplicationUser
                    {
                        UserName = user.Email,
                        Email = user.Email,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        LastActivity = DateTimeOffset.Now
                    };
                    var result = UserManager.Create(newUser, "PassWord12!");
                }
            });

            base.Seed(context);
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
        }

        private List<RegisterViewModel> DefaultUsers()
        {
            #region Users
             
            var users = new List<RegisterViewModel>();
            users.Add(new RegisterViewModel
            {
                Email = "wilison@hotmail.com",
                FirstName = "Wilison",
                LastName = "K"
            });
            users.Add(new RegisterViewModel
            {
                Email = "sam12@yahoo.com",
                FirstName = "Sam",
                LastName = "Son"
            });
            users.Add(new RegisterViewModel
            {
                Email = "elenag@hotmail.com",
                FirstName = "Elena",
                LastName = "Gilbert"
            });
            users.Add(new RegisterViewModel
            {
                Email = "jermey@live.com",
                FirstName = "Jermey",
                LastName = "Gilbert"
            });
            users.Add(new RegisterViewModel
            {
                Email = "keenw123@yahoo.com",
                FirstName = "Keen",
                LastName = "Wisley"
            });
            users.Add(new RegisterViewModel
            {
                Email = "henerym@hotmail.com",
                FirstName = "Henery",
                LastName = "L"
            });
            return users;

            #endregion
        }
    }
}