using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolManagementApi.Data;
using SchoolManagementApi.DTOs;
using SchoolManagementApi.Intefaces.Roles;
using SchoolManagementApi.Models.UserModels;

namespace SchoolManagementApi.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class RoleController(ApplicationDbContext context, IRoleService roleService, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager) : ControllerBase
  {
    private readonly ApplicationDbContext _context = context;
    private readonly IRoleService _roleService = roleService;
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly RoleManager<IdentityRole> _roleManager = roleManager;

		[HttpPost]
		[Route("add-new-role")]
		public async Task<GenericResponse> AddRole([FromBody] string roleName)
		{
			var result = await _roleService.AddRole(roleName);
			if (result)
			{
				return new GenericResponse()
				{
					Status = HttpStatusCode.OK.ToString(),
					Message = $"{roleName} Role Added Successfully",
					Data = new
					{
							RoleName = roleName
					}
				};
			}
			return new GenericResponse()
			{
				Status = HttpStatusCode.BadRequest.ToString(),
				Message = "Failed to add role",
				Data = null
			};
		}

		[HttpGet]
		[Route("get-all-roles")]
		[Authorize]
		public async Task<GenericResponse> GetAllRoles()
		{
			var result = await _roleService.GetRoleList();
			if (result.Count > 0)
			{
				return new GenericResponse()
				{
					Status = HttpStatusCode.OK.ToString(),
					Message = "All Roles Retrieved Successfully",
					Data = result
				};
			}
			else
			{
				return new GenericResponse()
				{
					Status = HttpStatusCode.NotFound.ToString(),
					Message = "No Role Has Been Added!!!",
					Data = null
				};
			}
		}

		[HttpGet]
		[Route("get-selected-roles")]
		public async Task<GenericResponse> GetSelectedRoles()
		{
			var result = await _roleService.GetSelectedRoleList();
			if (result.Count > 0)
			{
				return new GenericResponse()
				{
					Status = HttpStatusCode.OK.ToString(),
					Message = "All Roles Retrieved Successfully",
					Data = result
				};
			}
			else
			{
				return new GenericResponse()
				{
					Status = HttpStatusCode.NotFound.ToString(),
					Message = "No Role Has Been Added!!!",
					Data = null
				};
			}
		}
		public class EditRoleRequest
		{
			public string RoleName { get; set; } = string.Empty;
			public string EditedRole { get; set; } = string.Empty;
		}

		[HttpPut]
		[Route("edit-role")]
		public async Task<GenericResponse> EditRole([FromBody] EditRoleRequest request)
		{
			var result = await _roleService.EditRole(request.RoleName, request.EditedRole);
			if (result)
			{
				return new GenericResponse()
				{
					Status = HttpStatusCode.OK.ToString(),
					Message = $"{request.RoleName} Role Edited Successfully",
					Data = new
					{
							OldRoleName = request.RoleName,
							NewRoleName = request.EditedRole
					}
				};
			}
			return new GenericResponse()
			{
				Status = HttpStatusCode.OK.ToString(),
				Message = "Failed to edit role",
			};
		}

		[HttpDelete]
		[Route("delete-role")]
		public async Task<GenericResponse> DeleteRole([FromBody] string roleName)
		{
			var result = await _roleService.DeleteRole(roleName);
			if (result)
			{
				return new GenericResponse()
				{
					Status = HttpStatusCode.OK.ToString(),
					Message = $"{roleName} Role Deleted Successfully",
					Data = result
				};
			}
			return new GenericResponse()
			{
				Status = HttpStatusCode.BadRequest.ToString(),
				Message = "Failed to delete role",
			};
		}

		// add more rols to user
		[HttpPost]
    [Route("add-role-to-user")]
		public async Task<GenericResponse> AddRoleToUser([FromBody] ChangeRoleDto changeRoleDto)
		{
			// get user with the id
			var user = await _context.Users
				.Where(x => x.UniqueId == changeRoleDto.UniqueId)
				.FirstOrDefaultAsync();
			var role = await _context.Roles.FirstOrDefaultAsync(x => x.Name == changeRoleDto.RoleName);
			if (user != null && role != null)
			{
				// get roles of the user
				var roleExists = await _context.UserRoles.FirstOrDefaultAsync(x => x.UserId == user.Id && x.RoleId == role.Id);

				if (roleExists != null)
				{
					return new GenericResponse()
					{
						Status = HttpStatusCode.OK.ToString(),
						Message = $"{user.UserName} Already Has Role {changeRoleDto.RoleName}",
					};
				}
				else
				{
					var newRole = new IdentityUserRole<string>
					{
						UserId = user.Id, RoleId = role.Id
					};
					_context.UserRoles.Add(newRole);
					await _context.SaveChangesAsync();
					return new GenericResponse()
					{
						Status = HttpStatusCode.OK.ToString(),
						Message = $"{changeRoleDto.RoleName} Role Added To {user.UserName}",
						Data = true
					};
				}
			}
			else
			{
				return new GenericResponse()
				{
					Status = HttpStatusCode.OK.ToString(),
					Message = "User or role not found",
					Data = null
				};
			}
		}

		// remove role from user
		[HttpDelete]
		[Route("remove-user-from-role")]
		public async Task<GenericResponse> RemoveRoleFromUser([FromBody] DeleteRoleDto changeRoleDto)
		{
			var user = await _context.Users
				.Where(x => x.UniqueId == changeRoleDto.UniqueId)
				.FirstOrDefaultAsync();
			var role = await _context.Roles.FirstOrDefaultAsync(x => x.Name == changeRoleDto.RoleName2);
			Console.WriteLine($"user = {user!.UserName} and role = {role!.Name}");

			if (user != null && role != null)
			{
				var roleExists = await _context.UserRoles.FirstOrDefaultAsync(x => x.UserId == user.Id && x.RoleId == role.Id);
				if (roleExists == null)
				{
					return new GenericResponse()
					{
						Status = HttpStatusCode.OK.ToString(),
						Message = $"{user.UserName} does not have Role {changeRoleDto.RoleName2}",
					};
				}
				_context.UserRoles.Remove(roleExists);
				await _context.SaveChangesAsync();
				return new GenericResponse()
				{
					Status = HttpStatusCode.OK.ToString(),
					Message = $"{user.UserName} removed from {changeRoleDto.RoleName2} Role",
					Data = true
				};
			}
			return new GenericResponse()
			{
				Status = HttpStatusCode.OK.ToString(),
				Message = "User or role not found",
				Data = null
			};
		}

		// get all users by role name
		[HttpGet]
		[Route("get-all-users-by-role")]
		public async Task<GenericResponse> GetUsersByRole([FromBody] string roleName)
		{
			var role = await _roleManager.FindByNameAsync(roleName);
			if (role == null)
			{
				return new GenericResponse()
				{
						Status = HttpStatusCode.BadRequest.ToString(),
						Message = "Role not found",
						Data = null
				};
			}

			var usersInRole = await _userManager.GetUsersInRoleAsync(role.Name);
			return new GenericResponse()
			{
				Status = HttpStatusCode.OK.ToString(),
				Message = $"Users having role {roleName} retrieved successfully",
				Data = usersInRole
			};
		}
  }
}