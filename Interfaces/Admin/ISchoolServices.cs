using SchoolManagementApi.DTOs;
using SchoolManagementApi.Models;
using SchoolManagementApi.Models.UserModels;

namespace SchoolManagementApi.Intefaces.Admin
{
  public interface ISchoolServices
  {
    Task<bool> AddSchool(School school);
    Task<bool> OrganizationExists(string organizationUniqueId, string adminId);
    Task<School> GetSchoolById(string schoolId);
    Task<List<School>> AllScchools(int page, int pageSize);
    Task<int> AllSchoolCount();
    Task<int> AllOrganizationSchoolsCount(string OrganizationUniqueId);
    Task<int> AllSchoolsInZoneCount(string ZoneId);
    Task<List<School>> AllOrganizationScchools(string OrganizationUniqueId, int page, int pageSize);
    Task<List<OrganizationData>> OrganizationData(string organizationUniqueId);
    Task<List<School>> AllZoneScchools(string ZoneId, int page, int pageSize);
    Task<List<TeachingStaff>> GetAllTeachersInSchool(string schoolId, int page, int pageSize);
    Task<List<NonTeachingStaff>> GetNonTeachersInSchool(string schoolId, int page, int pageSize);
    Task<List<Parent>> GetParentsInSchool(string schoolId, int page, int pageSize);
    Task<List<Student>> GetStudentsInSchool(string schoolId, int page, int pageSize);
    Task<int> GetAllTeachersInSchoolCount(string schoolId);
    Task<int> GetNonTeachersInSchoolCount(string schoolId);
    Task<int> GetParentsInSchoolCount(string schoolId);
    Task<int> GetStudentsInSchoolCount(string schoolId);
    Task<List<School>> GetSchoolByIdList(List<string> schoolIds);
    Task<List<Subject>> GetSubjectsByIdList(List<string> subjectIds);
    Task<List<Department>> GetDepartmentsBySchoolId(string schoolId);
    Task<List<ClassArms>> GetStudentClassesBySchoolId(string schoolId);
    Task<List<Parent>> GetSchoolParents(string schoolId);
    Task<List<School>> SearchSchool(string searchString);
    Task<SchoolSession> AddSchoolSession(SessionDto sessionDto);
    Task<List<SchoolSession>> GetSchoolSessions(string schoolUniqueId);
    Task<bool> AddSchoolTerms(SchoolTermDto schoolTermDto);
    Task<List<SchoolTerm>> GetSchoolTerms(string schoolId);
  }
}