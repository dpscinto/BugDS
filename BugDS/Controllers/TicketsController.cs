using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BugDS.Models;
using BugDS.Models.CodeFirst;
using System.IO;
using Microsoft.AspNet.Identity;
using BugDS.Helper;

namespace BugDS.Controllers
{
    [RequireHttps]
    [Authorize]
    public class TicketsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Tickets
        public ActionResult Index()
        {
            var userId = User.Identity.GetUserId();
            IQueryable<Ticket> tickets;
            if (User.IsInRole("Admin"))
            {
                tickets = db.Tickets.Include(t => t.AssigneeUser).Include(t => t.CreatedUser);

            }

            else if (User.IsInRole("Project Manager") || User.IsInRole("Developer"))
            {
                var user = db.Users.Find(userId);
                tickets = user.Projects.SelectMany(p => p.Tickets).AsQueryable();
            }

            else
            {
                tickets = db.Tickets.Where(t => t.CreatedUserId == userId);
            }
            return View(tickets.ToList());
        }

        // GET: Tickets/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            return View(ticket);
        }

        // GET: Tickets/Create
        public ActionResult Create(int? projectId)
        {

            var project = db.Projects.Find(projectId);
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Name");
            ViewBag.TypeId = new SelectList(db.TicketTypes, "Id", "Name");

            if (!User.IsInRole("Submitter"))
            {
                ViewBag.PriorityId = new SelectList(db.TicketPriorities, "Id", "Name");
            }
            return View();
        }

        // POST: Tickets/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Description,MediaURL,ProjectId,PriorityId,TypeId")] Ticket ticket, HttpPostedFileBase image)
        {

            if (ModelState.IsValid)
            {
                var userId = User.Identity.GetUserId();

                if (ImageUploadValidator.IsWebFriendlyImage(image))
                {
                    var fileName = Path.GetFileName(image.FileName);
                    image.SaveAs(Path.Combine(Server.MapPath("~/images/Tickets/"), fileName));
                    ticket.MediaURL = "~/images/Tickets/" + fileName;
                }

                ticket.CreatedUserId = userId;
                ticket.Created = new DateTimeOffset(DateTime.Now);
                if (User.IsInRole("Submitter"))
                {
                    ticket.PriorityId = db.TicketPriorities.FirstOrDefault(n => n.Name == "High").Id;
                }
                ticket.StatusId = db.TicketStatuses.FirstOrDefault(n => n.Name == "Unassigned").Id;
                db.Tickets.Add(ticket);
                db.SaveChanges();
                return RedirectToAction("Index", "Tickets");
            }

            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Name");
            ViewBag.PriorityId = new SelectList(db.TicketPriorities, "Id", "Name");
            ViewBag.TypeId = new SelectList(db.TicketTypes, "Id", "Name");
            return View(ticket);
        }

        // GET: Tickets/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }

            ViewBag.PriorityId = new SelectList(db.TicketPriorities, "Id", "Name", ticket.PriorityId);
            ViewBag.StatusId = new SelectList(db.TicketStatuses, "Id", "Name", ticket.StatusId);
            ViewBag.TypeId = new SelectList(db.TicketTypes, "Id", "Name", ticket.TypeId);
            return View(ticket);
        }

        // POST: Tickets/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Created,LastModified,Description,MediaURL,CreatedUserId,AssigneeUserId,ProjectId,PriorityId,StatusId,TypeId")] Ticket ticket, HttpPostedFileBase image)
        {

            var userId = User.Identity.GetUserId();
            var modified = new DateTimeOffset(DateTime.Now);
            var oldTic = db.Tickets.AsNoTracking().FirstOrDefault(t => t.Id == ticket.Id);

            if (ModelState.IsValid)
            {
                ticket.LastModified = System.DateTimeOffset.Now;
                db.Tickets.Attach(ticket);
                db.Entry(ticket).Property("Description").IsModified = true;
                db.Entry(ticket).Property("MediaURL").IsModified = true;
                db.Entry(ticket).Property("PriorityId").IsModified = true;
                db.Entry(ticket).Property("StatusId").IsModified = true;
                db.Entry(ticket).Property("TypeId").IsModified = true;

                if (oldTic?.Description != ticket.Description)
                {
                    Log ticketlog = new Log
                    {
                        TicketId = ticket.Id,
                        Property = "Description",
                        ChangedOld = oldTic?.Description,
                        ChangedNew = ticket.Description,
                        Modified = modified,
                        UserId = userId
                    };
                    db.Logs.Add(ticketlog);
                }

                if (oldTic?.MediaURL != ticket.MediaURL)
                {
                    Log ticketlog = new Log
                    {
                        TicketId = ticket.Id,
                        Property = "Attachment",
                        ChangedOld = oldTic?.MediaURL,
                        ChangedNew = ticket.MediaURL,
                        Modified = modified,
                        UserId = userId
                    };
                    db.Logs.Add(ticketlog);
                }

                if (oldTic?.PriorityId != ticket.PriorityId)
                {
                    Log ticketlog = new Log
                    {
                        TicketId = ticket.Id,
                        Property = "Priority",
                        ChangedOld = db.TicketPriorities.Find(oldTic?.PriorityId).Name,
                        ChangedNew = db.TicketPriorities.Find(ticket.PriorityId).Name,
                        Modified = modified,
                        UserId = userId
                    };
                    db.Logs.Add(ticketlog);
                }

                if (oldTic?.StatusId != ticket.StatusId)
                {
                    Log ticketlog = new Log
                    {
                        TicketId = ticket.Id,
                        Property = "Status",
                        ChangedOld = db.TicketStatuses.Find(oldTic?.StatusId).Name,
                        ChangedNew = db.TicketStatuses.Find(ticket.StatusId).Name,
                        Modified = modified,
                        UserId = userId
                    };
                    db.Logs.Add(ticketlog);
                }

                if (oldTic?.TypeId != ticket.TypeId)
                {
                    Log ticketlog = new Log
                    {
                        TicketId = ticket.Id,
                        Property = "Type",
                        ChangedOld = db.TicketTypes.Find(oldTic?.TypeId).Name,
                        ChangedNew = db.TicketTypes.Find(ticket.TypeId).Name,
                        Modified = modified,
                        UserId = userId
                    };
                    db.Logs.Add(ticketlog);
                }

                db.SaveChanges();
                return RedirectToAction("Index", "Tickets");

                // See "EditHelper.cs"
                //db.Update(ticket, "Title", "Description", "AssignedToUserId", "TicketTypeId", "TicketPriorityId", "TicketStatusId", "ProjectId");
            }

            ViewBag.PriorityId = new SelectList(db.TicketPriorities, "Id", "Name");
            ViewBag.StatusId = new SelectList(db.TicketStatuses, "Id", "Name");
            ViewBag.TypeId = new SelectList(db.TicketTypes, "Id", "Name");
            return View(ticket);
        }

        // GET: Tickets/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            return View(ticket);
        }

        // POST: Tickets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Ticket ticket = db.Tickets.Find(id);
            db.Tickets.Remove(ticket);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        // GET: Tickets/Assign/5
        public ActionResult Assign(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }

            AssignUserToTicketViewModel ViewModel = new AssignUserToTicketViewModel();

            UserRolesHelper helper = new UserRolesHelper();
            var devRole = helper.UsersInRole("Developer").Where(u => u.Projects.Any(p => p.Id == ticket.ProjectId));
            var selected = ticket.AssigneeUserId;

            ViewModel.Users = new SelectList(devRole, "Id", "FullName", selected);
            ViewModel.Ticket = ticket;


            return View(ViewModel);
        }

        // POST: Tickets/Assign/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Assign([Bind(Include = "Id, LastModified, AssigneeUserId")] Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                ticket.LastModified = System.DateTimeOffset.Now;
                db.Tickets.Attach(ticket);
                db.Entry(ticket).Property("AssigneeUserId").IsModified = true;
                db.Entry(ticket).Property("LastModified").IsModified = true;

                db.SaveChanges();
                return RedirectToAction("Index", "Tickets");
            }
            return View();
        }
    }
}
