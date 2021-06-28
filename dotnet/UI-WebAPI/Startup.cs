using System;
using BL.DBManagers;
using DAL.MySQL;
using DAL.MySQL.MySQLRepositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace UI_WebAPI
{
    public class Startup
    {
        public Startup(IHostEnvironment env, IConfiguration configuration)
        {
            Configuration = configuration;
            CurrentEnvironment = env;
        }

        private string ConnectionString { get; set; }
        private string RedisConnection { get; set; }
        private IHostEnvironment CurrentEnvironment { get; }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
//	services.AddHttpsRedirection(options =>
//    {
//        options.RedirectStatusCode = StatusCodes.Status307TemporaryRedirect;
//        options.HttpsPort = 5001;
//    });	
            RedisConnection = Configuration.GetSection("Redis")["ConnectionString"];
            ConnectionString = Configuration.GetConnectionString("StemtestDb");
            services.AddCors(options => options.AddPolicy("CorsPolicy",
                builder =>
                {
                    builder.AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials()
                        .WithOrigins("https://localhost:44316", "http://localhost:44316",
                            "https://localhost:44319", "http://localhost:44319",
                            "https://localhost:5001", "https://localhost:5003", "http://localhost:5001",
                            "https://localhost:5003");
                }));

            //WITHOUT REDIS
            if (CurrentEnvironment.IsDevelopment())
                services.AddSignalR()
                    .AddHubOptions<SessionHub>(options =>
                    {
                        options.ClientTimeoutInterval = TimeSpan.FromHours(1);
                        options.EnableDetailedErrors = true;
                    });

            //SIGNALR WITH REDIS BACKPLANE
            if (CurrentEnvironment.IsProduction())
            {
                if (RedisConnection != "")
                    services.AddSignalR()
                        .AddHubOptions<SessionHub>(options =>
                        {
                            options.ClientTimeoutInterval = TimeSpan.FromHours(1);
                            options.EnableDetailedErrors = true;
                        })
                        .AddStackExchangeRedis(RedisConnection,
                            options => options.Configuration.ChannelPrefix = "UI-WebAPI");
                else
                    services.AddSignalR()
                        .AddHubOptions<SessionHub>(options =>
                        {
                            options.ClientTimeoutInterval = TimeSpan.FromHours(1);
                            options.EnableDetailedErrors = true;
                        });
            }

            services.AddMvc(options => options.OutputFormatters.RemoveType<StringOutputFormatter>());
            services.AddControllers();
            //DbManager services
            services.AddScoped<DbTestManager>();
            services.AddScoped<DbSessionManager>();
            services.AddScoped<DbUserManager>();
            services.AddScoped<DbPartyManager>();
            //Repo services
            services.AddScoped<MySQLTestRepository>();
            services.AddScoped<MySQLSessionRepository>();
            services.AddScoped<MySQLUserRepository>();
            services.AddScoped<MySQLPartyRepository>();
            //add db context scoped
            services.AddDbContext<StemtestDbContext>(options=>options.UseMySql(ConnectionString,o=>o.EnableRetryOnFailure(10,TimeSpan.FromSeconds(10),null)));
            services.AddSignalR();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment()) app.UseDeveloperExceptionPage();

            if (env.IsProduction())
            {
            }

            //needed for nginx reverse proxy
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            //app.UseHttpsRedirection();

            app.UseRouting();
            app.UseCors("CorsPolicy");

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                //SignalR mapping
                endpoints.MapHub<SessionHub>("/api/sessionHub");
            });
        }
    }
}