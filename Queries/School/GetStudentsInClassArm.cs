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
    public class GetStudentsInClassArmQuery : IRequest<GenericResponse>
    {
      public string StudentClassId { get; set; } = string.Empty;
      public int Page { get; set; } = 0;
      public int PageSize { get; set; } = 0;
    }


    public class GetStudentsInClassArmHandler(IStudentClassServices studentClassServices, ILoggerManager logger) : IRequestHandler<GetStudentsInClassArmQuery, GenericResponse>
    {
      private readonly IStudentClassServices _studentClassServices = studentClassServices;
      private readonly ILoggerManager _logger = logger;

      public async Task<GenericResponse> Handle(GetStudentsInClassArmQuery request, CancellationToken cancellationToken)
      {
        var studentsCount = await _studentClassServices.StudentsByClassArmCount(request.StudentClassId);
        int totalPages = 1;
        if (request.Page != 0 || request.PageSize != 0)
          totalPages = (int)Math.Ceiling((double)studentsCount / request.PageSize);
        
        request.Page = Math.Min(Math.Max(request.Page, 1), totalPages);

        try
        {
          var students = await _studentClassServices.StudentsByClassArm(request.StudentClassId, request.Page, request.PageSize);
          if (students.Count != 0)
          {
            var response = new PageRespnses.StudentsPageResponse
            {
              Students = students,
              TotalPages = totalPages,
              CurrentPage = request.Page,
              PagesLeft = totalPages - request.Page
            };
            return new GenericResponse
            {
              Status = HttpStatusCode.OK.ToString(),
              Message = $"{students.Count} students in class retrieved successfully",
              Data = response
            };
          }
          return new GenericResponse
          {
            Status = HttpStatusCode.OK.ToString(),
            Message = "no students found in this class",
            Data = students
          };
        }
        catch (Exception ex)
        {
          _logger.LogError($"Error getting students in the same class arm - {ex.Message}");
          WatchLogger.LogError(ex.ToString(), $"Error getting students in the same class arm - {ex.Message}");
          return new GenericResponse
          {
            Status = HttpStatusCode.InternalServerError.ToString(),
            Message = $"Error getting school- {ex.Message}",
          };
        }
      }
    }
  }
}