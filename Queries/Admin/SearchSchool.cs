using System.Net;
using MediatR;
using SchoolManagementApi.DTOs;
using SchoolManagementApi.Interfaces.Admin;

namespace SchoolManagementApi.Queries.Admin
{
  public class SearchSchool
  {
    public record SearchSchoolQuery(string searchQuery) : IRequest<GenericResponse>;

    public class SearchSchoolHandler(ISchoolServices schoolServices) : IRequestHandler<SearchSchoolQuery, GenericResponse>
    {
      private readonly ISchoolServices _schoolServices = schoolServices;
      public async Task<GenericResponse> Handle(SearchSchoolQuery request, CancellationToken cancellationToken)
      {
        try
        {
          var result = await _schoolServices.SearchSchool(request.searchQuery);
          if (result.Count != 0)
          {
            return new GenericResponse
            {
              Status = HttpStatusCode.OK.ToString(),
              Message = $"{result.Count} schools found",
              Data = result
            };
          }
          return new GenericResponse
          {
            Status = HttpStatusCode.OK.ToString(),
            Message = $"No schoolfound",
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