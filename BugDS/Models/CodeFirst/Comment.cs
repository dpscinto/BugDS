using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BugDS.Models.CodeFirst
{
    public class Comment
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int TicketId { get; set; }
        public DateTimeOffset Created { get; set; }
        [AllowHtml]
        public string Description { get; set; }
        public string MediaURL { get; set; }       

        public virtual Ticket Ticket { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}