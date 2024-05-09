namespace SchoolManagementApi.DTOs
{
  public class UploadFilesDto
  {
    public required string FileName { get; set; }
    public IFormFile? FileContent { get; set; }
  }
}