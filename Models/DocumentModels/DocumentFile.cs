using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SchoolManagementApi.DTOs;
using SchoolManagementApi.Models.UserModels;

namespace SchoolManagementApi.Models.DocumentModels
{
  public class DocumentFile
  {
    [Key]
    public Guid DocumenetId { get; set; }
    public List<string> FilesUrls { get; set; } = [];

    [ForeignKey("UserId")]
    public string UserId { get; set; } = string.Empty;
    public virtual ApplicationUser? User { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; }
  }
}