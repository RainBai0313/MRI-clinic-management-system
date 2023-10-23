using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace _5032Project_v2.Models
{
    public partial class ClinicModel : DbContext
    {
        public ClinicModel()
            : base("name=ClinicModel")
        {
        }

        public virtual DbSet<Clinic> Clinics { get; set; }
        public virtual DbSet<MRIRecord> MRIRecords { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
        }
    }
}
