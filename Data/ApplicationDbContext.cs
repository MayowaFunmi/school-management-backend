using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SchoolManagementApi.Models;
using SchoolManagementApi.Models.DocumentModels;
using SchoolManagementApi.Models.Lessons;
using SchoolManagementApi.Models.UserModels;

namespace SchoolManagementApi.Data
{
  public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
  {
    public DbSet<Organization> Organizations { get; set; }
    public DbSet<Zone> Zones { get; set; }
    public DbSet<School> Schools { get; set; }
    public DbSet<Department> Departments { get; set; }
    public DbSet<StudentClass> StudentClasses { get; set; }
    public DbSet<ClassArms> ClassArms { get; set; }
    public DbSet<Subject> Subjects { get; set; }
    public DbSet<TeachingStaff> TeachingStaffs { get; set; }
    public DbSet<NonTeachingStaff> NonTeachingStaffs { get; set; }
    public DbSet<Parent> Parents { get; set; }
    public DbSet<Student> Students { get; set; }
    public DbSet<DocumentFile> DocumentFiles { get; set; }
    public DbSet<StudentsCARecord> StudentsCARecords { get; set; }
    public DbSet<SchoolSession> SchoolSessions { get; set; }
    public DbSet<SchoolTerm> SchoolTerms { get; set; }
    public DbSet<StudentsScores> StudentsScores { get; set; }
    public DbSet<ClassAttendance> ClassAttendances { get; set; }
    public DbSet<LessonNote> LessonNotes { get; set; }
    public DbSet<LessonPeriod> LessonPeriods { get; set; }
    public DbSet<LessonPeriodTemplate> LessonPeriodTemplates { get; set; }
    public DbSet<LessonNoteTemplate> LessonNoteTemplates { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      base.OnModelCreating(modelBuilder);

      modelBuilder.Entity<IdentityUserRole<string>>().HasKey(ur => new { ur.UserId, ur.RoleId });

      modelBuilder.Entity<Organization>()
        .HasIndex(o => o.OrganizationUniqueId)
        .IsUnique();
      
      modelBuilder.Entity<School>()
        .HasIndex(s => s.SchoolUniqueId)
        .IsUnique();

      modelBuilder.Entity<NonTeachingStaff>()
        .HasOne(s => s.CurrentPostingZone)
        .WithMany()
        .OnDelete(DeleteBehavior.NoAction);

      modelBuilder.Entity<TeachingStaff>()
        .HasOne(s => s.CurrentPostingZone)
        .WithMany()
        .OnDelete(DeleteBehavior.NoAction);
      
      modelBuilder.Entity<Student>()
        .HasOne(s => s.Parent)
        .WithMany()
        .OnDelete(DeleteBehavior.NoAction);

      modelBuilder.Entity<Student>()
        .HasOne(s => s.StudentClass)
        .WithMany()
        .OnDelete(DeleteBehavior.NoAction);

      modelBuilder.Entity<Student>()
        .HasOne(s => s.Department)
        .WithMany()
        .OnDelete(DeleteBehavior.NoAction);

      modelBuilder.Entity<Student>()
        .HasOne(s => s.CurrentSchool)
        .WithMany()
        .OnDelete(DeleteBehavior.NoAction);

      modelBuilder.Entity<Student>()
        .HasOne(s => s.SchoolZone)
        .WithMany()
        .OnDelete(DeleteBehavior.NoAction);

      modelBuilder.Entity<StudentClass>()
        .HasMany(sc => sc.ClassArms)
        .WithOne()
        .HasForeignKey(ca => ca.StudentClassId)
        .OnDelete(DeleteBehavior.Cascade);

      modelBuilder.Entity<School>()
        .HasOne(s => s.Zone)
        .WithMany(z => z.Schools)
        .OnDelete(DeleteBehavior.NoAction);

      modelBuilder.Entity<SchoolTerm>()
        .HasOne(d => d.SchoolSession)
        .WithMany(s => s.SchoolTerms)
        .OnDelete(DeleteBehavior.NoAction);

      modelBuilder.Entity<StudentsScores>()
        .HasOne(d => d.StudentsCARecord)
        .WithMany(s => s.StudentsScores)
        .OnDelete(DeleteBehavior.NoAction);

      // modelBuilder.Entity<SchoolSession>()
      //   .HasIndex(e => e.Name)
      //   .IsUnique();

      // modelBuilder.Entity<SchoolTerm>()
      //   .HasIndex(e => e.Name)
      //   .IsUnique();

      modelBuilder.Entity<LessonPeriod>()
        .HasOne(d => d.LessonNotes)
        .WithMany(s => s.Periods)
        .OnDelete(DeleteBehavior.NoAction);

      // modelBuilder.Entity<LessonNote>()
      //   .Property(b => b.CustomFields)
      //   .HasColumnType("jsonb");

      // modelBuilder.Entity<ClassArms>()
      //   .HasOne(c => c.StudentClass)
      //   .WithMany(s => s.ClassArms)
      //   .OnDelete(DeleteBehavior.NoAction);
        
    }
  }
}