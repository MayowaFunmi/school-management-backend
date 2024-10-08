using System.Net;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementApi.DTOs;
using SchoolManagementApi.Interfaces.Admin;
using SchoolManagementApi.Queries.Admin;
using SchoolManagementApi.Queries.School;

namespace SchoolManagementApi.Controllers
{
  public class SchoolController(IMediator mediator, ISchoolServices schoolServices) : BaseController
  {
    private readonly IMediator _mediator = mediator;
    private readonly ISchoolServices _schoolServices = schoolServices;

    [HttpGet]
    [Route("get-schools-by-id")]
    [Authorize]
    public async Task<IActionResult> SchoolsById([FromQuery] string schoolIds)
    {
      var schools = await _schoolServices.GetSchoolByIdList(schoolIds);

      if (schools.Count == 0)
      {
        return Ok(new GenericResponse
        {
          Status = HttpStatusCode.OK.ToString(),
          Message = "No School Found",
          Data = null
        });
      }
      return Ok(new GenericResponse
      {
        Status = HttpStatusCode.OK.ToString(),
        Message = "Schools retrieved from ids successfully",
        Data = schools
      });
    }

    [HttpGet]
    [Route("get-school-by-id/{schoolId}")]
    [Authorize]
    public async Task<IActionResult> GetSchool(string schoolId)
    {
      try
      {
        if (string.IsNullOrEmpty(schoolId))
          return BadRequest("School Id cannot be null");
        var response = await _mediator.Send(new GetSchoolById.GetSchoolByIdQuery(schoolId));
          return response.Status == HttpStatusCode.OK.ToString()
          ? Ok(response) : BadRequest(response);
      }
      catch (Exception ex)
      {
        return StatusCode(500, $"An error occurred while processing your request - {ex.Message}");
      }
    }

    [HttpGet]
    [Route("get-school-by-unique-id/{schoolUniqueId}")]
    [Authorize]
    public async Task<IActionResult> GetSchoolByUniqueId(string schoolUniqueId)
    {
      try
      {
        if (string.IsNullOrEmpty(schoolUniqueId))
          return BadRequest("School uniqueId cannot be null");
        var response = await _mediator.Send(new GetSchoolByUniqueId.GetSchoolByUniqueIdQuery(schoolUniqueId));
          return response.Status == HttpStatusCode.OK.ToString()
          ? Ok(response) : BadRequest(response);
      }
      catch (Exception ex)
      {
        return StatusCode(500, $"An error occurred while processing your request - {ex.Message}");
      }
    }

    [HttpGet]
    [Route("get-school-by-admin-id/{adminId}")]
    [Authorize]
    public async Task<IActionResult> GetSchoolByAdmin(string adminId)
    {
      try
      {
        if (string.IsNullOrEmpty(adminId))
          return BadRequest("Admin Id cannot be null");
        var response = await _mediator.Send(new GetSchoolByAdminId.GetSchoolByAdminIdQuery(adminId));
          return response.Status == HttpStatusCode.OK.ToString()
          ? Ok(response) : BadRequest(response);
      }
      catch (Exception ex)
      {
        return StatusCode(500, $"An error occurred while processing your request - {ex.Message}");
      }
    }

    [HttpGet]
    [Route("get-subjects-by-id")]
    public async Task<IActionResult> SubjectsById([FromQuery] string subjectIds)
    {
      var subjects = await _schoolServices.GetSubjectsByIdList(subjectIds);

      if (subjects.Count == 0)
      {
        return NotFound(new GenericResponse
        {
          Status = HttpStatusCode.OK.ToString(),
          Message = "No Subject Found",
          Data = null
        });
      }
      return Ok(new GenericResponse
      {
        Status = HttpStatusCode.OK.ToString(),
        Message = "Subjects retrieved from ids successfully",
        Data = subjects
      });
    }

    [HttpGet]
    [Route("get-class-arms-by-id")]
    public async Task<IActionResult> ClassArmsById([FromQuery] string classArmIds)
    {
      var classArms = await _schoolServices.GetClassArmsByIdList(classArmIds);
      if (classArms.Count == 0)
      {
        return NotFound(new GenericResponse
        {
          Status = HttpStatusCode.OK.ToString(),
          Message = "No class arm Found",
        });
      }
      return Ok(new GenericResponse
      {
        Status = HttpStatusCode.OK.ToString(),
        Message = "classArms retrieved from ids successfully",
        Data = classArms
      });
    }

