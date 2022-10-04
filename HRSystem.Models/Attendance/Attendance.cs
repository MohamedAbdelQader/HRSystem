using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRSystem.Models
{
    public class Attendance
    {
        public int ID { get; set; }
        public string EmployeeID { get; set; }
        public DateTime AttendanceDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public virtual Employee Employee { get; set; }
    }
}
