using System.Collections.Generic;
using BL.Domain.Identity;
using BL.Domain.Sessie;
using BL.Domain.Test;
using DAL.MySQL.MySQLRepositories;

namespace BL.DBManagers
{
    public class DbTestManager
    {
        private readonly MySQLTestRepository _repo;

        public DbTestManager(MySQLTestRepository mySqlTestRepository)
        {
            _repo = mySqlTestRepository;
        }

        public void CreateSharedTest(SharedTest sharedTest)
        {
            _repo.CreateSharedTest(sharedTest);
        }

        public IEnumerable<Test> GetTestsFromUser(int userId)
        {
            return _repo.GetAllTests(userId);
        }

        public void CopyTestToUser(int userId, User teacher)
        {
            _repo.CopyTestToUser(userId, teacher);
        }

        public SharedTest GetSharedTest(int stestId)
        {
            return _repo.GetSharedTest(stestId);
        }

        public void DeleteSharedTest(int stestId)
        {
            _repo.DeleteSharedTest(stestId);
        }

        public int CreateTest(Test test)
        {
            return _repo.CreateTest(test);
        }

        public Test GetTest(int testId)
        {
            return _repo.GetTest(testId);
        }

        public IEnumerable<Test> GetTestsWithTag(Tag tag)
        {
            return _repo.GetTestsWithTag(tag);
        }

        public IEnumerable<Tag> GetAllTags()
        {
            return _repo.GetAllTags();
        }

        public Statement GetStatement(int stellingId, bool includeAo = false)
        {
            return _repo.GetStatements(stellingId, includeAo);
        }

        public int AddStatement(int testId, Statement statement)
        {
            return _repo.AddStatement(testId, statement);
        }

        public int FindOrCreateAnswerOption(string opinion)
        {
            return _repo.FindOrCreateAnswerOption(opinion);
        }

        public void AddAnswerOption(int statementId, int answerOptionId, bool isCorrect = false)
        {
            _repo.AddAnswerOption(statementId, answerOptionId, isCorrect);
        }

        public int FindOrCreateTag(string tagText)
        {
            return _repo.FindOrCreateTag(tagText);
        }

        public void AddTag(int testId, int tagId)
        {
            _repo.AddTag(testId, tagId);
        }

        public AnswerOption GetAnswerOption(int id)
        {
            return _repo.GetAnswerOption(id);
        }

        public void AddDefaultAnswerOptions(int statementId, string correctAnswer)
        {
            _repo.AddDefaultAnswerOptions(statementId, correctAnswer);
        }

        public int AddStudentSession(int teacherSessionId, StudentSession studentSession)
        {
            return _repo.AddStudentSession(teacherSessionId, studentSession);
        }

        public void DeleteTest(int testId)
        {
            _repo.DeleteTest(testId);
        }

        public IEnumerable<Statement> GetAllStatementsFromTest(int testId)
        {
            return _repo.GetAllStatementsFromTest(testId);
        }

        public Answer GetAnswer(int answerId)
        {
            return _repo.GetAnswer(answerId);
        }

        public int CreateNewTag(string text)
        {
            return _repo.CreateNewTag(text);
        }

        public Tag GetTag(int id)
        {
            return _repo.GetTag(id);
        }

        public void UpdateTag(int id, string text)
        {
            _repo.UpdateTag(id, text);
        }

        public void DeleteTag(int id)
        {
            _repo.DeleteTag(id);
        }

        public IEnumerable<Tag> GetTagsFromTest(int testId)
        {
            return _repo.GetTagsFromTest(testId);
        }

        public void DeleteTagFromTest(int testId, int tagId)
        {
            _repo.DeleteTagFromTest(testId, tagId);
        }

        public void DeleteStatement(int statementId, int testId)
        {
            _repo.DeleteStatement(statementId, testId);
        }

        public void DeleteAnswerOptions(int statementId)
        {
            _repo.DeleteAnswerOptions(statementId);
        }

        public void UpdateStatement(Statement statement)
        {
            _repo.UpdateStatement(statement);
        }

        public void ReSeedDatabase()
        {
            _repo.ReSeedDatabase();
        }

        public IEnumerable<SharedTest> GetSharedTestsForUser(User teacher, string? searchString, string? chk1)
        {
            return _repo.GetSharedTestsForUser(teacher, searchString, chk1);
        }

        public void AddDefinition(int testId, string word, string explanation)
        {
            _repo.AddDefinition(testId, word, explanation);
        }

        public IEnumerable<Definition> GetDefinitions(int testId)
        {
            return _repo.GetDefinitions(testId);
        }

        public void DeleteDefinition(int id)
        {
            _repo.DeleteDefinition(id);
        }

        public Test GetStemTest()
        {
            return _repo.GetStemTest();
        }
    }
}