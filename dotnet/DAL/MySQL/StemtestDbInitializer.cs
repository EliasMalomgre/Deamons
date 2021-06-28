using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using BL.Domain.Test;
using CsvHelper;
using CsvHelper.Configuration.Attributes;
using Microsoft.EntityFrameworkCore;

namespace DAL.MySQL
{
    internal static class StemtestDbInitializer
    {
        //start als je deze lijnnrs veranderd laten weten aan seppe
        private const string STEMTEST = "..\\DAL\\csv\\stemtest.csv";

        private const string WOORDENLIJST = "..\\DAL\\csv\\woordenlijst.csv";
        //end veranderen lijnnrs

        private static bool hasRunDuringAppExecution;

        public static void Initialize(StemtestDbContext context, bool dropCreateDatabase = false)
        {
            if (!hasRunDuringAppExecution)
            {
                // Delete database if requesed
                if (dropCreateDatabase)
                    context.Database.EnsureDeleted();

                // Create database and seed dummy-data if needed 
                if (context.Database.EnsureCreated())
                {
                    // 'false' if database already exists
                    // Seed initial (dummy-)data into newly created database
                    ImportStemtest(context);
                    LoadWoordverklaringen(context);
                }

                hasRunDuringAppExecution = true;
            }
        }

        public static void ReSeed(StemtestDbContext ctx)
        {
            var test = ctx.Tests.Include(t => t.Statements)
                .ThenInclude(s => s.AnswerOptions)
                .FirstOrDefault(t => t.Id == 1);

            IQueryable<Party> parties = ctx.Parties.Include(p => p.Answers);
            foreach (var p in parties)
            foreach (var a in p.Answers)
                ctx.Answers.Remove(a);

            ctx.SaveChanges();

            var statements = test.Statements;
            foreach (var s in statements)
            {
                var otherAnswers = ctx.Answers.Include(a => a.Statement).Where(a => a.Statement.Id == s.Id);
                foreach (var answer in otherAnswers)
                {
                    answer.Statement = null;
                    ctx.Update(answer);
                }

                foreach (var sao in s.AnswerOptions) ctx.Remove(sao);

                var chosenStatements = ctx.ChosenStatements.Where(cs => cs.Statement.Id == s.Id);
                foreach (var ca in chosenStatements) ctx.Remove(ca);
                ctx.Statements.Remove(s);
            }

            ctx.SaveChanges();

            var definitions = ctx.Definitions.Where(d => d.Test.Id == 1);
            foreach (var d in definitions) ctx.Remove(d);

            ctx.SaveChanges();

            var teachersessions = ctx.TeacherSessions.Where(ts => ts.Test.Id == 1);
            foreach (var ts in teachersessions)
            {
                ts.Test = null;
                ctx.Update(ts);

                foreach (var partyTeacherSession in ts.ChosenParties) ctx.Remove(partyTeacherSession);
            }


            ctx.Tests.Remove(test);
            ctx.SaveChanges();

            foreach (var p in parties) ctx.Remove(p);

            var ao1 = ctx.AnswerOptions.Find(1);
            var ao2 = ctx.AnswerOptions.Find(2);
            var ao3 = ctx.AnswerOptions.Find(3);
            ctx.Remove(ao1);
            ctx.Remove(ao2);
            ctx.Remove(ao3);


            ImportStemtest(ctx);
            LoadWoordverklaringen(ctx);
        }


