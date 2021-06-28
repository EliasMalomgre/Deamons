using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace BL.Domain.Test
{
    public class Statement
    {
        public Statement()
        {
        }

        public Statement(int id)
        {
            Definitions = new List<Definition>();
            Id = id;
        }

        public Statement(int id, string text, string explanation = "geen uitleg")
        {
            Definitions = new List<Definition>();
            Id = id;
            Text = text;
            Explanation = explanation;
        }

        [Key] public int Id { get; set; }

        public string Text { get; set; }
        public string Explanation { get; set; }

        //Voor CustomTest
        //public AnswerOption CorrectAnswerOption { get; set; }

        public List<Definition> Definitions { get; set; }

        [JsonIgnore] [IgnoreDataMember] public virtual ICollection<StatementAnswerOption> AnswerOptions { get; set; }
    }
}