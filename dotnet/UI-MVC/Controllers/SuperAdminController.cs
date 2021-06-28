using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BL.DBManagers;
using BL.Domain.Identity;
using BL.Domain.Test;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Storage.v1.Data;
using Google.Cloud.Storage.V1;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using UI.MVC.Models;

namespace UI.MVC.Controllers
{
    [Authorize(Policy = "OnlySuperadmin")]
    public class SuperAdminController : Controller
    {
        private readonly CloudStorageOptions _options;

        private readonly StorageClient _storage;
        private readonly DbPartyManager _dbPartyManager;
        private readonly DbTestManager _dbTestManager;
        private readonly DbUserManager _dbUserManager;
        private readonly IEmailSender _emailSender;
        private readonly IStringLocalizer<HomeController> _localizer;
        private readonly UserManager<ApplicationUser> _userManager;

        public SuperAdminController(UserManager<ApplicationUser> userManager, IEmailSender emailSender,
            IStringLocalizer<HomeController> localizer,
            DbPartyManager dbPartyManager, DbTestManager dbTestManager, DbUserManager dbUserManager,
            IOptions<CloudStorageOptions> options)
        {
            _dbPartyManager = dbPartyManager;
            _dbTestManager = dbTestManager;
            _dbUserManager = dbUserManager;
            _localizer = localizer;

            _userManager = userManager;
            _emailSender = emailSender;


            var credential = GoogleCredential.FromFile("../cs2-vandepoel-seppe-272810-2abf455ff0f2.json");
            _storage = StorageClient.Create(credential);
            _options = options.Value;
        }

        // GET
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult PartiesList()
        {
            var parties = _dbPartyManager.GetAllParties();
            return View(parties);
        }

        public IActionResult AdminList(string? searchString)
        {
            IEnumerable<Organisation> organisations;
            if (searchString == null) organisations = _dbUserManager.GetAllOrganisations().ToList();
            else
                organisations = _dbUserManager.GetOrganisationsWithName(searchString).ToList();

            var amountOfSchoolsAndOrg = new Dictionary<Organisation, int[]>();
            foreach (var organisation in organisations)
            {
                var admin = _dbUserManager.GetAdminFromOrg(organisation.Id) ?? new User {UserId = -1};
                amountOfSchoolsAndOrg.Add(organisation, new[]
                {
                    _dbUserManager.GetAllClassesFromSchool(organisation.Id).Count(), admin.UserId
                });
            }

            return View(amountOfSchoolsAndOrg);
        }


        public IActionResult RequestAccount()
        {
            return View();
        }

        public IActionResult SuccessfulRequest()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ModifyParty(string partyId)
        {
            var party = _dbPartyManager.GetParty(partyId);
            return View(party);
        }

        [HttpPost]
        public IActionResult ModifyParty(string partyId, string orientation, string partyLeader, string colour)
        {
            var party = _dbPartyManager.GetParty(partyId);
            party.Orientation = orientation;
            party.PartyLeader = partyLeader;
            party.Colour = colour;
            _dbPartyManager.UpdateParty(party);
            return RedirectToAction("PartiesList");
        }

        [HttpGet]
        public IActionResult CreateParty()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateParty(string partyName)
        {
            var party = new Party();
            party.Name = partyName;
            _dbPartyManager.CreateParty(party);
            return RedirectToAction("ModifyParty", new {partyId = partyName});
        }

        public IActionResult DeleteParty(string partyId)
        {
            _dbPartyManager.DeleteParty(partyId);
            return RedirectToAction("PartiesList");
        }

        public IActionResult ModifyAnswers(string partyId)
        {
            var party = _dbPartyManager.GetParty(partyId);
            var statements = _dbTestManager.GetAllStatementsFromTest(1);
            IEnumerable<Answer> answers = _dbPartyManager.GetPartyAnswers(partyId);
            ViewBag.data = statements;
            ViewBag.answers = answers;
            return View(party);
        }

        [HttpPost]
        public IActionResult ModifyAnswers(string partyId, int statementId)
        {
            var party = _dbPartyManager.GetParty(partyId);
            var statements = _dbTestManager.GetAllStatementsFromTest(1);
            ViewBag.data = statements;
            var statement = _dbTestManager.GetStatement(statementId);
            var answer = _dbPartyManager.GetAnswerByStatement(partyId, statementId);
            ViewData["statement"] = statement;
            ViewData["answer"] = answer;
            return View(party);
        }

