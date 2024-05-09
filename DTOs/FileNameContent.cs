using System.ComponentModel.DataAnnotations;

namespace SchoolManagementApi.DTOs
{
  public class FileNameContent
  {
    [Key]
    public Guid FileId { get; set; }
    public string? FileName { get; set; }
    public string? FileUrl { get; set; }
  }
}