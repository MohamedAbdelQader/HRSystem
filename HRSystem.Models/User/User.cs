using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRSystem.Models
{
    public class User : IdentityUser
    {
        public bool IsDeleted { get; set; }
        public string Img { get; set; }
        public virtual Employee Employee { get; set; }
        public DateTime BirthDate { get; set; }
    }
}
