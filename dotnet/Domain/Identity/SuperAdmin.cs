using System.Collections.Generic;
using BL.Domain.Sessie;

namespace BL.Domain.Identity
{
    public class SuperAdmin : User
    {
        public SuperAdmin()
        {
            Classes = new List<Class>();
        }
    }
}