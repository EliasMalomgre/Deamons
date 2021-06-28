using System.Collections.Generic;
using System.Linq;
using BL.DBManagers;
using BL.Domain.Identity;
using BL.Domain.Test;

namespace BL
{
    public class TestManager
    {
        private readonly DbTestManager _dbTestManager;
        private DbUserManager _dbUserManager;

        public TestManager(DbTestManager dbTestManager, DbUserManager dbUserManager)
        {
            _dbTestManager = dbTestManager;
            _dbUserManager = dbUserManager;
        }

        public int MakeTest(string name, User user)
        {
            var test = new Test();
            test.Title = name;
            test.Maker = user;

            var existingTests = _dbTestManager.GetTestsFromUser(user.UserId);
            if (existingTests.FirstOrDefault(t => t.Title == name) != null)
                return -1; //test bestaat al
            return _dbTestManager.CreateTest(test);
        }

        public int MakeStatement(int testId, string text, string explanation)
        {
            var statement = new Statement();
            statement.AnswerOptions = new List<StatementAnswerOption>();
            statement.Definitions = new List<Definition>();

            statement.Text = text;
            statement.Explanation = explanation;

            return _dbTestManager.AddStatement(testId, statement);
        }

        public void AddAnswerOption(int statementId, string text, bool isCorrect = false)
        {
            var AoId = _dbTestManager.FindOrCreateAnswerOption(text);
            _dbTestManager.AddAnswerOption(statementId, AoId, isCorrect);
        }

        public void AddTag(int testId, string tag)
        {
            var tagId = _dbTestManager.FindOrCreateTag(tag);
            _dbTestManager.AddTag(testId, tagId);
        }
    }
}