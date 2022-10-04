using HRSystem.Models;
using HRSystem.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRSystem.MVC
{
    public class OfficialHolidaysController : Controller
    {
        private readonly OfficialHolidaysRepository OffRepo;
        private readonly UnitOfWork UnitOfWork;
        public OfficialHolidaysController(UnitOfWork _UnitOfWork, OfficialHolidaysRepository _OffRepo)
        {
            UnitOfWork = _UnitOfWork;
            OffRepo = _OffRepo;
        }
        [Authorize(Roles = "hr")]
        [HttpGet]
        public IActionResult Get()
        {
            ViewBag.OffHolidays = OffRepo.GetList().ToList();
            return View();
        }
       
        [Authorize(Roles = "hr")]
        [Route("OfficialHoliday/Add")]
        [HttpPost]
        public IActionResult Add(OfficialHolidays obj) 
        {
            var result = OffRepo.Add(obj);
            UnitOfWork.Save();
            return RedirectToAction("Get","OfficialHolidays");
        }
        [Authorize(Roles = "hr")]
        public IActionResult Remove(int ID)
        {
            var result = OffRepo.Remove(ID);
            UnitOfWork.Save();
            return RedirectToAction("Get", "OfficialHolidays");
        }
    }
}
