using SchoolManagementApi.Models;
using SchoolManagementApi.Models.UserModels;

namespace SchoolManagementApi.Intefaces.Admin
{
  public interface IOrganizationService
  {
    Task<Organization> CreateOrganization(Organization organization);
    Task<List<Organization>> RetrieveAdminOrganizations(string adminId);
    Task<List<Organization>> AllOrganizations();
    Task<List<TeachingStaff>> GetAllTeachersInOrganization(string organizationId, int page, int pageSize);

  }
}