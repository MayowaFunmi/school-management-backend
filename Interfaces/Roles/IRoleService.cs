using Microsoft.AspNetCore.Identity;
using SchoolManagementApi.DTOs;

namespace SchoolManagementApi.Interfaces.Roles
{
  public interface IRoleService
  {
    Task<bool> AddRole(string roleName);
    Task<bool> EditRole(string roleName, string editedRole);
    Task<bool> DeleteRole(string roleName);
    Task<List<IdentityRole>> GetRoleList();
    List<IdentityRole> GetSelectedRoleList();
    Task<List<OrganizationUsersWithRole>> GetOrganizationUserWithRoles(string organizationUniqueId, string roleName);
  }
}
