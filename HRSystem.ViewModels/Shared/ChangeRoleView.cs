using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRSystem.ViewModels
{
    public class ChangeRoleView
    {
        public string? UserID { get; set; }
        public string? UserName { get; set; }
        public List<string>? UserRoles { get; set; }
        public List<string>? AllRoles { get; set; }
        public string? RoleName { get; set; }
    }
}
