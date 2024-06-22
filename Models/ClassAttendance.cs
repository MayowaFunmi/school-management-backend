using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SchoolManagementApi.Models.UserModels;

namespace SchoolManagementApi.Models
{
  public class ClassAttendance
  {
    [Key]
    public Guid AttendanceId { get; set; }
    [ForeignKey("StudentId")]
    public required string StudentId { get; set; }
    public required string ClassArmId { get; set; }
    public DateTime DayAndTime { get; set; }
    public string MorningAttendance { get; set; } = string.Empty;
    public string AfternoonAttendance { get; set; } = string.Empty;
    public bool IsMarked { get; set; } = false;
    public Student? Student { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsMarkedLate => CreatedAt.Date > DayAndTime.Date;
  }
}