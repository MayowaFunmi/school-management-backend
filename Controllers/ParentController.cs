using System.Net;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementApi.Commands.Profiles;
using SchoolManagementApi.Intefaces.Profiles;
using SchoolManagementApi.Queries.Profiles;

namespace SchoolManagementApi.Controllers
{
  public class ParentController(IMediator mediator) : BaseController
  {
    private readonly IMediator _mediator = mediator;
    
    [HttpPost]
    [Route("create-parent-profile")]
    [Authorize(Policy = "Parent")]
    public async Task<IActionResult> CreateParentProfile(AddParent.AddParentCommand request)
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
    [Route("get-parent-profile/{parentId}")]
    [Authorize]

    public async Task<IActionResult> GetParentById(string parentId)
    {
      try
      {
        if (string.IsNullOrEmpty(CurrentUserId))
          return BadRequest("You are not an authenticated");
        if (string.IsNullOrEmpty(parentId))
          return BadRequest("parent Id cannot be empty");
        if (parentId != CurrentUserId)
          return BadRequest("Logged in user and staff id are not the same");

        var response = await _mediator.Send(new GetParentById.GetParentByIdQuery(parentId));
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