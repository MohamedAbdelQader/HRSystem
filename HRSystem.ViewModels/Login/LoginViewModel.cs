using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRSystem.ViewModels
{
    public class LoginViewModel
    {
        [Required, StringLength(50, MinimumLength = 8)]
        public string? Email { get; set; }
        [Required, DataType(DataType.Password), StringLength(50, MinimumLength = 6)]
        public string? Password { get; set; }
        [Required, Display(Name = "Remember Me")]
        public bool RememberMe { get; set; }
        public bool IsDeleted { get; set; }
    }
}
