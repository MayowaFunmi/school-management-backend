using System.Net;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementApi.Commands.Admin;
using SchoolManagementApi.DTOs;
using SchoolManagementApi.Interfaces.Admin;
using SchoolManagementApi.Queries.Admin;
using SchoolManagementApi.Queries.Profiles;

namespace SchoolManagementApi.Controllers
{
  public class AdminController(IAdminService adminService, IOrganizationService organizationService, IMediator mediator) : BaseController
  {
    private readonly IAdminService _adminService = adminService;
    private readonly IOrganizationService _organizationService = organizationService;
    private readonly IMediator _mediator = mediator;

    [HttpGet]
    [Route("get-all-users")]
    [Authorize(Policy = "OwnerSuperAdmin")]
    public async Task<GenericResponse> ShowAllUsers()
    {
      var usersWithRoles = await _adminService.GetAllUsersWithRoles();
      return new GenericResponse
      {
        Status = HttpStatusCode.OK.ToString(),
        Message = $"{usersWithRoles.Count} users found",
        Data = usersWithRoles
      };
    }

    [HttpGet]
    [Route("check-organization-status")]
    [Authorize(Policy = "OrganizationAdmin")]
    public async Task<IActionResult> CheckOrganizationDuplicate([FromQuery] string organizationName)
    {
      try
      {
        var response = await _organizationService.CheckOrganizationStatus(organizationName);
        return response.Status == HttpStatusCode.OK.ToString()
        ? Ok(response) : Conflict(response);
      }
      catch (Exception ex)
      {
        return StatusCode(500, $"An error occurred while processing your request - {ex.Message}");
      }
    }

    [HttpPost]
    [Route("create-organization")]
    [Authorize(Policy = "OrganizationAdmin")]
    public async Task<IActionResult> CreateOrganization(CreateOrganization.CreateOrganizationsCommand request)
    {
      try
      {
        if (string.IsNullOrEmpty(request.OrganizationName) || string.IsNullOrEmpty(request.AdminId) || request.States.Count == 0)
          return BadRequest("All fields are required");

        if (string.IsNullOrEmpty(CurrentUserId))
          return Unauthorized("You are not authenticated");
          
        request.AdminId = CurrentUserId;
        var response = await _mediator.Send(request);
        
        return response.Status switch
        {
            "OK" => Ok(response),
            "BadRequest" => BadRequest(response),
            "NotFound" => NotFound(response),
            _ => StatusCode(500, "An unexpected error occurred")
        };
      }
      catch (Exception ex)
      {
        return StatusCode(500, $"An error occurred while processing your request - {ex.Message}");
      }
    }

    [HttpPost]
    [Route("create-organization-for-admin/{adminId}")]
    [Authorize(Policy = "OwnerSuperAdmin")]

    public async Task<IActionResult> CreateOrganizationForAdmin(string adminId)
    {
      if (string.IsNullOrEmpty(adminId))
          return BadRequest("please provide admin id");
      try
      {
        var request = new CreateOrganization.CreateOrganizationsCommand
        {
            AdminId = adminId
        };
        var response = await _mediator.Send(request);
        return response.Status switch
        {
            "Conflict" => Conflict(response),
            "NotFound" => NotFound(response),
            "BadRequest" => BadRequest(response),
            _=> Ok(response)
        };
      }
      catch (Exception ex)
      {
        return StatusCode(500, $"An error occurred while processing your request - {ex.Message}");
      }
    }


    // [HttpGet]
    // [Route("all-admin-organizations")]
    // [Authorize(Policy = "OwnerSuperAdmin")]
    // public async Task<IActionResult> AllAdminOrganizations(GetOrganizationsByAdminId.GetOrganizationByAdminIdQuery request)
    // {
    //   request.AdminId = CurrentUserId!;
    //   var response = await _mediator.Send(request);
    //   return response.Status == HttpStatusCode.OK.ToString()
    //     ? Ok(response) : BadRequest(response);
    // }

    [HttpGet]
    [Route("get-admin-organizations/{adminId}")]
    [Authorize(Policy = "OrganizationAdmin")]

    public async Task<IActionResult> GetAllAdminOrganizations(string adminId)
    {
      try
      {
        if (string.IsNullOrEmpty(CurrentUserId))
          return BadRequest("You are not an authenticated");
        if (string.IsNullOrEmpty(adminId))
          return BadRequest("admin Id cannot be empty");
        if (adminId != CurrentUserId)
          return BadRequest("Logged in user and staff id are not the same");

        var response = await _mediator.Send(new GetOrganizationsByAdminId.GetOrganizationByAdminIdQuery(adminId));
        return response.Status == HttpStatusCode.OK.ToString()
          ? Ok(response) : NotFound(response);
      }
      catch (Exception ex)
      {
        return StatusCode(500, $"An error occurred while processing your request - {ex.Message}");
      }
    }

