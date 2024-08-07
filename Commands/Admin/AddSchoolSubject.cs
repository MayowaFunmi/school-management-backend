using System.Net;
using MediatR;
using SchoolManagementApi.DTOs;
using SchoolManagementApi.Interfaces.Admin;
using SchoolManagementApi.Models;

namespace SchoolManagementApi.Commands.Admin
{
  public class AddSchoolSubject
  {
    public class AddSchoolSubjectCommand : IRequest<GenericResponse>
    {
      public string SubjectName { get; set; } = string.Empty;
      public List<string> SubjectNamesList { get; set;} = [];
    }

    public class AddSchoolSubjectHandler(ISubjectService subjectService) : IRequestHandler<AddSchoolSubjectCommand, GenericResponse>
    {
      private readonly ISubjectService _subjectService = subjectService;

      public async Task<GenericResponse> Handle(AddSchoolSubjectCommand request, CancellationToken cancellationToken)
      {
        try
        {
          var subjectCreated = new Subject();
          var subjectList = new Subject();
          var subjectListCreated = new List<Subject>();
          if (!string.IsNullOrEmpty(request.SubjectName) && request.SubjectNamesList.Count == 0)
          {
            subjectCreated = await _subjectService.AddSubject(request.SubjectName);
            if (subjectCreated == null)
            {
              return new GenericResponse
              {
                Status = HttpStatusCode.BadRequest.ToString(),
                Message = $"{request.SubjectName} already exist",
              };
            }
          }
          else if (string.IsNullOrEmpty(request.SubjectName) && request.SubjectNamesList.Count != 0)
          {
            foreach (var subject in request.SubjectNamesList)
            {
              subjectList = await _subjectService.AddSubject(subject);
              if (subjectList != null)
                subjectListCreated.Add(subjectList);
            }
          }
          else if (!string.IsNullOrEmpty(request.SubjectName) && request.SubjectNamesList.Count != 0)
          {
            return new GenericResponse
            {
              Status = HttpStatusCode.BadRequest.ToString(),
              Message = "Use one option to add subject",
            };
          }
          if (subjectCreated != null && subjectCreated.SubjectId != Guid.Empty)
          {
            return new GenericResponse
            {
              Status = HttpStatusCode.OK.ToString(),
              Message = "School subject created successfully",
            };
          }

          if (subjectListCreated.Count != 0)
          {
            return new GenericResponse
            {
              Status = HttpStatusCode.OK.ToString(),
              Message = "School subject list created successfully",
            };
          }
          return new GenericResponse
          {
            Status = HttpStatusCode.BadRequest.ToString(),
            Message = "Failed to create school subject",
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