        private static void ImportStemtest(StemtestDbContext context)
        {
            var tagCheck1 = context.Tags.FirstOrDefault(t => t.Name == "Brexit");
            if (tagCheck1 == null)
            {
                var t1 = new Tag();
                t1.Name = "Brexit";
                context.Tags.Add(t1);
            }

            var tagCheck2 = context.Tags.FirstOrDefault(t => t.Name == "Internationale politiek");
            if (tagCheck2 == null)
            {
                var t2 = new Tag();
                t2.Name = "Internationale politiek";
                context.Tags.Add(t2);
            }

            var tagCheck3 = context.Tags.FirstOrDefault(t => t.Name == "Democratie");
            if (tagCheck3 == null)
            {
                var t3 = new Tag();
                t3.Name = "Democratie";
                context.Tags.Add(t3);
            }

            var akkoord = new AnswerOption();
            akkoord.Id = 1;
            akkoord.Opinion = "Akkoord";
            akkoord.Statements = new List<StatementAnswerOption>();
            var nietAkkoord = new AnswerOption();
            nietAkkoord.Id = 2;
            nietAkkoord.Opinion = "Niet akkoord";
            akkoord.Statements = new List<StatementAnswerOption>();
            var overgeslagen = new AnswerOption();
            overgeslagen.Id = 3;
            overgeslagen.Opinion = "Overslaan";
            overgeslagen.Statements = new List<StatementAnswerOption>();
            context.AnswerOptions.Add(akkoord);
            context.AnswerOptions.Add(nietAkkoord);
            context.AnswerOptions.Add(overgeslagen);
            context.SaveChanges();

            var stemtest = new Test();
            stemtest.Id = 1;
            stemtest.Title = "De stemtest";
            stemtest.Statements = new List<Statement>();
            context.Tests.Add(stemtest);
            context.SaveChanges();

            var CDNV = new Party();
            CDNV.Answers = new List<Answer>();
            CDNV.Name = "CD&V";
            CDNV.Orientation = "centrum";
            CDNV.Colour = "Orange";
            CDNV.PartyLeader = "Joachim Coens";
            CDNV.ImageLink =
                "https://storage.googleapis.com/download/storage/v1/b/daemonsstemtest/o/PiratenPartij.png?generation=1590348159607323&alt=media";
            CDNV.MediaLink = "OxnPZUw-U8Q";
            context.Parties.Add(CDNV);

            var Groen = new Party();
            Groen.Answers = new List<Answer>();
            Groen.Name = "Groen";
            Groen.Orientation = "linkser";
            Groen.Colour = "Green";
            Groen.PartyLeader = "Meyrem Almaci";
            Groen.ImageLink =
                "https://storage.googleapis.com/download/storage/v1/b/daemonsstemtest/o/Groen.png?generation=1590244505364432&alt=media";
            Groen.MediaLink = "9Uj0z0KOB3o";
            context.Parties.Add(Groen);

            var NVA = new Party();
            NVA.Answers = new List<Answer>();
            NVA.Name = "N-VA";
            NVA.Orientation = "rechtser";
            NVA.Colour = "Yellow";
            NVA.PartyLeader = "Bart De Wever";
            NVA.ImageLink =
                "https://storage.googleapis.com/download/storage/v1/b/daemonsstemtest/o/N-VA.png?generation=1590244515353364&alt=media";
            NVA.MediaLink = "OT6JXoETJn4";
            context.Parties.Add(NVA);

            var VLD = new Party();
            VLD.Answers = new List<Answer>();
            VLD.Name = "Open VLD";
            VLD.Orientation = "rechts";
            VLD.Colour = "Blue";
            VLD.PartyLeader = "Gwendolyn Rutten";
            VLD.ImageLink =
                "https://storage.googleapis.com/download/storage/v1/b/daemonsstemtest/o/Open%20VLD.png?generation=1590244525376216&alt=media";
            VLD.MediaLink = "zSb8VD6bgvo";
            context.Parties.Add(VLD);

            var SPA = new Party();
            SPA.Answers = new List<Answer>();
            SPA.Name = "Sp.a";
            SPA.Orientation = "links";
            SPA.Colour = "Red";
            SPA.PartyLeader = "Conner Rousseau";
            SPA.ImageLink =
                "https://storage.googleapis.com/download/storage/v1/b/daemonsstemtest/o/Sp.a.png?generation=1590244564572319&alt=media";
            SPA.MediaLink = "Zzs7kEbJGnc";
            context.Parties.Add(SPA);

            var VB = new Party();
            VB.Answers = new List<Answer>();
            VB.Name = "Vlaams Belang";
            VB.Orientation = "extreem rechts";
            VB.Colour = "Black";
            VB.PartyLeader = "Tom Van Grieken";
            VB.ImageLink =
                "https://storage.googleapis.com/download/storage/v1/b/daemonsstemtest/o/Vlaams%20Belang.jpg?generation=1590244573899425&alt=media";
            VB.MediaLink = "lzwxS6sNuao";
            context.Parties.Add(VB);

            var PVDA = new Party();
            PVDA.Answers = new List<Answer>();
            PVDA.Name = "PvdA";
            PVDA.Orientation = "extreem links";
            PVDA.Colour = "Crimson";
            PVDA.PartyLeader = "Peter Mertens";
            PVDA.ImageLink =
                "https://storage.googleapis.com/download/storage/v1/b/daemonsstemtest/o/PvdA.png?generation=1590244549462506&alt=media";
            PVDA.MediaLink = "Q08m9LqXYBk";
            context.Parties.Add(PVDA);

            context.SaveChanges();


            using (var reader = new StreamReader(STEMTEST))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                csv.Configuration.Delimiter = ";";
                csv.Configuration.IgnoreQuotes = true;
                var records = csv.GetRecords<CsvDataClass>();
                foreach (var r in records)
                {
                    var statement = new Statement();
                    statement.Text = r.Stelling;
                    statement.Explanation = r.VerkorteOmschrijving;
                    statement.AnswerOptions = new List<StatementAnswerOption>();
                    stemtest.Statements.Add(statement);
                    context.Update(stemtest);
                    context.SaveChanges();

                    var sao = new StatementAnswerOption();
                    sao.Statement = statement;
                    sao.AnswerOption = akkoord;
                    var sao2 = new StatementAnswerOption();
                    sao2.Statement = statement;
                    sao2.AnswerOption = nietAkkoord;
                    var sao3 = new StatementAnswerOption();
                    sao3.Statement = statement;
                    sao3.AnswerOption = overgeslagen;
                    statement.AnswerOptions.Add(sao);
                    statement.AnswerOptions.Add(sao2);
                    statement.AnswerOptions.Add(sao3);
                    context.Update(statement);

                    var answerCDNV = new Answer();
                    answerCDNV.Statement = statement;
                    answerCDNV.Argument = r.MotivatieCDNV;
                    if (r.AntwoordCDNV)
                        answerCDNV.ChosenAnswer = akkoord;
                    else
                        answerCDNV.ChosenAnswer = nietAkkoord;

                    CDNV.Answers.Add(answerCDNV);
                    context.Update(CDNV);

                    var answerGroen = new Answer();
                    answerGroen.Statement = statement;
                    answerGroen.Argument = r.MotivatieGroen;
                    if (r.AntwoordGroen)
                        answerGroen.ChosenAnswer = akkoord;
                    else
                        answerGroen.ChosenAnswer = nietAkkoord;

                    Groen.Answers.Add(answerGroen);
                    context.Update(Groen);

                    var answerNVA = new Answer();
                    answerNVA.Statement = statement;
                    answerNVA.Argument = r.MotivatieNVA;
                    if (r.AntwoordNVA)
                        answerNVA.ChosenAnswer = akkoord;
                    else
                        answerNVA.ChosenAnswer = nietAkkoord;

                    NVA.Answers.Add(answerNVA);
                    context.Update(NVA);

                    var answerVLD = new Answer();
                    answerVLD.Statement = statement;
                    answerVLD.Argument = r.MotivatieVLD;
                    if (r.AntwoordVLD)
                        answerVLD.ChosenAnswer = akkoord;
                    else
                        answerVLD.ChosenAnswer = nietAkkoord;

                    VLD.Answers.Add(answerVLD);
                    context.Update(VLD);

                    var answerSPA = new Answer();
                    answerSPA.Statement = statement;
                    answerSPA.Argument = r.MotivatieSPA;
                    if (r.AntwoordSPA)
                        answerSPA.ChosenAnswer = akkoord;
                    else
                        answerSPA.ChosenAnswer = nietAkkoord;

                    SPA.Answers.Add(answerSPA);
                    context.Update(SPA);

                    var answerVB = new Answer();
                    answerVB.Statement = statement;
                    answerVB.Argument = r.MotivatieVB;
                    if (r.AntwoordVB)
                        answerVB.ChosenAnswer = akkoord;
                    else
                        answerVB.ChosenAnswer = nietAkkoord;

                    VB.Answers.Add(answerVB);
                    context.Update(VB);

                    var answerPVDA = new Answer();
                    answerPVDA.Statement = statement;
                    answerPVDA.Argument = r.MotivatiePVDA;
                    if (r.AntwoordPVDA)
                        answerPVDA.ChosenAnswer = akkoord;
                    else
                        answerPVDA.ChosenAnswer = nietAkkoord;

                    PVDA.Answers.Add(answerPVDA);
                    context.Update(PVDA);

                    context.SaveChanges();
                }
            }
        }

