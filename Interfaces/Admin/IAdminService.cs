using SchoolManagementApi.DTOs;
using SchoolManagementApi.Models;
using SchoolManagementApi.Models.UserModels;

namespace SchoolManagementApi.Interfaces.Admin
{
  public interface IAdminService
  {
    Task<List<UserWithRoles>> GetAllUsersWithRoles();
    Task<OrganizationUserCount> OrganizationUsersByRole(string organizationId, string roleName, int page, int pageSize);
    Task<UserWithRoles> GetUserByUniqueId(string uniqueId, string? userRole);
    Task<ApplicationUser> GetUserById(string userId);
    Task<List<Subject>> GetSubjectsInSchool();
  }
}