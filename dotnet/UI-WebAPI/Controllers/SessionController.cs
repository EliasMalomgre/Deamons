using System;
using System.Collections.Generic;
using System.Linq;
using BL;
using BL.DBManagers;
using BL.Domain.Test;
using Microsoft.AspNetCore.Mvc;
using UI_WebAPI.ConvertModels;

namespace UI_WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SessionController : ControllerBase
    {
        private readonly SessionManager _sessionManager;
        private readonly DbPartyManager _dbPartyManager;
        private readonly DbSessionManager _dbSessionManager;
        private readonly DbTestManager _dbTestManager;

        public SessionController(DbTestManager dbTestManager, DbSessionManager dbSessionManager,
            DbPartyManager dbPartyManager,
            DbUserManager dbUserManager)
        {
            _dbTestManager = dbTestManager;
            _dbSessionManager = dbSessionManager;
            _dbPartyManager = dbPartyManager;
            _sessionManager = new SessionManager(_dbSessionManager, _dbTestManager, dbUserManager, _dbPartyManager);
        }

        [HttpGet]
        [Route("GetStatement/{sessionCode}/{index}")]
        public IActionResult GetStatement(int sessionCode, int index)
        {
            var statement = _dbSessionManager.GetStatementFromChosenStatementsByIndex(sessionCode, index);

            if (statement != null)
            {
                Console.WriteLine("statement" + statement.Id + "given" + statement.Explanation);
                return Ok(statement);
            }

            return NotFound();
        }

        [HttpGet]
        [Route("GetAnswerOptions/{sessionCode}/{index}")]
        public IActionResult GetAnswerOptions(int sessionCode, int index)
        {
            var answerOptions = _dbSessionManager.GetAnswerOptionsByIndex(sessionCode, index).ToList();

            if (answerOptions.Count != 0) return Ok(answerOptions);

            return NotFound();
        }

        [HttpGet]
        [Route("GetGameSettings/{sessionCode}")]
        public IActionResult GetGameSettings(int sessionCode)
        {
            var ts = _dbSessionManager.GetTeacherSession(sessionCode);
            var amountOfStatements = _dbSessionManager.GetStatementCount(sessionCode);

            var options = new
            {
                id = 0,
                amountOfStatements,
                ts.Settings.ArgumentsAllowed,
                ts.Settings.DefinitionsGiven,
                ts.Settings.SkipAllowed,
                ts.GameType,
                ts.Settings.Colour1,
                ts.Settings.Colour2,
                ts.Settings.Colour3,
                ts.Settings.Colour4,
                ts.Settings.Colour5,
                ts.Settings.Colour6,
                ts.Settings.ColourSkip
            };
            return Ok(options);
        }

        [HttpGet]
        [Route("GetChosenParties/{sessionCode}")]
        public IActionResult GetChosenParties(int sessionCode)
        {
            IEnumerable<Party> partijen = _dbSessionManager.GetChosenParties(sessionCode).ToList();
            var parties = new List<object>();

            foreach (var partij in partijen)
                parties.Add(new
                {
                    partij.Name,
                    partij.Orientation,
                    partij.Colour,
                    partij.PartyLeader,
                    Logo = partij.ImageLink,
                    PartyMediaLink = partij.MediaLink
                });

            if (partijen.Count() != 0) return Ok(parties);
            return NotFound();
        }

        [HttpPost("{sessionCode}")]
        [Route("AddStudent/{sessionCode}")]
        public IActionResult AddStudent(int sessionCode)
        {
            var gameManager = new GameManager(sessionCode, _dbTestManager, _dbSessionManager, _dbPartyManager);
            var studentId = gameManager.AddStudent();
            return Ok(studentId);
        }

        [HttpPut("{sessionCode}/{studentId}/{selectedParty}")]
        [Route("selectParty/{sessionCode}/{studentId}/{selectedParty}")]
        public IActionResult PutSelectParty(int sessionCode, int studentId, string selectedParty)
        {
            var gameManager = new GameManager(sessionCode, _dbTestManager, _dbSessionManager, _dbPartyManager);
            gameManager.SelectParty(studentId, selectedParty);
            return NoContent();
        }

        [HttpGet]
        [Route("EndGame/{sessionCode}/{studentId}")]
        public IActionResult GetEndPartyGame(int sessionCode, int studentId)
        {
            var manager = new GameManager(sessionCode, _dbTestManager, _dbSessionManager, _dbPartyManager);
            var score = manager.CalculateScore(studentId);
            if (score == null) return BadRequest();

            return Ok(score);
        }

        [HttpGet]
        [Route("EndDebateGame/{sessionCode}/{studentId}")]
        public IActionResult GetEndDebateGame(int sessionCode, int studentId)
        {
            var manager = new GameManager(sessionCode, _dbTestManager, _dbSessionManager, _dbPartyManager);
            var scores = manager.CalculateDebateScore(studentId);
            if (scores == null) return BadRequest();

            var scoresToReturn = new List<object>();

            foreach (var score in scores)
                scoresToReturn.Add
                (new
                    {
                        partyName = score.Key.Name,
                        percentage = score.Value
                    }
                );

            return Ok(scoresToReturn);
        }


        [HttpGet]
        [Route("EndCustomPartyGame/{sessionCode}/{studentId}")]
        public IActionResult EndCustomPartyGame(int sessionCode, int studentId)
        {
            var manager = new GameManager(sessionCode, _dbTestManager, _dbSessionManager, _dbPartyManager);
            var score = manager.CalculateCustomPartyGameScore(studentId);

            if (score == null) return BadRequest();

            return Ok(score);
        }

        [HttpGet]
        [Route("GetCustomDebateGameResults/{sessionCode}")]
        public IActionResult GetCustomDebateGameResults(int sessionCode)
        {
            var manager = new GameManager(sessionCode, _dbTestManager, _dbSessionManager, _dbPartyManager);
            var statements = _dbSessionManager.GetAllChosenStatements(sessionCode).ToList();
            statements = statements.OrderBy(st => st.Id).ToList();
            var results = new List<object>();
            var index = 0;

            foreach (var statement in statements)
            {
                var answers = manager.GetAllAnswers().ToList();
                var answersForStatement = manager.GetAllAnswers().Where(a => a.Statement.Id == statement.Id).ToList();
                var flot = answeroptionValues(statement, answersForStatement);

                results.Add(new
                {
                    id = /*statement.Id,*/index,
                    values = flot
                });
                index++;
            }

            return Ok(results);
        }

        private float[] answeroptionValues(Statement statement, List<Answer> answersForStatement)
        {
            var aos = statement.AnswerOptions.ToList();
            var answeroptionValues = new float[aos.Count];

            for (var j = 0; j < aos.Count; j++)
            {
                var ansOption = aos[j];
                for (var i = 0; i < answersForStatement.Count; i++)
                    if (answersForStatement[i].ChosenAnswer.Id == ansOption.AnswerOption.Id)
                        answeroptionValues[j] += 1.0f;
            }

            return answeroptionValues;
        }


        [HttpGet]
        [Route("DebateChart/{sessionCode}/{index}")]
        public IActionResult DebateChart(int sessionCode, int index)
        {
            var data = _sessionManager.DebateResult(sessionCode, index);
            var dataPoints = new List<DataPoint>();

            foreach (var entry in data) dataPoints.Add(new DataPoint(entry.Value, entry.Key, null));

            if (!dataPoints.Any()) return BadRequest();

            return Ok(dataPoints);
        }

        [HttpGet]
        [Route("PastDebateChart/{sessionId}/{index}")]
        public IActionResult PastDebateChart(int sessionId, int index)
        {
            var data = _sessionManager.PastDebateResult(sessionId, index);
            var dataPoints = new List<DataPoint>();

            foreach (var entry in data) dataPoints.Add(new DataPoint(entry.Value, entry.Key, null));

            if (!dataPoints.Any()) return BadRequest();

            return Ok(dataPoints);
        }

        [HttpGet]
        [Route("GetDefinitions/{sessionCode}")]
        public IActionResult GetDefinitions(int sessionCode)
        {
            return Ok(_dbSessionManager.GetDefinitions(sessionCode));
        }

        [HttpGet]
        [Route("PartyChart/{sessionCode}")]
        public IActionResult PartyChart(int sessionCode)
        {
            var data = _sessionManager.PartyResult(sessionCode);
            var dataPoints = new List<DataPoint>();
            var parties = _dbSessionManager.GetChosenParties(sessionCode).ToList();
            foreach (var entry in data)
            {
                var colour = parties.Find(p => p.Name == entry.Key).Colour;
                dataPoints.Add(new DataPoint(entry.Value, entry.Key, colour));
            }

            return Ok(dataPoints);
        }

        [HttpGet]
        [Route("PastPartyChart/{sessionId}")]
        public IActionResult PastPartyChart(int sessionId)
        {
            var data = _sessionManager.PastPartyResult(sessionId);
            var dataPoints = new List<DataPoint>();
            var parties = _dbSessionManager.GetChosenPartiesById(sessionId).ToList();
            foreach (var entry in data)
            {
                var colour = parties.Find(p => p.Name == entry.Key).Colour;
                dataPoints.Add(new DataPoint(entry.Value, entry.Key, colour));
            }

            return Ok(dataPoints);
        }
    }
}