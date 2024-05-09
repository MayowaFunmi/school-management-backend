using System.Net;
using MediatR;
using SchoolManagementApi.DTOs;
using SchoolManagementApi.Intefaces.Admin;

namespace SchoolManagementApi.Queries.Admin
{
  public class GetAllSchools
  {
    public class GetAllSchoolsQuery : IRequest<GenericResponse>
    {
      public int Page { get; set; }
      public int PageSize { get; set; }
    };
    
    public class GetAllSchoolsHandler(ISchoolServices schoolServices) : IRequestHandler<GetAllSchoolsQuery, GenericResponse>
    {
      private readonly ISchoolServices _schoolServices = schoolServices;

      public async Task<GenericResponse> Handle(GetAllSchoolsQuery request, CancellationToken cancellationToken)
      {
        int totalSchoolCount = await _schoolServices.AllSchoolCount();
        int totalPages = (int)Math.Ceiling((double)totalSchoolCount / request.PageSize);
        try
        {
          var schools = await _schoolServices.AllScchools(request.Page, request.PageSize);
          if (schools.Count != 0)
          {
            var response = new PaginationResponse
            {
              Schools = schools,
              TotalPages = totalPages,
              CurrentPage = request.Page,
              PagesLeft = totalPages - request.Page,
            };
            return new GenericResponse
            {
              Status = HttpStatusCode.OK.ToString(),
              Message = $"{schools.Count} registered schools retrieved succesfully",
              Data = response
            };
          }
          return new GenericResponse
          {
            Status = HttpStatusCode.OK.ToString(),
            Message = $"No school has been registered",
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