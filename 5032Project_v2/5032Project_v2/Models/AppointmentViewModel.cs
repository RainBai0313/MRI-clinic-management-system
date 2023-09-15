using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace _5032Project_v2.Models
{
    public class AppointmentViewModel
    {
        public int AppointmentId { get; set; }
        public DateTime DateTime { get; set; }
        public int Status { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

    }

}