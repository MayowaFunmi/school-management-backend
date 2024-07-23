using SchoolManagementApi.DTOs;
using SchoolManagementApi.Models.DocumentModels;
using SchoolManagementApi.Models.UserModels;

namespace SchoolManagementApi.Interfaces.Profiles
{
  public interface INonTeachingStaffInterface
  {
    Task<NonTeachingStaff> AddNonTeachingStaff(NonTeachingStaff nonTeachingStaff);
    Task<NonTeachingStaff> GetStaffById(string userId);
    Task<NonTeachingStaff> GetStaffByUniqueId(string uniqueId);
    Task<NonTeachingStaff> NonTeachingStaffExists(string userId);
    Task<string> OrganizationExists(string organizationUniqueId);
    Task<DocumentFile> UploadDocuments(string userId, List<string> filesList);
  }
}