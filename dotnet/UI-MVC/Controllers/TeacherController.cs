using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BL;
using BL.DBManagers;
using BL.Domain.Identity;
using BL.Domain.Sessie;
using BL.Domain.Test;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Localization;
using QRCoder;
using UI.MVC.Models;

namespace UI.MVC.Controllers
{
    [Authorize(Policy = "TeacherOrHigher")]
    public class TeacherController : Controller
    {
        private readonly DbPartyManager _dbPartyManager;
        private readonly DbSessionManager _dbSessionManager;
        private readonly DbTestManager _dbTestManager;
        private readonly DbUserManager _dbUserManager;
        private readonly TestManager _mgr;
        private readonly SessionManager _sessionManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private GameManager _gameManager;
        private readonly IStringLocalizer<TeacherController> _localizer;

        public TeacherController(IStringLocalizer<TeacherController> localizer,
            UserManager<ApplicationUser> userManager, DbUserManager dbUserManager, DbTestManager dbTestManager,
            DbSessionManager dbSessionManager, DbPartyManager dbPartyManager)
        {
            
            
            
            _localizer = localizer;
            _dbSessionManager = dbSessionManager;
            _dbTestManager = dbTestManager;
            _dbUserManager = dbUserManager;
            _dbPartyManager = dbPartyManager;
            _mgr = new TestManager(_dbTestManager, _dbUserManager);
            _userManager = userManager;
            _sessionManager = new SessionManager(_dbSessionManager, _dbTestManager, _dbUserManager, _dbPartyManager);
        }

        public IActionResult Index(string error)
        {
            ViewBag.Error = error;
            return View();
        }

        public IActionResult ShowSessions()
        {
            var curUser = _userManager.FindByNameAsync(User.Identity.Name).Result;
            if (curUser == null) return NotFound("ApplicationUser not found");
            var user = _dbUserManager.GetUser(curUser.Id);
            ViewBag.EndedTeacherSessions = _dbUserManager.GetEndedTeacherSessions(user.ApplicationUserId)
                .OrderByDescending(x => x.Id).ToList();
            ViewBag.TeacherSessions = _dbUserManager.GetNotStartedTeacherSessions(user.ApplicationUserId)
                .OrderByDescending(x => x.Id).ToList();

            return View();
        }

        // GET
        public IActionResult StartSession(bool preparingSession)
        {
            var curUser = _userManager.FindByNameAsync(User.Identity.Name).Result;
            if (curUser == null) return NotFound("ApplicationUser not found");
            var user = _dbUserManager.GetUser(curUser.Id);

            var testItems = new List<SelectListItem>();
            var tests = _dbTestManager.GetTestsFromUser(user.UserId).ToList();
            var stemTest = _dbTestManager.GetStemTest();
            testItems.Add(new SelectListItem {Value = stemTest.Id.ToString(), Text = stemTest.Title});
            foreach (var test in tests)
                testItems.Add(new SelectListItem {Value = test.Id.ToString(), Text = test.Title});

            var classNames = new List<SelectListItem>();
            _dbUserManager.GetClasses(user.ApplicationUserId).ToList()
                .ForEach(clas => classNames.Add(new SelectListItem(clas.Name, clas.Name)));
            if (!classNames.Any())
                return RedirectToAction("Index",
                    new {error = _localizer["You must possess classes to able start a session!"]});
            ViewBag.Tests = testItems;
            ViewBag.ClassNames = classNames;
            ViewBag.PreparingSession = preparingSession;

            return View();
        }

