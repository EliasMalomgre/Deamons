using System.ComponentModel.DataAnnotations;

namespace BL.Domain.Sessie
{
    public class Class
    {
        public Class(string name, int numberOfStudents)
        {
            Name = name;
            NumberOfStudents = numberOfStudents;
        }

        [Key] public int Id { get; set; }

        public string Name { get; set; }
        public string Year { get; set; }
        public int NumberOfStudents { get; set; }
    }
}