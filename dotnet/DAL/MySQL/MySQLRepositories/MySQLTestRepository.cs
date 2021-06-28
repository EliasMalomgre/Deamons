using System;
using System.Collections.Generic;
using System.Linq;
using BL.Domain.Identity;
using BL.Domain.Sessie;
using BL.Domain.Test;
using Microsoft.EntityFrameworkCore;

namespace DAL.MySQL.MySQLRepositories
{
    public class MySQLTestRepository
    {
        private readonly StemtestDbContext ctx;

        public MySQLTestRepository(StemtestDbContext stemtestDbContext)
        {
            ctx = stemtestDbContext;
        }


        public IEnumerable<Test> GetAllTests(int userId)
        {
            //waarschuwing: deze methode geeft testen terug zonder inhoud
            //purpose: overzicht geven van testen
            return ctx.Tests.Where(t => t.Maker.UserId == userId)
                .Include(e => e.Maker)
                .Include(e => e.Tags);
        }

        public Test GetTest(int id)
        {
            return ctx.Tests
                .Include(e => e.Statements)
                .ThenInclude(e => e.AnswerOptions)
                .Include(e => e.Statements)
                .ThenInclude(e => e.Definitions)
                .Include(e => e.Maker)
                .Include(t => t.Tags)
                .First(e => e.Id == id);
        }

        public int CreateTest(Test test)
        {
            ctx.Users.Attach(test.Maker.ApplicationUser);
            ctx.Organisations.Attach(test.Maker.Organisation);
            ctx.DataUsers.Attach(test.Maker);
            ctx.Tests.Add(test);
            ctx.SaveChanges();
            return test.Id;
        }

        public void CopyTestToUser(int testId, User teacher)
        {
            var test = ctx.Tests
                .AsNoTracking()
                .Include(t => t.Statements).ThenInclude(s => s.Definitions)
                .Include(t => t.Statements).ThenInclude(s => s.AnswerOptions).ThenInclude(sao => sao.AnswerOption)
                .Include(t => t.Tags).ThenInclude(t => t.Tag)
                .First(t => t.Id == testId);

            var definitions = ctx.Definitions.AsNoTracking().Where(d => d.Test.Id == test.Id).ToList();


            var testCopy = new Test();
            testCopy.Tags = new List<TestTag>();
            testCopy.Statements = new List<Statement>();
            testCopy.Maker = teacher;
            testCopy.Title = test.Title;

            var definitionsCopy = new List<Definition>();

            foreach (var definition in definitions)
                definitionsCopy.Add(new Definition
                {
                    Explanation = definition.Explanation,
                    Test = testCopy,
                    Word = definition.Word
                });

            var statementAnswerOptions = new List<StatementAnswerOption>();
            foreach (var statement in test.Statements)
            {
                var copyStatement = new Statement();
                copyStatement.Explanation = statement.Explanation;
                copyStatement.Text = statement.Text;
                copyStatement.AnswerOptions = new List<StatementAnswerOption>();
                foreach (var sao in statement.AnswerOptions)
                    statementAnswerOptions.Add(new StatementAnswerOption
                    {
                        AnswerOption = sao.AnswerOption,
                        Statement = statement
                    });

                testCopy.Statements.Add(copyStatement);
            }

            testCopy = ctx.Tests.Add(testCopy).Entity;
            ctx.Statements.AddRange(testCopy.Statements);
            ctx.Definitions.AddRange(definitionsCopy);
            ctx.SaveChanges();
            foreach (var statement in testCopy.Statements)
            foreach (var sao in statementAnswerOptions)
                if (sao.Statement.Text == statement.Text)
                    AddAnswerOption(statement.Id, sao.AnswerOption.Id);
            foreach (var tag in test.Tags) AddTag(testCopy.Id, tag.Tag.Id);
        }

