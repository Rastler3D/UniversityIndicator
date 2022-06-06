using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Univer.Data;
using Univer.Models;


namespace Univer.Controllers
{
    public class AdminController : Controller
    {
        private ApplicationDbContext _db;

        public AdminController(ApplicationDbContext db)
        {
            _db = db;
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Universities(int pageNumber = 1, int pageSize = 20, ConfirmationStatus? statusFilter = null, String searchFilter = "")
        {
            IEnumerable<University> universities = _db.Universities;
            ViewBag.statusFilter = statusFilter;
            ViewBag.pageSize = pageSize;
            ViewBag.searchFilter = searchFilter;
            universities = statusFilter switch
            {
                ConfirmationStatus.Confirmed => universities.Where(x => x.Status == ConfirmationStatus.Confirmed),
                ConfirmationStatus.Unconfirmed => universities.Where(x => x.Status == ConfirmationStatus.Unconfirmed),
                _ => universities
            };
            universities = universities.Where(x => x.Name.Contains(searchFilter ?? "", StringComparison.OrdinalIgnoreCase));
            return View(PaginatedList<University>.Create(universities, pageNumber, pageSize));
        }


        [Authorize(Roles = "Admin")]
        public IActionResult Users(int pageNumber = 1, int pageSize = 20)
        {

            return View(PaginatedList<User>.Create(_db.Users, pageNumber, pageSize));
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult ChangeProfileRole(int id, Role roleName)
        {
            User user = _db.Users.Find(id);
            user.Role = roleName;
            _db.SaveChanges();
            return Redirect(Request.Headers["referer"]);
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult ChangeUniversityStatus(int id, ConfirmationStatus universityStatus)
        {
            University university = _db.Universities.Find(id);
            university.Status = universityStatus;
            _db.SaveChanges();
            return Redirect(Request.Headers["referer"]);
        }
    }
}