        public IActionResult InitialiseSession(StartSessionViewModel startSession, bool selectStatements)
        {
            var curUser = _userManager.FindByNameAsync(User.Identity.Name).Result;
            var user = _dbUserManager.GetUser(curUser.Id);
            startSession.UserId = user.ApplicationUserId;
            var gameType = (GameType) Enum.Parse(typeof(GameType), startSession.Type, true);

            var sessionCode = _sessionManager.StartSession(startSession.Test, gameType, startSession.ClassName,
                startSession.UserId, startSession.Arguments, startSession.Definitions, startSession.Skip,
                startSession.ForceWaiting, startSession.Colour1, startSession.Colour2, startSession.Colour3,
                startSession.Colour4, startSession.Colour5, startSession.Colour6, startSession.SkipColour);

            var session = new TeacherSessionViewModel();
            session.ClassName = startSession.ClassName;
            session.SessionCode = sessionCode;
            session.GameType = gameType;

            if (selectStatements)
                return RedirectToAction("SelectSatementsTest", "Teacher", new
                {
                    session.SessionCode,
                    session.ClassName,
                    session.GameType,
                    startSession.PreparingSession, TestId = startSession.Test
                });

            _gameManager = new GameManager(session.SessionCode, _dbTestManager, _dbSessionManager, _dbPartyManager);
            _gameManager.SetChosenStatements();

            if (gameType == GameType.PARTYGAME || gameType == GameType.DEBATEGAME)
                return RedirectToAction("PartiesSelection", new
                {
                    session.SessionCode,
                    session.ClassName,
                    session.GameType,
                    startSession.PreparingSession
                });

            if (startSession.PreparingSession) return RedirectToAction("ShowSessions");

            return RedirectToAction("CodeScreen", session);
        }

        public IActionResult SelectSatementsTest(TeacherSessionViewModel session, bool preparingSession, int testId)
        {
            var test = _dbTestManager.GetTest(testId);
            ViewBag.Test = test;
            ViewBag.PreparingSession = preparingSession;
            return View(session);
        }

        [HttpPost]
        public IActionResult SelectSatementsTest(TeacherSessionViewModel session, ICollection<int> selectedStatementsId,
            bool preparingSession)
        {
            _dbSessionManager.SetChosenStatements(session.SessionCode, selectedStatementsId.OrderBy(x => x).ToList());
            if (session.GameType == GameType.PARTYGAME || session.GameType == GameType.DEBATEGAME)
                return RedirectToAction("PartiesSelection", new
                {
                    session.SessionCode,
                    session.ClassName,
                    session.GameType, PreparingSession = preparingSession
                });

            if (preparingSession) return RedirectToAction("ShowSessions");

            return RedirectToAction("CodeScreen", session);
        }

        public IActionResult PartiesSelection(TeacherSessionViewModel session, bool preparingSession, string error)
        {
            ViewBag.Parties = _dbPartyManager.GetAllParties().ToList();
            ViewBag.error = error;
            ViewBag.PreparingSession = preparingSession;
            return View(session);
        }

        [HttpPost]
        public IActionResult PartiesSelection(TeacherSessionViewModel session, bool preparingSession,
            ICollection<string> parties)
        {
            var chosenParties = parties.Where(x => x != null).ToList();
            if (!chosenParties.Any())
            {
                string error = _localizer["You must atleast select one party"];
                return RedirectToAction("PartiesSelection", new
                {
                    session.SessionCode,
                    session.ClassName,
                    session.GameType, PreparingSession = preparingSession, error
                });
            }

            _dbSessionManager.AddChosenPartiesToTeacherSession(session.SessionCode, chosenParties.ToList());

            if (preparingSession) return RedirectToAction("ShowSessions");

            return RedirectToAction("CodeScreen", session);
        }

        //CONVERTS QR CODE TO BYTES
        private static byte[] BitmapToBytes(Bitmap img)
        {
            using (var stream = new MemoryStream())
            {
                img.Save(stream, ImageFormat.Png);
                return stream.ToArray();
            }
        }

        public IActionResult CodeScreen(TeacherSessionViewModel session)
        {
            _gameManager = new GameManager(session.SessionCode, _dbTestManager, _dbSessionManager, _dbPartyManager);
            _gameManager.SetStartDate();

            session.MaxAmountStudents = _gameManager.GetMaxStudents();
            session.CurrentStudentCount = _gameManager.GetCurrentStudents();
            session.StatementCount = _dbSessionManager.GetStatementCount(session.SessionCode);
            ViewBag.ForceWaiting = _gameManager.GetForceWaiting();
            session.CurrentStatement = -1;

            var qrGenerator = new QRCodeGenerator();
            var routeUrl = "https://" + "www.daemonsstemtest.be" + "/Game/EnterGame?sessioncode=" +
                           session.SessionCode;
            var qrCodeData = qrGenerator.CreateQrCode(routeUrl, QRCodeGenerator.ECCLevel.Q);
            var qrCode = new QRCode(qrCodeData);
            var qrCodeImage = qrCode.GetGraphic(20);

            ViewBag.QRCode = BitmapToBytes(qrCodeImage);

            return View(session);
        }

