using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace BL.Domain.Test
{
    public class AnswerOption
    {
        public AnswerOption(string opinion)
        {
            Opinion = opinion;
        }

        public AnswerOption()
        {
        }

        [Key] public int Id { get; set; }

        [JsonIgnore] [IgnoreDataMember] public virtual ICollection<StatementAnswerOption> Statements { get; set; }

        public string Opinion { get; set; }
    }
}