        public static void LoadWoordverklaringen(StemtestDbContext context)
        {
            var test = context.Tests.Find(1);

            using (var reader = new StreamReader(WOORDENLIJST))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                csv.Configuration.Delimiter = ";";
                csv.Configuration.IgnoreQuotes = true;
                csv.Configuration.HasHeaderRecord = false;
                var records = csv.GetRecords<Woordverklaringen>();
                foreach (var r in records)
                {
                    var definition = new Definition();
                    definition.Word = r.Woord;
                    definition.Explanation = r.Verklaring;
                    definition.Test = test;
                    context.Add(definition);
                    context.SaveChanges();
                }
            }
        }


        private class CsvDataClass
        {
            [Name("Stelling")] public string Stelling { get; set; }
            [Name("Verkorte omschrijving")] public string VerkorteOmschrijving { get; set; }
            [Name("Antwoord CD&V")] public bool AntwoordCDNV { get; set; }
            [Name("Motivatie CD&V")] public string MotivatieCDNV { get; set; }
            [Name("Antwoord Groen")] public bool AntwoordGroen { get; set; }
            [Name("Motivatie Groen")] public string MotivatieGroen { get; set; }
            [Name("Antwoord N-VA")] public bool AntwoordNVA { get; set; }
            [Name("Motivatie N-VA")] public string MotivatieNVA { get; set; }
            [Name("Antwoord Open VLD")] public bool AntwoordVLD { get; set; }
            [Name("Motivatie Open VLD")] public string MotivatieVLD { get; set; }
            [Name("Antwoord Sp.a")] public bool AntwoordSPA { get; set; }
            [Name("Motivatie Sp.a")] public string MotivatieSPA { get; set; }
            [Name("Antwoord Vlaams Belang")] public bool AntwoordVB { get; set; }
            [Name("Motivatie Vlaams Belang")] public string MotivatieVB { get; set; }
            [Name("Antwoord PvdA")] public bool AntwoordPVDA { get; set; }
            [Name("Motivatie PvdA")] public string MotivatiePVDA { get; set; }
        }

        private class Woordverklaringen
        {
            [Index(0)] public string Woord { get; set; }
            [Index(1)] public string Verklaring { get; set; }
        }
    }
}