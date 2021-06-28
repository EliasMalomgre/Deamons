using System;
using System.Collections.Generic;
using System.Linq;
using BL.Domain.Identity;
using BL.Domain.Sessie;
using BL.Domain.Test;
using Microsoft.EntityFrameworkCore;

namespace DAL.MySQL.MySQLRepositories
{
    public class MySQLSessionRepository
    {
        private readonly StemtestDbContext ctx;

        public MySQLSessionRepository(StemtestDbContext stemtestDbContext)
        {
            ctx = stemtestDbContext;
        }

        public int CreateTeacherSession(TeacherSession teacherSession)
        {
            ctx.TeacherSessions.Add(teacherSession);
            ctx.SaveChanges();
            return teacherSession.Id;
        }

        public void UpdateTeacherSession(TeacherSession teacherSession)
        {
            ctx.TeacherSessions.Update(teacherSession);
            ctx.SaveChanges();
        }

        public TeacherSession GetTeacherSession(int sessieCode)
        {
            return ctx.TeacherSessions
                .Include(ts => ts.Settings)
                .Include(l => l.Test)
                .ThenInclude(t => t.Statements)
                .Include(l => l.Test)
                .ThenInclude(t => t.Tags)
                .Include(l => l.Class)
                .Include(l => l.Teacher)
                .Include(l => l.StudentSessions).ThenInclude(ss => ss.Answers)
                .ThenInclude(a => a.ChosenAnswer)
                .Include(ts => ts.ChosenStatements).ThenInclude(s => s.Statement)
                .FirstOrDefault(l => l.SessionCode == sessieCode);
        }

        public int GetStatementCount(int sessionCode)
        {
            var ts = ctx.TeacherSessions
                .Include(l => l.Test)
                .Include(ts => ts.ChosenStatements)
                .ThenInclude(cs => cs.Statement)
                .First(l => l.SessionCode == sessionCode);
            return ts.ChosenStatements.Count;
        }

        public IEnumerable<Party> GetChosenParties(int sessionCode)
        {
            var pts = ctx.TeacherSessions.Include(t => t.ChosenParties)
                .ThenInclude(c => c.Party)
                .FirstOrDefault(t => t.SessionCode == sessionCode)
                .ChosenParties;
            if (pts == null)
            {
                throw new Exception("Data could not be retrieved for the database!");
            }

            var parties = new List<Party>();
            foreach (var pt in pts) parties.Add(pt.Party);

            return parties.AsEnumerable();
        }

        public IEnumerable<Party> GetChosenPartiesById(int sessionId)
        {
            var pts = ctx.TeacherSessions.Include(t => t.ChosenParties)
                .ThenInclude(c => c.Party)
                .FirstOrDefault(t => t.Id == sessionId)
                .ChosenParties;
            if (pts == null)
            {
                throw new Exception("Data could not be retrieved for the database!");
            }

            var parties = new List<Party>();
            foreach (var pt in pts) parties.Add(pt.Party);

            return parties.AsEnumerable();
        }

        public IEnumerable<Statement> GetStatements(int sessionCode)
        {
            var ts = ctx.TeacherSessions
                .Include(ts => ts.ChosenStatements)
                .ThenInclude(cs => cs.Statement)
                .FirstOrDefault(l => l.SessionCode == sessionCode);
            if (ts == null) throw new NullReferenceException("Test could not be found in database.");

            var stats = new List<Statement>();
            ts.ChosenStatements = ts.ChosenStatements.OrderBy(st => st.Statement.Id).ToList();
            foreach (var cs in ts.ChosenStatements) stats.Add(cs.Statement);

            return stats;
        }

        public IEnumerable<Statement> GetStatementsPastSesion(int sessionId)
        {
            var ts = ctx.TeacherSessions
                .Include(ts => ts.ChosenStatements)
                .ThenInclude(cs => cs.Statement)
                .FirstOrDefault(l => l.Id == sessionId);
            if (ts == null) throw new NullReferenceException("Test could not be found in database.");

            var stats = new List<Statement>();
            ts.ChosenStatements = ts.ChosenStatements.OrderBy(st => st.Statement.Id).ToList();

            foreach (var cs in ts.ChosenStatements) stats.Add(cs.Statement);

            return stats;
        }

