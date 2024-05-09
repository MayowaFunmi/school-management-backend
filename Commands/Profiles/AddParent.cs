using System.Net;
using MediatR;
using SchoolManagementApi.Data;
using SchoolManagementApi.DTOs;
using SchoolManagementApi.Intefaces.Profiles;
using SchoolManagementApi.Models.UserModels;
using static SchoolManagementApi.Constants.DictionaryMaps;

namespace SchoolManagementApi.Commands.Profiles
{
  public class AddParent
  {
    public class AddParentCommand : IRequest<GenericResponse>
    {
      public required string UserId { get; set; }
      public required string OrganizationUniqueId { get; set; }
      public required string StudentSchoolId { get; set; }
      public required string Title { get; set; }
      public required string Gender { get; set; }
      public required string RelationshipType { get; set; }
      public required string Address { get; set; }
      public required DateTime DateOfBirth { get; set; }
      public required int Age { get; set; }
      public required string Religion { get; set; }
      public required string MaritalStatus { get; set; }
      public required string StateOfOrigin { get; set; }
      public required string LgaOfOrigin { get; set; }
      public required string LgaOfResidence { get; set; }
      public required string Occupation { get; set; }
    }

    public class AddparentHandler(IParentService parentService, ApplicationDbContext context) : IRequestHandler<AddParentCommand, GenericResponse>
    {
      private readonly IParentService _parentService = parentService;
      private readonly ApplicationDbContext _context = context;

      public async Task<GenericResponse> Handle(AddParentCommand request, CancellationToken cancellationToken)
      {
        try
        {
          var organizationId = await _parentService.OrganizationExists(request.OrganizationUniqueId);
          if (string.IsNullOrEmpty(organizationId))
          {
            return new GenericResponse
            {
              Status = HttpStatusCode.OK.ToString(),
              Message = $"Organization not found"
            };
          }

          var parent = await _parentService.ParentProfileExists(request.UserId);
          if (parent)
          {
            return new GenericResponse
            {
              Status = HttpStatusCode.OK.ToString(),
              Message = $"parent profile already exists"
            };
          }
          var parentData = MapToParent(request);
          var profile = await _parentService.AddParentProfile(parentData);
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
              Message = "Parent profile created sucessfully",
              Data = profile
            };
          }
          return new GenericResponse
          {
            Status = HttpStatusCode.BadRequest.ToString(),
            Message = "Failed to add parent profile",
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

      private static Parent MapToParent(AddParentCommand request)
      {
        return new Parent 
        {
          UserId = request.UserId,
          OrganizationUniqueId = request.OrganizationUniqueId,
          StudentSchoolId = Guid.Parse(request.StudentSchoolId),
          Title = TitleMap.TitleDictionary.TryGetValue(request.Title!, out string? value) ? value : "Mr",
          Gender = TitleMap.GenderDictionary.TryGetValue(request.Gender!, out string? GenderValue) ? GenderValue : "Male",
          RelationshipType = TitleMap.RelationshipDictionary.TryGetValue(request.RelationshipType!, out string? RelationshipValue) ? RelationshipValue : "father",
          Address = request.Address,
          DateOfBirth = request.DateOfBirth,
          Age = request.Age,
          Religion = TitleMap.ReligionDictionary.TryGetValue(request.Religion!, out string? ReligionValue) ? ReligionValue : "Christianity",
          MaritalStatus = TitleMap.MaritalDictionary.TryGetValue(request.MaritalStatus!, out string? MaritalValue) ? MaritalValue : "Married",
          StateOfOrigin = request.StateOfOrigin,
          LgaOfOrigin = request.LgaOfOrigin,
          LgaOfResidence = request.LgaOfResidence,
          Occupation = request.Occupation
        };
      }
    }
  }
}