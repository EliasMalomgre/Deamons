using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using BL.Domain.Identity;

namespace BL.Domain.Test
{
    public class Test
    {
        public Test()
        {
        }

        public Test(int id, string title, Teacher maker)
        {
            Statements = new List<Statement>();
            Id = id;
            Title = title;
            Maker = maker;
        }

        public int Id { get; set; }
        public string Title { get; set; }

        [NotMapped] public User Maker { get; set; }

        public List<Statement> Statements { get; set; }

        [JsonIgnore] [IgnoreDataMember] public virtual ICollection<TestTag> Tags { get; set; }

        public void AddQuestion(string vraag, string uitleg)
        {
            var statement = new Statement(Statements.Count + 1, vraag, uitleg);
        }

        public void AddQuestion(string vraag)
        {
            AddQuestion(vraag, null);
        }
    }
}