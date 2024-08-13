using System.Net;
using MediatR;
using SchoolManagementApi.Data;
using SchoolManagementApi.DTOs;
using SchoolManagementApi.Interfaces.Profiles;
using SchoolManagementApi.Models.UserModels;
using static SchoolManagementApi.Constants.DictionaryMaps;

namespace SchoolManagementApi.Commands.Profiles
{
  public class AddParent
  {
    public class AddParentCommand : IRequest<GenericResponse>
    {
      public string UserId { get; set; } = string.Empty;
      public string SchoolUniqueId { get; set; } = string.Empty;
      public string Title { get; set; } = string.Empty;
      public string Gender { get; set; } = string.Empty;
      public string RelationshipType { get; set; } = string.Empty;
      public string Address { get; set; } = string.Empty;
      public DateTime DateOfBirth { get; set; }
      public int Age { get; set; }
      public string Religion { get; set; } = string.Empty;
      public string MaritalStatus { get; set; } = string.Empty;
      public string StateOfOrigin { get; set; } = string.Empty;
      public string LgaOfOrigin { get; set; } = string.Empty;
      public string LgaOfResidence { get; set; } = string.Empty;
      public string Occupation { get; set; } = string.Empty;
    }

    public class AddparentHandler(IParentService parentService, ApplicationDbContext context) : IRequestHandler<AddParentCommand, GenericResponse>
    {
      private readonly IParentService _parentService = parentService;
      private readonly ApplicationDbContext _context = context;

      public async Task<GenericResponse> Handle(AddParentCommand request, CancellationToken cancellationToken)
      {
        try
        {
          var parent = await _parentService.ParentProfileExists(request.UserId);
          if (parent)
          {
            return new GenericResponse
            {
              Status = HttpStatusCode.Conflict.ToString(),
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
          SchoolUniqueId = request.SchoolUniqueId,
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