using BL.Domain.Identity;
using BL.Domain.Sessie;
using Microsoft.AspNetCore.Identity;

namespace UI.MVC.Services
{
    public class IdentityDataInitializer
    {
        public static void SeedData(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            SeedRoles(roleManager);
            SeedUsers(userManager);
        }

        private static void SeedUsers(UserManager<ApplicationUser> userManager)
        {
            if (userManager.FindByEmailAsync("superadmin@treecompany.be").Result == null)
            {
                IdentityResult result;
                var organisation = new Organisation("Karel de Grote hogeschool");
                var teacher2 = new ApplicationUser
                {
                    Email = "arthur@kdg.be",
                    UserName = "arthur@kdg.be",
                    EmailConfirmed = true
                };
                var arthurClass = new Class("INF203", 40);
                var password2 = "arthur123";
                var arthur = new Teacher();
                arthur.Organisation = organisation;
                arthur.Classes.Add(arthurClass);
                teacher2.User = arthur;

                result = userManager.CreateAsync(teacher2, password2).Result;
                var teacher = new ApplicationUser
                {
                    Email = "teacher@kdg.be",
                    UserName = "teacher@kdg.be",
                    EmailConfirmed = true
                };
                var dominiqueClass = new Class("INF203", 40);
                var dominiqueClass2 = new Class("INF201", 35);
                var password = "teacher123";
                var dominique = new Teacher();
                dominique.Organisation = organisation;
                dominique.Classes.Add(dominiqueClass);
                dominique.Classes.Add(dominiqueClass2);
                teacher.User = dominique;
                result = userManager.CreateAsync(teacher, password).Result;

                var school = new ApplicationUser
                {
                    Email = "admin@kdg.be",
                    UserName = "admin@kdg.be",
                    EmailConfirmed = true
                };
                password = "admin123";
                result = userManager.CreateAsync(school, password).Result;
                var admin = new Admin();
                admin.Organisation = organisation;
                school.User = admin;

                var superadmin = new ApplicationUser
                {
                    Email = "superadmin@treecompany.be",
                    UserName = "superadmin@treecompany.be",
                    EmailConfirmed = true
                };
                password = "superadmin123";
                var michiel = new SuperAdmin();
                michiel.Organisation = new Organisation("TreeCompany");
                superadmin.User = michiel;
                result = userManager.CreateAsync(superadmin, password).Result;
            }

            if (!userManager
                .IsInRoleAsync(userManager.FindByEmailAsync("superadmin@treecompany.be").Result, "Superadmin").Result)
            {
                var arthurUser = userManager.FindByNameAsync("arthur@kdg.be").Result;
                var identityResultA = userManager.AddToRoleAsync(arthurUser, "Teacher").Result;

                var applicationUser = userManager.FindByNameAsync("superadmin@treecompany.be").Result;
                var identityResult = userManager.AddToRoleAsync(applicationUser, "SUPERADMIN").Result;

                applicationUser = userManager.FindByNameAsync("admin@kdg.be").Result;
                identityResult = userManager.AddToRoleAsync(applicationUser, "ADMIN").Result;

                applicationUser = userManager.FindByNameAsync("teacher@kdg.be").Result;
                identityResult = userManager.AddToRoleAsync(applicationUser, "TEACHER").Result;
            }
        }

        private static void SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            if (!roleManager.RoleExistsAsync("Superadmin").Result)
            {
                IdentityResult result;
                var role1 = new IdentityRole();
                role1.Name = "Superadmin";
                result = roleManager.CreateAsync(role1).Result;

                var role2 = new IdentityRole();
                role2.Name = "Admin";
                result = roleManager.CreateAsync(role2).Result;

                var role3 = new IdentityRole();
                role3.Name = "Teacher";
                result = roleManager.CreateAsync(role3).Result;
            }
        }
    }
}