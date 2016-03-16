using BugDS.Models.CodeFirst;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BugDS.Models
{
    public class CreateEditTicketViewModel
    {
        public Ticket Ticket { get; set; }

        public int ProjectId { get; set; }
        public SelectList Projects { get; set; }
        public string SelectedProject { get; set; }

        public int PriorityId { get; set; }
        public SelectList Priorities { get; set; }
        public string SelectedPriority { get; set; }

        public int StatusId { get; set; }
        public SelectList Statuses { get; set; }
        public string SelectedStatus { get; set; }

        public int TypeId { get; set; }
        public SelectList Types { get; set; }
        public string SelectedType { get; set; }

    }
}