using System.ComponentModel.DataAnnotations;

namespace BL.Domain.Test
{
    public class Definition
    {
        [Key] public int Id { get; set; }

        public string Word { get; set; }
        public string Explanation { get; set; }

        public Test Test { get; set; }
    }
}