using System.Net;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementApi.Commands.Profiles;
using SchoolManagementApi.Commands.Students;
using SchoolManagementApi.Queries.Profiles;
using SchoolManagementApi.Queries.School;
using SchoolManagementApi.Queries.Students;

namespace SchoolManagementApi.Controllers
{
  public class StudentController(IMediator mediator) : BaseController
  {
    private readonly IMediator _mediator = mediator;
    
    [HttpPost]
    [Route("create-student-profile")]
    [Authorize(Policy = "Student")]
    public async Task<IActionResult> CreateStudentProfile(AddStudent.AddStudentCommand request)
    {
      try
      {
        if (string.IsNullOrEmpty(CurrentUserId))
          return BadRequest("You are not allowed");

        if (CurrentUserId != request.UserId)
          return BadRequest("You are not allowed to create teacher profile");

        var response = await _mediator.Send(request);
        return response.Status == HttpStatusCode.OK.ToString()
          ? Ok(response) : BadRequest(response);
      }
      catch (Exception ex)
      {
        return StatusCode(500, $"An error occurred while processing your request - {ex.Message}");
      }
    }

    [HttpGet]
    [Route("get-student-profile/{studentId}")]
    [Authorize]

    public async Task<IActionResult> GetParentById(string studentId)
    {
      try
      {
        if (string.IsNullOrEmpty(CurrentUserId))
          return BadRequest("You are not an authenticated");
        if (string.IsNullOrEmpty(studentId))
          return BadRequest("student Id cannot be empty");
        if (studentId != CurrentUserId)
          return BadRequest("Logged in user and staff id are not the same");

        var response = await _mediator.Send(new GetStudentById.GetStudentByIdQuery(studentId));
        return response.Status == HttpStatusCode.OK.ToString()
          ? Ok(response) : BadRequest(response);
      }
      catch (Exception ex)
      {
        return StatusCode(500, $"An error occurred while processing your request - {ex.Message}");
      }
    }

    [HttpGet]
    [Route("get-students-in-class-arm/{studentClassId}")]
    [Authorize]

    public async Task<IActionResult> StudentsInClassArm(string studentClassId, [FromQuery] GetStudentsInClassArm.GetStudentsInClassArmQuery request)
    {
      if (string.IsNullOrEmpty(CurrentUserId))
          return BadRequest("You are not an authenticated");
      if (string.IsNullOrEmpty(studentClassId))
        return BadRequest("class Id cannot be empty");

      if (request.Page == default || request.PageSize == default)
      return BadRequest("Both Page and PageSize must be specified.");

      if (request.Page <= 0 || request.PageSize <= 0)
        return BadRequest("Page and PageSize must be greater than zero.");

      request.StudentClassId = studentClassId;
      try
      {
        var response = await _mediator.Send(request);
        return response.Status == HttpStatusCode.OK.ToString()
          ? Ok(response) : BadRequest(response);
      }
      catch (Exception ex)
      {
        return StatusCode(500, $"An error occurred while processing your request - {ex.Message}");
      }
    }

    [HttpGet]
    [Route("get-students-in-class/{classId}")] // all students in the same class arm without pagination
    [Authorize]
    public async Task<IActionResult> StudentsInClass(string classId)
    {
      try
      {
        if (string.IsNullOrEmpty(CurrentUserId))
          return BadRequest("You are not an authenticated");
        if (string.IsNullOrEmpty(classId))
          return BadRequest("class Id cannot be empty");
        
        var response = await _mediator.Send(new GetStudentsInClass.GetStudentsInClassQuery(classId));
        return response.Status == HttpStatusCode.OK.ToString()
          ? Ok(response) : BadRequest(response);
      }
      catch (Exception ex)
      {
        return StatusCode(500, $"An error occurred while processing your request - {ex.Message}");
      }
    }

    [HttpPost]
    [Route("add-students-scores")]
    [Authorize]

    public async Task<IActionResult> AddCATests(AddStudentsCA.AddStudentsCACommand request)
    {
      if (string.IsNullOrEmpty(request.ClassId) 
        || string.IsNullOrEmpty(request.SubjectId)
        || string.IsNullOrEmpty(request.Term)
        || string.IsNullOrEmpty(request.SessionId)
        || request.StudentsScores.Count != 0)
          return BadRequest("All field are required");
      try
      {
        var response = await _mediator.Send(request);
        return response.Status == HttpStatusCode.OK.ToString()
          ? Ok(response) : BadRequest(response);
      }
      catch (Exception ex)
      {
        return StatusCode(500, $"An error occurred while processing your request - {ex.Message}");
      }
    }

    [HttpGet]
    [Route("get-students-result")]
    [Authorize]

    public async Task<IActionResult> GetResult(GetStudentsResults.GetStudentsResultsQuery request)
    {
      if (string.IsNullOrEmpty(request.ClassId) 
        || string.IsNullOrEmpty(request.SubjectId)
        || string.IsNullOrEmpty(request.Term)
        || string.IsNullOrEmpty(request.SessionId))
          return BadRequest("All field are required");
      try
      {
        var response = await _mediator.Send(request);
        return response.Status == HttpStatusCode.OK.ToString()
          ? Ok(response) : BadRequest(response);
      }
      catch (Exception ex)
      {
        return StatusCode(500, $"An error occurred while processing your request - {ex.Message}");
      }
    }
  }
}