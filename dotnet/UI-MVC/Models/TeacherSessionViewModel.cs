using BL.Domain.Sessie;

namespace UI.MVC.Models
{
    public class TeacherSessionViewModel
    {
        public int SessionCode { get; set; }
        public int StatementCount { get; set; }
        public int CurrentStatement { get; set; }
        public string ClassName { get; set; }
        public int CurrentStudentCount { get; set; }
        public int MaxAmountStudents { get; set; }
        public GameType GameType { get; set; }
    }
}