    [HttpGet]
    [Route("get-school-departments/{schoolId}")]
    public async Task<IActionResult> SchoolDepartments(string schoolId)
    {
      var departments = await _schoolServices.GetDepartmentsBySchoolId(schoolId);
      if (departments.Count == 0)
      {
        return Ok(new GenericResponse
        {
          Status = HttpStatusCode.OK.ToString(),
          Message = "No Department Found",
          Data = null
        });
      }
      return Ok(new GenericResponse
      {
        Status = HttpStatusCode.OK.ToString(),
        Message = "Departments in school retrieved successfully",
        Data = departments
      });
    }

    [HttpGet]
    [Route("get-school-class-list/{schoolId}")]
    public async Task<IActionResult> SchoolClassArms(string schoolId)
    {
      var classes = await _schoolServices.GetStudentClassesBySchoolId(schoolId);
      if (classes.Count == 0)
      {
        return Ok(new GenericResponse
        {
          Status = HttpStatusCode.OK.ToString(),
          Message = "No Class Arms Found",
          Data = null
        });
      }
      return Ok(new GenericResponse
      {
        Status = HttpStatusCode.OK.ToString(),
        Message = "Classes in school retrieved successfully",
        Data = classes
      });
    }

    [HttpGet]
    [Route("get-school-parents/{schoolUniqueId}")]
    public async Task<IActionResult> SchoolParents(string schoolUniqueId)
    {
      var parents = await _schoolServices.GetSchoolParents(schoolUniqueId);
      if (parents.Count == 0)
      {
        return Ok(new GenericResponse
        {
          Status = HttpStatusCode.OK.ToString(),
          Message = "No parents Found",
          Data = null
        });
      }
      return Ok(new GenericResponse
      {
        Status = HttpStatusCode.OK.ToString(),
        Message = "Parents in school retrieved successfully",
        Data = parents
      });
    }

    [HttpGet]
    [Route("search-school-by-param/{searchParam}")]
    [Authorize]
    public async Task<IActionResult> GetSchoolFromSearch(string searchParam)
    {
      try
      {
        if (string.IsNullOrEmpty(searchParam))
          return BadRequest("search parameter cannot be empty");
        var response = await _mediator.Send(new SearchSchool.SearchSchoolQuery(searchParam));
        return response.Status == HttpStatusCode.OK.ToString()
          ? Ok(response) : BadRequest(response);
      }
      catch (Exception ex)
      {
        return StatusCode(500, $"An error occurred while processing your request - {ex.Message}");
      }
    }

    [HttpGet]
    [Route("school-teaching-staff-count/{schoolId}")]
    [Authorize]
    public async Task<GenericResponse> TeachingStaffCount(string schoolId)
    {
      try
      {
        if (string.IsNullOrEmpty(schoolId))
          throw new ArgumentException("school id cannot be empty");
        var schoolCount = await _schoolServices.GetAllTeachersInSchoolCount(schoolId);
        return new GenericResponse
        {
          Status = HttpStatusCode.OK.ToString(),
          Message = "number of teachers in school retrieved successfully",
          Data = schoolCount
        };
      }
      catch (Exception ex)
      {
        return new GenericResponse
        {
          Status = HttpStatusCode.InsufficientStorage.ToString(),
          Message = $"An internal server error occured - {ex.Message}",
        };
      }
    }

    [HttpGet]
    [Route("school-non-teaching-staff-count/{schoolId}")]
    [Authorize]
    public async Task<GenericResponse> NonTeachingStaffCount(string schoolId)
    {
      try
      {
        if (string.IsNullOrEmpty(schoolId))
          throw new ArgumentException("school id cannot be empty");
        var schoolCount = await _schoolServices.GetNonTeachersInSchoolCount(schoolId);
        return new GenericResponse
        {
          Status = HttpStatusCode.OK.ToString(),
          Message = "number of non teachers in school retrieved successfully",
          Data = schoolCount
        };
      }
      catch (Exception ex)
      {
        return new GenericResponse
        {
          Status = HttpStatusCode.InsufficientStorage.ToString(),
          Message = $"An internal server error occured - {ex.Message}",
        };
      }
    }

