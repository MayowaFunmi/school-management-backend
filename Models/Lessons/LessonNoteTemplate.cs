using System.ComponentModel.DataAnnotations;
using SchoolManagementApi.DTOs;

namespace SchoolManagementApi.Models.Lessons
{
  public class LessonNoteTemplate
  {
    [Key]
    public Guid Id { get; set; }
    public string TemplateName { get; set; } = string.Empty;
    public List<LessonPeriodTemplate> TemplatePeriods { get; set; } = [];
    public List<CustomField> CustomFields { get; set; } = [];
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
  }
}