using SchoolManagementApi.Models.UserModels;

namespace SchoolManagementApi.Interfaces.Profiles
{
  public interface IParentService
  {

    Task<string> OrganizationExists(string organizationUniqueId);
    Task<Parent> AddParentProfile(Parent parent);
    Task<bool> ParentProfileExists(string userId);
    Task<Parent> GetParentById(string parentId);
    Task<Parent> GetParentByUniqueId(string parentUniqueId);
    Task<Parent> GetStudentParent(string studentId);
    Task<List<Parent>> GetParentInSchool(string schoolId);
  }
}