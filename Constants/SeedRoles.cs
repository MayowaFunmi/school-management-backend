using Microsoft.AspNetCore.Identity;
using SchoolManagementApi.Models.UserModels;
using SchoolManagementApi.Utilities;
using static SchoolManagementApi.Constants.RoleCionstants;

namespace SchoolManagementApi.Constants
{
  public class SeedRoles
  {
    public static async Task SeedRolesAndAdminAsync(IServiceProvider serviceProvider, IConfiguration configuration)
    {
      var userManager = serviceProvider.GetService<UserManager<ApplicationUser>>()!;
      var roleManager = serviceProvider.GetService<RoleManager<IdentityRole>>()!;

      if (!roleManager.Roles.Any())
      {
        await roleManager.CreateAsync(new IdentityRole(Roles.Owner.ToString()));
        await roleManager.CreateAsync(new IdentityRole(Roles.Users.ToString()));
        await roleManager.CreateAsync(new IdentityRole(Roles.Admin.ToString()));
        await roleManager.CreateAsync(new IdentityRole(Roles.SuperAdmin.ToString()));
        await roleManager.CreateAsync(new IdentityRole(Roles.OrganizationAdmin.ToString()));
        await roleManager.CreateAsync(new IdentityRole(Roles.TeachingStaff.ToString()));
        await roleManager.CreateAsync(new IdentityRole(Roles.NonTeachingStaff.ToString()));
        await roleManager.CreateAsync(new IdentityRole(Roles.Parent.ToString()));
        await roleManager.CreateAsync(new IdentityRole(Roles.Student.ToString()));
      }
      
      // create user
      var email = configuration.GetSection("Credentials:Email").Value;
      var username = configuration.GetSection("Credentials:Username").Value;
      var password = configuration.GetSection("Credentials:Password").Value;

      var user = new ApplicationUser
      {
        UniqueId = GenerateUserCode.GenerateUserUniqueId(),
        UserName = username,
        Email = email,
        FirstName = "Mayowa",
        LastName = "Akinade",
        EmailConfirmed = true,
        PhoneNumber = "09032055129",
        PhoneNumberConfirmed = true,
        CreatedAt = DateTime.UtcNow
      };

      if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
      {
        var userRole = await userManager.FindByEmailAsync(email);
        if (userRole == null)
        {
          await userManager.CreateAsync(user, password);
          await userManager.AddToRolesAsync(user, [StaticUserRoles.Owner, StaticUserRoles.Users, StaticUserRoles.SuperAdmin, StaticUserRoles.Admin, StaticUserRoles.TeachingStaff, StaticUserRoles.NonTeachingStaff]);
        }
      }
    }
  }
}