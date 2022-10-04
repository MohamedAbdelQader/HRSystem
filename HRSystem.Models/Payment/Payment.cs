using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRSystem.Models
{
    public class Payment
    {
        public int Id { get; set; }
        public string EmployeeID { get; set; }
        public decimal Salary { get; set; }
        public DateTime RecieveDate { get; set; }
        public virtual Employee Employee { get; set; }

    }
}
