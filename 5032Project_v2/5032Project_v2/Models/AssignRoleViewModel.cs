using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace _5032Project_v2.Models
{
    public class AssignRoleViewModel
    {
        public string UserId { get; set; }
        public string RoleName { get; set; }  // This should be a single string
        public List<string> AvailableRoles { get; set; } = new List<string>();
    }
}