using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Univer.Data;
using Univer.Models;
using Univer.ViewModels;
using Univer.ViewModels.University;

namespace Univer.Controllers
{
    public class UniversityController : Controller
    {
        private ApplicationDbContext _db;
        string _filesPath;

        public UniversityController(ApplicationDbContext db, IWebHostEnvironment env)
        {
            _db = db;
            _filesPath = Path.Combine(env.WebRootPath, "images");

        }
        public IActionResult Index(int pageNumber = 1, int pageSize = 20, ConfirmationStatus? statusFilter = null, String searchFilter = "")
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
        public IActionResult Details(int id)
        {
            University university = _db.Universities.Find(id);
            return View(university);
        }
        [Authorize]
        public IActionResult Add()
        {
            return View();
        }
        [Authorize]
        public IActionResult AddIndicators(int id)
        {
            UniversityIndicatorsViewModel universityIndicators = new()
            {
                UniversityId = id,
                Indicators = _db.Indicators.AsEnumerable().Select(x => new IndicatorValue{ Indicator = x }).ToList()
                
            };
            
            return View(universityIndicators);
        }
        [Authorize]
        public IActionResult UpdateIndicators(int id)
        {
            University university = _db.Universities.Find(id);
            bool IndicatorsExist = university.Indicators.Any();
            if (IndicatorsExist)
            {
                IEnumerable<UniversityIndicatorsViewModel> indicators = university.Indicators.GroupBy(x => x.Year).Select(indicators => 
                    new UniversityIndicatorsViewModel
                    {
                        Indicators = indicators.Select(x => 
                            new IndicatorValue 
                            { 
                                Indicator = x.Indicator,
                                Value = x.Value,
                                UniversityIndicatorId = x.Id
                            }
                        ).ToList(),
                        UniversityId = university.Id,
                        Year = indicators.Key,
                    }
                );
                return View(indicators);
            }
            ModelState.AddModelError("", "Нет показателей у вуза");
            return View();
        }
        [HttpPost]
        [Authorize]
        public IActionResult Delete(int id)
        {
            var university = _db.Universities.Find(id);
            _db.Remove(university);
            _db.SaveChanges();
            return Redirect(Request.Headers["referer"]);
        }
        [Authorize]
        public IActionResult Update(int id)
        {
            var university = _db.Universities.Find(id);

            return View(university);
        }
        [Authorize]
        [HttpPost]
        public IActionResult Update(IFormFile uploadedFile, University university)
        {
            if (!_db.Universities.Any(x => x.Id == university.Id))
            {
                ModelState.AddModelError("", "Вуз еще не был добавлен");
            }
            if (uploadedFile == null)
            {
                ModelState.AddModelError("", "Добавьте логотип");
            }

            if (ModelState.IsValid)
            {
                university.Image = SaveFiles(uploadedFile, university.Name);
                if (HttpContext.User.IsInRole(Role.Admin.ToString()))
                {
                    university.Status = ConfirmationStatus.Confirmed;
                }
                else
                {
                    university.Status = ConfirmationStatus.Unconfirmed;
                }
                _db.Update(university);
                _db.SaveChanges();
                return Redirect(Request.Headers["referer"]);
            }
            return View(university);
        }
        [HttpPost]
        [Authorize]
        public IActionResult UpdateIndicators(UniversityIndicatorsViewModel model)
        {
            bool IndicatorsExist = _db.UniversityIndicators.Any(x => x.University.Id == model.UniversityId && x.Year == model.Year);
            if (IndicatorsExist)
            {
                University university = _db.Universities.Find(model.UniversityId);
                UniversityIndicator[] indicators = model.Indicators.Select(indicator =>
                    new UniversityIndicator()
                    {
                        Indicator = _db.Indicators.Find(indicator.Indicator.Id),
                        University = university,
                        Value = indicator.Value,
                        Year = model.Year,
                        Id = indicator.UniversityIndicatorId,

                    }
                ).ToArray();
                _db.UpdateRange(indicators);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            ModelState.AddModelError("", "Показатели этого года для данного вуза не добавлены");
            return View(model);
        }

        [HttpPost]
        [Authorize]
        public IActionResult AddIndicators(UniversityIndicatorsViewModel model)
        {
            bool IndicatorsExist = _db.UniversityIndicators.Any(x => x.University.Id == model.UniversityId && x.Year == model.Year);
            if (!IndicatorsExist)
            {
                University university = _db.Universities.Find(model.UniversityId);
                university.Indicators = model.Indicators.Select(indicator => 
                    new UniversityIndicator()
                    {
                        Indicator = _db.Indicators.Find(indicator.Indicator.Id),
                        University = university,
                        Value = indicator.Value,
                        Year = model.Year
                    }
                ).ToList();
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            ModelState.AddModelError("", "Показатели этого года для данного вуза уже заполнены");
            return View(model);
        }
        [HttpPost]
        [Authorize]
        public IActionResult Add(IFormFile uploadedFile, University university)
        {
            University oldUniversity = _db.Universities.FirstOrDefault(x => x.Name == university.Name);
            if(oldUniversity != null)
            {
                ModelState.AddModelError("", "Вуз с таким названием уже существует");
            }
            if(uploadedFile == null)
            {
                ModelState.AddModelError("", "Добавьте логотип");
            }

            if (ModelState.IsValid)
            {
                university.Image = SaveFiles(uploadedFile, university.Name);
                if (HttpContext.User.IsInRole(Role.Admin.ToString()))
                {
                    university.Status = ConfirmationStatus.Confirmed;
                }
                else
                {
                    university.Status = ConfirmationStatus.Unconfirmed;
                }
                _db.Add(university);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(university);

        }
        public IActionResult Polynom(int id)
        {
            University university = _db.Universities.Find(id);
            return View(university);
        }
       
        private string SaveFiles(IFormFile file, string fileName = null)
        {
            string filepath = Path.Combine(_filesPath, $"{fileName ?? file.Name}.png");

            using (var fileStream = new FileStream(filepath, FileMode.Create))
            {
                file.CopyTo(fileStream);
            }

            return $"/images/{fileName ?? file.Name}.png";
        }


    }

}
