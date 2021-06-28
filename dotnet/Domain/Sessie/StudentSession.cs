using System.Collections.Generic;
using BL.Domain.Test;

namespace BL.Domain.Sessie
{
    public class StudentSession
    {
        public StudentSession()
        {
            Answers = new List<Answer>();
        }

        public int Id { get; set; }
        public string Score { get; set; }
        public int LastAnsweredStatement { get; set; }
        public List<Answer> Answers { get; set; }
        public string SelectedParty { get; set; }
    }
}