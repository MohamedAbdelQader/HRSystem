using HRSystem.Models;
using HRSystem.Repositories;
using HRSystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRSystem.MVC
{
    public class GroupController : Controller
    {
        private readonly GroupRepository GrRepo;
        private readonly EmployeeRepository EmpRepo;
        private readonly UnitOfWork UnitOfWork;
        public GroupController(EmployeeRepository _EmpRepo,GroupRepository _GrRepo, UnitOfWork unitOfWork)
        {
            EmpRepo = _EmpRepo;
            GrRepo = _GrRepo;
            UnitOfWork = unitOfWork;
        }
        [Authorize(Roles = "hr")]
        public IActionResult Groups()
        {
            ViewBag.Groups = GrRepo.GetList().ToList();
            return View();
        }
        [Authorize(Roles ="hr")]
        [HttpGet]
        public IActionResult AddEmployees(int ID)
        {
            var Emp= EmpRepo.GetList().Where(g=>g.GroupID!=ID).ToList();
            List<EmployeeViewModel> EmpViews = new List<EmployeeViewModel>();
            for(int i = 0; i < Emp.Count; i++)
            {
                EmpViews.Add(new EmployeeViewModel
                {
                    GroupName = Emp[i].Group.Name,
                    UserId = Emp[i].UserId,
                    FullName = Emp[i].FullName,
                    GroupID = ID
                });
            }
            //var Group = GrRepo.GetByID(ID);
            return View(EmpViews);
        }
        [Authorize(Roles = "hr")]
        [HttpPost]
        public async Task<IActionResult> AddEmployees(EmployeeViewModel emp)
        {
            var Employee = EmpRepo.GetByID(emp.UserId);
            var Group = GrRepo.GetByID(emp.GroupID);
            var res = await GrRepo.AddToGroup(Employee, Group);
            UnitOfWork.Save();
            return RedirectToAction("Groups");
        }
        [Authorize(Roles = "hr")]
        [Route("AddGroup")]
        [HttpGet]
        public IActionResult AddGroup()
        {
            return View();
        }
        [Authorize(Roles = "hr")]
        [Route("AddGroup")]
        [HttpPost]
        public IActionResult AddGroup(Group obj)
        {
            var result = GrRepo.Add(obj);
            UnitOfWork.Save();
            return RedirectToAction("Groups");
        }
        public IActionResult Remove(int ID)
        {
            var Emp = EmpRepo.GetList().Where(g=>g.GroupID==ID).ToList();
            for(int i = 0; i < Emp.Count; i++)
            {
                Emp[i].GroupID = 0;
            }
            var result = GrRepo.Remove(ID);
            UnitOfWork.Save();
            return RedirectToAction("Groups");
        }
    }
}
