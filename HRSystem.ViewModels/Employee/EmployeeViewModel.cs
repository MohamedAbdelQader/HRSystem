using HRSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRSystem.ViewModels
{
    public class EmployeeViewModel
    {
        public string? UserId { get; set; }
        public string? Email { get; set; }
        public string? FullName { get; set; }
        public string? NID { get; set; }
        public string? GroupName { get; set; }
        public int GroupID { get; set; }
        public decimal Salary { get; set; }
        public decimal LastSalary { get; set; }
        public decimal SalaryAfterMonth { get; set; }
        public float Additions { get; set; }
        public float Discounts { get; set; }
        public string? Address { get; set; }
        public string? Nationality { get; set; }
        public int CounterOfMonth { get; set; }
        public float AdditionHours { get; set; }
        public float DiscountHours { get; set; }
        public int WorkHours { get; set; }
        public bool IsDeleted { get; set; }
        public Gender Gender { get; set; }
        public DateTime FirstDay { get; set; }
        public DateTime SecondDay { get; set; }
        public DateTime BirthDate { get; set; }
        public DateTime DateOfContract { get; set; }
        public DateTime AttendanceDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public string? Phone { get; set; }
    }
}
