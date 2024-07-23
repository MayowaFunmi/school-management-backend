using System.Net;
using MediatR;
using SchoolManagementApi.Data;
using SchoolManagementApi.DTOs;
using SchoolManagementApi.Interfaces.Profiles;
using SchoolManagementApi.Models.UserModels;
using static SchoolManagementApi.Constants.DictionaryMaps;

namespace SchoolManagementApi.Commands.Profiles
{
  public class AddStudent
  {
    public class AddStudentCommand : IRequest<GenericResponse>
    {
      public string UserId { get; set; } = string.Empty;
      public string MiddleName { get; set; } = string.Empty;
      public string AdmissionNumber { get; set; } = string.Empty;
      public string AdmissionYear { get; set; } = string.Empty;
      public string SchoolZoneId { get; set; } = string.Empty;
      public string CurrentSchoolId { get; set; } = string.Empty;
      public string DepartmentId { get; set; } = string.Empty;
      public string StudentClassId { get; set; } = string.Empty;
      public List<string> PreviousSchoolsIds { get; set; } = [];
      public string Gender { get; set; } = string.Empty;
      public DateTime DateOfBirth { get; set; }
      public int Age { get; set; }
      public string Address { get; set; } = string.Empty;
      public string Religion { get; set; } = string.Empty;
      public string ParentId { get; set; } = string.Empty;
    }

    public class AddStudentHandler(ApplicationDbContext context, IStudentService studentService) : IRequestHandler<AddStudentCommand, GenericResponse>
    {
      private readonly ApplicationDbContext _context = context;
      private readonly IStudentService _studentService = studentService;

      public async Task<GenericResponse> Handle(AddStudentCommand request, CancellationToken cancellationToken)
      {
        try
        {
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