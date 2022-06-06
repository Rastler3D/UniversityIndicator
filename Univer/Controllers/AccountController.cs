using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Univer.Data;
using Univer.Models;
using Univer.ViewModels;

namespace Univer.Controllers
{
    public class AccountController : Controller
    {
        private ApplicationDbContext _db;
        private string _filesPath;

        public AccountController(ApplicationDbContext db, IWebHostEnvironment env) 
        {
            _db = db;
            _filesPath = Path.Combine(env.WebRootPath, "images");
        }
        
        public IActionResult Login()
        {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return Redirect(Request.Query["ReturnUrl"].FirstOrDefault("/University/Index"));
        }

        [HttpPost]
        public async Task<IActionResult> Login(Login form)
        {
            if (ModelState.IsValid)
            {
                User user = _db.Users.FirstOrDefault(user => user.Email == form.Email);

                if (user != null && user.VerifyPassword(form.Password))
                {
                    await Authenticate(user);
                    
                    return Redirect(Request.Query["ReturnUrl"].FirstOrDefault("/University/Index"));
                   
                }

                ModelState.AddModelError("", "Неверный логин или пароль");
                
            }
            return View(form);
        }

        public IActionResult Registration()
        {
            return View();
        }

        [HttpPost] 
        public async Task<IActionResult> Registration(Registration form)
        {

            if (ModelState.IsValid)
            {
                User user = _db.Users.FirstOrDefault(user => user.Email == form.Email);

                if (user == null)
                {
                    string passwordSalt = PasswordHasher.GenerateSalt();
                    string passwordHash = PasswordHasher.HashPassword(form.Password, passwordSalt);

                    user = new User 
                    { 
                        Email = form.Email,
                        FirstName = form.FirstName,
                        LastName = form.LastName,
                        Role = Role.User,
                        PasswordSalt = passwordSalt,
                        PasswordHash = passwordHash,
                    };

                    _db.Users.Add(user);
                    _db.SaveChanges();

                    await Authenticate(user);

                    return Redirect(Request.Query["ReturnUrl"].FirstOrDefault("/University/Index"));

                }

                ModelState.AddModelError("", "Пользователь с таким Email-адресом уже зарегистрирован");

            }

            return View(form);
        }
        [Authorize]
        public IActionResult Profile()
        {
            return View(_db.AuthorizedUser);
        }



        private async Task  Authenticate(User user)
        {

            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.FirstName),
                new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Role.ToString()),
                new Claim("ProfileImage", user.ProfileImage??"/images/person.svg")
            };
            
            ClaimsIdentity id = new ClaimsIdentity(  
                claims,
                "ApplicationCookie",
                ClaimsIdentity.DefaultNameClaimType,
                ClaimsIdentity.DefaultRoleClaimType
            );
            
            ClaimsPrincipal principal = new ClaimsPrincipal(id);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Update(UpdateProfile form)
        {
            if (ModelState.IsValid && HttpContext.User.Identity.IsAuthenticated)
            {
                User user = _db.AuthorizedUser;
                if (form.profileImageFile != null)
                {
                    string filepath = Path.Combine(_filesPath, $"ProfileImage{user.Id}.png");

                    using (var fileStream = new FileStream(filepath, FileMode.OpenOrCreate))
                    {
                        form.profileImageFile.CopyTo(fileStream);
                    }

                    user.ProfileImage = $"/images/ProfileImage{user.Id}.png";
                }
                user.FirstName = form.FirstName;
                user.LastName = form.LastName;
                user.Email = form.Email;

                _db.SaveChanges();

                await Authenticate(user);
                return Ok();
            }

            return BadRequest();
        }

        [HttpPost]
        [Authorize]
        public IActionResult ChangePassword(ChangePassword form)
        {
            if (ModelState.IsValid && HttpContext.User.Identity.IsAuthenticated)
            {
                User user = _db.AuthorizedUser;
                if (user.VerifyPassword(form.OldPassword))
                {
                    string passwordSalt = user.PasswordSalt;
                    user.PasswordHash = PasswordHasher.HashPassword(form.NewPassword, passwordSalt);
                }
                _db.SaveChanges();

                return Ok();
            }

            return BadRequest();
        }


    }
}