        [HttpGet]
        public IActionResult ModifyAnswer(string partyId, int statementId)
        {
            var answer = _dbPartyManager.GetAnswerByStatement(partyId, statementId);
            ViewBag.party = partyId;
            ViewBag.statement = _dbTestManager.GetStatement(statementId).Text;
            ViewBag.agree = "Akkoord";
            ViewBag.disagree = "Niet akkoord";
            return View(answer);
        }

        [HttpPost]
        public IActionResult ModifyAnswer(int answerId, string partyId, string argument, int ao)
        {
            var answer = _dbTestManager.GetAnswer(answerId);
            answer.Argument = argument;
            answer.ChosenAnswer = _dbTestManager.GetAnswerOption(ao);
            _dbPartyManager.UpdateAnswer(answer);
            return RedirectToAction("ModifyAnswers", new {partyId});
        }

        [HttpPost]
        public async Task<IActionResult> SendAccountRequest(RegisterSchoolViewModel registerSchoolViewModel)
        {
            if (!ModelState.IsValid) return View("RequestAccount");

            //make new applicationUser for login -> not able to login until admin activates account.
            var applicationUser = new ApplicationUser();
            applicationUser.Email = registerSchoolViewModel.Email;
            applicationUser.UserName = registerSchoolViewModel.Email;

            //make new Admin User type to associate to login user
            var admin = new Admin();
            var organisation = new Organisation(registerSchoolViewModel.SchoolName);
            organisation.City = registerSchoolViewModel.City;
            organisation.Postalcode = registerSchoolViewModel.PostalCode;
            organisation.StreetAndNumber = registerSchoolViewModel.StreetAndNumber;
            admin.Organisation = organisation;

            //Associate the login account and data user
            applicationUser.User = admin;

            //NOTE: Users have to create a password later
            var result1 = await _userManager.CreateAsync(applicationUser);
            //Add to role Admin
            var result2 = await _userManager.AddToRoleAsync(applicationUser, "Admin");

            //send the confirmation email to school account
            if (result1.Succeeded && result2.Succeeded)
            {
                var code = await _userManager.GeneratePasswordResetTokenAsync(applicationUser);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                var callbackUrl = Url.Page(
                    "/Account/ResetPassword",
                    null,
                    new {area = "Identity", code},
                    Request.Scheme);

                await _emailSender.SendEmailAsync(
                    applicationUser.Email,
                    "Please set up your account",
                    $"Please set your password by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                return RedirectToAction("SuccessfulRequest");
            }

            //if something went wrong
            return BadRequest("Something went wrong sending your request. Please try again.");
        }


        [HttpGet]
        public IActionResult TagList()
        {
            return View(_dbTestManager.GetAllTags());
        }

        [HttpPost]
        public IActionResult CreateTag(string tagText)
        {
            _dbTestManager.CreateNewTag(tagText);
            return RedirectToAction("TagList", new {tagId = -1});
        }

        [HttpPost]
        public IActionResult TagList(int tagId)
        {
            if (tagId != -1)
            {
                var tag = _dbTestManager.GetTag(tagId);
                ViewBag.id = tag.Id;
                ViewBag.text = tag.Name;
            }

            return View(_dbTestManager.GetAllTags());
        }

        [HttpPost]
        public IActionResult ModifyTag(int tagId, string editTagName)
        {
            _dbTestManager.UpdateTag(tagId, editTagName);
            return RedirectToAction("TagList", new {tagId = -1});
        }

        [HttpGet]
        public IActionResult DeleteTag(int tagId)
        {
            _dbTestManager.DeleteTag(tagId);
            return RedirectToAction("TagList", new {tagId = -1});
        }

        //MEDIA
        [HttpGet]
        public IActionResult MediaManager(string partyId)
        {
            var party = _dbPartyManager.GetParty(partyId);
            return View(party);
        }

        [HttpPost]
        public async Task<IActionResult> MediaManager(string partyId, IFormFile file, string link)
        {
            var party = _dbPartyManager.GetParty(partyId);
            if (file != null)
                try
                {
                    //lokale kopie maken
                    FileStream fileStream = null;
                    Directory.CreateDirectory(Directory.GetCurrentDirectory() + "\\uploadfile");
                    var filepath = Directory.GetCurrentDirectory() + "\\uploadfile";
                    if (file.Length > 0)
                        using (var stream = new FileStream(filepath + "\\" + file.FileName, FileMode.CreateNew))
                        {
                            await file.CopyToAsync(stream);
                        }

                    var path = Path.Combine(filepath + "\\" + file.FileName);

                    //oude file verwijderen
                    foreach (var storageObject in _storage.ListObjects(_options.BucketName, ""))
                        if (storageObject.Name.Contains(partyId))
                            _storage.DeleteObject(_options.BucketName, storageObject.Name);


                    //nieuwe file uploaden
                    using (var f = System.IO.File.OpenRead(path))
                    {
                        string objectName = null;
                        objectName = Path.GetFileName(path);
                        var options = new UploadObjectOptions();
                        _storage.UploadObject(_options.BucketName, partyId + Path.GetExtension(path), null, f);
                    }

                    //file rechten beheren
                    var newObject =
                        await _storage.GetObjectAsync(_options.BucketName, partyId + Path.GetExtension(path));
                    newObject.Acl = newObject.Acl ?? new List<ObjectAccessControl>();
                    _storage.UpdateObject(newObject, new UpdateObjectOptions
                    {
                        PredefinedAcl = PredefinedObjectAcl.PublicRead
                    });

                    //nieuwe file linken aan partij
                    party.ImageLink = newObject.MediaLink;
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message);
                }
                finally
                {
                    //lokale file verwijderen
                    Directory.Delete(Directory.GetCurrentDirectory() + "\\uploadfile", true);
                }

            if (link != null)
                if (party.MediaLink == null || !party.MediaLink.Equals(link))
                {
                    var match = Regex.Match(link, @"v=[a-zA-Z0-9]+");
                    var vidId = match.Value.Substring(2);
                    party.MediaLink = vidId;
                }

            _dbPartyManager.UpdateParty(party);

            return View(party);
        }