        public Statement GetStatementByIndex(int sessionCode, int index)
        {
            var statements =
                ctx.TeacherSessions.Include(l => l.Test).ThenInclude(t => t.Statements).ThenInclude(s => s.Definitions)
                    .FirstOrDefault(l => l.SessionCode == sessionCode).Test.Statements;

            if (statements.Count <= index) return null;

            return statements[index];
        }

        public bool SessieCodeInUse(int sessionCode)
        {
            var sessie = ctx.TeacherSessions.FirstOrDefault(l => l.SessionCode == sessionCode);
            return sessie != null;
        }

        public IEnumerable<AnswerOption> GetAnswerOptionsByIndex(int sessionCode, int index)
        {
            var ts = ctx.TeacherSessions
                .Include(ts => ts.ChosenStatements)
                .ThenInclude(cs => cs.Statement)
                .ThenInclude(s => s.AnswerOptions)
                .ThenInclude(ao => ao.AnswerOption)
                .FirstOrDefault(t => t.SessionCode == sessionCode);
            ts.ChosenStatements = ts.ChosenStatements.OrderBy(st => st.Statement.Id).ToList();
            var statement = ts.ChosenStatements[index].Statement;

            var aoList = new List<AnswerOption>();
            foreach (var ao in statement.AnswerOptions) aoList.Add(ao.AnswerOption);

            return aoList.AsEnumerable();
        }

        public IEnumerable<AnswerOption> GetAnswerOptionsById(int sessionId, int index)
        {
            var ts = ctx.TeacherSessions
                .Include(ts => ts.ChosenStatements)
                .ThenInclude(cs => cs.Statement)
                .ThenInclude(s => s.AnswerOptions)
                .ThenInclude(ao => ao.AnswerOption)
                .FirstOrDefault(t => t.Id == sessionId);
            ts.ChosenStatements = ts.ChosenStatements.OrderBy(s => s.Statement.Id).ToList();
            var statement = ts.ChosenStatements[index].Statement;

            var aoList = new List<AnswerOption>();
            foreach (var ao in statement.AnswerOptions) aoList.Add(ao.AnswerOption);

            return aoList.AsEnumerable();
        }

        public void AddChosenPartiesToTeacherSession(int sessionCode, List<string> partyNames)
        {
            var ts = ctx.TeacherSessions.FirstOrDefault(t => t.SessionCode == sessionCode);

            foreach (var partyName in partyNames)
            {
                var party = ctx.Parties.Find(partyName);
                var pt = new PartyTeacherSession();
                pt.Party = party;
                pt.TeacherSession = ts;
                ctx.Add(pt);
                ctx.SaveChanges();
            }
        }

        public void AnswerStatement(int answerOptionId, int studentSessionId, string argument, int statementId)
        {
            var answer = new Answer();
            answer.Statement = ctx.Statements.Find(statementId);
            answer.Argument = argument;
            answer.ChosenAnswer = ctx.AnswerOptions.Find(answerOptionId);
            var sts = ctx.StudentSessions.Find(studentSessionId);
            sts.Answers.Add(answer);
            ctx.StudentSessions.Update(sts);
            ctx.SaveChanges();
        }

        public IEnumerable<Answer> GetStudentAnswersForStatement(int sessionCode, int index)
        {
            var ts = ctx.TeacherSessions
                .Include(t => t.StudentSessions)
                .ThenInclude(s => s.Answers)
                .ThenInclude(a => a.ChosenAnswer)
                .Include(t => t.StudentSessions)
                .FirstOrDefault(t => t.SessionCode == sessionCode);
            if (ts == null)
            {
                throw new Exception("Data could not be retrieved for the database!");
            }

            var answers = new List<Answer>();
            var ao = ctx.AnswerOptions.Find(3);
            foreach (var sts in ts.StudentSessions)
                if (sts.Answers.Count <= index)
                {
                }
                else
                {
                    answers.Add(sts.Answers[index]);
                }

            return answers;
        }

        public IEnumerable<Answer> GetStudentAnswersForStatementById(int sessionId, int index)
        {
            var ts = ctx.TeacherSessions
                .Include(t => t.StudentSessions)
                .ThenInclude(s => s.Answers)
                .ThenInclude(a => a.ChosenAnswer)
                .Include(t => t.StudentSessions)
                .FirstOrDefault(t => t.Id == sessionId);
            if (ts == null)
            {
                throw new Exception("Data could not be retrieved for the database!");
            }

            var answers = new List<Answer>();
            var ao = ctx.AnswerOptions.Find(3);
            foreach (var sts in ts.StudentSessions)
                if (sts.Answers.Count <= index)
                {
                }
                else
                {
                    answers.Add(sts.Answers[index]);
                }

            return answers;
        }

