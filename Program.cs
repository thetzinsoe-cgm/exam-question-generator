using WkHtmlToPdfDotNet;
using WkHtmlToPdfDotNet.Contracts;
using log4net;
using log4net.Config;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ExamSystem.Constraints;
using ExamSystem.DAO.Base;
using ExamSystem.DAO.UnitOfWork;
using ExamSystem.DAO.AdminUser;
using ExamSystem.DAO.Auth;
using ExamSystem.DAO.Dashboard;
using ExamSystem.DAO.Grade;
using ExamSystem.DAO.Subject;
using ExamSystem.DAO.Question;
using ExamSystem.DAO.Answer;
using ExamSystem.DAO.MarkingRule;
using ExamSystem.DAO.Exam;
using ExamSystem.DAO.ExamQuestion;
using ExamSystem.DAO.Token;
using ExamSystem.DTOs.Common;
using ExamSystem.Entity;
using ExamSystem.Middlewares;
using ExamSystem.Services;
using ExamSystem.Services.Auth;
using ExamSystem.Services.Dashboard;
using ExamSystem.Services.AdminUser;
using ExamSystem.Services.Grade;
using ExamSystem.Services.Subject;
using ExamSystem.Services.Question;
using ExamSystem.Services.MarkingRule;
using ExamSystem.Services.Exam;
using ExamSystem.Services.Token;
using ExamSystem.Services.PdfRender;
using ExamSystem.Utilities;
using System.Reflection;
using System.Text;

namespace ExamSystem
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var logsPath = Path.Combine(Directory.GetCurrentDirectory(), "Logs");
            GlobalContext.Properties["LogPath"] = logsPath;
            Environment.SetEnvironmentVariable("LogPath", logsPath);

            var mvcLogsPath = Path.Combine(logsPath, "MVC-log-file");
            try
            {
                if (!Directory.Exists(logsPath)) Directory.CreateDirectory(logsPath);
                if (!Directory.Exists(mvcLogsPath)) Directory.CreateDirectory(mvcLogsPath);
                Console.WriteLine($"Ensured Logs folders exist at: {mvcLogsPath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to create Logs folders: {ex.Message}");
            }

            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            var configFile = new FileInfo(Path.Combine(Directory.GetCurrentDirectory(), "log4net.config"));
            try
            {
                XmlConfigurator.Configure(logRepository, configFile);
                Console.WriteLine("Log4net configuration loaded successfully");
                var testLogger = LogManager.GetLogger(typeof(Program));
                testLogger.Info("Application starting - Log4net initialized successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to load log4net configuration: {ex.Message}");
            }

            var builder = WebApplication.CreateBuilder(args);
            Consts.Configure(builder.Configuration);
            DatabaseHelper.Initialize(builder.Configuration);

            builder.Services.AddSingleton<ILog>(_ =>
                LogManager.GetLogger(typeof(Program))
            );

            builder.Services.AddControllers().AddNewtonsoftJson();

            DatabaseHelper.Initialize(builder.Configuration);
            builder.Services.AddDbContext<exam_system_entities>(options =>
            {
                var provider = DatabaseHelper.GetProvider();
                var connectionString = DatabaseHelper.ConnectionString();
                if (provider == "MariaDb")
                {
                    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
                }
                else if (provider == "SqlServer")
                {
                    options.UseSqlServer(connectionString);
                }
                else
                {
                    throw new Exception("Unsupported database provider");
                }
            });

            builder.Services.AddControllersWithViews();
            builder.Services.AddControllers();

            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(60);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            builder.Services.AddHttpContextAccessor();

            builder.Services.AddScoped<SessionService>();
            builder.Services.AddScoped<BaseDao>();

            builder.Services.AddScoped<IAuthManager, AuthManager>();
            builder.Services.AddScoped<IAuthDao, AuthDao>();

            builder.Services.AddScoped<IDashboardService, DashboardService>();
            builder.Services.AddScoped<IDashboardDao, DashboardDao>();

            builder.Services.AddScoped<IAdminUserService, AdminUserService>();
            builder.Services.AddScoped<IAdminUserDao, AdminUserDao>();

            builder.Services.AddScoped<IGradeService, GradeService>();
            builder.Services.AddScoped<IGradeDao, GradeDao>();

            builder.Services.AddScoped<ISubjectService, SubjectService>();
            builder.Services.AddScoped<ISubjectDao, SubjectDao>();

            builder.Services.AddScoped<IQuestionService, QuestionService>();
            builder.Services.AddScoped<IQuestionDao, QuestionDao>();
            builder.Services.AddScoped<IAnswerDao, AnswerDao>();

            builder.Services.AddScoped<IMarkingRuleService, MarkingRuleService>();
            builder.Services.AddScoped<IMarkingRuleDao, MarkingRuleDao>();

            builder.Services.AddScoped<IExamService, ExamService>();
            builder.Services.AddScoped<IExamDao, ExamDao>();
            builder.Services.AddScoped<IExamQuestionDao, ExamQuestionDao>();

            builder.Services.AddScoped<ITokenService, TokenService>();
            builder.Services.AddScoped<ITokenDao, TokenDao>();

            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<IViewRenderService, ViewRenderService>();

            builder.Services.AddSingleton<IConverter, SynchronizedConverter>(provider =>
                new SynchronizedConverter(new PdfTools()));

            builder.Services.AddScoped<FilePathHelper>();
            builder.Services.AddScoped<PaginationHelper>();
            builder.Services.AddScoped<DropDownHelper>();

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
            {
                options.LoginPath = "/admin/login";
                options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
                options.SlidingExpiration = true;
                options.AccessDeniedPath = "/errors/403";
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                    ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
                    ValidAudience = builder.Configuration["JwtSettings:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Key"]))
                };
            });

            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("WebPolicy", policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.AuthenticationSchemes.Add(CookieAuthenticationDefaults.AuthenticationScheme);
                });
                options.AddPolicy("ApiPolicy", policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
                });
            });

            builder.Services.Configure<FormOptions>(options =>
            {
                options.ValueCountLimit = 2048;
                options.MultipartBodyLengthLimit = 52428800;
            });

            builder.Services.AddControllersWithViews()
                .AddViewLocalization()
                .AddDataAnnotationsLocalization();

            var app = builder.Build();

            var httpContextAccessor = app.Services.GetRequiredService<IHttpContextAccessor>();
            HttpContextHelper.Configure(httpContextAccessor);
            AuthUser.Configure(httpContextAccessor);
            Auth.Configure(httpContextAccessor);
            DocumentExportHelper.Initialize(builder.Environment);

            app.UseStatusCodePagesWithReExecute("/errors/{0}");

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto,
                KnownNetworks = { },
                KnownProxies = { }
            });

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseSession();

            app.UseMiddleware<PreventBackHistoryMiddleware>();
            app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

            app.MapControllerRoute(
                name: "admin",
                pattern: "{controller=Dashboard}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
