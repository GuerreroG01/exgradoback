using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Serilog;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using dotenv.net;
using ExGradoBack.Data;
using ExGradoBack.Repositories;
using ExGradoBack.Services;
using Hangfire;
using Hangfire.MySql;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ExGradoBack
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            DotEnv.Load();

            var dbHost = Environment.GetEnvironmentVariable("DB_HOST") ?? "undefined";
            var dbName = Environment.GetEnvironmentVariable("DB_NAME") ?? "undefined";
            var dbUser = Environment.GetEnvironmentVariable("DB_USER") ?? "undefined";
            var dbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD") ?? "undefined";
            
            var connectionString = $"Server={dbHost};Database={dbName};User Id={dbUser};Password={dbPassword};Allow User Variables=true;";
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 41)),
                mySqlOptions => mySqlOptions.EnableRetryOnFailure()));

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

            Log.Logger = new LoggerConfiguration()
                .WriteTo.File("Logs/myapp.log", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            builder.Logging.AddSerilog();
            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
                app.UseDeveloperExceptionPage();
            }
            
            app.UseHttpsRedirection();
            app.UseCors("AllowLocalNetwork");
            app.UseAuthorization();
            app.UseHangfireDashboard();
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