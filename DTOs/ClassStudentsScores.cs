using SchoolManagementApi.Models.UserModels;

namespace SchoolManagementApi.DTOs
{
  public class ClassStudentsScores
  {
    public string StudentId { get; set; } = string.Empty;
    public string SessionName { get; set; } = string.Empty;
    public string TermName { get; set; } = string.Empty;
    public StudentData StudentData { get; set; }
    public int CATest1 { get; set; } = 0;
    public int CATest2 { get; set; } = 0;
    public int CATest3 { get; set; } = 0;
    public int Exam { get; set; } = 0;
    public int Total => CATest1 + CATest2 + CATest3 + Exam;
  }
}