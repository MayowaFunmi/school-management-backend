using System.Net;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SchoolManagementApi.Data;
using SchoolManagementApi.DTOs;
using SchoolManagementApi.Interfaces.Admin;
using SchoolManagementApi.Interfaces.LoggerManager;
using SchoolManagementApi.Models;
using SchoolManagementApi.Models.UserModels;
using SchoolManagementApi.Utilities;
using WatchDog;

namespace SchoolManagementApi.Commands.Admin
{
  public class CreateOrganization
  {
    public class CreateOrganizationsCommand : IRequest<GenericResponse>
    {
      public string OrganizationName { get; set; } = string.Empty;
      public string AdminId { get; set; } = string.Empty;
    }

    public class CreateOrganizationHandler(UserManager<ApplicationUser> userManager, ApplicationDbContext context, IOrganizationService organizationService, ILoggerManager logger) : IRequestHandler<CreateOrganizationsCommand, GenericResponse>
    {
      private readonly UserManager<ApplicationUser> _userManager = userManager;
      private readonly ApplicationDbContext _context = context;
      private readonly IOrganizationService _organizationService = organizationService;
      private readonly ILoggerManager _logger = logger;

      public async Task<GenericResponse> Handle(CreateOrganizationsCommand request, CancellationToken cancellationToken)
      {
        if (string.IsNullOrEmpty(request.AdminId) || string.IsNullOrEmpty(request.OrganizationName))
        {
          return new GenericResponse
          {
            Status = HttpStatusCode.BadRequest.ToString(),
            Message = "Admin Id or Organization name cannot eb empty",
          };
        }

        try
        {
          // get the admin user
          var admin = await _userManager.Users
                      .AsNoTracking()
                      .AnyAsync(u => u.Id == request.AdminId!, cancellationToken: cancellationToken);
          if (!admin)
          {
            return new GenericResponse
            {
              Status = HttpStatusCode.NotFound.ToString(),
              Message = "Admin not found",
            };
          }
          
          // create orga instance
          var organization = new Organization
          {
            OrganizationUniqueId = GenerateUserCode.GenerateOrgUniqueId(),
            AdminId = request.AdminId,
            Name = request.OrganizationName.ToLower()
          };
          // use NLP to check if similar school exists to avoid duplicate entry
          var createdOrganization = await _organizationService.CreateOrganization(organization);
          if (createdOrganization == null)
          {
            return new GenericResponse
            {
              Status = HttpStatusCode.BadRequest.ToString(),
              Message = "Organization is already registered",
            };
          }
          var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == request.AdminId, cancellationToken: cancellationToken);
          if (user != null) 
          {
            user.OrganizationId = createdOrganization.OrganizationUniqueId;
            _context.Users.Update(user);
            await _context.SaveChangesAsync(cancellationToken);
            return new GenericResponse
            {
              Status = HttpStatusCode.OK.ToString(),
              Message = "Organization Created Successfully",
              Data = createdOrganization
            };
          }
          else
          {
            return new GenericResponse
            {
              Status = HttpStatusCode.BadRequest.ToString(),
              Message = "Failed To Create Organization",
            };
          }
        }
        catch (Exception ex)
        {
          _logger.LogError($"Error getting organizations for admin id - {ex.Message}");
          WatchLogger.LogError(ex.ToString(), $"Error getting organizations for admin id - {ex.Message}");
          return new GenericResponse
          {
            Status = HttpStatusCode.InternalServerError.ToString(),
            Message = $"Error getting organizations for admin id - {ex.Message}",
          };
        }
      }
    }
  }
}