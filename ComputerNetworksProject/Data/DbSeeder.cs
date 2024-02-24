using ComputerNetworksProject.Constants;
using Microsoft.AspNetCore.Identity;

namespace ComputerNetworksProject.Data
{
    public static class DbSeeder
    {
        public static async Task SeedRolesAndAdminAsync(IServiceProvider service)
        {
            //Seed Roles
            var userManager = service.GetService<UserManager<User>>();
            var roleManager = service.GetService<RoleManager<IdentityRole>>();
            await roleManager.CreateAsync(new IdentityRole(Roles.Admin.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Roles.User.ToString()));

            // creating admin

            var user = new User
            {
                UserName = "tomerkakou@gmail.com",
                Email = "tomerkakou@gmail.com",
                FirstName = "Tomer",
                LastName = "Kakou",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true
            };
            var userInDb = await userManager.FindByEmailAsync(user.Email);
            if (userInDb == null)
            {
                await userManager.CreateAsync(user, "Admin@123");
                await userManager.AddToRoleAsync(user, Roles.Admin.ToString());
            }

            var _db = service.GetService<ApplicationDbContext>();
            var category = new Category
            {
                Id = 1,
                Name = "Default",
            };
            if(await _db.Categories.FindAsync(1) is null)
            {
                _db.Categories.Add(category);
                await _db.SaveChangesAsync();
            }
        }

    }
}
