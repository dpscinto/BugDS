using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugDS.Models.CodeFirst
{
    public class Log
    {
        public int Id { get; set; }
        public string TicketId { get; set; }
        public string UserId { get; set; }
        public string Property { get; set; }
        public string ChangedOld { get; set; }
        public string ChangedNew { get; set; }

        public virtual Ticket Ticket { get; set; }
        public virtual ApplicationUser User { get; set; }

    }
}