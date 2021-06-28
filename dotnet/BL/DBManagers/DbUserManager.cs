using System.Collections.Generic;
using BL.Domain.Identity;
using BL.Domain.Sessie;
using DAL.MySQL.MySQLRepositories;

namespace BL.DBManagers
{
    public class DbUserManager
    {
        private readonly MySQLUserRepository _repo;

        public DbUserManager(MySQLUserRepository mySqlUserRepository)
        {
            _repo = mySqlUserRepository;
        }

        public int GetAmountOfSessionsFromOrganisation(int organisationId)
        {
            return _repo.GetAmountOfSessionsFromOrganisation(organisationId);
        }

        public IEnumerable<Teacher> GetTeachersFromOrg(int organisationId)
        {
            return _repo.GetTeachersFromOrg(organisationId);
        }

        public Admin GetAdminFromOrg(int organisationId)
        {
            return _repo.GetAdminFromOrg(organisationId);
        }

        public IEnumerable<Class> GetAllClassesFromSchool(int organisationId)
        {
            return _repo.GetClassesFromSchool(organisationId);
        }

        public IEnumerable<Organisation> GetAllOrganisations()
        {
            return _repo.GetAllOrganisations();
        }

        public IEnumerable<Organisation> GetOrganisationsWithName(string name)
        {
            return _repo.GetOrganisationsWithName(name);
        }

        public IEnumerable<Class> GetClasses(string userId)
        {
            return _repo.GetAllClasses(userId);
        }

        public Class GetClass(string userId, string className)
        {
            return _repo.GetClass(userId, className);
        }

        public User GetUser(string userId)
        {
            return _repo.GetUser(userId);
        }

        public bool AddUser(User user, Organisation organisation)
        {
            return _repo.AddUser(user, organisation);
        }

        public User GetUser(int adminId)
        {
            return _repo.GetUser(adminId);
        }

        public bool DeleteTeacher(string id)
        {
            return _repo.DeleteTeacher(id);
        }

        public User UpdateUser(User user)
        {
            return _repo.UpdateUser(user);
        }

        public IEnumerable<ApplicationUser> GetApplicationUsersFromOrg(int organisationId)
        {
            return _repo.GetApplicationUsersFromOrg(organisationId);
        }


        public void DeleteClass(int classId)
        {
            _repo.DeleteClass(classId);
        }

        public void CreateClass(int teacherId, string name, int amountOfStudents, string year)
        {
            _repo.CreateClass(teacherId, name, amountOfStudents, year);
        }

        public void ModifyClass(int classId, string name, int amountOfStudents, string year)
        {
            _repo.ModifyClass(classId, name, amountOfStudents, year);
        }

        public Class GetClass(int classId)
        {
            return _repo.GetClass(classId);
        }

        public IEnumerable<TeacherSession> GetEndedTeacherSessions(string userId)
        {
            return _repo.GetEndedTeacherSessions(userId);
        }

        public IEnumerable<TeacherSession> GetNotStartedTeacherSessions(string userId)
        {
            return _repo.GetNotStartedTeacherSessions(userId);
        }
    }
}