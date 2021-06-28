using System.ComponentModel.DataAnnotations;
using BL.Domain.Test;

namespace BL.Domain.Sessie
{
    public class ChosenStatement
    {
        [Key] public int Id { get; set; }

        public Statement Statement { get; set; }
    }
}