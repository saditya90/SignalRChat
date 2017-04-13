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
                        ImagePath = user.ImagePath,
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
                LastName = "K",
                ImagePath = "/Images/UsersImage/img2.jpg"
            });
            users.Add(new RegisterViewModel
            {
                Email = "sam12@yahoo.com",
                FirstName = "Sam",
                LastName = "Son",
                ImagePath = "/Images/UsersImage/imgDefault.jpg"
            });
            users.Add(new RegisterViewModel
            {
                Email = "elenag@hotmail.com",
                FirstName = "Elena",
                LastName = "Gilbert",
                ImagePath = "/Images/UsersImage/img1.jpg"
            });
            users.Add(new RegisterViewModel
            {
                Email = "jermey@live.com",
                FirstName = "Jermey",
                LastName = "Gilbert",
                ImagePath = "/Images/UsersImage/img3.jpg"
            });
            users.Add(new RegisterViewModel
            {
                Email = "keenw123@yahoo.com",
                FirstName = "Keen",
                LastName = "Wisley",
                ImagePath = "/Images/UsersImage/imgDefault.jpg"
            });
            users.Add(new RegisterViewModel
            {
                Email = "john@hotmail.com",
                FirstName = "Jhon",
                LastName = "Gilbert",
                ImagePath = "/Images/UsersImage/img4.jpg"
            });
            return users;

            #endregion
        }
    }
}