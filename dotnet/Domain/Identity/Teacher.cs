using System.Collections.Generic;
using BL.Domain.Sessie;

namespace BL.Domain.Identity
{
    public class Teacher : User
    {
        public Teacher()
        {
            Classes = new List<Class>();
        }

        public void
            AddQuestion(int vanTestId, int statementId,
                int toStatementId) //interpretatie: kopieer stelling van test A naar test B
        {
            //TODO dal services nodig voor uitwerking
        }
    }
}