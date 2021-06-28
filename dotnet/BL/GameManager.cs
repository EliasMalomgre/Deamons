using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using BL.DBManagers;
using BL.Domain.Sessie;
using BL.Domain.Test;

namespace BL
{
    public class GameManager
    {
        private readonly DbPartyManager _partyManager;
        private readonly int _sessionCode;
        private readonly DbSessionManager _sessionManager;
        private TeacherSession _teacherSession;
        private readonly DbTestManager _testManager;

        public GameManager(int sessionCode, DbTestManager dbTestManager, DbSessionManager dbSessionManager,
            DbPartyManager dbPartyManager)
        {
            _testManager = dbTestManager;
            _sessionManager = dbSessionManager;
            _partyManager = dbPartyManager;
            _teacherSession = _sessionManager.GetTeacherSession(sessionCode);
            _sessionCode = sessionCode;
        }

        public int AddStudent()
        {
            if (_teacherSession.CurrentAmountStudents < _teacherSession.MaxAmountStudents)
            {
                var sts = new StudentSession();
                sts.Answers = new List<Answer>();
                sts.LastAnsweredStatement = 0;
                sts.Score = string.Empty;
                var studentId = _testManager.AddStudentSession(_teacherSession.Id, sts);
                _teacherSession.CurrentAmountStudents++;
                Update();
                return studentId;
            }

            return -1;
        }


        public void SelectParty(int studentId, string selectedParty)
        {
            _teacherSession.StudentSessions.Find(s => s.Id == studentId).SelectedParty = selectedParty;
            Update();
        }

        public void AnswerStatement(string argument, int studentId, int answeroptionId)
        {
            var session = _teacherSession.StudentSessions.Find(s => s.Id == studentId);
            var statements = _teacherSession.ChosenStatements.OrderBy(s => s.Statement.Id).ToList();
            var statementId = statements[session.LastAnsweredStatement].Statement.Id;
            _sessionManager.AnswerStatement(answeroptionId, studentId, argument, statementId);
            session.LastAnsweredStatement++;
            Update();
        }

        public string CalculateScore(int studentId)
        {
            var student = _teacherSession.StudentSessions.Find(s => s.Id == studentId);
            var selectedParty = student.SelectedParty;

            var timeOut = 0;
            do
            {
                Thread.Sleep(200);
                student = _teacherSession.StudentSessions.Find(s => s.Id == studentId);
                timeOut++;
                _teacherSession = _sessionManager.GetTeacherSession(_sessionCode);
            } while (student.Answers.Count != _teacherSession.ChosenStatements.Count && timeOut < 15);

            string result;
            if (student.Answers.Count == _teacherSession.ChosenStatements.Count && student.SelectedParty != null)
            {
                var partyAnswers = _sessionManager
                    .GetPartyAnswersForChosenStatements(_teacherSession.SessionCode, selectedParty).ToList();
                ;

                var total = student.Answers.Count;
                var score = 0;
                for (var i = 0; i < total; i++)
                    if (student.Answers[i].ChosenAnswer.Id == 3)
                    {
                        total--;
                    }
                    else
                    {
                        if (student.Answers[i].ChosenAnswer.Id == partyAnswers[i].ChosenAnswer.Id)
                        {
                            score++;
                            student.Answers[i].Correct = true;
                        }
                        else
                        {
                            student.Answers[i].Correct = false;
                        }
                    }

                result = score + "/" + total;
            }
            else
            {
                result = null;
            }

            student.Score = result;
            Update();
            return result;
        }

        public Dictionary<Party, string> CalculateDebateScore(int studentId)
        {
            var percentages = new Dictionary<Party, string>();
            var parties = _sessionManager.GetChosenParties(_teacherSession.SessionCode).ToList();
            var answers = GetAnswers(studentId);
            var answerCount = answers.Count;
            foreach (var party in parties)
            {
                var correctAnswer = 0;
                var partyAnswers = _partyManager.GetPartyAnswers(party.Name);
                for (var i = 0; i < answers.Count; i++)
                    if (answers[i].ChosenAnswer.Id == partyAnswers[i].ChosenAnswer.Id)
                        correctAnswer++;
                var precentage = correctAnswer * 100 / answerCount + "%";
                percentages.Add(party, precentage);
            }

            percentages = percentages.OrderByDescending(x => int.Parse(x.Value.Trim('%')))
                .ToDictionary(x => x.Key, x => x.Value);
            var student = _teacherSession.StudentSessions.Find(s => s.Id == studentId);
            var result = "";
            for (var i = 0; i < parties.Count; i++)
                result += percentages.ElementAt(i).Key.Name + ": " + percentages.ElementAt(i).Value + "\t";

            student.Score = result;
            Update();
            return percentages;
        }

