using HRSystem.Models;
using HRSystem.ViewModels;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRSystem.Repositories
{
    public class AttendanceRepository : GeneralRepositories<Attendance>
    {

        private readonly PaymentRepository PayRepo;
        public AttendanceRepository(PaymentRepository _PayRepo,HRContext context) :base(context)
        {
            PayRepo = _PayRepo;
        }
        public PaginingViewModel<List<Attendance>> Get(int _ID = 0,string? EmployeeName = null ,
            string? AttendanceDate= null, bool isAscending = false, int pageIndex = 1,
                       int pageSize = 20)
        {
            var filter = PredicateBuilder.New<Attendance>();
            var oldFiler = filter;
            if (_ID != 0)
                filter = filter.Or(p => p.ID==_ID);
            if (!string.IsNullOrEmpty(EmployeeName))
                filter = filter.Or(p => p.Employee.FullName.Contains(EmployeeName));
            if (!string.IsNullOrEmpty(AttendanceDate))
                filter = filter.Or(p => p.AttendanceDate.ToShortDateString().Contains(AttendanceDate));
            if (filter == oldFiler)
                filter = null;
            var query = base.Get(filter, isAscending, pageIndex, pageSize);

            var result =
            query.Select(i => new Attendance()
            {
                ID = i.ID,
                EmployeeID = i.EmployeeID,
                AttendanceDate = i.AttendanceDate,
                CheckOutDate = i.CheckOutDate,
                Employee = i.Employee,
            });

            PaginingViewModel<List<Attendance>>
                finalResult = new PaginingViewModel<List<Attendance>>()
                {
                    PageIndex = pageIndex,
                    PageSize = pageSize,
                    Count = base.GetList().Count(),
                    Data = result.ToList()
                };

            return finalResult;
        }
        public new Attendance Add(Attendance obj)=>
        base.Add(obj).Entity;

        public Attendance GetForAttendance(string ID)
        {
            var filter = PredicateBuilder.New<Attendance>();
            if (ID != null) { 
                filter = filter.And(f => f.EmployeeID == ID);
                filter = filter.And(f => f.AttendanceDate.DayOfYear==DateTime.Today.DayOfYear);
                }
            var result = base.GetByID(filter);
            if (result != null) {
                if (result.AttendanceDate.Date.DayOfYear == DateTime.Today.DayOfYear)
                {
                    return result;
                }
                else return new Attendance();
            }
            else return new Attendance();
        }
        public Attendance EndAttendance(Attendance obj)
        {
            var filter = PredicateBuilder.New<Attendance>();
            if (obj.EmployeeID != null)
            {
                filter = filter.And(f => f.EmployeeID == obj.EmployeeID);
                filter = filter.And(f => f.AttendanceDate.Date.DayOfYear == DateTime.Today.DayOfYear);
            }
            var result = base.GetByID(filter);
            result.CheckOutDate = obj.CheckOutDate;
            var Additions = result.CheckOutDate - result.AttendanceDate;
            decimal Salary = result.Employee.Salary / 30 /result.Employee.WorkHours;
            if (Additions.Hours > result.Employee.WorkHours)
            {
                result.Employee.Additions += (Additions.Hours - result.Employee.WorkHours)
                    * (float)Salary * 2;
                result.Employee.LastSalary = (decimal)((float)result.Employee.Salary + result.Employee.Additions
                    - result.Employee.Discounts);

            }
            else if(Additions.Hours< result.Employee.WorkHours){
                result.Employee.Discounts -= (Additions.Hours - result.Employee.WorkHours)
                    * (float)Salary * 2;
                result.Employee.LastSalary = (decimal)((float)result.Employee.Salary + result.Employee.Additions
                    - result.Employee.Discounts);
                
            }
            if (DateTime.Compare(result.Employee.ReceivingMoneyTime, DateTime.Now) == 0)
            {
                result.Employee.LastSalary = result.Employee.Salary / 30 * result.Employee.CounterOfMonth;
                result.Employee.SalaryAfterMonth = (decimal)((float)result.Employee.LastSalary +
                                result.Employee.Additions - result.Employee.Discounts);
                result.Employee.CounterOfMonth = 0;
                result.Employee.Additions = 0;
                result.Employee.Discounts = 0;
                result.Employee.LastSalary = 0;
                var Payments = PayRepo.Add(new Payment
                {
                    EmployeeID = result.EmployeeID,
                    RecieveDate = result.Employee.ReceivingMoneyTime,
                    Salary = result.Employee.SalaryAfterMonth,
                });
                result.Employee.ReceivingMoneyTime = DateTime.Now.AddDays(30);
            }
            result.Employee.CounterOfMonth++;
            return base.Update(result).Entity;
        }
        
    }
}
