using System.Net;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementApi.Commands.LessonNotes;
using SchoolManagementApi.Queries.LessonNote;

namespace SchoolManagementApi.Controllers
{
  public class LessonNoteController(IMediator mediator) : BaseController
  {
    private readonly IMediator _mediator = mediator;

    [HttpPost]
    [Route("create-lesson-note")]
    [Authorize]

    public async Task<IActionResult> AddLessonNote(CreateLessonNotes.CreateLessonNotesCommand request)
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
    [Route("get-teacher-lesson-notes/{teacherId}")]
    [Authorize]
    public async Task<IActionResult> TeacherNotes(string teacherId, [FromQuery] GetTeacherLessonNotes.GetTeacherLessonNotesQuery request)
    {
      if (string.IsNullOrEmpty(teacherId))
        return BadRequest("teacher Id must be specified.");

      // Check if both page and pageSize are specified
      if (request.Page == default || request.PageSize == default)
        return BadRequest("Both Page and PageSize must be specified.");

      if (request.Page <= 0 || request.PageSize <= 0)
        return BadRequest("Page and PageSize must be greater than zero.");

      // Set the organizationId in the request object
      request.TeacherId = teacherId;
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
    [Route("get-lesson-note-by-id/{lessonNoteId}")]
    [Authorize]

    public async Task<IActionResult> GetNoteById(GetLessonNoteById.GetLessonNoteByIdQuery request)
    {
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
  }
}