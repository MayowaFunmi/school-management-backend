using System.ComponentModel.DataAnnotations;

namespace SchoolManagementApi.DTOs
{
  public class CustomField
  {
    [Key]
    public Guid Id { get; set; }
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
  }
}