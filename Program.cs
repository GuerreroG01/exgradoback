using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Serilog;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using dotenv.net;
using ExGradoBack.Data;
using ExGradoBack.Repositories;
using ExGradoBack.Services;
using ExGradoBack.Hubs;
using Hangfire;
using Hangfire.MySql;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using OfficeOpenXml;
using MySqlConnector;
using ExGradoBack.Filters;

namespace ExGradoBack
{
    public class Program
    {
        public static void Main(string[] args)
        {
            ExcelPackage.License.SetNonCommercialPersonal("ExGradoApp"); 
            var builder = WebApplication.CreateBuilder(args);
            builder.WebHost.ConfigureKestrel(options =>
            {
                options.ListenAnyIP(5140);
            });
            DotEnv.Load();

            var dbHost = Environment.GetEnvironmentVariable("DB_HOST") ?? "undefined";
            var dbName = Environment.GetEnvironmentVariable("DB_NAME") ?? "undefined";
            var dbUser = Environment.GetEnvironmentVariable("DB_USER") ?? "undefined";
            var dbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD") ?? "undefined";

            var smtpHost = Environment.GetEnvironmentVariable("SMTP_HOST");
            var smtpPort = int.Parse(Environment.GetEnvironmentVariable("SMTP_PORT") ?? "25");
            var smtpFrom = Environment.GetEnvironmentVariable("SMTP_FROM");
            var smtpUser = Environment.GetEnvironmentVariable("SMTP_USER");
            var smtpPass = Environment.GetEnvironmentVariable("SMTP_PASS");
            
            var connectionString = $"Server={dbHost};Database={dbName};User Id={dbUser};Password={dbPassword};Allow User Variables=true;";
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 41)),
                    mySqlOptions =>
                    {
                        mySqlOptions.EnableRetryOnFailure();
                        // Forzar minúsculas
                        mySqlOptions.MigrationsHistoryTable("__efmigrationshistory");
                    }));

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddControllers();

            builder.Services.AddHangfire(config => 
                config.UseStorage(new MySqlStorage(connectionString, new MySqlStorageOptions())));
            builder.Services.AddHangfireServer();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowLocalNetwork", policy =>
                {
                    policy.SetIsOriginAllowed(origin =>
                    {
                        return origin.StartsWith("http://192.168.") ||
                            origin.StartsWith("http://10.") ||
                            origin.StartsWith("http://172.") ||
                            origin.StartsWith("http://localhost");
                    })
                    .AllowAnyHeader()
                    .AllowCredentials()
                    .AllowAnyMethod()
                    .WithExposedHeaders("Content-Disposition");
                });
            });
            /*builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy
                        .AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .WithExposedHeaders("Content-Disposition"); // <-- Para que el navegador lea la información del encabezado Content-Disposition en una respuesta
                });
            });*/
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IAuthRepository, AuthRepository>();
            builder.Services.AddScoped<IRolService, RolService>();
            builder.Services.AddScoped<IRolRepository, RolRepository>();
            builder.Services.AddScoped<IBackupService, BackupService>();
            builder.Services.AddScoped<IBackupRepository, BackupRepository>();
            builder.Services.AddScoped<IProfileRepository, ProfileRepository>();
            builder.Services.AddScoped<IProfileService, ProfileService>();
            builder.Services.AddScoped<IVehiculoInfoRepository, VehiculoInfoRepository>();
            builder.Services.AddScoped<IVehiculoInfoService, VehiculoInfoService>();
            builder.Services.AddScoped<IMarcaRepuestoRepository, MarcaRepuestoRepository>();
            builder.Services.AddScoped<IMarcaRepuestoService, MarcaRepuestoService>();
            builder.Services.AddScoped<IRepuestoRepository, RepuestoRepository>();
            builder.Services.AddScoped<IRepuestoService, RepuestoService>();
            builder.Services.AddScoped<IProveedorRepository, ProveedorRepository>();
            builder.Services.AddScoped<IProveedorService, ProveedorService>();
            builder.Services.AddScoped<IFacturaRepository, FacturaRepository>();
            builder.Services.AddScoped<IFacturaService, FacturaService>();
            builder.Services.AddScoped<IDetalleFacturaRepository, DetalleFacturaRepository>();
            builder.Services.AddScoped<IDetalleFacturaService, DetalleFacturaService>();
            builder.Services.AddScoped<IOrdenCompraRepository, OrdenCompraRepository>();
            builder.Services.AddScoped<IOrdenCompraService, OrdenCompraService>();
            builder.Services.AddScoped<IDetalleOrdenRepository, DetalleOrdenRepository>();
            builder.Services.AddScoped<IDetalleOrdenService, DetalleOrdenService>();
            builder.Services.AddScoped<IDetalleOrdenService, DetalleOrdenService>();
            builder.Services.AddScoped<IActividadService, ActividadService>();
            builder.Services.AddScoped<IActividadRepository, ActividadRepository>();
            builder.Services.AddScoped<IReportService, ReportService>();
            builder.Services.AddScoped<IExportExcellService, ExportExcellService>();
            builder.Services.AddScoped<IEmailService>(sp =>
            new EmailService(
                Environment.GetEnvironmentVariable("SMTP_HOST") ?? "smtp.gmail.com",
                int.Parse(Environment.GetEnvironmentVariable("SMTP_PORT") ?? "587"),
                Environment.GetEnvironmentVariable("SMTP_FROM") ?? "undefined",
                Environment.GetEnvironmentVariable("SMTP_USER") ?? "undefined",
                Environment.GetEnvironmentVariable("SMTP_PASS") ?? "",
                sp.GetRequiredService<ILogger<EmailService>>(),
                sp.GetRequiredService<IRepuestoService>()
            ));

            Log.Logger = new LoggerConfiguration()
                //.MinimumLevel.Debug()
                //.Enrich.FromLogContext()
                .WriteTo.File("Logs/myapp.log", rollingInterval: RollingInterval.Day)
                .WriteTo.Console()
                .CreateLogger();

            builder.Host.UseSerilog();
            //builder.Logging.ClearProviders();

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    var jwtSecret = Environment.GetEnvironmentVariable("JsonWebTokenSecret") ?? "";
                    var key = Encoding.UTF8.GetBytes(jwtSecret);

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = true,
                        ValidIssuer = "ExGradoSystem.com",
                        ValidateAudience = true,
                        ValidAudience = "ExGradoSystem.com",
                        ValidateLifetime = true,
                        NameClaimType = ClaimTypes.NameIdentifier,
                    };
                });

            builder.Services.AddAuthorization();
            builder.Services.AddSignalR();
            var app = builder.Build();

            // Aplica migraciones pendientes al iniciar la aplicación
            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                try
                {
                    db.Database.Migrate(); // aplica solo lo que falte
                }
                catch (MySqlException ex) when (ex.Message.Contains("already exists"))
                {
                    Console.WriteLine("⚠️ Tabla ya existente. Se omite la creación duplicada.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error aplicando migraciones: {ex.Message}");
                }
            }

            app.MapHub<StockHub>("/stockHub");
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
                app.UseDeveloperExceptionPage();
            }
            
            app.UseHttpsRedirection();
            app.UseCors("AllowLocalNetwork");
            app.UseAuthentication();
            app.UseAuthorization();
            if (app.Environment.IsDevelopment())
            {
                app.UseHangfireDashboard("/hangfire", new DashboardOptions
                {
                    Authorization = new[] { new AutorizacionPanelHangfire() }
                });
            }
            else
            {
                app.UseHangfireDashboard("/hangfire");
            }

            var wwwrootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            if (!Directory.Exists(wwwrootPath))
            {
                Directory.CreateDirectory(wwwrootPath);
            }
            app.UseStaticFiles(); //Servir archivos estáticos en wwwroot
            ExGradoBack.Jobs.HangfireJobsConfig.ConfigurateJobs();
            app.MapControllers();

            var summaries = new[]
            {
                "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
            };

            app.MapGet("/weatherforecast", () =>
            {
                var forecast = Enumerable.Range(1, 5).Select(index =>
                    new WeatherForecast
                    (
                        DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                        Random.Shared.Next(-20, 55),
                        summaries[Random.Shared.Next(summaries.Length)]
                    ))
                    .ToArray();
                return forecast;
            })
            .WithName("GetWeatherForecast");

            app.Run();
        }

        record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
        {
            public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
        }
    }
}