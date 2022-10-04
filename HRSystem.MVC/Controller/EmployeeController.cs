using HRSystem.Models;
using HRSystem.Repositories;
using HRSystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HRSystem.MVC
{
    public class EmployeeController : Controller
    {
        private readonly EmployeeRepository EmpRepo;
        private readonly OfficialHolidaysRepository OffRepo;
        private readonly GroupRepository GrRepo;
        private readonly AttendanceRepository AttRepo;
        private readonly HRRepository HRRepo;
        private readonly UnitOfWork UnitOfWork;
        private readonly RoleManager<IdentityRole> RoleManager;
        private readonly UserManager<User> UserManager;
        public EmployeeController(GroupRepository _GrRepo, UserManager<User> _UserManager, HRRepository _HRRepo, AttendanceRepository _AttRepo, OfficialHolidaysRepository _OffRepo, RoleManager<IdentityRole> _RoleManager, UnitOfWork _UnitOfWork, EmployeeRepository _EmpRepo)
        {
            GrRepo = _GrRepo;
            UserManager = _UserManager;
            HRRepo = _HRRepo;
            AttRepo = _AttRepo;
            OffRepo = _OffRepo;
            RoleManager = _RoleManager;
            UnitOfWork = _UnitOfWork;
            EmpRepo = _EmpRepo;
        }
        [Authorize(Roles = "hr")]
        public IActionResult Employees(string? _ID = null, string? Email = null, string? Address = null, bool isAscending = false, int pageIndex = 1,
                        int pageSize = 20)
        {
            var AllEmployees = EmpRepo.Get(_ID, Email, Address, isAscending, pageIndex,
                        pageSize);
            return View(AllEmployees);
        }
        public IActionResult Index()
        {
            return View();
        }
        [Route("Employee/Add")]
        [HttpGet]
        public IActionResult Add()
        {
            ViewBag.Groups = GrRepo.GetList().ToList();

            return View();
        }
        [Route("Employee/Add")]
        [HttpPost]
        public async Task<IActionResult> Add(EmployeeEditViewModel model)
        {
            if (model != null)
            {
                if (model.FullName != null && model.FullName.Length > 6)
                {
                    if (model.BirthDate.Year < 2002)
                    {
                        if (model.Email != null && model.Email.Length > 8)
                        {
                            if (model.Password != null && model.Password.Length >= 8)
                            {
                                if (model.Phone != null && model.Phone.Length == 11)
                                {
                                    if (model.DateOfContract.Year > 1995)
                                    {
                                        if (model.Address != null && model.Address.Length > 3)
                                        {
                                            if (model.Nationality != null && model.Nationality.Length > 4)
                                            {
                                                if (model.NID.Length > 13)
                                                {
                                                    if (model.Salary > 3000)
                                                    {
                                                        if (model.uploadedimg != null)
                                                        {
                                                            string Uploade = "/Content/Uploads/EmployeeImg/";
                                                            IFormFile? s = model.uploadedimg;
                                                            string NewFileName = Guid.NewGuid().ToString() + s.FileName;
                                                            model.Img = Uploade + NewFileName;
                                                            FileStream fs = new FileStream(Path.Combine(
                                                                Directory.GetCurrentDirectory(), "Content", "Uploads", "EmployeeImg", NewFileName
                                                                ), FileMode.Create);
                                                            s.CopyTo(fs);
                                                            fs.Position = 0;
                                                            model.Role = "Employee";
                                                            if (model.GroupID == null ||model.GroupID==0)
                                                                model.GroupID = 0;
                                                            var result = await EmpRepo.Add(model);
                                                            var OfficialHolidays = OffRepo.GetList().ToList();
                                                            for (int i = 0; i < OfficialHolidays.Count(); i++)
                                                            {
                                                                var Different = OfficialHolidays[i].ToDate.DayOfYear - OfficialHolidays[i].FromDate.DayOfYear;
                                                                for (int x = 0; x <= Different; x++)
                                                                {
                                                                    var Att = AttRepo.Add(new Attendance
                                                                    {
                                                                        AttendanceDate = OfficialHolidays[i].FromDate.Date.AddDays(x).Add(new TimeSpan(9, 0, 0))
                                                                        ,
                                                                        CheckOutDate = OfficialHolidays[i].FromDate.Date.AddDays(x).Add(new TimeSpan(17, 0, 0))
                                                                        ,
                                                                        EmployeeID = result.UserId
                                                                    });
                                                                    result.CounterOfMonth++;
                                                                }
                                                            }
                                                            var Updating = EmpRepo.Update(result).Entity;
                                                            UnitOfWork.Save();
                                                            return RedirectToAction("Employees", "Employee");
                                                        }
                                                        else
                                                        {
                                                            ModelState.AddModelError("Img", "CV Is Required");
                                                            ViewBag.Groups = GrRepo.GetList().ToList();
                                                            return View();
                                                        }
                                                    }
                                                    else
                                                    {
                                                        ModelState.AddModelError("NID", "Salary is Required and more than 3000");
                                                        ViewBag.Groups = GrRepo.GetList().ToList();
                                                        return View();
                                                    }
                                                }
                                                else
                                                {
                                                    ModelState.AddModelError("NID", "The National ID Should be More than 13 Number");
                                                    ViewBag.Groups = GrRepo.GetList().ToList();
                                                    return View();
                                                }
                                            }
                                            else
                                            {
                                                ModelState.AddModelError("Nationality", "Nationality is Required");
                                                ViewBag.Groups = GrRepo.GetList().ToList();
                                                return View();
                                            }
                                        }
                                        else
                                        {
                                            ModelState.AddModelError("Address", "The Address is Required");
                                            ViewBag.Groups = GrRepo.GetList().ToList();
                                            return View();
                                        }
                                    }
                                    else
                                    {
                                        ModelState.AddModelError("DateOfContract", "Date of Contact Must be after 1995");
                                        ViewBag.Groups = GrRepo.GetList().ToList();
                                        return View();
                                    }

                                }

                                else
                                {
                                    ModelState.AddModelError("Phone", "Phone Number Should be 11 number");
                                    ViewBag.Groups = GrRepo.GetList().ToList();
                                    return View();
                                }
                            }
                            else
                            {
                                ModelState.AddModelError("Password", "You Should Enter Strong Password");
                                ViewBag.Groups = GrRepo.GetList().ToList();
                                return View();
                            }
                        }
                        else
                        {
                            ModelState.AddModelError("Email", "Enter Right Email Please");
                            ViewBag.Groups = GrRepo.GetList().ToList();
                            return View();
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("BirthDate", "You Should be older than 20");
                        ViewBag.Groups = GrRepo.GetList().ToList();
                        return View();
                    }
                }
                else
                {
                    ModelState.AddModelError("FullName", "Enter Correct Name please");
                    ViewBag.Groups = GrRepo.GetList().ToList();
                    return View();
                }
            }
            else
            {
                ModelState.AddModelError("AllData", "Please , Enter all data ");
                ViewBag.Groups = GrRepo.GetList().ToList();
                return View();
            }
        }
        [Route("Employee/Update")]
        [HttpGet]
        public IActionResult Update()
        {

            var Emp = EmpRepo.GetByID(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var EmpEdit = new EmployeeEditViewModel
            {

                Address = Emp.Address,
                Email = Emp.User.Email,
                FullName = Emp.FullName,
                Phone = Emp.User.PhoneNumber,
            };
            return View(EmpEdit);
        }
        [Route("Employee/Update")]
        [HttpPost]
        public async Task<IActionResult> Update(EmployeeEditViewModel obj)
        {
            obj.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await EmpRepo.Update(obj);
            UnitOfWork.Save();
            return RedirectToAction("Employees", "Employee");
        }
        [Authorize(Roles = "hr")]
        [Route("Employee/UpdateForHR")]
        [HttpGet]
        public IActionResult UpdateForHR(string ID)
        {

            var Emp = EmpRepo.GetByID(ID);
            var user = HRRepo.GetByID(ID);
            var EmpEdit = EmpRepo.GetForEditing(user, Emp);

            return View(EmpEdit);
        }
        [Authorize(Roles = "hr")]
        [Route("Employee/UpdateForHR")]
        [HttpPost]
        public async Task<IActionResult> UpdateForHR(EmployeeEditViewModel obj)
        {

            var result = await EmpRepo.UpdateForHR(obj);
            UnitOfWork.Save();
            return RedirectToAction("Employees", "Employee");
        }
        [Authorize(Roles = "hr")]
        [Route("Holidays/Add")]
        [HttpGet]
        public IActionResult AddHolidays()
        {
            return View();
        }
        [Authorize(Roles = "hr")]
        [Route("Holidays/Add")]
        [HttpPost]
        public IActionResult AddHolidays(HolidaysViewModel obj)
        {
            EmpRepo.AddHolidays(obj);
            UnitOfWork.Save();
            return RedirectToAction("Employees");
        }

        [Authorize(Roles = "hr")]
        [HttpGet]
        public IActionResult Salaries()
        {
            var get = EmpRepo.Salaries();
            return View(get);
        }
        [Authorize(Roles = "hr")]
        [HttpGet]
        public IActionResult UpdateSalary(string ID)
        {
            var Emp = EmpRepo.GetByID(ID);
            return View(Emp);
        }
        [Authorize(Roles = "hr")]
        [HttpPost]
        public IActionResult UpdateSalary(Employee obj)
        {
            var result = EmpRepo.UpdateSalary(obj);
            UnitOfWork.Save();
            return RedirectToAction("Salaries");
        }
        [HttpGet]
        public IActionResult Profile()
        {
            var Emp = EmpRepo.GetByID(User.FindFirstValue(ClaimTypes.NameIdentifier));
            return View(Emp);
        }
        public IActionResult EmpAttendance()
        {
            var result = AttRepo.GetList().Where(a => a.EmployeeID
                         == User.FindFirstValue(ClaimTypes.NameIdentifier)).ToList();
            return View(result);
        }
    }
}