        public string CalculateCustomPartyGameScore(int studentId)
        {
            var score = 0;
            var correctOptions = _sessionManager.GetCorrectAOForChosenStatements(_teacherSession.SessionCode).ToList();
            var lls = _teacherSession.StudentSessions.Find(s => s.Id == studentId);

            for (var i = 0; i < lls.Answers.Count; i++)
                if (lls.Answers[i].ChosenAnswer.Id == correctOptions[i].Id)
                {
                    score++;
                    lls.Answers[i].Correct = true;
                }

            var result = score + "/" + lls.Answers.Count;
            lls.Score = result;
            Update();
            return result;
        }

        public List<Answer> GetAnswers(int studentId)
        {
            var student = _teacherSession.StudentSessions.Find(s => s.Id == studentId);
            return student.Answers;
        }

        public int GetMaxStudents()
        {
            return _teacherSession.MaxAmountStudents;
        }

        public int GetCurrentStudents()
        {
            return _teacherSession.StudentSessions.Count;
        }

        public int CurrentStudentAnswers(int statementIndex)
        {
            return _teacherSession.StudentSessions.FindAll(s => s.Answers.Count == statementIndex + 1).Count;
        }

        public List<int> GetDistribution(List<Answer> answers, List<AnswerOption> answerOptions)
        {
            var distribution = new List<int>();
            for (var i = 0; i < answerOptions.Count; i++) distribution.Add(0);
            foreach (var answer in answers)
                for (var i = 0; i < answerOptions.Count; i++)
                    if (answerOptions[i].Id.Equals(answer.ChosenAnswer.Id))
                        distribution[i]++;
            return distribution;
        }

        public bool GetAllowSkip()
        {
            return _teacherSession.Settings.SkipAllowed;
        }

        public bool GetAllowArguments()
        {
            return _teacherSession.Settings.ArgumentsAllowed;
        }

        public bool GetAllowedDefinitions()
        {
            return _teacherSession.Settings.DefinitionsGiven;
        }

        public bool GetForceWaiting()
        {
            return _teacherSession.Settings.ForceWaiting;
        }

        public List<string> GetColours()
        {
            var colours = new List<string>();
            colours.Add(_teacherSession.Settings.Colour1);
            colours.Add(_teacherSession.Settings.Colour2);
            colours.Add(_teacherSession.Settings.ColourSkip);
            colours.Add(_teacherSession.Settings.Colour3);
            colours.Add(_teacherSession.Settings.Colour4);
            colours.Add(_teacherSession.Settings.Colour5);
            colours.Add(_teacherSession.Settings.Colour6);

            return colours;
        }

        public void SetCurrentSatement(int current)
        {
            _teacherSession.CurrentStatement = current;
            Update();
        }

        public void SetStartDate()
        {
            _teacherSession.Date = DateTime.Now;
            Update();
        }

        public void SetChosenStatements()
        {
            _teacherSession.ChosenStatements = new List<ChosenStatement>();
            var statements = _teacherSession.Test.Statements;
            for (var i = 0; i < statements.Count; i++)
            {
                var statement = new ChosenStatement();
                statement.Statement = statements[i];
                _teacherSession.ChosenStatements.Add(statement);
            }

            Update();
        }

        private void Update()
        {
            _sessionManager.UpdateLeerkrachtSessie(_teacherSession);
        }

        public List<Answer> GetAllAnswers()
        {
            var answers = new List<Answer>();

            foreach (var sts in _teacherSession.StudentSessions)
                if (sts.Answers.Count == _teacherSession.ChosenStatements.Count)
                    answers.AddRange(sts.Answers);

            return answers;
        }
    }
}