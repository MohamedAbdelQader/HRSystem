using HRSystem.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRSystem.ViewModels
{

    public class EmployeeEditViewModel
    {
        public string? UserId { get; set; }
        public string? Password { get; set; }
        [Required,MinLength(7),EmailAddress]
        public string? Email { get; set; }
        [Required ,MinLength(7)]
        public string? FullName { get; set; }
        [Required,MinLength(14),MaxLength(14)]
        public string? NID { get; set; }
        [Required]
        public decimal Salary { get; set; }
        public string? Address { get; set; }
        [Required]
        public string? Nationality { get; set; }
        public string? Role { get; set; }
        public int GroupID { get; set; }
        public int WorkHours { get; set; }
        public bool IsDeleted { get; set; }
        [Required]
        public Gender Gender { get; set; }
        public DateTime FirstDay { get; set; }
        public DateTime SecondDay { get; set; }
        [Required]
        public DateTime BirthDate { get; set; }
        [Required]
        public DateTime DateOfContract { get; set; }
        public DateTime AttendanceDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        [Required,MaxLength(11),MinLength(11)]
        public string? Phone { get; set; }
        public List<string>? Roles { get; set; }
        public string? Img { get; set; }
        public IFormFile? uploadedimg { get; set; }
    }
}
