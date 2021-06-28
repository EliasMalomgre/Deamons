using System.Linq;
using BL;
using BL.DBManagers;
using BL.Domain.Sessie;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using UI.MVC.Models;

namespace UI.MVC.Controllers
{
    public class GameController : Controller
    {
        private readonly DbPartyManager _dbPartyManager;

        private readonly DbSessionManager _dbSessionManager;
        private readonly DbTestManager _dbTestManager;
        private GameManager _gameManager;
        private readonly IStringLocalizer<GameController> _localizer;


        public GameController(IStringLocalizer<GameController> localizer, DbSessionManager dbSessionManager,
            DbTestManager dbTestManager, DbPartyManager dbPartyManager)
        {
            _localizer = localizer;
            _dbSessionManager = dbSessionManager;
            _dbTestManager = dbTestManager;
            _dbPartyManager = dbPartyManager;
        }

        // GET
        public IActionResult Index(string error)
        {
            ViewBag.Error = error;
            return View();
        }

        public IActionResult WaitingScreen(SessionViewModel session, string argument, int answerId)
        {
            if (!_dbSessionManager.SessieCodeInUse(session.TeacherSessionCode))
            {
                string error = _localizer["Session has ended"];
                return RedirectToAction("Index", "Game", new {error});
            }

            _gameManager = new GameManager(session.TeacherSessionCode, _dbTestManager, _dbSessionManager,
                _dbPartyManager);

            if (session.CurrentStatementId != 0)
            {
                if (session.GameType == GameType.DEBATEGAME || session.GameType == GameType.CUSTOMGAME_DEBATE)
                {
                    var answerOptions = _dbSessionManager
                        .GetAnswerOptionsByIndex(session.TeacherSessionCode, session.CurrentStatementId - 1).ToList();
                    var colours = _gameManager.GetColours();
                    if (answerId == 3)
                    {
                        ViewBag.Colour = colours[2];
                    }
                    else
                    {
                        var skipIndex = answerOptions.IndexOf(answerOptions.Find(a => a.Id == 3));
                        answerOptions.RemoveAt(skipIndex);
                        colours.RemoveAt(2);
                        var index = answerOptions.IndexOf(answerOptions.Find(a => a.Id == answerId));
                        ViewBag.Colour = colours[index];
                    }
                }

                _gameManager.AnswerStatement(argument, session.StudentSessionId, answerId);
            }

            if (_gameManager.GetForceWaiting()) return RedirectToAction("AnswerStatement", session);
            var teacherSession = _dbSessionManager.GetTeacherSession(session.TeacherSessionCode);
            ViewBag.ClassName = teacherSession.Class.Name;
            ViewBag.SchoolName = _dbSessionManager.GetOrganisation(session.TeacherSessionCode).Name;
            ViewBag.SessionTitle = teacherSession.Test.Title;
            return View(session);
        }

        [HttpPost]
        public IActionResult Index(SessionViewModel session)
        {
            if (!_dbSessionManager.SessieCodeInUse(session.TeacherSessionCode))
            {
                string error = _localizer["Invalid Sessioncode"];
                return RedirectToAction("Index", new {error});
            }

            session.StatementCount = _dbSessionManager.GetStatementCount(session.TeacherSessionCode);
            _gameManager = new GameManager(session.TeacherSessionCode, _dbTestManager, _dbSessionManager,
                _dbPartyManager);
            session.StudentSessionId = _gameManager.AddStudent();
            if (session.StudentSessionId == -1)
            {
                string error = _localizer["Session is full"];
                return RedirectToAction("Index", new {error});
            }

            session.GameType = _dbSessionManager.GetGameType(session.TeacherSessionCode);

            if (session.GameType == GameType.PARTYGAME) return RedirectToAction("PartySelection", session);

            return RedirectToAction("WaitingScreen", session);
        }