        public void CreateSharedTest(SharedTest sharedTest)
        {
            var test = ctx.Tests
                .AsNoTracking()
                .Include(t => t.Statements).ThenInclude(s => s.Definitions)
                .Include(t => t.Statements).ThenInclude(s => s.AnswerOptions).ThenInclude(sao => sao.AnswerOption)
                .Include(t => t.Tags).ThenInclude(t => t.Tag)
                .First(t => t.Id == sharedTest.Test.Id);

            var definitions = ctx.Definitions.AsNoTracking().Where(d => d.Test.Id == test.Id).ToList();


            var testCopy = new Test();
            testCopy.Tags = new List<TestTag>();
            testCopy.Statements = new List<Statement>();
            testCopy.Maker = null;
            testCopy.Title = test.Title;

            var definitionsCopy = new List<Definition>();

            foreach (var definition in definitions)
                definitionsCopy.Add(new Definition
                {
                    Explanation = definition.Explanation,
                    Test = testCopy,
                    Word = definition.Word
                });
            var statementAnswerOptions = new List<StatementAnswerOption>();
            foreach (var statement in test.Statements)
            {
                var copyStatement = new Statement();
                copyStatement.Explanation = statement.Explanation;
                copyStatement.Text = statement.Text;
                copyStatement.AnswerOptions = new List<StatementAnswerOption>();

                foreach (var sao in statement.AnswerOptions.ToList())
                    statementAnswerOptions.Add(new StatementAnswerOption
                    {
                        AnswerOption = sao.AnswerOption,
                        Statement = statement
                    });
                testCopy.Statements.Add(copyStatement);
            }

            sharedTest.Test = testCopy;

            ctx.SharedTests.Add(sharedTest);
            testCopy = ctx.Tests.Add(sharedTest.Test).Entity;
            ctx.Statements.AddRange(sharedTest.Test.Statements);
            ctx.Definitions.AddRange(definitionsCopy);
            ctx.SaveChanges();
            testCopy = ctx.Tests.Include(t => t.Statements).ThenInclude(s => s.AnswerOptions)
                .First(t => t.Id == testCopy.Id);

            foreach (var statement in testCopy.Statements)
            foreach (var sao in statementAnswerOptions)
                if (sao.Statement.Text == statement.Text)
                    AddAnswerOption(statement.Id, sao.AnswerOption.Id);
            foreach (var tag in test.Tags) AddTag(testCopy.Id, tag.Tag.Id);
        }

        public SharedTest GetSharedTest(int stestId)
        {
            return ctx.SharedTests.Include(st => st.Creator)
                .FirstOrDefault(st => st.Id == stestId);
        }

        public IEnumerable<Test> GetTestsWithTag(Tag tag)
        {
            var tests = ctx.Tests
                .Include(e => e.Tags)
                .Include(e => e.Maker).ToList();
            var goodTests = new List<Test>();

            foreach (var test in tests)
            foreach (var testTag in test.Tags)
                if (testTag.Tag == tag)
                    goodTests.Add(test);
            return goodTests;
        }

        public Tag GetTag(int id)
        {
            return ctx.Tags.Find(id);
        }

        public IEnumerable<Tag> GetAllTags()
        {
            return ctx.Tags.Distinct();
        }

        public void UpdateTag(int id, string text)
        {
            var tag = ctx.Tags.Find(id);
            if (tag == null)
            {
                throw new Exception("Data could not be retrieved for the database!");
            }

            tag.Name = text;
            ctx.Tags.Update(tag);
            ctx.SaveChanges();
        }

        public int CreateNewTag(string text)
        {
            var tag = new Tag();
            tag.Name = text;
            ctx.Tags.Add(tag);
            ctx.SaveChanges();
            return tag.Id;
        }

        public void DeleteTag(int id)
        {
            var tag = ctx.Tags
                .Include(t => t.Tests)
                .FirstOrDefault(t => t.Id == id);
            if (tag == null)
            {
                throw new Exception("Data could not be retrieved for the database!");
            }

            foreach (var tt in tag.Tests) ctx.Remove(tt);

            ctx.Tags.Remove(tag);
            ctx.SaveChanges();
        }

        public Statement GetStatements(int stellingId, bool includeAo = false)
        {
            if (includeAo)
                return ctx.Statements.Include(s => s.Definitions)
                    .Include(s => s.AnswerOptions)
                    .ThenInclude(ao => ao.AnswerOption)
                    .FirstOrDefault(s => s.Id == stellingId);
            return ctx.Statements.Include(s => s.Definitions).FirstOrDefault(s => s.Id == stellingId);
        }

        public IEnumerable<AnswerOption> GetAnswerOptions(int statementId)
        {
            var statement = ctx.Statements.Include(s => s.AnswerOptions).ThenInclude(sao => sao.AnswerOption)
                .ThenInclude(ao => ao.Opinion).FirstOrDefault(s => s.Id == statementId);
            if (statement == null) return null;

            var aws = new List<AnswerOption>();
            foreach (var mogelijkheid in statement.AnswerOptions) aws.Add(mogelijkheid.AnswerOption);

            return aws.AsEnumerable();
        }

        public AnswerOption GetAnswerOption(int id)
        {
            return ctx.AnswerOptions.FirstOrDefault(ao => ao.Id == id);
        }

        public int AddStatement(int testId, Statement statement)
        {
            var test = ctx.Tests.Include(t => t.Statements).First(t => t.Id == testId);
            test.Statements.Add(statement);
            ctx.Update(test);
            ctx.SaveChanges();
            return statement.Id;
        }

