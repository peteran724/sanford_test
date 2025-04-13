using BlueCorp.ThirdPL.Contract;
using BlueCorp.ThirdPL.Job;
using BlueCorp.ThirdPL.Service;
using Hangfire;
using Hangfire.MemoryStorage;
using System.Text.Json.Serialization;

namespace BlueCorp.ThirdPL
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(
                    builder =>
                    {
                        builder.SetIsOriginAllowed(_ => true);
                        builder.AllowAnyMethod();
                        builder.AllowAnyHeader();
                        builder.AllowCredentials();
                    });
            });

            builder.Services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            })
            ;

            var Configuration = builder.Configuration;

            ConfigIoC(builder.Services);

            // Add services to the container.
            builder.Services.AddAuthorization();

            //Hangfire
            builder.Services.AddHangfire(config =>config.UseMemoryStorage());
            builder.Services.AddHangfireServer();
            builder.Services.AddScoped<FileHandlerJob>();

            var app = builder.Build();

            //check folder
            string rootPath = app.Environment.ContentRootPath;
            var dirs = new List<string> { "bluecorp-incoming", "bluecorp-processed", "bluecorp-failed" };
            dirs.ForEach(dir =>
            {
                string path = Path.Combine(rootPath, dir);
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
            });

            // Configure the HTTP request pipeline.

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.UseRouting().UseCors().UseAuthorization().UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            using (var scope = app.Services.CreateScope())
            {
                var jobService = scope.ServiceProvider.GetRequiredService<FileHandlerJob>();
                jobService.ScheduleJobs();
            }

            app.Run();
        }

        private static ServiceProvider ConfigIoC(IServiceCollection services)
        {
            services.AddScoped<IFileHandlerService, FileHandlerService>();
            return services.BuildServiceProvider();
        }
    }
}