        [HttpGet]
        public IActionResult EnterGame(int sessioncode)
        {
            var session = new SessionViewModel();
            session.TeacherSessionCode = sessioncode;
            if (!_dbSessionManager.SessieCodeInUse(session.TeacherSessionCode))
            {
                string error = _localizer["Invalid Sessioncode"];
                return RedirectToAction("Index", new {error});
            }

            session.StatementCount = _dbSessionManager.GetStatementCount(session.TeacherSessionCode);
            _gameManager = new GameManager(session.TeacherSessionCode, _dbTestManager, _dbSessionManager,
                _dbPartyManager);
            session.StudentSessionId = _gameManager.AddStudent();
            session.GameType = _dbSessionManager.GetGameType(session.TeacherSessionCode);

            if (session.GameType == GameType.PARTYGAME) return RedirectToAction("PartySelection", session);

            return RedirectToAction("WaitingScreen", session);
        }

        public IActionResult AnswerStatement(SessionViewModel session)
        {
            if (!_dbSessionManager.SessieCodeInUse(session.TeacherSessionCode))
            {
                string error = _localizer["Session has ended"];
                return RedirectToAction("Index", "Game", new {error});
            }

            if (session.CurrentStatementId < session.StatementCount)
            {
                _gameManager = new GameManager(session.TeacherSessionCode, _dbTestManager, _dbSessionManager,
                    _dbPartyManager);
                var statement =
                    _dbSessionManager.GetStatementFromChosenStatementsByIndex(session.TeacherSessionCode,
                        session.CurrentStatementId);
                ViewBag.Statement = statement;
                var definitions = _dbSessionManager.GetDefinitions(session.TeacherSessionCode).ToList();
                ViewBag.Definitions = definitions;
                var answerOptions = _dbSessionManager
                    .GetAnswerOptionsByIndex(session.TeacherSessionCode, session.CurrentStatementId).ToList();
                ViewBag.AnswerOptions = answerOptions;
                ViewBag.Definition = _gameManager.GetAllowedDefinitions();
                ViewBag.Skip = _gameManager.GetAllowSkip();
                ViewBag.Arguments = _gameManager.GetAllowArguments();
                return View(session);
            }

            return RedirectToAction("Result", session);
        }

        public IActionResult PartySelection(SessionViewModel session)
        {
            if (!_dbSessionManager.SessieCodeInUse(session.TeacherSessionCode))
                return RedirectToAction("Index", "Game");
            if (session.PartyName != null)
            {
                _gameManager = new GameManager(session.TeacherSessionCode, _dbTestManager, _dbSessionManager,
                    _dbPartyManager);
                _gameManager.SelectParty(session.StudentSessionId, session.PartyName);
                return RedirectToAction("WaitingScreen", session);
            }

            var parties = _dbSessionManager.GetChosenParties(session.TeacherSessionCode).ToList();
            ViewBag.Parties = parties;
            return View(session);
        }

        public IActionResult Result(SessionViewModel session)
        {
            if (session.GameType == GameType.PARTYGAME) return RedirectToAction("PartyResult", session);
            if (session.GameType == GameType.DEBATEGAME) return RedirectToAction("DebateResult", session);
            if (session.GameType == GameType.CUSTOMGAME_PARTY) return RedirectToAction("CustomPartyResult", session);
            if (session.GameType == GameType.CUSTOMGAME_DEBATE) return RedirectToAction("CustomDebateResult", session);

            return NoContent();
        }

        public IActionResult PartyResult(SessionViewModel session)
        {
            if (!_dbSessionManager.SessieCodeInUse(session.TeacherSessionCode))
                return RedirectToAction("Index", "Game");
            ViewBag.Statements = _dbSessionManager.GetStatements(session.TeacherSessionCode);

            _gameManager = new GameManager(session.TeacherSessionCode, _dbTestManager, _dbSessionManager,
                _dbPartyManager);
            var answers = _gameManager.GetAnswers(session.StudentSessionId);
            ViewBag.Answers = answers;

            var partyAnswers = _dbSessionManager
                .GetPartyAnswersForChosenStatements(session.TeacherSessionCode, session.PartyName).ToList();

            ViewBag.PartyAnswers = partyAnswers;

            var result = _gameManager.CalculateScore(session.StudentSessionId);
            ViewBag.Result = result;

            return View(session);
        }

