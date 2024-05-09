using SchoolManagementApi.Models.UserModels;

namespace SchoolManagementApi.DTOs
{
  public class UserpageResponse
  {
    public IList<ApplicationUser> Users { get; set; }
    public int TotalPages { get; set; }
    public int CurrentPage { get; set; }
    public int PagesLeft { get; set; }
  }
}