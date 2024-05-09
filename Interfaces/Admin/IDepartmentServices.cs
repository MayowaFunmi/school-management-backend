using SchoolManagementApi.Models;

namespace SchoolManagementApi.Intefaces.Admin
{
  public interface IDepartmentServices
  {
    Task<Department> AddDepartment(Department department);
    Task<bool> DepartmentExists(string departmentName);
    Task<List<Department>> GetSchoolDepartments(string schoolId);
  }
}