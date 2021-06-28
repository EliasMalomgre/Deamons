using System.ComponentModel.DataAnnotations;
using BL.Domain.Sessie;

namespace BL.Domain.Test
{
    public class PartyTeacherSession
    {
        [Key] public int Id { get; set; }

        public Party Party { get; set; }
        public TeacherSession TeacherSession { get; set; }
    }
}