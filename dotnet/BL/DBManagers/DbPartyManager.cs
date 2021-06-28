using System.Collections.Generic;
using BL.Domain.Test;
using DAL.MySQL.MySQLRepositories;

namespace BL.DBManagers
{
    public class DbPartyManager
    {
        private readonly MySQLPartyRepository _repo;

        public DbPartyManager(MySQLPartyRepository mySqlPartyRepository)
        {
            _repo = mySqlPartyRepository;
        }

        public Party GetParty(string partyName)
        {
            return _repo.GetParty(partyName);
        }

        public List<Answer> GetPartyAnswers(string partyName)
        {
            return _repo.GetPartyAnswers(partyName);
        }

        public IEnumerable<Party> GetAllParties()
        {
            return _repo.GetAllParties();
        }

        public string CreateParty(Party party)
        {
            return _repo.CreateParty(party);
        }

        public void UpdateParty(Party party)
        {
            _repo.UpdateParty(party);
        }

        public void DeleteParty(string partyName)
        {
            _repo.DeleteParty(partyName);
        }

        public Answer GetAnswerByStatement(string partyName, int statementId)
        {
            return _repo.GetAnswerByStatement(partyName, statementId);
        }

        public void UpdateAnswer(Answer answer)
        {
            _repo.UpdateAnswer(answer);
        }
    }
}