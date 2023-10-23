namespace _5032Project_v2.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Appointment
    {
        [Key]
        public int AppointmentId { get; set; }

        [StringLength(128)]
        public string UserId { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }
        public DateTime DateTime { get; set; }

        public int Status { get; set; }
        [Required]
        public int ClinicId { get; set; }

        public int? FeedbackRating { get; set; }

        public string FeedbackComment { get; set; }

        public virtual ICollection<MRIRecord> MRIRecords { get; set; }
    }
}
