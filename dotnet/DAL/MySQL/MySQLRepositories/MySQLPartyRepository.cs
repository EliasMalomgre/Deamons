using System;
using System.Collections.Generic;
using System.Linq;
using BL.Domain.Test;
using Microsoft.EntityFrameworkCore;

namespace DAL.MySQL.MySQLRepositories
{
    public class MySQLPartyRepository
    {
        private readonly StemtestDbContext ctx;

        public MySQLPartyRepository(StemtestDbContext stemtestDbContext)
        {
            ctx = stemtestDbContext;
        }

        public Party GetParty(string name)
        {
            return ctx.Parties
                .Include(p => p.Answers)
                .ThenInclude(o => o.ChosenAnswer)
                .Include(p => p.Answers)
                .ThenInclude(p => p.Statement)
                .FirstOrDefault(p => p.Name == name);
        }

        public IEnumerable<Party> GetAllParties()
        {
            return ctx.Parties.AsEnumerable();
        }

        public List<Answer> GetPartyAnswers(string partij)
        {
            var p = GetParty(partij);
            return p.Answers;
        }

        public string CreateParty(Party party)
        {
            var test = ctx.Tests.Include(t => t.Statements).FirstOrDefault(t => t.Id == 1);
            ctx.Parties.Add(party);

            foreach (var statement in test.Statements)
            {
                var answer = new Answer();
                answer.ChosenAnswer = ctx.AnswerOptions.Find(1);
                answer.Statement = statement;
                party.Answers.Add(answer);
            }

            ctx.SaveChanges();
            return party.Name;
        }

        public void UpdateParty(Party party)
        {
            ctx.Parties.Update(party);
            ctx.SaveChanges();
        }

        public void DeleteParty(string partyName)
        {
            var p = ctx.Parties.Include(p => p.Answers)
                .Include(p => p.TeacherSessions)
                .FirstOrDefault(p => p.Name.Equals(partyName));
            if (p == null)
            {
                throw new Exception("Data could not be retrieved for the database!");
            }

            foreach (var answer in p.Answers) ctx.Answers.Remove(answer);

            foreach (var pts in p.TeacherSessions) ctx.Remove(pts);

            ctx.Remove(p);
            ctx.SaveChanges();
        }

        public Answer GetAnswerByStatement(string partyName, int statementId)
        {
            var partyAnswers = GetPartyAnswers(partyName);
            return partyAnswers.Find(p => p.Statement.Id == statementId);
        }

        public void UpdateAnswer(Answer answer)
        {
            ctx.Answers.Update(answer);
            ctx.SaveChanges();
        }
    }
}