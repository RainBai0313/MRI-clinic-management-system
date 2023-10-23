using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;

namespace _5032Project_v2.Models
{
    public partial class AppointmentModel : DbContext
    {
        public AppointmentModel() : base("name=AppointmentModel") { }

        public virtual DbSet<Appointment> Appointments { get; set; }
        public virtual DbSet<MRIRecord> MRIRecords { get; set; }

        public virtual DbSet<Clinic> Clinics { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);
        }
    }
}
