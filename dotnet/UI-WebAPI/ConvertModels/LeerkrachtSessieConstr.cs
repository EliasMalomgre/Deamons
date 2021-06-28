using BL.Domain.Identity;
using BL.Domain.Sessie;
using BL.Domain.Test;

namespace UI_WebAPI.ConvertModels
{
    public class LeerkrachtSessieConstr
    {
        public Test Test { get; set; }
        public GameType GameType { get; set; }
        public Class Class { get; set; }
        public Teacher Teacher { get; set; }
        public int SessieCode { get; set; }
    }
}