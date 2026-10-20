namespace webProgramming.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using webProgramming.Models; // ApplicationUser sınıfının olduğu yer

    internal sealed class Configuration : DbMigrationsConfiguration<webProgramming.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(webProgramming.Models.ApplicationDbContext context)
        {
            // Kimlik yöneticilerini oluştur
            var roleStore = new RoleStore<IdentityRole>(context);
            var roleManager = new RoleManager<IdentityRole>(roleStore);
            var userStore = new UserStore<ApplicationUser>(context);
            var userManager = new UserManager<ApplicationUser>(userStore);

            // 1. Admin rolü yoksa oluştur
            if (!context.Roles.Any(r => r.Name == "Admin"))
            {
                roleManager.Create(new IdentityRole { Name = "Admin" });
            }

            // 2. admin kullanıcısı yoksa oluştur ve şifresini belirle
            if (!context.Users.Any(u => u.UserName == "admin"))
            {
                var user = new ApplicationUser { UserName = "admin", Email = "admin@site.com" };
                var result = userManager.Create(user, "Admin123!");

                // 3. Başarıyla oluştuysa ona Admin yetkisi ver
                if (result.Succeeded)
                {
                    userManager.AddToRole(user.Id, "Admin");
                }
            }
        }
    }
}