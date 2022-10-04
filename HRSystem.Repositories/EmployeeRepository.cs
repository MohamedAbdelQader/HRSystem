using HRSystem.Models;
using HRSystem.ViewModels;
using LinqKit;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRSystem.Repositories
{
    public class EmployeeRepository : GeneralRepositories<Employee>
    {
        private readonly RoleManager<IdentityRole> RoleManager;
        private readonly UserManager<User> UserManager;
        private readonly HRRepository HRRepo;

        private readonly AttendanceRepository AttRepo;
        public EmployeeRepository(AttendanceRepository _AttRepo,RoleManager<IdentityRole> _RoleManager ,HRRepository _HRRepo, UserManager<User> _UserManager,HRContext Context) :base(Context)
        {
         
            AttRepo = _AttRepo;
            RoleManager = _RoleManager;
            HRRepo = _HRRepo;
            UserManager = _UserManager;
        }
        public PaginingViewModel<List<EmployeeViewModel>> Get(string? _ID = null, string? Email = null,string? Address = null, bool isAscending = false, int pageIndex = 1,
                        int pageSize = 20)
        {
            var filter = PredicateBuilder.New<Employee>();
            var oldFiler = filter;
            if (_ID != null)
                filter = filter.Or(p => p.UserId.Contains(_ID));
            if (!string.IsNullOrEmpty(Email))
                filter = filter.Or(p => p.User.Email.Contains(Email));
            if (!string.IsNullOrEmpty(Address))
                filter = filter.Or(p => p.Address.Contains(Address));
            if (filter == oldFiler)
                filter = null;
            var query = base.Get(filter, isAscending, pageIndex, pageSize);

            var result =
            query.Select(i => new EmployeeViewModel()
            {
                UserId = i.UserId,
                Address = i.Address,
                FirstDay = i.HolidayFirstDay,
                SecondDay = i.HolidaySecondDay,
                Email = i.User.Email,
                AttendanceDate = i.Attendances.Select(a=>a.AttendanceDate).FirstOrDefault(),
                IsDeleted = i.User.IsDeleted,
                Phone = i.User.PhoneNumber,
                BirthDate = i.User.BirthDate,
                CheckOutDate = i.Attendances.Select(a => a.CheckOutDate).FirstOrDefault(),
                DateOfContract = i.DateOfContract,
                FullName = i.FullName,
                Gender = i.Gender,
                Nationality = i.Nationality,
                NID = i.NID,
                Salary = i.Salary,
                LastSalary = (decimal)((float)i.Salary + i.Additions - i.Discounts),
            });

            PaginingViewModel<List<EmployeeViewModel>>
                finalResult = new PaginingViewModel<List<EmployeeViewModel>>()
                {
                    PageIndex = pageIndex,
                    PageSize = pageSize,
                    Count = base.GetList().Count(),
                    Data = result.ToList()
                };

            return finalResult;
        }
        public Employee GetByID(string ID)
        {
            var filter = PredicateBuilder.New<Employee>();
            if (ID != null)
                filter = filter.Or(u => u.UserId == ID);
            var query = GetByID(filter);
            return query;
        }
        public async Task<Employee> UpdateGroup(Employee Emp)
        {
            return base.Update(Emp).Entity;
        }
        public async Task<Employee> Add(EmployeeEditViewModel model)
        {

            var user = new User
            {
                Email = model.Email,
                UserName = model.Email,
                PasswordHash = model.Password,
                IsDeleted = model.IsDeleted,
                BirthDate = model.BirthDate,
                PhoneNumber = model.Phone,
            };
                user.Img = model.Img;
            
            var how = await UserManager.CreateAsync(user,model.Password);
            if (how.Succeeded)
            {
                var Holidays = GetList().FirstOrDefault();
                var UserForRole = HRRepo.GetByEmail(model.Email);

                if (Holidays != null)
                {
                    var Emp = base.Add(new Employee
                    {
                        UserId = UserForRole.Id,
                        Address = model.Address,
                        DateOfContract = model.DateOfContract,
                        FullName = model.FullName,
                        Gender = model.Gender,
                        Nationality = model.Nationality,
                        NID = model.NID,
                        Salary = model.Salary,
                        HolidayFirstDay = Holidays.HolidayFirstDay,
                        HolidaySecondDay = Holidays.HolidaySecondDay,

                        GroupID = model.GroupID
                    }).Entity;
                    if (Emp.HolidayFirstDay.Year >= DateTime.Now.Year)
                    {
                        var Att = AttRepo.Add(new Attendance
                        {
                            AttendanceDate = Emp.HolidayFirstDay.Add(new TimeSpan(9, 0, 0)),
                            CheckOutDate = Emp.HolidayFirstDay.Add(new TimeSpan(17, 0, 0)),
                            EmployeeID = Emp.UserId
                        });
                        Emp.CounterOfMonth++;
                        Emp.NumberHolidays++;
                    }
                    if (Emp.HolidaySecondDay.DayOfYear > Emp.HolidayFirstDay.DayOfYear)
                    {
                        var att2 = AttRepo.Add(new Attendance
                        {
                            AttendanceDate = Emp.HolidaySecondDay.Add(new TimeSpan(9, 0, 0)),
                            CheckOutDate = Emp.HolidaySecondDay.Add(new TimeSpan(17, 0, 0)),
                            EmployeeID = Emp.UserId
                        });
                        Emp.CounterOfMonth++;
                        Emp.NumberHolidays++;
                    }

                    


                    var GetRoleEmployee = RoleManager.Roles.Select(r => r.Name).Where(r => r == "Employee").FirstOrDefault();
                    if (GetRoleEmployee == null)
                    {
                        var roleEmployee = await RoleManager.CreateAsync(new IdentityRole
                        {
                            Name = "Employee",
                        });
                    }
                    else
                    {
                        var addroleEmployee = await UserManager.AddToRoleAsync(UserForRole, "Employee");
                    }
                    var GetRole = RoleManager.Roles.Select(r => r.Name).Where(r => r == model.Role).FirstOrDefault();
                    if (GetRole == null)
                    {
                        var role = await RoleManager.CreateAsync(new IdentityRole
                        {
                            Name = model.Role,
                        });
                        var addedwithrole = await UserManager.AddToRoleAsync(UserForRole, model.Role);
                        return Emp;
                    }
                    else
                    {
                        var addedwithrole = await UserManager.AddToRoleAsync(UserForRole, model.Role);
                        return Emp;
                    }
                }
                else
                {
                    var Emp = base.Add(new Employee
                    {
                        UserId = UserForRole.Id,
                        Address = model.Address,
                        DateOfContract = model.DateOfContract,
                        FullName = model.FullName,
                        Gender = model.Gender,
                        Nationality = model.Nationality,
                        NID = model.NID,
                        Salary = model.Salary,
                        LastSalary = model.Salary,
                        GroupID = model.GroupID
                    }).Entity;
                    var GetRoleEmployee = RoleManager.Roles.Select(r => r.Name).Where(r => r == "Employee").FirstOrDefault();
                    if (GetRoleEmployee == null)
                    {
                        var roleEmployee = await RoleManager.CreateAsync(new IdentityRole
                        {
                            Name = "Employee",
                        });
                    }
                    else
                    {
                        var addroleEmployee = await UserManager.AddToRoleAsync(UserForRole, "Employee");
                    }
                    var GetRole = RoleManager.Roles.Select(r => r.Name).Where(r => r == model.Role).FirstOrDefault();
                    if (GetRole == null)
                    {
                        var role = await RoleManager.CreateAsync(new IdentityRole
                        {
                            Name = model.Role,
                        });
                        var addedwithrole = await UserManager.AddToRoleAsync(UserForRole, model.Role);
                        return Emp;
                    }
                    else
                    {
                        var addedwithrole = await UserManager.AddToRoleAsync(UserForRole, model.Role);
                        return Emp;
                    }
                }
            }
            else
                return new Employee();
        }
        public async Task<Employee> Update(EmployeeEditViewModel obj)
        {
            var Emp = GetByID(obj.UserId);
            var User = HRRepo.GetByID(obj.UserId);
            Emp.FullName = obj.FullName;
            Emp.Address = obj.Address;
            Emp.User.Email = obj.Email;
            Emp.User.PhoneNumber = obj.Phone;
            if(obj.Password!=null)
                await HRRepo.Password(User, obj.Password);
            var UpdatingUser = await UserManager.UpdateAsync(User);
            return base.Update(Emp).Entity;
        }
        public async Task<Employee> UpdateForHR(EmployeeEditViewModel obj)
        {
            var Emp = GetByID(obj.UserId);
            var User = HRRepo.GetByID(obj.UserId);
            if (obj.FirstDay.DayOfYear > DateTime.Now.DayOfYear && obj.FirstDay.Year >= DateTime.Now.Year) 
            {
                var FirstFilter = PredicateBuilder.New<Attendance>();
                if (obj.UserId != null) { 
                    FirstFilter = FirstFilter.And(u => u.EmployeeID == Emp.UserId);
                    FirstFilter = FirstFilter.And(u => u.AttendanceDate.DayOfYear == Emp.HolidayFirstDay.DayOfYear);
                }
                var AttFirst = AttRepo.GetByID(FirstFilter);
                           
                    AttFirst.AttendanceDate = obj.FirstDay.Date.Add(new TimeSpan(9, 0, 0));
                    AttFirst.CheckOutDate = obj.FirstDay.Date.Add(new TimeSpan (17,0,0));
                    if (obj.SecondDay.DayOfYear > obj.FirstDay.DayOfYear 
                    && obj.SecondDay.DayOfYear > DateTime.Now.DayOfYear 
                    && obj.SecondDay.Year >= DateTime.Now.Year)
                    {
                    var SecondFilter = PredicateBuilder.New<Attendance>();
                    if (obj.UserId != null)
                    {
                        SecondFilter = SecondFilter.And(u => u.EmployeeID == Emp.UserId);
                        SecondFilter = SecondFilter.And(u => u.AttendanceDate.DayOfYear == Emp.HolidaySecondDay.DayOfYear);
                        var AttSecond = AttRepo.GetByID(SecondFilter);

                        AttSecond.AttendanceDate = obj.SecondDay.Date.Add(new TimeSpan(9, 0, 0));
                        AttSecond.CheckOutDate = obj.SecondDay.Date.Add(new TimeSpan(17, 0, 0));
                        var updAttSecond = AttRepo.Update(AttSecond).Entity;
                    }
                    }
                    var updAttFirst = AttRepo.Update(AttFirst).Entity;
                
            }
            
            
            Emp.NID = obj.NID;
            Emp.Address = obj.Address;
            Emp.DateOfContract = obj.DateOfContract;
            Emp.HolidayFirstDay = obj.FirstDay;
            Emp.HolidaySecondDay = obj.SecondDay;
            Emp.User.Email = obj.Email;
            Emp.FullName = obj.FullName;
            Emp.User.PhoneNumber = obj.Phone;
            Emp.Salary = obj.Salary;
            Emp.WorkHours = obj.WorkHours;
            if(obj.Password!=null)
                await HRRepo.Password(User, obj.Password);
            var UpdatingUser = await UserManager.UpdateAsync(User);
            return base.Update(Emp).Entity;
        }
        public  EmployeeEditViewModel GetForEditing(User user,Employee Emp)=>
            new EmployeeEditViewModel
            {
                UserId = user.Id,
                NID = Emp.NID,
                Address = Emp.Address,
                DateOfContract = Emp.DateOfContract,
                FirstDay = Emp.HolidayFirstDay,
                SecondDay = Emp.HolidaySecondDay,
                Email = user.Email,
                FullName = Emp.FullName,
                Phone = user.PhoneNumber,
                Salary = Emp.Salary,
                Roles = UserManager.GetRolesAsync(user).Result.ToList(),
                WorkHours = Emp.WorkHours,
            };

        public void AddHolidays(HolidaysViewModel obj)
        {
            
            var Employees = base.GetList().ToList();
            for(int i = 0; i < Employees.Count; i++)
            {
                if(Employees[i].HolidayFirstDay.DayOfYear <= obj.FirstDay.DayOfYear) { 
                    Employees[i].HolidayFirstDay = obj.FirstDay;
                    var getAtt = AttRepo.GetList().Where(a => a.AttendanceDate.DayOfYear == obj.FirstDay.DayOfYear
                            && a.EmployeeID == Employees[i].UserId).FirstOrDefault();
                    if (getAtt == null) { 
                        var att = AttRepo.Add(new Attendance
                        {
                            AttendanceDate = Employees[i].HolidayFirstDay.Add(new TimeSpan(9, 0, 0)),
                            CheckOutDate = Employees[i].HolidayFirstDay.Add(new TimeSpan(17, 0, 0)),
                            EmployeeID = Employees[i].UserId
                        });
                        Employees[i].CounterOfMonth++;
                        Employees[i].NumberHolidays++;
                    }
                }
                if(Employees[i].HolidaySecondDay.DayOfYear <= obj.SecondDay.DayOfYear) { 
                if (obj.SecondDay > obj.FirstDay)
                {
                            Employees[i].HolidaySecondDay = obj.SecondDay;
                        var getAtt = AttRepo.GetList().Where(a => a.AttendanceDate.DayOfYear == obj.SecondDay.DayOfYear
                        && a.EmployeeID == Employees[i].UserId).FirstOrDefault();
                        if (getAtt == null)
                        {
                            var att2 = AttRepo.Add(new Attendance
                            {
                                AttendanceDate = Employees[i].HolidaySecondDay.Add(new TimeSpan(9, 0, 0)),
                                CheckOutDate = Employees[i].HolidaySecondDay.Add(new TimeSpan(17, 0, 0)),
                                EmployeeID = Employees[i].UserId
                            });
                            Employees[i].CounterOfMonth++;
                            Employees[i].NumberHolidays++;
                        }
                }
                }
               
                var result = Update(Employees[i]).Entity;
            }
           
        }
        public List<EmployeeViewModel> Salaries()
        {
            var GetAll = GetList().ToList();
            var result = new List<EmployeeViewModel>();
            foreach(var i in GetAll)
            {
                
                result.Add(new EmployeeViewModel
                {
                    FullName = i.FullName,
                    Salary = i.Salary,
                    LastSalary = i.LastSalary,
                    SalaryAfterMonth = i.SalaryAfterMonth,
                    Additions = i.Additions,
                    Discounts = i.Discounts,
                    CounterOfMonth = i.CounterOfMonth,
                    AdditionHours = i.Additions / ((float)i.Salary / 30 / i.WorkHours) / 2,
                    DiscountHours = i.Discounts / ((float)i.Salary / 30 / i.WorkHours) / 2,
                });
            }
            return result;
        }
        public Employee UpdateSalary(Employee obj)
        {
            var Get = GetByID(obj.UserId);
            Get.Salary = obj.Salary;
            Get.Additions = obj.Additions;
            Get.Discounts = obj.Discounts;
            Get.LastSalary =(decimal)((float) obj.Salary + obj.Additions - obj.Discounts);
            return base.Update(Get).Entity;
        }
    }
}
