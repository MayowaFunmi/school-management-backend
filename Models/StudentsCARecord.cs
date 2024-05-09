using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolManagementApi.Models
{
  public class StudentsCARecord
  {
    [Key]
    public Guid TestId { get; set; }
    //public string SchoolId { get; set; } = string.Empty;
    public string ClassId { get; set; } = string.Empty; // class arm id
    public string SubjectId { get; set; } = string.Empty;
    [ForeignKey("SchoolSessionId")]
    public string SchoolSessionId { get; set; }
    public SchoolSession SchoolSession { get; set; }
    public string Term { get; set; } = string.Empty;
    public List<StudentsScores> StudentsScores { get; set; } = [];
  }
}