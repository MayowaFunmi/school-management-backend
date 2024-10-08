using SchoolManagementApi.DTOs;
using SchoolManagementApi.Models.DocumentModels;

namespace SchoolManagementApi.Interfaces.Uploads
{
  public interface IFileService
  {
    Task<string> ProcessFilesAsync(UploadFilesDto filesDto);
    Task<string> ProcessFiles(IFormFile file);
  }
}