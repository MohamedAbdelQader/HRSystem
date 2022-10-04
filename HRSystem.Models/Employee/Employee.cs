using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRSystem.Models
{
    public enum Gender
    {
        Male =1,
        Female =2
    }
    public class Employee
    {
        public string UserId { get; set; }
        public string FullName { get; set; }
        public string NID { get; set; }
        public int GroupID { get; set; }
        public decimal Salary { get; set; }
        public decimal LastSalary { get; set; }
        public decimal SalaryAfterMonth { get; set; }
        public string Address { get; set; }
        public string Nationality { get; set; }
        public float Additions { get; set; }
        public float Discounts { get; set; }
        public int CounterOfMonth { get; set; } = 0;
        public int WorkHours { get; set; } = 8;
        public int NumberHolidays { get; set; } = 0;
        public Gender Gender { get; set; }
        public DateTime HolidayFirstDay { get; set; }
        public DateTime HolidaySecondDay { get; set; }
        public DateTime DateOfContract { get; set; }
        public DateTime ReceivingMoneyTime { get; set; }
        public virtual User User { get; set; }
        public virtual Group Group { get; set; }
        public virtual ICollection<Attendance> Attendances { get; set; }
        public virtual ICollection<Payment> Payments { get; set; }
    }
}
