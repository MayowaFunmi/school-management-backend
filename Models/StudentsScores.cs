using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SchoolManagementApi.Models.UserModels;

namespace SchoolManagementApi.Models
{
  public class StudentsScores
  {
    [Key]
    public Guid StudentScoresId { get; set; }
    [ForeignKey("StudentsCARecordId")]
    public string StudentsCARecordId { get; set; } = string.Empty;
    [ForeignKey("StudentId")]
    public string StudentId { get; set; } = string.Empty;
    public int CATest1 { get; set; } = 0;
    public int CATest2 { get; set; } = 0;
    public int CATest3 { get; set; } = 0;
    public int Exam { get; set; } = 0;
    public int Total => CATest1 + CATest2 + CATest3 + Exam;
    public Student? Student { get; set; }
    public StudentsCARecord? StudentsCARecord { get; set; }
  }
}