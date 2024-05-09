using System.Net;
using MediatR;
using SchoolManagementApi.DTOs;
using SchoolManagementApi.Intefaces.Admin;
using SchoolManagementApi.Intefaces.LoggerManager;
using WatchDog;

namespace SchoolManagementApi.Queries.Students
{
  public class GetStudentsResults
  {
    public class GetStudentsResultsQuery : IRequest<GenericResponse>
    {
      public string ClassId { get; set; } = string.Empty;
      public string SubjectId { get; set; } = string.Empty;
      public string SessionId { get; set; } = string.Empty;
      public string Term { get; set; } = string.Empty;
    }

    public class GetStudentsResultsHandler(IStudentClassServices studentClassServices, ILoggerManager logger) : IRequestHandler<GetStudentsResultsQuery, GenericResponse>
    {
      private readonly IStudentClassServices _studentClassServices = studentClassServices;
      private readonly ILoggerManager _logger = logger;
      public async Task<GenericResponse> Handle(GetStudentsResultsQuery request, CancellationToken cancellationToken)
      {
        try
        {
          var result = await _studentClassServices.GetClassStudentsScores(request.SessionId, request.ClassId, request.SubjectId, request.Term);
          if (result.Count != 0)
          {
            return new GenericResponse
            {
              Status = HttpStatusCode.OK.ToString(),
              Message = "Results retrieved successfully",
              Data = result
            };
          }
          return new GenericResponse
          {
            Status = HttpStatusCode.OK.ToString(),
            Message = "No Result Found",
          };
        }
        catch (Exception ex)
      {
        _logger.LogError($"Error getting results - {ex.Message}");
        WatchLogger.LogError(ex.ToString(), $"Error getting results - {ex.Message}");
        throw;
      }
      }
    }
  }
}