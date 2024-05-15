using System.ComponentModel.DataAnnotations;

namespace SchoolManagementApi.Models
{
  public class SchoolSession
  {
    [Key]
    public Guid SchoolSessionId { get; set; }
    //public string SchoolId { get; set; } = string.Empty;
    
    public string Name { get; set; } = string.Empty;
    public DateTime SessionStarts { get; set; }
    public DateTime SessionEnds { get; set; }
    public List<SchoolTerm> SchoolTerms { get; set; } = [];
  }
}