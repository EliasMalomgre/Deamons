using System.ComponentModel.DataAnnotations;

namespace BL.Domain.Test
{
    public class StatementAnswerOption
    {
        public StatementAnswerOption()
        {
            isCorrectAnswer = false;
        }

        [Key] public int Id { get; set; }

        public Statement Statement { get; set; }
        public int StatementId { get; set; }
        public AnswerOption AnswerOption { get; set; }
        public int AnswerOptionId { get; set; }
        public bool isCorrectAnswer { get; set; }
    }
}