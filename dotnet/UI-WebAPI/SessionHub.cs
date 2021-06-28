using System;
using System.Linq;
using System.Threading.Tasks;
using BL.DBManagers;
using Microsoft.AspNetCore.SignalR;

namespace UI_WebAPI
{
    public class SessionHub : Hub
    {
        private readonly DbSessionManager _dbSessionManager;

        public SessionHub(DbSessionManager dbSessionManager)
        {
            _dbSessionManager = dbSessionManager;
        }

        public async Task AddToGroup(string sessionCode)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, sessionCode);
            var ts = _dbSessionManager.GetTeacherSession(Convert.ToInt32(sessionCode));
            var teacher = ts.Teacher;
            var amountOfUsers = ts.CurrentAmountStudents;
            await Clients.Group(teacher.ApplicationUserId).SendAsync("RefreshUsers", amountOfUsers);
        }

        public async Task RefreshTeacherResults(string tsessioncode)
        {
            var ts = _dbSessionManager.GetTeacherSession(Convert.ToInt32(tsessioncode));
            var teacher = ts.Teacher;
            await Clients.Groups(teacher.ApplicationUserId).SendAsync("RefreshPage");
        }

        public async Task AddTeacherGroup(string sessionCode)
        {
            var teacher = _dbSessionManager.GetTeacherSession(Convert.ToInt32(sessionCode)).Teacher;
            await Groups.AddToGroupAsync(Context.ConnectionId, teacher.ApplicationUserId);
        }

        public async Task SendGameReadySignal(string sessionId)
        {
            var pos = _dbSessionManager.GetTeacherSession(Convert.ToInt32(sessionId));
            await Clients.Group(sessionId).SendAsync("StopWaiting", pos.CurrentStatement.ToString());
        }


        public async Task GetTeacherPosition(string tSessionCode)
        {
            var pos = _dbSessionManager.GetTeacherSession(Convert.ToInt32(tSessionCode));
            await Clients.Caller.SendAsync("SessionPosition", pos.CurrentStatement.ToString(),
                pos.ChosenStatements.Count().ToString());
        }
    }
}