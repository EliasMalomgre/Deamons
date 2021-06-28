using System.ComponentModel.DataAnnotations;

namespace BL.Domain.Test
{
    public class Answer
    {
        public Answer()
        {
        }

        public Answer(AnswerOption answerOption, string argument = "")
        {
            Argument = argument;
            ChosenAnswer = answerOption;
        }

        [Key] public int Id { get; set; }

        public string Argument { get; set; }
        public AnswerOption ChosenAnswer { get; set; }
        public bool Correct { set; get; }
        public Statement Statement { get; set; }
    }
}