        public IActionResult ShowParty(SessionViewModel session)
        {
            if (!_dbSessionManager.SessieCodeInUse(session.TeacherSessionCode))
            {
                string error = _localizer["Session has ended"];
                return RedirectToAction("Index", "Game", new {error});
            }

            ViewBag.Party = _dbPartyManager.GetParty(session.PartyName);
            return View(session);
        }

        public IActionResult DebateResult(SessionViewModel session)
        {
            if (!_dbSessionManager.SessieCodeInUse(session.TeacherSessionCode))
            {
                string error = _localizer["Session has ended"];
                return RedirectToAction("Index", "Game", new {error});
            }

            _gameManager = new GameManager(session.TeacherSessionCode, _dbTestManager, _dbSessionManager,
                _dbPartyManager);
            var debateResult = _gameManager.CalculateDebateScore(session.StudentSessionId);
            var precentages = debateResult.Values.ToList();
            var parties = debateResult.Keys.ToList();

            ViewBag.Precentages = precentages;
            ViewBag.Parties = parties;

            return View(session);
        }

        public IActionResult StatementResult(SessionViewModel session)
        {
            if (!_dbSessionManager.SessieCodeInUse(session.TeacherSessionCode))
            {
                string error = _localizer["Session has ended"];
                return RedirectToAction("Index", "Game", new {error});
            }

            var answerOptions = _dbSessionManager
                .GetAnswerOptionsByIndex(session.TeacherSessionCode, session.CurrentStatementId).ToList();
            ViewBag.AnswerOptions = answerOptions;
            var statement =
                _dbSessionManager.GetStatementFromChosenStatementsByIndex(session.TeacherSessionCode,
                    session.CurrentStatementId);
            ViewBag.Statement = statement;
            var definitions = _dbSessionManager.GetDefinitions(session.TeacherSessionCode).ToList();
            ViewBag.Definitions = definitions;
            var parties = _dbSessionManager.GetChosenParties(session.TeacherSessionCode).ToList();
            ViewBag.Parties = parties;
            var partyAnswers = _dbSessionManager
                .GetChosenPartyAnswersOfStatement(session.TeacherSessionCode, session.CurrentStatementId).ToList();
            ViewBag.PartyAnswers = partyAnswers;
            _gameManager = new GameManager(session.TeacherSessionCode, _dbTestManager, _dbSessionManager,
                _dbPartyManager);
            ViewBag.Definition = _gameManager.GetAllowedDefinitions();

            return View(session);
        }

        public IActionResult CustomPartyResult(SessionViewModel session)
        {
            if (!_dbSessionManager.SessieCodeInUse(session.TeacherSessionCode))
            {
                string error = _localizer["Session has ended"];
                return RedirectToAction("Index", "Game", new {error});
            }

            ViewBag.Statements = _dbSessionManager.GetStatements(session.TeacherSessionCode);

            _gameManager = new GameManager(session.TeacherSessionCode, _dbTestManager, _dbSessionManager,
                _dbPartyManager);
            ViewBag.Answers = _gameManager.GetAnswers(session.StudentSessionId);

            var correctOptions = _dbSessionManager.GetCorrectAOForChosenStatements(session.TeacherSessionCode).ToList();
            ViewBag.CorrectAnswers = correctOptions;

            var result = _gameManager.CalculateCustomPartyGameScore(session.StudentSessionId);
            ViewBag.Result = result;
            return View(session);
        }

        public IActionResult CustomDebateResult(SessionViewModel session)
        {
            if (!_dbSessionManager.SessieCodeInUse(session.TeacherSessionCode))
            {
                string error = _localizer["Session has ended"];
                return RedirectToAction("Index", "Game", new {error});
            }

            ViewBag.Statements = _dbSessionManager.GetStatements(session.TeacherSessionCode);

            _gameManager = new GameManager(session.TeacherSessionCode, _dbTestManager, _dbSessionManager,
                _dbPartyManager);
            ViewBag.Answers = _gameManager.GetAnswers(session.StudentSessionId);

            return View(session);
        }
    }
}