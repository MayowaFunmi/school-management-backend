using System.Net;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementApi.Commands.Admin;
using SchoolManagementApi.DTOs;
using SchoolManagementApi.Intefaces.Admin;
using SchoolManagementApi.Queries.Admin;

namespace SchoolManagementApi.Controllers
{
  public class AdminController(IAdminService adminService, IMediator mediator) : BaseController
  {
    private readonly IAdminService _adminService = adminService;
    private readonly IMediator _mediator = mediator;

    [HttpGet]
    [Route("get-all-users")]
    //[Authorize(Policy = "OwnerSuperAdmin")]
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

    [HttpPost]
    [Route("create-organization")]
    [Authorize(Policy = "OrganizationAdmin")]
    public async Task<IActionResult> CreateOrganization(CreateOrganization.CreateOrganizationsCommand request)
    {
      try
      {
        if (string.IsNullOrEmpty(request.OrganizationName))
        {
            return BadRequest("Organization Name cannot be empty");
        }

        if (string.IsNullOrEmpty(CurrentUserId))
        {
            return Unauthorized("You are not an admin");
        }
        request.AdminId = CurrentUserId!;
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
    [Route("create-organization-for-admin/{adminId}")]
    [Authorize(Policy = "OwnerSuperAdmin")]

    public async Task<IActionResult> CreateOrganizationForAdmin(string adminId)
    {
      var request = new CreateOrganization.CreateOrganizationsCommand
      {
          AdminId = adminId
      };
      var response = await _mediator.Send(request);
      return response.Status == HttpStatusCode.OK.ToString()
        ? Ok(response) : BadRequest(response);
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
          ? Ok(response) : BadRequest(response);
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
        ? Ok(response) : BadRequest(response);
    }

    [HttpPost]
    [Route("add-zone")]
    [Authorize(Policy = "OrganizationAdmin")]
    public async Task<IActionResult> CreateZone(CreateZone.CreateZoneCommand request)
    {
      try
      {
        if (string.IsNullOrEmpty(request.OrganizationUniqueId) && string.IsNullOrEmpty(request.Name) && string.IsNullOrEmpty(request.AdminId) && string.IsNullOrEmpty(request.State))
        {
          return BadRequest("Some fields cannot be empty");
        }

        if (string.IsNullOrEmpty(CurrentUserId))
        {
          return Unauthorized("You are not an admin");
        }
        request.AdminId = CurrentUserId!;
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
          ? Ok(response) : BadRequest(response);
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
        if (string.IsNullOrEmpty(request.OrganizationUniqueId) || string.IsNullOrEmpty(request.ZoneId) || string.IsNullOrEmpty(request.Name) || string.IsNullOrEmpty(request.Address) || string.IsNullOrEmpty(request.State) || string.IsNullOrEmpty(request.LocalGovtArea))
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
          : BadRequest(response);
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
          : BadRequest(response);
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
          ? Ok(response) : BadRequest(response);
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
          ? Ok(response) : BadRequest(response);
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
  }
}