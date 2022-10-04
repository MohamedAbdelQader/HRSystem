using HRSystem.Models;
using LinqKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRSystem.Repositories
{
    public class OfficialHolidaysRepository : GeneralRepositories<OfficialHolidays>
    {
        private readonly EmployeeRepository EmpRepo;
        private readonly AttendanceRepository AttRepo;
        public OfficialHolidaysRepository(AttendanceRepository _AttRepo, EmployeeRepository _EmpRepo,HRContext _DBContext) : base(_DBContext)
        {
            AttRepo = _AttRepo;
            EmpRepo = _EmpRepo;
        }

        public new OfficialHolidays Add(OfficialHolidays obj)
        {
            var Off = base.Add(obj).Entity;
            var Different = obj.ToDate.DayOfYear - obj.FromDate.DayOfYear;

            var Employees = EmpRepo.GetList().ToList();
            for (int i = 0; i < Employees.Count(); i++)
            {
                for(int x = 0; x <= Different; x++) { 
                var Att = AttRepo.Add(new Attendance
                {
                    AttendanceDate = obj.FromDate.Date.AddDays(x).Add(new TimeSpan(9, 0, 0))
                    ,CheckOutDate = obj.FromDate.Date.AddDays(x).Add(new TimeSpan(17,0,0))
                    ,EmployeeID = Employees[i].UserId
                });
                    Employees[i].CounterOfMonth++;
                    Employees[i].NumberHolidays++;
                }
            }
            return Off;
        }
        public bool Remove(int ID)
        {
            var filter = PredicateBuilder.New<OfficialHolidays>();
            if (ID != 0)
                filter = filter.Or(o => o.ID == ID);
            var result = base.GetByID(filter);
            if (result != null)
            {
                var different = result.ToDate.DayOfYear - result.FromDate.DayOfYear;
                for (int x = 0; x <= different; x++)
                {
                    var Attfilter = AttRepo.GetList()
                        .Where(f => f.AttendanceDate.Date == result.FromDate.AddDays(x)).ToList();
                    for (int i = 0; i < Attfilter.Count; i++)
                    {
                        Attfilter[i].Employee.CounterOfMonth--;
                        Attfilter[i].Employee.NumberHolidays--;
                        var AttRemove = AttRepo.Remove(Attfilter[i]).Entity;

                    }
                }
                var removing = base.Remove(result).Entity;
                return true;
            }
            else return false;
        }
            
    }
}
