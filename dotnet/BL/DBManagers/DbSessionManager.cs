using System.Collections.Generic;
using BL.Domain.Identity;
using BL.Domain.Sessie;
using BL.Domain.Test;
using DAL.MySQL.MySQLRepositories;

namespace BL.DBManagers
{
    public class DbSessionManager
    {
        private readonly MySQLSessionRepository _repo;

        public DbSessionManager(MySQLSessionRepository mySqlSessionRepository)
        {
            _repo = mySqlSessionRepository;
        }

        public TeacherSession GetTeacherSession(int sessieCode)
        {
            return _repo.GetTeacherSession(sessieCode);
        }

        public int CreateLeerkrachtSessie(TeacherSession teacherSession)
        {
            return _repo.CreateTeacherSession(teacherSession);
        }

        public void UpdateLeerkrachtSessie(TeacherSession teacherSession)
        {
            _repo.UpdateTeacherSession(teacherSession);
        }

        public int GetStatementCount(int sessionCode)
        {
            return _repo.GetStatementCount(sessionCode);
        }

        public IEnumerable<Party> GetChosenParties(int sessionCode)
        {
            return _repo.GetChosenParties(sessionCode);
        }

        public IEnumerable<Party> GetChosenPartiesById(int sessionId)
        {
            return _repo.GetChosenPartiesById(sessionId);
        }

        public IEnumerable<Statement> GetStatements(int sessieCode)
        {
            return _repo.GetStatements(sessieCode);
        }

        public IEnumerable<Statement> GetStatementsPastSession(int sessionId)
        {
            return _repo.GetStatementsPastSesion(sessionId);
        }

        public bool SessieCodeInUse(int sessionCode)
        {
            return _repo.SessieCodeInUse(sessionCode);
        }

        public IEnumerable<AnswerOption> GetAnswerOptionsByIndex(int sessionCode, int index)
        {
            return _repo.GetAnswerOptionsByIndex(sessionCode, index);
        }

        public IEnumerable<AnswerOption> GetAnswerOptionsById(int sessionId, int index)
        {
            return _repo.GetAnswerOptionsById(sessionId, index);
        }

        public void AddChosenPartiesToTeacherSession(int sessionId, List<string> partyNames)
        {
            _repo.AddChosenPartiesToTeacherSession(sessionId, partyNames);
        }

        public void AnswerStatement(int answerOptionId, int studentSessionId, string argument, int statementId)
        {
            _repo.AnswerStatement(answerOptionId, studentSessionId, argument, statementId);
        }

        public IEnumerable<Answer> GetStudentAnswersForStatement(int sessionCode, int index)
        {
            return _repo.GetStudentAnswersForStatement(sessionCode, index);
        }

        public IEnumerable<Answer> GetStudentAnswersForStatementById(int sessionId, int index)
        {
            return _repo.GetStudentAnswersForStatementById(sessionId, index);
        }

        public Organisation GetOrganisation(int sessionCode)
        {
            return _repo.GetOrganisation(sessionCode);
        }

        public IEnumerable<StudentSession> GetStudentSessionsBySessionCode(int sessionCode)
        {
            return _repo.GetStudentSessionsBySessionCode(sessionCode);
        }

        public IEnumerable<StudentSession> GetStudentSessionsBySessionId(int sessionId)
        {
            return _repo.GetStudentSessionsBySessionId(sessionId);
        }

        public void NullifySessionCode(int sessionCode)
        {
            _repo.NullifySessionCode(sessionCode);
        }

        public GameType GetGameType(int sessionCode)
        {
            return _repo.GetGameType(sessionCode);
        }

        public IEnumerable<Answer> GetChosenPartyAnswersOfStatement(int sessionCode, int index)
        {
            return _repo.GetChosenPartyAnswersOfStatement(sessionCode, index);
        }

        public IEnumerable<AnswerOption> GetCorrectAnswerOptions(int sessionCode)
        {
            return _repo.GetCorrectAnswerOptions(sessionCode);
        }

        public IEnumerable<Definition> GetDefinitions(int sessionCode)
        {
            return _repo.GetDefinitions(sessionCode);
        }

        public void SetChosenStatements(int sessionCode, List<int> statementIds)
        {
            _repo.SetChosenStatements(sessionCode, statementIds);
        }

        public Statement GetStatementFromChosenStatementsByIndex(int sessionCode, int index)
        {
            return _repo.GetStatementFromChosenStatementsByIndex(sessionCode, index);
        }

        public IEnumerable<Answer> GetPartyAnswersForChosenStatements(int sessionCode, string partyId)
        {
            return _repo.GetPartyAnswersForChosenStatements(sessionCode, partyId);
        }

        public IEnumerable<AnswerOption> GetCorrectAOForChosenStatements(int sessionCode)
        {
            return _repo.GetCorrectAOForChosenStatements(sessionCode);
        }

        public IEnumerable<Statement> GetAllChosenStatements(int sessieCode)
        {
            return _repo.GetAllChosenStatements(sessieCode);
        }
    }
}