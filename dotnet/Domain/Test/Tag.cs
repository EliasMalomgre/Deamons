using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace BL.Domain.Test
{
    public class Tag
    {
        [Key] public int Id { get; set; }

        public string Name { get; set; }

        [JsonIgnore] [IgnoreDataMember] public virtual ICollection<TestTag> Tests { get; set; }
    }
}