        public IActionResult ShowStatement(TeacherSessionViewModel session)
        {
            if (session.CurrentStatement == -1) session.CurrentStatement = 0;

            _gameManager = new GameManager(session.SessionCode, _dbTestManager, _dbSessionManager, _dbPartyManager);
            _gameManager.SetCurrentSatement(session.CurrentStatement);
            if (session.CurrentStatement >= session.StatementCount) return RedirectToAction("TestResult", session);

            _gameManager = new GameManager(session.SessionCode, _dbTestManager, _dbSessionManager, _dbPartyManager);

            session.CurrentStudentCount = _gameManager.GetCurrentStudents();

            var statement =
                _dbSessionManager.GetStatementFromChosenStatementsByIndex(session.SessionCode,
                    session.CurrentStatement);
            ViewBag.Statement = statement;

            var definitions = _dbSessionManager.GetDefinitions(session.SessionCode).ToList();
            ViewBag.Definitions = definitions;

            ViewBag.Definition = _gameManager.GetAllowedDefinitions();

            ViewBag.AnswerCount = _gameManager.CurrentStudentAnswers(session.CurrentStatement);

            return View(session);
        }

        public IActionResult StatementResult(TeacherSessionViewModel session)
        {
            _gameManager = new GameManager(session.SessionCode, _dbTestManager, _dbSessionManager, _dbPartyManager);
            var statement =
                _dbSessionManager.GetStatementFromChosenStatementsByIndex(session.SessionCode,
                    session.CurrentStatement);
            ViewBag.Statement = statement;

            var answers = _dbSessionManager.GetStudentAnswersForStatement(session.SessionCode, session.CurrentStatement)
                .ToList();
            var answerOptions = _dbSessionManager.GetAnswerOptionsByIndex(session.SessionCode, session.CurrentStatement)
                .ToList();
            ViewBag.AnswerOptions = answerOptions;
            ViewBag.Distribution = _gameManager.GetDistribution(answers, answerOptions);
            ViewBag.Answers = answers;
            var parties = _dbSessionManager.GetChosenParties(session.SessionCode).ToList();
            ViewBag.Parties = parties;
            var partyAnswers = _dbSessionManager
                .GetChosenPartyAnswersOfStatement(session.SessionCode, session.CurrentStatement).ToList();
            ViewBag.PartyAnswers = partyAnswers;
            ViewBag.Skip = _gameManager.GetAllowSkip();

            return View(session);
        }

        public IActionResult TestResult(TeacherSessionViewModel session)
        {
            session.CurrentStatement = session.StatementCount + 1;

            _gameManager = new GameManager(session.SessionCode, _dbTestManager, _dbSessionManager, _dbPartyManager);
            _gameManager.SetCurrentSatement(session.CurrentStatement);

            if (session.GameType == GameType.PARTYGAME) return RedirectToAction("PartyTestResult", "Teacher", session);
            if (session.GameType == GameType.DEBATEGAME) return RedirectToAction("DebateTestResult", session);
            if (session.GameType == GameType.CUSTOMGAME_PARTY)
                return RedirectToAction("CustomPartyTestResult", session);
            if (session.GameType == GameType.CUSTOMGAME_DEBATE)
                return RedirectToAction("CustomDebateTestResult", session);

            return NoContent();
        }


        public IActionResult DebateTestResult(TeacherSessionViewModel session)
        {
            ViewBag.Statements = _dbSessionManager.GetStatements(session.SessionCode).ToList();

            return View(session);
        }

        public IActionResult PartyTestResult(TeacherSessionViewModel session)
        {
            ViewBag.Students = _dbSessionManager.GetStudentSessionsBySessionCode(session.SessionCode).Where(x=>x.Score!="").ToList().OrderBy(x=>x.SelectedParty).ThenByDescending(x=>x.Score);
            return View(session);
        }

