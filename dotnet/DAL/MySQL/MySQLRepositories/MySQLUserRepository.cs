using System;
using System.Collections.Generic;
using System.Linq;
using BL.Domain.Identity;
using BL.Domain.Sessie;
using BL.Domain.Test;
using Microsoft.EntityFrameworkCore;

namespace DAL.MySQL.MySQLRepositories
{
    public class MySQLUserRepository
    {
        private readonly StemtestDbContext ctx;

        public MySQLUserRepository(StemtestDbContext stemtestDbContext)
        {
            ctx = stemtestDbContext;
        }

        public bool AddUser(User user, Organisation organisation)
        {
            user.Organisation = organisation;
            ctx.DataUsers.Add(user);
            ctx.Organisations.Attach(organisation);
            ctx.Users.Attach(user.ApplicationUser);
            ctx.SaveChanges();
            return true;
        }


        public int GetAmountOfSessionsFromOrganisation(int organisationId)
        {
            IEnumerable<User> teachersInSchool =
                ctx.DataUsers.Where(t => t.Organisation.Id == organisationId);

            var teacherSessions = new List<TeacherSession>();

            foreach (var teacher in teachersInSchool.ToList())
                teacherSessions.AddRange(ctx.TeacherSessions
                    .Where(ts => ts.Teacher.UserId == teacher.UserId && ts.SessionCode == -1).ToList());

            return teacherSessions.Count;
        }

        public IEnumerable<Teacher> GetTeachersFromOrg(int organisationId)
        {
            return ctx.DataUsers.OfType<Teacher>().Where(u => u.Organisation.Id == organisationId)
                .Include(u => u.ApplicationUser).ToList();
        }

        public Admin GetAdminFromOrg(int organisationId)
        {
            return ctx.DataUsers.Include(d => d.Organisation).Where(u => u.Organisation.Id == organisationId)
                .OfType<Admin>().FirstOrDefault();
        }

        public User GetUser(int userId)
        {
            return ctx.DataUsers.Where(u => u.UserId == userId).Include(u => u.Organisation)
                .Include(u => u.ApplicationUser).FirstOrDefault();
        }

        public IEnumerable<Class> GetClassesFromSchool(int organisationId)
        {
            IEnumerable<Teacher> teachers = ctx.DataUsers.Include(u => u.Organisation)
                .Include(u => u.ApplicationUser).OfType<Teacher>()
                .Include(t => t.Classes)
                .Where(t => t.Organisation.Id == organisationId);

            var classes = new List<Class>();
            foreach (var t in teachers)
            foreach (var c in t.Classes)
                classes.Add(c);

            return classes;
        }

        public IEnumerable<Organisation> GetAllOrganisations()
        {
            return ctx.Organisations;
        }

        public IEnumerable<Organisation> GetOrganisationsWithName(string name)
        {
            return ctx.Organisations.Where(o => o.Name.Contains(name));
        }

        public User GetUser(string userId)
        {
            return ctx.DataUsers
                .Include(u => u.Organisation).Include(u => u.ApplicationUser)
                .FirstOrDefault(u => u.ApplicationUserId == userId);
        }

        public IEnumerable<Class> GetAllClasses(string userId)
        {
            var user = ctx.DataUsers.Include(u => u.Classes)
                .FirstOrDefault(u => u.ApplicationUserId == userId);
            if (user == null)
            {
                throw new Exception("Data could not be retrieved for the database!");
            }

            return user.Classes;
        }

