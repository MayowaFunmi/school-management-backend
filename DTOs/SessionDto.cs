namespace SchoolManagementApi.DTOs
{
  public class SessionDto
  {
    public string Name { get; set; } = string.Empty;
    public string SchoolUniqueId { get; set; } = string.Empty;
    public DateTime SessionStarts { get; set; }
    public DateTime SessionEnds { get; set; }
  }
}