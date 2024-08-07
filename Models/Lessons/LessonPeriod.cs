using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SchoolManagementApi.DTOs;
using static SchoolManagementApi.Commands.LessonNotes.CreateLessonNotes;

namespace SchoolManagementApi.Models.Lessons
{
  public class LessonPeriod
  {
    [Key]
    public Guid Id { get; set; }
    public string LessonNotesId { get; set; } = string.Empty;
    public string SubTopic { get; set; } = string.Empty;
    public string BehaviouralObjective { get; set; } = string.Empty;
    public string PreviousKnowledge { get; set; } = string.Empty;
    public string Presentations { get; set; } = string.Empty; // steps and notes = contents of the lesson
    public string Evaluations { get; set; } = string.Empty;
    public string Conclusion { get; set; } = string.Empty;
    public string Assignment { get; set; } = string.Empty;
    public LessonNote LessonNotes { get; set; } = null!;
    public List<CustomField> CustomFields { get; set; } = [];
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
  }
}