        public IActionResult CustomDebateTestResult(TeacherSessionViewModel session)
        {
            ViewBag.Students = _dbSessionManager.GetStudentSessionsBySessionCode(session.SessionCode).ToList();
            ViewBag.Statements = _dbSessionManager.GetStatements(session.SessionCode).ToList();

            return View(session);
        }

        public IActionResult CustomPartyTestResult(TeacherSessionViewModel session)
        {
            ViewBag.Students = _dbSessionManager.GetStudentSessionsBySessionCode(session.SessionCode).ToList().OrderByDescending(x=>x.Score);

            return View(session);
        }

        public IActionResult EndSession(int sessionCode)
        {
            _sessionManager.EndSession(sessionCode);
            return RedirectToAction("Index");
        }

        public IActionResult PastTestResult(PastSessionModel session)
        {
            if (session.GameType == GameType.PARTYGAME)
                return RedirectToAction("PastPartyTestResult", "Teacher", session);
            if (session.GameType == GameType.DEBATEGAME) return RedirectToAction("PastDebateTestResult", session);
            if (session.GameType == GameType.CUSTOMGAME_PARTY)
                return RedirectToAction("PastCustomPartyTestResult", session);
            if (session.GameType == GameType.CUSTOMGAME_DEBATE)
                return RedirectToAction("PastCustomDebateTestResult", session);

            return NoContent();
        }


        public IActionResult PastDebateTestResult(PastSessionModel session)
        {
            ViewBag.Statements = _dbSessionManager.GetStatementsPastSession(session.Id).ToList();

            return View(session);
        }

        public IActionResult PastPartyTestResult(PastSessionModel session)
        {
            ViewBag.Students = _dbSessionManager.GetStudentSessionsBySessionId(session.Id).ToList().OrderBy(x=>x.SelectedParty).ThenByDescending(x=>x.Score);

            return View(session);
        }

        public IActionResult PastCustomDebateTestResult(PastSessionModel session)
        {
            ViewBag.Statements = _dbSessionManager.GetStatementsPastSession(session.Id).ToList();


            return View(session);
        }

        public IActionResult PastCustomPartyTestResult(PastSessionModel session)
        {
            ViewBag.Students = _dbSessionManager.GetStudentSessionsBySessionId(session.Id).OrderByDescending(x=>x.Score);

            return View(session);
        }

        public IActionResult CreateTest(string error)
        {
            ViewBag.Error = error;
            return View();
        }

        public IActionResult ModifyTest(int testId)
        {
            var curUser = _userManager.FindByNameAsync(User.Identity.Name).Result;
            if (curUser == null) return NotFound("ApplicationUser not found");

            var user = _dbUserManager.GetUser(curUser.Id);
            if (user == null) return NotFound("Data user not found");

            var test = _dbTestManager.GetTest(testId);
            if (test.Maker.UserId != user.UserId)
                return Unauthorized("You do not have the necessary permissions to modify this test");
            ViewData["test"] = test;

            ViewData["tags"] = _dbTestManager.GetTagsFromTest(testId);
            ViewData["availableTags"] = _dbTestManager.GetAllTags();
            ViewData["definitions"] = _dbTestManager.GetDefinitions(testId);
            return View(test.Statements);
        }

        public IActionResult ModifyStatement(int statementId)
        {
            var statement = _dbTestManager.GetStatement(statementId, true);
            return View(statement);
        }

        //POST
        [HttpPost]
        public IActionResult MakeNewTest(string testName)
        {
            var curUser = _userManager.FindByNameAsync(User.Identity.Name).Result;
            if (curUser != null)
            {
                var testMaker = _dbUserManager.GetUser(curUser.Id);
                if (testMaker == null)
                    return RedirectToAction("CreateTest", new {error = "Teacher could not be found"});

                var id = _mgr.MakeTest(testName, testMaker);
                if (id == -1)
                    return RedirectToAction("CreateTest", new {error = "You already have a test with this name."});

                return RedirectToAction("ModifyTest", new {testId = id});
            }

            return RedirectToAction("CreateTest",
                new {error = "Something went wrong with your login, please re-login and try again"});
        }

