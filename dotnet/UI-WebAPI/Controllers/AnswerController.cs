using System;
using System.Collections.Generic;
using System.Linq;
using BL;
using BL.DBManagers;
using Microsoft.AspNetCore.Mvc;

namespace UI_WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AnswerController : ControllerBase
    {
        private readonly DbPartyManager _dbPartyManager;
        private readonly DbSessionManager _dbSessionManager;
        private readonly DbTestManager _dbTestManager;


        public AnswerController(DbTestManager dbTestManager, DbSessionManager dbSessionManager,
            DbPartyManager dbPartyManager)
        {
            _dbTestManager = dbTestManager;
            _dbSessionManager = dbSessionManager;
            _dbPartyManager = dbPartyManager;
        }

        [HttpPut]
        [Route("AnswerStatement/{sessionCode}/{studentId}/{argument}/{answerOptionId}")]
        public IActionResult AnswerStatement(int sessionCode, int studentId, string argument, int answerOptionId)
        {
            var gameManager = new GameManager(sessionCode, _dbTestManager, _dbSessionManager, _dbPartyManager);
            gameManager.AnswerStatement(argument, studentId, answerOptionId);
            Console.WriteLine("Answer verwerkt" + argument + " " + answerOptionId);
            return NoContent();
        }

        [HttpGet]
        [Route("GetPartyAnswers/{sessionCode}/{partyName}")]
        public IActionResult GetPartyAnswers(int sessionCode, string partyName)
        {
            var partyAnswers = _dbSessionManager.GetPartyAnswersForChosenStatements(sessionCode, partyName).ToList();

            var answers = new List<object>();

            foreach (var answer in partyAnswers)
                answers.Add(new
                    {
                        id = answer.Id,
                        argument = answer.Argument,
                        chosenAnswerId = answer.ChosenAnswer.Id,
                        statementId = answer.Statement.Id,
                        partyName
                    }
                );
            if (partyAnswers.Count() != 0) return Ok(answers);

            return NotFound();
        }

        [HttpGet]
        [Route("GetAllPartyAnswers/{sessionCode}")]
        public IActionResult GetAllPartyAnswers(int sessionCode)
        {
            var parties = _dbSessionManager.GetChosenParties(sessionCode).ToList();
            var answers = new List<object>();
            foreach (var party in parties)
            {
                var partyAnswers = _dbSessionManager.GetPartyAnswersForChosenStatements(sessionCode, party.Name)
                    .ToList();

                foreach (var answer in partyAnswers)
                    answers.Add(new
                        {
                            id = answer.Id,
                            argument = answer.Argument,
                            chosenAnswerId = answer.ChosenAnswer.Id,
                            statementId = answer.Statement.Id,
                            partyName = party.Name
                        }
                    );
            }

            if (answers.Count != 0) return Ok(answers);

            return NotFound();
        }

        [HttpGet]
        [Route("GetCPgAnswers/{sessionCode}")]
        public IActionResult GetCPgAnswers(int sessionCode)
        {
            var correctAnswers = _dbSessionManager.GetCorrectAnswerOptions(sessionCode).ToList();
            var answers = new List<object>();

            foreach (var answer in correctAnswers)
                answers.Add(new
                {
                    id = answer.Id,
                    opinion = answer.Opinion,
                    correct = true
                });

            return Ok(answers);
        }
    }
}