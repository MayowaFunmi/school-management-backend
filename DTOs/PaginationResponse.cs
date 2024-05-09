using SchoolManagementApi.Models;

namespace SchoolManagementApi.DTOs
{
  public class PaginationResponse
  {
    public IList<School> Schools { get; set; }
    public int TotalPages { get; set; }
    public int CurrentPage { get; set; }
    public int PagesLeft { get; set; }
  }
}