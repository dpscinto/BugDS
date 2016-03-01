namespace BugDS.Migrations
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<BugDS.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(BugDS.Models.ApplicationDbContext context)
        {
            var roleManager = new RoleManager<IdentityRole>(
            new RoleStore<IdentityRole>(context));

            if (!context.Roles.Any(r => r.Name == "Admin"))
            {
                roleManager.Create(new IdentityRole { Name = "Admin" });
            }

            var userManager = new UserManager<ApplicationUser>(
            new UserStore<ApplicationUser>(context));

            if (!context.Users.Any(u => u.Email == "your email address"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "dpscinto@gmail.com",
                    Email = "dpscinto@gmail.com",
                    DisplayName = "dpscinto"
                }, "C0derF0undry!");

            }

            var userId = userManager.FindByEmail("dpscinto@gmail.com").Id;
            userManager.AddToRole(userId, "Admin");

            //For Moderator

            //    if (!context.Roles.Any(r => r.Name == "Moderator"))
            //    {
            //        roleManager.Create(new IdentityRole { Name = "Moderator" });
            //    }

            //    if (!context.Users.Any(u => u.Email == "moderator@coderfoundry.com"))
            //    {
            //        userManager.Create(new ApplicationUser
            //        {
            //            UserName = "moderator@coderfoundry.com",
            //            Email = "moderator@coderfoundry.com",
            //            DisplayName = "CF Moderator"
            //        }, "Password-1");
            //    }

            //    userId = userManager.FindByEmail("moderator@coderfoundry.com").Id;
            //    userManager.AddToRole(userId, "Moderator");
            //}
        }
    }
}

