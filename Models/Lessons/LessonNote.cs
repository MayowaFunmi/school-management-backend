using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SchoolManagementApi.DTOs;
using SchoolManagementApi.Models.UserModels;

namespace SchoolManagementApi.Models.Lessons
{
  public class LessonNote
  {
    [Key]
    public Guid Id { get; set; }
    [ForeignKey("TeacherId")]
    public string TeacherId { get; set; } = string.Empty;
    public ApplicationUser Teacher { get; set; } = null!;
    public int WeekNumber { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    [ForeignKey("SubjectId")]
    public string SubjectId { get; set; } = string.Empty;
    public Subject Subject { get; set; } = null!;
    [ForeignKey("ClassArmId")]
    public string ClassArmId { get; set; } = string.Empty;
    public ClassArms ClassArm { get; set; } = null!;
    public string Topic { get; set; } = string.Empty;
    public string SubTopic { get; set; } = string.Empty;
    public string ReferenceBook { get; set; } = string.Empty;
    public string InstructionalAid { get; set; } = string.Empty;
    public bool IsTemplate { get; set; } = false;
    public List<LessonPeriod> Periods { get; set; } = [];
    public List<CustomField> CustomFields { get; set; } = [];
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
  }
}