        public int FindOrCreateAnswerOption(string opinion)
        {
            var ao = ctx.AnswerOptions.FirstOrDefault(aw => aw.Opinion.Equals(opinion));
            if (ao != null) return ao.Id;

            ao = new AnswerOption();
            ao.Opinion = opinion;
            ctx.AnswerOptions.Add(ao);
            ctx.SaveChanges();
            return ao.Id;
        }

        public void AddAnswerOption(int statementId, int answerOptionId, bool isCorrect = false)
        {
            var statement = ctx.Statements.Include(s => s.AnswerOptions).FirstOrDefault(s => s.Id == statementId);
            if (statement.AnswerOptions == null) statement.AnswerOptions = new List<StatementAnswerOption>();
            var answerOption = ctx.AnswerOptions.Find(answerOptionId);
            if (statement == null || answerOption == null)
            {
                throw new Exception("Data could not be retrieved for the database!");
            }

            var sao = new StatementAnswerOption();
            sao.Statement = statement;
            sao.AnswerOption = answerOption;
            sao.isCorrectAnswer = isCorrect;
            statement.AnswerOptions.Add(sao);
            ctx.Statements.Update(statement);
            ctx.SaveChanges();
        }

        public void AddDefaultAnswerOptions(int statementId, string correctAnswer)
        {
            var statement = ctx.Statements.Include(s => s.AnswerOptions).ThenInclude(ao => ao.AnswerOption)
                .FirstOrDefault(s => s.Id == statementId);
            if (statement == null)
            {
                throw new Exception("Data could not be retrieved for the database!");
            }

            var correct = 1;
            if (correctAnswer.Equals("Niet akkoord")) correct = 2;

            for (var i = 1; i <= 3; i++)
            {
                var sao = new StatementAnswerOption();
                sao.AnswerOption = GetAnswerOption(i);
                sao.Statement = statement;
                if (i == correct) sao.isCorrectAnswer = true;
                statement.AnswerOptions.Add(sao);
            }

            ctx.Statements.Update(statement);
            ctx.SaveChanges();
        }

        public int FindOrCreateTag(string tagText)
        {
            var tag = ctx.Tags.FirstOrDefault(t => t.Name.Equals(tagText));
            if (tag != null) return tag.Id;

            tag = new Tag();
            tag.Name = tagText;
            ctx.Tags.Add(tag);
            ctx.SaveChanges();
            return tag.Id;
        }

        public void AddTag(int testId, int tagId)
        {
            var test = GetTest(testId);
            var tag = ctx.Tags.Find(tagId);

            var tt = new TestTag();
            tt.Test = test;
            tt.Tag = tag;
            test.Tags.Add(tt);
            ctx.Tests.Update(test);
            ctx.SaveChanges();
        }

        public int AddStudentSession(int teacherSessionId, StudentSession studentSession)
        {
            var ts = ctx.TeacherSessions.Find(teacherSessionId);
            if (ts.StudentSessions == null) ts.StudentSessions = new List<StudentSession>();
            ts.StudentSessions.Add(studentSession);
            ctx.TeacherSessions.Update(ts);
            ctx.SaveChanges();
            return studentSession.Id;
        }

        public void DeleteTest(int testId)
        {
            var test = GetTest(testId);
            Console.WriteLine(test.Title);

            var definitions = ctx.Definitions.Where(d => d.Test.Id == testId);
            foreach (var def in definitions) ctx.Remove(def);
            foreach (var statement in test.Statements)
            {
                var stat = ctx.Statements.Find(statement.Id);

                foreach (var stao in stat.AnswerOptions) ctx.Remove(stao);
                ctx.Remove(statement);
            }

            foreach (var tt in test.Tags) ctx.Remove(tt);

            var tsList = ctx.TeacherSessions
                .Include(t => t.StudentSessions)
                .ThenInclude(s => s.Answers)
                .Include(ts => ts.ChosenStatements)
                .Where(ts => ts.Test.Id == testId).ToList();
            foreach (var ts in tsList)
            {
                foreach (var sts in ts.StudentSessions)
                {
                    foreach (var answer in sts.Answers) ctx.Remove(answer);
                    ctx.Remove(sts);
                }

                foreach (var cs in ts.ChosenStatements) ctx.Remove(cs);

                ctx.Remove(ts);
            }

            ctx.Remove(test);
            ctx.SaveChanges();
        }

        public IEnumerable<Statement> GetAllStatementsFromTest(int testId)
        {
            var test = ctx.Tests.Include(t => t.Statements).FirstOrDefault(t => t.Id == testId);
            return test.Statements;
        }

        public Answer GetAnswer(int answerId)
        {
            return ctx.Answers.Find(answerId);
        }