    [HttpGet]
    [Route("get-organization-by-id/{organizationId}")]
    [Authorize]

    public async Task<IActionResult> GetOrganizationById(string organizationId)
    {
      try
      {
        if (string.IsNullOrEmpty(organizationId))
          return BadRequest("organization id cannot be empty");

        var response = await _mediator.Send(new GetOrganizationById.GetOrganizationByIdQuery(organizationId));
        return response.Status == HttpStatusCode.OK.ToString()
          ? Ok(response) : NotFound(response);
      }
      catch (Exception ex)
      {
        return StatusCode(500, $"An error occurred while processing your request - {ex.Message}");
      }
    }

    [HttpGet]
    [Route("get-all-organizations")]
    [Authorize(Policy = "OwnerSuperAdmin")]
    public async Task<IActionResult> AllOrganizations()
    {
      var response = await _mediator.Send(new GetAllOrganizations.GetAllOrganizationsQuery());
      return response.Status == HttpStatusCode.OK.ToString()
        ? Ok(response) : NotFound(response);
    }

    [HttpPost]
    [Route("add-zone")]
    [Authorize(Policy = "OrganizationAdmin")]
    public async Task<IActionResult> CreateZone(CreateZone.CreateZoneCommand request)
    {
      try
      {
        if (string.IsNullOrEmpty(request.OrganizationUniqueId) ||
           string.IsNullOrEmpty(request.Name) || 
           string.IsNullOrEmpty(request.AdminId) ||
            string.IsNullOrEmpty(request.State) ||
            request.LocalGovtAreas.Count == 0
          )
        {
          return BadRequest("Some fields cannot be empty");
        }

        if (string.IsNullOrEmpty(CurrentUserId))
        {
          return Unauthorized("You are not an admin");
        }

        if (request.AdminId != CurrentUserId)
          return Unauthorized("You are not an admin for this organization");
        
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
    [Route("get-all-organization-zones/{organizationId}")]
    [Authorize]
    public async Task<IActionResult> GetOrganizationZonesById(string organizationId)
    {
      try
      {
        if (string.IsNullOrEmpty(organizationId))
        {
          return BadRequest("Organization Id cannot be empty");
        }
        var response = await _mediator.Send(new GetOrganizationZones.GetOrganizationZonesQuery(organizationId));
        return response.Status == HttpStatusCode.OK.ToString()
          ? Ok(response) : NotFound(response);
      }
      catch (Exception ex)
      {
        return StatusCode(500, $"An error occurred while processing your request - {ex.Message}");
      }
    }

    [HttpGet]
    [Route("show-all-organization-zones/{organizationUniqueId}")]
    [Authorize]
    public async Task<IActionResult> GetAllOrganizationZonesByuniqueId(string organizationUniqueId)
    {
      try
      {
        if (string.IsNullOrEmpty(organizationUniqueId))
        {
          return BadRequest("Organization Unique Id cannot be empty");
        }
        var response = await _mediator.Send(new GetAllZonesByUniqueId.GetAllZonesByUniqueIdCommand(organizationUniqueId));
        return response.Status == HttpStatusCode.OK.ToString()
          ? Ok(response) : BadRequest(response);
      }
      catch (Exception ex)
      {
        return StatusCode(500, $"An error occurred while processing your request - {ex.Message}");
      }
    }

    [HttpGet]
    [Route("admin-get-all-organization-zones")]
    [Authorize(Policy = "OwnerSuperAdmin")]
    public async Task<IActionResult> GetOranizationZonesForAdmin(GetAllOrganizationZones.GetAllOrganizationZonesQuery request)
    {
      try
      {
        if (string.IsNullOrEmpty(CurrentUserId))
          return BadRequest("You are not an Admin");
        if (string.IsNullOrEmpty(request.OrganizationUniqueId))
          return BadRequest("Organization Unique Id cannot be empty");
        request.AdminId = CurrentUserId;
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
    [Route("add-zone-for-admin/{adminId}")]
    [Authorize(Policy = "OwnerSuperAdmin")]

    public async Task<IActionResult> CreateZoneForAdmin(string adminId)
    {
      var request = new CreateZone.CreateZoneCommand
      {
          AdminId = adminId
      };
      var response = await _mediator.Send(request);
      return response.Status == HttpStatusCode.OK.ToString()
        ? Ok(response) : BadRequest(response);
    }

    [HttpPost]
    [Route("add-school")]
    [Authorize(Policy = "OrganizationAdmin")]
    public async Task<IActionResult> CreateSchool(CreateSchool.CreateSchoolCommand request)
    {
      try
      {
        if (string.IsNullOrEmpty(CurrentUserId))
          return Unauthorized("You are not an admin");
        if (string.IsNullOrEmpty(request.OrganizationUniqueId) || string.IsNullOrEmpty(request.ZoneId) || string.IsNullOrEmpty(request.Name) || string.IsNullOrEmpty(request.Address))
          return BadRequest("All fields are required");
        request.AdminId = CurrentUserId;
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
    [Route("get-schools-in-zone/{zoneId}")]
    [Authorize]
    public async Task<IActionResult> GetSchoolsInZone(string zoneId)
    {
      // bool pageSpecified = request.Page != default;
      // bool pageSizeSpecified = request.PageSize != default;
      if (string.IsNullOrEmpty(zoneId))
        return BadRequest("Zone Id must be specified.");
      
      try
      {
        var request = new GetAllSchoolsInZone.GetAllSchoolsInZoneQuery
        {
          ZoneId = zoneId
        };
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
    [Route("get-schools-in-organization/{organizationUniqueId}")]
    [Authorize]
    public async Task<IActionResult> GetSchoolsInOrganization(string organizationUniqueId, [FromQuery] GetAllOrganizationSchools.GetAllOrganizationSchoolsQuery request)
    {
      if (string.IsNullOrEmpty(organizationUniqueId))
        return BadRequest("Organization Unique Id must be specified.");

      // Check if both page and pageSize are specified
      if (request.Page == default || request.PageSize == default)
        return BadRequest("Both Page and PageSize must be specified.");

      if (request.Page <= 0 || request.PageSize <= 0)
        return BadRequest("Page and PageSize must be greater than zero.");

      // Set the organizationId in the request object
      request.OrganizationUniqueId = organizationUniqueId;
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

    // get organization users with specified roles
		[HttpGet]
		[Route("get-organization-users-by-role/{organizationId}")]
		[Authorize]
		public async Task<IActionResult> OrganizationUsersByRoles(string organizationId, [FromQuery] GetOrganizationUsersByRole.GetOrganizationUsersByRoleQuery request)
		{
			if (string.IsNullOrEmpty(organizationId) || string.IsNullOrEmpty(request.RoleName))
				return BadRequest("Organization Id or roleName must be specified.");

			// Check if both page and pageSize are specified
			if (request.Page == default || request.PageSize == default)
				return BadRequest("Both Page and PageSize must be specified.");

			if (request.Page <= 0 || request.PageSize <= 0)
				return BadRequest("Page and PageSize must be greater than zero.");
      
      request.OrganizationId = organizationId;
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
    [Route("get-all-schools")]
    [Authorize(Policy = "OwnerSuperAdmin")]
    public async Task<IActionResult> GetAllSchools(GetAllSchools.GetAllSchoolsQuery request)
    {
      bool pageSpecified = request.Page != default;
      bool pageSizeSpecified = request.PageSize != default;

      if (!pageSpecified || !pageSizeSpecified)
        return BadRequest("Page and PageSize must be specified.");
      
      if (request.Page == 0 || request.PageSize == 0)
        return BadRequest("Page and PageSize must not be zero value.");

      try
      {
        var response = await _mediator.Send(request);
        return response.Status == HttpStatusCode.OK.ToString()
          ? Ok(response) : NotFound(response);
      }
      catch (Exception ex)
      {
        return StatusCode(500, $"An error occurred while processing your request - {ex.Message}");
      }
    }

    [HttpGet]
    [Route("get-school-departments/{schoolId}")]
    [Authorize]
    public async Task<IActionResult> GetDepartmentsInSchool(string schoolId)
    {
      Console.WriteLine("school id = " + schoolId);
      try
      {
        if (string.IsNullOrEmpty(schoolId))
        {
          return BadRequest("You must provide a school id");
        }
        var response = await _mediator.Send(new GetSchoolDepartments.GetSchoolDepartmentsQuery(schoolId));
        return response.Status == HttpStatusCode.OK.ToString()
        ? Ok(response) : BadRequest(response);
      }
      catch (Exception ex)
      {
        return StatusCode(500, $"An error occurred while processing your request - {ex.Message}");
      }
    }

    [HttpPost]
    [Route("add-school-department/{schoolId}")]
    [Authorize(Policy = "AdminOrganizationAdmin")]
    public async Task<IActionResult> AddDepartment(string schoolId, [FromBody] CreateDepartment.CreateDepartmentCommand request)
    {
      try
      {
        if (string.IsNullOrEmpty(CurrentUserId))
          return Unauthorized("You are not an admin");
        if (string.IsNullOrEmpty(schoolId) || string.IsNullOrEmpty(request.Name))
          return BadRequest("All fields are required");
        
        request.AdminId = CurrentUserId;
        request.SchoolId = schoolId;
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
    [Route("add-student-class/{schoolId}")]
    [Authorize(Policy = "Admin")]
    public async Task<IActionResult> AddStudentClass(string schoolId, [FromBody] CreateStudentClass.CreateStudentClassCommand request)
    {
      try
      {
        bool arm = request.Arm != default;

        if (!arm)
          return BadRequest("Class Arm must be set");
        if (request.Arm < 1)
          return BadRequest("Class Arm must not be less than 1");

        if (string.IsNullOrEmpty(schoolId) || string.IsNullOrEmpty(request.Name))
          return BadRequest("School Id or Name must be set");

        request.SchoolId = schoolId;
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
    [Route("get-claases-in-school/{schoolId}")]
    [Authorize]
    public async Task<IActionResult> GetAllClassesInSchool(string schoolId)
    {
      try
      {
        if (string.IsNullOrEmpty(schoolId))
          return BadRequest("School Id cannot be null");
        var response = await _mediator.Send(new GetAllStudentClasses.GetAllStudentClassesQuery(schoolId));
          return response.Status == HttpStatusCode.OK.ToString()
          ? Ok(response) : BadRequest(response);
      }
      catch (Exception ex)
      {
        return StatusCode(500, $"An error occurred while processing your request - {ex.Message}");
      }
    }

    [HttpGet]
    [Route("get-all-school-classarms")]
    public async Task<IActionResult> AllSchoolClassArms(GetAllClassArms.GetAllClassArmsQuery request)
    {
      try
      {
        if (string.IsNullOrEmpty(CurrentUserId))
          return BadRequest("You are not an Admin");
        if (string.IsNullOrEmpty(request.SchoolId) || string.IsNullOrEmpty(request.ClassId))
          return BadRequest("School Id or Class Id cannot be null");
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
    [Route("get-organization-school-data/{organizationUniqueId}")]
    [Authorize(Policy = "OwnerSuperAdmin")]
    public async Task<IActionResult> GetOrganizationSchoolData(string organizationUniqueId)
    {
      try
      {
        if (string.IsNullOrEmpty(CurrentUserId))
          return BadRequest("You are not authorize");
        if (string.IsNullOrEmpty(organizationUniqueId))
          return BadRequest("Organization Unique Id cannot b null");
        var response = await _mediator.Send(new GetOrganizationSchoolData.GetOrganizationSchoolDataQuery(organizationUniqueId));
        return response.Status == HttpStatusCode.OK.ToString()
          ? Ok(response) : BadRequest(response);
      }
      catch (Exception ex)
      {
        return StatusCode(500, $"An error occurred while processing your request - {ex.Message}");
      }
    }

    [HttpPost]
    [Route("add-school-subject")]
    [Authorize(Policy = "OwnerSuperAdmin")]
    public async Task<IActionResult> AddSubjects(AddSchoolSubject.AddSchoolSubjectCommand request)
    {
      try
      {
        if (string.IsNullOrEmpty(CurrentUserId))
          return BadRequest("You are not authorized");
        var response = await _mediator.Send(request);
        return response.Status == HttpStatusCode.OK.ToString()
          ? Ok(response) : BadRequest(response.Message);
        //return Ok(response);
      }
      catch (Exception ex)
      {
        return StatusCode(500, $"An error occurred while processing your request - {ex.Message}");
      }
    }

    [HttpGet]
    [Route("get-all-subjects")]
    public async Task<IActionResult> GetAllSubjects()
    {
      try
      {
        var res = await _adminService.GetSubjectsInSchool();
        return Ok(res);
      }
      catch (Exception ex)
      {
        return StatusCode(500, $"An error occurred while processing your request - {ex.Message}");
      }
    }

    [HttpGet]
    [Route("get-user-by-unique-id/{uniqueId}")]
    [Authorize]
    public async Task<IActionResult> GetUserDetails(string uniqueId, [FromQuery] GetUserByUniqueId.GetUserByUniqueIdQuery request)
    {
      try
      {
        if (string.IsNullOrEmpty(uniqueId))
          return BadRequest("unique id cannot be empty");
        request.UniqueId = uniqueId;
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
    [Route("get-user-by-id/{userId}")]
    [Authorize]
    public async Task<IActionResult> GetUserById(string userId)
    {
      try
      {
        if (string.IsNullOrEmpty(userId))
          return BadRequest("user id cannot be empty");
        var response = await _mediator.Send(new GetUserById.GetUserByIdQuery(userId));
        return response.Status == HttpStatusCode.OK.ToString()
          ? Ok(response) : NotFound(response);
      }
      catch (Exception ex)
      {
        return StatusCode(500, $"An error occurred while processing your request - {ex.Message}");
      }
    }
  }
}