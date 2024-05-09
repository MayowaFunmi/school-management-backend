using SchoolManagementApi.Models.UserModels;

namespace SchoolManagementApi.DTOs
{
  public class OrganizationUserCount
  {
    public List<ApplicationUser> Users{ get; set; }
    public int UserCount { get; set; }
  }
}