        public Organisation GetOrganisation(int sessionCode)
        {
            var ts = ctx.TeacherSessions
                .Include(t => t.Teacher)
                .ThenInclude(t => t.Organisation)
                .FirstOrDefault(t => t.SessionCode == sessionCode);
            if (ts == null)
            {
                throw new Exception("Data could not be retrieved for the database!");
            }

            return ts.Teacher.Organisation;
        }

        public IEnumerable<StudentSession> GetStudentSessionsBySessionCode(int sessionCode)
        {
            var ts = ctx.TeacherSessions
                .Include(t => t.StudentSessions)
                .ThenInclude(ss => ss.Answers)
                .FirstOrDefault(t => t.SessionCode == sessionCode);
            if (ts == null)
            {
                throw new Exception("Data could not be retrieved for the database!");
            }

            return ts.StudentSessions;
        }

        public IEnumerable<StudentSession> GetStudentSessionsBySessionId(int sessionId)
        {
            var ts = ctx.TeacherSessions
                .Include(t => t.StudentSessions)
                .ThenInclude(ss => ss.Answers)
                .FirstOrDefault(t => t.Id == sessionId);
            if (ts == null)
            {
                throw new Exception("Data could not be retrieved for the database!");
            }

            return ts.StudentSessions;
        }

        public void NullifySessionCode(int sessionCode)
        {
            var ts = ctx.TeacherSessions.FirstOrDefault(ts => ts.SessionCode == sessionCode);
            if (ts == null)
            {
                throw new Exception("Data could not be retrieved for the database!");
            }

            ts.SessionCode = -1;
            ctx.TeacherSessions.Update(ts);
            ctx.SaveChanges();
        }

        public GameType GetGameType(int sessionCode)
        {
            var ts = ctx.TeacherSessions.FirstOrDefault(ts => ts.SessionCode == sessionCode);
            if (ts == null)
            {
                throw new Exception("Data could not be retrieved for the database!");
            }

            return ts.GameType;
        }

        public IEnumerable<Answer> GetChosenPartyAnswersOfStatement(int sessionCode, int index)
        {
            var ts = ctx.TeacherSessions.Include(s => s.ChosenParties)
                .ThenInclude(c => c.Party)
                .ThenInclude(p => p.Answers).ThenInclude(a => a.Statement).ThenInclude(s => s.AnswerOptions)
                .ThenInclude(sao => sao.AnswerOption)
                .Include(ts => ts.ChosenStatements)
                .ThenInclude(cs => cs.Statement)
                .FirstOrDefault(s => s.SessionCode == sessionCode);
            if (ts == null)
            {
                throw new Exception("Data could not be retrieved for the database!");
            }

            var statement = GetStatementByIndex(sessionCode, index);
            var answers = new List<Answer>();
            foreach (var pts in ts.ChosenParties)
                answers.Add(pts.Party.Answers.FirstOrDefault(a => a.Statement.Id == statement.Id));

            return answers;
        }

        public IEnumerable<AnswerOption> GetCorrectAnswerOptions(int sessionCode)
        {
            var ts = ctx.TeacherSessions.Include(ts => ts.Test)
                .ThenInclude(t => t.Statements)
                .ThenInclude(s => s.AnswerOptions)
                .ThenInclude(ao => ao.AnswerOption)
                .FirstOrDefault(ts => ts.SessionCode == sessionCode);
            var aos = new List<AnswerOption>();
            foreach (var statement in ts.Test.Statements)
            foreach (var sao in statement.AnswerOptions)
                if (sao.isCorrectAnswer)
                    aos.Add(sao.AnswerOption);

            return aos;
        }

        public IEnumerable<Definition> GetDefinitions(int sessieCode)
        {
            var ts = ctx.TeacherSessions
                .Include(ts => ts.Test)
                .FirstOrDefault(ts => ts.SessionCode == sessieCode);
            if (ts == null)
            {
                throw new Exception("Data could not be retrieved for the database!");
            }

            var definitions = ctx.Definitions.Where(d => d.Test.Id == ts.Test.Id).ToList();
            return definitions;
        }

