using System.Net;
using MediatR;
using SchoolManagementApi.Data;
using SchoolManagementApi.DTOs;
using SchoolManagementApi.Intefaces.Profiles;
using SchoolManagementApi.Models.UserModels;
using static SchoolManagementApi.Constants.DictionaryMaps;

namespace SchoolManagementApi.Commands.Profiles
{
  public class AddNonTeachingStaff
  {
    public class AddNonTeachingStaffCommand : IRequest<GenericResponse>
    {
      public string UserId { get; set; } = string.Empty;
      public string OrganizationUniqueId { get; set; } = string.Empty;
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
    }

    public class AddNonTeachingStaffHandler(INonTeachingStaffInterface nonTeachingStaffInterface, ApplicationDbContext context) : IRequestHandler<AddNonTeachingStaffCommand, GenericResponse>
    {
      private readonly INonTeachingStaffInterface _nonTeachingStaffInterface = nonTeachingStaffInterface;
      private readonly ApplicationDbContext _context = context;

      public async Task<GenericResponse> Handle(AddNonTeachingStaffCommand request, CancellationToken cancellationToken)
      {
        try
        {
          var checkStaffExists = await _nonTeachingStaffInterface.NonTeachingStaffExists(request.UserId);
          if (checkStaffExists != null)
          {
            return new GenericResponse
            {
              Status = HttpStatusCode.OK.ToString(),
              Message = $"Profile already exists"
            };
          }

          var staff = MapToNonTeachingStaff(request);
          var createdStaff = await _nonTeachingStaffInterface.AddNonTeachingStaff(staff);
          if (createdStaff != null)
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
              Message = "Non Teaching Staff profile created sucessfully",
              Data = createdStaff
            };
          }
          return new GenericResponse
          {
            Status = HttpStatusCode.BadRequest.ToString(),
            Message = "Failed to add staff profile",
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

      private static NonTeachingStaff MapToNonTeachingStaff(AddNonTeachingStaffCommand request)
      {
        return new NonTeachingStaff
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
          FirstAppointment = request.FirstAppointment,
          YearsInService = request.YearsInService,
          Qualification = TitleMap.QualificationDictionary.TryGetValue(request.Qualification!, out string? QualificationValue) ? QualificationValue : "BEd",
          Discipline = request.Discipline,
          CurrentPostingSchoolId = Guid.Parse(request.CurrentPostingSchoolId),
          PreviousSchoolsIds = request.PreviousSchoolsIds,
          CurrentPostingZoneId = Guid.Parse(request.CurrentPostingZoneId),
        };
      }
    }
  }
}