namespace SchoolManagementApi.DTOs
{
  public class ScoresDto
  {
    public string StudentId { get; set; } = string.Empty;
    public int CATest1 { get; set; } = 0;
    public int CATest2 { get; set; } = 0;
    public int CATest3 { get; set; } = 0;
    public int Exam { get; set; } = 0;
  }
}