using Microsoft.AspNetCore.Identity;
using SchoolManagementApi.Intefaces.Roles;

namespace SchoolManagementApi.Services.RoleServices
{
  public class RoleService(RoleManager<IdentityRole> roleManager) : IRoleService
  {
    private readonly RoleManager<IdentityRole> _roleManager = roleManager;

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

    public async Task<List<IdentityRole>> GetSelectedRoleList()
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
  }
}