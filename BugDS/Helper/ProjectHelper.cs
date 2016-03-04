using BugDS.Models;
using BugDS.Models.CodeFirst;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugDS.Helper
{
    public class ProjectHelper
    {

        private ApplicationDbContext db = new ApplicationDbContext();

        public bool IsUserInProject(string userId, int projectId)
        {
            var project = db.Projects.Find(projectId);
            var result = project.Users.Any(p => p.Id == userId);

            return result;
        }

        public IList<Project> ListUserProjects(string userId)
        {
            var user = db.Users.Find(userId);
            return user.Projects.ToList();
        }

        public void AddUserToProject(string userId, int projectId)
        {
            var project = db.Projects.Find(projectId);
            var user = db.Users.Find(userId);

            project.Users.Add(user);
            db.SaveChanges();
        }

        public void RemoveUserFromProject(string userId, int projectId)
        {
            var project = db.Projects.Find(projectId);
            var user = db.Users.Find(userId);

            project.Users.Remove(user);
            db.SaveChanges();
        }

        public IList<ApplicationUser> UsersInProject(int projectId)
        {

            var project = db.Projects.Find(projectId);
            return project.Users.ToList();

        }

        //public IList<ApplicationUser> UsersNotInProject(string projectId)
        //{
        //    var resultList = new List<ApplicationUser>();

        //    foreach (var user in manager.Users)
        //    {
        //        if (!IsUserInProject(user.Id, projectId))
        //        {
        //            resultList.Add(user);
        //        }
        //    }
        //    return resultList;
        //}
    }
}