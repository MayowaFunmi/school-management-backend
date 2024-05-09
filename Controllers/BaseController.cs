using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace SchoolManagementApi.Controllers
{
  [ApiController]
  [Route("api/[controller]")]

  public class BaseController : ControllerBase
  {
    public BaseController() { }

    private string? ClaimId => User?.Claims
      .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
    
    private List<string>? ClaimRoles => User?.Claims
      .Where(c => c.Type == ClaimTypes.Role)
      .Select(c => c.Value)
      .ToList();

    private bool HasRoles => (ClaimRoles?.Count) != 0;

    public string? CurrentUserId => ClaimId is null ? null : ClaimId;

    public List<string>? CurrentUserRoles => HasRoles ? ClaimRoles : null;
  }
}