using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace SchoolManagementApi.Models
{
  public class SchoolTerm
  {
    [Key]
    public Guid SchoolTermId { get; set; }
    public string SchoolId { get; set; } = string.Empty;
    [ForeignKey("SchoolSessionId")]
    public string? SchoolSessionId { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime TermStarts { get; set; }
    public DateTime TermEnds { get; set; }
    [JsonIgnore]
    public SchoolSession? SchoolSession { get; set; }
  }
}