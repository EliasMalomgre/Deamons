using BL.Domain.Identity;
using BL.Domain.Sessie;
using BL.Domain.Test;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DAL.MySQL
{
    public sealed class StemtestDbContext : IdentityDbContext<ApplicationUser>
    {
        //start als je deze lijnen hieronder veranderen laten weten aan seppe        
        //private const string USERNAME = "DAEMONS";
        //private const string PASSWORD = "DAEMONS";
        //private const string ADDRESS = "35.205.99.231";

        private const string USERNAME = "DAEMONS1";
        private const string PASSWORD = "daemons1";

        private const string ADDRESS = "34.77.106.120";
        //end als je veranderd seppe

        private const string SCHEMA = "stemtestdb";


        public StemtestDbContext(DbContextOptions<StemtestDbContext> options) : base(options)
        {
            StemtestDbInitializer.Initialize(this, false);
        }

        //hier komen alle tabellen
        //example:
        public DbSet<Party> Parties { get; set; }
        public DbSet<TeacherSession> TeacherSessions { get; set; }
        public DbSet<StudentSession> StudentSessions { get; set; }
        public DbSet<Answer> Answers { get; set; }
        public DbSet<AnswerOption> AnswerOptions { get; set; }
        public DbSet<Test> Tests { get; set; }
        public DbSet<Statement> Statements { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Definition> Definitions { get; set; }
        public DbSet<Class> Classes { get; set; }
        public DbSet<User> DataUsers { get; set; }
        public DbSet<Organisation> Organisations { get; set; }
        public DbSet<ChosenStatement> ChosenStatements { get; set; }
        public DbSet<SessionSettings> SessionSettings { get; set; }
        public DbSet<SharedTest> SharedTests { get; set; }

        public void ReSeedDatabase()
        {
            StemtestDbInitializer.ReSeed(this);
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<TestTag>().Property<int>("FK_TEST");
            modelBuilder.Entity<TestTag>().Property<int>("FK_TAG");

            modelBuilder.Entity<TestTag>().HasOne(t => t.Test).WithMany(t => t.Tags)
                .HasForeignKey("FK_TEST");
            modelBuilder.Entity<TestTag>().HasOne(t => t.Tag).WithMany(t => t.Tests)
                .HasForeignKey("FK_TAG");

            modelBuilder.Entity<StatementAnswerOption>().HasKey(sao => new {sao.StatementId, sao.AnswerOptionId});

            modelBuilder.Entity<PartyTeacherSession>().Property<string>("FK_PARTIJ");
            modelBuilder.Entity<PartyTeacherSession>().Property<int>("FK_LKRSESSIE");

            modelBuilder.Entity<PartyTeacherSession>().HasOne(p => p.Party).WithMany(l => l.TeacherSessions)
                .HasForeignKey("FK_PARTIJ");
            modelBuilder.Entity<PartyTeacherSession>().HasOne(l => l.TeacherSession)
                .WithMany(p => p.ChosenParties)
                .HasForeignKey("FK_LKRSESSIE");

            //PARTIJ
            modelBuilder.Entity<Party>(entity => { });

            //LEERKRACHTSESSIE
            modelBuilder.Entity<TeacherSession>().HasOne(e => e.Teacher);
            modelBuilder.Entity<TeacherSession>(entity => { });

            //TESTEN
            modelBuilder.Entity<Test>().HasOne(e => e.Maker);
            modelBuilder.Entity<Test>(entity => { });

            //LEERLINGSESSIES
            modelBuilder.Entity<StudentSession>(entity => { });

            //ANTWOORDMOGELIJKHEDEN
            modelBuilder.Entity<AnswerOption>(entity => { });

            //ANTWOORDEN
            modelBuilder.Entity<Answer>().HasOne(e => e.ChosenAnswer);
            modelBuilder.Entity<Answer>(entity => { });

            //STELLINGEN
            modelBuilder.Entity<Statement>(entity => { });
            modelBuilder.Entity<StatementAnswerOption>(entity => { });

            //WOORDVERKLARINGEN
            modelBuilder.Entity<Definition>(entity => { });

            //TAGS
            modelBuilder.Entity<Tag>(entity => { });

            //KLASSEN
            modelBuilder.Entity<Class>(entity => { });

            //LEERKRACHT
            modelBuilder.Entity<Admin>().HasOne(e => e.Organisation);
            modelBuilder.Entity<Teacher>().HasMany(e => e.Classes);
        }
    }
}