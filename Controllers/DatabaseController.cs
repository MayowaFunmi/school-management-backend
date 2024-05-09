using System.Net;
using System.Security.Cryptography;
using System.Text;
using Bogus;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementApi.Constants;
using SchoolManagementApi.Data;
using SchoolManagementApi.DTOs;
using SchoolManagementApi.Models;
using SchoolManagementApi.Models.UserModels;
using SchoolManagementApi.Utilities;

namespace SchoolManagementApi.Controllers
{
  //[Authorize(Policy = "OwnerSuperAdmin")]
  public class DatabaseController(ApplicationDbContext context, UserManager<ApplicationUser> userManager) : BaseController
  {
    private readonly ApplicationDbContext _context = context;
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    // Generate user data
    [HttpPost]
    [Route("generate-fake-users")]
    public async Task<GenericResponse> GenerateUsers()
    {
      try
      {
        var faker = new Faker<ApplicationUser>()
          .RuleFor(u => u.UniqueId, f => GenerateUserCode.GenerateUserUniqueId())
          .RuleFor(u => u.FirstName, f => f.Name.FirstName())
          .RuleFor(u => u.LastName, f => f.Name.LastName())
          .RuleFor(u => u.Email, (f, u) => GenerateEmail(u.FirstName, u.LastName))
          .RuleFor(u => u.PhoneNumber, f => f.Phone.PhoneNumber())
          .RuleFor(u => u.UserName, (f, u) => GenerateUsername(u.FirstName))
          .RuleFor(u => u.PasswordHash, (f, u) => GeneratePassword(u.LastName));

        var users = faker.Generate(100);
        await _context.Users.AddRangeAsync(users);
        await _context.SaveChangesAsync();
        foreach(var user in users)
        {
          await _userManager.AddToRoleAsync(user, StaticUserRoles.Users);
        }
        return new GenericResponse
        {
          Status = HttpStatusCode.OK.ToString(),
          Message = $"{users.Count} users added successfully",
          Data = users
        };
      }
      catch (Exception ex)
      {
        return new GenericResponse
        {
          Status = HttpStatusCode.InternalServerError.ToString(),
          Message = $"Error during fake user generation - {ex.Message}",
        };
      }
    }

    [HttpPost]
    [Route("create-fake-organizations")]
    public async Task<GenericResponse> GenerateOrganizations()
    {
      var adminIds = new List<string>
      {
        "01eef9fe-0e7c-48a1-87d2-36043f00d9ce",
        "1a592415-06b1-4088-8b5c-bc43e4882f77",
        "4cf07751-9546-445b-a79d-bd32c0e63a68",
      };
      try
      {
        var faker = new Faker<Organization>()
          .RuleFor(o => o.OrganizationUniqueId, f => GenerateUserCode.GenerateOrgUniqueId())
          .RuleFor(o => o.AdminId, f => f.PickRandom(adminIds))
          .RuleFor(o => o.Name, f => f.Company.CompanyName());
        var organizations = faker.Generate(5);
        await _context.Organizations.AddRangeAsync(organizations);
        await _context.SaveChangesAsync();
        return new GenericResponse
        {
          Status = HttpStatusCode.OK.ToString(),
          Message = $"{organizations.Count} organizations added successfully",
          Data = organizations
        };
      }
      catch (Exception ex)
      {
        return new GenericResponse
        {
          Status = HttpStatusCode.InternalServerError.ToString(),
          Message = $"Error during fake user generation - {ex.Message}",
        };
      }
    }

    private static string GenerateEmail(string firstName, string lastName)
    {
      return $"{firstName.ToLower()}.{lastName.ToLower()}@gmail.com";
    }

    private static string GenerateUsername(string firstName)
    {
      return $"{firstName.ToLower()}123";
    }

    private static string GeneratePassword(string lastName)
    {
      // Combine first name and last name, convert to lowercase, and hash the result
      string combinedName = $"{lastName.ToLower()}123";
      return GetMd5Hash(combinedName);
    }

    private static string GetMd5Hash(string input)
    {
      // Calculate MD5 hash for the input string
      byte[] inputBytes = Encoding.UTF8.GetBytes(input);
      byte[] hashBytes = MD5.HashData(inputBytes);
      StringBuilder sb = new();
      for (int i = 0; i < hashBytes.Length; i++)
      {
          sb.Append(hashBytes[i].ToString("x2"));
      }
      return sb.ToString();
    }
  }
}