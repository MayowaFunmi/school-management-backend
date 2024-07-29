using System.Net;
using MediatR;
using SchoolManagementApi.Data;
using SchoolManagementApi.DTOs;
using SchoolManagementApi.Interfaces.Profiles;
using SchoolManagementApi.Models.UserModels;
using static SchoolManagementApi.Constants.DictionaryMaps;

namespace SchoolManagementApi.Commands.Profiles
{
  public class AddTeachingStaff
  {
    public class AddTeachingStaffCommand : IRequest<GenericResponse>
    {
      public string UserId { get; set; } = string.Empty;
      public string Title { get; set; } = string.Empty;
      public string MiddleName { get; set; } = string.Empty;
      public string Gender { get; set; } = string.Empty;
      public DateTime DateOfBirth { get; set; }
      public int Age { get; set; }
      public string StateOfOrigin { get; set; } = string.Empty;
      public string LgaOfOrigin { get; set; } = string.Empty;
      public string Address { get; set; } = string.Empty;
      public string Religion { get; set; } = string.Empty;
      public string MaritalStatus { get; set; } = string.Empty;
      public string AboutMe { get; set; } = string.Empty;
      public string Designation { get; set; } = string.Empty;
      public int GradeLevel { get; set; }
      public int Step { get; set; }
      public DateTime FirstAppointment { get; set; }
      public int YearsInService { get; set; }
      public string Qualification { get; set; } = string.Empty;
      public string Discipline { get; set; } = string.Empty;
      public string CurrentPostingZoneId { get; set; } = string.Empty;
      public string CurrentPostingSchoolId { get; set; } = string.Empty;
      public List<string> PreviousSchoolsIds { get; set; } = [];
      public string PublishedWork { get; set; } = string.Empty;
      public string CurrentSubjectId { get; set; } = string.Empty;
      public List<string> OtherSubjects { get; set; } = [];
    }

    public class AddTeachingStaffHandler(ITeachingStaffInterface teachingStaffInterface, ApplicationDbContext context) : IRequestHandler<AddTeachingStaffCommand, GenericResponse>
    {
      private readonly ITeachingStaffInterface _teachingStaffInterface = teachingStaffInterface;
      private readonly ApplicationDbContext _context = context;

      public async Task<GenericResponse> Handle(AddTeachingStaffCommand request, CancellationToken cancellationToken)
      {
        try
        {
          var teacherProfileExists = await _teachingStaffInterface.TeachingStaffExists(request.UserId);
          if (teacherProfileExists)
          {
            return new GenericResponse
            {
              Status = HttpStatusCode.Conflict.ToString(),
              Message = $"Profile already exists"
            };
          }
          var teacher = MapToTeachingStaff(request);
          var createdTeacher = await _teachingStaffInterface.AddTeachingStaff(teacher);
          if (createdTeacher != null)
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
              Message = "Teacher profile created sucessfully",
            };
          }
          return new GenericResponse
          {
            Status = HttpStatusCode.BadRequest.ToString(),
            Message = "Failed to add teacher profile",
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

      private static TeachingStaff MapToTeachingStaff(AddTeachingStaffCommand request)
      {
        return new TeachingStaff
          {
            UserId = request.UserId,
            Title = TitleMap.TitleDictionary.TryGetValue(request.Title!, out string? value) ? value : "Mr",
            Gender = TitleMap.GenderDictionary.TryGetValue(request.Gender!, out string? GenderValue) ? GenderValue : "Male",
            DateOfBirth = request.DateOfBirth,
            Age = request.Age,
            StateOfOrigin = request.StateOfOrigin,
            LgaOfOrigin = request.LgaOfOrigin,
            Address = request.Address,
            Religion = TitleMap.ReligionDictionary.TryGetValue(request.Religion!, out string? ReligionValue) ? ReligionValue : "Christianity",
            MaritalStatus = TitleMap.MaritalDictionary.TryGetValue(request.MaritalStatus!, out string? MaritalValue) ? MaritalValue : "Married",
            AboutMe = request.AboutMe,
            Designation = TitleMap.DesignationDictionary.TryGetValue(request.Designation!, out string? DesignationValue) ? DesignationValue : "ClassTeacher",
            GradeLevel = request.GradeLevel,
            Step = request.Step,
            FirstAppointment = request.FirstAppointment,
            YearsInService = request.YearsInService,
            Qualification = TitleMap.QualificationDictionary.TryGetValue(request.Qualification!, out string? QualificationValue) ? QualificationValue : "BEd",
            Discipline = request.Discipline,
            CurrentPostingZoneId = Guid.Parse(request.CurrentPostingZoneId),
            CurrentPostingSchoolId = Guid.Parse(request.CurrentPostingSchoolId),
            PreviousSchoolsIds = request.PreviousSchoolsIds,
            PublishedWork = request.PublishedWork,
            CurrentSubjectId = Guid.Parse(request.CurrentSubjectId),
            OtherSubjects = request.OtherSubjects
          };
      }
    }
  }
}