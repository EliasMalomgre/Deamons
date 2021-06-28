using System.Collections.Generic;
using BL.Domain.Sessie;

namespace BL.Domain.Identity
{
    public class Admin : User
    {
        //public Organisation Organisation { get; set; }

        public Admin()
        {
            Classes = new List<Class>();
        }
    }
}