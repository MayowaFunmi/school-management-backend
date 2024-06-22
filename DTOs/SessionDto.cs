namespace SchoolManagementApi.DTOs
{
  public class SessionDto
  {
    public string Name { get; set; }
    public string SchoolId { get; set; } = string.Empty;
    public DateTime SessionStarts { get; set; }
    public DateTime SessionEnds { get; set; }
  }
}