using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SchoolManagementApi.Data;
using SchoolManagementApi.DTOs;
using SchoolManagementApi.Intefaces.LoggerManager;
using SchoolManagementApi.Intefaces.Roles;
using SchoolManagementApi.Models.UserModels;
using WatchDog;

namespace SchoolManagementApi.Services.RoleServices
{
  public class RoleService(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager, ILoggerManager logger) : IRoleService
  {
    private readonly RoleManager<IdentityRole> _roleManager = roleManager;
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly ILoggerManager _logger = logger;

    public async Task<bool> AddRole(string roleName)
    {
      var role = new IdentityRole(roleName);
      var result = await _roleManager.CreateAsync(role);
      return result.Succeeded;
    }

    public async Task<bool> DeleteRole(string roleName)
    {
      var role = await _roleManager.FindByNameAsync(roleName);
      if (role != null)
      {
          var result = await _roleManager.DeleteAsync(role);
          return result.Succeeded;
      }
      return false;
    }

    public async Task<bool> EditRole(string roleName, string editedRole)
    {
      var role = await _roleManager.FindByNameAsync(roleName);
      if (role != null)
      {
        role.Name = editedRole; // Update the role's name
        var result = await _roleManager.UpdateAsync(role);
        return result.Succeeded;
      }
      return false;
    }

    public async Task<List<IdentityRole>> GetRoleList()
    {
      List<IdentityRole> roles = [];
      foreach (var role in _roleManager.Roles)
      {
        roles.Add(role);
      }
      return roles;
    }

    public List<IdentityRole> GetSelectedRoleList()
    {
      List<string> outRole = ["OrganizationAdmin, Admin, TeachingStaff", "NonTeachingStaff", "Parent", "Student"];
      List<IdentityRole> roles = [];
      foreach (var role in _roleManager.Roles)
      {
        if (!string.IsNullOrEmpty(role.Name) && outRole.Contains(role.Name))
          roles.Add(role);
      }
      return roles;
    }

    public async Task<List<OrganizationUsersWithRole>> GetOrganizationUserWithRoles(string organizationUniqueId, string roleName)
    {
      try
      {
        var users = await _userManager.Users
                                    .AsNoTracking()
                                    .Where(u => u.OrganizationId == organizationUniqueId)
                                    .ToListAsync();

        var usersInRole = new List<OrganizationUsersWithRole>();

        foreach (var user in users)
        {
          var roles = await _userManager.GetRolesAsync(user);
          if (roles.Contains(roleName))
          {
            var userWithRole = new OrganizationUsersWithRole
            {
              Id = user.Id,
              FirstName = user.FirstName,
              MiddleName = user.MiddleName,
              LastName = user.LastName,
              UniqueId = user.UniqueId
            };
            usersInRole.Add(userWithRole);
          }
        }

        return usersInRole;
      }
      catch (Exception ex)
      {
        _logger.LogError($"Error getting all organization role users- {ex.Message}");
        WatchLogger.LogError(ex.ToString(), $"Error getting all organization role users - {ex.Message}");
        throw;
      }
    }
  }
}