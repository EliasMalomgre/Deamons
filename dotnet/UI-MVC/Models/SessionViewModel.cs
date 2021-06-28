using BL.Domain.Sessie;

namespace UI.MVC.Models
{
    public class SessionViewModel
    {
        public int StudentSessionId { get; set; }
        public int TeacherSessionCode { get; set; }
        public int CurrentStatementId { get; set; }
        public int StatementCount { get; set; }
        public string PartyName { get; set; }
        public GameType GameType { get; set; }
    }
}