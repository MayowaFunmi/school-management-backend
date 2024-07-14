using System.Net;
using MediatR;
using SchoolManagementApi.DTOs;
using SchoolManagementApi.Intefaces.Admin;
using SchoolManagementApi.Models;

namespace SchoolManagementApi.Commands.Students
{
  public class ClassRegister
  {
    public class ClassAttendanceModel
    {
      public required string StudentId { get; set; }
      public string MorningAttendance { get; set; } = string.Empty;
      public string AfternoonAttendance { get; set; } = string.Empty;
    }

    public class ClassRegisterCommand : IRequest<GenericResponse>
    {
      public required string ClassArmId { get; set; }
      public DateTime DayAndTime { get; set; }
      public List<ClassAttendanceModel> Registers { get; set; } = [];
    }

    public class ClassRegisterHandler(IStudentClassServices studentClassServices) : IRequestHandler<ClassRegisterCommand, GenericResponse>
    {
      private readonly IStudentClassServices _studentClassServices = studentClassServices;

      public async Task<GenericResponse> Handle(ClassRegisterCommand request, CancellationToken cancellationToken)
      {
        try
        {
          bool track = true;
          foreach(var reg in request.Registers)
          {
            var registered = await SubmitRegister(reg, request.ClassArmId, request.DayAndTime);
            if (!registered)
            {
              track = false;
              return new GenericResponse
              {
                Status = HttpStatusCode.BadRequest.ToString(),
                Message = "Failed to register a student",
              };
            }
          }
          if (track)
          {
            return new GenericResponse
            {
              Status = HttpStatusCode.OK.ToString(),
              Message = "Class attendane marked successfully",
            };
          }
          return new GenericResponse
          {
            Status = HttpStatusCode.BadRequest.ToString(),
            Message = "Failed to mark class attendane",
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

      private async Task<bool> SubmitRegister(ClassAttendanceModel model, string classArmId, DateTime dayAndTime)
      {
        var classAttendance = new ClassAttendance
        {
          StudentId = model.StudentId,
          ClassArmId = classArmId,
          DayAndTime = dayAndTime,
          MorningAttendance = model.MorningAttendance.ToLower(),
          AfternoonAttendance = model.AfternoonAttendance.ToLower(),
          IsMarked = true
        };
        var attendance = await _studentClassServices.MarkStudentsAttendance(classAttendance);
        if (attendance != null)
          return true;
        else
          return false;
      }
    }
  }
}