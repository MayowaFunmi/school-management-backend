using SchoolManagementApi.DTOs;
using SchoolManagementApi.Models;

namespace SchoolManagementApi.Intefaces.Admin
{
  public interface IAdminService
  {
    Task<List<UserWithRoles>> GetAllUsersWithRoles();
    Task<OrganizationUserCount> OrganizationUsersByRole(string organizationId, string roleName, int page, int pageSize);
    Task<UserWithRoles> GetUserByUniqueId(string uniqueId, string? userRole);
    Task<List<Subject>> GetSubjectsInSchool();
  }
}