        public void SetChosenStatements(int sessionCode, List<int> statementIds)
        {
            var ts = ctx.TeacherSessions.FirstOrDefault(ts => ts.SessionCode == sessionCode);
            if (ts == null) throw new Exception("Ho eens makker, er was iets null!");

            ts.ChosenStatements = new List<ChosenStatement>();

            foreach (var i in statementIds)
            {
                var chosenStatement = new ChosenStatement();
                chosenStatement.Statement = ctx.Statements.Find(i);
                ts.ChosenStatements.Add(chosenStatement);
                ctx.Update(ts);
                ctx.SaveChanges();
            }
        }

        public Statement GetStatementFromChosenStatementsByIndex(int sessionCode, int index)
        {
            var ts = ctx.TeacherSessions.Include(ts => ts.ChosenStatements)
                .ThenInclude(cs => cs.Statement)
                .FirstOrDefault(ts => ts.SessionCode == sessionCode);
            if (ts == null) throw new Exception("Ho eens even makker, er is hier iets null");

            ts.ChosenStatements = ts.ChosenStatements.OrderBy(s => s.Statement.Id).ToList();
            return ts.ChosenStatements[index].Statement;
        }

        public IEnumerable<Statement> GetAllChosenStatements(int sessieCode)
        {
            var ts = ctx.TeacherSessions.Include(ts => ts.ChosenStatements)
                .ThenInclude(cs => cs.Statement)
                .ThenInclude(s => s.AnswerOptions)
                .ThenInclude(ao => ao.AnswerOption)
                .FirstOrDefault(ts => ts.SessionCode == sessieCode);

            var statements = new List<Statement>();

            foreach (var cs in ts.ChosenStatements) statements.Add(cs.Statement);

            return statements;
        }

        public IEnumerable<Answer> GetPartyAnswersForChosenStatements(int sessionCode, string partyId)
        {
            var ts = ctx.TeacherSessions.Include(ts => ts.ChosenStatements)
                .ThenInclude(cs => cs.Statement).FirstOrDefault(ts => ts.SessionCode == sessionCode);
            if (ts == null) throw new Exception("Ho eens makker, er is hier iets null!");

            var party = ctx.Parties.Include(p => p.Answers)
                .ThenInclude(a => a.Statement).ThenInclude(a => a.AnswerOptions).ThenInclude(a => a.AnswerOption)
                .FirstOrDefault(p => p.Name.Equals(partyId));
            if (party == null) throw new Exception("Ho eens makker, er is hier iets null!");

            var answers = new List<Answer>();
            foreach (var cs in ts.ChosenStatements)
                answers.Add(party.Answers.Find(a => a.Statement.Id == cs.Statement.Id));

            return answers;
        }

        public IEnumerable<AnswerOption> GetCorrectAOForChosenStatements(int sessionCode)
        {
            var ts = ctx.TeacherSessions.Include(ts => ts.ChosenStatements)
                .ThenInclude(ca => ca.Statement)
                .ThenInclude(s => s.AnswerOptions)
                .ThenInclude(ao => ao.AnswerOption)
                .FirstOrDefault(ts => ts.SessionCode == sessionCode);

            if (ts == null) throw new Exception("Ho eens even makker, er is iets null!");

            ts.ChosenStatements = ts.ChosenStatements.OrderBy(s => s.Statement.Id).ToList();
            var aoList = new List<AnswerOption>();
            foreach (var ca in ts.ChosenStatements)
                aoList.Add(ca.Statement.AnswerOptions.FirstOrDefault(ao => ao.isCorrectAnswer).AnswerOption);

            return aoList;
        }

        public List<Answer> GetAllPartyAnswersFromChosenStatements(int sessioncode)
        {
            var ts = ctx.TeacherSessions.Include(ts => ts.ChosenParties)
                .ThenInclude(ps => ps.Party)
                .FirstOrDefault(ts => ts.SessionCode == sessioncode);
            if (ts == null) throw new Exception("Ho eens even makker, er is hier iets null!");

            var parties = ts.ChosenParties;
            var answers = new List<Answer>();
            foreach (var pts in parties)
            foreach (var ans in GetPartyAnswersForChosenStatements(sessioncode, pts.Party.Name))
                answers.Add(ans);

            return answers;
        }
    }
}