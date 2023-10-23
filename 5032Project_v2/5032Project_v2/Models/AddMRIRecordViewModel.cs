using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace _5032Project_v2.Models
{
    public class AddMRIRecordViewModel
    {
        public int AppointmentId { get; set; }
        public string Result { get; set; }

        public HttpPostedFileBase MRIAttachment { get; set; }
    }
}