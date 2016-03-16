using BugDS.Models.CodeFirst;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BugDS.Models
{
    public class AssignUserToTicketViewModel
    {
        public Ticket Ticket { get; set; }
        public SelectList Users { get; set; }
    }
}