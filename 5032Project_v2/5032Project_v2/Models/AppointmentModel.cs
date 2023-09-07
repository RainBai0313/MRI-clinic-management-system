using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace _5032Project_v2.Models
{
    public partial class AppointmentModel : DbContext
    {
        public AppointmentModel()
            : base("name=AppointmentModel")
        {
        }

        public virtual DbSet<Appointment> Appointments { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
        }
    }
}
