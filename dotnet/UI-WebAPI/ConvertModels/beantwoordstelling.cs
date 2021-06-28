using BL.Domain.Test;

namespace UI_WebAPI.ConvertModels
{
    public class beantwoordstelling
    {
        public int SessieId { get; set; }
        public string Argumentering { get; set; }
        public int LeerlingId { get; set; }
        public AnswerOption AnswerOption { get; set; }
    }
}