    [HttpGet]
    [Route("school-parents-count/{schoolId}")]
    [Authorize]
    public async Task<GenericResponse> SchoolParentsCount(string schoolId)
    {
      try
      {
        if (string.IsNullOrEmpty(schoolId))
          throw new ArgumentException("school id cannot be empty");
        var schoolCount = await _schoolServices.GetParentsInSchoolCount(schoolId);
        return new GenericResponse
        {
          Status = HttpStatusCode.OK.ToString(),
          Message = "number of parents in school retrieved successfully",
          Data = schoolCount
        };
      }
      catch (Exception ex)
      {
        return new GenericResponse
        {
          Status = HttpStatusCode.InsufficientStorage.ToString(),
          Message = $"An internal server error occured - {ex.Message}",
        };
      }
    }

    [HttpGet]
    [Route("school-students-count/{schoolId}")]
    [Authorize]
    public async Task<GenericResponse> SchoolStudentsCount(string schoolId)
    {
      try
      {
        if (string.IsNullOrEmpty(schoolId))
          throw new ArgumentException("school id cannot be empty");
        var schoolCount = await _schoolServices.GetStudentsInSchoolCount(schoolId);
        return new GenericResponse
        {
          Status = HttpStatusCode.OK.ToString(),
          Message = "number of students in school retrieved successfully",
          Data = schoolCount
        };
      }
      catch (Exception ex)
      {
        return new GenericResponse
        {
          Status = HttpStatusCode.InsufficientStorage.ToString(),
          Message = $"An internal server error occured - {ex.Message}",
        };
      }
    }

    [HttpGet]
    [Route("get-teachers-in-school/{schoolId}")]
    [Authorize]
    public async Task<IActionResult> TeachersInSchool(string schoolId, [FromQuery] GetTeachingStaff.GetTeachingStaffQuery request)
    {
      if (string.IsNullOrEmpty(schoolId))
        return BadRequest("school Id must be specified.");

      // Check if both page and pageSize are specified
      if (request.Page == default || request.PageSize == default)
        return BadRequest("Both Page and PageSize must be specified.");

      if (request.Page <= 0 || request.PageSize <= 0)
        return BadRequest("Page and PageSize must be greater than zero.");

      // Set the organizationId in the request object
      request.SchoolId = schoolId;
      try
      {
        var response = await _mediator.Send(request);
        return response.Status == HttpStatusCode.OK.ToString()
          ? Ok(response)
          : NotFound(response);
      }
      catch (Exception ex)
      {
        return StatusCode(500, $"An error occurred while processing your request - {ex.Message}");
      }
    }

    [HttpGet]
    [Route("get-non-teachers-in-school/{schoolId}")]
    [Authorize]
    public async Task<IActionResult> NonTeachersInSchool(string schoolId, [FromQuery] GetNonTeachingStaff.GetNonTeachingStaffQuery request)
    {
      if (string.IsNullOrEmpty(schoolId))
        return BadRequest("school Id must be specified.");

      // Check if both page and pageSize are specified
      if (request.Page == default || request.PageSize == default)
        return BadRequest("Both Page and PageSize must be specified.");

      if (request.Page <= 0 || request.PageSize <= 0)
        return BadRequest("Page and PageSize must be greater than zero.");

      // Set the organizationId in the request object
      request.SchoolId = schoolId;
      try
      {
        var response = await _mediator.Send(request);
        return response.Status == HttpStatusCode.OK.ToString()
          ? Ok(response)
          : NotFound(response);
      }
      catch (Exception ex)
      {
        return StatusCode(500, $"An error occurred while processing your request - {ex.Message}");
      }
    }

