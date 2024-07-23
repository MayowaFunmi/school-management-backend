using System.Net;
using MediatR;
using SchoolManagementApi.DTOs;
using SchoolManagementApi.Interfaces.Profiles;
using SchoolManagementApi.Interfaces.Uploads;

namespace SchoolManagementApi.Commands.Uploads
{
  public class UploadFiles
  {
    public class UploadFilesCommand : IRequest<GenericResponse>
    {
      public string? StaffId { get; set; }
      public List<IFormFile>? Files { get; set; }
    }

    public class UploadFilesHandler(ITeachingStaffInterface teachingStaffInterface, IFileService fileService) : IRequestHandler<UploadFilesCommand, GenericResponse>
    {
      private readonly ITeachingStaffInterface _teachingStaffInterface = teachingStaffInterface;
      private readonly IFileService _fileService = fileService;

      public async Task<GenericResponse> Handle(UploadFilesCommand request, CancellationToken cancellationToken)
      {
        try
        {
          var filesList = new List<string>();
          foreach (var file in request.Files!)
          {
            var url = await _fileService.ProcessFiles(file);
            filesList.Add(url);
          }
          var savedDocs = await _teachingStaffInterface.UploadDocuments(request.StaffId!, filesList);
          if (savedDocs != null)
          {
            return new GenericResponse
            {
              Status = HttpStatusCode.OK.ToString(),
              Message = "Documents and files saved successfully",
              Data = savedDocs
            };
          }
          return new GenericResponse
          {
            Status = HttpStatusCode.BadRequest.ToString(),
            Message = "Failed to save documents and files",
          };
        }
        catch (Exception ex)
        {
          return new GenericResponse
          {
            Status = HttpStatusCode.InternalServerError.ToString(),
            Message = $"An internal server error occurred - {ex.Message}"
          };
        }
      }
    }
  }
}