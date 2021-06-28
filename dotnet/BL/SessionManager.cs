using System;
using System.Collections.Generic;
using System.Linq;
using BL.DBManagers;
using BL.Domain.Sessie;
using BL.Domain.Test;

namespace BL
{
    public class SessionManager
    {
        private DbPartyManager _partyMgr;
        private readonly DbSessionManager _sessionMgr;
        private readonly DbTestManager TestMgr;
        private readonly DbUserManager UserMgr;

        public SessionManager(DbSessionManager dbSessionManager, DbTestManager dbTestManager,
            DbUserManager dbUserManager, DbPartyManager dbPartyManager)
        {
            _sessionMgr = dbSessionManager;
            TestMgr = dbTestManager;
            UserMgr = dbUserManager;
            _partyMgr = dbPartyManager;
        }

        public int StartSession(int testId, GameType gameType, string className, string userId, bool arguments,
            bool definitions, bool skipStatements, bool forceWaiting, string colour1, string colour2, string colour3,
            string colour4, string colour5, string colour6, string colourSkip)
        {
            var test = TestMgr.GetTest(testId);
            var @class = UserMgr.GetClass(userId, className);
            var random = new Random();
            var isUnique = false;
            var sessionCode = 0;
            while (!isUnique)
            {
                sessionCode = random.Next(899999) + 100000;
                //Nakijken of niet in use
                isUnique = !_sessionMgr.SessieCodeInUse(sessionCode);
            }

            var user = UserMgr.GetUser(userId);
            if (user == null) Console.WriteLine("ERROR ERROR ERROR");

            var teacherSession = new TeacherSession(test, gameType, @class, user, sessionCode);
            teacherSession.Settings = new SessionSettings();
            teacherSession.Settings.ArgumentsAllowed = arguments;
            teacherSession.Settings.DefinitionsGiven = definitions;
            teacherSession.Settings.ForceWaiting = forceWaiting;
            teacherSession.Settings.SkipAllowed = skipStatements;
            
            if (colour1 == null || colour1 == "#7FFFD4")
                teacherSession.Settings.Colour1 = "#8CB369";
            else
                teacherSession.Settings.Colour1 = colour1;
            if (colour2 == null || colour2 == "##7FFFD4")
                teacherSession.Settings.Colour2 = "#D7263D";
            else
                teacherSession.Settings.Colour2 = colour2;
            if (colour3 == null || colour3 == "#7FFFD4")
                teacherSession.Settings.Colour3 = "#F85A3E";
            else
                teacherSession.Settings.Colour3 = colour3;
            if (colour4 == null || colour4 == "#7FFFD4")
                teacherSession.Settings.Colour4 = "#1098F7";
            else
                teacherSession.Settings.Colour4 = colour4;
            if (colour5 == null || colour5 == "#7FFFD4")
                teacherSession.Settings.Colour5 = "#F49D37";
            else
                teacherSession.Settings.Colour5 = colour5;
            if (colour6 == null || colour6 == "#7FFFD4")
                teacherSession.Settings.Colour6 = "#AA1155";
            else
                teacherSession.Settings.Colour6 = colour6;
            if (colourSkip == null || colourSkip == "#7FFFD4")
                teacherSession.Settings.ColourSkip = "#E0ACD5";
            else
                teacherSession.Settings.ColourSkip = colourSkip;
            teacherSession.CurrentStatement = -1;

            _sessionMgr.CreateLeerkrachtSessie(teacherSession);
            return sessionCode;
        }

        public void EndSession(int sessionCode)
        {
            _sessionMgr.NullifySessionCode(sessionCode);
        }

        public Dictionary<string, int> DebateResult(int sessionCode, int index)
        {
            var studentAnswers = _sessionMgr.GetStudentAnswersForStatement(sessionCode, index).ToList();
            var answerOptions = _sessionMgr.GetAnswerOptionsByIndex(sessionCode, index).ToList();
            return CalculateDebateResult(studentAnswers, answerOptions);
        }


        public Dictionary<string, int> PastDebateResult(int sessionId, int index)
        {
            var studentAnswers = _sessionMgr.GetStudentAnswersForStatementById(sessionId, index).ToList();
            var answerOptions = _sessionMgr.GetAnswerOptionsById(sessionId, index).ToList();
            return CalculateDebateResult(studentAnswers, answerOptions);
        }

        private Dictionary<string, int> CalculateDebateResult(List<Answer> studentAnswers,
            List<AnswerOption> answerOptions)
        {
            var dataPoints = new Dictionary<string, int>();
            var answerCount = studentAnswers.Count;
            foreach (var option in answerOptions)
            {
                var count = 0;
                foreach (var answer in studentAnswers)
                    if (option.Opinion.Equals(answer.ChosenAnswer.Opinion))
                        count++;
                int precentage;
                if (answerCount == 0)
                    precentage = 0;
                else
                    precentage = count * 100 / answerCount;
                dataPoints.Add(option.Opinion, precentage);
            }

            return dataPoints;
        }

        public Dictionary<string, int> PartyResult(int sessionCode)
        {
            var parties = _sessionMgr.GetChosenParties(sessionCode).ToList();
            var sessions = _sessionMgr.GetStudentSessionsBySessionCode(sessionCode).ToList();

            return CalculatePartyResult(parties, sessions);
        }

        public Dictionary<string, int> PastPartyResult(int sessionId)
        {
            var parties = _sessionMgr.GetChosenPartiesById(sessionId).ToList();
            var sessions = _sessionMgr.GetStudentSessionsBySessionId(sessionId).ToList();
            return CalculatePartyResult(parties, sessions);
        }

        private Dictionary<string, int> CalculatePartyResult(List<Party> parties, List<StudentSession> sessions)
        {
            var data = new Dictionary<string, int>();
            sessions = sessions.Where(x => x.Score != "").ToList();
            foreach (var party in parties)
            {
                var count = 0;
                double total = 0;
                foreach (var session in sessions)
                    if (session.SelectedParty.Equals(party.Name) && session.Score != "")
                    {
                        var split = session.Score.Split('/');
                        var score = double.Parse(split[0]);
                        var statementCount = double.Parse(split[1]);
                        if (statementCount > 0) total += score / statementCount * 100;

                        count++;
                    }

                int percentage;
                if (count == 0)
                    percentage = 0;
                else
                    percentage = (int) total / count;

                data.Add(party.Name, percentage);
            }

            return data;
        }
    }
}