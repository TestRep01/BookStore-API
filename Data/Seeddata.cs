using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookStore_API.Data
{
    public static class Seeddata
    {
        public async static Task Seed(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManger)
        {
            await SeedRoles(roleManger);
            await  SeedUsers(userManager);
        }


        private async static Task SeedUsers(UserManager<IdentityUser> userManager)
        {
            if (await userManager.FindByEmailAsync("admin@Bookstore.com") == null)
            {
                var user = new IdentityUser
                {
                    UserName = "admin",
                    Email = "admin@Bookstore.com"
                };

                var result = await userManager.CreateAsync(user, "P@$$Word0");
                if(result.Succeeded)
                {
                   await userManager.AddToRoleAsync(user, "Administrator");
                }
            }

            if (await userManager.FindByEmailAsync("customer@Bookstore.com") == null)
            {
                var user = new IdentityUser
                {
                    UserName = "customer",
                    Email = "customer@Bookstore.com"
                };

                var result = await userManager.CreateAsync(user, "P@$$Word0");
                if (result.Succeeded)
                {
                   await userManager.AddToRoleAsync(user, "Customer");
                }
            }
        }

        
        private async static Task SeedRoles(RoleManager<IdentityRole> roleManger) 
        {
          if(! await roleManger.RoleExistsAsync("Administrator"))
            {
                var role = new IdentityRole
                { 
                 Name = "Administrator"
                };

                await roleManger.CreateAsync(role);
            }

            if (!await roleManger.RoleExistsAsync("Customer"))
            {
                var role = new IdentityRole
                {
                    Name = "Customer"
                };

               await roleManger.CreateAsync(role);
            }

        }
    }
}
