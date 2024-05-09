using Microsoft.AspNetCore.Authorization;

namespace SchoolManagementApi.Configurations
{
  public class AuthPolicy
  {
    public static void ConfigureAuthorization(AuthorizationOptions options)
    {
      options.AddPolicy("OwnerOnly", policy => policy.RequireRole("Owner"));
      options.AddPolicy("OwnerAdminSuperAdmin", policy => policy.RequireRole("Owner", "Admin", "SuperAdmin"));
      options.AddPolicy("OwnerOrganizationAdminSuperAdmin", policy => policy.RequireRole("Owner", "OrganizationAdmin", "SuperAdmin"));
      options.AddPolicy("OwnerSuperAdmin", policy => policy.RequireRole("Owner", "SuperAdmin"));
      options.AddPolicy("OwnerOrganizationAdmin", policy => policy.RequireRole("Owner", "OrganizationAdmin"));
      options.AddPolicy("Admin", policy => policy.RequireRole("Admin")); // school admin
      options.AddPolicy("OrganizationAdmin", policy => policy.RequireRole("OrganizationAdmin"));
      options.AddPolicy("AdminOrganizationAdmin", policy => policy.RequireRole("Admin", "OrganizationAdmin"));

      options.AddPolicy("AdminSuperAdmin", policy => policy.RequireRole("Admin", "SuperAdmin"));
      options.AddPolicy("TeachingStaff", policy => policy.RequireRole("TeachingStaff"));
      options.AddPolicy("NonTeachingStaff", policy => policy.RequireRole("NonTeachingStaff"));
      options.AddPolicy("Parent", policy => policy.RequireRole("Parent"));
      options.AddPolicy("Student", policy => policy.RequireRole("Student"));
      options.AddPolicy("TeacherStudent", policy => policy.RequireRole("Teacher", "Student", "Admin", "Owner"));
    }
  }
}