        public bool DeleteTeacher(string id)
        {
            var user = GetTeacher(id);
            ctx.Classes.RemoveRange(user.Classes);
            IEnumerable<TeacherSession> sessions = new List<TeacherSession>();
            sessions = ctx.TeacherSessions.Include(ts => ts.StudentSessions)
                .Include(ts => ts.ChosenParties)
                .Include(ts => ts.ChosenStatements)
                .Where(session => session.Teacher.UserId == user.UserId).ToList();
            ctx.RemoveRange(sessions);
            ctx.Classes.RemoveRange(user.Classes);
            IEnumerable<Test> tests = ctx.Tests.Where(t => t.Maker.UserId == user.UserId)
                .Include(t=>t.Tags)
                .Include(t => t.Maker)
                .Include(t => t.Statements).ThenInclude(s => s.Definitions)
                .Include(t => t.Statements).ThenInclude(s => s.AnswerOptions);
            foreach (var test in tests.ToList())
            {
                DeleteTest(test);
            }

            IEnumerable<SharedTest> sharedTests = ctx.SharedTests.Include(st => st.Test).Where(st=>st.Creator==user);
            foreach (var sharedTest in sharedTests)
            {
                ctx.SharedTests.Remove(sharedTest);
                DeleteTest(sharedTest.Test);
            }
            ctx.DataUsers.Remove(user);
            return true;
        }
        private void DeleteTest(Test testinput)
        {
            var test = testinput;
            Console.WriteLine(test.Title);

            var definitions = ctx.Definitions.Where(d => d.Test.Id == test.Id);
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
                .Where(ts => ts.Test.Id == test.Id).ToList();
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
        private void DeleteSharedTest(SharedTest sharedTest)
        {
            ctx.SharedTests.Remove(sharedTest);
            DeleteTest(sharedTest.Test);
        }

        public Teacher GetTeacher(string id)
        {
            return ctx.DataUsers.OfType<Teacher>().Include(d => d.Organisation)
                .Include(d => d.Classes).FirstOrDefault(d => d.ApplicationUserId == id);
        }

        public Class GetClass(string userId, string className)
        {
            var klassen = GetAllClasses(userId);
            return klassen.First(k => k.Name == className);
        }

        public void DeleteClass(int classId)
        {
            var klas = ctx.Classes.Find(classId);
            if (klas == null)
            {
                throw new Exception("Data could not be retrieved for the database!");
            }

            var teachersessions = ctx.TeacherSessions.Include(ts => ts.Class)
                .Include(ts => ts.ChosenStatements)
                .Where(ts => ts.Class.Id == classId);
            foreach (var teacherSession in teachersessions)
            {
                foreach (var chosenStatement in teacherSession.ChosenStatements) ctx.Remove(chosenStatement);

                ctx.Remove(teacherSession);
            }

            ctx.Classes.Remove(klas);
            ctx.SaveChanges();
        }

        public void CreateClass(int teacherId, string name, int amountOfStudents, string year)
        {
            var klas = new Class(name, amountOfStudents);
            klas.Year = year;
            var teacher = ctx.DataUsers.FirstOrDefault(t => t.UserId == teacherId);
            if (teacher.Classes == null) teacher.Classes = new List<Class>();
            teacher.Classes.Add(klas);
            ctx.Update(teacher);
            ctx.SaveChanges();
        }

        public User UpdateUser(User user)
        {
            ctx.DataUsers.Update(user);
            return user;
        }

        public IEnumerable<ApplicationUser> GetApplicationUsersFromOrg(int organisationId)
        {
            return ctx.Users.Where(u => u.User.Organisation.Id == organisationId).ToList();
        }

        public void ModifyClass(int classId, string name, int amountOfStudents, string year)
        {
            var klas = ctx.Classes.Find(classId);
            klas.Year = year;
            klas.Name = name;
            klas.NumberOfStudents = amountOfStudents;
            ctx.Classes.Update(klas);
            ctx.SaveChanges();
        }

        public Class GetClass(int classId)
        {
            return ctx.Classes.Find(classId);
        }

        public IEnumerable<TeacherSession> GetEndedTeacherSessions(string userId)
        {
            IEnumerable<TeacherSession> teacherSessions =
                ctx.TeacherSessions.Include(t=>t.Test).Include(c=>c.Class)
                    .Include(s=>s.StudentSessions)
                    .Where(t => t.Teacher.ApplicationUser.Id.Equals(userId)&&t.SessionCode==-1);
            if (teacherSessions == null)
            {
                throw new Exception("Data could not be retrieved for the database!");
            }
            return teacherSessions;
        }

        public IEnumerable<TeacherSession> GetNotStartedTeacherSessions(string userId)
        {
            IEnumerable<TeacherSession> teacherSessions =
                ctx.TeacherSessions.Include(t=>t.Test).Include(c=>c.Class)
                    .Include(s=>s.StudentSessions)
                    .Where(t => t.Teacher.ApplicationUser.Id.Equals(userId)&&t.Date == DateTime.MinValue);
            if (teacherSessions == null)
            {
                throw new Exception("Data could not be retrieved for the database!");
            }

            return teacherSessions;
        }
    }
}