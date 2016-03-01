using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BugDS.Models.CodeFirst
{
    public class Ticket
    {

        public Ticket()
        {
            this.Logs = new HashSet<Log>();
            this.Comments = new HashSet<Comment>();

        }

        public int Id { get; set; }
        public string Name { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset? Modified { get; set; }
        [AllowHtml]
        public string Description { get; set; }
        public string MediaURL { get; set; }
        public string CreatedUserId { get; set; }
        public string AssigneeUserId { get; set; }
        public string ProjectId { get; set; }
        public string PriorityId { get; set; }
        public string StatusId { get; set; }
        public string TypeId { get; set; }

        public virtual ApplicationUser CreatedUser { get; set; }
        public virtual ApplicationUser AssigneeUser { get; set; }
        public virtual Project Project { get; set; }
        public virtual TicketPriority Priority { get; set; }
        public virtual TicketStatus Status { get; set; }
        public virtual TicketType Type { get; set; }

        public virtual ICollection<Log> Logs { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }

    }
}