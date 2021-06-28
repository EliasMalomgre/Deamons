using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using BL;
using BL.DBManagers;
using BL.Domain.Identity;
using BL.Domain.Sessie;
using BL.Domain.Test;
using DAL.MySQL;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;

namespace CA_TEST
{
    class QueryTesting
    {
        
        
        static void Main(string[] args)
        {
            string link = "https://www.youtube.com/watch?v=5FdEzwgOmtY";
            Match match = Regex.Match(link, @"v=[a-zA-Z0-9]+");
            string vidId = match.Value.Substring(2);
            Console.WriteLine(vidId);

        }
    }
}