    [HttpGet]
    [Route("get-parents-in-school/{schoolUniqueId}")]
    [Authorize]
    public async Task<IActionResult> ParentsInSchool(string schoolUniqueId, [FromQuery] GetParents.GetParentsQuery request)
    {
      if (string.IsNullOrEmpty(schoolUniqueId))
        return BadRequest("school Id must be specified.");

      // Check if both page and pageSize are specified
      if (request.Page == default || request.PageSize == default)
        return BadRequest("Both Page and PageSize must be specified.");

      if (request.Page <= 0 || request.PageSize <= 0)
        return BadRequest("Page and PageSize must be greater than zero.");

      // Set the organizationId in the request object
      request.SchoolUniqueId = schoolUniqueId;
      try
      {
        var response = await _mediator.Send(request);
        return response.Status == HttpStatusCode.OK.ToString()
          ? Ok(response)
          : NotFound(response);
      }
      catch (Exception ex)
      {
        return StatusCode(500, $"An error occurred while processing your request - {ex.Message}");
      }
    }

    [HttpGet]
    [Route("get-students-in-school/{schoolId}")]
    [Authorize]
    public async Task<IActionResult> StudentsInSchool(string schoolId, [FromQuery] GetStudents.GetStudentsQuery request)
    {
      if (string.IsNullOrEmpty(schoolId))
        return BadRequest("school Id must be specified.");

      // Check if both page and pageSize are specified
      if (request.Page == default || request.PageSize == default)
        return BadRequest("Both Page and PageSize must be specified.");

      if (request.Page <= 0 || request.PageSize <= 0)
        return BadRequest("Page and PageSize must be greater than zero.");

      // Set the organizationId in the request object
      request.SchoolId = schoolId;
      try
      {
        var response = await _mediator.Send(request);
        return response.Status == HttpStatusCode.OK.ToString()
          ? Ok(response)
          : BadRequest(response);
      }
      catch (Exception ex)
      {
        return StatusCode(500, $"An error occurred while processing your request - {ex.Message}");
      }
    }

    [HttpPost]
    [Route("add-school-session")]
    [Authorize(Policy = "AdminOrganizationAdmin")]
    public async Task<IActionResult> AddSchoolSession([FromBody] SessionDto sessionDto)
    {
      if (string.IsNullOrEmpty(sessionDto.Name) || string.IsNullOrEmpty(sessionDto.SchoolUniqueId))
        return BadRequest("session name and school id cannot be empty");
      try
      {
        var response = await _schoolServices.AddSchoolSession(sessionDto);
        return response.Status switch
        {
          "BadRequest" => BadRequest(response),
          "NotFound" => NotFound(response),
          _ => Ok(response)
        };
      }
      catch (Exception ex)
      {
        return StatusCode(500, $"An error occurred while processing your request - {ex.Message}");
      }
    }

    [HttpGet]
    [Route("get-school-session/{schoolUniqueId}")]
    public async Task<IActionResult> GetSchoolSession(string schoolUniqueId)
    {
      try
      {
        var session = await _schoolServices.GetSchoolSessions(schoolUniqueId);
        return Ok(session);
      }
      catch (Exception ex)
      {
        return StatusCode(500, $"An error occurred while processing your request - {ex.Message}");
      }
    }

    [HttpPost]
    [Route("add-school-terms")]
    [Authorize(Policy = "AdminOrganizationAdmin")]
    public async Task<IActionResult> AddSchoolTerms([FromBody] SchoolTermDto schoolTermDto)
    {
      try
      {        
        if (string.IsNullOrEmpty(schoolTermDto.SchoolSessionId) || schoolTermDto.SchoolTerms.Count == 0)
          return BadRequest("All fields are required");
        var response = await _schoolServices.AddSchoolTerms(schoolTermDto);
        return response.Status switch
        {
            "BadRequest" => BadRequest(response),
            "NotFound" => NotFound(response),
            _ => Ok(response)
        };
      }
      catch (Exception ex)
      {
        return StatusCode(500, $"An error occurred while processing your request - {ex.Message}");
      }
    }

    [HttpGet]
    [Route("get-school-term/{schoolid}")]
    public async Task<IActionResult> GetSchoolTerm(string schoolid)
    {
      try
      {
        var terms = await _schoolServices.GetSchoolTerms(schoolid);
        if (terms.Count == 0)
          return NotFound("No terms found for this school");
        return Ok(terms);
      }
      catch (Exception ex)
      {
        return StatusCode(500, $"An error occurred while processing your request - {ex.Message}");
      }
    }
  }
}