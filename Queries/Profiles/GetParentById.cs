using System.Net;
using MediatR;
using SchoolManagementApi.DTOs;
using SchoolManagementApi.Interfaces.LoggerManager;
using SchoolManagementApi.Interfaces.Profiles;
using WatchDog;

namespace SchoolManagementApi.Queries.Profiles
{
  public class GetParentById
  {
    public record GetParentByIdQuery(string parentId) : IRequest<GenericResponse>;

    public class GetParentByIdHandler(IParentService parentService, ILoggerManager logger) : IRequestHandler<GetParentByIdQuery, GenericResponse>
    {
      private readonly IParentService _parentService = parentService;
      private readonly ILoggerManager _logger = logger;

      public async Task<GenericResponse> Handle(GetParentByIdQuery request, CancellationToken cancellationToken)
      {
        try
        {
          var parent = await _parentService.GetParentById(request.parentId);
          if (parent != null)
          {
            return new GenericResponse
            {
              Status = HttpStatusCode.OK.ToString(),
              Message = "Parent retrieved successfully",
              Data = parent
            };
          }
          return new GenericResponse
          {
            Status = HttpStatusCode.OK.ToString(),
            Message = $"Parent not found",
          };
        }
        catch (Exception ex)
        {
          _logger.LogError($"Error getting parent by id - {ex.Message}");
          WatchLogger.LogError(ex.ToString(), $"Error getting parent by id - {ex.Message}");
          return new GenericResponse
          {
            Status = HttpStatusCode.InternalServerError.ToString(),
            Message = $"Error getting parent by id - {ex.Message}",
          };
        }
      }
    }
  }
}