        [HttpPost]
        public IActionResult AddStatementToTest(string statementName, string statementExplanation, string rightAnswer,
            int testId, bool defaultAnswers, ICollection<string> answerOptions, ICollection<bool> aoc)
        {
            var testFromPage = _dbTestManager.GetTest(testId);
            var statementId = _mgr.MakeStatement(testId, statementName, statementExplanation);
            if (defaultAnswers)
            {
                _dbTestManager.AddDefaultAnswerOptions(statementId, rightAnswer);
            }
            else
            {
                var counter = 0;
                foreach (var opinion in answerOptions)
                {
                    if (aoc.ToList()[counter])
                        _mgr.AddAnswerOption(statementId, opinion, true);
                    else
                        _mgr.AddAnswerOption(statementId, opinion);

                    counter++;
                }

                _mgr.AddAnswerOption(statementId, "Overslaan");
            }

            return RedirectToAction("ModifyTest", new {testId = testFromPage.Id});
        }

        public IActionResult TestList()
        {
            var curUser = _userManager.FindByNameAsync(User.Identity.Name).Result;
            if (curUser == null) return NotFound("ApplicationUser not found");

            var user = _dbUserManager.GetUser(curUser.Id);
            if (user == null) return NotFound("Data user not found");

            var tests = _dbTestManager.GetTestsFromUser(user.UserId).ToList();
            return View(tests);
        }

        public IActionResult DeleteTest(int testId)
        {
            _dbTestManager.DeleteTest(testId);
            return RedirectToAction("TestList");
        }

        public IActionResult DeleteStatement(int statementId, int testId)
        {
            _dbTestManager.DeleteStatement(statementId, testId);
            return RedirectToAction("ModifyTest", new {testId});
        }

        //TAGS
        [HttpGet]
        public IActionResult DeleteTag(int tagId, int testId)
        {
            _dbTestManager.DeleteTagFromTest(testId, tagId);
            return RedirectToAction("ModifyTest", new {testId});
        }

        [HttpPost]
        public IActionResult AddTagToTest(int testId, int tagId)
        {
            _dbTestManager.AddTag(testId, tagId);
            return RedirectToAction("ModifyTest", new {testId});
        }

        [HttpPost]
        public IActionResult AddDefinitionToTest(int testId, string word, string explanation)
        {
            _dbTestManager.AddDefinition(testId, word, explanation);
            return RedirectToAction("ModifyTest", new {testId});
        }

        [HttpGet]
        public IActionResult DeleteDefinition(int id, int testId)
        {
            _dbTestManager.DeleteDefinition(id);
            return RedirectToAction("ModifyTest", new {testId});
        }


        [HttpPost]
        public IActionResult ModifyStatement(int statementId, string statementName, string statementExplanation,
            ICollection<string> answerOptions, ICollection<bool> aoc)
        {
            var statement = _dbTestManager.GetStatement(statementId, true);
            statement.Text = statementName;
            statement.Explanation = statementExplanation;
            _dbTestManager.UpdateStatement(statement);
            _dbTestManager.DeleteAnswerOptions(statementId);

            var counter = 0;
            foreach (var opinion in answerOptions)
            {
                if (aoc.ToList()[counter])
                    _mgr.AddAnswerOption(statementId, opinion, true);
                else
                    _mgr.AddAnswerOption(statementId, opinion);

                counter++;
            }

            return RedirectToAction("TestList");
        }

        //CLASS
        [HttpGet]
        public IActionResult ClassList()
        {
            var curUser = _userManager.FindByNameAsync(User.Identity.Name).Result;
            var user = _dbUserManager.GetUser(curUser.Id);
            var classes = _dbUserManager.GetClasses(user.ApplicationUserId);
            ViewBag.teacher = user.UserId;

            return View(classes);
        }

        [HttpGet]
        public IActionResult DeleteClass(int classId)
        {
            _dbUserManager.DeleteClass(classId);

            return RedirectToAction("ClassList");
        }

        [HttpGet]
        public IActionResult CreateClass(int teacherId)
        {
            ViewBag.teacher = teacherId;
            return View();
        }

