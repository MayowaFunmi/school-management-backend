using System.Net;
using MediatR;
using SchoolManagementApi.DTOs;
using SchoolManagementApi.Intefaces.Admin;

namespace SchoolManagementApi.Commands.Students
{
  public class AddStudentsCA
  {
    public class AddStudentsCACommand : IRequest<GenericResponse>
    {
      public string ClassId { get; set; } = string.Empty; // class arm id
      public string SubjectId { get; set; } = string.Empty;
      public string SessionId { get; set; } = string.Empty;
      public string Term { get; set; } = string.Empty;
      public List<ScoresDto> StudentsScores { get; set; } = [];
    }

    public class AddStudentsCAHandler(IStudentClassServices studentClassServices) : IRequestHandler<AddStudentsCACommand, GenericResponse>
    {
      private readonly IStudentClassServices _studentClassServices = studentClassServices;

      public async Task<GenericResponse> Handle(AddStudentsCACommand request, CancellationToken cancellationToken)
      {
        try
        {
          var scores = await _studentClassServices.AddStudentsCATest(request);
          if (scores != null)
          {
            return new GenericResponse
            {
              Status = HttpStatusCode.OK.ToString(),
              Message = "Scores recorded for students successfully",
              Data = scores
            };
          }
          return new GenericResponse
          {
            Status = HttpStatusCode.BadRequest.ToString(),
            Message = "Failed to record Scores for students",
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
    }
  }
}