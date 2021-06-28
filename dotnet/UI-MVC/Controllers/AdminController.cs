using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using BL.DBManagers;
using BL.Domain.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using UI.MVC.Models;

namespace UI.MVC.Controllers
{
    public class AdminController : Controller
    {
        private readonly DbUserManager _dbUserManager;
        private readonly IEmailSender _emailSender;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminController(UserManager<ApplicationUser> userManager, IEmailSender emailSender,
            DbUserManager dbUserManager)
        {
            _userManager = userManager;
            _dbUserManager = dbUserManager;
            _emailSender = emailSender;
        }
        //GET
        //---------------------

        [HttpGet]
        public IActionResult Dashboard(int? adminId)
        {
            User user;
            //If user is not a superadmin but enters this method with an id return Unauthorized request
            if (adminId != null && !User.IsInRole("Superadmin"))
                return Unauthorized("You do not have the rights to view this dashboard");
            //User is superadmin who wants to see an admin's dashboard
            if (adminId != null && User.IsInRole("Superadmin"))
            {
                user = _dbUserManager.GetUser(adminId.Value);
            }
            //Superadmin wants to see own test
            else if (adminId == null && User.IsInRole("Superadmin"))
            {
                IdentityUser idUser = _userManager.FindByNameAsync(User.Identity.Name).Result;
                user = _dbUserManager.GetUser(idUser.Id);
            }
            //User is admin
            else
            {
                user = _dbUserManager.GetUser(_userManager.FindByNameAsync(User.Identity.Name).Result.Id) as Admin;
            }

            if (user == null) return BadRequest("Could not find Admin");

            ViewBag.AmountOfClasses = _dbUserManager.GetAllClassesFromSchool(user.Organisation.Id).Count();
            ViewBag.AmountOfSessions = _dbUserManager.GetAmountOfSessionsFromOrganisation(user.Organisation.Id);
            return View(user.Organisation);
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ModifyTeacher(string teacherId)
        {
            var applicationUser = await _userManager.FindByIdAsync(teacherId);
            ViewBag.Email = applicationUser.Email;
            return View(applicationUser);
        }

        [HttpGet]
        public async Task<IActionResult> TeacherList()
        {
            var applicationUser = await _userManager.FindByNameAsync(User.Identity.Name);
            var admin = _dbUserManager.GetUser(applicationUser.Id);

            var teachers = _dbUserManager.GetTeachersFromOrg(admin.Organisation.Id).ToList();
            var teacherUsers = new List<ApplicationUser>();
            foreach (var teacher in teachers) teacherUsers.Add(teacher.ApplicationUser);

            return View(teacherUsers);
        }

        [HttpGet]
        public IActionResult AddTeacher(string error)
        {
            ViewBag.Error = error;
            return View();
        }

        public IActionResult CreationSuccessful()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ModifyAdmin(string error)
        {
            IdentityUser identityUser = await _userManager.FindByNameAsync(User.Identity.Name);
            var admin = _dbUserManager.GetUser(identityUser.Id);
            ViewBag.Email = identityUser.Email;
            ViewBag.SchoolName = admin.Organisation.Name;
            ViewBag.Postalcode = admin.Organisation.Postalcode;
            ViewBag.StreetAndNumber = admin.Organisation.StreetAndNumber;
            ViewBag.City = admin.Organisation.City;
            ViewBag.Error = error;
            return View();
        }
        //-----------------------
        //END OF GETS

        //POST
        //-----------------------

        [HttpPost]
        public async Task<IActionResult> ModifyAdmin(SchoolRequestEmailViewModel model)
        {
            var identityUser = await _userManager.FindByNameAsync(User.Identity.Name);
            var admin = _dbUserManager.GetUser(identityUser.Id);

            if (!ModelState.IsValid)
            {
                ViewBag.Email = identityUser.Email;
                ViewBag.SchoolName = admin.Organisation.Name;
                ViewBag.Postalcode = admin.Organisation.Postalcode;
                ViewBag.StreetAndNumber = admin.Organisation.StreetAndNumber;
                ViewBag.City = admin.Organisation.City;
                return View("ModifyAdmin");
            }

            identityUser.Email = model.Email;
            identityUser.UserName = model.Email;
            admin.Organisation.Name = model.SchoolName;
            admin.Organisation.City = model.City;
            admin.Organisation.Postalcode = model.PostalCode;
            admin.Organisation.StreetAndNumber = model.StreetAndNumber;
            identityUser.User = admin;
            var result = await _userManager.UpdateAsync(identityUser);

            ViewBag.Email = identityUser.Email;
            ViewBag.SchoolName = admin.Organisation.Name;
            ViewBag.Postalcode = admin.Organisation.Postalcode;
            ViewBag.StreetAndNumber = admin.Organisation.StreetAndNumber;
            ViewBag.City = admin.Organisation.City;

            if (!result.Succeeded) return RedirectToAction("ModifyAdmin", new {error = "Failed to update user."});
            return View("ModifyAdmin");
        }

        [HttpPost]
        public async Task<IActionResult> AddTeacher(CreateTeacherModelView modelView)
        {
            //get creator
            var adminUser = _userManager.FindByNameAsync(User.Identity.Name).Result;
            //get user linked to adminUser with org
            var admin = _dbUserManager.GetUser(adminUser.Id);
            //Get organisation from admin
            var school = admin.Organisation;
            //Create new applicationUser
            var newTeacher = new ApplicationUser
            {
                Email = modelView.Email,
                UserName = modelView.Email
            };
            await _userManager.CreateAsync(newTeacher);
            var createdTeacher = _userManager.FindByEmailAsync(newTeacher.Email).Result;
            await _userManager.AddToRoleAsync(createdTeacher, "Teacher");
            var teacher = new Teacher();
            teacher.ApplicationUser = createdTeacher;

            var dataAdded = _dbUserManager.AddUser(teacher, school);
            if (!dataAdded) return RedirectToAction("AddTeacher", new {error = "Data user not added."});

            var code = await _userManager.GeneratePasswordResetTokenAsync(newTeacher);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var callbackUrl = Url.Page(
                "/Account/ResetPassword",
                null,
                new {area = "Identity", code},
                Request.Scheme);

            await _emailSender.SendEmailAsync(
                newTeacher.Email,
                "Please set up your account",
                $"Please set your password by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

            return RedirectToAction("CreationSuccessful");
        }

        [HttpPost]
        public async Task<IActionResult> ModifyTeacher(string email, string id)
        {
            var toUpdate = await _userManager.FindByIdAsync(id);
            toUpdate.Email = email;
            toUpdate.UserName = email;
            await _userManager.UpdateAsync(toUpdate);
            return RedirectToAction("TeacherList");
        }

        public async Task<IActionResult> DeleteTeacher(string id)
        {
            var toDelete = await _userManager.FindByIdAsync(id);
            var dataDeleted = _dbUserManager.DeleteTeacher(id);
            if (!dataDeleted) return BadRequest("Could not delete related data");
            var result = await _userManager.DeleteAsync(toDelete);
            if (!result.Succeeded) return BadRequest("Could not delete teacher");
            return RedirectToAction("TeacherList");
        }

        //-----------------------
        //END OF POSTS
    }
}