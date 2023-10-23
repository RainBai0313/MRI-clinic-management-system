namespace _5032Project_v2.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("MRIRecord")]
    public partial class MRIRecord
    {
        public int Id { get; set; }

        public int AppointmentId { get; set; }
        public string Result { get; set; }
        public string FilePath { get; set; }
        public string FileName { get; set; }
    }
}
