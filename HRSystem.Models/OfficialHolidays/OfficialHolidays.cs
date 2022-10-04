using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRSystem.Models
{
    public class OfficialHolidays
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public DateTime FromDate { get; set; } 
        public DateTime ToDate { get; set; } 
    }
}
