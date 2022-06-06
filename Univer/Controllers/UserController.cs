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
    public class UserController : Controller
    {
        private ApplicationDbContext _db;

        public UserController(ApplicationDbContext db)
        {
            _db = db;
        }
        [Authorize]
        public IActionResult Universities(int pageNumber = 1, int pageSize = 20, ConfirmationStatus? statusFilter = null, String searchFilter = "")
        {
            IEnumerable<University> universities = _db.AuthorizedUser.AddedUniversities;
            ViewBag.statusFilter = statusFilter;
            ViewBag.pageSize = pageSize;
            ViewBag.searchFilter = searchFilter;
            universities = statusFilter switch
            {
                ConfirmationStatus.Confirmed => universities.Where(x => x.Status == ConfirmationStatus.Confirmed),
                ConfirmationStatus.Unconfirmed => universities.Where(x => x.Status == ConfirmationStatus.Unconfirmed),
                _ => universities
            };
            universities = universities.Where(x => x.Name.Contains(searchFilter??"", StringComparison.OrdinalIgnoreCase));
            return View(PaginatedList<University>.Create(universities, pageNumber, pageSize));
        }
    }
}
