using System.Net;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SchoolManagementApi.Data;
using SchoolManagementApi.DTOs;
using SchoolManagementApi.Intefaces.Admin;
using SchoolManagementApi.Models;
using SchoolManagementApi.Models.UserModels;
using SchoolManagementApi.Utilities;

namespace SchoolManagementApi.Commands.Admin
{
  public class CreateOrganization
  {
    public class CreateOrganizationsCommand : IRequest<GenericResponse>
    {
      public string? OrganizationName { get; set; }
      public string? AdminId { get; set; }
    }

    public class CreateOrganizationHandler(UserManager<ApplicationUser> userManager, ApplicationDbContext context, IOrganizationService organizationService) : IRequestHandler<CreateOrganizationsCommand, GenericResponse>
    {
      private readonly UserManager<ApplicationUser> _userManager = userManager;
      private readonly ApplicationDbContext _context = context;
      private readonly IOrganizationService _organizationService = organizationService;

      public async Task<GenericResponse> Handle(CreateOrganizationsCommand request, CancellationToken cancellationToken)
      {
        // get the admin user
        var admin = await _userManager.FindByIdAsync(request.AdminId!);
        if (admin == null)
        {
          return new GenericResponse
          {
            Status = HttpStatusCode.BadRequest.ToString(),
            Message = "Admin not found",
          };
        }
        // create orga instance
        var organization = new Organization
        {
          OrganizationUniqueId = GenerateUserCode.GenerateOrgUniqueId(),
          AdminId = request.AdminId!,
          Name = request.OrganizationName!
        };
        // call service
        var createdOrganization = await _organizationService.CreateOrganization(organization);
        var user = await _context.Users.FirstOrDefaultAsync(u => u.OrganizationId == request.AdminId);
        if (createdOrganization != null && user != null) 
        {
          user.OrganizationId = createdOrganization.OrganizationId.ToString();
          await _context.SaveChangesAsync();
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
    }
  }
}