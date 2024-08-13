using System.Net;
using MediatR;
using SchoolManagementApi.DTOs;
using SchoolManagementApi.Interfaces.Admin;
using SchoolManagementApi.Interfaces.LoggerManager;
using WatchDog;

namespace SchoolManagementApi.Queries.School
{
  public class GetSchoolByUniqueId
  {
    public record GetSchoolByUniqueIdQuery(string SchoolUniqueId) : IRequest<GenericResponse>;

    public class GetSchoolByUniqueIdHandler(ISchoolServices schoolServices, ILoggerManager logger) : IRequestHandler<GetSchoolByUniqueIdQuery, GenericResponse>
    {
      private readonly ISchoolServices _schoolServices = schoolServices;
      private readonly ILoggerManager _logger = logger;

      public async Task<GenericResponse> Handle(GetSchoolByUniqueIdQuery request, CancellationToken cancellationToken)
      {
        try
        {
          var school = await _schoolServices.GetSchoolByUniqueId(request.SchoolUniqueId);
          if (school != null)
          {
            return new GenericResponse
            {
              Status = HttpStatusCode.OK.ToString(),
              Message = "school retrieved successfully",
              Data = school
            };
          }
          return new GenericResponse
          {
            Status = HttpStatusCode.BadRequest.ToString(),
            Message = "failed to get school",
          };
        }
        catch (Exception ex)
        {
          _logger.LogError($"Error getting school by school unique id - {ex.Message}");
          WatchLogger.LogError(ex.ToString(), $"Error getting school by school unique id - {ex.Message}");
          return new GenericResponse
          {
            Status = HttpStatusCode.InternalServerError.ToString(),
            Message = $"Error getting school by school unique id- {ex.Message}",
          };
        }
      }
    }
  }
}