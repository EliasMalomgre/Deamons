using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using BL.DBManagers;
using BL.Domain.Identity;
using BL.Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using UI.MVC.Models;

namespace UI.MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly DbUserManager _dbUserManager;
        private readonly IEmailSender _emailSender;
        private readonly ILogger<HomeController> _logger;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IStringLocalizer<HomeController> _localizer;

        public HomeController(ILogger<HomeController> logger, RoleManager<IdentityRole> roleManager,
            IStringLocalizer<HomeController> localizer,
            UserManager<ApplicationUser> userManager, IEmailSender emailSender, DbUserManager dbUserManager)
        {
            _localizer = localizer;
            _logger = logger;
            _roleManager = roleManager;
            _userManager = userManager;
            _emailSender = emailSender;
            _dbUserManager = dbUserManager;
        }

        public IActionResult Index()
        {
            ViewBag.text = _localizer["Home"];
            return View();
        }

        [HttpPost]
        public IActionResult SetLanguage(string culture, string returnUrl)
        {
            //Hierzo stellen we de gekozen culture in in een cookie
            Response.Cookies.Append(
                //1. selecteer cookie
                CookieRequestCultureProvider.DefaultCookieName,
                //2. stel culture in
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                //3. stel vervaltermijn in
                new CookieOptions {Expires = DateTimeOffset.UtcNow.AddDays(1)});

            return RedirectToAction("Index", "Game");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }

        public IActionResult ForSchools()
        {
            return View();
        }

        public IActionResult RequestSchoolAccount()
        {
            return View();
        }

        public IActionResult SuccessfulRequest()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SendAccountRequest(SchoolRequestEmailViewModel registerSchoolViewModel)
        {
            if (!ModelState.IsValid) return View("RequestSchoolAccount");
            var message = new StringBuilder();
            message.Append("<p> School email: " + registerSchoolViewModel.Email + "</p>");
            message.Append("<p>Schoolname: " + registerSchoolViewModel.SchoolName + "</p>");
            message.Append("<p>City: " + registerSchoolViewModel.City + "</p>");
            message.Append("<p>Postal code: " + registerSchoolViewModel.PostalCode + "</p>");
            message.Append("<p>Street + Number:" + registerSchoolViewModel.StreetAndNumber + "</p>");
            await _emailSender.SendEmailAsync("arthur.decraemer@student.kdg.be", "School application",
                message.ToString());
            return View("SuccessfulRequest");
        }
    }
}