        public IEnumerable<Tag> GetTagsFromTest(int testId)
        {
            var test = ctx.Tests.Include(t => t.Tags)
                .ThenInclude(tt => tt.Tag)
                .FirstOrDefault(t => t.Id == testId);
            if (test == null)
            {
                throw new Exception("Data could not be retrieved for the database!");
            }

            var tags = new List<Tag>();
            foreach (var tt in test.Tags) tags.Add(tt.Tag);

            return tags;
        }

        public void DeleteTagFromTest(int testId, int tagId)
        {
            var test = ctx.Tests
                .Include(t => t.Tags)
                .ThenInclude(tt => tt.Tag)
                .FirstOrDefault(t => t.Id == testId);
            if (test == null)
            {
                throw new Exception("Data could not be retrieved for the database!");
            }

            foreach (var tt in test.Tags)
                if (tt.Tag.Id == tagId)
                    ctx.Remove(tt);

            ctx.SaveChanges();
        }

        //STATEMENTS
        public void DeleteStatement(int statementId, int testId)
        {
            var test = ctx.Tests
                .Include(t => t.Statements)
                .FirstOrDefault(t => t.Id == testId);
            if (test == null)
            {
                throw new Exception("Data could not be retrieved for the database!");
            }

            var chosenStatements = ctx.ChosenStatements.Where(cs => cs.Statement.Id == statementId);
            foreach (var cs in chosenStatements) ctx.Remove(cs);

            test.Statements.Remove(ctx.Statements.Find(statementId));
            ctx.Update(test);
            ctx.Statements.Remove(ctx.Statements.Find(statementId));
            ctx.SaveChanges();
        }

        public void DeleteAnswerOptions(int statementId)
        {
            var statement = ctx.Statements.Include(s => s.AnswerOptions)
                .FirstOrDefault(s => s.Id == statementId);
            if (statement == null)
            {
                throw new Exception("Data could not be retrieved for the database!");
            }

            foreach (var sao in statement.AnswerOptions) ctx.Remove(sao);

            ctx.SaveChanges();
        }

        public void UpdateStatement(Statement statement)
        {
            ctx.Statements.Update(statement);
            ctx.SaveChanges();
        }


        public void ReSeedDatabase()
        {
            ctx.ReSeedDatabase();
        }

        public IEnumerable<SharedTest> GetSharedTestsForUser(User teacher, string? searchString, string? chk1)
        {
            IEnumerable<SharedTest> sharedTests;
            //get all shared tests for user
            sharedTests = ctx.SharedTests.Where(st => st.PublicShared || st.Organisation.Id == teacher.Organisation.Id)
                .Include(st => st.Creator)
                .Include(st => st.Test)
                .Include(st => st.Organisation);

            //filter on test name if there is a search string
            if (searchString != null)
                sharedTests = sharedTests.Where(st => st.Test.Title.ToLower().Contains(searchString.ToLower()))
                    .ToList();
            //If only school tests checked
            if (chk1 != null)
                sharedTests = sharedTests.Where(st => st.Organisation.Id == teacher.Organisation.Id).ToList();

            return sharedTests.OrderBy(st => st.Test.Title);
        }

        private T DetachEntity<T>(T entity, StemtestDbContext db) where T : class
        {
            db.Entry(entity).State = EntityState.Detached;
            if (entity.GetType().GetProperty("Id") != null) entity.GetType().GetProperty("Id").SetValue(entity, 0);
            return entity;
        }

        private List<T> DetachEntities<T>(List<T> entities, StemtestDbContext db) where T : class
        {
            foreach (var entity in entities) DetachEntity(entity, db);
            return entities;
        }

        public void DeleteSharedTest(int stestId)
        {
            var sharedTest = ctx.SharedTests.Include(st => st.Test).FirstOrDefault(st => st.Id == stestId);
            ctx.SharedTests.Remove(sharedTest);
            DeleteTest(sharedTest.Test.Id);
        }

        public void AddDefinition(int testId, string word, string explanation)
        {
            var test = ctx.Tests.FirstOrDefault(t => t.Id == testId);
            var definition = new Definition();
            definition.Word = word;
            definition.Explanation = explanation;
            definition.Test = test;
            ctx.Definitions.Add(definition);
            ctx.SaveChanges();
        }

        public IEnumerable<Definition> GetDefinitions(int testId)
        {
            return ctx.Definitions.Include(d => d.Test).Where(d => d.Test.Id == testId);
        }

        public void DeleteDefinition(int id)
        {
            var definition = ctx.Definitions.FirstOrDefault(d => d.Id == id);
            if (definition==null)
            {
                throw new Exception("Data could not be retrieved for the database!");
            }
            ctx.Remove(definition);
            ctx.SaveChanges();
        }

        public Test GetStemTest()
        {
            return ctx.Tests.FirstOrDefault(t => t.Id == 1);
        }
    }
}