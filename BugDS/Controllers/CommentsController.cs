using BugDS.Models;
using BugDS.Models.CodeFirst;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace BugDS.Controllers
{
    public class CommentsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // POST: Comments/Create
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,TicketId,Description,MediaURL")]Comment comment, HttpPostedFileBase image)
        {         

            if (ModelState.IsValid)
            {
                comment.UserId = User.Identity.GetUserId();
                comment.Created = System.DateTimeOffset.Now;

                if (ImageUploadValidator.IsWebFriendlyImage(image))
                {
                    var fileName = Path.GetFileName(image.FileName);
                    image.SaveAs(Path.Combine(Server.MapPath("~/images/Comments/"), fileName));
                    comment.MediaURL = "~/images/Comments/" + fileName;
                }

                db.Comments.Add(comment);

                db.SaveChanges();
                db.Comments.Include("Ticket").FirstOrDefault(t=>t.TicketId == comment.TicketId);
                var dev = db.Users.Find(comment.Ticket.AssigneeUserId).Email;
                new EmailService().SendAsync(new IdentityMessage
                {
                    Destination = dev,
                    Subject = "Ticket Updates",
                    Body = "Updates have been made to one of your tickets!"
                });

            }
            return RedirectToAction("Details", "Tickets", new { id = comment.TicketId });

        }

        // GET: Comments/Edit/5
        [Authorize]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comment comment = db.Comments.Find(id);
            if (comment == null)
            {
                return HttpNotFound();
            }
            return View(comment);
        }


        // POST: Comments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,TicketId,Description")] Comment comment)
        {

            if (ModelState.IsValid)
            {
                db.Comments.Attach(comment);

                db.Entry(comment).Property("Description").IsModified = true;

                db.SaveChanges();

            }
            return RedirectToAction("Details", "Tickets", new { id = comment.TicketId });
        }

        // GET: Comments/Delete/5
        [Authorize]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comment comment = db.Comments.Find(id);
            if (comment == null)
            {
                return HttpNotFound();
            }
            return View(comment);
        }

        // POST: Comments/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Comment comment = db.Comments.Find(id);
            db.Comments.Remove(comment);
            db.SaveChanges();
            return RedirectToAction("Details", "Tickets", new { id = comment.TicketId });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}