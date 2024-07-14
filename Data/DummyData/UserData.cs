using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SchoolManagementApi.Models.UserModels;

namespace SchoolManagementApi.Data.DummyData
{
  public class UserData
  {
    public static void SeedUserData(ModelBuilder modelBuilder)
    {
      var passwordHasher = new PasswordHasher<ApplicationUser>();
      modelBuilder.Entity<ApplicationUser>().HasData(
        new ApplicationUser
        {
          Id = Guid.NewGuid().ToString(),
          
        }
      );
    }
  }
}