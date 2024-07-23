using System.Net;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementApi.Commands.Profiles;
using SchoolManagementApi.Commands.Students;
using SchoolManagementApi.Interfaces.Admin;
using SchoolManagementApi.Queries.Profiles;
using SchoolManagementApi.Queries.School;
using SchoolManagementApi.Queries.Students;

namespace SchoolManagementApi.Controllers
{
  public class StudentController(IMediator mediator, IStudentClassServices studentClassServices) : BaseController
  {
    private readonly IMediator _mediator = mediator;
    private readonly IStudentClassServices _studentClassServices = studentClassServices;
    
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

    [HttpPost]
    [Route("mark-student-class-attendance")]
    [Authorize]
    public async Task<IActionResult>MarkClassAttendance(ClassRegister.ClassRegisterCommand request)
    {
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
    [Route("get-class-attendance")]
    [Authorize]
    public async Task<IActionResult> GetClassRegister(GetStudentAttendance.GetStudentAttendanceQuery request)
    {
      try
      {
        if (!string.IsNullOrEmpty(request.ClassArmId) || request.Date == default)
          return BadRequest("Class arm and date cannot be null");
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
    [Route("get-classarm-daily-attendance")]
    [Authorize]

    public async Task<IActionResult> GetClassDailyAttendanceCount([FromQuery] string classArmId, [FromQuery] DateTime date)
    {
      if(string.IsNullOrEmpty(classArmId) || date == default)
        return BadRequest("class arm and date cannot be diled");
      try
      {
        var count = await _studentClassServices.GetClassDailyAttendanceCount(classArmId, date);
        return Ok(new { ClassArmId = classArmId, Date = date, Count = count });
      }
      catch (Exception ex)
      {
        return StatusCode(500, $"An error occurred whil getting daily attendance record - {ex.Message}");
      }
    }

    [HttpGet]
    [Route("get-classArmId-weekly-attendance")]
    [Authorize]

    public async Task<IActionResult> GetClassWeeklyAttendanceCount([FromQuery] string classArmId, [FromQuery] DateTime startDate)
    {
      if(string.IsNullOrEmpty(classArmId) || startDate == default)
        return BadRequest("class arm and date cannot be diled");
      try
      {
        var count = await _studentClassServices.GetClassWeeklyAttendanceCount(classArmId, startDate);
        return Ok(new { ClassArmId = classArmId, StartDate = startDate, Count = count });
      }
      catch (Exception ex)
      {
        return StatusCode(500, $"An error occurred whil getting weekly attendance record - {ex.Message}");
      }
    }

    [HttpGet]
    [Route("get-student-daily-attendance")]
    [Authorize]
    public async Task<IActionResult> GetStudentDailyAttendanceCount([FromQuery] string classArmId, [FromQuery] string studentId, [FromQuery] DateTime date)
    {
      if(string.IsNullOrEmpty(classArmId) || string.IsNullOrEmpty(studentId) || date == default)
        return BadRequest("class arm, student id and date cannot be diled");
      try
      {
        var count = await _studentClassServices.GetStudentDailyAttendanceCount(classArmId, studentId, date);
        return Ok(new { ClassArmId = classArmId, StudentId = studentId, Date = date, Count = count });
      }
      catch (Exception ex)
      {
        return StatusCode(500, $"An error occurred whil getting student daily attendance count - {ex.Message}");
      }
    }

    [HttpGet]
    [Route("get-student-weekly-attendance")]
    [Authorize]
    public async Task<IActionResult> GetStudentWeeklyAttendanceCount([FromQuery] string classArmId, [FromQuery] string studentId, [FromQuery] DateTime startDate)
    {
      if(string.IsNullOrEmpty(classArmId) || string.IsNullOrEmpty(studentId) || startDate == default)
        return BadRequest("class arm, student id and date cannot be diled");
      try
      {
        var count = await _studentClassServices.GetStudentWeeklyAttendanceCount(classArmId, studentId, startDate);
        return Ok(new { ClassArmId = classArmId, StudentId = studentId, Date = startDate, Count = count });
      }
      catch (Exception ex)
      {
        return StatusCode(500, $"An error occurred whil getting student weekly attendance count - {ex.Message}");
      }
    }

    [HttpGet]
    [Route("get-class-weekly-attendance-counts")]
    [Authorize]
    public async Task<IActionResult> StudentsClassWeeklyAttendanceCounts([FromQuery] string classArmId, [FromQuery] string studentId, [FromQuery] DateTime startDate)
    {
      if(string.IsNullOrEmpty(classArmId) || string.IsNullOrEmpty(studentId) || startDate == default)
        return BadRequest("class arm, student id and date cannot be diled");
      try
      {
        var response = await _studentClassServices.StudentsClassWeeklyAttendanceCounts(classArmId, startDate);
        return Ok(new { ClassArmId = classArmId, StudentId = studentId, Date = startDate, Response = response });
      }
      catch (Exception ex)
      {
        return StatusCode(500, $"An error occurred whil getting student weekly attendance count - {ex.Message}");
      }
    }
  }
}