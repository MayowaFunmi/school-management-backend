namespace SchoolManagementApi.DTOs
{
  public class TermDto
  {
    public string TermName { get; set; }
    public DateTime TermStarts { get; set; }
    public DateTime TermEnds { get; set; }
  }

  public class SchoolTermDto
  {
    public string SchoolSessionId { get; set; }
    public List<TermDto> SchoolTerms { get; set; }
  }
}