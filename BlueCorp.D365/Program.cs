using BlueCorp.Common;
using BlueCorp.D365.Client;
using BlueCorp.D365.Contract;
using BlueCorp.D365.DataRepository;
using BlueCorp.D365.Mapper;
using BlueCorp.D365.Service;
using System.Text.Json.Serialization;

namespace BlueCorp.D365
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

            builder.Services.AddAutoMapper(typeof(AutomapperConfig));

            // Add services to the container.
            builder.Services.AddAuthorization();

            //http client
            builder.Services.AddHttpClient("ThirdPL", client =>
            {
                client.BaseAddress = new Uri(Configuration["BlueCorp_ThirdPL_BaseUrl"]);
                var apiKey = KeyVaults.APIKeys["D365-3PL"];
                client.DefaultRequestHeaders.Add("api-key", apiKey);
                client.Timeout = TimeSpan.FromMinutes(5);
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.UseRouting().UseCors().UseAuthorization().UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.Run();
        }

        private static ServiceProvider ConfigIoC(IServiceCollection services)
        {
            services.AddScoped<IDispatchClient, DispatchClient>();
            services.AddScoped<IDispatchService, DispatchService>();
            services.AddScoped<IPayLoadRepo, PayLoadMemoryRepo>();

            return services.BuildServiceProvider();
        }
    }
}
