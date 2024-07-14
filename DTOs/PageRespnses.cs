using SchoolManagementApi.Models.UserModels;

namespace SchoolManagementApi.DTOs
{
  public class PageRespnses
  {
    public class TeachersPageResponse
    {
      public IList<TeachingStaff> Teachers { get; set; } = [];
      public int TotalPages { get; set; }
      public int CurrentPage { get; set; }
      public int PagesLeft { get; set; }
    }

    public class NonTeachersPageResponse
    {
      public IList<NonTeachingStaff> NonTeachers { get; set; } = [];
      public int TotalPages { get; set; }
      public int CurrentPage { get; set; }
      public int PagesLeft { get; set; }
    }

    public class ParentsPageResponse
    {
      public IList<Parent> Parents { get; set; } = [];
      public int TotalPages { get; set; }
      public int CurrentPage { get; set; }
      public int PagesLeft { get; set; }
    }

    public class StudentsPageResponse
    {
      public IList<Student> Students { get; set; } = [];
      public int TotalPages { get; set; }
      public int CurrentPage { get; set; }
      public int PagesLeft { get; set; }
    }
  }
}