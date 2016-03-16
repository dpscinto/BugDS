namespace BugDS.Migrations
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using Models;
    using Models.CodeFirst;
    using System;
    using System.Collections.Generic;
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

            //Admin Role
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

            //Project Manager Role
            if (!context.Roles.Any(r => r.Name == "Project Manager"))
            {
                roleManager.Create(new IdentityRole { Name = "Project Manager" });
            }


            //Developer Role
            if (!context.Roles.Any(r => r.Name == "Developer"))
            {
                roleManager.Create(new IdentityRole { Name = "Developer" });
            }

            //Submitter Role
            if (!context.Roles.Any(r => r.Name == "Submitter"))
            {
                roleManager.Create(new IdentityRole { Name = "Submitter" });
            }

            //if (!context.CategoryLists.Any())
            //{
            //    context.CategoryLists.AddRange(
            //        new List<CategoryList> {
            //    new CategoryList { Name = "Automobile" },

            ////////////////////////////////////////////////////////////////////

            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },

            context.TicketPriorities.AddOrUpdate(
                p => p.Name,
            new TicketPriority { Name = "Low" },
            new TicketPriority { Name = "Medium" },
            new TicketPriority { Name = "High" },
            new TicketPriority { Name = "Urgent" },
            new TicketPriority { Name = "Critical" }

                );

            context.TicketStatuses.AddOrUpdate(
                p => p.Name,
            new TicketStatus { Name = "Unassigned" },
            new TicketStatus { Name = "To Do" },
            new TicketStatus { Name = "Doing" },
            new TicketStatus { Name = "Done" }

                );


            context.TicketTypes.AddOrUpdate(
                p => p.Name,
            new TicketType { Name = "Database" },
            new TicketType { Name = "Security" },
            new TicketType { Name = "Forms" },
            new TicketType { Name = "Front End" },
            new TicketType { Name = "Other" }

               );

        }
    }
}

