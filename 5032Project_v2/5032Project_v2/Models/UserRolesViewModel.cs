using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace _5032Project_v2.Models
{
    public class UserRolesViewModel
    {
        public string UserId { get; set; }
        public string Username { get; set; }
        public List<string> RoleNames { get; set; }
        public List<string> AllRoles { get; set; }
    }
}