using System.Data.Entity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace webProgramming.Models
{
    // This tells the app to create the DB and Admin account if they don't exist yet
    public class DbInitializer : CreateDatabaseIfNotExists<ApplicationDbContext>
    {
        protected override void Seed(ApplicationDbContext context)
        {
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            // 1. Create the Roles
            if (!roleManager.RoleExists("Admin"))
                roleManager.Create(new IdentityRole("Admin"));
                
            if (!roleManager.RoleExists("User"))
                roleManager.Create(new IdentityRole("User"));

            // 2. Create the default Admin User
            // 1. "Admin" adında bir rol var mı kontrol et, yoksa oluştur
            // 2. "admin" kullanıcı adıyla bir hesap var mı kontrol et (Email araması yerine)
            if (userManager.FindByName("admin") == null)
            {
                var adminUser = new ApplicationUser { UserName = "admin" };

                // Şifre kurallara uygun olmalı (Büyük harf, sayı ve özel karakter)
                var result = userManager.Create(adminUser, "Admin123!");

                if (result.Succeeded)
                {
                    // Kullanıcı başarıyla oluştuysa Admin rolünü ata
                    userManager.AddToRole(adminUser.Id, "Admin");
                }
            }

            base.Seed(context);
        }
    }
}