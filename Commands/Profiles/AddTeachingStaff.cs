using System.Net;
using MediatR;
using SchoolManagementApi.Data;
using SchoolManagementApi.DTOs;
using SchoolManagementApi.Intefaces.Profiles;
using SchoolManagementApi.Models.UserModels;
using static SchoolManagementApi.Constants.DictionaryMaps;

namespace SchoolManagementApi.Commands.Profiles
{
  public class AddTeachingStaff
  {
    public class AddTeachingStaffCommand : IRequest<GenericResponse>
    {
      public required string UserId { get; set; }
      public required string OrganizationUniqueId { get; set; }
      public string? Title { get; set; }
      public string? MiddleName { get; set; }
      public required string Gender { get; set; }
      public required DateTime DateOfBirth { get; set; }
      public required int Age { get; set; }
      public required string StateOfOrigin { get; set; }
      public required string LgaOfOrigin { get; set; }
      public required string Address { get; set; }
      public required string Religion { get; set; }
      public required string MaritalStatus { get; set; }
      public required string AboutMe { get; set; }
      public required string Designation { get; set; }
      public required int GradeLevel { get; set; }
      public required int Step { get; set; }
      public required DateTime FirstAppointment { get; set; }
      public required int YearsInService { get; set; }
      public required string Qualification { get; set; }
      public required string Discipline { get; set; }
      public required string CurrentPostingZoneId { get; set; }
      public required string CurrentPostingSchoolId { get; set; }
      public List<string>? PreviousSchoolsIds { get; set; }
      public string? PublishedWork { get; set; }
      public required string CurrentSubjectId { get; set; }
      public List<string>? OtherSubjects { get; set; } = [];
    }

    public class AddTeachingStaffHandler(ITeachingStaffInterface teachingStaffInterface, ApplicationDbContext context) : IRequestHandler<AddTeachingStaffCommand, GenericResponse>
    {
      private readonly ITeachingStaffInterface _teachingStaffInterface = teachingStaffInterface;
      private readonly ApplicationDbContext _context = context;

      public async Task<GenericResponse> Handle(AddTeachingStaffCommand request, CancellationToken cancellationToken)
      {
        try
        {
          // check if organization exists
          var organizationId = await _teachingStaffInterface.OrganizationExists(request.OrganizationUniqueId);
          if (string.IsNullOrEmpty(organizationId))
          {
            return new GenericResponse
            {
              Status = HttpStatusCode.OK.ToString(),
              Message = $"Organization not found"
            };
          }
          var teacherProfile = await _teachingStaffInterface.TeachingStaffExists(request.UserId);
          if (teacherProfile != null)
          {
            return new GenericResponse
            {
              Status = HttpStatusCode.OK.ToString(),
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
              user.OrganizationId = organizationId;
              await _context.SaveChangesAsync(cancellationToken);
            }
            
            return new GenericResponse
            {
              Status = HttpStatusCode.OK.ToString(),
              Message = "Teacher profile created sucessfully",
              Data = createdTeacher
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
            OrganizationUniqueId = request.OrganizationUniqueId,
            Title = TitleMap.TitleDictionary.TryGetValue(request.Title!, out string? value) ? value : "Mr",
            MiddleName = request.MiddleName!,
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