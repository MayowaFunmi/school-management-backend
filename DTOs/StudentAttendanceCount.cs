namespace SchoolManagementApi.DTOs
{
  public class StudentAttendanceCount
  {
    public string StudentUniqueId { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string ClassArmId { get; set; } = string.Empty;
    public int Count { get; set; }
  }
}