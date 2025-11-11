using Microsoft.AspNetCore.Identity;
using System;

namespace Base_Asp_Core_MVC_with_Identity.Areas.Identity.Data
{
    public static class ContextSeed
    {
        public static async Task SeedRolesAsync(UserManager<UserSystemIdentity> userManager, RoleManager<IdentityRole> roleManager)
        {
            //Seed Roles
            await roleManager.CreateAsync(new IdentityRole(Roles.Admin.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Roles.Employee.ToString()));
            //await roleManager.CreateAsync(new IdentityRole(Roles.ManagerCategory.ToString()));
            //await roleManager.CreateAsync(new IdentityRole(Roles.ManagerImport.ToString()));
            //await roleManager.CreateAsync(new IdentityRole(Roles.ManagerReturn.ToString()));
            //await roleManager.CreateAsync(new IdentityRole(Roles.ManagerStock.ToString()));
            //await roleManager.CreateAsync(new IdentityRole(Roles.ManagerSales.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Roles.ViewReport.ToString()));
        }

        public static async Task SeedSuperAdminAsync(UserManager<UserSystemIdentity> userManager, RoleManager<IdentityRole> roleManager)
        {
            //Seed Default User
            var defaultUser = new UserSystemIdentity
            {
                UserName = "admin@gmail.com",
                Email = "admin@gmail.com",
                FirstName = "Admin",
                LastName = "Manager",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true

            };
            defaultUser.ProfilePicture = new byte[0];
            if (userManager.Users.All(u => u.Id != defaultUser.Id))
            {
                var user = await userManager.FindByEmailAsync(defaultUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(defaultUser, "12345@Ab");
                    await userManager.AddToRoleAsync(defaultUser, Roles.Admin.ToString());
                    await userManager.AddToRoleAsync(defaultUser, Roles.Employee.ToString());
                    //await userManager.AddToRoleAsync(defaultUser,Roles.ManagerCategory.ToString());
                    //await userManager.AddToRoleAsync(defaultUser,Roles.ManagerImport.ToString());
                    //await userManager.AddToRoleAsync(defaultUser,Roles.ManagerReturn.ToString());
                    //await userManager.AddToRoleAsync(defaultUser,Roles.ManagerStock.ToString());
                    //await userManager.AddToRoleAsync(defaultUser,Roles.ManagerSales.ToString());
                    await userManager.AddToRoleAsync(defaultUser, Roles.ViewReport.ToString());
                }

            }
        }

    }
}
