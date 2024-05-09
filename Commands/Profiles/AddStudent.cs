using System.Net;
using MediatR;
using SchoolManagementApi.Data;
using SchoolManagementApi.DTOs;
using SchoolManagementApi.Intefaces.Profiles;
using SchoolManagementApi.Models.UserModels;
using static SchoolManagementApi.Constants.DictionaryMaps;

namespace SchoolManagementApi.Commands.Profiles
{
  public class AddStudent
  {
    public class AddStudentCommand : IRequest<GenericResponse>
    {
      public required string UserId { get; set; }
      public required string OrganizationUniqueId { get; set; }
      public required string MiddleName { get; set; }
      public required string AdmissionNumber { get; set; }
      public required string AdmissionYear { get; set; }
      public required string SchoolZoneId { get; set; }
      public required string CurrentSchoolId { get; set; }
      public required string DepartmentId { get; set; }
      public required string StudentClassId { get; set; }
      public List<string> PreviousSchoolsIds { get; set; } = [];
      public required string Gender { get; set; }      
      public required DateTime DateOfBirth { get; set; }
      public required int Age { get; set; }
      public required string Address { get; set; }
      public required string Religion { get; set; }
      public required string ParentId { get; set; }
    }

    public class AddStudentHandler(ApplicationDbContext context, IStudentService studentService) : IRequestHandler<AddStudentCommand, GenericResponse>
    {
      private readonly ApplicationDbContext _context = context;
      private readonly IStudentService _studentService = studentService;

      public async Task<GenericResponse> Handle(AddStudentCommand request, CancellationToken cancellationToken)
      {
        try
        {
          var organizationId = await _studentService.OrganizationExists(request.OrganizationUniqueId);
          if (string.IsNullOrEmpty(organizationId))
          {
            return new GenericResponse
            {
              Status = HttpStatusCode.OK.ToString(),
              Message = $"Organization not found"
            };
          }

          var student = await _studentService.StudentProfileExists(request.UserId);
          if (student)
          {
            return new GenericResponse
            {
              Status = HttpStatusCode.OK.ToString(),
              Message = $"studnet profile already exists"
            };
          }
          var studentData = MapToStudent(request);
          var profile = await _studentService.AddStudentProfile(studentData);
          if (profile != null)
          {
            var user = _context.Users.FirstOrDefault(u => u.Id == request.UserId);
            if (user != null)
            {
              user.PercentageCompleted += 30;
              user.OrganizationId = organizationId;
              await _context.SaveChangesAsync(cancellationToken);
            }
            
            return new GenericResponse
            {
              Status = HttpStatusCode.OK.ToString(),
              Message = "student profile created sucessfully",
              Data = profile
            };
          }
          return new GenericResponse
          {
            Status = HttpStatusCode.BadRequest.ToString(),
            Message = "Failed to add student profile",
          };
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

      private static Student MapToStudent(AddStudentCommand request)
      {
        return new Student
        {
          UserId = request.UserId,
          OrganizationUniqueId = request.OrganizationUniqueId,
          MiddleName = request.MiddleName,
          AdmissionNumber = request.AdmissionNumber,
          AdmissionYear = request.AdmissionYear,
          SchoolZoneId = Guid.Parse(request.SchoolZoneId),
          CurrentSchoolId = Guid.Parse(request.CurrentSchoolId),
          DepartmentId = Guid.Parse(request.DepartmentId),
          StudentClassId = Guid.Parse(request.StudentClassId),
          PreviousSchoolsIds = request.PreviousSchoolsIds,
          Gender = TitleMap.GenderDictionary.TryGetValue(request.Gender!, out string? GenderValue) ? GenderValue : "Male",
          DateOfBirth = request.DateOfBirth,
          Address = request.Address,
          Age = request.Age,
          Religion = TitleMap.ReligionDictionary.TryGetValue(request.Religion!, out string? ReligionValue) ? ReligionValue : "Christianity",
          ParentId = Guid.Parse(request.ParentId),
        };
      }
    }
  }
}