        public async Task<IActionResult> BlockSchool(int id)
        {
            var admin = _dbUserManager.GetAdminFromOrg(id);
            admin.Organisation.Blocked = true;

            _dbUserManager.UpdateUser(admin);
            foreach (var user in _dbUserManager.GetApplicationUsersFromOrg(id))
            {
                user.Blocked = true;
                await _userManager.UpdateAsync(user);
            }

            return RedirectToAction("AdminList");
        }

        public async Task<IActionResult> UnBlockSchool(int id)
        {
            var admin = _dbUserManager.GetAdminFromOrg(id);
            admin.Organisation.Blocked = false;

            _dbUserManager.UpdateUser(admin);
            foreach (var user in _dbUserManager.GetApplicationUsersFromOrg(id))
            {
                user.Blocked = false;
                await _userManager.UpdateAsync(user);
            }

            return RedirectToAction("AdminList");
        }

        [HttpGet]
        public IActionResult ManageCSV()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ManageCSV(List<IFormFile> files)
        {
            var text = "";
            var basepath = "..\\DAL\\csv\\";

            foreach (var file in files)
                if (file != null)
                {
                    if (file.FileName == "stemtest.csv")
                    {
                        System.IO.File.Delete(basepath + "stemtest.csv");
                        var path = Path.GetFullPath(basepath);
                        path = Path.Combine(path, file.FileName);
                        using (var stream = System.IO.File.Create(path))
                        {
                            await file.CopyToAsync(stream);
                        }

                        text += "stemtest succesvol vervangen";
                    }

                    if (file.FileName == "woordenlijst.csv")
                    {
                        System.IO.File.Delete(basepath + "woordenlijst.csv");
                        var path = Path.GetFullPath("..\\DAL\\csv\\");
                        path = Path.Combine(path, file.FileName);
                        using (var stream = System.IO.File.Create(path))
                        {
                            await file.CopyToAsync(stream);
                        }

                        text += "woordenlijst succesvol vervangen";
                    }
                }

            ViewBag.text = text;

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ReSeedDatabase()
        {
            _dbTestManager.ReSeedDatabase();
            return RedirectToAction("ManageCSV");
        }
    }
}