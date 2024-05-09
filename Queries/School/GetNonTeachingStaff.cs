using System.Net;
using MediatR;
using SchoolManagementApi.DTOs;
using SchoolManagementApi.Intefaces.Admin;

namespace SchoolManagementApi.Queries.School
{
  public class GetNonTeachingStaff
  {
    public class GetNonTeachingStaffQuery : IRequest<GenericResponse>
    {
      public string SchoolId { get; set; } = string.Empty;
      public int Page { get; set; } = 0;
      public int PageSize { get; set; } = 0;
    }

    public class GetNonTeachingStaffHandler(ISchoolServices schoolServices) : IRequestHandler<GetNonTeachingStaffQuery, GenericResponse>
    {
      private readonly ISchoolServices _schoolServices = schoolServices;

      public async Task<GenericResponse> Handle(GetNonTeachingStaffQuery request, CancellationToken cancellationToken)
      {
        var teachersCount = await _schoolServices.GetNonTeachersInSchoolCount(request.SchoolId);
        int totalPages = 1;
        if (request.Page != 0 || request.PageSize != 0)
          totalPages = (int)Math.Ceiling((double)teachersCount / request.PageSize);
        
        request.Page = Math.Min(Math.Max(request.Page, 1), totalPages);

        try
        {
          var teachers = await _schoolServices.GetNonTeachersInSchool(request.SchoolId, request.Page, request.PageSize);
          if (teachers.Count != 0)
          {
            var response = new PageRespnses.NonTeachersPageResponse
            {
              NonTeachers = teachers,
              TotalPages = totalPages,
              CurrentPage = request.Page,
              PagesLeft = totalPages - request.Page
            };
            return new GenericResponse
            {
              Status = HttpStatusCode.OK.ToString(),
              Message = $"{teachers.Count} non teachers retrieved succesfully",
              Data = response
            };
          }
          return new GenericResponse
          {
            Status = HttpStatusCode.OK.ToString(),
            Message = $"No non teachers found in this school",
          };
        }
        catch (Exception ex)
        {
          return new GenericResponse
          {
            Status = HttpStatusCode.InternalServerError.ToString(),
            Message = $"An internal server error occurred - {ex.Message}"
          };
        }
      }
    }
  }
}