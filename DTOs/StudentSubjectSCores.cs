namespace SchoolManagementApi.DTOs
{
  public class StudentSubjectSCores
  {
    public string Session { get; set; }
    public string Term { get; set; }
    public string SubjectName { get; set; }
    public string ClassArm { get; set; }
    public int CATest1 { get; set; } = 0;
    public int CATest2 { get; set; } = 0;
    public int CATest3 { get; set; } = 0;
    public int Exam { get; set; } = 0;
    public int Total => CATest1 + CATest2 + CATest3 + Exam;
  }
}