using System.Net;
using MediatR;
using SchoolManagementApi.DTOs;
using SchoolManagementApi.Intefaces.Admin;
using SchoolManagementApi.Intefaces.LoggerManager;
using WatchDog;

namespace SchoolManagementApi.Queries.School
{
  public class GetStudentsInClassArm
  {
    public class GetStudentsInClassArmQuery : IRequest<PageDataResponse>
    {
      public string StudentClassId { get; set; } = string.Empty;
      public int Page { get; set; } = 0;
      public int PageSize { get; set; } = 0;
    }


    public class GetStudentsInClassArmHandler(IStudentClassServices studentClassServices, ILoggerManager logger) : IRequestHandler<GetStudentsInClassArmQuery, PageDataResponse>
    {
      private readonly IStudentClassServices _studentClassServices = studentClassServices;
      private readonly ILoggerManager _logger = logger;

      public async Task<PageDataResponse> Handle(GetStudentsInClassArmQuery request, CancellationToken cancellationToken)
      {
        try
        {
          var studentsCount = await _studentClassServices.StudentsByClassArmCount(request.StudentClassId);
          var students = await _studentClassServices.StudentsByClassArm(request.StudentClassId, request.Page, request.PageSize);
          var paginationMetaData = new PaginationMetaData(request.Page, request.PageSize, studentsCount);

          return new PageDataResponse
          {
            Status = HttpStatusCode.OK.ToString(),
            Message = "Students in the same class retrieved successfully",
            Data = students,
            Pagination = paginationMetaData
          };
        }
        catch (Exception ex)
        {
          _logger.LogError($"Error getting students in the same class arm - {ex.Message}");
          WatchLogger.LogError(ex.ToString(), $"Error getting students in the same class arm - {ex.Message}");
          return new PageDataResponse
          {
            Status = HttpStatusCode.InternalServerError.ToString(),
            Message = $"Error getting school- {ex.Message}",
          };
        }
      }
    }
  }
}