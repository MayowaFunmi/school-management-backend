using System.Net;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementApi.Commands.Profiles;
using SchoolManagementApi.Commands.Uploads;
using SchoolManagementApi.DTOs;
using SchoolManagementApi.Interfaces.Profiles;
using SchoolManagementApi.Queries.Profiles;

namespace SchoolManagementApi.Controllers
{
  public class StaffController(ITeachingStaffInterface teachingStaff, IMediator mediator) : BaseController
  {
    private readonly IMediator _mediator = mediator;
    private readonly ITeachingStaffInterface _teachingStaff = teachingStaff;
    
    [HttpPost]
    [Route("add-teaching-staff-profile")]
    [Authorize(Policy = "TeachingStaff")]
    public async Task<IActionResult> CreateTeacherProfile(AddTeachingStaff.AddTeachingStaffCommand request)
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

    [HttpPost]
    [Route("add-non-teaching-staff-profile")]
    [Authorize(Policy = "NonTeachingStaff")]
    public async Task<IActionResult> CreateNonTeacherProfile(AddNonTeachingStaff.AddNonTeachingStaffCommand request)
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
    [Route("get-teaching-staff-by-id/{staffId}")]
    [Authorize]
    public async Task<IActionResult> GetTeachingStaffById(string staffId)
    {
      try
      {
        if (string.IsNullOrEmpty(CurrentUserId))
          return BadRequest("You are not an authenticated");
        if (string.IsNullOrEmpty(staffId))
          return BadRequest("staff Id cannot be empty");
        if (staffId != CurrentUserId)
          return BadRequest("Logged in user and staff id are not the same");
        var response = await _mediator.Send(new GetTeachingStaffById.GetTeachingStaffByIdQuery(staffId));
        return response.Status == HttpStatusCode.OK.ToString()
          ? Ok(response) : BadRequest(response);
      }
      catch (Exception ex)
      {
        return StatusCode(500, $"An error occurred while processing your request - {ex.Message}");
      }
    }

    [HttpGet]
    [Route("get-non-teaching-staff-by-id/{staffId}")]
    [Authorize]
    public async Task<IActionResult> GetNonTeachingStaffById(string staffId)
    {
      try
      {
        if (string.IsNullOrEmpty(CurrentUserId))
          return BadRequest("You are not an authenticated");
        if (string.IsNullOrEmpty(staffId))
          return BadRequest("staff Id cannot be empty");
        var response = await _mediator.Send(new GetNonTeachingStaffById.GetNonTeachingStaffByIdQuery(staffId));
        return response.Status == HttpStatusCode.OK.ToString()
          ? Ok(response) : BadRequest(response);
      }
      catch (Exception ex)
      {
        return StatusCode(500, $"An error occurred while processing your request - {ex.Message}");
      }
    }

    [HttpGet]
    [Route("get-teacher-current-classes")]
    [Authorize]
    public async Task<IActionResult> GetTeacherCurrentClasses([FromQuery] List<string> classArmIds)
    {
      try
      {
        var classes = await _teachingStaff.GetTeacherClasses(classArmIds);
        if (classes.Count == 0)
        {
          return NotFound(new GenericResponse
          {
            Status = HttpStatusCode.OK.ToString(),
            Message = "No Class Found",
          });
        }
        return Ok(new GenericResponse
        {
          Status = HttpStatusCode.OK.ToString(),
          Message = "Class arms retrieved from ids successfully",
          Data = classes
        });
      }
      catch (Exception ex)
      {
        return StatusCode(500, $"An error occurred while processing your request - {ex.Message}");
      }
    }

    // [HttpPost]
    // [Consumes("multipart/form-data")]
    // [Route("upload-document-files")]
    // [Authorize]
    // public async Task<IActionResult> UploadFiles([FromForm] List<UploadFilesDto> uploadFilesDtos)
    // {
    //   try
    //   {
    //     if (string.IsNullOrEmpty(CurrentUserId))
    //       return BadRequest("You are not allowed");

    //     if (uploadFilesDtos == null || uploadFilesDtos.Count == 0)
    //       return BadRequest("No files uploaded");

    //     var request = new AddUploadDocuments.AddUploadDocumentsCommand
    //     {
    //       StaffId = CurrentUserId,
    //       UploadFilesDtos = uploadFilesDtos
    //     };
    //     var response = await _mediator.Send(request);
    //     return response.Status == HttpStatusCode.OK.ToString()
    //       ? Ok(response) : BadRequest(response);
    //   }
    //   catch (Exception ex)
    //   {
    //     return StatusCode(500, $"An error occurred while processing your request - {ex.Message}");
    //   }
    // }

    [HttpPost]
    [Consumes("multipart/form-data")]
    [Route("upload-document-files")]
    //[Authorize]
    public async Task<IActionResult> UploadDocuments([FromForm] UploadFiles.UploadFilesCommand request)
    {
      try
      {
        if (string.IsNullOrEmpty(CurrentUserId))
          return BadRequest("You are not allowed");

        if (request.Files == null || request.Files.Count == 0)
          return BadRequest("No files uploaded");
        request.StaffId = CurrentUserId;

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
    [Consumes("multipart/form-data")]
    [Route("upload-profile-pictures")]
    [Authorize]
    public async Task<IActionResult> UploadProfilePicture([FromForm] UploadProfilePicture.UploadProfilePictureCommand request)
    {
      try
      {
        if (string.IsNullOrEmpty(CurrentUserId))
          return BadRequest("You are not allowed");

        if (request.ProfilePicture == null)
          return BadRequest("No files uploaded");
        request.StaffId = CurrentUserId;

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