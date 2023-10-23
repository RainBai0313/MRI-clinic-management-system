namespace _5032Project_v2.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Clinic
    {
        public int Id { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        [Required]
        [StringLength(255)]
        public string Email { get; set; }

        [StringLength(500)]
        public string ContactInfo { get; set; }

        public string ClinicName { get; set; }
    }
}