        [HttpPost]
        public IActionResult CreateClass(string name, int amount, string year, int teacherId)
        {
            _dbUserManager.CreateClass(teacherId, name, amount, year);

            return RedirectToAction("ClassList");
        }

        [HttpPost]
        public IActionResult ModifyClass(int classId, string name, int amount, string year)
        {
            _dbUserManager.ModifyClass(classId, name, amount, year);

            return RedirectToAction("ClassList");
        }

        public IActionResult ModifyClass(int classId)
        {
            var clas = _dbUserManager.GetClass(classId);
            return View(clas);
        }

        [HttpGet]
        public async Task<IActionResult> ShareTest(int testId)
        {
            var test = _dbTestManager.GetTest(testId);
            IdentityUser idUser = await _userManager.FindByNameAsync(User.Identity.Name);
            var teacher = _dbUserManager.GetUser(idUser.Id);
            if (test.Maker.UserId == teacher.UserId) return View(test);

            return Unauthorized("Access denied. You do not have access to this test.");
        }

        public IActionResult ShareTestWithSchool(int testId)
        {
            var ogTest = _dbTestManager.GetTest(testId);
            var newSharedTest = new SharedTest
            {
                Test = ogTest,
                Creator = ogTest.Maker,
                Organisation = ogTest.Maker.Organisation,
                OrganisationShared = true,
                PublicShared = false
            };

            _dbTestManager.CreateSharedTest(newSharedTest);

            return RedirectToAction("TestList");
        }

        public IActionResult ShareTestWithPublic(int testId)
        {
            var ogTest = _dbTestManager.GetTest(testId);
            //To get creator with org included
            var creator = _dbUserManager.GetUser(ogTest.Maker.UserId);
            var newSharedTest = new SharedTest
            {
                Test = ogTest,
                Creator = creator,
                Organisation = creator.Organisation,
                OrganisationShared = true,
                PublicShared = true
            };

            _dbTestManager.CreateSharedTest(newSharedTest);

            return RedirectToAction("TestList");
        }

        [HttpGet]
        public async Task<IActionResult> BrowseSharedTests(string? searchString, string? chk1)
        {
            IEnumerable<SharedTest> sharedTests;
            var tests = new List<Test>();
            IdentityUser idUser = await _userManager.FindByNameAsync(User.Identity.Name);
            var teacher = _dbUserManager.GetUser(idUser.Id);
            sharedTests = _dbTestManager.GetSharedTestsForUser(teacher, searchString, chk1).ToList();
            var creatorsOfTest = new Dictionary<int, string>();
            var iscreator = new Dictionary<int, bool>();
            var testIdWithSharedTestId = new Dictionary<int, int>();
            foreach (var stest in sharedTests)
            {
                tests.Add(stest.Test);
                var creator = _dbUserManager.GetUser(stest.Creator.UserId);
                creatorsOfTest.Add(stest.Test.Id, creator.ApplicationUser.UserName);
                iscreator.Add(stest.Test.Id, stest.Creator.UserId == teacher.UserId);
                testIdWithSharedTestId.Add(stest.Test.Id, stest.Id);
            }

            ViewBag.Creators = creatorsOfTest;
            ViewBag.IsCreatorOfTests = iscreator;
            ViewBag.TestIdWithSharedId = testIdWithSharedTestId;
            return View(tests.ToList());
        }

        public IActionResult CopySharedTest(int testId)
        {
            var curUser = _dbUserManager.GetUser(_userManager.FindByNameAsync(User.Identity.Name).Result.Id);
            _dbTestManager.CopyTestToUser(testId, curUser);
            return RedirectToAction("TestList");
        }

        public IActionResult DeleteSharedTest(int stestId)
        {
            var curUser = _dbUserManager.GetUser(_userManager.FindByNameAsync(User.Identity.Name).Result.Id);
            var sharedTest = _dbTestManager.GetSharedTest(stestId);
            if (curUser.UserId != sharedTest.Creator.UserId)
                return Unauthorized("You do not have permission to delete this test.");

            _dbTestManager.DeleteSharedTest(stestId);

            return RedirectToAction("BrowseSharedTests");
        }
    }
}