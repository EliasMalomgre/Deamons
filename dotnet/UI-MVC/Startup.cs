using System;
using System.Collections.Generic;
using System.Globalization;
using BL.DBManagers;
using BL.Domain.Identity;
using DAL.MySQL;
using DAL.MySQL.MySQLRepositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using UI.MVC.Services;

namespace UI.MVC
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
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

            services.AddDbContext<StemtestDbContext>(
                options => { options.UseMySql(Configuration.GetConnectionString("StemtestDb"),o=>o.EnableRetryOnFailure(10,TimeSpan.FromSeconds(10),null )); });

            services.AddCors(options => options.AddPolicy("CorsPolicy",
                builder => {
                    builder.AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials()
                        .WithOrigins("https://localhost:44319", "http://localhost:44319",
                            "https://localhost:44316", "http://localhost:44316",
                            "https://localhost:5000", "https://localhost:5002",
                            "http://localhost:5000", "http://localhost:5002");
                }));
            services.AddIdentity<ApplicationUser, IdentityRole>(
                    options => options.SignIn.RequireConfirmedAccount = true).AddDefaultTokenProviders()
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<StemtestDbContext>();
            services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminOrHigher", policy =>
                    policy.RequireRole("Admin", "Superadmin"));
                options.AddPolicy("TeacherOrHigher", policy =>
                    policy.RequireRole("Teacher", "Admin", "Superadmin"));
                options.AddPolicy("OnlySuperadmin", policy =>
                    policy.RequireRole("Superadmin"));
            });
            services.AddTransient<IEmailSender, EmailSender>();
            services.Configure<AuthMessageSenderOptions>(options =>
                Configuration.GetSection("AuthMessageSenderOptions").Bind(options));

            //Google cloud
            services.Configure<CloudStorageOptions>(
                Configuration.GetSection("GoogleCloudStorage"));

            //ZEER EENVOUDIG VOOR TESTING PURPOSES AANPASSEN VOOR PRODUCTION!
            services.Configure<IdentityOptions>(options =>
            {
                // Password settings.
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 1;

                // Lockout settings.
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                // User settings.
                options.User.AllowedUserNameCharacters =
                    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.User.RequireUniqueEmail = false;
            });


            services.ConfigureApplicationCookie(options =>
            {
                // Cookie settings
                options.Cookie.HttpOnly = false;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(70);

                options.LoginPath = "/Identity/Account/Login";
                options.LogoutPath = "/Identity/Account/Logout";
                options.AccessDeniedPath = "/Identity/Account/AccessDenied";
                options.SlidingExpiration = true;
            });

            services.AddLocalization(options => { options.ResourcesPath = "Resources"; });

            #region RequestLocalizationOptions

            //hier wordt localization verder ingesteld
            services.Configure<RequestLocalizationOptions>(options =>
            {
                //een lijst van alle ondersteunde culturen
                var supportedCultures = new List<CultureInfo>
                {
                    new CultureInfo("nl"),
                    new CultureInfo("en")
                };
                //je standaard cultuur: indien de verzochte culture niet beschikbaar is, of er geen info komt van de client
                options.DefaultRequestCulture = new RequestCulture("nl");
                //voor nummers, datums, etc. te formatteren => globalization
                options.SupportedCultures = supportedCultures;
                //voor de localized UI strings => localization
                options.SupportedUICultures = supportedCultures;
            });

            #endregion

            //Nodig voor identity files
            services.AddMvc()
                .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
                .AddDataAnnotationsLocalization()
                .AddRazorPagesOptions(options =>
                {
                    options.Conventions.AuthorizeAreaFolder("Identity", "/Account/Manage");
                    options.Conventions.AuthorizeAreaPage("Identity", "/Account/Logout");
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseRequestLocalization(app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>()
                .Value);

            //app.UseHttpsRedirection();

            app.UseStaticFiles();
            app.UseWebSockets();
            app.UseRouting();
            app.UseCors("CorsPolicy");
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllerRoute(
                    "default",
                    "{controller=Game}/{action=Index}/{id?}");
            });
        }
    }
}