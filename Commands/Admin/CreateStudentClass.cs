using System.Net;
using MediatR;
using SchoolManagementApi.DTOs;
using SchoolManagementApi.Intefaces.Admin;
using SchoolManagementApi.Models;

namespace SchoolManagementApi.Commands.Admin
{
  public class CreateStudentClass
  {
    public class CreateStudentClassCommand : IRequest<GenericResponse>
    {
      public string? SchoolId { get; set; }
      public string? Name { get; set; }
      public int Arm { get; set; }
    }

    public class CreateStudentClassHandler(IStudentClassServices studentClassServices) : IRequestHandler<CreateStudentClassCommand, GenericResponse>
    {
      private readonly IStudentClassServices _studentClassServices = studentClassServices;

      public async Task<GenericResponse> Handle(CreateStudentClassCommand request, CancellationToken cancellationToken)
      {
        try
        {
          var studentClasses = new StudentClass
          {
            SchoolId = Guid.Parse(request.SchoolId!),
            Name = request.Name!,
            Arm = request.Arm
          };
          var studentClassesCreated = await _studentClassServices.AddStudentClass(studentClasses);
          if (studentClassesCreated != null)
          {
            await _studentClassServices.GenerateClassArms(studentClasses);
            return new GenericResponse
            {
              Status = HttpStatusCode.OK.ToString(),
              Message = "Student classes and class arms created successfully",
              Data = studentClassesCreated
            };
          }
          return new GenericResponse
          {
            Status = HttpStatusCode.BadRequest.ToString(),
            Message = "Failed to create Student classes and class arms",
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