using HRSystem.Models;
using HRSystem.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HRSystem.MVC
{
    public class AttendanceController : Controller
    {
        private readonly AttendanceRepository AttRepo;
        private readonly UnitOfWork UnitOfWork;
        public AttendanceController(UnitOfWork _UnitOfWork,AttendanceRepository attRepo)
        {
            UnitOfWork = _UnitOfWork;
            AttRepo = attRepo;
        }
        [Authorize(Roles = "hr")]
        [HttpGet]
        public IActionResult Get(int ID=0,string? EmployeeName= null, string? AttendanceDate=null,
            bool isAscending = false, int pageIndex = 1,int pageSize = 20)
        {
            var result = AttRepo.Get(ID, EmployeeName, AttendanceDate, isAscending, pageIndex, pageSize);
            return View(result);
        }
        [Authorize(Roles = "hr")]
        [HttpGet]
        public IActionResult GetAttendance(string EmpID)
        {
            var Emp = AttRepo.GetList().Where(e => e.EmployeeID == EmpID).ToList();
            return View(Emp);
        }
        [HttpGet]
        public IActionResult StartAttendance()
        {
            var att = AttRepo.GetForAttendance(User.FindFirstValue(ClaimTypes.NameIdentifier));
            return View("Attendance",att);
        }
        [HttpPost]
        public IActionResult StartAttendance(Attendance obj)
        {
            obj.EmployeeID = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = AttRepo.Add(obj);
            UnitOfWork.Save();
            return View("Attendance",result);
        }
        [HttpPost]
        public IActionResult EndAttendance(Attendance obj)
        {
            obj.EmployeeID = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = AttRepo.EndAttendance(obj);
            UnitOfWork.Save();
            return View("Attendance", result);
        }
    }
}
