namespace SchoolManagementApi.Constants
{
  public class RoleCionstants
  {
    public enum Roles
    {
      Users,
      Admin,
      SuperAdmin,
      OrganizationAdmin,
      TeachingStaff,
      NonTeachingStaff,
      Parent,
      Student,
      Owner
    }

    public enum DesignationEnum
    {
      Supervisor,
      Principal,
      VicePrincipal,
      HeadOfDepartment,
      ClassTeacher,
      OfficeAssistant,
      Secretary,
      Librarian,
      Typist,
      Others
    }

    public enum TitleEnum
    {
      Miss, Mr, Mrs, Dr, Prof
    }

    public enum QualificationEnum
    {
      SSCE,
      OND,
      NCE,
      HND,
      BSc,
      BEd,
      PGDE,
      BArts,
      MSc,
      PhD
    }

    public enum GenderEnum
    {
      Male,
      Female
    }

    public enum ReligionEnum
    {
      Christianity,
      Islam
    }

    public enum MaritalStatus
    {
      Single, Engaged, Married, Divorced, Widow
    }
  }
}