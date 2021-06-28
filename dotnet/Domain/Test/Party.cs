using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace BL.Domain.Test
{
    public class Party
    {
        public Party()
        {
            Answers = new List<Answer>();
        }

        public Party(string name)
        {
            Name = name;
            Answers = new List<Answer>();
        }

        public Party(string name, string orientation, string colour, string partyLeader)
        {
            Name = name;
            Orientation = orientation;
            Colour = colour;
            PartyLeader = partyLeader;
            Answers = new List<Answer>();
        }

        [Key] public string Name { get; set; }

        public string Orientation { get; set; }
        public string Colour { get; set; }
        public string PartyLeader { get; set; }
        public string ImageLink { get; set; }
        public string MediaLink { get; set; }
        public string Description { get; set; }
        public List<Answer> Answers { get; set; }

        [JsonIgnore] [IgnoreDataMember] public virtual List<PartyTeacherSession> TeacherSessions { get; set; }
    }
}