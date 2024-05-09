using System.Net;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolManagementApi.Data;
using SchoolManagementApi.DTOs;
using SchoolManagementApi.Intefaces.Profiles;
using SchoolManagementApi.Intefaces.Uploads;

namespace SchoolManagementApi.Commands.Uploads
{
  public class UploadProfilePicture
  {
    public class UploadProfilePictureCommand : IRequest<GenericResponse>
    {
      public string? StaffId { get; set; }
      public required IFormFile ProfilePicture { get; set; }
    }

    public class UploadProfilePictureHandler(ApplicationDbContext context, IFileService fileService, ITeachingStaffInterface teachingStaffInterface) : IRequestHandler<UploadProfilePictureCommand, GenericResponse>
    {
      private readonly ApplicationDbContext _context = context;
      private readonly IFileService _fileService = fileService;
      private readonly ITeachingStaffInterface _teachingStaffInterface = teachingStaffInterface;

      public async Task<GenericResponse> Handle(UploadProfilePictureCommand request, CancellationToken cancellationToken)
      {
        try
        {
          // check if user profile exist
          var staff = await _context.TeachingStaffs.FirstOrDefaultAsync(t => t.UserId == request.StaffId);
          if (staff == null)
          {
            return new GenericResponse
            {
              Status = HttpStatusCode.OK.ToString(),
              Message = $"User does not have a profile yet"
            };
          }
          else
          {
            if (!string.IsNullOrEmpty(staff.ProfilePicture))
            {
              return new GenericResponse
              {
                Status = HttpStatusCode.OK.ToString(),
                Message = $"User already has a profile picture uploaded"
              };
            }
            else
            {
              var profilePictureUrl = await _fileService.ProcessFiles(request.ProfilePicture);
              Console.WriteLine("picture url = "+ profilePictureUrl);
              var user = _context.Users.FirstOrDefault(u => u.Id == request.StaffId);          
      
              if (user != null && !string.IsNullOrEmpty(profilePictureUrl))
              {
                staff.ProfilePicture = profilePictureUrl;
                user.PercentageCompleted += 10;
                await _context.SaveChangesAsync(cancellationToken);
                return new GenericResponse
                {
                  Status = HttpStatusCode.OK.ToString(),
                  Message = "Profile picture uploaded successfully",
                };
              }
              return new GenericResponse
              {
                Status = HttpStatusCode.OK.ToString(),
                Message = "Failed to upload profile picture",
              };
            }
          }
        }
        catch (Exception ex)
        {
          return new GenericResponse
          {
            Status = HttpStatusCode.InternalServerError.ToString(),
            Message = $"An internal server error occurred - {ex.Message}",
          };
        }
      }
    }
  }
}