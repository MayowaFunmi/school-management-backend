using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using SchoolManagementApi.Data;
using SchoolManagementApi.DTOs;
using SchoolManagementApi.Intefaces.LoggerManager;
using SchoolManagementApi.Intefaces.Uploads;
using SchoolManagementApi.Models.DocumentModels;
using WatchDog;

namespace SchoolManagementApi.Services.Uploads
{
  public class FileService(ILoggerManager logger, Cloudinary cloudinary) : IFileService
  {
    private readonly ILoggerManager _logger = logger;
    private readonly Cloudinary _cloudinary = cloudinary;

    public async Task<string> ProcessFiles(IFormFile file)
    {
      try
      {
        if (file == null || file.Length == 0)
          throw new ArgumentException("File is null or empty");
        var uploadParams = new ImageUploadParams()
        {
          File = new FileDescription(file.Name, file.OpenReadStream()),
          PublicId = Guid.NewGuid().ToString(), // Generate unique public ID
          UseFilename = true,
          UniqueFilename = false,
        };
        var uploadResult = await _cloudinary.UploadAsync(uploadParams);
        return uploadResult.Url.ToString();
      }
      catch (Exception ex)
      {
        _logger.LogError($"Error generating student class arms - {ex.Message}");
        WatchLogger.LogError(ex.ToString(), $"Error generating student class arms - {ex.Message}");
        throw;
      }
    }

    public async Task<string> ProcessFilesAsync(UploadFilesDto filesDto)
    {
      if (filesDto.FileContent == null || filesDto.FileContent.Length == 0)
        throw new ArgumentException("File is null or empty");

      var uploadParams = new ImageUploadParams()
      {
        File = new FileDescription(filesDto.FileContent.Name, filesDto.FileContent.OpenReadStream()),
        PublicId = Guid.NewGuid().ToString(), // Generate unique public ID
        UseFilename = true,
        UniqueFilename = false,
      };
      var uploadResult = await _cloudinary.UploadAsync(uploadParams);
      return uploadResult.Url.ToString();
    }
  }
}