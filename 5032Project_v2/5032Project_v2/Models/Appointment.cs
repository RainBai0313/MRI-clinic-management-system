namespace _5032Project_v2.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Appointment
    {
        public int AppointmentId { get; set; }

        [StringLength(128)]
        public string UserId { get; set; }

        public DateTime DateTime { get; set; }

        public int Status { get; set; }
    }
}
