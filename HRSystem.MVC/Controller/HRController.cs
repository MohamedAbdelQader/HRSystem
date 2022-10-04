using HRSystem.Models;
using HRSystem.Repositories;
using HRSystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HRSystem.MVC
{
    public class HRController : Controller
    {
        private readonly HRRepository HRRepo;
        private readonly UnitOfWork UnitOfWork;
        private readonly PaymentRepository PayRepo;
        private readonly UserManager<User> UserManager;
        private readonly RoleManager<IdentityRole> RoleManager;
        public HRController(PaymentRepository _PayRepo,RoleManager<IdentityRole> _RoleManager, UserManager<User> _UserManager, UnitOfWork _UnitOfWork
            , HRRepository _HRRepo)
        {
            PayRepo = _PayRepo;
            RoleManager = _RoleManager;
            UserManager = _UserManager;
            UnitOfWork = _UnitOfWork;
            HRRepo = _HRRepo;
        }
        [Authorize(Roles = "hr")]
        [Route("HR/AddHR")]
        [HttpGet]
        public IActionResult AddHR()
        {
            return View();
        }
        [Authorize(Roles = "hr")]
        [HttpPost]
        public async Task<IActionResult> AddHR(User obj)
        {

            obj.UserName = obj.Email;
            var result = await HRRepo.AddHR(obj);
            if (result.Succeeded == true)
            {
                UnitOfWork.Save();
                return RedirectToAction("login", "HR");
            }
            else
            {
                return View(result.Errors);
            }
        }
        [Route("Login")]
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [Route("Login")]
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel obj)
        {

            if (obj != null)
            {
                if (obj.Password != null)
                {
                    if (obj.Email != null)
                    {
                        var user = HRRepo.GetByEmail(obj.Email);
                        if (user != null)
                        {

                            var result = await HRRepo.SignIn(obj);
                            if (!result.Succeeded)
                            {
                                ModelState.AddModelError("", "Wrong Email or Password !!");
                            }
                            else if (result.IsLockedOut)
                            {
                                ModelState.AddModelError("", "Sorry, Please Try again Later ");
                            }
                            else
                            {
                                if (UserManager.GetRolesAsync(user).Result.FirstOrDefault() == "hr") { return RedirectToAction("Employees", "Employee"); }
                                else if (UserManager.GetRolesAsync(user).Result.FirstOrDefault() == "Employee") {
                                     
                                    return RedirectToAction("StartAttendance", "Attendance"); }
                            }
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Enter Correct Email");
                        return View();
                    }
                    
                }
                else
                {
                    ModelState.AddModelError("", "Enter Correct Password");
                    return View();
                }
            }
            else
            {
                ModelState.AddModelError("", "Enter Correct Email and Password");
                return View();
            }
            return View();
        }
        [HttpGet]
        [Route("SignOut")]
        public new async Task<IActionResult> SignOut()
        {
            await HRRepo.SignOut();
            return RedirectToAction("Login", "HR");
        }
        [Authorize(Roles = "hr")]
        [HttpGet]
        public IActionResult GetGroups()
        {
            var AllRoles = RoleManager.Roles.Select(e => new IdentityRole
            {
                Name = e.Name,
                Id = e.Id,
                NormalizedName = e.NormalizedName
            }).ToList();
            return View(AllRoles);
        }
        [HttpGet]
        public IActionResult GetPayment()
        {
            var result = PayRepo.GetList().ToList();
            return View(result);
        }
        [HttpGet]
        public IActionResult UpdateProfile()
        {
            var HR = HRRepo.GetByID(User.FindFirstValue(ClaimTypes.NameIdentifier));
            return View(HR);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateProfile(User obj)
        {
            var hr = await HRRepo.Update(obj);
            UnitOfWork.Save();
            return RedirectToAction("UpdateProfile");
        }
        [Authorize(Roles = "hr")]
        [HttpGet]
        public async Task<IActionResult> SoftDelete(string ID)
        {
            await HRRepo.Delete(ID);
            UnitOfWork.Save();
            return RedirectToAction("Employees", "Employee");
        }
    }
}
