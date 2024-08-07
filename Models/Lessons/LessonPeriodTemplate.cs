using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SchoolManagementApi.DTOs;

namespace SchoolManagementApi.Models.Lessons
{
  public class LessonPeriodTemplate
  {
    [Key]
    public Guid Id { get; set; }
    [ForeignKey("TemplateId")]
    public string TemplateId { get; set; } = string.Empty;
    public LessonNoteTemplate TemplateNote { get; set; } = null!;
    public List<CustomField> CustomFields { get; set; } = [];
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
  }
}