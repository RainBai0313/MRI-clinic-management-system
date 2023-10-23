using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace _5032Project_v2.Models
{
    public class AppointmentDetailsViewModel
    {
        public Appointment Appointment { get; set; }
        public string ClinicName { get; set; }

        public virtual List<MRIRecord> MRIRecords { get; set; }
    }
}