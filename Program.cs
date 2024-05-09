using System.Text;
using CloudinaryDotNet;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using SchoolManagementApi.Configurations;
using SchoolManagementApi.Constants;
using SchoolManagementApi.Data;
using SchoolManagementApi.Intefaces.Admin;
using SchoolManagementApi.Intefaces.LoggerManager;
using SchoolManagementApi.Intefaces.Profiles;
using SchoolManagementApi.Intefaces.Roles;
using SchoolManagementApi.Intefaces.Uploads;
using SchoolManagementApi.Models.UserModels;
using SchoolManagementApi.Services.Admin;
using SchoolManagementApi.Services.LoggerManager;
using SchoolManagementApi.Services.Profiles;
using SchoolManagementApi.Services.RoleServices;
using SchoolManagementApi.Services.Uploads;
using Swashbuckle.AspNetCore.Filters;
using WatchDog;
using WatchDog.src.Enums;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddControllers();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

var connectionString = builder.Configuration.GetConnectionString("PostgresDatabase");
builder.Services.AddDbContext<ApplicationDbContext>(options => 
    //options.UseSqlServer(builder.Configuration.GetConnectionString("MyDatabaseConnection")));
    options.UseNpgsql(connectionString));

builder.Services.AddHangfire(x =>
    x.UsePostgreSqlStorage(connectionString));

builder.Services.AddHangfireServer();



IConfigurationSection cloudinaryConfig = builder.Configuration.GetSection("Cloudinary");
Account cloudinaryAccount = new(
    cloudinaryConfig["CloudName"],
    cloudinaryConfig["ApiKey"],
    cloudinaryConfig["ApiSecret"]
);
builder.Services.AddSingleton(new Cloudinary(cloudinaryAccount));

builder.Services.AddTransient<ApplicationDbContext>();
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddHttpContextAccessor();
    
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT aUTHORIZATION HEADER USING TOKEN",
        In = ParameterLocation.Header,
        BearerFormat = "JWT",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
    options.OperationFilter<SecurityRequirementsOperationFilter>();
});

builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequiredLength = 6;
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.SignIn.RequireConfirmedEmail = false;
});

builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<ILoggerManager, LoggerManager>();
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<IOrganizationService, OrganizationService>();
builder.Services.AddScoped<IZoneService, ZoneService>();
builder.Services.AddScoped<IDepartmentServices, DepartmentServices>();
builder.Services.AddScoped<ISchoolServices, SchoolServices>();
builder.Services.AddScoped<IStudentClassServices, StudentClassServices>();
builder.Services.AddScoped<ITeachingStaffInterface, TeachingStaffService>();
builder.Services.AddScoped<INonTeachingStaffInterface, NonTeachingStaffService>();
builder.Services.AddScoped<IFileService, FileService>();
builder.Services.AddScoped<ISubjectService, SubjectService>();
builder.Services.AddScoped<IParentService, ParentService>();
builder.Services.AddScoped<IStudentService, StudentService>();

builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAnyOrigin",
            builder =>
            {
                builder.AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod();
            }
        );
    });

// add authentication and jwtbearer
builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.SaveToken = true;
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
            ValidAudience = builder.Configuration["JWT:ValidAudience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"]!))
        };
    });

    builder.Services.AddAuthorization(AuthPolicy.ConfigureAuthorization);

    // add watch dog
    builder.Services.AddWatchDogServices(opt => {
        opt.IsAutoClear = false;
        opt.ClearTimeSchedule = WatchDogAutoClearScheduleEnum.Monthly;
        opt.SetExternalDbConnString = builder.Configuration.GetConnectionString(name:"MyDatabaseConnection");
        opt.DbDriverOption = WatchDogDbDriverEnum.MSSQL;
    });
    
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAnyOrigin");
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.UseHangfireDashboard("/dashboard");
//app.UseHangfireServer();

app.UseWatchDogExceptionLogger();
app.UseWatchDog(opt => {
    opt.WatchPageUsername = builder.Configuration["WatchDogParams:Username"];
    opt.WatchPagePassword = builder.Configuration["WatchDogParams:Password"];
});

using (var scope = app.Services.CreateScope())
{
    var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
    await SeedRoles.SeedRolesAndAdminAsync(scope.ServiceProvider, configuration);
}

app.Run();
