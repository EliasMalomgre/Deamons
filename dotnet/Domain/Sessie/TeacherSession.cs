using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using BL.Domain.Identity;
using BL.Domain.Test;

namespace BL.Domain.Sessie
{
    public class TeacherSession
    {
        //ctor voor startSession uit sessieService
        public TeacherSession(Test.Test test, GameType gameType, Class @class, User user, int sessionCode)
        {
            StudentSessions = new List<StudentSession>();
            Test = test;
            Class = @class;
            GameType = gameType;
            Teacher = user;
            SessionCode = sessionCode;
            MaxAmountStudents = Class.NumberOfStudents;
        }

        public TeacherSession()
        {
        }

        [Key] public int Id { get; set; }

        public int SessionCode { get; set; }
        public int MaxAmountStudents { get; set; }
        public int CurrentAmountStudents { get; set; }
        public Class Class { get; set; }
        public Test.Test Test { get; set; }
        public User Teacher { get; set; }
        public int CurrentStatement { get; set; }

        public GameType GameType { get; set; }
        public DateTime Date { get; set; }

        //options
        public SessionSettings Settings { get; set; }

        public List<StudentSession> StudentSessions { get; set; }
        public List<ChosenStatement> ChosenStatements { get; set; }
        [IgnoreDataMember] [JsonIgnore] public virtual List<PartyTeacherSession> ChosenParties { get; set; }
    }
}